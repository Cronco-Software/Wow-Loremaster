using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoremasterHelper.Models;
using System.Security.Claims;

namespace LoremasterHelper.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Calculator(string region)
        {
 
            SimulationForm form = new SimulationForm();
            form.Region = region;
            form.IsAuthenticated = false;

            if (User.Identity.IsAuthenticated)
            {
                form.IsAuthenticated = true;
                form.BattleTag = UserBattletag;
            }
            
            return View(form);
        }

        [HttpPost]
        public ActionResult Sim(SimulationForm sim)
        {

            if (sim == null || sim.Toons == null || sim.Toons.Count < 1)
            {
                return RedirectToAction("Calculator", new { region = sim.RegionLink });
            }
            else
            {
                string progressString = string.Join("|",
                    from t in sim.Toons
                    group t by t.Server into serverGrp
                    select serverGrp.Key + "_" + string.Join("~", serverGrp.Select(x => x.Name)));
                                    
                return RedirectToAction("Progress", new { sim = sim.Toons[0].Region + ";" + progressString });
            }
        }

        [HttpGet]
        public ActionResult Progress(string sim)
        {
            if (string.IsNullOrWhiteSpace(sim) || !sim.Contains(";") || !sim.Contains("_"))
            {
                return RedirectToAction("Calculator");
            }

            List<CharacterDefinition> toons = new List<CharacterDefinition>();

            var simParts = sim.Split(';');
            var region = simParts[0];
            var serverGroups = simParts[1].Split('|');
            foreach (var serverGrp in serverGroups)
            {
                var serverParts = serverGrp.Split('_');
                var server = serverParts[0];
                var toonList = serverParts[1].Split('~');
                foreach (var t in toonList)
                {
                    toons.Add(new CharacterDefinition()
                    {
                        Server = server,
                        Region = region,
                        Name = t
                    });
                }
            }

            if (toons == null || toons.Count < 1)
            {
                return RedirectToAction("Calculator");
            }

            SimulationForm form = new SimulationForm();
            form.Region = region;
            form.Toons = toons;
            form.IsAuthenticated = false;

            if (User.Identity.IsAuthenticated)
            {
                form.IsAuthenticated = true;
                form.BattleTag = UserBattletag;
            }

            return View(form);
        }

        

        [OutputCache(Duration = Config.OutputCacheDuration, VaryByParam = "region")]
        public ActionResult Achievements(string region)
        {
            var form = new Layout()
            {
                Region = region
            };
             
            return View(form);
        }
        
    }
}