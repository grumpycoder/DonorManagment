using System.Web.Mvc;

namespace Web.Areas.DonorTax
{
    public class DonorTaxAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "DonorTax";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "DonorTax_default",
                "DonorTax/{controller}/{action}/{id}",
                new { area="DonorTax", controller="Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}