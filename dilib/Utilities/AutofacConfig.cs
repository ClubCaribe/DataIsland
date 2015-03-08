

using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dataislandcommon.Utilities
{
    public class AutofacConfig
    {
        #region Autofac Container
        private static Lazy<AutofacContainerHolder> container = new Lazy<AutofacContainerHolder>(() =>
        {
            var container = new AutofacContainerHolder();
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IContainer GetConfiguredContainer()
        {
            return container.Value.Container;
        }

        public static void SetContainer(IContainer cnt)
        {
            container.Value.Container = cnt;
        }
        #endregion
    }

    public class AutofacContainerHolder
    {
        public IContainer Container { get; set; }
    }
}
