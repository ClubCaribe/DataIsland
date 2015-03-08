using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dataislandcommon.Models.userdb
{
    public class UserAccount
    {
        [Key]
        public string Id { get; set; }
        public string UserName { get; set; }
        public DateTime RegisterDate { get; set; }
        public string Roles { get; set; }
        public string Password { get; set; }
        public DateTime ProAccountExpirationTime { get; set; }
        public string Name { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public DateTime LastLoginTime { get; set; }
        public string UiLanguage { get; set; }
        public string LanguageDirection { get; set; }
        public string Email { get; set; }
        public string CountryCode { get; set; }
        public string SecurityStamp { get; set; }
        public bool EmailConfirmed { get; set; }

    }
}
