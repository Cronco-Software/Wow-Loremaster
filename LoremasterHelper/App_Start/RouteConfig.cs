using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace LoremasterHelper
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "char",
                url: "Calculator/{region}",
                defaults: new { controller = "Home", action = "Calculator", region = UrlParameter.Optional });

            routes.MapRoute(
                name: "progress",
                url: "Progress",
                defaults: new { controller = "Home", action = "Progress" });

            routes.MapRoute(
                name: "ach",
                url: "Achievements/{region}",
                defaults: new { controller = "Home", action = "Achievements", region = UrlParameter.Optional });

            routes.MapRoute(
                name: "pp",
                url: "PrivacyPolicy/{region}",
                defaults: new { controller = "Home", action = "PrivacyPolicy", region = UrlParameter.Optional });

            routes.MapRoute(
                name: "terms",
                url: "Terms/{region}",
                defaults: new { controller = "Home", action = "Terms", region = UrlParameter.Optional });

            routes.MapRoute(
                name: "about",
                url: "About/{region}",
                defaults: new { controller = "Home", action = "About", region = UrlParameter.Optional });

            routes.MapRoute(
                name: "faq",
                url: "FAQ/{region}",
                defaults: new { controller = "Home", action = "FAQ", region = UrlParameter.Optional });

            routes.MapRoute(
                name: "thanks",
                url: "Thanks",
                defaults: new { controller = "Home", action = "Thanks" });

            routes.MapRoute(
                name: "guides",
                url: "Guides/{guide}/{region}",
                defaults: new { controller = "Guides", action = "Index", guide = UrlParameter.Optional, region = UrlParameter.Optional });

            routes.MapRoute(
                name: "sitemap",
                url: "sitemap.xml",
                defaults: new { controller = "Services", action = "Sitemap" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Calculator", id = UrlParameter.Optional }
            );
        }
    }
}
