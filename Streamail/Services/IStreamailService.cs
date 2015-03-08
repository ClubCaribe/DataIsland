using Streamail.Interfaces;
using Streamail.Models.db;
using Streamail.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Streamail.Services
{
    public interface IStreamailService
    {
        Task<bool> CreateStreamail(StreamailEntity streamail, bool notifyRecipients, string ownerUsername);
        Task<StreamailEntity> GetStreamail(string id, int numOfInitialMessages, string ownerUsername);
        Task ReceiveMessage(StreamailMessage msg, string ownerUsername);
        Task<bool> SendMessage(StreamailMessage message, string ownerUsername);
        Task<bool> CheckStreamailExists(string id, string ownerUsername);
        Task<int> GetNumOfUnreadMessages(string streamailId, string ownerUsername);
        Task<List<StreamailMessage>> GetMessages(string streamailId, int pageNum, int pageSize, string ownerUsername);
        Task<bool> MarkAllMessagesAsRead(string streamailId, string ownerUsername);
        Task<bool> MarkMessageAsRead(string messageId, string ownerUsername);
        Task ReceiveMarkMessageAsRead(string messageId, string participantId, string ownerUsername);
    }
}
