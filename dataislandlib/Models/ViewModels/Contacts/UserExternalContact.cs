using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dataislandcommon.Models.ViewModels.Contacts
{
    public class UserExternalContact
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string AvatarPath { get; set; }
        public string DataislandId { get; set; }
    }
}
