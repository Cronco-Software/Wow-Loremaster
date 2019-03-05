using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoremasterHelper
{
    public class Zone
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AllianceQuestCount { get; set; }
        public int HordeQuestCount { get; set; }
    }
}