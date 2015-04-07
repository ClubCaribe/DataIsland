using FileManager.Classes.db;
using FileManager.Models.db;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace FileManager.Services.db
{
    public interface IDbForeignSharedResourcesService
    {
        Task<bool> AddResource(string id, string ownerId, string name, bool isDirectory, DiFileContext db);
        Task<bool> CheckResourceExists(string id, DiFileContext db);
        Task<bool> DeleteResource(string id, DiFileContext db);
        Task<List<ForeignSharedResource>> GetSharedResources(DiFileContext db);
        Task<ForeignSharedResource> GetSharedResource(string id, DiFileContext db);
    }
}
