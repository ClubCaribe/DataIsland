using FileManager.Models.db;
using FileManager.Services.FileManager;
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
    [RoutePrefix("api/filemanager/shared")]
    public class sharedResourcesController : ApiController
    {
        private readonly ISharedResourcesService Resources;
        public sharedResourcesController(ISharedResourcesService _resources)
        {
            this.Resources = _resources;
        }

        [Route("getresourcesoptions")]
        [HttpPost]
        public async Task<List<SharedResource>> GetResourcesOptions(List<string> filenames)
        {
            List<SharedResource> resources = await this.Resources.GetSelectedResourcesOptions(filenames, this.User.Identity.Name);
            return resources;
        }

        [Route("setresourcesoptions")]
        [HttpPost]
        public async Task<bool> SetResourceOptions(List<SharedResource> sharedResources)
        {
            return await this.Resources.SetResources(sharedResources, this.User.Identity.Name);
        }

        [Route("getresourcessharedbyme")]
        [HttpGet]
        public async Task<List<SharedResource>> GetResourcesSharedByMe()
        {
            return await this.Resources.GetSharedResources(this.User.Identity.Name);
        }

        [Route("getresourcessharedbyothers")]
        [HttpGet]
        public async Task<List<ForeignSharedResource>> GetResourcesSharedByOthers()
        {
            return await this.Resources.GetForeignResources(this.User.Identity.Name);
        }

        [Route("getforeignresourcesbyuserid")]
        [HttpPost]
        public async Task<List<ForeignSharedResource>> GetForeignResourcesByUserID(JObject data)
        {
            dynamic postData = data;
            string userId = postData.userId;
            return await this.Resources.GetForeignResourcesByUserId(userId, this.User.Identity.Name);
        }

        [Route("deletesharedresource/{id}")]
        [HttpGet]
        public async Task<bool> DeleteSharedResource(string id)
        {
            await this.Resources.DeleteForeignSharedResources(id, true, this.User.Identity.Name);
            return true;
        }
    }
}