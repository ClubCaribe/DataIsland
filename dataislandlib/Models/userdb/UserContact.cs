using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dataislandcommon.Models.userdb
{
    public class UserContact
    {
        [Key]
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string InitialMessage { get; set; }
        public bool Accepted { get; set; }
        public string DataIslandId { get; set; }
        public bool RequestToAccept { get; set; }
        public bool IsFavourite { get; set; }
        
    }
}
