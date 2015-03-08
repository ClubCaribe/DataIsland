using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using dataislandcommon.Models.Communication;

namespace dataislandcommon.Services.Communication
{
    public class CommandProxyService : dataislandcommon.Services.Communication.ICommandProxyService
    {
        public CommandProxyService()
        {

        }

        public string SerializeCommand(string commandName, object command)
        {
            string serializedCommand = JsonConvert.SerializeObject(command);

            CommandProxy proxy = new CommandProxy();
            proxy.CommandName = commandName;
            proxy.Command = Convert.ToBase64String(Encoding.UTF8.GetBytes(serializedCommand));

            return JsonConvert.SerializeObject(proxy);
        }

        public string GetCommandName(string command)
        {
            CommandProxy proxy = (CommandProxy)JsonConvert.DeserializeObject(command);
            return proxy.CommandName;
        }

        public object GetCommand(string command)
        {
            CommandProxy proxy = (CommandProxy)JsonConvert.DeserializeObject(command);
            string serializedCommand = Encoding.UTF8.GetString(Convert.FromBase64String(proxy.Command));
            return JsonConvert.DeserializeObject(serializedCommand);
        }
    }
}
