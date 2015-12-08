using System.Web.Http;
using Web.Models;

namespace Web.Controllers.Api
{

    public class DonorTaxApiController : ApiBaseController
    {

        public IHttpActionResult Get()
        {
            var vm = new TaxViewModel();
            vm.HandleRequest();
            vm.IsValid = true;
            return Ok(vm);
        }

        public IHttpActionResult Post(TaxViewModel vm)
        {
            vm.IsValid = ModelState.IsValid;
            vm.HandleRequest();

            // NOTE: Must clear the model state in order to bind
            //       the @Html helpers to the new model values
            if (vm.IsValid)
            {
                ModelState.Clear();
            }
            else
            {
                foreach (var item in vm.ValidationErrors)
                {
                    ModelState.AddModelError(item.Key, item.Value);
                }
            }

            return Ok(vm);
        }

    }
}