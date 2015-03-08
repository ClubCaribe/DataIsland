using Streamail.Classes.db;
using System;
namespace Streamail.Services.db
{
    public interface IStreamailDatabaseManagerSingleton
    {
        DiStreamailContext GetDbContext(string username);
        void UpdateDatabase(string username);
    }
}
