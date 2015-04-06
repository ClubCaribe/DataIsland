using dataislandcommon.Models.DataCache;
using dataislandcommon.Services.FileSystem;
using dataislandcommon.Services.Utilities;
using dimain.Services.System;
using dimain.Services.System.Cache;
using FileManager.Models.db;
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
    [RoutePrefix("api/filemanager/foreignresources")]
    public class foreignResourcesController : ApiController
    {
        public IFilePathProviderService PathProvider { get; set; }

        public IDataCacheService DataCacheService { get; set; }

        public IDiUserService DiUsers { get; set; }

        public ISharedResourcesService SharedResources { get; set; }

        public IUtilitiesSingleton Utilities { get; set; }

        public IUserPassportTokensSingleton PassportTokenSingleton { get; set; }

        private readonly IDirectoryService DirectoryService;

        public foreignResourcesController(IDirectoryService dirService)
        {
            this.DirectoryService = dirService;
        }

        [Route("listdirectory/{userId}/{resourceId}/{*path}")]
        [HttpGet]
        public async Task<List<DiDirectoryListingEntry>> ListDirectory(string userId, string resourceId, string path)
        {
            string callingUser = this.User.Identity.Name;
            callingUser = await this.DiUsers.GetUserIdByFromUsername(callingUser);
            if (!string.IsNullOrEmpty(callingUser))
            {
                string ownerUsername = await this.DiUsers.GetUsernameFromUserId(this.Utilities.UnescapeUserId(userId));
                if (!string.IsNullOrEmpty(ownerUsername))
                {
                    if (await this.SharedResources.CheckRecipientExists(this.Utilities.UnescapeUserId(resourceId), callingUser, ownerUsername))
                    {
                        SharedResource sharedResource = await this.SharedResources.GetSharedResourceByID(this.Utilities.UnescapeUserId(resourceId), ownerUsername);
                        if (sharedResource != null)
                        {

                            string pathprefix = PathProvider.GetUserFilesPath(ownerUsername);
                            DataCache cache = await DataCacheService.GetDataCache(ownerUsername);
                            cache["fileManagerPath:" + resourceId] = path;
                            await DataCacheService.SaveDataCache(cache);
                            string fullPath = sharedResource.FullPath + "/" + path;
                            List<DiDirectoryListingEntry> entries = await DirectoryService.ListDirectory(pathprefix, fullPath, ownerUsername);
                            if (entries != null)
                            {
                                foreach (DiDirectoryListingEntry entry in entries)
                                {
                                    entry.FullName = entry.FullName.Replace(sharedResource.FullPath, "");
                                    if (entry.IsDirectory)
                                    {
                                        DiDirectoryInfo diInf = (DiDirectoryInfo)entry.FileSystemObject;
                                        diInf.FullName = diInf.FullName.Replace(sharedResource.FullPath, "");
                                    }
                                }
                            }
                            return entries;
                        }
                    }
                }
            }
            return null;
        }

        [Route("getpermissions/{userId}/{resourceId}/{*path}")]
        [HttpGet]
        public async Task<Dictionary<string,bool>> GetResourcePermissions(string userId, string resourceId)
        {
            Dictionary<string, bool> permissions = new Dictionary<string, bool>();
            permissions["Read"] = false;
            permissions["Write"] = false;
            permissions["All"] = false;

            string callingUser = this.User.Identity.Name;
            callingUser = await this.DiUsers.GetUserIdByFromUsername(callingUser);
            if (!string.IsNullOrEmpty(callingUser))
            {
                string ownerUsername = await this.DiUsers.GetUsernameFromUserId(this.Utilities.UnescapeUserId(userId));
                if (!string.IsNullOrEmpty(ownerUsername))
                {
                    if (await this.SharedResources.CheckRecipientExists(this.Utilities.UnescapeUserId(resourceId), callingUser, ownerUsername))
                    {
                        SharedResource sharedResource = await this.SharedResources.GetSharedResourceByID(this.Utilities.UnescapeUserId(resourceId), ownerUsername);
                        if (sharedResource != null)
                        {
                            permissions["Read"] = sharedResource.IsRead;
                            permissions["Write"] = sharedResource.IsWrite;
                            permissions["All"] = sharedResource.IsAll;
                        }
                    }
                }
            }
            return permissions;
        }

        [Route("move")]
        [HttpPost]
        public async Task<bool> MoveFile(JObject jsonData)
        {
            dynamic postData = jsonData;
            string userId, resourceId;
            userId = postData.userId;
            resourceId = postData.resourceId;

            string callingUser = this.User.Identity.Name;
            callingUser = await this.DiUsers.GetUserIdByFromUsername(callingUser);
            if (!string.IsNullOrEmpty(callingUser))
            {
                string ownerUsername = await this.DiUsers.GetUsernameFromUserId(this.Utilities.UnescapeUserId(userId));
                if (!string.IsNullOrEmpty(ownerUsername))
                {
                    if (await this.SharedResources.CheckRecipientExists(this.Utilities.UnescapeUserId(resourceId), callingUser, ownerUsername))
                    {
                        SharedResource sharedResource = await this.SharedResources.GetSharedResourceByID(this.Utilities.UnescapeUserId(resourceId), ownerUsername);
                        if (sharedResource != null)
                        {
                            if (this.User.Identity.IsAuthenticated)
                            {
                                string oldFilePath = postData.OldFileName;
                                string newFilePath = postData.NewFileName;
                                string pathprefix = PathProvider.GetUserFilesPath(ownerUsername);
                                if (File.Exists(pathprefix + sharedResource.FullPath + "/" + oldFilePath) || Directory.Exists(pathprefix + sharedResource.FullPath + "/" + oldFilePath))
                                {
                                    string previewPath = PathProvider.GetUserDataPath(this.User.Identity.Name) + FileManagerConsts.FilesPreviewsPath;
                                    if (File.Exists(pathprefix + sharedResource.FullPath + "/" + oldFilePath))
                                    {
                                        string oldFileNamePreview = previewPath + sharedResource.FullPath + "/" + oldFilePath;
                                        string newFileNamePreview = previewPath + sharedResource.FullPath + "/" + newFilePath;
                                        oldFileNamePreview = Path.ChangeExtension(oldFileNamePreview, ".png");
                                        newFileNamePreview = Path.ChangeExtension(newFileNamePreview, ".png");
                                        File.Move(pathprefix + sharedResource.FullPath + "/" + oldFilePath, pathprefix + sharedResource.FullPath + "/" + newFilePath);
                                        File.Move(oldFileNamePreview, newFileNamePreview);
                                        if (File.Exists(pathprefix + sharedResource.FullPath + "/" + newFilePath))
                                        {
                                            return true;
                                        }
                                    }
                                    if (Directory.Exists(pathprefix + sharedResource.FullPath + "/" + oldFilePath))
                                    {
                                        string oldFileNamePreview = previewPath + sharedResource.FullPath + "/" + oldFilePath;
                                        string newFileNamePreview = previewPath + sharedResource.FullPath + "/" + newFilePath;
                                        Directory.Move(pathprefix + sharedResource.FullPath + "/" + oldFilePath, pathprefix + sharedResource.FullPath + "/" + newFilePath);
                                        Directory.Move(oldFileNamePreview, newFileNamePreview);
                                        if (Directory.Exists(pathprefix + sharedResource.FullPath + "/" + newFilePath))
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}