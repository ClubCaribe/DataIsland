using FileManager.Classes.db;
using System;
namespace FileManager.Services.db
{
    public interface IFileDatabaseManagerSingleton
    {
        DiFileContext GetDbContext(string username);
        void UpdateDatabase(string username);
    }
}
