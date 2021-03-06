﻿using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers.web
{
    public class DonorTaxController : BaseController
    {

        // GET: DonorTax/Home
        public ActionResult Index()
        {
            var vm = new TaxViewModel();
            vm.HandleRequest();
            vm.IsValid = true;
            return View(vm);

        }

        [HttpPost]
        public ActionResult Index(TaxViewModel vm)
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

            return View(vm);
        }

    }
}