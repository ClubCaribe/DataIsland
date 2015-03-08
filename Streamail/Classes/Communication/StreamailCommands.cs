using dataislandcommon.Classes.Attributes;
using dataislandcommon.Interfaces.Communication;
using dataislandcommon.Utilities;
using Streamail.Models.db;
using Streamail.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace Streamail.Classes.Communication
{
    [DataIslandCommands]
    public class StreamailCommands : IDataIslandCommandHub
    {
        
        public StreamailCommands()
        {
        }
        public string Username
        {
            get;
            set;
        }

        public string UserId
        {
            get;
            set;
        }

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

        public async Task SendMessage(StreamailMessage message)
        {
            using (var scope = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                IStreamailService streamailService = scope.Resolve<IStreamailService>();
                await streamailService.ReceiveMessage(message, Username);
            }
        }

        public async Task SendMarkMessageAsRead(string messageId)
        {
            using (var scope = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                IStreamailService streamailService = scope.Resolve<IStreamailService>();
                await streamailService.ReceiveMarkMessageAsRead(messageId, this.SenderId, this.Username);
            }
        }
    }
}
