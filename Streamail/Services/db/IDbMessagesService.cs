using Streamail.Classes.db;
using Streamail.Models.db;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Streamail.Services.db
{
    public interface IDbMessagesService
    {
        Task<bool> AddMessage(string id, string streamailId, string renderedMessage, string rawMessage, DateTime sendTime, string parentMessageId, string rootMessageId, string rendererName, string ownerUsername);
        Task<bool> AddMessage(StreamailMessage msg, string ownerUsername);
        Task<bool> AddRawMessage(string id, string streamailId, string rawMessage, DateTime sendTime, string parentMessageId, string rootMessageId, string rendererName, string ownerUsername);
        Task<bool> AddRawMessage(string id, string rawMessage, string ownerUsername);
        Task<bool> CheckMessageExists(string id, string ownerUsername);
        Task DeleteMessage(string id, string ownerUsername);
        Task DeleteMessagesFromStreamail(string streamailId, string ownerUsername);
        Task<List<StreamailMessage>> FindMessages(string streamailId, string searchPhrase, string ownerUsername);
        Task<StreamailMessage> GetMessage(string id, string ownerUsername);
        Task<List<StreamailMessage>> GetMessages(string streamailId, int pageNum, int pageSize, string ownerUsername);
        Task<List<StreamailMessage>> GetMessages(string streamailId, string ownerUsername);
        Task<List<StreamailMessage>> GetMessagesFromGivenID(string messageId, string ownerUsername);
        Task<bool> UpdateReadStatus(string id, bool isRead, string ownerUsername);
        Task<bool> UpdateRenderedMessage(string id, string renderedMessage, string ownerUsername);
        Task<int> GetNumberOfUnreadMessages(string streamailID, string ownerUsername);
        Task<bool> MarkAllMessagesAsRead(string streamailId, string ownerUsername);
        Task<bool> MarkMessageAsRead(string messageId, string ownerUsername);
        Task<string> GetStreamailID(string messageId, string ownerUsername);
        Task<string> GetMessageSenderID(string messageId, string ownerUsername);
    }
}
