using System;
using System.Collections.Generic;
namespace dimain.Services.System.Cache
{
    public interface IMemoryCacheSingleton
    {
        Dictionary<string, string> DataIslandsUrls { get; }
        string DataIslandID { get; set; }
        void Remove(string name);
        object this[string name] { get; set; }
        object GetOrCreate(string key, object newobject);
    }
}
