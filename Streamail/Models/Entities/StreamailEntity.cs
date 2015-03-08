using Streamail.Models.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streamail.Models.Entities
{
    public class StreamailEntity
    {
        public StreamailHeaders Headers { get; set; }
        public List<StreamailAdministrator> Administrators { get; set; }
        public List<StreamailMessage> Messages { get; set; }
        public List<Participant> Participants { get; set; }
    }
}
