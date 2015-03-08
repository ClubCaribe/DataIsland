using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dimain.Models.maindb
{
	public class DbConnectionSetting
	{
		public string DatabaseProvider { get; set; }
		public string ConnectionString { get; set; }
		public string ManifestToken { get; set; }
	}
}
