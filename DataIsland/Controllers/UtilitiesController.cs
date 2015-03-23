using dataislandcommon.Services.System;
using dataislandcommon.Services.Utilities;
using dimain.Models.maindb;
using dimain.Services.System;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DataIsland.Controllers
{
    public class UtilitiesController : Controller
    {
        
        public IUserService UserService { get; set; }

        
        public IDiUserService DiUsers { get; set; }

        
        public IUtilitiesSingleton Utilities { get; set; }

        
        public IImageUtilitiesSingleton ImageUtilities { get; set; }

        // GET: Utilities
        public ActionResult Index()
        {
            return View();
        }

        public FileResult userAvatar(string username, int size, string type)
        {
            byte[] userAvatar = UserService.GetUserAvatar(username,size,type);
            if (userAvatar == null)
            {
                string defaultAvatarPath = Server.MapPath("~/Content/panel/images/profile-picture.png");
                using (System.Drawing.Image img = System.Drawing.Image.FromFile(defaultAvatarPath))
                {
                    using (System.Drawing.Bitmap resizedImage = ImageUtilities.TransformPicture((System.Drawing.Bitmap)img, size, size, type))
                    {
                        userAvatar = ImageUtilities.TransformImageToByte(resizedImage, "png");
                    }
                }
            }
            Response.Expires = 1440;
            return File(userAvatar, Utilities.GetProperContentType("png", true), username + "avatar.png");
        }

        public async Task<FileResult> useridavatar(string username, int size, string type)
        {
            byte[] userAvatar = await UserService.GetUserAvatarFromUserId(this.Utilities.UnescapeUserId(username), size, type);
            if (userAvatar == null)
            {
                string defaultAvatarPath = Server.MapPath("~/Content/panel/images/profile-picture.png");
                using (System.Drawing.Image img = System.Drawing.Image.FromFile(defaultAvatarPath))
                {
                    using (System.Drawing.Bitmap resizedImage = ImageUtilities.TransformPicture((System.Drawing.Bitmap)img, size, size, type))
                    {
                        userAvatar = ImageUtilities.TransformImageToByte(resizedImage, "png");
                    }
                }
            }
            Response.Expires = 1440;
            return File(userAvatar, Utilities.GetProperContentType("png", true), username + "avatar.png");
        }

        public ActionResult RefreshAvatar(string id)
        {
            string avatarUrl = "/utilities/userAvatar/" + id + "/" + Request["size"] + "/" + Request["type"];
            ViewBag.ImageUrl = avatarUrl;
            return View();
        }

        public ActionResult RefreshImage(string path)
        {
            string imgUrl = path;
            ViewBag.ImageUrl = "/"+imgUrl;
            return View();
        }

        public async Task<string> UpdateDatabases()
        {
            List<DiUser> users = await this.DiUsers.GetUsers();
            foreach(DiUser usr in users)
            {
                dataislandcommon.Bootstrapper.UpdateDatabase(usr.Username);
                Streamail.Bootstraper.UpdateDatabase(usr.Username);
                FileManager.Bootstraper.UpdateDatabase(usr.Username);
                PluginAreaBootstrapper.RunUpdateDatabaseForLoadedPlugins(usr.Username);
            }
            return "ok";
        }
    }
}