using Streamail.Classes.db;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Streamail.Services.db
{
    public interface IDbMessageCustomFiledsService
    {
        Task<bool> AddMessageCustomFields(Dictionary<string, object> customFields, string messageId, string ownerUsername);
        Task DeleteCustomFileds(string messageId, DiStreamailContext db);
        Task<Dictionary<string, object>> GetCustomFields(string messageId, DiStreamailContext db);
    }
}
