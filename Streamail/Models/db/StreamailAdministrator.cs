using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streamail.Models.db
{
    public enum StreamailAdministrationRole
    {
        Owner,
        Administrator,
        Editor
    }
    public class StreamailAdministrator
    {
        public string ID { get; set; }
        public string StreamailID { get; set; }
        public string ParticipantID { get; set; }
        public string ParticipantName { get; set; }
        public StreamailAdministrationRole  Role { get; set; }
    }
}
