using dataislandcommon.Models.System;
using dataislandcommon.Services;
using dataislandcommon.Services.Utilities;
using dataislandcommon.Utilities;
using dimain.Services.Communication;
using dimain.Services.db;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using dimain.Models.maindb;
using dimain.Services.System.Cache;

namespace dimain.Services.System
{
    public class DataIslandService : dimain.Services.System.IDataIslandService
    {
        
        public IMainDiCommandsService MainDiCommands { get; set; }

        
        public ICryptographySingleton Cryptography { get; set; }

        
        public IMainDatabaseManagerSingleton MainDbManager { get; set; }

        
        public IMemoryCacheSingleton MemoryCache { get; set; }

        private readonly IDataIslandSettingsService _settings;

        private readonly IDataProviderSingleton _dataProvider;

        public DataIslandService(IDataIslandSettingsService settings, IDataProviderSingleton dataProvider)
        {
            _settings = settings;
            _dataProvider = dataProvider;
        }

        public void InitDataIslandStateObject()
        {
            DataIslandState distate = new DataIslandState();
            _dataProvider.SetModel(DiConsts.DataIslandSate, distate);
        }

        public DataIslandState GetDataIslandStateObject()
        {
            return (DataIslandState)_dataProvider.GetModel(DiConsts.DataIslandSate);
        }

        public void SetRootDocumentPath(string path)
        {
            DataIslandState state = GetDataIslandStateObject();
            state.RootFilePath = path;
        }

        public async Task StartDataIsland()
        {

            bool isInitialised = (bool)((await _settings.GetSetting(DiConsts.DataIslandIsInitialised))??false);
            if (isInitialised)
            {
                string dataIslandID = await GetDataIslandID();
                await MainDiCommands.SetDataIslandIp(dataIslandID);
            }

            DataIslandState distate = GetDataIslandStateObject();
            distate.IsInitialized = ((isInitialised) ? true : false);
            
        }

        private async Task<bool> GenerateDataIslandKeys(string password)
        {
            List<string> keys = Cryptography.GenerateRsaKeys(password);
            if (keys != null && keys.Count > 0)
            {
                await _settings.SetSetting(DiConsts.PublicKey, keys[0]);
                await _settings.SetSetting(DiConsts.PrivateKey, keys[1]);
                return true;
            }

            return false;
        }

        public async Task<bool> RegisterDataIsland(string name, string description, string domain, string webpage, bool isPublic, string adminEmail)
        {
            string password = Guid.NewGuid().ToString();
            List<string> keys = Cryptography.GenerateRsaKeys(password);

            RegisterDataiSlandArgs args = new RegisterDataiSlandArgs();
            args.DataIslandID = Guid.NewGuid().ToString();
            args.Description = description;
            args.Domain = domain;
            args.ID = args.DataIslandID;
            args.IsPublic = isPublic;
            args.Name = name;
            args.PublicKey = keys[0];
            args.WebPage = webpage;

            var res = await MainDiCommands.RegisterDataIsland(args);
            if (res)
            {
                await _settings.SetSetting(DiConsts.DataIslandID, args.DataIslandID);
                await _settings.SetSetting(DiConsts.DataIslandName, name);
                await _settings.SetSetting(DiConsts.DataIslandPassword, password);
                await _settings.SetSetting(DiConsts.PublicKey, keys[0]);
                await _settings.SetSetting(DiConsts.PrivateKey, keys[1]);
                await _settings.SetSetting(DiConsts.DataIslandDomain, domain);
                await _settings.SetSetting(DiConsts.DataIslandWebPage, webpage);
                await _settings.SetSetting(DiConsts.DataIslandIsPublic, ((isPublic) ? "true" : "false"));
                await _settings.SetSetting(DiConsts.DataIslandAdminEmail, adminEmail);
                await _settings.SetSetting(DiConsts.DataIslandIsInitialised, "true");

                //update state in state object

                ((DataIslandState)_dataProvider.GetModel(DiConsts.DataIslandSate)).IsInitialized = true;

                return true;
            }

            return false;
        }

