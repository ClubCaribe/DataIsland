using Streamail.Classes.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Newtonsoft.Json;
using Streamail.Models.db;

namespace Streamail.Services.db
{
    public class DbMessageCustomFiledsService : Streamail.Services.db.IDbMessageCustomFiledsService
    {
        private readonly IStreamailDatabaseManagerSingleton dbManager;

        public DbMessageCustomFiledsService(IStreamailDatabaseManagerSingleton manager)
        {
            this.dbManager = manager;
        }

        private async Task<bool> CheckCustomFiledExists(string name, string messageId, DiStreamailContext db )
        {
            var res = await db.MessageCustomFields.Where(x => x.MessageID == messageId && x.Name.ToLower() == name.ToLower()).ToListAsync();
            if(res!=null && res.Count>0)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> AddMessageCustomFields(Dictionary<string,object> customFields, string messageId, string ownerUsername)
        {
            using(var db = this.dbManager.GetDbContext(ownerUsername))
            {
                foreach(var entry in customFields)
                {
                    if(!await this.CheckCustomFiledExists(entry.Key,messageId,db))
                    {
                        MessageCustomField cf = new MessageCustomField();
                        cf.ID = Guid.NewGuid().ToString();
                        cf.MessageID = messageId;
                        cf.Name = entry.Key;
                        cf.Value = JsonConvert.SerializeObject(entry.Value);
                        cf.ValueType = entry.Value.GetType().ToString();
                        db.MessageCustomFields.Add(cf);
                    }
                }
                await db.SaveChangesAsync();
                return true;
            }
        }

        public async Task<Dictionary<string,object>> GetCustomFields(string messageId, DiStreamailContext db)
        {
            var res = await db.MessageCustomFields.Where(x => x.MessageID == messageId).ToListAsync();
            Dictionary<string, object> customFields = new Dictionary<string, object>();

            foreach(var cf in res)
            {
                customFields[cf.Name] = JsonConvert.DeserializeObject(cf.Value, Type.GetType(cf.ValueType));
            }

            return customFields;
        }

        public async Task DeleteCustomFileds(string messageId,DiStreamailContext db)
        {
            await db.Database.ExecuteSqlCommandAsync("DELETE FROM MessageCustomFields WHERE MessageID=\"" + messageId + "\"");
        }

    }
}
