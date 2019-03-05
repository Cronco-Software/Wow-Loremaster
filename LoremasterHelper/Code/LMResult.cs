using System; 
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoremasterHelper
{
    public class LMResult
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public long? CompletedTimestamp { get; set; }
        public int TotalMetas { get; set; }
        public int CompletedMetas { get; set; }
        public string Icon { get; set; }
        public int TotalQuests { get; set; }
        public int QuestsCompleted { get; set; }
        public int QuestCompletedPercent { get; set; }
        public string GuideLink { get; set; }

        public IEnumerable<MetaResult> MetaProgress { get; set; }
        public IEnumerable<CharacterRecomentationResult> CharacterSummary { get; set; }
    }
}