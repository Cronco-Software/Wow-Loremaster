using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LoremasterHelper.Models;
using System.Web.Mvc;

namespace LoremasterHelper
{
    public class Simulation
    {
        public static LMResult Run(IEnumerable<BNetCharacterDetails> toons)
        {
            var linkBuilder = new UrlHelper(HttpContext.Current.Request.RequestContext);

            LMResult lm = new LMResult();
            lm.Id = GameData.Loremaster.AchievementId;
            lm.Name = GameData.Loremaster.Name;
            lm.Icon = GameData.Loremaster.Icon;
            lm.GuideLink = linkBuilder.Action("Index", "Guides", new { guide = GameData.Loremaster.Guide });

            var main = toons.OrderByDescending(x => x.Achievements.Length).First();


            List<MetaResult> mResults = new List<MetaResult>();
            foreach (Meta m in GameData.Loremaster.Criteria)
            {
                MetaResult mResult = new MetaResult();
                mResult.Id = m.AchievementId;
                mResult.Faction = m.Faction;
                mResult.Icon = m.Icon;
                mResult.Name = m.Name;

                if (!string.IsNullOrWhiteSpace(m.Guide))
                {
                    mResult.GuideLink = linkBuilder.Action("Index", "Guides", new { guide = m.Guide });
                }

                if (m.LinkedMeta != null)
                {
                    mResult.LinkedMetaId = m.LinkedMeta.AchievementId;
                }

                List<AchieveResult> achResults = new List<AchieveResult>();
                foreach (Achievement a in m.Criteria)
                {
                    AchieveResult aResult = new AchieveResult();
                    aResult.Faction = a.Faction;
                    aResult.Id = a.Id;
                    aResult.ZoneString = a.ZoneString;
                    aResult.Icon = a.Icon;

                    if (!string.IsNullOrWhiteSpace(a.Guide))
                    {
                        aResult.GuideLink = linkBuilder.Action("Index", "Guides", new { guide = a.Guide });
                    }

                    if (a.LinkedAchieve != null)
                    {
                        aResult.LinkedAchieveId = a.LinkedAchieve.Id;
                    }

                    aResult.Name = a.Name;
                    var aAch = main.Achievements.FirstOrDefault(x => x.Id == a.Id);
                    if (aAch != null)
                    {
                        aResult.CompletedTimestamp = aAch.Timestamp;
                        aResult.TotalQuests = Math.Max(a.Zones.Sum(x => x.AllianceQuestCount), a.Zones.Sum(x => x.HordeQuestCount));
                        aResult.QuestsCompleted = aResult.TotalQuests;
                        aResult.PercentComplete = 100;
                    }
                    else
                    {
                        IEnumerable<BNetCharacterDetails> availableToons;
                        if (a.Faction == Faction.Neutral)
                        {
                            availableToons = toons;
                        }
                        else
                        {
                            availableToons = toons.Where(x => x.Faction == a.Faction);
                        }

                        var recToon = (from t in availableToons
                                       let qcCount =
                                            t.TempAchievementProgress =
                                                (from prog in t.ZoneProgress
                                                 where a.Zones.Any(x => x.Id == prog.Key)
                                                 select prog.Value).Sum()
                                       where qcCount > Config.SimQuestThreshold
                                       orderby qcCount descending
                                       select t).FirstOrDefault();

                        if (recToon != null)
                        {
                            aResult.Recomended = new BNetCharacter()
                            {
                                Class = recToon.Class,
                                Faction = recToon.Faction,
                                Level = recToon.Level,
                                Name = recToon.Name,
                                Race = recToon.Race,
                                Realm = recToon.Realm,
                                ThumbnailPath = recToon.ThumbnailPath
                            };

                            aResult.QuestsCompleted = recToon.TempAchievementProgress;
                            if (recToon.Faction == Faction.Alliance)
                            {
                                aResult.TotalQuests = a.Zones.Sum(x => x.AllianceQuestCount);
                            }
                            else
                            {
                                aResult.TotalQuests = a.Zones.Sum(x => x.HordeQuestCount);
                            }

                            aResult.PercentComplete = (int)(((float)aResult.QuestsCompleted / aResult.TotalQuests) * 100);
                        }
                        else
                        {
                            aResult.QuestsCompleted = 0;
                            aResult.PercentComplete = 0;
                            aResult.TotalQuests = Math.Max(a.Zones.Sum(x => x.AllianceQuestCount), a.Zones.Sum(x => x.HordeQuestCount));
                        }
                    }
                    achResults.Add(aResult);
                }
                mResult.AchievProgress = achResults;

                var mAch = main.Achievements.FirstOrDefault(x => x.Id == m.AchievementId);
                if (mAch != null)
                {
                    mResult.CompletedTimestamp = mAch.Timestamp;
                }

                mResults.Add(mResult);
            }

            //filter out linked metas/achieves
            List<int> metasToRemove = new List<int>();
            foreach (MetaResult ma in mResults.Where(x => x.LinkedMetaId.HasValue))
            {
                if (!(metasToRemove.Contains(ma.Id) || metasToRemove.Contains(ma.LinkedMetaId.Value)))
                {
                    MetaResult mb = mResults.Single(x => x.Id == ma.LinkedMetaId);

                    if (ma.CompletedTimestamp.HasValue)
                    {
                        metasToRemove.Add(mb.Id);
                    }
                    else if (mb.CompletedTimestamp.HasValue)
                    {
                        metasToRemove.Add(ma.Id);
                    }
                    else
                    {

                        int aQuestComplete = ma.AchievProgress.Where(x => !x.CompletedTimestamp.HasValue).Sum(x => x.QuestsCompleted);
                        int bQuestComplete = mb.AchievProgress.Where(x => !x.CompletedTimestamp.HasValue).Sum(x => x.QuestsCompleted);

                        if (aQuestComplete > bQuestComplete)
                        {
                            metasToRemove.Add(mb.Id);
                        }
                        else
                        {
                            metasToRemove.Add(ma.Id);
                        }
                    }
                }
            }
            foreach (var mId in metasToRemove)
            {
                mResults.RemoveAll(x => x.Id == mId);
            }

            List<int> achToRemove = new List<int>();
            foreach (MetaResult ma in mResults.Where(x => x.AchievProgress != null))
            {
                achToRemove.Clear();

                foreach (AchieveResult a1 in ma.AchievProgress.Where(x => x.LinkedAchieveId.HasValue))
                {
                    if (!(achToRemove.Contains(a1.Id) || achToRemove.Contains(a1.LinkedAchieveId.Value)))
                    {
                        var a2 = ma.AchievProgress.Single(x => x.Id == a1.LinkedAchieveId.Value);

                        if (a1.QuestsCompleted > a2.QuestsCompleted)
                        {
                            achToRemove.Add(a2.Id);
                        }
                        else
                        {
                            achToRemove.Add(a1.Id);
                        }
                    }
                }

                foreach (int aId in achToRemove)
                {
                    ma.AchievProgress.RemoveAll(x => x.Id == aId);
                }
            }

            //calc totals
            
            foreach(var meta in mResults)
            {
                
                meta.TotalQuests = meta.AchievProgress.Sum(x => x.TotalQuests);
                meta.QuestsCompleted = meta.AchievProgress.Sum(x => x.QuestsCompleted);
                meta.QuestsCompletedPercent = (int)(((float)meta.QuestsCompleted / meta.TotalQuests) * 100);
                meta.TotalAchievs = meta.AchievProgress.Count;
                meta.CompletedAchieves = meta.AchievProgress.Count(x => x.CompletedTimestamp.HasValue);
            }


            lm.TotalMetas = mResults.Count;
            lm.CompletedMetas = mResults.Count(x => x.CompletedTimestamp.HasValue);
            foreach (var meta in mResults.Where(x => !x.CompletedTimestamp.HasValue))
            {
                meta.TotalAchievs = meta.AchievProgress.Count;
                meta.CompletedAchieves = meta.AchievProgress.Count(x => x.CompletedTimestamp.HasValue);
            }

            //character summary
            List<CharacterRecomentationResult> toonSummary = new List<CharacterRecomentationResult>();

            var inProgress = mResults.Where(x => x.AchievProgress != null).SelectMany(x => x.AchievProgress).Where(x => x.Recomended != null).OrderByDescending(x => x.PercentComplete);

            foreach (var inProg in inProgress)
            {
                var summary = toonSummary.FirstOrDefault(x => x.Toon.Name == inProg.Recomended.Name && x.Toon.Realm == inProg.Recomended.Realm);
                if (summary == null)
                {
                    summary = new CharacterRecomentationResult();
                    summary.Toon = inProg.Recomended;
                    toonSummary.Add(summary);
                }
                summary.AchievesInProgress.Add(new AchieveResult()
                {
                    CompletedTimestamp = inProg.CompletedTimestamp,
                    Faction = inProg.Faction,
                    Id = inProg.Id,
                    LinkedAchieveId = inProg.LinkedAchieveId,
                    Name = inProg.Name,
                    QuestsCompleted = inProg.QuestsCompleted,
                    ZoneString = inProg.ZoneString,
                    Icon = inProg.Icon,
                    TotalQuests = inProg.TotalQuests,
                    PercentComplete = inProg.PercentComplete
                });

            }

            lm.CharacterSummary = toonSummary.OrderByDescending(x => x.AchievesInProgress.Count);

            lm.MetaProgress = mResults;
            lm.TotalQuests = mResults.Sum(x => x.TotalQuests);

            var lmAchieve = main.Achievements.FirstOrDefault(x => x.Id == lm.Id);
            if (lmAchieve != null)
            {
                lm.CompletedTimestamp = lmAchieve.Timestamp;
            }

            lm.QuestsCompleted = mResults.Sum(x => x.QuestsCompleted);
            lm.QuestCompletedPercent = (int)(((float)lm.QuestsCompleted / lm.TotalQuests) * 100);

            return lm;
        }
    }
}