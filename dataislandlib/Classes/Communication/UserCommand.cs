using dataislandcommon.Utilities;
using dimain.Services.System;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using dataislandcommon.Models.ViewModels.Communicaion;
using System.Diagnostics;
using System.Reflection;
using dimain.Services.Communication;
using Newtonsoft.Json;

namespace dataislandcommon.Classes.Communication
{
    public class UserCommand : DynamicObject
    {

        public string UserId { get; set; }
        public string SenderId { get; set; }
        public string ExecutingAssembly { get; set; }
        public override bool TryInvokeMember(
        InvokeMemberBinder binder, object[] args, out object result)
        {
            bool operationResult = true;
            using (var ioc = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                IDiCommandManagerSingleton commandManagerService = ioc.Resolve<IDiCommandManagerSingleton>();
                DiUserCommand cmm = new DiUserCommand();
                cmm.Version = 1;
                cmm.CommandName = binder.Name;
                cmm.Arguments = args;
                cmm.AssemblyName = ExecutingAssembly;
                cmm.SenderId = SenderId;

                string scommand = JsonConvert.SerializeObject(cmm);

                commandManagerService.SendCommand(UserId, scommand);

                result = operationResult;
                return true;
            }
        }
    }
}
