﻿using System;
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
            vm.IsValid = true;
            return View(vm);

//            var tax = db.Constituents.Where(c => c.ConstituentId == "10782387129").Include(i => i.TaxItems).FirstOrDefault(); 
//
//            return View(tax);
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