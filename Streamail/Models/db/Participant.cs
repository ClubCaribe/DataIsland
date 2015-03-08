using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streamail.Models.db
{
    public class Participant
    {
        [Key]
        public string ID { get; set; }
        public string ParticipantID { get; set; }
        public string ParticipantName { get; set; }
        public string EntityID { get; set; }
        public bool IsSender { get; set; }
        public bool IsRead { get; set; }
    }
}
