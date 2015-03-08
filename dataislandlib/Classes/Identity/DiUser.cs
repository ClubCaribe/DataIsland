using dataislandcommon.Models.userdb;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dataislandcommon.Classes.Identity
{
    public class DiUser : IUser
    {
        public DiUser()
        {
            this.UserRoles = new List<string>();
            userAccount = new UserAccount();
            userAccount.Id = Guid.NewGuid().ToString();
        }

        public DiUser(string userName)
            : this()
	    {
	    }

        public void SetUser(UserAccount account)
        {
            userAccount = account;
        }

        public UserAccount userAccount { get; set; }

        public List<string> UserRoles { get; set; }

        public virtual string Id
        {
            get
            {
                return userAccount.Id;
            }
        }


        public virtual string UserName
        {
            get
            {
                return userAccount.UserName;
            }
            set
            {
                userAccount.UserName = value;
            }
        }

        public string Email {
            get
            {
                return userAccount.Email;
            }
            set
            {
                userAccount.Email = value;
            }
        }

        public string SecurityStamp
        {
            get
            {
                return userAccount.SecurityStamp;
            }
            set
            {
                userAccount.SecurityStamp = value;
            }
        }

        public string PasswordHash
        {
            get
            {
                return userAccount.Password;
            }
            set
            {
                userAccount.Password = value;
            }
        }
    }
}
