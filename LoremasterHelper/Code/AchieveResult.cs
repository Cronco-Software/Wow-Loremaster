using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LoremasterHelper.Models;

namespace LoremasterHelper
{
    public class AchieveResult
    {
        public int Id { get; set; }
        public int? LinkedAchieveId { get; set; }
        public string Name { get; set; }
        public long? CompletedTimestamp { get; set; }
        public BNetCharacter Recomended { get; set; }
        public Faction Faction { get; set; }
        public int QuestsCompleted { get; set; }
        public int TotalQuests { get; set; }
        public int PercentComplete { get; set; }
        public string ZoneString { get; set; }
        public string Icon { get; set; }
        public string GuideLink { get; set; }
        
        
    }
}