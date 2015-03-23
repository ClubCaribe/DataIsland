
using Autofac;
using dataislandcommon.Utilities;
using FileManager.Services.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager
{
    public class Bootstraper
    {
        public static void Initialise()
        {

           // Database.SetInitializer(new MigrateDatabaseToLatestVersion<DiUserContext, Configuration>());

        }

        public static void UpdateDatabase(string user)
        {
            try
            {
                using (var scope = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
                {
                    IFileDatabaseManagerSingleton databaseManager = scope.Resolve<IFileDatabaseManagerSingleton>();
                    databaseManager.UpdateDatabase(user);
                }
            }
            catch
            {

            }
        }
    }
}
