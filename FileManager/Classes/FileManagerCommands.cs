using Autofac;
using dataislandcommon.Classes.Attributes;
using dataislandcommon.Interfaces.Communication;
using dataislandcommon.Utilities;
using FileManager.Services.FileManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Classes
{
    [DataIslandCommands]
    public class FileManagerCommands : IDataIslandCommandHub
    {
        public FileManagerCommands()
        {

        }

        public string Username { get; set; }
        public string UserId { get; set; }
        public string SenderId
        {
            get;
            set;
        }

        public int CommandVersion
        {
            get;
            set;
        }

        public async Task AddForeignResource(string id, string ownerId, string name, bool isDirectory)
        {
            using (var scope = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                ISharedResourcesService resources = scope.Resolve<ISharedResourcesService>();
                await resources.AddForeignSharedResource(id, ownerId, name, isDirectory, this.Username);
            }
        }

        public async Task DeleteForeignResource(string resId)
        {
            using (var scope = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                ISharedResourcesService resources = scope.Resolve<ISharedResourcesService>();
                await resources.DeleteForeignSharedResources(resId,false, this.Username);
            }
        }

        public async Task DeleteForeignResourceByUser(string resId)
        {
            using (var scope = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                ISharedResourcesService resources = scope.Resolve<ISharedResourcesService>();
                await resources.RemoveRecipientFromSharedResource(resId, this.SenderId, this.Username);
            }
        }
    }
}
