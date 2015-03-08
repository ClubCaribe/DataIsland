using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Reflection;
using System.Web.Compilation;
using DataIsland.App_Start;

using dataislandcommon.Utilities;

namespace DataIsland
{
    public class PluginAreaBootstrapper
    {
        public static readonly List<Assembly> PluginAssemblies = new List<Assembly>();

        public static List<string> PluginNames()
        {
            return PluginAssemblies.Select(
                pluginAssembly => pluginAssembly.GetName().Name)
                .ToList();
        }

        public static void Init()
        {
            var fullPluginPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Areas");

            foreach (var file in Directory.EnumerateFiles(fullPluginPath, "*DiPlugin*.dll", SearchOption.AllDirectories))
                PluginAssemblies.Add(Assembly.LoadFile(file));

            PluginAssemblies.ForEach(BuildManager.AddReferencedAssembly);

            // Add assembly handler for strongly-typed view models
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
        }

        private static Assembly AssemblyResolve(object sender, ResolveEventArgs resolveArgs)
        {
            var currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            // Check we don't already have the assembly loaded
            foreach (var assembly in currentAssemblies)
            {
                if (assembly.FullName == resolveArgs.Name || assembly.GetName().Name == resolveArgs.Name)
                {
                    return assembly;
                }
            }

            var _fullPluginPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Areas");
            // Load from directory
            return LoadAssemblyFromPath(resolveArgs.Name, _fullPluginPath);
        }



        private static Assembly LoadAssemblyFromPath(string assemblyName, string directoryPath)
        {
            foreach (string file in Directory.GetFiles(directoryPath))
            {
                Assembly assembly;

                if (TryLoadAssemblyFromFile(file, assemblyName, out assembly))
                {
                    return assembly;
                }
            }

            return null;
        }

        private static bool TryLoadAssemblyFromFile(string file, string assemblyName, out Assembly assembly)
        {
            try
            {
                // Convert the filename into an absolute file name for
                // use with LoadFile.
                file = new FileInfo(file).FullName;

                if (AssemblyName.GetAssemblyName(file).Name == assemblyName)
                {
                    assembly = Assembly.LoadFile(file);
                    return true;
                }
            }
            catch
            {
            }

            assembly = null;
            return false;
        }

        public static void RunInitForLoadedPlugins()
        {
            List<Assembly> loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.Contains("DiPlugin")).ToList();
            if (loadedAssemblies != null && loadedAssemblies.Count > 0)
            {
                

                foreach (Assembly asm in loadedAssemblies)
                {
                    foreach (Type tp in asm.GetTypes())
                    {
                        if (tp.Name.ToLower() == "bootstraper")
                        {
                            MethodInfo methodInfo = tp.GetMethod("Initialise");
                            if (methodInfo != null)
                            {
                                methodInfo.Invoke(null, null);
                            }
                        }
                    }
                }
            }
        }

        public static void RunUpdateDatabaseForLoadedPlugins(string username)
        {
            List<Assembly> loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.Contains("DiPlugin")).ToList();
            if (loadedAssemblies != null && loadedAssemblies.Count > 0)
            {
                foreach (Assembly asm in loadedAssemblies)
                {
                    foreach (Type tp in asm.GetTypes())
                    {
                        if (tp.Name.ToLower() == "bootstraper")
                        {
                            MethodInfo methodInfo = tp.GetMethod("UpdateDatabase");
                            if (methodInfo != null)
                            {
                                methodInfo.Invoke(null, new object[] { username });
                            }
                        }
                    }
                }
            }
        }
    }
}