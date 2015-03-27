using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using dataislandcommon.Services.System;
using DataIsland.Models;
using System.Threading.Tasks;
using System.Text;
using System.IO;

using System.Web.Security;
using dimain.Services.System;
using dataislandcommon.Services.Utilities;
using dataislandcommon.Utilities.enums;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using dataislandcommon.Classes.Identity;
using System.Net.Http;
using dataislandcommon.Models.DataCache;
using Newtonsoft.Json;
using dimain.Services.System.Cache;
using dataislandcommon.Utilities;

namespace DataIsland.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDataIslandService _diService;
        private readonly IUserService _userService;

		
		public IUtilitiesSingleton Utilities { get; set; }

        
        public IUserRoleService UserRoles { get; set; }

        
        public IDataCacheService DataCacheService { get; set; }

        
        public ISystemSecurityService SystemCommunication { get; set; }

        public IDiUserService DiUsers { get; set; }
        public IDataIslandSettingsService DiSettings { get; set; }

        public ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }

        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
        }

        public HomeController(IDataIslandService diService, IUserService userService)
        {
            _diService = diService;
            _userService = userService;

        }

        //
        // GET: /Home/
        public ActionResult Index()
        {
            //var user = new DiUser { UserName = "superuser", Email = "pr@sealcast.com" };

            //var result = await UserManager.CreateAsync(user, "jqu3daiq!");
            //await UserManager.AddToRoleAsync(user.Id, "all");

            return RedirectToAction("Index", "home", new { area = "panel" });
        }

        public ActionResult registerdataisland()
        {
            RegisterDataislandModel model = new RegisterDataislandModel();
            ViewBag.ShowError = false;
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> registerdataisland(RegisterDataislandModel model)
        {
            bool issuccess = true;
            if (ModelState.IsValid)
            {
                if (!_diService.IsDataIslandInitialised())
                {
                    issuccess = await _diService.RegisterDataIsland(model.DataIslandName, model.Description, model.Domain, model.WebPage, model.IsPublic, model.Email);
                    if (issuccess)
                    {

                        var user = new DiUser { UserName = model.Username, Email = model.Email };

                        var result = await UserManager.CreateAsync(user, model.AdministratorPassword);
                        await UserManager.AddToRoleAsync(user.Id, "all");
                        if (result.Succeeded)
                        {
                            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                            try
                            {
                                HttpClient cl = new HttpClient();

                                List<KeyValuePair<string, string>> formargs = new List<KeyValuePair<string, string>>();
                                formargs.Add(new KeyValuePair<string, string>("grant_type", "password"));
                                formargs.Add(new KeyValuePair<string, string>("username", model.Username));
                                formargs.Add(new KeyValuePair<string, string>("password", model.AdministratorPassword));
                                formargs.Add(new KeyValuePair<string, string>("client_id", "diHttpApp"));
                                HttpContent cnt = new FormUrlEncodedContent(formargs);
                                var oDomain = await this.DiSettings.GetSetting(DiConsts.DataIslandDomain);
                                string dataIslandDomain = "http://localhost";
                                if (oDomain != null)
                                {
                                    dataIslandDomain = (string)oDomain;
                                }
                                HttpResponseMessage resp = await cl.PostAsync(dataIslandDomain + "/token", cnt);
                                if (resp.IsSuccessStatusCode)
                                {
                                    DataCache cache = await DataCacheService.GetDataCache(model.Username);
                                    if (cache != null)
                                    {
                                        string data = await resp.Content.ReadAsStringAsync();
                                        TokenResponseModel tokenResponse = JsonConvert.DeserializeObject<TokenResponseModel>(data);
                                        cache.AccessToken = tokenResponse.access_token;
                                        cache.AccessTokenExpirationUtc = DateTime.UtcNow.AddSeconds(int.Parse(tokenResponse.expires_in));
                                        cache.RefreshToken = tokenResponse.refresh_token;
                                        await DataCacheService.SaveDataCache(cache);
                                    }
                                }


                            }
                            catch
                            {
                            }

                            return RedirectPermanent("/panel/");

                        }
                        else
                        {
                            ViewBag.ShowError = issuccess;
                            ViewBag.InformationHeadline = "Error occured";
                            ViewBag.InformationDescription = "Cannot register user at the moment. Please try again";
                            return View(model);
                        }
                    }

                    else
                    {
                        ViewBag.ShowError = issuccess;
                        ViewBag.InformationHeadline = "Error occured";
                        ViewBag.InformationDescription = "Cannot register DataIsland. Please try again";
                        return View(model);
                    }
                }
            }
            ViewBag.ShowError = issuccess;
            ViewBag.InformationHeadline = "Error occured";
            ViewBag.InformationDescription = "Unknown error occured. Please try again";
            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> doesUserNameExist(string Username)
        {
            bool user = await _userService.CheckUserExists(Username);
            return Json(!user);
        }

		public async Task<FileContentResult> getConstantsVariables()
		{
            DataCache cache = await DataCacheService.GetDataCache(this.User.Identity.Name);

			StringBuilder constants = new StringBuilder();
            constants.AppendLine(String.Format("var diApiToken = '{0}';", ((!string.IsNullOrEmpty(cache.AccessToken)) ? cache.AccessToken : "")));
            string userPassport = await SystemCommunication.IssueUserPassport(this.User.Identity.Name,4);
            constants.AppendLine(String.Format("var diUserPassport = '{0}';", userPassport));
			//constants.AppendLine(String.Format("var diRefreshToken = '{0}';", ((!string.IsNullOrEmpty(_session.SessionObject.RefreshToken)) ? _session.SessionObject.RefreshToken : "")));
            DateTime expirationTime = cache.AccessTokenExpirationUtc ?? DateTime.UtcNow;
            constants.AppendLine(String.Format("var diTokenExpirationTime = new Date('{0}');", expirationTime.ToString("s", System.Globalization.CultureInfo.InvariantCulture)));
            string userId = await this.DiUsers.GetUserIdByFromUsername(this.User.Identity.Name);
            userId = this.Utilities.EscapeUserId(userId);
            constants.AppendLine(String.Format("var diCurrentUserID = '{0}';", userId));
            constants.Append(@"
function refreshToken(){
    var currentTime = new Date().getTime();
    var timespan = diTokenExpirationTime - currentTime;
    timespan = timespan - 2000;
    setTimeout(function(){
        $.ajax({
                url: '/api/panel/user/refreshapitoken',
                type: 'GET',
                dataType: ""json"",
                beforeSend: function (xhr) {
                            xhr.setRequestHeader(""Authorization"", ""Bearer ""+diApiToken)
                    }
                }).done(function(resp) {
                        
                        if(resp.result){
                            diApiToken = resp.newToken;
                            diTokenExpirationTime =new Date(resp.expirationTime);
                            refreshToken();
                        }
                    });
    },timespan);
}


$(document).ready(function(){
    refreshToken();
    setTimeout(function(){
        // Start the connection.
        $.connection.hub.start().done(function () { });
        $.connection.hub.disconnected(function () {
            setTimeout(function () { $.connection.hub.start() }, 1000);
        });
    },1000);
});
");
			byte[] outcome = Encoding.UTF8.GetBytes(constants.ToString());
			return File(outcome, Utilities.GetProperContentType("js"));

		}
	}
}