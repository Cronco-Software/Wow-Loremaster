using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LoremasterHelper.Models;

namespace LoremasterHelper
{
    public class CharacterRecomentationResult
    {
        public BNetCharacter Toon { get; set; }
        public List<AchieveResult> AchievesInProgress { get; set; }

        public CharacterRecomentationResult()
        {
            this.AchievesInProgress = new List<AchieveResult>();
        }
    }
}