using DataIsland.Areas.filemanager.CustomActions;
using dataislandcommon.Models.DataCache;
using dataislandcommon.Services.FileSystem;
using dataislandcommon.Services.Utilities;
using dimain.Services.System;
using dimain.Services.System.Cache;
using FileManager.Services.FileManager;
using FileManager.Utilities;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace DataIsland.Areas.filemanager.Controllers
{
    [RouteArea("filemanager")]
    [RoutePrefix("file")]
    [Route("{action}")]
    public class fileController : Controller
    {
        
        public IFilePathProviderService PathProvider { get; set; }

        
        public IImageUtilitiesSingleton ImageUtilities { get; set; }

        
        public IUtilitiesSingleton Utilities { get; set; }

        
        public IFileManagerService FileManager { get; set; }

        
        public IDataCacheService DataCacheService { get; set; }



        private readonly IFileService FileService;

        public fileController(IFileService fileService)
        {

            FileService = fileService;

        }

        // GET: filemanager/file
        public ActionResult Index()
        {
            return View();
        }

        [Route("thumbnail/{size}/{*path}")]
        public FileContentResult Thumbnail(int size, string path)
        {
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.Private);
            HttpContext.Response.Cache.SetMaxAge(new TimeSpan(1, 0, 0));
            HttpContext.Response.Cache.SetExpires(DateTime.Now.AddDays(1));
            HttpContext.Response.ExpiresAbsolute = DateTime.Now.AddDays(1);
            HttpContext.Response.Cache.SetNoServerCaching();
            string pathprefix = PathProvider.GetUserFilesPath(this.User.Identity.Name);
            if (System.IO.File.Exists(pathprefix + path) || System.IO.Directory.Exists(pathprefix + path))
            {
                DateTime lastModifiedDate = DateTime.Now;
                if (System.IO.File.Exists(pathprefix + path))
                {
                    string previewPath = PathProvider.GetUserDataPath(this.User.Identity.Name) + FileManagerConsts.FilesPreviewsPath + path;
                    previewPath = Path.GetDirectoryName(previewPath) + "/" + Path.GetFileNameWithoutExtension(previewPath) + ".png";

                    lastModifiedDate = new System.IO.FileInfo(previewPath).LastWriteTime;
                }
                if (System.IO.Directory.Exists(pathprefix + path))
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

                if (System.IO.File.Exists(pathprefix + path))
                {
                    //if it is a file try get pregenerated preview first. if it fails generate preview old way
                    string previewPath = PathProvider.GetUserDataPath(this.User.Identity.Name) + FileManagerConsts.FilesPreviewsPath + path;
                    previewPath = Path.GetDirectoryName(previewPath)+"/"+Path.GetFileNameWithoutExtension(previewPath)+".png";
                    if (System.IO.File.Exists(previewPath))
                    {
                        using (Bitmap preview = (Bitmap)Image.FromFile(previewPath))
                        {
                            using (Bitmap bmp = ImageUtilities.ResizePictureSquare(preview, size, size,Color.Transparent))
                            {
                                if (bmp != null)
                                {
                                    byte[] output = ImageUtilities.TransformImageToByte(bmp, "png");
                                    return File(output, Utilities.GetProperContentType("png", true), Path.GetFileName(pathprefix + path));
                                }
                            }
                        }
                    }
                }
                //we cannot get pregenerated preview so we need to generate it from scratch
                using (Bitmap thmbnail = FileService.GenerateThumbnailToImage(pathprefix, path))
                {
                    using (Bitmap bmp = ImageUtilities.ResizePictureSquare(thmbnail, size, size, Color.Transparent))
                    {
                        if (bmp != null)
                        {
                            byte[] output = ImageUtilities.TransformImageToByte(bmp, "png");
                            return File(output, Utilities.GetProperContentType("png", true), Path.GetFileName(pathprefix + path));
                        }
                    }
                }
                
            }
            else
            {
                Response.StatusCode = 404;
            }
            return null;
        }


        [Route("preview/{size}/{*path}")]
        public FileContentResult Preview(int size, string path)
        {
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.Private);
            HttpContext.Response.Cache.SetMaxAge(new TimeSpan(1, 0, 0));
            HttpContext.Response.Cache.SetExpires(DateTime.Now.AddDays(1));
            HttpContext.Response.Cache.SetNoServerCaching();
            HttpContext.Response.ExpiresAbsolute = DateTime.Now.AddDays(1);

            string pathprefix = PathProvider.GetUserFilesPath(this.User.Identity.Name);
            if (System.IO.File.Exists(pathprefix + path) || System.IO.Directory.Exists(pathprefix + path))
            {
                DateTime lastModifiedDate = DateTime.Now;
                if (System.IO.File.Exists(pathprefix + path))
                {
                    string previewPath = PathProvider.GetUserDataPath(this.User.Identity.Name) + FileManagerConsts.FilesPreviewsPath + path;
                    previewPath = Path.GetDirectoryName(previewPath) + "/" + Path.GetFileNameWithoutExtension(previewPath) + ".png";

                    lastModifiedDate = new System.IO.FileInfo(previewPath).LastWriteTime;
                }
                if(System.IO.Directory.Exists(pathprefix + path))
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

                if (System.IO.File.Exists(pathprefix + path))
                {
                    //if it is a file try get pregenerated preview first. if it fails generate preview old way
                    string previewPath = PathProvider.GetUserDataPath(this.User.Identity.Name) + FileManagerConsts.FilesPreviewsPath + path;
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
                                    return File(output, Utilities.GetProperContentType("jpg", true), Path.GetFileName(pathprefix + path));
                                }
                            }
                        }
                    }
                }
                //we cannot get pregenerated preview so we need to generate it from scratch
                using (Bitmap thmbnail = FileService.GenerateThumbnailToImage(pathprefix, path))
                {
                    using (Bitmap bmp = ImageUtilities.ResizePictureSquare(thmbnail, size, size, Color.White))
                    {
                        if (bmp != null)
                        {
                            byte[] output = ImageUtilities.TransformImageToByte(bmp, "jpg");
                            return File(output, Utilities.GetProperContentType("jpg", true), Path.GetFileName(pathprefix + path));
                        }
                    }
                }

            }
            else
            {
                Response.StatusCode = 404;
                return null;
            }
            return null;
        }


        [Route("previewcustomsize/{width}/{height}/{*path}")]
        public FileContentResult PreviewCustomSize(int width, int height, string path)
        {
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.Private);
            HttpContext.Response.Cache.SetMaxAge(new TimeSpan(1, 0, 0));
            HttpContext.Response.Cache.SetExpires(DateTime.Now.AddDays(1));
            HttpContext.Response.Cache.SetNoServerCaching();
            HttpContext.Response.ExpiresAbsolute = DateTime.Now.AddDays(1);
            string pathprefix = PathProvider.GetUserFilesPath(this.User.Identity.Name);
            if (System.IO.File.Exists(pathprefix + path) || System.IO.Directory.Exists(pathprefix + path))
            {

                DateTime lastModifiedDate = DateTime.Now;
                if (System.IO.File.Exists(pathprefix + path))
                {
                    string previewPath = PathProvider.GetUserDataPath(this.User.Identity.Name) + FileManagerConsts.FilesPreviewsPath + path;
                    previewPath = Path.GetDirectoryName(previewPath) + "/" + Path.GetFileNameWithoutExtension(previewPath) + ".png";

                    lastModifiedDate = new System.IO.FileInfo(previewPath).LastWriteTime;
                }
                if (System.IO.Directory.Exists(pathprefix + path))
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

                if (System.IO.File.Exists(pathprefix + path))
                {
                    //if it is a file try get pregenerated preview first. if it fails generate preview old way
                    string previewPath = PathProvider.GetUserDataPath(this.User.Identity.Name) + FileManagerConsts.FilesPreviewsPath + path;
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
                                    return File(output, Utilities.GetProperContentType("jpg", true), Path.GetFileName(pathprefix + path));
                                }
                            }
                        }
                    }
                }
                //we cannot get pregenerated preview so we need to generate it from scratch
                using (Bitmap thmbnail = FileService.GenerateThumbnailToImage(pathprefix, path))
                {
                    using (Bitmap bmp = ImageUtilities.ResizePictureSquare(thmbnail, width, height, Color.White))
                    {
                        if (bmp != null)
                        {
                            byte[] output = ImageUtilities.TransformImageToByte(bmp, "jpg");
                            return File(output, Utilities.GetProperContentType("jpg", true), Path.GetFileName(pathprefix + path));
                        }
                    }
                }

            }
            else
            {
                Response.StatusCode = 404;
            }
            return null;
        }

        [Route("uploadfile")]
        public async Task<string> UploadFile()
        {
            Stream fs = Request.Files[0].InputStream;
            byte[] filechunk = new byte[fs.Length];
            int streamLengths = await fs.ReadAsync(filechunk, 0, filechunk.Length);
            var chunks = int.Parse(Request["chunks"]);
            var chunk = int.Parse(Request["chunk"]);
            var name = Request["name"];

            string uploadPath = PathProvider.GetUserUploadFolder(this.User.Identity.Name);
            System.IO.File.WriteAllBytes(uploadPath+"/"+chunk.ToString() + "_" + name, filechunk);
            if (chunk >= chunks - 1)
            {
                DataCache cache = await DataCacheService.GetDataCache(this.User.Identity.Name);
                string pathprefix = PathProvider.GetUserFilesPath(this.User.Identity.Name);
                string filePath = ((cache.Data.Keys.Contains("fileManagerPath") && cache["fileManagerPath"] != null) ? cache["fileManagerPath"].ToString() : "");
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
                        string previewPath = PathProvider.GetUserDataPath(this.User.Identity.Name) + FileManagerConsts.FilesPreviewsPath + filePath;
                        Directory.CreateDirectory(previewPath);
                        previewPath = previewPath + "/" + Path.GetFileNameWithoutExtension(pathprefix + filePath + "/" + name) + ".png";
                        System.IO.File.WriteAllBytes(previewPath, preview);
                    }
                }
            }

            return "{'OK': 1, 'info': 'Upload successful.'}";
        }

        [Route("get/{*path}")]
        public FilePathResult DownloadFile(string path)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                string pathprefix = PathProvider.GetUserFilesPath(this.User.Identity.Name);
                if (System.IO.File.Exists(pathprefix + path))
                {

                    return File(pathprefix + path, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(pathprefix + path));
                }
            }
            return null;
        }

        [Route("zip/{name}")]
        public ZipDownloadResult DownloadZip(string name)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                string pathprefix = PathProvider.GetUserDataPath(this.User.Identity.Name) + FileManagerConsts.CompressedFilesPath;
                Directory.CreateDirectory(pathprefix);

                return new ZipDownloadResult(pathprefix + name);
            }
            return null;
        }
        
    }
}