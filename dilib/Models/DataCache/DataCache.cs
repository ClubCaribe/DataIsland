using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dataislandcommon.Models.DataCache
{
    public class DataCache
    {
        public string Id { get; set; }
        public Dictionary<string,object> Data { get; set; }

        public string AccessToken { get; set; }
        public DateTime? AccessTokenExpirationUtc { get; set; }
        public DateTime? AccessTokenIssueDateUtc { get; set; }
        public string RefreshToken { get; set; }

        public DataCache()
        {
            Data = new Dictionary<string, object>();
        }

        public object this[string key]
        {
            get { return Data[key]; }
            set { Data[key] = value; }
        }


    }
}
