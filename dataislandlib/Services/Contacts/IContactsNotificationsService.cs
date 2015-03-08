using System;
namespace dataislandcommon.Services.Contacts
{
    public interface IContactsNotificationsService
    {
        void ContactAccepted(string username, string ownerUsername);
        void ContactDeleted(string username, string ownerUsername);
        void ContactRequestAdded(string username, string ownerUsername);
    }
}
