using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dataislandcommon.Models.ViewModels.Communicaion
{
    public class DiUserCommandEnvelope
    {
        public string UserCommand { get; set; }
        public string Recipient { get; set; }
        public string Key { get; set; }
        public string Sender { get; set; }
        public string Signature { get; set; }
        public int Version { get; set; }
        
    }
}
