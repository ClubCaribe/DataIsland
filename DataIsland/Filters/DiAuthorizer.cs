using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using dataislandcommon.Services.System;
using DataIsland.App_Start;
using Autofac;
using dimain.Services.System;
using dataislandcommon.Attributes;
using dataislandcommon.Utilities;

namespace DataIsland.Filters
{
    public class DiAuthorizer : AuthorizeAttribute
    {
        
        public DiAuthorizer()
        {
            
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            using (var uContainer = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                try
                {
                    List<string> allowedurls = new List<string>() { "home/doesusernameexist", "home/registerdataisland" };

                    bool continueWithoutAuthorization = false;

                    foreach (var allowedurl in allowedurls)
                    {
                        if (httpContext.Request.Url.PathAndQuery.ToLower().Contains(allowedurl.ToLower()))
                        {
                            continueWithoutAuthorization = true;
                        }
                    }

                    if (continueWithoutAuthorization)
                    {
                        return true;
                    }
                    if (!uContainer.Resolve<IDataIslandService>().IsDataIslandInitialised())
                    {
                        httpContext.Response.Redirect("/home/registerdataisland");
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                catch
                {
                }
                return false;
            }
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            using (var uContainer = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                IDataIslandService diService = uContainer.Resolve<IDataIslandService>();
                if (diService.IsDataIslandInitialised())
                {
                    return;
                }
            }
            base.OnAuthorization(filterContext);

        }
    }
}