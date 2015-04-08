using FileManager.Classes.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using FileManager.Models.db;

namespace FileManager.Services.db
{
    public class DbForeignSharedResourcesService : IDbForeignSharedResourcesService
    {
        public DbForeignSharedResourcesService()
        {

        }

        public async Task<bool> CheckResourceExists(string id, DiFileContext db)
        {
            var res = await db.ForeignResources.Where(x => x.ID == id).ToListAsync();
            if (res.Count > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> AddResource(string id, string ownerId, string name, bool isDirectory, DiFileContext db)
        {
            if (!await this.CheckResourceExists(id, db))
            {
                ForeignSharedResource resource = new ForeignSharedResource();
                resource.ID = id;
                resource.IsAccessible = true;
                resource.IsDirectory = isDirectory;
                resource.Name = name;
                resource.OwnerID = ownerId;
                db.ForeignResources.Add(resource);
                await db.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> DeleteResource(string id, DiFileContext db)
        {
            int count = await db.Database.ExecuteSqlCommandAsync("DELETE FROM ForeignSharedResource WHERE ID=\"" + id + "\"");
            if (count > 0)
            {
                await db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<ForeignSharedResource>> GetSharedResources(DiFileContext db)
        {
            var res = await db.ForeignResources.ToListAsync();
            return res;
        }

        public async Task<ForeignSharedResource> GetSharedResource(string id, DiFileContext db)
        {
            var res = await db.ForeignResources.Where(x => x.ID == id).SingleOrDefaultAsync();
            return res;
        }

        public async Task<List<ForeignSharedResource>> GetSharedResourcesByUserId(string userId, DiFileContext db)
        {
            var res = await db.ForeignResources.Where(x => x.OwnerID == userId).ToListAsync();
            return res;
        }
    }
}
