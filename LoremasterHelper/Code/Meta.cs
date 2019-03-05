using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoremasterHelper
{
    public class Meta
    {
        public int AchievementId { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public Faction Faction { get; set; }
        public int TotalQuests { get; set; }
        public Meta LinkedMeta { get; set; }
        public Achievement[] Criteria { get; set; }
        public string Guide { get; set; }
    }
}