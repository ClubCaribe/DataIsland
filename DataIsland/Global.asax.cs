using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.IO;
using DataIsland.App_Start;
using System.Web.Http;
using System.Web.SessionState;
using DataIsland.Classes;
using Autofac;
using dimain.Services.System;
using System.Configuration;
using dataislandcommon.Utilities;
using Microsoft.AspNet.SignalR;
using Autofac.Integration.Mvc;
using System.Web.Optimization;

namespace DataIsland
{
    public class MvcApplication : System.Web.HttpApplication
    {

        protected void Application_Start()
        {

            ViewEngines.Engines.Add(new PluginRazorViewEngine());

            //GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            EnsureSqliteDllsLoaded();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(AutofacRegistrationConfig.GetContainer()));

            using (var container = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                IDataIslandService diService = container.Resolve<IDataIslandService>();
                string storagePath = HttpContext.Current.Server.MapPath("~/App_Data/DataIsland/");

                if (ConfigurationManager.AppSettings.AllKeys.Contains("RootDataIslandStoragePath"))
                {
                    storagePath = ConfigurationManager.AppSettings["RootDataIslandStoragePath"];
                }

                diService.InitDataIslandStateObject();
                diService.SetRootDocumentPath(storagePath);

                dimain.Bootstrapper.Initialise();
                dataislandcommon.Bootstrapper.Initialise();
                PluginAreaBootstrapper.RunInitForLoadedPlugins();
            }

            BundleConfig.RegisterBundles(BundleTable.Bundles);

        }

        

        // http://stackoverflow.com/questions/281145/asp-net-hostingenvironment-shadowcopybinassemblies
        public void EnsureSqliteDllsLoaded()
        {
            int iBitNess = IntPtr.Size * 8;

            string strTargetDirectory = System.Reflection.Assembly.GetAssembly(typeof(dataislandcommon.Bootstrapper)).Location;
            strTargetDirectory = System.IO.Path.GetDirectoryName(strTargetDirectory);

            try
            {
                File.Copy(Server.MapPath("~/Content/sqlite/sqlite3.dll"), strTargetDirectory + "/sqlite3.dll");
            }
            catch
            {
            }

            
        }
    }
}
