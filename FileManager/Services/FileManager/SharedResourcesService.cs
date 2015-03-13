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
                using (var db = this.DbManager.GetDbContext(ownerUsername))
                {
                    foreach(SharedResource res in sharedResources)
                    {
                        if (!string.IsNullOrEmpty(res.ID))
                        {
                            await this.Resources.DeleteResource(res.FullPath, db);
                            await this.Recipients.DeleteRecipients(res.ID, db);
                        }

                        if (res.Recipients != null && res.Recipients.Count > 0 && (res.IsAll || res.IsPublic || res.IsRead || res.IsWrite))
                        {
                            string resId = await this.Resources.AddResource(res.FullPath, res.IsDirectory, res.IsRead, res.IsWrite, res.IsAll, res.IsPublic, db);

                            foreach (ResourceRecipient rec in res.Recipients)
                            {
                                await this.Recipients.AddRecipient(resId, rec.RecipientID, false, db);
                            }
                        }
                    }
                }

                return true;
            }
            return false;
        }


    }
}
