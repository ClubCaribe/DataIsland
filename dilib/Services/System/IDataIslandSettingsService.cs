using dimain.Models.maindb;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace dimain.Services.System
{
    public interface IDataIslandSettingsService
    {
        Task<bool> DeleteSetting(string name);
        Task<object> GetSetting(string name);
        Dictionary<string, object> GetAllSettingsAsDictionary();
        string GetSettingNonAsync(string name);
        Task<bool> SetSetting(string name, object value);
		DbConnectionSettings GetDbConnectionSettings();
		bool SaveDbConnectionSettings(DbConnectionSettings settings);
    }
}
