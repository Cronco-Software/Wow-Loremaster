using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Elmah;

namespace LoremasterHelper.Models
{
    
    public static class BNetContext
    {
        private static readonly object _credentialLock = new object();
        private static Dictionary<string, ClientCredential> _regionClientCredentials;
        static BNetContext()
        {
            _regionClientCredentials = new Dictionary<string, ClientCredential>();
            foreach (var region in Config.SupportedRegions)
            {
                _regionClientCredentials.Add(region, new ClientCredential() { Token = string.Empty, Expires = DateTime.Now.AddSeconds(-1) });
            }
        }

        private static string GetClientCredentials(string region)
        {
            string token = string.Empty;
            if (_regionClientCredentials.ContainsKey(region))
            {
                lock (_credentialLock)
                {
                    var cred = _regionClientCredentials[region];

                    if (cred.Expires < DateTime.Now)
                    {
                        string apiUrl = string.Format("{0}/oauth/token?grant_type=client_credentials&client_id={1}&client_secret={2}",
                            Config.OAuthBaseUrl(region), Config.AuthApiKey, Config.AuthApiSecret);

                        var respJson = GetApiResponse(apiUrl);
                        if (!string.IsNullOrWhiteSpace(respJson))
                        {
                            try
                            {
                                JObject credObject = JObject.Parse(respJson);
                                cred.Token = (string)credObject["access_token"];
                                cred.Expires = DateTime.Now.AddSeconds((int)credObject["expires_in"] - Config.ClientCrediantialExpiryPadding);
                            }
                            catch (Exception ex)
                            {
                                cred.Token = string.Empty;
                                cred.Expires = DateTime.Now.AddSeconds(-1);
                                var context = System.Web.HttpContext.Current;
                                ErrorLog.GetDefault(context).Log(new Error(ex, context));
                            }
                        }
                    }
                    token = cred.Token;
                }
            }
            return token;
        }



        public static BNetUser GetUser(string region, string token)
        {
            BNetUser user = null;

            string apiUrl = Config.OAuthBaseUrl(region) + "/oauth/userinfo?access_token=" + token;
            string apiResponse = GetApiResponse(apiUrl);

            if (!string.IsNullOrWhiteSpace(apiResponse))
            {
                dynamic userObject = JObject.Parse(apiResponse);
                user = new BNetUser();
                user.BattleTag = userObject.battletag;
                user.Id = userObject.id.ToString();
            }
            return user;
        }

        public static IEnumerable<BNetCharacter> GetAccountCharacters(string region, string token)
        {
            List<BNetCharacter> chars = new List<BNetCharacter>();

            string apiUrl = Config.CommunityApiBaseUrl(region) + "/wow/user/characters?access_token=" + token;
            string apiResponse = GetApiResponse(apiUrl);

            if (!string.IsNullOrWhiteSpace(apiResponse))
            {
                JObject charObject = JObject.Parse(apiResponse);

                chars.AddRange(from co in charObject["characters"]
                               select new BNetCharacter
                               {
                                   Class = GameData.GetClassNameForId((int)co["class"]),
                                   Race = GameData.GetRaceNameForId((int)co["race"]),
                                   Level = (int)co["level"],
                                   Name = (string)co["name"],
                                   Realm = (string)co["realm"],
                                   Faction = GameData.GetFactionForRaceId((int)co["race"]),
                                   ThumbnailPath = (string)co["thumbnail"],
                                   Region = region
                               });

            }

            return chars;
        }


        public static BNetCharacterDetails GetCharacterDetails(string region, string server, string name)
        {
            BNetCharacterDetails dets = null;

            string token = GetClientCredentials(region);

            string apiUrl = string.Format("{0}/wow/character/{1}/{2}?fields=achievements%2Cquests&access_token={3}",
                Config.DataApiBaseUrl(region), Uri.EscapeDataString(server), Uri.EscapeDataString(name), token);

            string apiResponse = GetApiResponse(apiUrl);
            if (!string.IsNullOrWhiteSpace(apiResponse))
            {
                try
                {
                    JObject detObject = JObject.Parse(apiResponse);
                    dets = new BNetCharacterDetails();
                    dets.Class = GameData.GetClassNameForId((int)detObject["class"]);
                    dets.Race = GameData.GetRaceNameForId((int)detObject["race"]);
                    dets.Level = (int)detObject["level"];
                    dets.Name = (string)detObject["name"];
                    dets.Realm = (string)detObject["realm"];
                    dets.Faction = GameData.GetFactionForRaceId((int)detObject["race"]);
                    dets.ThumbnailPath = (string)detObject["thumbnail"];
                    dets.Region = region;

                    int achieveCount = detObject["achievements"]["achievementsCompleted"].Count();
                    List<CharacterAchiev> lmAchievs = new List<CharacterAchiev>();

                    for (int i = 0; i < achieveCount; i++)
                    {
                        int aId = (int)detObject["achievements"]["achievementsCompleted"][i];
                        if (GameData.Loremaster.AllAchievementIds.Contains(aId))
                        {
                            lmAchievs.Add(new CharacterAchiev()
                            {
                                Id = aId,
                                Timestamp = (long)detObject["achievements"]["achievementsCompletedTimestamp"][i]
                            });
                        }
                    }
                    dets.Achievements = lmAchievs.ToArray();

                    Dictionary<int, int> zoneProgress = new Dictionary<int, int>();
                    foreach (int questId in detObject["quests"])
                    {
                        int zoneId;
                        if (GameData.TryGetQuestZone(questId, out zoneId))
                        {
                            if (zoneProgress.ContainsKey(zoneId))
                            {
                                zoneProgress[zoneId]++;
                            }
                            else
                            {
                                zoneProgress.Add(zoneId, 1);
                            }
                        }
                    }
                    dets.ZoneProgress = zoneProgress;
                }
                catch (Exception ex)
                {
                    dets = null;
                    var context = System.Web.HttpContext.Current;
                    ErrorLog.GetDefault(context).Log(new Error(ex, context));
                }
                
            }

            return dets;
        }

        private static string GetApiResponse(string url)
        {
            string resp = null;

            HttpWebRequest apiRequest = (HttpWebRequest)WebRequest.Create(url);
            apiRequest.Method = "GET";

            try
            {
                using (HttpWebResponse apiResp = (HttpWebResponse)apiRequest.GetResponse())
                using (StreamReader reader = new StreamReader(apiResp.GetResponseStream()))
                {
                    resp = reader.ReadToEnd();
                }
            }
            catch (WebException wex)
            {
                var eres = wex.Response as HttpWebResponse;
                if (eres == null || eres.StatusCode != HttpStatusCode.NotFound)
                {
                    throw wex;
                }
            }
            
            return resp;
        }
    }
}