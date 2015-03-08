using System;
namespace dataislandcommon.Services.db
{
    public interface IDatabaseManagerSingleton
    {
        bool CheckDatabaseFileExists(string username, string applicationName);
        System.Data.Common.DbConnection GetConnection(string username, string applicationName);
        bool UpdateDatabase(System.Data.Entity.Migrations.DbMigrationsConfiguration configuration, string username, string applicationName);
    }
}
