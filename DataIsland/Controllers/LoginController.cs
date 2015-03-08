using DataIsland.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using dataislandcommon.Services.System;
using System.Threading.Tasks;
using DataIsland.Classes;
using System.Web.Security;
using System.Net;
using Newtonsoft.Json;
using dataislandcommon.Attributes;
using dimain.Services.System;

using dataislandcommon.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using dataislandcommon.Classes.Identity;
using System.Net.Http;
using dataislandcommon.Models.DataCache;
using System.Collections.Specialized;
using dimain.Services.System.Cache;
using dataislandcommon.Services.db;

namespace DataIsland.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserService _userService;

        
        public IUserDatabaseManagerSingleton UserDatabaseManager { get; set; }

        
        public IUserRoleService UserRoles { get; set; }

        
        public IDiUserService DiUsers { get; set; }

        
        public IDataIslandSettingsService DiSettings { get; set; }

        
        public IDataCacheService DataCacheService { get; set; }

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

        public LoginController(IUserService userService)
        {
            _userService = userService;
        }

        //
        // GET: /Login/
        public ActionResult Index()
        {
            ViewBag.Title = "[tr]Sign in[/tr]";
            return View(new UserLoginModel());
        }

        [HttpPost]
        public async Task<ActionResult> Index(UserLoginModel model)
        {
            ViewBag.Title = "[tr]Sign in[/tr]";
            if (ModelState.IsValid)
            {
                
                var result = await SignInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);
                switch (result)
                {
                    case SignInStatus.Success:
                        {
                            try
                            {
                                HttpClient cl = new HttpClient();

                                List<KeyValuePair<string,string>> formargs = new List<KeyValuePair<string,string>>();
                                formargs.Add(new KeyValuePair<string,string>("grant_type","password"));
                                formargs.Add(new KeyValuePair<string,string>("username",model.Username));
                                formargs.Add(new KeyValuePair<string,string>("password",model.Password));
                                formargs.Add(new KeyValuePair<string, string>("client_id", "diHttpApp"));
                                HttpContent cnt = new FormUrlEncodedContent(formargs);
                                HttpResponseMessage resp = await cl.PostAsync("http://localhost/token", cnt);
                                if (resp.IsSuccessStatusCode)
                                {
                                    DataCache cache = await DataCacheService.GetDataCache(model.Username);
                                    if (cache != null)
                                    {
                                        string data = await resp.Content.ReadAsStringAsync();
                                        TokenResponseModel tokenResponse = JsonConvert.DeserializeObject<TokenResponseModel>(data);
                                        cache.AccessToken = tokenResponse.access_token;
                                        cache.AccessTokenExpirationUtc = DateTime.Now.AddSeconds(int.Parse(tokenResponse.expires_in));
                                        cache.RefreshToken = tokenResponse.refresh_token;
                                        await DataCacheService.SaveDataCache(cache);
                                    }
                                }
                                
                                  
                            }
                            catch
                            {
                            }
                            if (Request.UrlReferrer != null)
                            {
                                NameValueCollection query = Request.UrlReferrer.ParseQueryString();
                                if (!string.IsNullOrEmpty(query["ReturnUrl"]))
                                {
                                    string redirectUrl = query["ReturnUrl"];
                                    if (!string.IsNullOrEmpty(redirectUrl))
                                    {
                                        return RedirectPermanent(redirectUrl);
                                    }
                                }
                            }
                            return RedirectPermanent("/panel");
                        }
                    case SignInStatus.LockedOut:
                        return View(model);
                    case SignInStatus.RequiresVerification:
                        return View(model);
                    case SignInStatus.Failure:
                    default:
                        return View(model);
                }
                
            }
            return View(model);
        }

        public string logout()
        {
            var autheticationManager = HttpContext.GetOwinContext().Authentication;
            autheticationManager.SignOut();
            return "ok";
        }
	}
}