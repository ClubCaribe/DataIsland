using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DataIsland
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            AreaRegistration.RegisterAllAreas();

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "DataIsland.Controllers" }
            );

            routes.MapRoute(
                name: "Avatar",
                url: "{controller}/{action}/{username}/{size}/{type}",
                defaults: new { controller = "Home", action = "Index", username = UrlParameter.Optional, size = UrlParameter.Optional, type = UrlParameter.Optional },
                namespaces: new[] { "DataIsland.Controllers" }
            );

            routes.MapRoute(
                name: "refreshImage",
                url: "{controller}/{action}/{*path}",
                defaults: new { controller = "Home", action = "Index", username = UrlParameter.Optional, size = UrlParameter.Optional, type = UrlParameter.Optional },
                namespaces: new[] { "DataIsland.Controllers" }
            );
        }
    }
}
