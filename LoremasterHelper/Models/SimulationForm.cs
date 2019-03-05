using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoremasterHelper.Models
{
    public class SimulationForm: Layout
    {
        public bool IsAuthenticated { get; set; }
        public string BattleTag { get; set; }
        public List< CharacterDefinition> Toons { get; set; }
    }
}