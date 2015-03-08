using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streamail.Models.db
{
    public class StreamailHeaders
    {
        [Key]
        public string ID { get; set; }
        public string Subject { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastModificationTime { get; set; }
        public string StreamailType { get; set; }
        public bool ReadOnly { get; set; }
        public bool CanAddSubmessages { get; set; }
        public int SubMessagesDepth { get; set; }
        public bool SendReadNotifications { get; set; }
        public string MessageSenderName { get; set; }
    }
}
