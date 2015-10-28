using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.TaxManager.Controllers
{
    public class HomeController : Controller
    {
        // GET: TaxManager/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}