using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dimain.Models.maindb
{
    public class DiUserExternalLogin
    {
        [Key]
        public string ProviderKey { get; set; }
        public string UserName { get; set; }
    }
}
