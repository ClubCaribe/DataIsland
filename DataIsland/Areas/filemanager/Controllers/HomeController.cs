using dataislandcommon.Attributes;
using dataislandcommon.Models.userdb;
using dataislandcommon.Services.System;
using FileManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DataIsland.Areas.filemanager.Controllers
{
    [RouteArea("filemanager")]
    [Route("{action=index}")]
    [Authorize]
    public class HomeController : Controller
    {
        
        public IFileManagerCommandService FileCommands { get; set; }

        public IUserContactsService Contacts { get; set; }
        // GET: filemanager/Home
        public ActionResult Index()
        {
            return View();
        }

        [Route("sharedResources/{username}")]
        public async Task<ActionResult> sharedResources(string username)
        {
            UserContact contact = await this.Contacts.GetContactByUsername(username, this.User.Identity.Name);
            if (contact != null)
            {
                ViewBag.UserID = contact.UserId;
            }
            return View();
        }
    }
}