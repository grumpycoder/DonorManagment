using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Areas.DonorTax.Models;
using Web.Controllers;

namespace Web.Areas.DonorTax.Controllers
{
    public class HomeController : BaseController
    {
        // GET: DonorTax/Home
        public ActionResult Index()
        {
            var vm = new TaxViewModel();
//            vm.SearchEntity.ConstituentId = "10782387129";
//            vm.EventCommand = "list";
            vm.HandleRequest();

            return View(vm);

//            var tax = db.Constituents.Where(c => c.ConstituentId == "10782387129").Include(i => i.TaxItems).FirstOrDefault(); 
//
//            return View(tax);
        }

        [HttpPost]
        public ActionResult Index(TaxViewModel vm)
        {
            vm.HandleRequest();

            // NOTE: Must clear the model state in order to bind
            //       the @Html helpers to the new model values
            ModelState.Clear();

            return View(vm);
        }
    }
}