using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Models.db
{
    public class SharedResource
    {
        [Key]
        public string ID { get; set; }
        public string FullPath { get; set; }
        public bool IsDirectory { get; set; }
        public bool IsPublic { get; set; }
        public bool IsRead { get; set; }
        public bool IsWrite { get; set; }
        public bool IsAll { get; set; }

        [NotMapped]
        public List<ResourceRecipient> Recipients { get; set; }
    }
}
