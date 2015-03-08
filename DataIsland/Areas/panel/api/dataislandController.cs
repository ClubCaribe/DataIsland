using dataislandcommon.Services.System;
using dataislandcommon.Attributes;
using dataislandcommon.Models.ViewModels.SettingsForm;
using dataislandcommon.Services.FileSystem;
using dataislandcommon.Services.Utilities;
using dataislandcommon.Utilities;
using dataislandcommon.Utilities.enums;
using dimain.Models.maindb;
using dimain.Services.System;

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Owin;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using dataislandcommon.Classes.Identity;

namespace DataIsland.Areas.panel.api
{
    [Authorize(Roles = "admin,all")]
    [RoutePrefix("api/panel/dataisland")]
    public class dataislandController : ApiController
    {
        
        public IDiUserService DiUsers { get; set; }

        
        public IUserRoleService UserRoles { get; set; }

        
        public IUserService UserService { get; set; }

        
        public IDataIslandService DataIsland { get; set; }

        
        public IFilePathProviderService FilePathProvider { get; set; }

        
        public IUIUtilitiesService UIUtilities { get; set; }

        
        public IDataIslandSettingsService DiSettings { get; set; }

        public ApplicationUserManager UserManager
        {
            get
            {
                return Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }

        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return Request.GetOwinContext().Get<ApplicationSignInManager>();
            }
        }

        public dataislandController()
        {

        }

        [Route("getusers")]
        [HttpPost]
        public async Task<List<dimain.Models.maindb.DiUser>> GetUsers(JObject jsonData)
        {
            int pageIndex, numOfElements;
            string searchPhrase;
            dynamic postData = jsonData;
            pageIndex = postData.pageIndex;
            numOfElements = postData.numOfElements;
            searchPhrase = postData.searchPhrase;
            return await DiUsers.GetUsers(pageIndex, numOfElements, searchPhrase);
        }

        [Route("getuserscount")]
        [HttpPost]
        public async Task<int> GetUsersCount(JObject jsonData)
        {
            dynamic postData = jsonData;
            string searchPhrase = postData.searchPhrase;
            return await DiUsers.GetUsersCount(searchPhrase);
        }

        [Route("updateusersettings")]
        [HttpPost]
        public async Task<bool> UpdateUserSettings(JObject jsonData)
        {
            dynamic postData = jsonData;
            string roles, username, email;
            roles = postData.roles;
            username = postData.username;
            email = postData.email;

            string userId = (await DiUsers.GetUserByUsername(username)).Id;
            await UserRoles.DeleteUserRoles(username);
            string[] userRoles = roles.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            await UserManager.AddToRolesAsync(userId, userRoles);
            if (!string.IsNullOrEmpty(email))
            {
                await UserService.SetEmail(username, email);
                await UserService.SetEmailConfirmed(username, true);
            }
            return true;
        }

        [Route("checkusernameexists")]
        [HttpPost]
        public async Task<bool> CheckUsernameExists(JObject jsonData)
        {
            bool res = false;
                dynamic postData = jsonData;
                string username = postData.username;
                res = !(await UserService.CheckUserExists(username));
            return res;
        }

        [Route("registeruser")]
        [HttpPost]
        public async Task<object> RegisterUser(JObject jsonData)
        {
            dynamic postData = jsonData;
            string username, password, roles,email;
            username = postData.username;
            password = postData.password;
            roles = postData.roles;
            email = postData.email;

            var user = new dataislandcommon.Classes.Identity.DiUser { UserName = username, Email = email };
            var result = await UserManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                List<string> listRoles = roles.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (string rl in listRoles)
                {
                    await UserManager.AddToRoleAsync(user.Id, rl);
                }
                return new { result = DiConsts.OK };
            }
            return new { result = DiConsts.Error };
        }

        [Route("deleteusers")]
        [HttpPost]
        public async Task<object> DeleteUsers(JObject jsonData)
        {
            bool delResult = false;

                delResult = true;
                dynamic postData = jsonData;
                string usernames = postData.usernames;
                List<string> usrArrays = usernames.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                string dataIslandID = await DataIsland.GetDataIslandID();
                
                foreach (var user in usrArrays)
                {
                    if (user != this.User.Identity.Name)
                    {
                        bool result = await UserService.DeleteUser(user, dataIslandID);
                        if (!result)
                        {
                            delResult = false;
                        }
                        else
                        {
                            await DiUsers.DeleteUser(user);
                            await UserRoles.DeleteUserRoles(user);
                        }
                    }
                    else
                    {
                        delResult = false;
                    }
                }
            return new { result = delResult };
        }

        [Route("getmaindisettings")]
        [HttpGet]
        public DiSettingsFormData GetSettingsForm()
        {
                string configPath = FilePathProvider.GetConfigPath("forms/settingsforms");
                Dictionary<string, object> settings = DiSettings.GetAllSettingsAsDictionary();
                DiSettingsForm form = UIUtilities.GetSettingsForm(configPath + "dataislandmainsettings.txt", settings);
                bool canProceed = false;
                foreach (var formScope in form.Scopes)
                {
                    if (this.User.IsInRole(formScope) || this.User.IsInRole(DiConsts.RoleAll))
                    {
                        canProceed = true;
                    }
                }
                if (canProceed)
                {
                    return form.Form;
                }
            return null;
        }

        [Route("savemaindisettings")]
        [HttpPost]
        public async Task<object> SaveMainDiSettings(DiSettingsFormData form)
        {
            bool res = false;
            if(UIUtilities.ValidateSettingsForm(form))
            {
                foreach(var entry in form.Entries)
                {
                    await DiSettings.SetSetting(entry.Name, entry.Value);
                }
                res = true;
            }

            return new { result = res };
        }

        
    }
}