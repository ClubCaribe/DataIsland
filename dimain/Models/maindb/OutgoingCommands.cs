using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dimain.Models.maindb
{
    public class OutgoingCommands
    {
        [Key]
        public string Id { get; set; }
        public string User { get; set; }
        public DateTime SendTime { get; set; }
        public int CommandSequence { get; set; }
        public string Command { get; set; }
    }
}
