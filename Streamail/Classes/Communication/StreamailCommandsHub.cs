using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using dataislandcommon.Utilities;
using Streamail.Services;

namespace Streamail.Classes.Communication
{
    public class StreamailCommandsHub : Hub
    {
        public StreamailCommandsHub()
        {

        }

        public async Task MarkMessageAsRead(string messadeId)
        {
            using (var scope = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                IStreamailService streamail = scope.Resolve<IStreamailService>();
                await streamail.MarkMessageAsRead(messadeId, this.Context.User.Identity.Name);
            }
        }
    }
}
