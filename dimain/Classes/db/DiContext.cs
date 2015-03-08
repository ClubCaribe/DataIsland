using dimain.Models.maindb;
using dimain.Models.oauth;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace dimain.Classes.db
{
	public class DiContext : DbContext
	{

        public DiContext()
        {

        }
		public DbSet<MainDiSetting> MainDiSetting { get; set; }
		public DbSet<OAuthClient> OAuthClient { get; set; }
		public DbSet<OAuthClientAuthorization> OAuthClientAuthorization { get; set; }
		public DbSet<OAuthNonce> OAuthNonce { get; set; }
		public DbSet<OAuthSymmetricCryptoKey> OAuthSymmetricCryptoKey { get; set; }
        public DbSet<UserRole> UserRole { get; set; }
        public DbSet<DiUser> DiUser { get; set; }
        public DbSet<DiUserExternalLogin> ExternalLogins { get; set; }
        public DbSet<Client> Client { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }
        public DbSet<DataCache> DataCache { get; set; }
        public DbSet<DiUserPassport> DiUserPassport { get; set; }
        public DbSet<DiUserData> DiUserData { get; set; }
        public DbSet<DataIslandData> DataislandData { get; set; }


		public DiContext(DbConnection connection)
			: base(connection,false)
		{
            Debug.Write(Database.Connection.ConnectionString);
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
