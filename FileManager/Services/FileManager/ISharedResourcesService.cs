﻿using FileManager.Models.db;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace FileManager.Services.FileManager
{
    public interface ISharedResourcesService
    {
        Task<List<SharedResource>> GetSelectedResourcesOptions(List<string> fullPaths, string ownerUsername);
        Task<bool> SetResources(List<SharedResource> sharedResources, string ownerUsername);
        Task<List<SharedResource>> GetSharedResources(string ownerUsername);
        Task<SharedResource> GetSharedResourceByID(string id, string ownerUsername);
        Task AddForeignSharedResource(string id, string ownerId, string name, bool isDirectory, string ownerUsername);
        Task DeleteForeignSharedResources(string id, bool sendDeleteCommand, string ownerUsername);
        Task<List<ForeignSharedResource>> GetForeignResources(string ownerUsername);
        Task<bool> CheckRecipientExists(string resourceId, string recipientId, string ownerUsername);
        Task RemoveRecipientFromSharedResource(string resourceId, string userId, string ownerUsername);
        Task<List<ForeignSharedResource>> GetForeignResourcesByUserId(string userId, string ownerUsername);
    }
}
