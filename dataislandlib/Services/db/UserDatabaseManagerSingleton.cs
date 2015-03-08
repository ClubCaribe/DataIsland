using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dataislandcommon.Classes.db;
using System.Data.Common;
using dataislandcommon.Migrations;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using dataislandcommon.Services;
using dataislandcommon.Services.FileSystem;
using dataislandcommon.Utilities;
using dataislandcommon.Services.db;

namespace dataislandcommon.Services.db
{
    public class UserDatabaseManagerSingleton : dataislandcommon.Services.db.IUserDatabaseManagerSingleton
    {
        private readonly IDatabaseManagerSingleton DbManager;

        public UserDatabaseManagerSingleton(IDatabaseManagerSingleton dbManager)
        {
            DbManager = dbManager;
        }

        

        public DiUserContext GetUserContext(string username)
        {
            
            var db = new DiUserContext(DbManager.GetConnection(username,"user"));
            
            return db;
        }

        public void UpdateDatabase(string username)
        {
            try
            {
                var configuration = new Configuration();
                DbManager.UpdateDatabase(configuration, username, "user");
            }
            catch
            {

            }
        }

        
    }
}
