using dilib.Services.Communication;
using dimain.Services.System;

using Streamail.Interfaces;
using Streamail.Models.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streamail.Services
{
    public class DefaultStreamailMessageSender : IStreamailMessageSender
    {
        
        public IDICommandsService Commands { get; set; }

        public DefaultStreamailMessageSender()
        {

        }

        public async Task<int> SendMessage(StreamailMessage message)
        {
            if (message.Participants.Count > 0)
            {
                string senderId = String.Empty;
                foreach (Participant prt in message.Participants)
                {
                    if (prt.IsSender)
                    {
                        senderId = prt.ParticipantID;
                    }
                }
                int numberOfSendMessages = 0;
                foreach (Participant prt in message.Participants)
                {
                    if (!prt.IsSender)
                    {
                        this.Commands.User(senderId, prt.ParticipantID).SendMessage(message);
                        numberOfSendMessages++;
                    }
                }
                return numberOfSendMessages;
            }
            return 0;
        }

        public async Task SendMarkMessageAsRead(string messageId, string participantId, string senderId)
        {
            this.Commands.User(senderId, participantId).SendMarkMessageAsRead(messageId);
        }

    }
}
