using dataislandcommon.Services.Notifications;
using dataislandcommon.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dataislandcommon.Services.Contacts
{
    public class ContactsNotificationsService : dataislandcommon.Services.Contacts.IContactsNotificationsService
    {
        private readonly INotifierService notifier;
        public ContactsNotificationsService(INotifierService ntf)
        {
            notifier = ntf;
        }

        public void ContactRequestAdded(string username, string ownerUsername)
        {
            string messageTemplate = @"
            <div style=""overflow:auto;"">
                <div style=""width:44px; float:left;"">
                    <a href=""/panel/contacts""><img width=""44"" class=""img-circle"" alt="""" src=""/panel/contacts/contactthumbnail/{0}/44/sqr"" ></a>
                </div>
                <div style=""margin-left:55px; line-height:44px;"">
                    <a href=""/panel/contacts"">[tr]User[/tr] {0} [tr]sent contact request[/tr]</a>
                </div>
            </div>";
            string message = String.Format(messageTemplate, username);
            this.notifier.SendNotification("Contacts", "", "[tr]Contacts[/tr]", DiConsts.IconsCssClasses.entypo_users, message, ownerUsername);
        }

        public void ContactAccepted(string username, string ownerUsername)
        {
            string messageTemplate = @"
            <div style=""overflow:auto;"">
                <div style=""width:44px; float:left;"">
                    <a href=""/panel/contacts""><img width=""44"" class=""img-circle"" alt="""" src=""/panel/contacts/contactthumbnail/{0}/44/sqr"" ></a>
                </div>
                <div style=""margin-left:55px; line-height:44px;"">
                    <a href=""/panel/contacts"">[tr]User[/tr] {0} [tr]accetped your contact request[/tr]</a>
                </div>
            </div>";
            string message = String.Format(messageTemplate, username);
            this.notifier.SendNotification("Contacts", "", "[tr]Contacts[/tr]", DiConsts.IconsCssClasses.entypo_users, message, ownerUsername);
        }

        public void ContactDeleted(string username, string ownerUsername)
        {
            string messageTemplate = @"
            <div style=""overflow:auto;"">
                <div style=""width:44px; float:left;"">
                    <a href=""/panel/contacts""><img width=""44"" class=""img-circle"" alt="""" src=""/panel/contacts/contactthumbnail/{0}/44/sqr"" ></a>
                </div>
                <div style=""margin-left:55px; line-height:44px;"">
                    <a href=""/panel/contacts"">[tr]User[/tr] {0} [tr]deleted your contact[/tr]</a>
                </div>
            </div>";
            string message = String.Format(messageTemplate, username);
            this.notifier.SendNotification("Contacts", "", "[tr]Contacts[/tr]", DiConsts.IconsCssClasses.entypo_users, message, ownerUsername);
        }
    }
}
