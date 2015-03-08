using System;
namespace dataislandcommon.Services
{
    public interface IDataProviderSingleton
    {
        bool DeleteModel(string name);
        object GetModel(string name);
        bool SetModel(string name, object model);
    }
}
