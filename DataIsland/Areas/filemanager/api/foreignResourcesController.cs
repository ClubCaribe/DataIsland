using dataislandcommon.Models.DataCache;
using dataislandcommon.Services.FileSystem;
using dataislandcommon.Services.Utilities;
using dimain.Services.System;
using dimain.Services.System.Cache;
using FileManager.Models.db;
using FileManager.Models.ViewModels;
using FileManager.Services.FileManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DataIsland.Areas.filemanager.api
{
    [RoutePrefix("api/filemanager/foreignresources")]
    public class foreignResourcesController : ApiController
    {
        public IFilePathProviderService PathProvider { get; set; }

        public IDataCacheService DataCacheService { get; set; }

        public IDiUserService DiUsers { get; set; }

        public ISharedResourcesService SharedResources { get; set; }

        public IUtilitiesSingleton Utilities { get; set; }

        private readonly IDirectoryService DirectoryService;

        public foreignResourcesController(IDirectoryService dirService)
        {
            this.DirectoryService = dirService;
        }

        [Route("listdirectory/{userId}/{resourceId}/{*path}")]
        [HttpGet]
        public async Task<List<DiDirectoryListingEntry>> ListDirectory(string userId, string resourceId, string path)
        {
            string ownerUsername = await this.DiUsers.GetUsernameFromUserId(this.Utilities.UnescapeUserId(userId));
            if (!string.IsNullOrEmpty(ownerUsername))
            {
                SharedResource sharedResource = await this.SharedResources.GetSharedResourceByID(this.Utilities.UnescapeUserId(resourceId), ownerUsername);
                if (sharedResource != null)
                {
                    string pathprefix = PathProvider.GetUserFilesPath(ownerUsername);
                    DataCache cache = await DataCacheService.GetDataCache(ownerUsername);
                    cache["fileManagerPath:" + resourceId] = path;
                    await DataCacheService.SaveDataCache(cache);
                    string fullPath = sharedResource.FullPath + "/" + path;
                    return await DirectoryService.ListDirectory(pathprefix, fullPath, ownerUsername);
                }
            }
            return null;
        }
    }
}