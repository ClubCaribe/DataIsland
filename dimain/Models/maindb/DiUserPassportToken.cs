using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dimain.Models.maindb
{
    public class DiUserPassportToken
    {
        [Key]
        public string ID { get; set; }
        public string UserID { get; set; }
        public DateTime ExpirationTime { get; set; }
    }
}
