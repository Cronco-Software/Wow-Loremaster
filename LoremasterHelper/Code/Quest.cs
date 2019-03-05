using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoremasterHelper
{
    public class Storyline
    {
        public int QuestId { get; set; }
        public string Name { get; set; }
        public string QuestName { get; set; }
        public Faction Faction { get; set; }
    }
}