using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dataislandcommon.Services.Communication;
using dimain.Services.System;
using Autofac;
using dataislandcommon.Utilities;
using dimain.Models.maindb;

namespace dimain.Services.Communication
{
    public class DiCommandManagerSingleton : dimain.Services.Communication.IDiCommandManagerSingleton
    {
        public Dictionary<string, IDiUserCommandSenderService> Senders { get; set; }

        public Dictionary<string, IDiUserCommandReceiverService> Receivers { get; set; }

        
        public IDiUserService UserService { get; set; }

        
        public IDataIslandService DataIslandService { get; set; }

        public DiCommandManagerSingleton()
        {
            Senders = new Dictionary<string, IDiUserCommandSenderService>();
            Receivers = new Dictionary<string, IDiUserCommandReceiverService>();
        }

        public async Task SendCommand(string userId, string command)
        {
            bool userExists = await UserService.CheckUserIdExists(userId);
            if(userExists)
            {
                Task.Factory.StartNew(() => this.ReceiveCommand(userId, command));
            }
            else
            {
                IDiUserCommandSenderService sender = null;
                if (!Senders.ContainsKey(userId))
                {
                    sender = AutofacConfig.GetConfiguredContainer().Resolve<IDiUserCommandSenderService>();
                    sender.UserId = userId;
                    if (await sender.RefreshUserData())
                    {
                        this.Senders[userId] = sender;
                    }
                }
                else
                {
                    sender = this.Senders[userId];
                }

                if(sender!=null)
                {
                    sender.SendCommand(command);
                }
            }
        }

        public async Task ReceiveCommand(string command)
        {

        }

        private async Task ReceiveCommand(string userId, string command)
        {

            if (!this.Receivers.ContainsKey(userId))
            {
                if (await UserService.CheckUserIdExists(userId))
                {
                    IDiUserCommandReceiverService receiver = AutofacConfig.GetConfiguredContainer().Resolve<IDiUserCommandReceiverService>();
                    receiver.UserId = userId;
                    string receiverUsername = await this.UserService.GetUsernameFromUserId(userId);
                    if (!string.IsNullOrEmpty(receiverUsername))
                    {
                        receiver.Username = receiverUsername;
                        this.Receivers[userId] = receiver;
                        receiver.ReceiveCommand(command);
                    }
                }
            }
            else
            {
                this.Receivers[userId].ReceiveCommand(command);
            }
        }
    }
}
