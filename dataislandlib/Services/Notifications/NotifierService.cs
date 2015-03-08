using Streamail.Models.db;
using Streamail.Models.Entities;
using Streamail.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dataislandcommon.Services.Notifications
{
    public class NotifierService : dataislandcommon.Services.Notifications.INotifierService
    {
        public IStreamailService Streamail { get; set; }
        public NotifierService()
        {

        }

        public async Task SendNotification(string notificatorName, string rendererName, string title, string iconCssClass, string notification, string ownerUsername)
        {
            if(!await this.Streamail.CheckStreamailExists("notificator",ownerUsername))
            {
                StreamailEntity streamail = new StreamailEntity();
                streamail.Headers = new StreamailHeaders();
                streamail.Headers.ID = "notificator";
                streamail.Headers.CanAddSubmessages = false;
                streamail.Headers.CreationTime = DateTime.UtcNow;
                streamail.Headers.LastModificationTime = DateTime.UtcNow;
                streamail.Headers.ReadOnly = true;
                streamail.Headers.SendReadNotifications = false;
                streamail.Headers.StreamailType = "notificator";
                streamail.Headers.Subject = "[tr]Notifications[/tr]";
                streamail.Headers.SubMessagesDepth = 0;
                streamail.Participants = new List<Participant>();
                await this.Streamail.CreateStreamail(streamail, false, ownerUsername);
            }

            StreamailMessage message = new StreamailMessage();
            message.ID = Guid.NewGuid().ToString();
            message.IsRead = true;
            message.Message = notification;
            message.ParentMessageID = "";
            message.ReceiveTime = DateTime.UtcNow;
            message.RendererName = notificatorName;
            message.RootMessageID = "";
            message.SendTime = DateTime.UtcNow;
            message.StreamailID = "notificator";
            message.SenderID = notificatorName;
            message.CustomFields = new Dictionary<string, object>();
            message.CustomFields["title"] = title;
            message.CustomFields["icon"] = iconCssClass;

            var res = await this.Streamail.SendMessage(message, ownerUsername);
        }
    }
}
