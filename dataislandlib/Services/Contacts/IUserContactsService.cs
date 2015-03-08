using dataislandcommon.Models.userdb;
using dataislandcommon.Models.ViewModels.Contacts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace dataislandcommon.Services.System
{
    public interface IUserContactsService
    {
        Task<List<UserExternalContact>> FindExternalUsers(string searchPhrase, string username);
        Task<List<string>> GetContactsUsernames(string username);
        Task<bool> CheckContactExists(string userId, string ownerUsername);
        Task<bool> AddContact(UserExternalContact exContact, bool isAccepted, string ownerusername);
        Task<bool> AddContactRequest(UserContact contact, string ownerUsername);
        Task<bool> AcceptContactRequest(string userId, string ownerUsername);
        Task<List<UserContact>> GetUserContacts(string ownerUsername);
        Task<bool> UpdateAcceptStatus(string userId, bool acceptSatatus, string ownerusername);
        Task<bool> DeleteContact(string userId, string ownerUsername);
        Task<bool> UpdateContact(UserContact contact, string ownerUsername);
        Task<UserContact> GetContact(string userId, string ownerUsername);
        Task<UserContact> GetContactByUsername(string username, string ownerUsername);
        Task<bool> SetFavourite(string userId, bool isFavourite, string ownerUsername);
        void UpdateContactAvatar(string userId, byte[] avatar, string ownerUsername);
        Task<byte[]> GetContactAvatar(string username, int size, string type, string ownerUsername);
        Task SendUpdateAvatarCommandToContacts(byte[] avatar, string ownerUsername);
        Task<bool> ResendContactRequest(string userId, string ownerUsername);
        Task<bool> UpdateContactName(string userId, string newName, string ownerUsername);
    }
}
