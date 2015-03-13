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
    public class DbSharedResourcesService : IDbSharedResourcesService
    {
        private readonly IFileDatabaseManagerSingleton _dbMngr;
        public DbSharedResourcesService(IFileDatabaseManagerSingleton manager)
        {
            this._dbMngr = manager;
        }

        public async Task<bool> CheckResourceExist(string fullPath, DiFileContext db)
        {
            var res = await db.SharedResources.Where(x => x.FullPath == fullPath).CountAsync();
            if(res>0)
            {
                return true;
            }

            return false;
        }

        public async Task<string> AddResource(string fullPath, bool isDirectory, bool isRead, bool isWrite, bool isFull, bool isPublic, DiFileContext db)
        {
            if (!await this.CheckResourceExist(fullPath, db))
            {
                SharedResource res = new SharedResource();
                res.FullPath = fullPath;
                res.ID = Guid.NewGuid().ToString();
                res.IsAll = isFull;
                res.IsDirectory = isDirectory;
                res.IsPublic = isPublic;
                res.IsRead = isRead;
                res.IsWrite = isWrite;

                db.SharedResources.Add(res);

                await db.SaveChangesAsync();

                return res.ID;
            }
            else
            {
                return await this.ModifyResource(fullPath, isDirectory, isRead, isWrite, isFull, isPublic, db);
            }
        }

        public async Task<List<SharedResource>> GetResources(List<string> fullPaths, DiFileContext db)
        {
            var res = await db.SharedResources.Where(x => fullPaths.Contains(x.FullPath)).ToListAsync();
            return res;
        }

        public async Task<SharedResource> GetResource(string fullPath, DiFileContext db)
        {
            var res = await db.SharedResources.Where(x => x.FullPath == fullPath).SingleOrDefaultAsync();
            return res;
        }

        public async Task<string> ModifyResource(string fullPath, bool isDirectory, bool isRead, bool isWrite, bool isFull, bool isPublic, DiFileContext db)
        {
            var res = await db.SharedResources.Where(x => x.FullPath == fullPath).SingleOrDefaultAsync();
            if(res!=null)
            {
                res.FullPath = fullPath;
                res.IsAll = isFull;
                res.IsDirectory = isDirectory;
                res.IsPublic = isPublic;
                res.IsRead = isRead;
                res.IsWrite = isWrite;

                await db.SaveChangesAsync();

                return res.ID;
            }
            return String.Empty;
        }

        public async Task<bool> DeleteResource(string fullPath, DiFileContext db)
        {
            int count = await db.Database.ExecuteSqlCommandAsync("DELETE FROM SharedResource WHERE FullPath=\"" + fullPath + "\"");
            if(count>0)
            {
                await db.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
