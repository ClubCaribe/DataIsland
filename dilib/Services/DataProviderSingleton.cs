using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dataislandcommon.Services
{
    public class DataProviderSingleton : dataislandcommon.Services.IDataProviderSingleton
    {

        private Dictionary<string,object> _datamodels = new Dictionary<string,object>();
        public Dictionary<string,object> DataModels
        {
            get { return _datamodels; }
        }

        public DataProviderSingleton()
        {

        }

        public object GetModel(string name)
        {
            if (DataModels.Keys.Contains(name))
            {
                return DataModels[name];
            }
            return null;
        }

        public bool DeleteModel(string name)
        {
            if (name.ToLower() != "session")
            {
                if (DataModels.Keys.Contains(name))
                {
                    DataModels.Remove(name);
                    return true;
                }
            }

            return false;
        }

        public bool SetModel(string name, object model)
        {
            try
            {
                DataModels[name] = model;
                return true;
            }
            catch
            {
            }
            return false;
        }

    }
}
