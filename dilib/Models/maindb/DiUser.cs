using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dimain.Models.maindb
{
    public class DiUser
    {
        [Key]
        public string Username { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime ProAccountExpirationDate { get; set; }
        public string Email { get; set; }
        public virtual List<UserRole> UserRoles { get; set; }
        public string UserId { get; set; }

    }
}
