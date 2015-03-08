using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dimain.Models.ViewModels
{
    public class UserDataCache
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string PublicKey { get; set; }
        public string DataIslandId { get; set; }
        public string DataIslandIp { get; set; }
        public bool UserIsLocal { get; set; }
    }
}
