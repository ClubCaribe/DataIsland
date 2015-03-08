using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dimain.Models.maindb
{
    public class DataIslandData
    {
        [Key]
        public string Id { get; set; }
        public string PublicKey { get; set; }
        public string Url { get; set; }
        public DateTime LastUpdate { get; set; }

    }
}
