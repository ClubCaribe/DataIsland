using dataislandcommon.Models.DataCache;
using dimain.Services.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Newtonsoft.Json;

namespace dimain.Services.System.Cache
{
    public class DataCacheService : IDataCacheService
    {
        private readonly IMainDatabaseManagerSingleton DbManager;

        public DataCacheService(IMainDatabaseManagerSingleton _dbManager)
        {
            DbManager = _dbManager;
        }

        public async Task<DataCache> GetDataCache(string id)
        {
            using (var db = DbManager.GetMainDatabaseConnection())
            {
                var res = await db.DataCache.FindAsync(id);
                if (res != null)
                {
                    try
                    {
                        DataCache data = null;
                        data = (DataCache)JsonConvert.DeserializeObject<DataCache>(res.Data);
                        return data;
                    }
                    catch
                    {
                    }
                }
                DataCache cache = new DataCache();
                cache.Id = id;
                return cache;
            }
        }

        public async Task<bool> SaveDataCache(DataCache data)
        {
            using (var db = DbManager.GetMainDatabaseConnection())
            {
                var res = await db.DataCache.FindAsync(data.Id);
                if (res != null)
                {
                    res.Data = JsonConvert.SerializeObject(data);
                }
                else
                {
                    Models.maindb.DataCache dt = new Models.maindb.DataCache();
                    dt.Id = data.Id;
                    dt.Data = JsonConvert.SerializeObject(data);
                    db.DataCache.Add(dt);
                }
                await db.SaveChangesAsync();
                return true;
            }
        }
    }
}
