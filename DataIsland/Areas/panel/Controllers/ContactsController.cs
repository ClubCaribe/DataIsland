using dataislandcommon.Models.userdb;
using dataislandcommon.Services.System;
using dataislandcommon.Services.Utilities;
using dataislandcommon.Utilities;
using dimain.Services.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DataIsland.Areas.panel.Controllers
{
    [RouteArea("panel")]
    [RoutePrefix("Contacts")]
    [Route("{action=index}")]
    public class ContactsController : Controller
    {
        
        
        public IUserContactsService Contacts { get; set; }
        
        public IUtilitiesSingleton Utilities { get; set; }
        
        public IImageUtilitiesSingleton ImageUtilities { get; set; }

        public IDiUserService DiUsers { get; set; }

        // GET: panel/Contacts
        public ActionResult Index()
        {
            return View();
        }

        [Route("chat/{chatUsername}")]
        public async Task<ActionResult> Chat(string chatUsername)
        {
            UserContact contact = await this.Contacts.GetContactByUsername(chatUsername, this.User.Identity.Name);
            if (contact != null)
            {
                ViewBag.UserID = contact.UserId;
            }
            return View();
        }

        [Route("contactthumbnail/{username}/{size}/{type}/{isuerid?}")]
        public async Task<FileResult> ContactThumbnail(string username, int size, string type, string isuerid)
        {
            string user = username;

            if(!string.IsNullOrEmpty(isuerid))
            {
                username = this.Utilities.UnescapeUserId(username);
                user = await this.DiUsers.GetUsernameFromUserId(username);
            }

            byte[] userAvatar = await this.Contacts.GetContactAvatar(user, size, type, this.User.Identity.Name);
            if (userAvatar == null)
            {
                string defaultAvatarPath = Server.MapPath(DiConsts.DefaultAvatarPath);
                System.Drawing.Image img = System.Drawing.Image.FromFile(defaultAvatarPath);
                System.Drawing.Bitmap resizedImage = ImageUtilities.TransformPicture((System.Drawing.Bitmap)img, size, size, type);
                userAvatar = ImageUtilities.TransformImageToByte(resizedImage, "png");
            }
            Response.Expires = 1440;
            return File(userAvatar, Utilities.GetProperContentType("png", true), username + "avatar.png");
        }
    }
}