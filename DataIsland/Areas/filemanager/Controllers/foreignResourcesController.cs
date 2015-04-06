using DataIsland.Areas.filemanager.CustomActions;
using dataislandcommon.Models.DataCache;
using dataislandcommon.Services.FileSystem;
using dataislandcommon.Services.Utilities;
using dimain.Services.System;
using dimain.Services.System.Cache;
using FileManager.Models.db;
using FileManager.Services.FileManager;
using FileManager.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace DataIsland.Areas.filemanager.Controllers
{
    [Authorize]
    [RouteArea("filemanager")]
    [RoutePrefix("foreignresources")]
    [Route("{action}")]
    public class foreignResourcesController : Controller
    {
        public IFilePathProviderService PathProvider { get; set; }

        public IDataCacheService DataCacheService { get; set; }

        public IDiUserService DiUsers { get; set; }

        public ISharedResourcesService SharedResources { get; set; }

        public IUtilitiesSingleton Utilities { get; set; }

        public IImageUtilitiesSingleton ImageUtilities { get; set; }

        public IUserPassportTokensSingleton PassportTokenSingleton { get; set; }

        private readonly IFileService FileService;

        public foreignResourcesController(IFileService fileService)
        {

            FileService = fileService;

        }

        [OutputCache(Duration = 3600, VaryByParam = "none", Location = OutputCacheLocation.Client)]
        [Route("thumbnail/{userId}/{resourceId}/{size}/{*path}")]
        public async Task<FileContentResult> Thumbnail(string userId, string resourceId, int size, string path)
        {
            string ownerUsername = await this.DiUsers.GetUsernameFromUserId(this.Utilities.UnescapeUserId(userId));

            string callingUser = this.User.Identity.Name;
            callingUser = await this.DiUsers.GetUserIdByFromUsername(callingUser);
            if (!string.IsNullOrEmpty(callingUser))
            {
                if (!await this.SharedResources.CheckRecipientExists(this.Utilities.UnescapeUserId(resourceId), callingUser, ownerUsername))
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
            
            if (!string.IsNullOrEmpty(ownerUsername))
            {
                SharedResource sharedResource = await this.SharedResources.GetSharedResourceByID(this.Utilities.UnescapeUserId(resourceId), ownerUsername);
                if (sharedResource != null)
                {
                    string fullPath = sharedResource.FullPath + "/" + path;
                    HttpContext.Response.Cache.SetCacheability(HttpCacheability.Private);
                    HttpContext.Response.Cache.SetMaxAge(new TimeSpan(1, 0, 0));
                    HttpContext.Response.Cache.SetExpires(DateTime.Now.AddDays(1));
                    HttpContext.Response.Cache.SetNoServerCaching();
                    HttpContext.Response.ExpiresAbsolute = DateTime.Now.AddDays(1);
                    string pathprefix = PathProvider.GetUserFilesPath(ownerUsername);
                    
                    if (System.IO.File.Exists(pathprefix + fullPath) || System.IO.Directory.Exists(pathprefix + fullPath))
                    {
                        DateTime lastModifiedDate = DateTime.Now;
                        if (System.IO.File.Exists(pathprefix + fullPath))
                        {
                            string previewPath = PathProvider.GetUserDataPath(ownerUsername) + FileManagerConsts.FilesPreviewsPath + fullPath;
                            previewPath = Path.GetDirectoryName(previewPath) + "/" + Path.GetFileNameWithoutExtension(previewPath) + ".png";

                            lastModifiedDate = new System.IO.FileInfo(previewPath).LastWriteTime;
                        }
                        if (System.IO.Directory.Exists(pathprefix + fullPath))
                        {
                            lastModifiedDate = new System.IO.FileInfo(pathprefix + fullPath).LastWriteTime;
                        }
                        string rawIfModifiedSince = Request.Headers
                                                     .Get("If-Modified-Since");
                        if (string.IsNullOrEmpty(rawIfModifiedSince))
                        {
                            // Set Last Modified time
                            HttpContext.Response.Cache.SetLastModified(lastModifiedDate);
                        }
                        else
                        {
                            DateTime ifModifiedSince = DateTime.Parse(rawIfModifiedSince);
                            DateTime timeToCompare = DateTime.Parse(lastModifiedDate.ToString());
                            // HTTP does not provide milliseconds, so remove it from the comparison
                            if (timeToCompare == ifModifiedSince)
                            {
                                // The requested file has not changed
                                Response.StatusCode = 304;
                                return null;
                            }
                        }

                        if (System.IO.File.Exists(pathprefix + fullPath))
                        {
                            //if it is a file try get pregenerated preview first. if it fails generate preview old way
                            string previewPath = PathProvider.GetUserDataPath(ownerUsername) + FileManagerConsts.FilesPreviewsPath + fullPath;
                            previewPath = Path.GetDirectoryName(previewPath) + "/" + Path.GetFileNameWithoutExtension(previewPath) + ".png";
                            if (System.IO.File.Exists(previewPath))
                            {
                                using (Bitmap preview = (Bitmap)Image.FromFile(previewPath))
                                {
                                    using (Bitmap bmp = ImageUtilities.ResizePictureSquare(preview, size, size, Color.Transparent))
                                    {
                                        if (bmp != null)
                                        {
                                            byte[] output = ImageUtilities.TransformImageToByte(bmp, "png");
                                            return File(output, Utilities.GetProperContentType("png", true), Path.GetFileName(pathprefix + fullPath));
                                        }
                                    }
                                }
                            }
                        }
                        //we cannot get pregenerated preview so we need to generate it from scratch
                        using (Bitmap thmbnail = FileService.GenerateThumbnailToImage(pathprefix, fullPath))
                        {
                            using (Bitmap bmp = ImageUtilities.ResizePictureSquare(thmbnail, size, size, Color.Transparent))
                            {
                                if (bmp != null)
                                {
                                    byte[] output = ImageUtilities.TransformImageToByte(bmp, "png");
                                    return File(output, Utilities.GetProperContentType("png", true), Path.GetFileName(pathprefix + fullPath));
                                }
                            }
                        }

                    }
                    else
                    {
                        Response.StatusCode = 404;
                    }
                }
            }
            return null;
        }

        [OutputCache(Duration = 3600, VaryByParam = "none", Location = OutputCacheLocation.Client)]
        [Route("preview/{userId}/{resourceId}/{size}/{*path}")]
        public async  Task<FileContentResult> Preview(string userId, string resourceId, int size, string path)
        {
            string ownerUsername = await this.DiUsers.GetUsernameFromUserId(this.Utilities.UnescapeUserId(userId));

            string callingUser = this.User.Identity.Name;
            callingUser = await this.DiUsers.GetUserIdByFromUsername(callingUser);
            if (!string.IsNullOrEmpty(callingUser))
            {
                if (!await this.SharedResources.CheckRecipientExists(this.Utilities.UnescapeUserId(resourceId), callingUser, ownerUsername))
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

            if (!string.IsNullOrEmpty(ownerUsername))
            {
                SharedResource sharedResource = await this.SharedResources.GetSharedResourceByID(this.Utilities.UnescapeUserId(resourceId), ownerUsername);
                if (sharedResource != null)
                {
                    string fullPath = sharedResource.FullPath + "/" + path;
                    HttpContext.Response.Cache.SetCacheability(HttpCacheability.Private);
                    HttpContext.Response.Cache.SetMaxAge(new TimeSpan(1, 0, 0));
                    HttpContext.Response.Cache.SetNoServerCaching();
                    HttpContext.Response.Cache.SetExpires(DateTime.Now.AddDays(1));
                    HttpContext.Response.ExpiresAbsolute = DateTime.Now.AddDays(1);

                    string pathprefix = PathProvider.GetUserFilesPath(ownerUsername);
                    if (System.IO.File.Exists(pathprefix + fullPath) || System.IO.Directory.Exists(pathprefix + fullPath))
                    {
                        DateTime lastModifiedDate = DateTime.Now;
                        if (System.IO.File.Exists(pathprefix + fullPath))
                        {
                            string previewPath = PathProvider.GetUserDataPath(ownerUsername) + FileManagerConsts.FilesPreviewsPath + fullPath;
                            previewPath = Path.GetDirectoryName(previewPath) + "/" + Path.GetFileNameWithoutExtension(previewPath) + ".png";

                            lastModifiedDate = new System.IO.FileInfo(previewPath).LastWriteTime;
                        }
                        if (System.IO.Directory.Exists(pathprefix + fullPath))
                        {
                            lastModifiedDate = new System.IO.FileInfo(pathprefix + fullPath).LastWriteTime;
                        }
                        string rawIfModifiedSince = Request.Headers
                                                     .Get("If-Modified-Since");
                        if (string.IsNullOrEmpty(rawIfModifiedSince))
                        {
                            // Set Last Modified time
                            HttpContext.Response.Cache.SetLastModified(lastModifiedDate);
                        }
                        else
                        {
                            DateTime ifModifiedSince = DateTime.Parse(rawIfModifiedSince);
                            DateTime timeToCompare = DateTime.Parse(lastModifiedDate.ToString());
                            // HTTP does not provide milliseconds, so remove it from the comparison
                            if (timeToCompare == ifModifiedSince)
                            {
                                // The requested file has not changed
                                Response.StatusCode = 304;
                                return null;
                            }
                        }

                        if (System.IO.File.Exists(pathprefix + fullPath))
                        {
                            //if it is a file try get pregenerated preview first. if it fails generate preview old way
                            string previewPath = PathProvider.GetUserDataPath(ownerUsername) + FileManagerConsts.FilesPreviewsPath + fullPath;
                            previewPath = Path.GetDirectoryName(previewPath) + "/" + Path.GetFileNameWithoutExtension(previewPath) + ".png";
                            if (System.IO.File.Exists(previewPath))
                            {
                                using (Bitmap preview = (Bitmap)Image.FromFile(previewPath))
                                {
                                    using (Bitmap bmp = ImageUtilities.ResizePictureIfLarger(preview, size, size,Color.White))
                                    {
                                        if (bmp != null)
                                        {
                                            byte[] output = ImageUtilities.TransformImageToByte(bmp, "jpg");
                                            return File(output, Utilities.GetProperContentType("jpg", true), Path.GetFileName(pathprefix + fullPath));
                                        }
                                    }
                                }
                            }
                        }
                        //we cannot get pregenerated preview so we need to generate it from scratch
                        using (Bitmap thmbnail = FileService.GenerateThumbnailToImage(pathprefix, fullPath))
                        {
                            using (Bitmap bmp = ImageUtilities.ResizePictureSquare(thmbnail, size, size,Color.White))
                            {
                                if (bmp != null)
                                {
                                    byte[] output = ImageUtilities.TransformImageToByte(bmp, "jpg");
                                    return File(output, Utilities.GetProperContentType("jpg", true), Path.GetFileName(pathprefix + fullPath));
                                }
                            }
                        }

                    }
                    else
                    {
                        Response.StatusCode = 404;
                        return null;
                    }
                }
            }
            return null;
        }

        [OutputCache(Duration = 3600, VaryByParam = "none", Location = OutputCacheLocation.Client)]
        [Route("previewcustomsize/{userId}/{resourceId}/{width}/{height}/{*path}")]
        public async Task<FileContentResult> PreviewCustomSize(string userId, string resourceId, int width, int height, string path)
        {
            string ownerUsername = await this.DiUsers.GetUsernameFromUserId(this.Utilities.UnescapeUserId(userId));

            string callingUser = this.User.Identity.Name;
            callingUser = await this.DiUsers.GetUserIdByFromUsername(callingUser);
            if (!string.IsNullOrEmpty(callingUser))
            {
                if (!await this.SharedResources.CheckRecipientExists(this.Utilities.UnescapeUserId(resourceId), callingUser, ownerUsername))
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

            if (!string.IsNullOrEmpty(ownerUsername))
            {
                SharedResource sharedResource = await this.SharedResources.GetSharedResourceByID(this.Utilities.UnescapeUserId(resourceId), ownerUsername);
                if (sharedResource != null)
                {
                    string fullPath = sharedResource.FullPath + "/" + path;
                    HttpContext.Response.Cache.SetCacheability(HttpCacheability.Private);
                    HttpContext.Response.Cache.SetMaxAge(new TimeSpan(1, 0, 0));
                    HttpContext.Response.Cache.SetExpires(DateTime.Now.AddDays(1));
                    HttpContext.Response.Cache.SetNoServerCaching();
                    HttpContext.Response.ExpiresAbsolute = DateTime.Now.AddDays(1);
                    string pathprefix = PathProvider.GetUserFilesPath(ownerUsername);
                    if (System.IO.File.Exists(pathprefix + fullPath) || System.IO.Directory.Exists(pathprefix + fullPath))
                    {

                        DateTime lastModifiedDate = DateTime.Now;
                        if (System.IO.File.Exists(pathprefix + fullPath))
                        {
                            string previewPath = PathProvider.GetUserDataPath(ownerUsername) + FileManagerConsts.FilesPreviewsPath + fullPath;
                            previewPath = Path.GetDirectoryName(previewPath) + "/" + Path.GetFileNameWithoutExtension(previewPath) + ".png";

                            lastModifiedDate = new System.IO.FileInfo(previewPath).LastWriteTime;
                        }
                        if (System.IO.Directory.Exists(pathprefix + fullPath))
                        {
                            lastModifiedDate = new System.IO.FileInfo(pathprefix + path).LastWriteTime;
                        }
                        string rawIfModifiedSince = Request.Headers
                                                     .Get("If-Modified-Since");
                        if (string.IsNullOrEmpty(rawIfModifiedSince))
                        {
                            // Set Last Modified time
                            HttpContext.Response.Cache.SetLastModified(lastModifiedDate);
                        }
                        else
                        {
                            DateTime ifModifiedSince = DateTime.Parse(rawIfModifiedSince);
                            DateTime timeToCompare = DateTime.Parse(lastModifiedDate.ToString());
                            // HTTP does not provide milliseconds, so remove it from the comparison
                            if (timeToCompare == ifModifiedSince)
                            {
                                // The requested file has not changed
                                Response.StatusCode = 304;
                                return null;
                            }
                        }

                        if (System.IO.File.Exists(pathprefix + fullPath))
                        {
                            //if it is a file try get pregenerated preview first. if it fails generate preview old way
                            string previewPath = PathProvider.GetUserDataPath(ownerUsername) + FileManagerConsts.FilesPreviewsPath + fullPath;
                            previewPath = Path.GetDirectoryName(previewPath) + "/" + Path.GetFileNameWithoutExtension(previewPath) + ".png";
                            if (System.IO.File.Exists(previewPath))
                            {
                                using (Bitmap preview = (Bitmap)Image.FromFile(previewPath))
                                {
                                    using (Bitmap bmp = ImageUtilities.ResizePictureSquare(preview, width, height,Color.White))
                                    {
                                        if (bmp != null)
                                        {
                                            byte[] output = ImageUtilities.TransformImageToByte(bmp, "jpg");
                                            return File(output, Utilities.GetProperContentType("jpg", true), Path.GetFileName(pathprefix + fullPath));
                                        }
                                    }
                                }
                            }
                        }
                        //we cannot get pregenerated preview so we need to generate it from scratch
                        using (Bitmap thmbnail = FileService.GenerateThumbnailToImage(pathprefix, fullPath))
                        {
                            using (Bitmap bmp = ImageUtilities.ResizePictureSquare(thmbnail, width, height,Color.White))
                            {
                                if (bmp != null)
                                {
                                    byte[] output = ImageUtilities.TransformImageToByte(bmp, "jpg");
                                    return File(output, Utilities.GetProperContentType("jpg", true), Path.GetFileName(pathprefix + fullPath));
                                }
                            }
                        }

                    }
                    else
                    {
                        Response.StatusCode = 404;
                    }
                }
            }
            return null;
        }

        [Route("uploadfile/{resourceId}/{userId}")]
        public async Task<string> UploadFile(string resourceId, string userId)
        {
            string ownerUsername = await this.DiUsers.GetUsernameFromUserId(this.Utilities.UnescapeUserId(userId));

            string callingUser = this.User.Identity.Name;
            callingUser = await this.DiUsers.GetUserIdByFromUsername(callingUser);
            if (!string.IsNullOrEmpty(callingUser))
            {
                if (!await this.SharedResources.CheckRecipientExists(this.Utilities.UnescapeUserId(resourceId), callingUser, ownerUsername))
                {
                    return "{'OK': 0, 'info': 'Access denied'}";
                }
            }
            else
            {
                return "{'OK': 0, 'info': 'Access denied'}";
            }

            SharedResource sharedResource = await this.SharedResources.GetSharedResourceByID(this.Utilities.UnescapeUserId(resourceId), ownerUsername);
            if (sharedResource != null)
            {
                if (sharedResource.IsDirectory && (sharedResource.IsWrite || sharedResource.IsAll))
                {
                    Stream fs = Request.Files[0].InputStream;
                    byte[] filechunk = new byte[fs.Length];
                    int streamLengths = await fs.ReadAsync(filechunk, 0, filechunk.Length);
                    var chunks = int.Parse(Request["chunks"]);
                    var chunk = int.Parse(Request["chunk"]);
                    var name = Request["name"];

                    string uploadPath = PathProvider.GetUserUploadFolder(ownerUsername);
                    System.IO.File.WriteAllBytes(uploadPath + "/" + chunk.ToString() + "_" + name, filechunk);
                    if (chunk >= chunks - 1)
                    {
                        string pathprefix = PathProvider.GetUserFilesPath(ownerUsername);
                        string filePath = sharedResource.FullPath;
                        using (FileStream finalStream = new FileStream(pathprefix + filePath + "/" + name, FileMode.Create, FileAccess.ReadWrite))
                        {
                            for (int i = 0; i < chunks; i++)
                            {
                                byte[] data = System.IO.File.ReadAllBytes(uploadPath + "/" + i.ToString() + "_" + name);
                                await finalStream.WriteAsync(data, 0, data.Length);
                                System.IO.File.Delete(uploadPath + "/" + i.ToString() + "_" + name);

                            }
                            finalStream.Close();

                            //try get file preview
                            byte[] preview = FileService.GetFilePreview(pathprefix, filePath + "/" + name);
                            if (preview != null)
                            {
                                string previewPath = PathProvider.GetUserDataPath(ownerUsername) + FileManagerConsts.FilesPreviewsPath + filePath;
                                Directory.CreateDirectory(previewPath);
                                previewPath = previewPath + "/" + Path.GetFileNameWithoutExtension(pathprefix + filePath + "/" + name) + ".png";
                                System.IO.File.WriteAllBytes(previewPath, preview);
                            }
                        }
                    }
                }

                return "{'OK': 1, 'info': 'Upload successful.'}";
            }

            return "{'OK': 0, 'info': 'Access denied'}";
        }

        [Route("get/{resourceId}/{userId}/{*path}")]
        [HttpGet]
        public async Task<FilePathResult> DownloadFile(string resourceId, string userId,string path)
        {
            string ownerUsername = await this.DiUsers.GetUsernameFromUserId(this.Utilities.UnescapeUserId(userId));

            string callingUser = this.User.Identity.Name;
            callingUser = await this.DiUsers.GetUserIdByFromUsername(callingUser);
            if (!string.IsNullOrEmpty(callingUser))
            {
                if (!await this.SharedResources.CheckRecipientExists(this.Utilities.UnescapeUserId(resourceId), callingUser, ownerUsername))
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

            SharedResource sharedResource = await this.SharedResources.GetSharedResourceByID(this.Utilities.UnescapeUserId(resourceId), ownerUsername);
            if (sharedResource != null)
            {
                string pathprefix = PathProvider.GetUserFilesPath(ownerUsername);
                if (System.IO.File.Exists(pathprefix + sharedResource.FullPath + "/" + path))
                {

                    return File(pathprefix + sharedResource.FullPath + "/" + path, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(pathprefix + sharedResource.FullPath + "/" + path));
                }
            }
            return null;
        }
    }
}