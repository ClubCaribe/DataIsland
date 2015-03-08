using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using scutils.transportprotocol;

namespace dimain.Services.Communication
{
    public class ServersApiSettingsSingleton : dimain.Services.Communication.IServersApiSettingsSingleton
    {


        private MainServerApiSettings _mainServerApiKey = new MainServerApiSettings();

        public MainServerApiSettings MainServerApiKey
        {
            get { return _mainServerApiKey; }
            set { _mainServerApiKey = value; }
        }

        public ServersApiSettingsSingleton()
        {
            //_mainServerApiKey.ApiBaseAddress = "http://localhost:49449/";
        }
    }
}
