using FileManager.Models.db;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace FileManager.Services.FileManager
{
    public interface ISharedResourcesService
    {
        Task<List<SharedResource>> GetSelectedResourcesOptions(List<string> fullPaths, string ownerUsername);
        Task<bool> SetResources(List<SharedResource> sharedResources, string ownerUsername);
    }
}
