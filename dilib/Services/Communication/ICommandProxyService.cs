using System;
namespace dataislandcommon.Services.Communication
{
    public interface ICommandProxyService
    {
        object GetCommand(string command);
        string GetCommandName(string command);
        string SerializeCommand(string commandName, object command);
    }
}
