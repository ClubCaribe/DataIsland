using dataislandcommon.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataIsland.Areas.panel.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {

        public SettingsController()
        {
        }

        // GET: panel/Settings
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult user()
        {
            return View();
        }
    }
}