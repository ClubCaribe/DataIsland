using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dimain.Models.maindb
{
    public class UserRole
    {
        [Key]
        public string ID { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
    }
}
