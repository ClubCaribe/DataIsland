using dataislandcommon.Attributes;
using FileManager.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataIsland.Areas.filemanager.Controllers
{
    [DiAuthorization]
    public class HomeController : Controller
    {
        
        public IFileManagerCommandService FileCommands { get; set; }
        // GET: filemanager/Home
        public ActionResult Index()
        {
            return View();
        }

        public string TestCommand()
        {
            FileCommands.TestCommand();
            return "ok";
        }
    }
}