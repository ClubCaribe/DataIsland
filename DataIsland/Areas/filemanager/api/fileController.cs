using dataislandcommon.Attributes;
using dataislandcommon.Services.FileSystem;
using dataislandcommon.Services.Utilities;
using FileManager.Models.ViewModels;
using FileManager.Services.FileManager;
using FileManager.Utilities;

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DataIsland.Areas.filemanager.api
{
    [Authorize]
    [RoutePrefix("api/filemanager/file")]
    public class fileController : ApiController
    {
        
        public IFilePathProviderService PathProvider { get; set; }

        
        public IImageUtilitiesSingleton ImageUtilities { get; set; }

        
        public IFileManagerService FileManager { get; set; }

        
        public ITaskDispatcherSingleton TaskDispatcher { get; set; }

        private readonly IFileService FileService;

        public fileController(IFileService fileService)
        {
            FileService = fileService;
        }

        [Route("delete")]
        [HttpPost]
        public object DeleteFiles(List<string> filenames)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                string pathprefix = PathProvider.GetUserFilesPath(this.User.Identity.Name);
                string previewPath = PathProvider.GetUserDataPath(this.User.Identity.Name) + FileManagerConsts.FilesPreviewsPath;
                foreach(var filename in filenames){
                    FileService.DeleteFile(pathprefix,previewPath, filename);
                }
                return new { result = true };
                
            }
            return new { result = false };
        }

        [Route("compress")]
        [HttpPost]
        public object CompressFiles(List<string> files)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                string pathprefix = PathProvider.GetUserFilesPath(this.User.Identity.Name);
                string tempPath = PathProvider.GetUserDataPath(this.User.Identity.Name) + FileManagerConsts.CompressedFilesPath;
                Directory.CreateDirectory(tempPath);
                string zipName = Guid.NewGuid().ToString();
                Task.Factory.StartNew(() => { FileManager.ZipFilesForDownload(this.User.Identity.Name, pathprefix, files, tempPath + zipName + ".zip"); });
                return new { name = zipName + ".zip" };
            }
            return new { name = "error" };
        }

        [Route("cancelcompressing/{filename}")]
        [HttpGet]
        public object CancelCompressingFile(string filename)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                IProgressManager imanager = TaskDispatcher.GetCompressingProgressManager(this.User.Identity.Name, filename);
                if(imanager!=null)
                {
                    ProgressManager manager = (ProgressManager)imanager;
                    manager.CancelOperation();
                    return new { result = true };
                }
                
            }
            return new { result = false };
        }

        [Route("renamecompressedfile/{filename}/{newfilename}")]
        [HttpGet]
        public object RenameCompressedFile(string filename, string newfilename)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                string tempPath = PathProvider.GetUserDataPath(this.User.Identity.Name) + FileManagerConsts.CompressedFilesPath;
                Directory.CreateDirectory(tempPath);
                if (File.Exists(tempPath + filename))
                {
                    IProgressManager imanager = TaskDispatcher.GetCompressingProgressManager(this.User.Identity.Name, filename);
                    if (imanager != null)
                    {
                        ProgressManager manager = (ProgressManager)imanager;
                        manager.NewFileName = newfilename;
                        return new { result = true };
                    }
                    else
                    {

                        File.Move(tempPath + filename, tempPath + newfilename);
                        if (File.Exists(tempPath + newfilename))
                        {
                            return new { result = true };
                        }
                    }
                }
            }
            return new { result = false };
        }

        [Route("getcompressedfiles")]
        public List<DiCompressedFile> GetCompressedFiles()
        {
            List<DiCompressedFile> files = new List<DiCompressedFile>();

            if (this.User.Identity.IsAuthenticated)
            {
                string tempPath = PathProvider.GetUserDataPath(this.User.Identity.Name) + FileManagerConsts.CompressedFilesPath;
                Directory.CreateDirectory(tempPath);
                files = FileManager.GetCompressedFiles(tempPath);
            }

            return files;
        }

        [Route("deletecompressedfile/{filename}")]
        [HttpGet]
        public object DeleteCompressedFile(string filename)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                string path = PathProvider.GetUserDataPath(this.User.Identity.Name) + FileManagerConsts.CompressedFilesPath;
                Directory.CreateDirectory(path);
                File.Delete(path + filename);
                if (!File.Exists(path + filename))
                {
                    return new { result = true };
                }
            }
            return new { result = false };
        }

        [Route("move")]
        [HttpPost]
        public object Move(JObject jsonData)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                dynamic postData = jsonData;
                string oldFilePath = postData.OldFileName;
                string newFilePath = postData.NewFileName;
                string pathprefix = PathProvider.GetUserFilesPath(this.User.Identity.Name);
                if (File.Exists(pathprefix + oldFilePath) || Directory.Exists(pathprefix + oldFilePath))
                {
                    string previewPath = PathProvider.GetUserDataPath(this.User.Identity.Name) + FileManagerConsts.FilesPreviewsPath;
                    if (File.Exists(pathprefix + oldFilePath))
                    {
                        string oldFileNamePreview = previewPath + oldFilePath;
                        string newFileNamePreview = previewPath + newFilePath;
                        oldFileNamePreview = Path.ChangeExtension(oldFileNamePreview, ".png");
                        newFileNamePreview = Path.ChangeExtension(newFileNamePreview, ".png");
                        File.Move(pathprefix + oldFilePath, pathprefix + newFilePath);
                        File.Move(oldFileNamePreview, newFileNamePreview);
                        if (File.Exists(pathprefix + newFilePath))
                        {
                            return new { result = true };
                        }
                    }
                    if (Directory.Exists(pathprefix + oldFilePath))
                    {
                        string oldFileNamePreview = previewPath + oldFilePath;
                        string newFileNamePreview = previewPath + newFilePath;
                        Directory.Move(pathprefix + oldFilePath, pathprefix + newFilePath);
                        Directory.Move(oldFileNamePreview, newFileNamePreview);
                        if (Directory.Exists(pathprefix + newFilePath))
                        {
                            return new { result = true };
                        }
                    }
                }
            }
            return new { result = false };
        }

        [Route("copy")]
        [HttpPost]
        public object Copy(JObject jsonData)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                dynamic postData = jsonData;
                string oldFilePath = postData.OldFileName;
                string newFilePath = postData.NewFileName;
                string pathprefix = PathProvider.GetUserFilesPath(this.User.Identity.Name);
                File.Copy(pathprefix + oldFilePath, pathprefix + newFilePath);
                if (File.Exists(pathprefix + newFilePath))
                {
                    return new { result = true };
                }
            }
            return new { result = false };
        }

        [Route("rotateflip")]
        [HttpPost]
        public object RotateFlipPreview(JObject jsonData)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                dynamic postData = jsonData;
                string path = postData.Path;
                string transformType = postData.TransformType;
                bool Result = FileManager.RotateFlipPreview(this.User.Identity.Name, path, transformType);
                return new { result = Result };
            }
            return new { result = false };
        }

    }
}
