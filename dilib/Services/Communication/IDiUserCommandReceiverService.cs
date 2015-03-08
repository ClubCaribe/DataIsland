using System;
namespace dataislandcommon.Services.Communication
{
    public interface IDiUserCommandReceiverService
    {
        void ReceiveCommand(string command);
        string UserId { get; set; }
        string Username { get; set; }
    }
}
