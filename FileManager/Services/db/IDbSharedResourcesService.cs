using FileManager.Classes.db;
using FileManager.Models.db;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace FileManager.Services.db
{
    public interface IDbSharedResourcesService
    {
        Task<string> AddResource(string fullPath, bool isDirectory, bool isRead, bool isWrite, bool isFull, bool isPublic, DiFileContext db);
        Task<bool> CheckResourceExist(string fullPath, DiFileContext db);
        Task<bool> DeleteResource(string fullPath, DiFileContext db);
        Task<List<SharedResource>> GetResources(List<string> fullPaths, DiFileContext db);
        Task<List<SharedResource>> GetResources(DiFileContext db);
        Task<SharedResource> GetResource(string fullPath, DiFileContext db);
        Task<string> ModifyResource(string fullPath, bool isDirectory, bool isRead, bool isWrite, bool isFull, bool isPublic, DiFileContext db);
    }
}
