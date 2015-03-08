using dataislandcommon.Classes.Communication;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace dataislandcommon.Services.Communication
{
    public class DICommandsService : dilib.Services.Communication.IDICommandsService
    {
        public DICommandsService()
        {

        }

        public dynamic User(string senderId, string userId)
        {
            UserCommand command = new UserCommand();
            command.UserId = userId;
            command.SenderId = senderId;
            try
            {
                var stackFrames = new StackTrace().GetFrames();
                command.ExecutingAssembly = stackFrames[1].GetMethod().DeclaringType.Assembly.GetName().Name;
            }
            catch
            {
            }

            
            return command;
        }


    }
}
