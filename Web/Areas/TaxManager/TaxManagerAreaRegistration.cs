using System.Web.Mvc;

namespace Web.Areas.TaxManager
{
    public class TaxManagerAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "TaxManager";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "TaxManager_default",
                "TaxManager/{controller}/{action}/{id}",
                new { area = "TaxManager", controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}