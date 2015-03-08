using dataislandcommon.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DataIsland.Areas.panel.Controllers
{
    [Authorize(Roles="admin,all")]
    public class dataislandController : Controller
    {
        // GET: panel/dataisland
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult users()
        {
            return View();
        }

        public ActionResult settings()
        {
            return View();
        }
    }
}