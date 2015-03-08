using dataislandcommon.Classes.Attributes;
using dataislandcommon.Interfaces.Communication;
using dataislandcommon.Models.userdb;
using dataislandcommon.Services.System;
using dataislandcommon.Utilities;
using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dataislandcommon.Classes.Communication
{
    [DataIslandCommands]
    public class UserCommandsHub : IDataIslandCommandHub
    {

        public UserCommandsHub()
        {
        }

        public string Username { get; set; }
        public string UserId { get; set; }

        public string SenderId
        {
            get;
            set;
        }

        public int CommandVersion
        {
            get;
            set;
        }

        public async Task SendContactRequest(UserContact contact)
        {
            using (var scope = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                IUserContactsService contacts = scope.Resolve<IUserContactsService>();
                await contacts.AddContactRequest(contact, this.Username);
            }
        }

        public async Task ContactDeleted(string userId)
        {
            using (var scope = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                IUserContactsService contacts = scope.Resolve<IUserContactsService>();
                await contacts.UpdateAcceptStatus(userId, false, this.Username);
            }
        }

        public async Task AcceptContact(string userId)
        {
            using (var scope = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                IUserContactsService contacts = scope.Resolve<IUserContactsService>();
                await contacts.UpdateAcceptStatus(userId, true, this.Username);
            }
        }

        public void UpdateContactAvatar(string userId, byte[] avatar)
        {
            using (var scope = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                IUserContactsService contacts = scope.Resolve<IUserContactsService>();
                contacts.UpdateContactAvatar(userId, avatar, this.Username);
            }
        }

        public async Task UpdateContactName(string userId, string newName)
        {
            using (var scope = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                IUserContactsService contacts = scope.Resolve<IUserContactsService>();
                await contacts.UpdateContactName(userId, newName, this.Username);
            }
        }
    }
}
