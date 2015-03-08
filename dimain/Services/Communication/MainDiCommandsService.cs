using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using scutils.maindicommands;
using scutils.transportprotocol;

using dimain.Services.System;
using Newtonsoft.Json;

namespace dimain.Services.Communication
{
    public class MainDiCommandsService : dimain.Services.Communication.IMainDiCommandsService
    {
        
        public IDataIslandSettingsService DiSettings { get; set; }

        private readonly IServersApiSettingsSingleton _serversSettings;

        public MainDiCommandsService(IServersApiSettingsSingleton serversSettings)
        {
            _serversSettings = serversSettings;
        }

        public async Task<bool> RegisterDataIsland(RegisterDataiSlandArgs args)
        {
            try
            {
                TransportCommand cmm = new TransportCommand();
                cmm.CommandName = "registerdataIsland";
                cmm.CommandArguments = args;
                string res = await TransportClient.SendCommand(cmm, _serversSettings.MainServerApiKey);
                if (res == "ok")
                {
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        public async Task<bool> SetDataIslandIp(string dataislandid)
        {
            try
            {
                object obDomain = await DiSettings.GetSetting("domain");
                string domain = ((obDomain != null) ? obDomain.ToString() : "");
                object isPublic = (await DiSettings.GetSetting("ispublic"));
                bool ispublic = (((bool)(isPublic)) ? true : false);
                SetDataIslandDomainArgs args = new SetDataIslandDomainArgs();
                args.Domain = domain;
                args.ID = dataislandid;
                args.IsPublic = ispublic;
                TransportCommand cmm = new TransportCommand();
                cmm.CommandName = "setdataislandip";
                cmm.CommandArguments = args;
                string res = await TransportClient.SendCommand(cmm, _serversSettings.MainServerApiKey);
                if (res == "ok")
                {
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        public async Task<bool> RegisterUser(RegisterUserArgs args)
        {
            try
            {
                TransportCommand cmm = new TransportCommand();
                cmm.CommandName = "registeruser";
                cmm.CommandArguments = args;
                string res = await TransportClient.SendCommand(cmm, _serversSettings.MainServerApiKey);
                if (res == "ok")
                {
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        public async Task<bool> DeleteUser(DeleteUserArgs args)
        {
            try
            {
                TransportCommand cmm = new TransportCommand();
                cmm.CommandName = "deleteuser";
                cmm.CommandArguments = args;
                string res = await TransportClient.SendCommand(cmm, _serversSettings.MainServerApiKey);
                if (res == "ok")
                {
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        public async Task<bool> RegisterUserEmail(AddUserEmailArgs args)
        {
            try
            {
                TransportCommand cmm = new TransportCommand();
                cmm.CommandName = "adduseremail";
                cmm.CommandArguments = args;
                string res = await TransportClient.SendCommand(cmm, _serversSettings.MainServerApiKey);
                if (res == "ok")
                {
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        public async Task<bool> DeleteUserEmail(string email)
        {
            try
            {
                TransportCommand cmm = new TransportCommand();
                cmm.CommandName = "deleteuseremail";
                cmm.CommandArguments = email;
                string res = await TransportClient.SendCommand(cmm, _serversSettings.MainServerApiKey);
                if (res == "ok")
                {
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        public async Task<bool> SetDataIslandData(SetDataIslandDataArgs dat)
        {
            try
            {
                TransportCommand cmm = new TransportCommand();
                cmm.CommandName = "setdataislanddata";
                cmm.CommandArguments = dat;
                string res = await TransportClient.SendCommand(cmm, _serversSettings.MainServerApiKey);
                if (res == "ok")
                {
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }


        public async Task<bool> CheckUserExists(string username)
        {
            try
            {
                return await MainDiCommands.CheckUserExists(username, _serversSettings.MainServerApiKey);
            }
            catch
            {
            }
            return true;
        }

        public async Task<string> GetUserDataIslandPublicKey(string username)
        {
            try
            {
                TransportCommand cmm = new TransportCommand();
                cmm.CommandName = "getuserdataislandpublickey";
                cmm.CommandArguments = username;
                string res = await TransportClient.SendCommand(cmm, _serversSettings.MainServerApiKey);
                if ((!string.IsNullOrEmpty(res)) && (res != "err"))
                {
                    return res;
                }
            }
            catch
            {
            }
            return "";
        }

        public async Task<string> GetUserDataIslandPublicKeyFromUserId(string userId)
        {
            try
            {
                TransportCommand cmm = new TransportCommand();
                cmm.CommandName = "getuserdataislandpublickeyfromuserid";
                cmm.CommandArguments = userId;
                string res = await TransportClient.SendCommand(cmm, _serversSettings.MainServerApiKey);
                if ((!string.IsNullOrEmpty(res)) && (res != "err"))
                {
                    return res;
                }
            }
            catch
            {
            }
            return "";
        }

        public async Task<bool> ExtendProAccount(string username, int months, string key)
        {
            return await MainDiCommands.ExtendProAccount(username, months, key, _serversSettings.MainServerApiKey);
        }

        public async Task<string> GetUserDataIslandID(string username)
        {
            return await MainDiCommands.GetUserDataIslandID(username, _serversSettings.MainServerApiKey);
        }

        public async Task<string> GetUserDataIslandIDFromUserId(string userId)
        {
            try
            {
                TransportCommand cmm = new TransportCommand();
                cmm.CommandName = "getuserdataislandidfromuserid";
                cmm.CommandArguments = userId;
                string res = await TransportClient.SendCommand(cmm, _serversSettings.MainServerApiKey);
                if ((!string.IsNullOrEmpty(res)) && (res != "err"))
                {
                    return res;
                }
            }
            catch
            {
            }
            return "";
        }

        public async Task<DateTime> GetUserProAccountExpirationDate(string username)
        {
            return await MainDiCommands.GetUserProAccountExpirationDate(username, _serversSettings.MainServerApiKey);
        }

        public async Task<string> GetUserPublicKey(string username)
        {
            return await MainDiCommands.GetUserPublicKey(username, _serversSettings.MainServerApiKey);
        }

        public async Task<string> GetUserPublicKeyFromUserId(string userId)
        {
            try
            {
                TransportCommand cmm = new TransportCommand();
                cmm.CommandName = "getuserpublickeyfromuserid";
                cmm.CommandArguments = userId;
                string res = await TransportClient.SendCommand(cmm, _serversSettings.MainServerApiKey);
                if ((!string.IsNullOrEmpty(res)) && (res != "err"))
                {
                    return res;
                }
            }
            catch
            {
            }
            return "";
        }

        public async Task<string> GetDataIslandIp(string dataislandid)
        {
            try
            {
                TransportCommand cmm = new TransportCommand();
                cmm.CommandName = "getdataislandip";
                cmm.CommandArguments = dataislandid;
                string res = await TransportClient.SendCommand(cmm, _serversSettings.MainServerApiKey);
                if ((!string.IsNullOrEmpty(res)) && (res != "err"))
                {
                    return res;
                }
            }
            catch
            {
            }
            return "";
        }

        public async Task<string> GetUserID(string username)
        {
            try
            {
                TransportCommand cmm = new TransportCommand();
                cmm.CommandName = "getuserid";
                cmm.CommandArguments = username;
                string res = await TransportClient.SendCommand(cmm, _serversSettings.MainServerApiKey);
                if ((!string.IsNullOrEmpty(res)) && (res != "err"))
                {
                    return res;
                }
            }
            catch
            {
            }
            return "";
        }

        public async Task<List<string>> FindUsers(string searchPhrase)
        {
            List<string> users = new List<string>();
            try
            {
                TransportCommand cmm = new TransportCommand();
                cmm.CommandName = "findusers";
                cmm.CommandArguments = searchPhrase;
                string res = await TransportClient.SendCommand(cmm, _serversSettings.MainServerApiKey);
                if ((!string.IsNullOrEmpty(res)) && (res != "err"))
                {
                    List<UserPublicDataArgs> usersList = JsonConvert.DeserializeObject<List<UserPublicDataArgs>>(res);
                    if(usersList.Count>0)
                    {
                        foreach(var user in usersList)
                        {
                            users.Add(user.Username);
                            users.Add(user.UserId);
                            users.Add(user.DataislandID);
                            
                        }
                    }
                }
            }
            catch
            {

            }
            return users;
        }
    }
    
}
