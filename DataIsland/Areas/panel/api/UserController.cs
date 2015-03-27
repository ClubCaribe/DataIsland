using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using dataislandcommon.Models.ViewModels;
using dataislandcommon.Services.System;

using dataislandcommon.Attributes;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http.Headers;
using System.IO;
using Newtonsoft.Json.Linq;
using dataislandcommon.Services.FileSystem;
using DataIsland.Classes;
using DataIsland.Models;
using Newtonsoft.Json;
using dimain.Services.System;
using dataislandcommon.Models.DataCache;
using System.Web.Http.Owin;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using dataislandcommon.Classes.Identity;
using dimain.Services.System.Cache;
using dataislandcommon.Utilities;
using dimain.Models.maindb;
using dimain.Models.ViewModels;

namespace DataIsland.Areas.panel.api
{
    [Authorize]
    [RoutePrefix("api/panel/user")]
    public class UserController : ApiController
    {
        
        public IDiUserService DiUsers { get; set; }

		
		public IUserService UserService { get; set; }

        
        public IFilePathProviderService PathProvider { get; set; }

        
        public IDataCacheService DataCacheService { get; set; }

        
        public IUserContactsService Contacts { get; set; }

        public IDataIslandSettingsService DiSettings { get; set; }

        public IUserPassportTokensSingleton PassportTokenSingleton { get; set; }

        public IDataIslandService DataIslandService { get; set; }

        public ApplicationUserManager UserManager
        {
            get
            {
                return Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }

        }


        public UserController()
        {
			
        }

		

        [Route("isuserloggedin")]
        [HttpGet]
        public bool IsUserLoggedIn()
        {
            return this.User.Identity.IsAuthenticated;
        }

		[Route("getuserdetails")]
		[HttpGet]
		public async Task<UserDetails> GetUserDetails()
		{
            UserDetails details = await UserService.GetUserDetails(this.User.Identity.Name);
			return details;
		}

        [Route("getusersettings")]
        [HttpGet]
        public async Task<Dictionary<string, dataislandcommon.Models.ViewModels.UserSetting>> GetUserSettings()
        {
            Dictionary<string, dataislandcommon.Models.ViewModels.UserSetting> settings = await UserService.GetUserSettings(this.User.Identity.Name);
            return settings;
        }

        [Route("saveusersettings")]
        [HttpPost]
        public async Task<bool> SaveUserSettings(Dictionary<string,dataislandcommon.Models.ViewModels.UserSetting> settings)
        {
            return await UserService.SaveUserSettings(this.User.Identity.Name, settings);
        }

        [Route("uploadavatar")]
        [HttpPost]
        public async Task<bool> SetUserAvatar()
        {
            if (Request.Content.IsMimeMultipartContent())
            {
                string uploadPath = PathProvider.GetUserUploadFolder(this.User.Identity.Name);

                MultipartFormDataStreamProvider streamProvider = new MultipartFormDataStreamProvider(uploadPath);
                await Request.Content.ReadAsMultipartAsync(streamProvider);

                if (streamProvider.FileData.Count > 0)
                {
                    byte[] avatar = File.ReadAllBytes(streamProvider.FileData[0].LocalFileName);
                    File.Delete(streamProvider.FileData[0].LocalFileName);
                    if (UserService.SaveUserAvatar(this.User.Identity.Name, avatar))
                    {
                        await this.Contacts.SendUpdateAvatarCommandToContacts(avatar, this.User.Identity.Name);
                        return true;
                    }
                }
            }
            return false;
        }

        [Route("updatepassword")]
        [HttpPost]
        public async Task<bool> SetUserPassword(JObject jsonData)
        {
            try
            {
                dynamic postData = jsonData;
                string newpassword = postData.newPassword;
                string userId = (await DiUsers.GetUserByUsername(this.User.Identity.Name)).Id;
                DiUserStore store = new DiUserStore();
                String hashedNewPassword = UserManager.PasswordHasher.HashPassword(newpassword);
                dataislandcommon.Classes.Identity.DiUser cUser = await store.FindByIdAsync(userId);
                await store.SetPasswordHashAsync(cUser, hashedNewPassword);
                await store.UpdateAsync(cUser);
                return true;
            }
            catch
            {

            }
            return false;
        }

