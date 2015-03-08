using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dataislandcommon.Models.ViewModels.Communicaion
{
    public class DiUserCommand
    {
        public string SenderId { get; set; }
        public string CommandName { get; set; }
        public object[] Arguments { get; set; }
        public string AssemblyName { get; set; }
        public int Version { get; set; }
    }
}
