using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dataislandcommon.Models.userdb
{
    public class UserExternalLogin
    {
        [Key]
        public string ProviderKey { get; set; }
        public string LoginProvider { get; set; }
    }
}
