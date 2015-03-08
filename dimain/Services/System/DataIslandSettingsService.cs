
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

using dataislandcommon.Services.db;
using dataislandcommon.Services.FileSystem;
using dataislandcommon.Services;
using dimain.Services.db;
using dimain.Models.maindb;
using dataislandcommon.Utilities;
using dataislandcommon.Services.Utilities;

namespace dimain.Services.System
{
    public class DataIslandSettingsService : IDataIslandSettingsService
    {
        private readonly IMainDatabaseManagerSingleton _dbManager;
		private readonly IFilePathProviderService _pathProvider;

		
		public IDataProviderSingleton DataProvider { get; set; }

        
        public IUtilitiesSingleton Utilities { get; set; }

        public DataIslandSettingsService(IMainDatabaseManagerSingleton dbManager, IFilePathProviderService pathProvider)
        {
            _dbManager = dbManager;
			_pathProvider = pathProvider;
        }

        public async Task<bool> SetSetting(string name, object value)
        {
            using (var db = _dbManager.GetMainDatabaseConnection())
            {
                List<MainDiSetting> res = db.MainDiSetting.Where(x => x.Name.ToLower() == name.ToLower()).ToList();
                if (res != null && res.Count > 0)
                {
                    res[0].Value = value.ToString();
                    await db.SaveChangesAsync();
                    return true;
                }
                else
                {
                    MainDiSetting stt = new MainDiSetting();
                    stt.Name = name;
                    stt.Value = value.ToString();
                    db.MainDiSetting.Add(stt);
                    await db.SaveChangesAsync();
                    return true;
                }
            }
        }

        public async Task<object> GetSetting(string name)
        {
            using (var db = _dbManager.GetMainDatabaseConnection())
            {

                MainDiSetting res = db.MainDiSetting.Find(name);
                if (res != null)
                {
                    return Utilities.ParseObjectFromString(res.Value);
                }

                return null;
            }
        }

        public Dictionary<string, object> GetAllSettingsAsDictionary()
        {
            using (var db = _dbManager.GetMainDatabaseConnection())
            {
                var res = db.MainDiSetting;
                Dictionary<string, object> results = new Dictionary<string, object>();
                foreach (var item in res)
                {
                    results[item.Name] = Utilities.ParseObjectFromString(item.Value);
                }
                return results;
            }
        }

        public string GetSettingNonAsync(string name)
        {
            using (var db = _dbManager.GetMainDatabaseConnectionNonAsync())
            {
                List<MainDiSetting> res = db.MainDiSetting.Where(x => x.Name.ToLower() == name.ToLower()).ToList();
                if (res != null && res.Count > 0)
                {
                    return res[0].Value;
                }

                return String.Empty;
            }
        }

        public async Task<bool> DeleteSetting(string name)
        {
            using (var db = _dbManager.GetMainDatabaseConnection())
            {
                await db.Database.ExecuteSqlCommandAsync("DELETE FROM MainDiSetting WHERE (Name = '" + name + "')", null);
                return true;
            }
        }

		public DbConnectionSettings GetDbConnectionSettings()
		{
            object settingsDataModel = DataProvider.GetModel(DiConsts.DatabaseSettings);
			if (settingsDataModel != null)
			{
				DbConnectionSettings stt = (DbConnectionSettings)settingsDataModel;
				return stt;
			}
			string mainPath = _pathProvider.GetMainDataPath();
			if (File.Exists(mainPath + "/databasesettings.txt"))
			{
				string fileContent = File.ReadAllText(mainPath + "/databasesettings.txt");
				DbConnectionSettings settings = JsonConvert.DeserializeObject<DbConnectionSettings>(fileContent);
                DataProvider.SetModel(DiConsts.DatabaseSettings, settings);
				return settings;
			}

			return null;
		}

		public bool SaveDbConnectionSettings(DbConnectionSettings settings)
		{
			try
			{
				string filepath = _pathProvider.GetMainDataPath() + "/databasesettings.txt";
				string filecontent = JsonConvert.SerializeObject(settings);
				File.WriteAllText(filepath, filecontent);
                DataProvider.SetModel(DiConsts.DatabaseSettings, settings);
				return true;
			}
			catch
			{
			}
			return false;
		}
    }
}
