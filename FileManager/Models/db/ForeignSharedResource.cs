using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Models.db
{
    public class ForeignSharedResource
    {
        public string ID { get; set; }
        public string OwnerID { get; set; }
        public string Name { get; set; }
        public bool IsDirectory { get; set; }
        public bool IsAccessible { get; set; }
    }
}
