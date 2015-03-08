using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streamail.Models.db
{
    public class ReadStatus
    {
        [Key]
        public string ID { get; set; }
        public string MessageID { get; set; }
        public string StreamailID { get; set; }
        public string ParticipantID { get; set; }
    }
}
