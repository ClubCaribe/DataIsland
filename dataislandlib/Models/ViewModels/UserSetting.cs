using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dataislandcommon.Models.ViewModels
{
    public class UserSetting
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool IsPublic { get; set; }
        public string Category { get; set; }
    }
}
