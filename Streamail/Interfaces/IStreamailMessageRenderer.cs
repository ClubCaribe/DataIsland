using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streamail.Interfaces
{
    public interface IStreamailMessageRenderer
    {
        string RenderMessage(string rawMessage);
        Task<string> RenderMessageAsync(string rawMessage);
        bool IsRendererAsync();
    }
}
