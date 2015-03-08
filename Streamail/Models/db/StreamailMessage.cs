using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streamail.Models.db
{
    public class StreamailMessage
    {
        [Key]
        public string ID { get; set; }
        public string StreamailID { get; set; }
        public string Message { get; set; }
        public DateTime SendTime { get; set; }
        public DateTime ReceiveTime { get; set; }
        public string ParentMessageID { get; set; }
        public string RootMessageID { get; set; }
        public string RendererName { get; set; }
        public bool IsRead { get; set; }
        public string SenderID { get; set; }

        [NotMapped]
        public List<StreamailMessage> ChildrenMessages { get; set; }
        [NotMapped]
        public List<Participant> Participants { get; set; }
        [NotMapped]
        public List<ReadStatus> ReadStatuses { get; set; }
        [NotMapped]
        public Dictionary<string,object> CustomFields { get; set; }

    }
}
