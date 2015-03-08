using dataislandcommon.Services.db;
using Streamail.Classes.db;
using Streamail.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streamail.Services.db
{
    public class StreamailDatabaseManagerSingleton : Streamail.Services.db.IStreamailDatabaseManagerSingleton
    {
        private readonly IDatabaseManagerSingleton DbManager;

        public StreamailDatabaseManagerSingleton(IDatabaseManagerSingleton dbManager)
        {
            DbManager = dbManager;
        }

        public DiStreamailContext GetDbContext(string username)
        {
            if(!this.DbManager.CheckDatabaseFileExists(username,"streamail"))
            {
                this.UpdateDatabase(username);
            }
            var db = new DiStreamailContext(DbManager.GetConnection(username, "streamail"));
            
            return db;
        }

        public void UpdateDatabase(string username)
        {
            try
            {
                var configuration = new Configuration();
                DbManager.UpdateDatabase(configuration, username, "streamail");
            }
            catch
            {

            }
        }
    }
}
