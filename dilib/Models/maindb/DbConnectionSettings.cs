using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dimain.Models.maindb
{
	public class DbConnectionSettings
	{

		private Dictionary<string,DbConnectionSetting> _dbSettings = new Dictionary<string,DbConnectionSetting>();
		public Dictionary<string,DbConnectionSetting> DbSettings
		{
			get { return _dbSettings; }
			set { _dbSettings = value; }
		}
	}
}
