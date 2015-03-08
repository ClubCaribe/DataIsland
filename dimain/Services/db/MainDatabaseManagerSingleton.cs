
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using dataislandcommon.Services.FileSystem;
using dataislandcommon.Services;
using dimain.Classes.db;
using dataislandcommon.Utilities;
using dimain.Migrations;
using System.Data.Entity.Migrations.Sql;

namespace dimain.Services.db
{
    public class MainDatabaseManagerSingleton : IMainDatabaseManagerSingleton
    {
        private readonly IFilePathProviderService _pathProvider;
		private readonly IDataProviderSingleton _dataProvider;

        public MainDatabaseManagerSingleton(IFilePathProviderService pathProvider, IDataProviderSingleton dataProvider)
        {
            _pathProvider = pathProvider;
			_dataProvider = dataProvider;
        }

        public DiContext GetMainDatabaseConnection()
        {
            string databasePath = _pathProvider.GetMainDatabasePath();
            
            string connectionString = "Data Source=[filename]".Replace(DiConsts.DbSettingsFileNamePlaceholder, databasePath);
            connectionString = connectionString.Replace(DiConsts.DbSettingsDatabaseNamePlaceholder, "dataisland");

            var connection = DbProviderFactories.GetFactory("System.Data.SQLite").CreateConnection();
            connection.ConnectionString = connectionString;
			var db = new DiContext(connection);
            return db;
        }

		public DiContext GetMainDatabaseConnectionNonAsync()
        {
			return GetMainDatabaseConnection();
        }

        public void UpdateDatabase()
        {
            try
            {
                string databasePath = _pathProvider.GetMainDatabasePath();
                string connectionString = "DataSource=[filename];FailIfMissing=false;".Replace(DiConsts.DbSettingsFileNamePlaceholder, databasePath);
                connectionString = connectionString.Replace(DiConsts.DbSettingsDatabaseNamePlaceholder, "dataisland");

                var configuration = new Configuration();

                configuration.TargetDatabase = new DbConnectionInfo(connectionString, "System.Data.SQLite");
                configuration.SetSqlGenerator("System.Data.SQLite", new SqliteSqlMigrationSqlGenerator());

                var migrator = new DbMigrator(configuration);
                migrator.Update();
            }
            catch
            {

            }
        }
    }
}
