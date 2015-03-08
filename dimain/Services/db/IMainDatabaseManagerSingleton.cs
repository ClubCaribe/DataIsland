
using dimain.Classes.db;
using SQLite;
using System;
using System.Threading.Tasks;

namespace dimain.Services.db
{
    public interface IMainDatabaseManagerSingleton
    {
		DiContext GetMainDatabaseConnection();
		DiContext GetMainDatabaseConnectionNonAsync();
        void UpdateDatabase();
    }
}
