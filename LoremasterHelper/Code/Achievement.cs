using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoremasterHelper
{
    public class Achievement
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public Faction Faction { get; set; }
        public Achievement LinkedAchieve { get; set; }
        public Storyline[] Storylines { get; set; }
        public Zone[] Zones { get; set; }
        public string ZoneString { get; set; }
        public string Guide { get; set; }
        public int MinLevel { get; set; }
        public int MaxLevel { get; set; }
    }
}