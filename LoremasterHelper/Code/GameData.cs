using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;


namespace LoremasterHelper
{
    public static class GameData
    {
        private static JObject _races;
        private static JObject _classes;
        public static Loremaster Loremaster { get; private set; }
        public static Dictionary<int, int> QuestZones { get; private set; }
        public static void Init()
        {
            string classFile = HttpContext.Current.ApplicationInstance.Server.MapPath("~/App_Data/classes.json");
            _classes = JObject.Parse(File.ReadAllText(classFile));

            string raceFile = HttpContext.Current.ApplicationInstance.Server.MapPath("~/App_Data/races.json");
            _races = JObject.Parse(File.ReadAllText(raceFile));


            BuildLoremasterObject();


            var lmZoneIds = Loremaster.Criteria.SelectMany(x => x.Criteria).SelectMany(x => x.Zones).Select(x => x.Id).Distinct();
            string qzFile = HttpContext.Current.ApplicationInstance.Server.MapPath("~/App_Data/questZones.json");
            JArray qzMap = JArray.Parse(File.ReadAllText(qzFile));
            QuestZones = new Dictionary<int, int>();
            foreach (var zq in qzMap)
            {
                if (lmZoneIds.Contains((int)zq["zoneId"]))
                {
                    QuestZones.Add((int)zq["questId"], (int)zq["zoneId"]);
                }
            }

        }

        private static void BuildLoremasterObject()
        {

            string lmFile = HttpContext.Current.ApplicationInstance.Server.MapPath("~/App_Data/loremaster.xml");
            XmlDocument lmDoc = new XmlDocument();
            lmDoc.Load(lmFile);

            Loremaster = new Loremaster();
            var lmNode = lmDoc.SelectSingleNode("/loremaster");
            Loremaster.AchievementId = int.Parse(lmNode.Attributes["achievementId"].Value);
            Loremaster.Icon = lmNode.Attributes["icon"].Value;
            Loremaster.Name = lmNode.Attributes["achievementName"].Value;
            Loremaster.Guide = lmNode.Attributes["guide"].Value;

            List<Meta> metas = new List<Meta>();
            foreach (XmlNode mNode in lmNode.SelectNodes("meta"))
            {
                Meta m = new Meta();
                m.Name = mNode.Attributes["achievementName"].Value;
                m.AchievementId = int.Parse(mNode.Attributes["achievementId"].Value);
                m.Faction = (Faction)int.Parse(mNode.Attributes["faction"].Value);
                m.Icon = mNode.Attributes["icon"].Value;
                m.Guide = mNode.Attributes["guide"].Value;


                List<Achievement> achs = new List<Achievement>();
                foreach (XmlNode aNode in mNode.SelectNodes("achievement"))
                {
                    Achievement a = new Achievement();
                    a.Name = aNode.Attributes["achievementName"].Value;
                    a.Id = int.Parse(aNode.Attributes["achievementId"].Value);
                    a.Faction = (Faction)int.Parse(aNode.Attributes["faction"].Value);
                    a.Icon = aNode.Attributes["icon"].Value;
                    a.Guide = aNode.Attributes["guide"].Value;
                    a.MinLevel = int.Parse(aNode.Attributes["minLevel"].Value);
                    a.MaxLevel = int.Parse(aNode.Attributes["maxLevel"].Value);

                    List<Storyline> quests = new List<Storyline>();
                    foreach (XmlNode qNode in aNode.SelectNodes("storyline"))
                    {
                        Storyline q = new Storyline();
                        q.Name = qNode.Attributes["name"].Value;
                        q.QuestId = int.Parse(qNode.Attributes["questId"].Value);
                        q.QuestName = qNode.Attributes["questName"].Value;
                        q.Faction = (Faction)int.Parse(qNode.Attributes["faction"].Value);
                        quests.Add(q);
                    }
                    a.Storylines = quests.ToArray();


                    List<Zone> zones = new List<Zone>();
                    foreach (XmlNode zNode in aNode.SelectNodes("zone"))
                    {
                        Zone zone = new Zone();
                        zone.Id = int.Parse(zNode.Attributes["id"].Value);
                        zone.Name = zNode.Attributes["name"].Value;
                        zone.AllianceQuestCount = int.Parse(zNode.Attributes["allianceQuests"].Value);
                        zone.HordeQuestCount = int.Parse(zNode.Attributes["hordeQuests"].Value);
                        zones.Add(zone);
                    }

                    a.ZoneString = string.Join(", ", zones.Select(x => x.Name));

                    a.Zones = zones.ToArray();
                    achs.Add(a);
                }

                m.Criteria = achs.ToArray();
                m.TotalQuests = m.Criteria.Sum(x => x.Storylines.Length);
                metas.Add(m);
            }
            Loremaster.Criteria = metas.ToArray();

            List<int> allAchievs = new List<int>();
            allAchievs.Add(Loremaster.AchievementId);
            allAchievs.AddRange(Loremaster.Criteria.Select(x => x.AchievementId));
            allAchievs.AddRange(Loremaster.Criteria.SelectMany(x => x.Criteria).Select(x => x.Id));
            Loremaster.AllAchievementIds = allAchievs.OrderBy(x => x).ToArray();

            //link faction different metas & achieves
            foreach (Meta m in Loremaster.Criteria)
            {
                if (m.LinkedMeta == null)
                {
                    var linked = Loremaster.Criteria.Where(x => x.Name == m.Name && x.AchievementId != m.AchievementId).SingleOrDefault();
                    if (linked != null)
                    {
                        linked.LinkedMeta = m;
                        m.LinkedMeta = linked;
                    }

                    foreach (Achievement a in m.Criteria)
                    {
                        if (a.LinkedAchieve == null)
                        {
                            var la = m.Criteria.SingleOrDefault(x => x.Name == a.Name && x.Id != a.Id);
                            if (la != null)
                            {
                                a.LinkedAchieve = la;
                                la.LinkedAchieve = a;
                            }
                        }
                    }
                }
            }
        }
        
        public static bool TryGetQuestZone(int questId, out int zoneId)
        {
            return QuestZones.TryGetValue(questId, out zoneId);
        }

        public static string GetClassNameForId(int id)
        {
            var classNode = _classes["classes"].Where(x => (int)x["id"] == id).FirstOrDefault();
            if (classNode != null)
            {
                return (string)classNode["name"];
            }

            return "";
        }

        
        public static string GetRaceNameForId(int id)
        {
            var raceNode = _races["races"].Where(x => (int)x["id"] == id).FirstOrDefault();
            if (raceNode != null)
            {
                return (string)raceNode["name"];
            }
            
            return "";
        }

        public static Faction GetFactionForRaceId(int id)
        {
            var raceNode = _races["races"].Where(x => (int)x["id"] == id).FirstOrDefault();
            if (raceNode != null)
            {
                var side = (string)raceNode["side"];
                if (!string.IsNullOrWhiteSpace(side))
                {
                    switch(side)
                    {
                        case "horde":
                            return Faction.Horde;
                        case "alliance":
                            return Faction.Alliance;
                        default:
                            return Faction.Neutral;

                    }
                }
            }
            return Faction.Neutral;
        }

    }

    public enum Faction
    {
        Alliance = 0,
        Horde = 1,
        Neutral = 2
    }
}