
using dataislandcommon.Classes.db;
using dataislandcommon.Migrations;
using dataislandcommon.Services.db;
using dataislandcommon.Utilities;
using Autofac;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using dataislandcommon.Services.Contacts;
using Streamail.Interfaces;

namespace dataislandcommon
{
    public class Bootstrapper
    {
        public static void Initialise()
        {

			//settings.GetDbConnectionSettings();
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<DiUserContext, Configuration>()); 
            
        }

        public static void UpdateDatabase(string user)
        {
            try
            {
                using (var scope = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
                {
                    IUserDatabaseManagerSingleton databaseManager = scope.Resolve<IUserDatabaseManagerSingleton>();
                    databaseManager.UpdateDatabase(user);
                }
            }
            catch
            {

            }
        }

        public static void AutofacRegistration(ContainerBuilder builder)
        {
        }
    }
}
