using dimain.Services.db;
using dataislandcommon.Services;
using dimain.Classes.db;
using dimain.Migrations;
using dimain.Services.System;
using Autofac;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using dataislandcommon.Utilities;

namespace dimain
{
    public class Bootstrapper
    {
        public static void Initialise()
        {
            using (var scope = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                IDataIslandService diService = scope.Resolve<IDataIslandService>();
                IDataIslandSettingsService settings = scope.Resolve<IDataIslandSettingsService>();
                IDataProviderSingleton dataProvider = scope.Resolve<IDataProviderSingleton>();
                IMainDatabaseManagerSingleton dbManager = scope.Resolve<IMainDatabaseManagerSingleton>();
                IDiUserService diUsers = scope.Resolve<IDiUserService>();
                IDiRefreshTokenService diAppClient = scope.Resolve<IDiRefreshTokenService>();
                IUserPassportTokensSingleton tokens = scope.Resolve<IUserPassportTokensSingleton>();

                //Database.SetInitializer(new MigrateDatabaseToLatestVersion<DiContext, Configuration>());
                dbManager.UpdateDatabase();

                diService.StartDataIsland().Wait();
                diAppClient.AddClient("diHttpApp", "diHttpApp", "website calls", Models.oauth.ApplicationTypes.JavaScript, true, 14400, "*").Wait();
                diUsers.UpdateUsersIds().Wait();
                tokens.InitTokens().Wait();
            }
        }
    }
}
