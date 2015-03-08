using System;
using System.Threading.Tasks;
namespace dataislandcommon.Services.Communication
{
    public interface IDiUserCommandSenderService
    {
        string UserId { get; set; }
        string DataIslandPublicKey { get; set; }
        string UserPublicKey { get; set; }
        string DataIslandUrl { get; set; }
        string Url { get; set; }
        string DataIslandId { get; set; }
        Task<bool> RefreshUserData();
        Task SendCommands();
        void SendCommand(string command);
        
    }
}
