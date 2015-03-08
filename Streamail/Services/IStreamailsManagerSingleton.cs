using Streamail.Interfaces;
using System;
using System.Threading.Tasks;
namespace Streamail.Services
{
    public interface IStreamailsManagerSingleton
    {
        void AddRenderer(string name, IStreamailMessageRenderer renderer);
        Task<string> RenderMessage(string rendererName, string rawMessage);
    }
}