        [Route("refreshapitoken")]
        [HttpGet]
        public async Task<object> RefreshToken()
        {
            try
            {
                dataislandcommon.Models.DataCache.DataCache cache = await DataCacheService.GetDataCache(this.User.Identity.Name);
                HttpClient cl = new HttpClient();

                List<KeyValuePair<string, string>> formargs = new List<KeyValuePair<string, string>>();
                formargs.Add(new KeyValuePair<string, string>("grant_type", "refresh_token"));
                formargs.Add(new KeyValuePair<string, string>("refresh_token", cache.RefreshToken));
                formargs.Add(new KeyValuePair<string, string>("client_id", "diHttpApp"));
                HttpContent cnt = new FormUrlEncodedContent(formargs);

                var oDomain = await this.DiSettings.GetSetting(DiConsts.DataIslandDomain);
                string dataIslandDomain = "http://localhost";
                if (oDomain != null)
                {
                    dataIslandDomain = (string)oDomain;
                }

                HttpResponseMessage resp = await cl.PostAsync(dataIslandDomain+"/token", cnt);
                if (resp.IsSuccessStatusCode)
                {
                    string data = await resp.Content.ReadAsStringAsync();
                    TokenResponseModel tokenResponse = JsonConvert.DeserializeObject<TokenResponseModel>(data);
                    cache.AccessToken = tokenResponse.access_token;
                    cache.AccessTokenExpirationUtc = DateTime.UtcNow.AddSeconds(int.Parse(tokenResponse.expires_in));
                    cache.RefreshToken = tokenResponse.refresh_token;

                    await DataCacheService.SaveDataCache(cache);
                    
                    return new { result = true, newToken = cache.AccessToken, expirationTime = cache.AccessTokenExpirationUtc };
                }
                

            }
            catch
            {
            }

            return new { result = false };
        }

        [Route("getpassporttoken")]
        [HttpPost]
        public async Task<ClientPassportToken> GetPassportToken(JObject jsonData)
        {
            dynamic postData = jsonData;
            string dataislandId = postData.dataislandId;
            dataislandcommon.Models.DataCache.DataCache cache = await DataCacheService.GetDataCache(this.User.Identity.Name);
            List<ClientPassportToken> tokens = null;
            if (cache.Data.ContainsKey("PassportTokens"))
            {
                try
                {
                    tokens = ((JArray)cache.Data["PassportTokens"]).ToObject<List<ClientPassportToken>>();
                }
                catch
                {
                    int i;
                    i = 10;
                }
            }
            else
            {
                tokens = new List<ClientPassportToken>();
            }
            bool saveTokens = false;
            DateTime timeTreshlod = DateTime.UtcNow;
            for (int i = tokens.Count - 1; i > -1; i--)
            {
                ClientPassportToken token = tokens[i];
                if (token.DataIslandID == dataislandId && token.ExpirationTime>timeTreshlod.AddSeconds(5))
                {
                    if (saveTokens)
                    {
                        cache.Data["PassportTokens"] = tokens;
                        await DataCacheService.SaveDataCache(cache);
                    }
                    return token;
                }
                if(token.ExpirationTime<timeTreshlod.AddSeconds(5))
                {
                    tokens.RemoveAt(i);
                    saveTokens = true;
                }
            }
            string diUrl = await this.DataIslandService.GetDataislandUrl(dataislandId);
            if (diUrl == "/")
            {
                string userId = await this.DiUsers.GetUserIdByFromUsername(this.User.Identity.Name);
                if(!string.IsNullOrEmpty(userId))
                {
                    DiUserPassportToken token = await this.PassportTokenSingleton.GeneratePassportToken(userId);
                    if (token != null)
                    {
                        
                        ClientPassportToken clientToken = new ClientPassportToken();
                        clientToken.DataIslandID = dataislandId;
                        clientToken.ExpirationTime = token.ExpirationTime;
                        clientToken.TokenID = token.ID;

                        tokens.Add(clientToken);
                        cache.Data["PassportTokens"] = tokens;
                        await DataCacheService.SaveDataCache(cache);

                        return clientToken;
                    }
                }
            }

            if (saveTokens)
            {
                cache.Data["PassportTokens"] = tokens;
                await DataCacheService.SaveDataCache(cache);
            }
            return null;
        }

        [Route("getuserdataislanddata")]
        [HttpPost]
        public async Task<ClientDataIslandData> GetUserDataIslandData(JObject jsonData)
        {
            dynamic postData = jsonData;
            string userId = postData.userId;
            
            DiUserData userData = await this.DiUsers.GetDIUserDataFromUserId(userId);
            if (userData != null)
            {
                ClientDataIslandData cdata = new ClientDataIslandData();
                cdata.DataislandID = userData.DatIslandId;
                cdata.DataislandUrl = await this.DataIslandService.GetDataislandUrl(userData.DatIslandId);
                cdata.UserID = userId;
                return cdata;
            }
            return null;
        }

        [Route("getdataislandurlfromuserid")]
        [HttpPost]
        public async Task<string> GetDataIslandUrlFromUserId(string userId)
        {
            DiUserData userData = await this.DiUsers.GetDIUserDataFromUserId(userId);
            if (userData != null)
            {
                string url = await this.DataIslandService.GetDataislandUrl(userData.DatIslandId);
                return url;
            }
            return "err";
        }

        [Route("getmenu")]
        [HttpGet]
        public Dictionary<string, List<MenuEntry>> GetUserMenus()
        {
            Dictionary<string, List<MenuEntry>> menus = new Dictionary<string, List<MenuEntry>>();
            if (this.User.IsInRole("admin") || this.User.IsInRole("all"))
            {
                menus["DataIsland"] = MenuProvider.GetDiAdminMenu();
            }
            else
            {
                menus["DataIsland"] = new List<MenuEntry>();
            }
            return menus;
        }
    }
}