using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoremasterHelper
{
    public class Loremaster
    {
        public int AchievementId { get; set; }
        public string Icon { get; set; }
        public string Name { get; set; }
        public string Guide { get; set; }
        public Meta[] Criteria { get; set; }
        public int[] AllAchievementIds { get; set; }
        
    }
}