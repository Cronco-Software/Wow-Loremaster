using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoremasterHelper.Models
{
    public class BNetCharacterDetails: BNetCharacter
    {
        public CharacterAchiev[] Achievements { get; set; }

        public Dictionary<int, int> ZoneProgress { get; set; }

        public int TempAchievementProgress { get; set; }
    }
}