        public async Task<string> GetDataIslandID()
        {
            return ((await _settings.GetSetting(DiConsts.DataIslandID))??"").ToString();
        }

        public bool IsDataIslandInitialised()
        {
            DataIslandState state = GetDataIslandStateObject();
            return state.IsInitialized;
        }

        public async Task<RSACryptoServiceProvider> GetDataIslandPublicRsaCryproProvider()
        {
            string publicKey = (await _settings.GetSetting(DiConsts.PublicKey)).ToString();
            RSACryptoServiceProvider provider = Cryptography.GetRsaServiceProviderFromPemKey(publicKey);
            return provider;
        }

        public RSACryptoServiceProvider GetDataIslandPublicRsaCryproProviderNonAsync()
        {
            string publicKey = _settings.GetSettingNonAsync(DiConsts.PublicKey);
            RSACryptoServiceProvider provider = Cryptography.GetRsaServiceProviderFromPemKey(publicKey);
            return provider;
        }

        public async Task<RSACryptoServiceProvider> GetDataIslandPrivateRsaCryptoProvider()
        {
            string password = (await _settings.GetSetting(DiConsts.DataIslandPassword)).ToString();
            string privateKey = (await _settings.GetSetting(DiConsts.PrivateKey)).ToString();
            RSACryptoServiceProvider provider = Cryptography.GetRsaServiceProviderFromPemKey(privateKey, password);
            return provider;
        }

        public RSACryptoServiceProvider GetDataIslandPrivateRsaCryptoProviderNonAsync()
        {
            string password = _settings.GetSettingNonAsync(DiConsts.DataIslandPassword);
            string privateKey = _settings.GetSettingNonAsync(DiConsts.PrivateKey);
            RSACryptoServiceProvider provider = Cryptography.GetRsaServiceProviderFromPemKey(privateKey, password);
            return provider;
        }

        public async Task<string> GetDataIslandPublicKey(string dataislandid)
        {
            string diPublicKey;
            var db = MainDbManager.GetMainDatabaseConnection();
            var res = await db.DataislandData.Where(x => x.Id == dataislandid).ToListAsync();
            if(res.Count>0)
            {
                if (res[0].LastUpdate.AddDays(1) > DateTime.Now)
                {
                    return res[0].PublicKey;
                }
                else
                {
                    diPublicKey = await this.MainDiCommands.GetUserDataIslandPublicKey(dataislandid);
                    if (!string.IsNullOrEmpty(diPublicKey) && diPublicKey != "err")
                    {
                        res[0].PublicKey = diPublicKey;
                        res[0].Url = await this.MainDiCommands.GetDataIslandIp(dataislandid);

                        await db.SaveChangesAsync();

                        return res[0].PublicKey;
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            diPublicKey = await this.MainDiCommands.GetUserDataIslandPublicKey(dataislandid);
            if(!string.IsNullOrEmpty(diPublicKey) && diPublicKey!="err")
            {
                DataIslandData data = new DataIslandData();
                data.Id = dataislandid;
                data.PublicKey = diPublicKey;
                data.Url = await this.MainDiCommands.GetDataIslandIp(dataislandid);
                data.LastUpdate = DateTime.Now;

                db.DataislandData.Add(data);

                await db.SaveChangesAsync();

                return data.PublicKey;
            }

            return null;
        }

        public async Task<string> GetDataislandUrl(string dataislandId)
        {
            if(this.MemoryCache.DataIslandID == dataislandId)
            {
                return "/";
            }
            Dictionary<string, string> urls = this.MemoryCache.DataIslandsUrls;
            if (urls.ContainsKey(dataislandId))
            {
                return urls[dataislandId];
            }
            string dataislandUrl = await this.MainDiCommands.GetDataIslandIp(dataislandId);
            urls[dataislandId] = dataislandUrl;
            return dataislandUrl;
        }
    }
}
