using dilib.Services.Communication;
using dimain.Services.System;
using FileManager.Models.db;
using FileManager.Services.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Services.FileManager
{
    public class SharedResourcesService : ISharedResourcesService
    {
        public IFileDatabaseManagerSingleton DbManager { get; set; }
        public IDbResourceRecipientService Recipients { get; set; }
        public IDbSharedResourcesService Resources { get; set; }
        public IDiUserService DiUsers { get; set; }
        public IDICommandsService Commands { get; set; }
        public IDbForeignSharedResourcesService ForeignResources { get; set; }

        public SharedResourcesService()
        {

        }

        public async Task<List<SharedResource>> GetSelectedResourcesOptions(List<string> fullPaths, string ownerUsername)
        {
            List<SharedResource> resources = new List<SharedResource>();
            if (fullPaths != null)
            {
                using (var db = this.DbManager.GetDbContext(ownerUsername))
                {
                    resources = await this.Resources.GetResources(fullPaths, db);
                    if (resources != null && resources.Count > 0)
                    {
                        foreach (SharedResource res in resources)
                        {
                            res.Recipients = await this.Recipients.GetRecipients(res.ID, db);
                        }
                    }
                }
            }
            return resources;
        }

        public async Task<bool> SetResources(List<SharedResource> sharedResources, string ownerUsername)
        {
            if (sharedResources != null && sharedResources.Count > 0)
            {
                string ownerUserId = await this.DiUsers.GetUserIdByFromUsername(ownerUsername);
                using (var db = this.DbManager.GetDbContext(ownerUsername))
                {
                    foreach(SharedResource res in sharedResources)
                    {
                        List<ResourceRecipient> currentRecipients = null;
                        if (!string.IsNullOrEmpty(res.ID))
                        {
                            currentRecipients = await this.Recipients.GetRecipients(res.ID, db);
                            await this.Resources.DeleteResource(res.FullPath, db);
                            await this.Recipients.DeleteRecipients(res.ID, db);
                        }

                        if (currentRecipients != null && currentRecipients.Count > 0)
                        {
                            foreach (var currentRecipient in currentRecipients)
                            {
                                bool exists = false;
                                if (res.Recipients != null && res.Recipients.Count > 0)
                                {
                                    foreach (var rec in res.Recipients)
                                    {
                                        if(rec.RecipientID == currentRecipient.RecipientID)
                                        {
                                            exists = true;
                                        }
                                    }
                                }
                                if(!exists)
                                {
                                    this.Commands.User(ownerUserId, currentRecipient.RecipientID).DeleteForeignResource(res.ID);
                                }
                            }
                        }

                        if (res.Recipients != null && res.Recipients.Count > 0 && (res.IsAll || res.IsPublic || res.IsRead || res.IsWrite))
                        {
                            string resId = await this.Resources.AddResource(res.FullPath, res.IsDirectory, res.IsRead, res.IsWrite, res.IsAll, res.IsPublic, db);
                            List<string> pathParts = res.FullPath.Split("/".ToCharArray(),StringSplitOptions.RemoveEmptyEntries).ToList();
                            string resourceName = pathParts[pathParts.Count-1];
                            foreach (ResourceRecipient rec in res.Recipients)
                            {
                                await this.Recipients.AddRecipient(resId, rec.RecipientID, false, db);

                                bool recipientExists = false;
                                if(currentRecipients!=null && currentRecipients.Count>0)
                                {
                                    foreach(var currentRecipient in currentRecipients)
                                    {
                                        if(currentRecipient.RecipientID == rec.RecipientID)
                                        {
                                            recipientExists = true;
                                        }
                                    }
                                }

                                if (!recipientExists)
                                {
                                    this.Commands.User(ownerUserId, rec.RecipientID).AddForeignResource(resId, ownerUserId, resourceName, res.IsDirectory);
                                }
                            }
                        }
                    }
                }

                return true;
            }
            return false;
        }

        public async Task<List<SharedResource>> GetSharedResources(string ownerUsername)
        {
            List<SharedResource> resources = null;
            using (var db = this.DbManager.GetDbContext(ownerUsername))
            {
                resources = await this.Resources.GetResources(db);
                foreach(var res in resources)
                {
                    res.Recipients = await this.Recipients.GetRecipients(res.ID, db);
                }
            }
            if (resources == null)
            {
                resources = new List<SharedResource>();
            }
            return resources;
        }

        public async Task<SharedResource> GetSharedResourceByID(string id, string ownerUsername)
        {
            using(var db = this.DbManager.GetDbContext(ownerUsername))
            {
                SharedResource res = await this.Resources.GetResourceByID(id, db);
                return res;
            }
        }

        public async Task AddForeignSharedResource(string id, string ownerId, string name, bool isDirectory, string ownerUsername)
        {
            using (var db = this.DbManager.GetDbContext(ownerUsername))
            {
                await this.ForeignResources.AddResource(id, ownerId, name, isDirectory, db);
            }
        }

        public async Task DeleteForeignSharedResources(string id, string ownerUsername)
        {
            using (var db = this.DbManager.GetDbContext(ownerUsername))
            {
                await this.ForeignResources.DeleteResource(id, db);
            }
        }

        public async Task<List<ForeignSharedResource>> GetForeignResources(string ownerUsername)
        {
            List<ForeignSharedResource> resources = null;
            using (var db = this.DbManager.GetDbContext(ownerUsername))
            {
                resources = await this.ForeignResources.GetSharedResources(db);
            }
            return resources;
        }

        public async Task<bool> CheckRecipientExists(string resourceId, string recipientId, string ownerUsername)
        {
            using (var db = this.DbManager.GetDbContext(ownerUsername))
            {
                bool res = await this.Recipients.CheckRecipientExists(resourceId, recipientId, db);
                return res;
            }
        }


    }
}
