using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoremasterHelper
{
    public class MetaResult
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public long? CompletedTimestamp { get; set; }
        public string Icon { get; set; }
        public int? LinkedMetaId { get; set; }
        public Faction Faction { get; set; }
        public int TotalAchievs { get; set; }
        public int CompletedAchieves { get; set; }
        public int TotalQuests { get; set; }
        public int QuestsCompleted { get; set; }
        public int QuestsCompletedPercent { get; set; }
        public List<AchieveResult> AchievProgress { get; set; }
        public string GuideLink { get; set; }
    }
}