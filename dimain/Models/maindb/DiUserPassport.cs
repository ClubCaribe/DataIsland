using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace dimain.Models.maindb
{
    public class DiUserPassport
    {
        [Key]
        public string Id { get; set; }
        public string PassportData { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
