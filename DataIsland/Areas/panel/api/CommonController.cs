using dataislandcommon.Attributes;
using dataislandcommon.Models.ViewModels.SettingsForm;
using dataislandcommon.Services.FileSystem;

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using dataislandcommon.Services.Utilities;
using dimain.Services.System;
using dataislandcommon.Utilities;

namespace DataIsland.Areas.panel.api
{
    [Authorize]
    [RoutePrefix("api/panel/common")]
    public class CommonController : ApiController
    {

        
        public IFilePathProviderService FilePathProvider { get; set; }

        
        public IUIUtilitiesService UiUtilities { get; set; }

        
        public IDataIslandSettingsService DiSettings { get; set; }

		public CommonController()
		{

		}

        [Route("getsettingsform/{section}/{formname}")]
        [HttpGet]
        public DiSettingsFormData GetSettingsForm(string section, string formname)
        {
                string configPath = FilePathProvider.GetConfigPath("forms/"+section);
                Dictionary<string, object> settings = DiSettings.GetAllSettingsAsDictionary();
                DiSettingsForm form = UiUtilities.GetSettingsForm(configPath + formname + ".txt", settings);
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

        
    }
}