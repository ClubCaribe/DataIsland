using Streamail.Classes.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Streamail.Models.db;

namespace Streamail.Services.db
{
    public class DbReadStatusesService : Streamail.Services.db.IDbReadStatusesService
    {
        private readonly IStreamailDatabaseManagerSingleton dbManager;
        public DbReadStatusesService(IStreamailDatabaseManagerSingleton manager)
        {
            dbManager = manager;
        }

        private async Task<bool> CheckStatusExists(string messageId, string participantId, DiStreamailContext db)
        {
            var res = await db.ReadStatuses.Where(x => x.MessageID == messageId && x.ParticipantID == participantId).ToListAsync();
            if(res!=null && res.Count>0)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> AddReadStatus(string messageId, string streamailId, string participantId, string ownerUsername)
        {
            using(var db = this.dbManager.GetDbContext(ownerUsername))
            {
                if(!await this.CheckStatusExists(messageId,participantId,db))
                {
                    ReadStatus status = new ReadStatus();
                    status.ID = Guid.NewGuid().ToString();
                    status.MessageID = messageId;
                    status.StreamailID = streamailId;
                    status.ParticipantID = participantId;
                    db.ReadStatuses.Add(status);

                    await db.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }

        public async Task<List<ReadStatus>> GetStatuses(string messageId, DiStreamailContext db)
        {
            return await db.ReadStatuses.Where(x => x.MessageID == messageId).ToListAsync();
        }

        public async Task DeleteReadStatuses(string messageId, DiStreamailContext db)
        {
            await db.Database.ExecuteSqlCommandAsync("DELETE FROM ReadStatuses WHERE MessageID = \"" + messageId + "\"");
        }
    }
}
