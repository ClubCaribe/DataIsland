using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dataislandcommon.Models.ViewModels
{
	public class UserDetails
	{
		public string Username { get; set; }
		public string Name { get; set; }
		public DateTime LastLoginTime { get; set; }
		public DateTime ProAccountExpirationTime { get; set; }
        public string Email { get; set; }
	}
}
