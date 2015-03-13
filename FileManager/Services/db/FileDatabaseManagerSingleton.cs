using dataislandcommon.Services.db;
using FileManager.Classes.db;
using FileManager.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Services.db
{
    public class FileDatabaseManagerSingleton : IFileDatabaseManagerSingleton
    {

        private readonly IDatabaseManagerSingleton DbManager;

        public FileDatabaseManagerSingleton(IDatabaseManagerSingleton dbManager)
        {
            DbManager = dbManager;
        }

        public DiFileContext GetDbContext(string username)
        {
            if(!this.DbManager.CheckDatabaseFileExists(username,"filemanager"))
            {
                this.UpdateDatabase(username);
            }
            var db = new DiFileContext(DbManager.GetConnection(username, "filemanager"));
            
            return db;
        }

        public void UpdateDatabase(string username)
        {
            try
            {
                var configuration = new Configuration();
                DbManager.UpdateDatabase(configuration, username, "filemanager");
            }
            catch
            {

            }
        }
    }
}
