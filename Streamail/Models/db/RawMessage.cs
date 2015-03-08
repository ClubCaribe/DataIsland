using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streamail.Models.db
{
    public class RawMessage
    {
        [Key]
        public string MessageId { get; set; }
        public string Message { get; set; }
    }
}
