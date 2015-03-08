using dimain.Models.maindb;
using dimain.Services.System;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dataislandcommon.Services.Communication
{
    public class DiUserCommandSenderService : dataislandcommon.Services.Communication.IDiUserCommandSenderService
    {

        public string UserId { get; set; }
        public string DataIslandPublicKey { get; set; }
        public string Url { get; set; }
        public string DataIslandId { get; set; }
        public string DataIslandUrl { get; set; }
        public string UserPublicKey { get; set; }

        
        public IDiUserService UserService { get; set; }

        
        public IDataIslandService DataIslandService { get; set; }

        private bool sendingInProgress = false;
        private Queue<string> commands = new Queue<string>();
        private DateTime lastDataRefresh = DateTime.Now.AddDays(-1);

        public DiUserCommandSenderService()
        {
        }

        public async Task<bool> RefreshUserData()
        {
            try
            {
                DiUserData userData = await UserService.GetDIUserDataFromUserId(this.UserId);
                if (userData != null)
                {
                    string diPublicKey = await DataIslandService.GetDataIslandPublicKey(userData.DatIslandId);
                    string diUrl = await DataIslandService.GetDataislandUrl(userData.DatIslandId);
                    if ((!string.IsNullOrEmpty(diPublicKey)) && (!string.IsNullOrEmpty(diUrl)))
                    {
                        this.DataIslandUrl = diUrl;
                        this.DataIslandPublicKey = diPublicKey;
                        this.DataIslandId = userData.DatIslandId;
                        this.UserPublicKey = userData.PublicKey;
                        this.lastDataRefresh = DateTime.Now.AddHours(2);
                        return true;
                    }

                }
            }
            catch
            {

            }
            return false;
        }

        public void SendCommand(string command)
        {
            commands.Enqueue(command);
            Task.Factory.StartNew(() => this.SendCommands());
        }

        public async Task SendCommands()
        {
            if(sendingInProgress)
            {
                return;
            }

            this.sendingInProgress = true;

            while(commands.Count>0)
            {
                string cmd = this.commands.Dequeue();
                //todo sending commands to another dataisland
            }

            this.sendingInProgress = false;
        }
    }
}
