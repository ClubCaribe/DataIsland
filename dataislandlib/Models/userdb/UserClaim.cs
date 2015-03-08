using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dataislandcommon.Models.userdb
{
    public class UserClaim
    {
        [Key]
        public string Id { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
