using Streamail.Classes.db;
using Streamail.Models.db;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Streamail.Services.db
{
    public interface IDbStreamailService
    {
        Task<bool> ChangeLastModificationTime(string id, DateTime lastModifyTime, string ownerUsername);
        Task<bool> ChangeSubject(string id, string newsubject, string ownerUsername);
        Task<bool> CheckStreamailExists(string id, string ownerUsername);
        Task<bool> CreateStreamail(string id, string subject, string streamailType, DateTime creationTime, DateTime lastModificationTime, int submessageDepth, bool isReadOnly, bool canAddSubMessages, bool sendReadNotifications, string ownerUsername);
        Task<bool> CreateStreamail(StreamailHeaders streamail, string ownerUsername);
        Task<bool> CreateStreamail(StreamailHeaders streamail, DiStreamailContext db);
        Task<bool> DeleteStreamail(string id, string ownerUsername);
        Task<List<string>> DeleteStreamails(List<string> ids, string ownerUsername);
        Task<bool> UpdateLastModificationTime(string id, DateTime modificationTime, string ownerUsername);
        Task<bool> UpdateStreamail(StreamailHeaders streamail, string ownerUsername);
        Task<List<StreamailHeaders>> GetStreamails(string ownerUsername);
        Task<StreamailHeaders> GetStreamail(string id, string ownerUsername);
        Task<List<StreamailHeaders>> GetStreamails(List<string> ids, string ownerUsername);
        Task<List<StreamailHeaders>> GetStreamails(string type, string ownerUsername);
        Task<StreamailHeaders> GetStreamailHeadersFromMessageID(string messageID, string ownerUsername);
    }
}
