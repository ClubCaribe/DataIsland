using FileManager.Classes.db;
using FileManager.Models.db;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace FileManager.Services.db
{
    public interface IDbResourceRecipientService
    {
        Task<bool> AddRecipient(string resourceId, string recipientId, bool deleted, DiFileContext db);
        Task<bool> CheckRecipientExists(string resourceId, string recipientId, DiFileContext db);
        Task<bool> DeleteRecipient(string resourceId, string recipientId, DiFileContext db);
        Task<bool> DeleteRecipients(string resourceId, DiFileContext db);
        Task<List<ResourceRecipient>> GetRecipients(string resourceId, DiFileContext db);
        Task<bool> UpdateRecipient(string resourceId, string recipientId, bool deleted, DiFileContext db);
    }
}
