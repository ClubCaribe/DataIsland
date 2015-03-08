using dataislandcommon.Attributes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataIsland.Areas.panel.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public HomeController()
        {

        }

        //
        // GET: /panel/Home/
        public ActionResult Index()
        {
            return View();
        }
	}
}