using dataislandcommon.Classes.Attributes;
using dataislandcommon.Models.ViewModels.Communicaion;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace dataislandcommon.Services.Communication
{
    public class DiUserCommandReceiverService : dataislandcommon.Services.Communication.IDiUserCommandReceiverService
    {
        private Queue<string> commands = null;
        private bool isReceivingCommands = false;
        public string UserId { get; set; }
        public string Username { get; set; }

        public DiUserCommandReceiverService()
        {
            commands = new Queue<string>();
        }

        public void ReceiveCommand(string command)
        {
            this.commands.Enqueue(command);
            Task.Factory.StartNew(async () => await this.ReceiveCommands());
        }

        private async Task ReceiveCommands()
        {
            if(this.isReceivingCommands)
            {
                return;
            }
            this.isReceivingCommands = true;
            while(commands.Count>0)
            {
                string commandString = commands.Dequeue();
                if(!string.IsNullOrEmpty(commandString))
                {
                    try
                    {
                        DiUserCommand cmm = JsonConvert.DeserializeObject<DiUserCommand>(commandString);
                        Assembly assembly = this.GetAssemblyFromName(cmm.AssemblyName);
                        
                        if(assembly!=null)
                        {
                            await this.InvokeCommand(assembly, cmm.CommandName, cmm.Arguments,cmm.SenderId,cmm.Version);
                        }
                    }
                    catch
                    {

                    }
                }
            }
            this.isReceivingCommands = false;
        }

        private Assembly GetAssemblyFromName(string assemblyName)
        {
            try
            {
                foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if(assembly.GetName().Name == assemblyName)
                    {
                        return assembly;
                    }
                }
            }
            catch
            {

            }
            return null;
        }

        private async Task<bool> InvokeCommand(Assembly assembly, string commandName, object[] arguments, string senderId, int commandVersion)
        {
            foreach(Type type in assembly.GetTypes())
            {
                Attribute attr = type.GetCustomAttribute(typeof(DataIslandCommandsAttribute));
                if(attr!=null)
                {
                    PropertyInfo ReceiverId = type.GetProperty("UserId");
                    PropertyInfo ReceiverUsername = type.GetProperty("Username");
                    PropertyInfo SenderId = type.GetProperty("SenderId");
                    PropertyInfo CommandVersion = type.GetProperty("CommandVersion");
                    if (SenderId != null && CommandVersion != null && ReceiverId !=null && ReceiverUsername != null)
                    {
                        foreach (MethodInfo method in type.GetMethods())
                        {
                            try
                            {
                                if (method.Name == commandName)
                                {
                                    ParameterInfo[] parametersInfo = method.GetParameters();
                                    object[] methodParams = new object[parametersInfo.Length];
                                    for(int i=0; i<parametersInfo.Length; i++)
                                    {
                                        if (i < arguments.Length)
                                        {
                                            try
                                            {
                                                if (arguments[i] == null)
                                                {
                                                    methodParams[i] = null;
                                                }
                                                else
                                                {
                                                    ParameterInfo parameter = parametersInfo[i];
                                                    if (arguments[i].GetType() == typeof(JObject))
                                                    {
                                                        JObject argObject = (JObject)arguments[i];
                                                        methodParams[i] = argObject.ToObject(parameter.ParameterType);
                                                    }
                                                    else
                                                    {
                                                        if (parameter.ParameterType == typeof(byte[]))
                                                        {
                                                            methodParams[i] = Convert.FromBase64String(arguments[i].ToString());
                                                        }
                                                        else
                                                        {
                                                            methodParams[i] = Convert.ChangeType(arguments[i], parameter.ParameterType);
                                                        }
                                                    }
                                                }
                                                
                                            }
                                            catch
                                            {
                                                methodParams[i] = arguments[i];
                                            }
                                        }
                                    }
                                    object invoker = Activator.CreateInstance(type);
                                    SenderId.SetValue(invoker, senderId);
                                    CommandVersion.SetValue(invoker, commandVersion);
                                    ReceiverId.SetValue(invoker, UserId);
                                    ReceiverUsername.SetValue(invoker, this.Username);
                                    if (method.ReturnType == typeof(Task))
                                    {
                                        await (Task)method.Invoke(invoker, methodParams);
                                    }
                                    else
                                    {
                                        method.Invoke(invoker, methodParams);
                                    }
                                    return true;
                                }
                            }
                            catch
                            {

                            }
                        }
                    }
                }
            }

            return false;
        }
        
    }
}
