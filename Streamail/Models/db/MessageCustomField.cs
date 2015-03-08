using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streamail.Models.db
{
    public class MessageCustomField
    {
        [Key]
        public string ID { get; set; }
        public string MessageID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string ValueType { get; set; }
    }
}
