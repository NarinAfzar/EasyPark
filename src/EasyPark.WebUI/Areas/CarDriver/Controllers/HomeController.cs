using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EasyPark.WebUI.Areas.CarDriver.Controllers
{
    public class HomeController : Controller
    {
        // GET: CarDriver/Home
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Tracking");
        }
    }
}