using Streamail.Classes.db;
using Streamail.Models.db;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Streamail.Services.db
{
    public interface IDbParticipantsService
    {
        Task<bool> AddParticipant(string entityId, string participantId, string participantName, bool isRead, string ownerUsername);
        Task<bool> AddParticipants(string entityId, List<Participant> participants, string ownerUsername);
        Task<bool> AddParticipants(string entityId, List<Participant> participants, DiStreamailContext db);
        Task<bool> AddSender(string entityId, string participantId, string participantName, string ownerUsername);
        Task<bool> DeleteEntityParticipant(string entityId, string participantId, string ownerUsername);
        Task<bool> DeleteEntityParticipants(string entityId, string ownerUsername);
        Task<List<Participant>> GetEntityParticipants(string entityId, string ownerUsername);
        Task<Participant> GetEntitySender(string entityId, string ownerUsername);
        Task<bool> SetReadStatus(string entityId, string participantId, bool isRead, string ownerUsername);
    }
}
