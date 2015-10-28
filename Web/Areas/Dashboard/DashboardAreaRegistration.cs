using System.Web.Mvc;

namespace Web.Areas.Dashboard
{
    public class DashboardAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Dashboard";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
               name: "Dashboard_Default",
               url: "Dashboard/{controller}/{action}/{id}",
               defaults: new
               {
                   area = "Dashboard",
                   controller = "Home",
                   action = "Index",
                   id = UrlParameter.Optional
               });

            //            context.MapRoute(
            //                "Dashboard_default",
            //                "Dashboard/{controller}/{action}/{id}",
            //                new { action = "Index", id = UrlParameter.Optional }
            //            );
        }
    }
}