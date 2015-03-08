using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dataislandcommon.Models.userdb
{
    public class UserSetting
    {
        [Key]
        public string Name { get; set; }
        public string Value { get; set; }
        public bool Visible { get; set; }
        public string Category { get; set; }
    }
}
