using Streamail.Models.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streamail.Interfaces
{
    public interface IStreamailMessageSender
    {
        Task<int> SendMessage(StreamailMessage msg);
        Task SendMarkMessageAsRead(string messageId, string participantId, string senderId);
    }
}
