using dataislandcommon.Models.userdb;
using dataislandcommon.Services.System;
using dimain.Services.System;
using Newtonsoft.Json.Linq;
using Streamail.Classes.Utilities;
using Streamail.Interfaces;
using Streamail.Models.db;
using Streamail.Models.Entities;
using Streamail.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DataIsland.Areas.panel.api
{
    [Authorize]
    [RoutePrefix("api/panel/chat")]
    public class ChatController : ApiController
    {
        private readonly IStreamailService Streamail;

        
        public IDiUserService DiUsers { get; set; }

        
        public IUserContactsService Contacts { get; set; }

        public ChatController(IStreamailService streamailService)
        {
            this.Streamail = streamailService;
        }

        [Route("sendmessage")]
        [HttpPost]
        public async Task<bool> SendMessage(JObject jsonData)
        {
            StreamailMessage message = new StreamailMessage();
            dynamic postData = jsonData;
            message.ID = Guid.NewGuid().ToString();
            message.IsRead = true;
            message.Message = postData.Message;
            message.ParentMessageID = "";
            message.ReceiveTime = DateTime.UtcNow;
            message.RendererName = "";
            message.RootMessageID = "";
            message.SendTime = DateTime.UtcNow;
            message.StreamailID = postData.Receiver;
            message.SenderID = await this.DiUsers.GetUserIdByFromUsername(this.User.Identity.Name);

            var res = await this.Streamail.SendMessage(message, this.User.Identity.Name);

            return res;
        }

        [Route("getnumberofunreadmessagesuser")]
        [HttpPost]
        public async Task<int> GetUserNumberOfUnreadMessages(JObject jsonData)
        {
            dynamic postData = jsonData;

            if (!await this.Streamail.CheckStreamailExists(postData.UserId, this.User.Identity.Name))
            {
                StreamailEntity streamail = new StreamailEntity();
                streamail.Headers = new StreamailHeaders();
                streamail.Headers.ID = postData.UserId;
                streamail.Headers.CanAddSubmessages = false;
                streamail.Headers.CreationTime = DateTime.UtcNow;
                streamail.Headers.LastModificationTime = DateTime.UtcNow;
                streamail.Headers.ReadOnly = false;
                streamail.Headers.SendReadNotifications = true;
                streamail.Headers.StreamailType = "chat";
                streamail.Headers.Subject = "[tr]Chat with[/tr] " + postData.UserName;
                streamail.Headers.SubMessagesDepth = 0;
                streamail.Headers.MessageSenderName = StreamailConsts.DefaultStreamailMessageSender;
                streamail.Participants = new List<Participant>();
                Participant me = new Participant();
                me.EntityID = postData.UserId;
                me.ID = Guid.NewGuid().ToString();
                me.IsRead = false;
                me.IsSender = false;
                me.ParticipantID = await this.DiUsers.GetUserIdByFromUsername(this.User.Identity.Name);
                me.ParticipantName = this.User.Identity.Name;
                streamail.Participants.Add(me);

                Participant usr = new Participant();
                usr.EntityID = postData.UserId;
                usr.ID = Guid.NewGuid().ToString();
                usr.IsRead = false;
                usr.IsSender = false;
                usr.ParticipantID = postData.UserId;
                usr.ParticipantName = postData.UserName;
                streamail.Participants.Add(usr);

                await this.Streamail.CreateStreamail(streamail,false, this.User.Identity.Name);
                return 0;
            }
            else
            {
                return await this.Streamail.GetNumOfUnreadMessages(postData.UserId, this.User.Identity.Name);
            }
            return 0;
        }

        [Route("getnumberofunreadmessagesalluser")]
        [HttpGet]
        public async Task<Dictionary<string,int>> GetNumberOfUnreadMessagesAllUsers()
        {
            Dictionary<string,int> counts = new Dictionary<string,int>();

            List<UserContact> contacts = await this.Contacts.GetUserContacts(this.User.Identity.Name);
            if(contacts!=null && contacts.Count>0)
            {
                foreach(var contact in contacts)
                {
                    int count = 0;
                    if (!await this.Streamail.CheckStreamailExists(contact.UserId, this.User.Identity.Name))
                    {
                        StreamailEntity streamail = new StreamailEntity();
                        streamail.Headers = new StreamailHeaders();
                        streamail.Headers.ID = contact.UserId;
                        streamail.Headers.CanAddSubmessages = false;
                        streamail.Headers.CreationTime = DateTime.UtcNow;
                        streamail.Headers.LastModificationTime = DateTime.UtcNow;
                        streamail.Headers.ReadOnly = false;
                        streamail.Headers.SendReadNotifications = true;
                        streamail.Headers.StreamailType = "chat";
                        streamail.Headers.Subject = "[tr]Chat with[/tr] " + contact.Username;
                        streamail.Headers.SubMessagesDepth = 0;
                        streamail.Headers.MessageSenderName = StreamailConsts.DefaultStreamailMessageSender;
                        streamail.Participants = new List<Participant>();
                        Participant me = new Participant();
                        me.EntityID = contact.UserId;
                        me.ID = Guid.NewGuid().ToString();
                        me.IsRead = false;
                        me.IsSender = false;
                        me.ParticipantID = await this.DiUsers.GetUserIdByFromUsername(this.User.Identity.Name);
                        me.ParticipantName = this.User.Identity.Name;
                        streamail.Participants.Add(me);

                        Participant usr = new Participant();
                        usr.EntityID = contact.UserId;
                        usr.ID = Guid.NewGuid().ToString();
                        usr.IsRead = false;
                        usr.IsSender = false;
                        usr.ParticipantID = contact.UserId;
                        usr.ParticipantName = contact.Username;
                        streamail.Participants.Add(usr);

                        await this.Streamail.CreateStreamail(streamail,false, this.User.Identity.Name);
                    }
                    else
                    {
                        count = await this.Streamail.GetNumOfUnreadMessages(contact.UserId, this.User.Identity.Name);
                    }
                    counts[contact.UserId] = count;
                }
            }

            return counts;
        }

        [Route("getchatmessages")]
        [HttpPost]
        public async Task<List<StreamailMessage>> GetChatMessages(JObject jsonData)
        {
            dynamic postData = jsonData;
            string streamailId = postData.UserId;
            int pageNum = postData.PageNum;
            int pageSize = postData.PageSize;
            await this.Streamail.MarkAllMessagesAsRead(streamailId, this.User.Identity.Name);
            return await this.Streamail.GetMessages(streamailId, pageNum, pageSize, this.User.Identity.Name);
        }

    }
}
