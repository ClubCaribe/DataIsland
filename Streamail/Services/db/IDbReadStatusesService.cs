using Streamail.Classes.db;
using Streamail.Models.db;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Streamail.Services.db
{
    public interface IDbReadStatusesService
    {
        Task<bool> AddReadStatus(string messageId, string streamailId, string participantId, string ownerUsername);
        Task DeleteReadStatuses(string messageId, DiStreamailContext db);
        Task<List<ReadStatus>> GetStatuses(string messageId, DiStreamailContext db);
    }
}
