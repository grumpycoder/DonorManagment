using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

//                        routes.MapRoute(
//                            name: "Default",
//                            url: "{controller}/{action}/{id}",
//                            defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
//                        );

            //Default route set to Dashboard area home controller
            routes.MapRoute(
                  "Default", // Route name
                  "{controller}/{action}/{id}", // URL with parameters
                  new { area = "Dashboard", controller = "Home", action = "Index", id = UrlParameter.Optional }, // Parameter defaults
                  new[] { "Web.Areas.Dashboard.Controllers" }
              ).DataTokens.Add("area", "Dashboard");


        }
    }
}
