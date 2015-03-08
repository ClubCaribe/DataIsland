using dataislandcommon.Models.DataCache;
using System;
using System.Threading.Tasks;
namespace dimain.Services.System.Cache
{
    public interface IDataCacheService
    {
        Task<DataCache> GetDataCache(string id);
        Task<bool> SaveDataCache(DataCache data);
    }
}
