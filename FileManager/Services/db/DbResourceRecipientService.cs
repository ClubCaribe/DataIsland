using FileManager.Classes.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using FileManager.Models.db;

namespace FileManager.Services.db
{
    public class DbResourceRecipientService : IDbResourceRecipientService
    {
        private readonly IFileDatabaseManagerSingleton _dbMngr;

        public DbResourceRecipientService(IFileDatabaseManagerSingleton manager)
        {
            this._dbMngr = manager;
        }

        public async Task<bool> CheckRecipientExists(string resourceId, string recipientId, DiFileContext db)
        {
            var res = await db.ResourceRecipients.Where(x => x.RecipientID == recipientId && x.ResourceID == resourceId).CountAsync();
            return res > 0;
        }

        public async Task<bool> AddRecipient(string resourceId, string recipientId, bool deleted, DiFileContext db)
        {
            if(! await this.CheckRecipientExists(resourceId,recipientId,db))
            {
                ResourceRecipient rec = new ResourceRecipient();
                rec.Deleted = deleted;
                rec.ID = Guid.NewGuid().ToString();
                rec.RecipientID = recipientId;
                rec.ResourceID = resourceId;

                db.ResourceRecipients.Add(rec);

                await db.SaveChangesAsync();

                return true;
            }
            return false;
        }

        public async Task<bool> UpdateRecipient(string resourceId, string recipientId, bool deleted, DiFileContext db)
        {
            var res = await db.ResourceRecipients.Where(x => x.ResourceID == resourceId && x.RecipientID == recipientId).SingleOrDefaultAsync();
            if(res!=null)
            {
                res.Deleted = deleted;
                await db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<ResourceRecipient>> GetRecipients(string resourceId, DiFileContext db)
        {
            var res = await db.ResourceRecipients.Where(x => x.ResourceID == resourceId).ToListAsync();
            return res;
        }

        public async Task<bool> DeleteRecipients(string resourceId, DiFileContext db)
        {
            int count = await db.Database.ExecuteSqlCommandAsync("DELETE FROM ResourceRecipient WHERE ResourceID = \"" + resourceId + "\"");
            if (count > 0)
            {
                await db.SaveChangesAsync();
            }
            return count > 0;
        }

        public async Task<bool> DeleteRecipient(string resourceId, string recipientId, DiFileContext db)
        {
            int count = await db.Database.ExecuteSqlCommandAsync("DELETE FROM ResourceRecipient WHERE ResourceID = \"" + resourceId + "\" AND RecipientID=\"" + recipientId + "\"");
            if (count > 0)
            {
                await db.SaveChangesAsync();
            }
            return count > 0;
        }


    }
}
