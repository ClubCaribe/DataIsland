using dataislandcommon.Classes.db;
using SQLite;
using System;
using System.Threading.Tasks;

namespace dataislandcommon.Services.db
{
    public interface IUserDatabaseManagerSingleton
    {
        DiUserContext GetUserContext(string username);
        void UpdateDatabase(string username);
    }
}
