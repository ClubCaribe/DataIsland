using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Models.db
{
    public class ResourceRecipient
    {
        [Key]
        public string ID { get; set; }
        public string ResourceID { get; set; }
        public string RecipientID { get; set; }
        public bool Deleted { get; set; }
    }
}
