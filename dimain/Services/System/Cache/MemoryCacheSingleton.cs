using dataislandcommon.Utilities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dimain.Services.System.Cache
{
    public class MemoryCacheSingleton : dimain.Services.System.Cache.IMemoryCacheSingleton
    {
        
        public IDataIslandSettingsService DiSettings { get; set; }
        public Dictionary<string,object> Objects { get; set; }

        public Dictionary<string, string> DataIslandsUrls
        {
            get
            {
                return (Dictionary<string, string>)this.GetOrCreate(DiConsts.MemoryCache.DataIslandUrl, new Dictionary<string, string>());
            }
        }

        public string DataIslandID
        {
            get
            {
                string diID = (string)this.GetOrCreate(DiConsts.DataIslandID, "");
                if(string.IsNullOrEmpty(diID))
                {
                    string rawid = this.DiSettings.GetSettingNonAsync(DiConsts.DataIslandID);
                    if(!string.IsNullOrEmpty(rawid))
                    {
                        this[DiConsts.DataIslandID] = rawid;
                        diID = rawid;
                    }
                }
                return diID;
            }
            set
            {
                this[DiConsts.DataIslandID] = value;
            }
        }
        public MemoryCacheSingleton()
        {
            this.Objects = new Dictionary<string, object>();
        }

        public object this[string name]
        {
            get
            {
                if (this.Objects.ContainsKey(name))
                {
                    return Objects[name];
                }
                return null;
            }

            set
            {
                Objects[name] = value;
            }
        }

        public void Remove(string name)
        {
            this.Objects.Remove(name);
        }

        public object GetOrCreate(string key, object newobject)
        {
            object obj = this[key];
            if(obj==null)
            {
                this[key] = newobject;
            }

            return this[key];
        }

    }
}
