using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using LoremasterHelper.Models;
using System.Net;
using System.IO;

namespace LoremasterHelper.Controllers
{
    public class ServicesController : BaseController
    {
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public string Simulate(string toonJson)
        {
            try
            {

                Response.ContentType = "application/json";
                if (string.IsNullOrWhiteSpace(toonJson))
                {
                    Response.StatusCode = 500;
                    return JsonConvert.SerializeObject(new { ErrorMessage = "No character definitions provided" });
                }

                CharacterDefinition[] toons = JsonConvert.DeserializeObject<CharacterDefinition[]>(toonJson);

                List<BNetCharacterDetails> dets = new List<BNetCharacterDetails>();
                for (int i = 0; i < toons.Length; i++)
                {
                    var charDetails = GetCharacterDetails(toons[i]);
                    if (charDetails != null)
                    {
                        dets.Add(charDetails);
                    }
                }

                if (dets.Count < 1)
                {
                    Response.StatusCode = 404;
                    return JsonConvert.SerializeObject(new { ErrorMessage = "Unable to retreive details for any character provided" });
                }

                LMResult simResult = Simulation.Run(dets);
                return JsonConvert.SerializeObject(new { ErrorMessage = "", Sim = simResult });

            }
            catch (Exception ex)
            {
                LogError(ex);
                Response.StatusCode = 500;
                return JsonConvert.SerializeObject(new { ErrorMessage = "System error occured" });
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public string GetCharacter(CharacterDefinition toon)
        {
            try
            {
                Response.ContentType = "application/json";

                var charDets = GetCharacterDetails(toon);

                if (charDets == null)
                {
                    Response.StatusCode = 404;
                    return JsonConvert.SerializeObject(new { ErrorMessage = "Character not found" });
                }

                return JsonConvert.SerializeObject(new { ErrorMessage = "", Toon = new BNetCharacter()
                {
                    Class = charDets.Class,
                    Faction = charDets.Faction,
                    Level = charDets.Level,
                    Name = charDets.Name,
                    Race = charDets.Race,
                    Realm = charDets.Realm,
                    ThumbnailPath = charDets.ThumbnailPath
                }});
            }
            catch (Exception ex)
            {
                LogError(ex);
                Response.StatusCode = 500;
                return JsonConvert.SerializeObject(new { ErrorMessage = "System error occured" });
            }
        }
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public string GetAccountCharacters()
        {
            try
            {
                Response.ContentType = "application/json";

                if (!User.Identity.IsAuthenticated)
                {
                    Response.StatusCode = 500;
                    return JsonConvert.SerializeObject(new { ErrorMessage = "User is not authenticated" });
                }

                string token = UserToken;
                BNetCharacter[] accountChars = HttpContext.Cache[token + ACCOUNT_CHAR_CACHE_KEY] as BNetCharacter[];
                if (accountChars == null)
                {
                    accountChars = BNetContext.GetAccountCharacters(UserRegion, token).ToArray();

                    if (accountChars == null)
                    {
                        Response.StatusCode = 404;
                        return JsonConvert.SerializeObject(new { ErrorMessage = "Unable to retrieve account characters" });
                    }

                    HttpContext.Cache.Add(
                        token + ACCOUNT_CHAR_CACHE_KEY,
                        accountChars,
                        null,
                        DateTime.Now.AddSeconds(Config.AccountCharacterCacheSeconds),
                        TimeSpan.Zero,
                        System.Web.Caching.CacheItemPriority.BelowNormal,
                        null);
                }

                return JsonConvert.SerializeObject(new { ErrorMessage = "", Toons = accountChars.OrderByDescending(x => x.Level).ThenBy(x => x.Realm) });
            }
            catch (Exception ex)
            {
                LogError(ex);
                Response.StatusCode = 500;
                return JsonConvert.SerializeObject(new { ErrorMessage = "System error occured" });
            }
        }
        

        private BNetCharacterDetails GetCharacterDetails(CharacterDefinition toon)
        {
            string toonKey = toon.Region + toon.Server + toon.Name + CHAR_DETAILS_CACHE_KEY;
            BNetCharacterDetails charDets = HttpContext.Cache[toonKey] as BNetCharacterDetails;
            if (charDets == null)
            {
                charDets = BNetContext.GetCharacterDetails(toon.Region, toon.Server, toon.Name);
                if (charDets != null)
                {
                    HttpContext.Cache.Add(
                        toonKey,
                        charDets,
                        null,
                        DateTime.Now.AddSeconds(Config.CharacterDetailsCacheSeconds),
                        TimeSpan.Zero,
                        System.Web.Caching.CacheItemPriority.BelowNormal,
                        null);
                }
            }
            return charDets;
        }


    }
}