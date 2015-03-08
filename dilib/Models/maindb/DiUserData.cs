using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dimain.Models.maindb
{
    public class DiUserData
    {
        [Key]
        public string UserId { get; set; }
        public string DatIslandId { get; set; }
        public DateTime LastUpdate { get; set; }
        public string PublicKey { get; set; }
    }
}
