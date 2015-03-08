using dataislandcommon.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using dataislandcommon.Services.FileSystem;

using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations.Sql;
using System.IO;

namespace dataislandcommon.Services.db
{
    public class DatabaseManagerSingleton : dataislandcommon.Services.db.IDatabaseManagerSingleton
    {
        
        public IFilePathProviderService PathProvider { get; set; }

        public bool CheckDatabaseFileExists(string username, string applicationName)
        {
            string databasePath = "";
            databasePath = PathProvider.GetUserDatabasesPath(username) + applicationName + ".db";
            return File.Exists(databasePath);
        }

        public DbConnection GetConnection(string username, string applicationName)
        {
            string databasePath = "";
            databasePath = PathProvider.GetUserDatabasesPath(username) + applicationName + ".db";
            string connectionString = "Data Source=[filename]".Replace(DiConsts.DbSettingsFileNamePlaceholder, databasePath);
            connectionString = connectionString.Replace(DiConsts.DbSettingsDatabaseNamePlaceholder, "dataisland");

            var connection = DbProviderFactories.GetFactory("System.Data.SQLite").CreateConnection();
            connection.ConnectionString = connectionString;
            return connection;
        }

        public bool UpdateDatabase(DbMigrationsConfiguration configuration, string username, string applicationName)
        {
            try
            {
                string databasePath = "";
                databasePath = PathProvider.GetUserDatabasesPath(username) + applicationName + ".db";
                string connectionString = "DataSource=[filename];FailIfMissing=false;".Replace(DiConsts.DbSettingsFileNamePlaceholder, databasePath);
                connectionString = connectionString.Replace(DiConsts.DbSettingsDatabaseNamePlaceholder, "dataisland");

                configuration.TargetDatabase = new DbConnectionInfo(connectionString, "System.Data.SQLite");
                configuration.SetSqlGenerator("System.Data.SQLite", new SqliteSqlMigrationSqlGenerator());

                var migrator = new DbMigrator(configuration);
                migrator.Update();

                return true;
            }
            catch
            {
            }
            return false;
        }
    }
}
