using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EGSW.Web.Controllers
{
    public class StaticController : Controller
    {
        // GET: Static
        public ActionResult Index()
        {
            return View();
        }



        // GET: Static
        public ActionResult AboutUs()
        {
            return View();
        }


        // GET: Static
        public ActionResult Faq()
        {
            return View();
        }
    }
}