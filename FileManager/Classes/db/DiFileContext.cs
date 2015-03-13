using FileManager.Models.db;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Classes.db
{
    public class DiFileContext : DbContext
    {
        public DbSet<SharedResource> SharedResources { get; set; }
        public DbSet<ResourceRecipient> ResourceRecipients { get; set; }
        public DbSet<ForeignSharedResource> ForeignResources { get; set; }

        public DiFileContext()
        {

        }

        public DiFileContext(DbConnection connection)
            : base(connection, true)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //does not pluralize table names
            modelBuilder.Conventions
                .Remove<PluralizingTableNameConvention>();

        }

        public static void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            //does not pluralize table names
            modelBuilder.Conventions
                .Remove<PluralizingTableNameConvention>();
        }
    }
}
