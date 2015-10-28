using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Areas.EventManager.Controllers
{
    public class HomeController : Controller
    {
        // GET: EventManager/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}