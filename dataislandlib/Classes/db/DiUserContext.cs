
using dataislandcommon.Models.userdb;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace dataislandcommon.Classes.db
{
    public class DiUserContext : DbContext
    {
        public DiUserContext()
        {

        }
        public DbSet<UserAccount> UserAccount { get; set; }
        public DbSet<UserSetting> UserSettings { get; set; }
        public DbSet<UserExternalLogin> ExternalLogins { get; set; }
        public DbSet<UserClaim> UserClaims { get; set; }
        public DbSet<UserContact> UserContacts { get; set; }
        public DbSet<ContactCategory> ContactCategories { get; set; }
        public DbSet<UserContactCategory> UserContactCategories { get; set; }


        public DiUserContext(DbConnection connection)
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
