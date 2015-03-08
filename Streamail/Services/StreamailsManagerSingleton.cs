using Streamail.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Streamail.Services
{
    public class StreamailsManagerSingleton : Streamail.Services.IStreamailsManagerSingleton
    {
        public Dictionary<string, IStreamailMessageRenderer> Renderers { get; set; }

        public StreamailsManagerSingleton()
        {
        }

        public void AddRenderer(string name, IStreamailMessageRenderer renderer)
        {
            this.Renderers[name] = renderer;
        }

        public async Task<string> RenderMessage(string rendererName, string rawMessage)
        {
            if (this.Renderers.ContainsKey(rendererName))
            {
                IStreamailMessageRenderer renderer = this.Renderers[rendererName];
                if(renderer.IsRendererAsync())
                {
                    return await renderer.RenderMessageAsync(rawMessage);
                }
                return this.Renderers[rendererName].RenderMessage(rawMessage);
            }
            return String.Empty;
        }


       
    }
}
