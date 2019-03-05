using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoremasterHelper.Models;

namespace LoremasterHelper.Controllers
{
    public class GuidesController : BaseController
    {
#if !DEBUG
        [OutputCache(Duration = Config.OutputCacheDuration, VaryByParam = "guide;region")]
#endif
        public ActionResult Index(string guide, string region)
        {
            Layout form = new Layout();
            form.Region = region;
            return View((guide ?? "Loremaster").Replace("-", "_"), form);
        }
    }
}