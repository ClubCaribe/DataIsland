using dataislandcommon.Utilities;
using dilib.Services.Communication;
using dimain.Services.System;
using Microsoft.AspNet.SignalR;
using Streamail.Classes.Communication;
using Streamail.Interfaces;
using Streamail.Models.db;
using Streamail.Models.Entities;
using Streamail.Services.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace Streamail.Services
{
    public class StreamailService : Streamail.Services.IStreamailService
    {
        
        public IDbStreamailService DbStreamails { get; set; }
        
        public IDbMessagesService DbMessages { get; set; }
        
        public IDbParticipantsService DbParticipants { get; set; }
        
        public IDbStreamailAdministratorsService DbAdministrators { get; set; }

        public IDbReadStatusesService DbReadStatuses { get; set; }

        public IDbMessageCustomFiledsService DbCustomFields { get; set; }
        
        public IStreamailDatabaseManagerSingleton dbManager { get; set; }
        
        public IStreamailsManagerSingleton StreamailManager { get; set; }
        
        public IDiUserService DiUsers { get; set; }



        public StreamailService()
        {

        }

        public async Task<bool> CreateStreamail(StreamailEntity streamail, bool notifyRecipients, string ownerUsername)
        {
            using (var db = this.dbManager.GetDbContext(ownerUsername))
            {
                if (await this.DbStreamails.CreateStreamail(streamail.Headers, db))
                {
                    if(await this.DbParticipants.AddParticipants(streamail.Headers.ID,streamail.Participants,db))
                    {
                        if(streamail.Administrators!=null)
                        {
                            if(await this.DbAdministrators.AddAdministrators(streamail.Administrators,streamail.Headers.ID,db))
                            {
                                await db.SaveChangesAsync();
                                return true;
                            }
                        }
                        else
                        {
                            await db.SaveChangesAsync();
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public async Task<bool> CheckStreamailExists(string id, string ownerUsername)
        {
            return await this.DbStreamails.CheckStreamailExists(id, ownerUsername);
        }

        public async Task<StreamailEntity> GetStreamail(string id,int numOfInitialMessages, string ownerUsername)
        {

            if (await this.DbStreamails.CheckStreamailExists(id, ownerUsername))
            {
                StreamailEntity strm = new StreamailEntity();
                strm.Headers = await this.DbStreamails.GetStreamail(id, ownerUsername);
                strm.Administrators = await this.DbAdministrators.GetAdministrators(id, ownerUsername);
                strm.Messages = await this.DbMessages.GetMessages(id, 1, numOfInitialMessages, ownerUsername);
                strm.Participants = await this.DbParticipants.GetEntityParticipants(id, ownerUsername);
                return strm;
            }
            return null;
        }

        public async Task<bool> SendMessage(StreamailMessage message, string ownerUsername)
        {
            message.ID = Guid.NewGuid().ToString();
            
            bool deleteParticipants = false;
            StreamailHeaders headers = await this.DbStreamails.GetStreamail(message.StreamailID, ownerUsername);
            if(!string.IsNullOrEmpty(headers.MessageSenderName))
            {
                if (message.Participants == null || message.Participants.Count == 0)
                {
                    List<Participant> defaultParticipants = await this.DbParticipants.GetEntityParticipants(message.StreamailID, ownerUsername);
                    if(defaultParticipants!=null && defaultParticipants.Count>0)
                    {
                        string senderId = await this.DiUsers.GetUserIdByFromUsername(ownerUsername);
                        foreach(Participant prt in defaultParticipants)
                        {
                            if(prt.ParticipantID == senderId)
                            {
                                prt.IsSender = true;
                            }
                        }
                    }
                    deleteParticipants = true;
                    message.Participants = defaultParticipants;
                }
                using(var scope = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
                {
                    object sender = null;
                    if(scope.TryResolveNamed(headers.MessageSenderName,typeof(IStreamailMessageSender),out sender))
                    {
                        await ((IStreamailMessageSender)sender).SendMessage(message);
                    }
                    
                }
                
            }

            List<Participant> msgParticipants = message.Participants;
            if (deleteParticipants)
            {
                message.Participants = null;
            }
            message.IsRead = true;
            await SaveMessage(message, ownerUsername);

            message.Participants = msgParticipants;

            IHubContext _hubContext = GlobalHost.ConnectionManager.GetHubContext<StreamailCommandsHub>();
            _hubContext.Clients.User(ownerUsername).NewStreamailMessage(message);

            return true;
        }

        public async Task ReceiveMessage(StreamailMessage msg, string ownerUsername)
        {
            msg.IsRead = false;

            if (msg.Participants.Count == 2)
            {
                string senderId = "";
                foreach (var participant in msg.Participants)
                {
                    if (participant.IsSender)
                    {
                        senderId = participant.ParticipantID;
                    }
                }
                if (!string.IsNullOrEmpty(senderId))
                {
                    string ownerUserId = await this.DiUsers.GetUserIdByFromUsername(ownerUsername);
                    if (msg.StreamailID == ownerUserId)
                    {
                        msg.StreamailID = senderId;
                    }
                }
            }

            List<Participant> streamailParticipants = await this.DbParticipants.GetEntityParticipants(msg.StreamailID, ownerUsername);
            bool areParticipantsTheSame = false;
            if (streamailParticipants.Count == msg.Participants.Count)
            {
                areParticipantsTheSame = true;
                foreach(var mpart in msg.Participants)
                {
                    bool exist = false;
                    foreach(var rpart in streamailParticipants)
                    {
                        if (rpart.ParticipantID == mpart.ParticipantID)
                        {
                            exist = true;
                        }
                    }
                    if(!exist)
                    {
                        areParticipantsTheSame = false;
                        break;
                    }
                }
            }

            List<Participant> msgParticipants = msg.Participants;
            if(areParticipantsTheSame)
            {
                msg.Participants = null;
            }
            await this.SaveMessage(msg, ownerUsername);
            msg.Participants = msgParticipants;

            IHubContext _hubContext = GlobalHost.ConnectionManager.GetHubContext<StreamailCommandsHub>();
            _hubContext.Clients.User(ownerUsername).NewStreamailMessage(msg);
            
        }

        public async Task SaveMessage(StreamailMessage message, string ownerUsername)
        {
            
            if (!string.IsNullOrEmpty(message.RendererName))
            {
                using (var scope = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
                {
                    object oRenderer = null;
                    if (scope.TryResolveNamed(message.RendererName, typeof(IStreamailMessageRenderer), out oRenderer))
                    {
                        IStreamailMessageRenderer renderer = (IStreamailMessageRenderer)oRenderer;
                        string renderedMessage;
                        if(renderer.IsRendererAsync())
                        {
                            renderedMessage = await renderer.RenderMessageAsync(message.Message);
                        }
                        else
                        {
                            renderedMessage = renderer.RenderMessage(message.Message);
                        }

                        await this.DbMessages.AddRawMessage(message.ID, message.Message, ownerUsername);
                        message.Message = renderedMessage;
                    }
                }
            }

            await this.DbMessages.AddMessage(message, ownerUsername);
            if(message.CustomFields!=null && message.CustomFields.Count>0)
            {
                await this.DbCustomFields.AddMessageCustomFields(message.CustomFields, message.ID, ownerUsername);
            }
            if (message.Participants != null && message.Participants.Count > 0)
            {
                await this.DbParticipants.AddParticipants(message.ID, message.Participants, ownerUsername);
            }
        }

        public async Task<int> GetNumOfUnreadMessages(string streamailId, string ownerUsername)
        {
            return await this.DbMessages.GetNumberOfUnreadMessages(streamailId, ownerUsername);
        }

        public async Task<List<StreamailMessage>> GetMessages(string streamailId,int pageNum, int pageSize, string ownerUsername)
        {
            return await this.DbMessages.GetMessages(streamailId, pageNum, pageSize, ownerUsername);
        }

        public async Task<bool> MarkAllMessagesAsRead(string streamailId,string ownerUsername)
        {
            return await this.DbMessages.MarkAllMessagesAsRead(streamailId, ownerUsername);
        }

        public async Task<bool> MarkMessageAsRead(string messageId, string ownerUsername)
        {
            if(await this.DbMessages.MarkMessageAsRead(messageId,ownerUsername))
            {
                StreamailHeaders headers = await this.DbStreamails.GetStreamailHeadersFromMessageID(messageId, ownerUsername);
                if(headers!=null)
                {
                    if (headers.SendReadNotifications)
                    {
                        if (!string.IsNullOrEmpty(headers.MessageSenderName))
                        {
                            string participantId = await this.DbMessages.GetMessageSenderID(messageId, ownerUsername);
                            if (!string.IsNullOrEmpty(participantId))
                            {
                                string senderId = await this.DiUsers.GetUserIdByFromUsername(ownerUsername);

                                using (var scope = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
                                {
                                    object sender = null;
                                    if (scope.TryResolveNamed(headers.MessageSenderName, typeof(IStreamailMessageSender), out sender))
                                    {
                                        await ((IStreamailMessageSender)sender).SendMarkMessageAsRead(messageId, participantId, senderId);
                                    }

                                }
                            }
                        }
                        
                    }
                }
                
                return true;
                
            }
            return false;
        }

        public async Task ReceiveMarkMessageAsRead(string messageId, string participantId, string ownerUsername)
        {
            StreamailHeaders headers = await this.DbStreamails.GetStreamailHeadersFromMessageID(messageId, ownerUsername);
            if(headers!=null)
            {
                await this.DbReadStatuses.AddReadStatus(messageId, headers.ID, participantId, ownerUsername);
                IHubContext _hubContext = GlobalHost.ConnectionManager.GetHubContext<StreamailCommandsHub>();
                _hubContext.Clients.User(ownerUsername).MessageRead(messageId, participantId);
            }
        }

    }
}
