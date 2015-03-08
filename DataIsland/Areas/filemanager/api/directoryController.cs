using dataislandcommon.Services.System;
using dataislandcommon.Attributes;
using dataislandcommon.Models.DataCache;
using dataislandcommon.Services.FileSystem;
using dimain.Services.System;
using dimain.Services.System.Cache;
using FileManager.Models.ViewModels;
using FileManager.Services.FileManager;
using FileManager.Utilities;

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DataIsland.Areas.filemanager.api
{
    [Authorize]
    [RoutePrefix("api/filemanager/directory")]
    public class directoryController : ApiController
    {
        
        public IFilePathProviderService PathProvider { get; set; }

        
        public IDataCacheService DataCacheService { get; set; }

        private readonly IDirectoryService DirectoryService;

        public directoryController(IDirectoryService _DirectoryService)
        {
            DirectoryService = _DirectoryService;
        }

        [Route("listdirectory/{*path}")]
        [HttpGet]
        public async Task<List<DiDirectoryListingEntry>> ListDirectory(string path)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                string pathprefix = PathProvider.GetUserFilesPath(this.User.Identity.Name);
                DataCache cache = await DataCacheService.GetDataCache(this.User.Identity.Name);
                cache["fileManagerPath"] = path;
                await DataCacheService.SaveDataCache(cache);
                return DirectoryService.ListDirectory(pathprefix, path);
            }

            return null;
        }

        [Route("delete")]
        [HttpPost]
        public object DeleteDirectories(List<string> directories)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                string pathprefix = PathProvider.GetUserFilesPath(this.User.Identity.Name);
                string previewPath = PathProvider.GetUserDataPath(this.User.Identity.Name) + FileManagerConsts.FilesPreviewsPath;
                foreach (string dirName in directories)
                {
                    DirectoryService.DeleteDirectory(pathprefix,previewPath, dirName, true);
                }
                return new { result = true };
            }
            return new { result = false };
        }

        [Route("create")]
        [HttpPost]
        public object CreateDirectory(JObject jsonData)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                dynamic postData = jsonData;
                string path = postData.path;
                string pathprefix = PathProvider.GetUserFilesPath(this.User.Identity.Name);
                if (DirectoryService.CreateDirectory(pathprefix, path)!=null)
                {
                    return new { result = true };
                }
            }
            return new { result = false }; 
        }
    }
}
