using System;
using System.Threading.Tasks;
namespace dimain.Services.Communication
{
    public interface IDiCommandManagerSingleton
    {
        Task ReceiveCommand(string command);
        Task SendCommand(string userId, string command);
    }
}
