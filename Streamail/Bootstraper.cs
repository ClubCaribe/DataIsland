using dataislandcommon.Utilities;
using Autofac;
using Streamail.Services.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Streamail.Interfaces;
using Streamail.Services;

namespace Streamail
{
    public class Bootstraper
    {
        public static void Initialise()
        {

        }

        public static void UpdateDatabase(string user)
        {
            try
            {
                using (var scope = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
                {
                    IStreamailDatabaseManagerSingleton databaseManager = scope.Resolve<IStreamailDatabaseManagerSingleton>();
                    databaseManager.UpdateDatabase(user);
                }
            }
            catch
            {

            }
        }

        public static void AutofacRegistration(ContainerBuilder builder)
        {
            builder.Register<DefaultStreamailMessageSender>(c => new DefaultStreamailMessageSender()).Named<IStreamailMessageSender>("DefaultStreamailMessageSender").PropertiesAutowired();
        }
    }
}
