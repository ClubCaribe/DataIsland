using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Autofac.Integration.SignalR;
using Autofac.Integration.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using dataislandcommon.Utilities;

namespace DataIsland.App_Start
{
    public class AutofacRegistrationConfig
    {
        public static void Register()
        {
            var builder = new ContainerBuilder();

            var assemblies = BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToArray();
           
            builder.RegisterAssemblyTypes(assemblies).Where(t => t.Name.ToLower().EndsWith("singleton")).AsImplementedInterfaces().SingleInstance().PropertiesAutowired();
            builder.RegisterAssemblyTypes(assemblies).Where(t => t.Name.ToLower().EndsWith("service")).AsImplementedInterfaces().PropertiesAutowired();

            foreach(Assembly asm in assemblies)
            {
                foreach(Type tp in asm.GetTypes())
                {
                    MethodInfo method = tp.GetMethod("AutofacRegistration");
                    if(method!=null && method.IsStatic)
                    {
                        method.Invoke(null, new object[] { builder });
                    }
                }
            }

            // Register your MVC controllers.
            builder.RegisterControllers(typeof(MvcApplication).Assembly).PropertiesAutowired();

            // OPTIONAL: Register model binders that require DI.
            builder.RegisterModelBinders(assemblies).PropertiesAutowired();
            builder.RegisterModelBinderProvider();

            // OPTIONAL: Register web abstractions like HttpContextBase.
            builder.RegisterModule<AutofacWebTypesModule>();

            // OPTIONAL: Enable property injection in view pages.
            builder.RegisterSource(new ViewRegistrationSource());

            // OPTIONAL: Enable property injection into action filters.
            builder.RegisterFilterProvider();

            builder.RegisterApiControllers(assemblies).PropertiesAutowired();

            //builder.RegisterHubs(assemblies).PropertiesAutowired();

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();

            AutofacConfig.SetContainer(container);
        }

        public static IContainer GetContainer()
        {
            IContainer cnt = AutofacConfig.GetConfiguredContainer();
            if(cnt == null)
            {
                AutofacRegistrationConfig.Register();
            }
            cnt = AutofacConfig.GetConfiguredContainer();
            return cnt;
        }
    }
}