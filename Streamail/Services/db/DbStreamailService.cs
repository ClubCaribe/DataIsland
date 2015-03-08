using Streamail.Services.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Streamail.Models.db;
using Streamail.Classes.db;

namespace Streamail.Services.db
{
    public class DbStreamailService : Streamail.Services.db.IDbStreamailService
    {
        private readonly IStreamailDatabaseManagerSingleton dbManager;
        public DbStreamailService(IStreamailDatabaseManagerSingleton manager)
        {
            this.dbManager = manager;
        }

        public async Task<bool> CheckStreamailExists(string id, string ownerUsername)
        {
            using(var db = dbManager.GetDbContext(ownerUsername))
            {
                var res = await db.Headers.Where(x => x.ID == id).ToListAsync();
                if (res.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> CheckStreamailExists(string id, DiStreamailContext db)
        {
            var res = await db.Headers.Where(x => x.ID == id).ToListAsync();
            if (res.Count > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> CreateStreamail(string id, string subject, string streamailType, DateTime creationTime, DateTime lastModificationTime, int submessageDepth, bool isReadOnly, bool canAddSubMessages, bool sendReadNotifications, string ownerUsername)
        {
            using (var db = dbManager.GetDbContext(ownerUsername))
            {
                if (!await this.CheckStreamailExists(id, db))
                {
                    StreamailHeaders str = new StreamailHeaders();
                    str.CreationTime = creationTime;
                    str.CanAddSubmessages = canAddSubMessages;
                    str.ID = id;
                    str.LastModificationTime = lastModificationTime;
                    str.ReadOnly = isReadOnly;
                    str.SendReadNotifications = sendReadNotifications;
                    str.StreamailType = streamailType;
                    str.Subject = subject;
                    str.SubMessagesDepth = submessageDepth;

                    db.Headers.Add(str);
                    await db.SaveChangesAsync();
                    return true;

                }
            }
            return false;
        }

        public async Task<bool> CreateStreamail(StreamailHeaders streamail, string ownerUsername)
        {
            using (var db = dbManager.GetDbContext(ownerUsername))
            {
                if (!await this.CheckStreamailExists(streamail.ID, db))
                {
                    db.Headers.Add(streamail);
                    await db.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> CreateStreamail(StreamailHeaders streamail, DiStreamailContext db)
        {
            if (!await this.CheckStreamailExists(streamail.ID, db))
            {
                db.Headers.Add(streamail);
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateLastModificationTime(string id, DateTime modificationTime, string ownerUsername)
        {
            using (var db = dbManager.GetDbContext(ownerUsername))
            {
                var strm = await db.Headers.Where(x => x.ID == id).SingleOrDefaultAsync();
                if (strm != null)
                {
                    strm.LastModificationTime = modificationTime;
                    await db.SaveChangesAsync();
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateStreamail(StreamailHeaders streamail, string ownerUsername)
        {
            using(var db = dbManager.GetDbContext(ownerUsername))
            {
                var strm = await db.Headers.Where(x => x.ID == streamail.ID).SingleOrDefaultAsync();
                if(strm!=null)
                {
                    strm.CanAddSubmessages = streamail.CanAddSubmessages;
                    strm.CreationTime = streamail.CreationTime;
                    strm.LastModificationTime = streamail.LastModificationTime;
                    strm.ReadOnly = streamail.ReadOnly;
                    strm.SendReadNotifications = streamail.SendReadNotifications;
                    strm.StreamailType = streamail.StreamailType;
                    strm.Subject = streamail.Subject;
                    strm.SubMessagesDepth = streamail.SubMessagesDepth;

                    await db.SaveChangesAsync();

                    return true;
                }
            }
            return false;
        }

        public async Task<bool> ChangeSubject(string id, string newsubject, string ownerUsername)
        {
            using(var db = dbManager.GetDbContext(ownerUsername))
            {
                var strm = await db.Headers.Where(x => x.ID == id).SingleOrDefaultAsync();
                if (strm != null)
                {
                    strm.Subject = newsubject;
                    await db.SaveChangesAsync();
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> ChangeLastModificationTime(string id, DateTime lastModifyTime, string ownerUsername)
        {
            using (var db = dbManager.GetDbContext(ownerUsername))
            {
                var strm = await db.Headers.Where(x => x.ID == id).SingleOrDefaultAsync();
                if (strm != null)
                {
                    strm.LastModificationTime = lastModifyTime;
                    await db.SaveChangesAsync();
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> DeleteStreamail(string id, string ownerUsername)
        {
            using (var db = dbManager.GetDbContext(ownerUsername))
            {
                int numOfRows = await db.Database.ExecuteSqlCommandAsync("DELETE FROM DbStreamails WHERE ID=\"" + id + "\"");
                if (numOfRows > 0)
                {
                    await db.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }

        public async Task<List<string>> DeleteStreamails(List<string> ids, string ownerUsername)
        {
            List<string> deletedIds = new List<string>();
            using (var db = dbManager.GetDbContext(ownerUsername))
            {
                foreach(string id in ids)
                {
                    int numOfRows = await db.Database.ExecuteSqlCommandAsync("DELETE FROM DbStreamails WHERE ID=\"" + id + "\"");
                    if (numOfRows > 0)
                    {
                        deletedIds.Add(id);
                    }
                }
                if (deletedIds.Count > 0)
                {
                    await db.SaveChangesAsync();
                }
            }
            return deletedIds;
        }

        public async Task<List<StreamailHeaders>> GetStreamails(string ownerUsername)
        {
            using (var db = dbManager.GetDbContext(ownerUsername))
            {
                var res = await db.Headers.ToListAsync();
                return res;
            }
        }

        public async Task<StreamailHeaders> GetStreamail(string id, string ownerUsername)
        {
            using (var db = dbManager.GetDbContext(ownerUsername))
            {
                var res = await db.Headers.Where(x => x.ID == id).SingleOrDefaultAsync();
                if (res != null)
                {
                    return res;
                }
            }
            return null;
        }

        public async Task<List<StreamailHeaders>> GetStreamails(List<string> ids,string ownerUsername)
        {
            using (var db = dbManager.GetDbContext(ownerUsername))
            {
                var res = await db.Headers.Where(x => ids.Contains(x.ID)).ToListAsync();
                return res;
            }
        }

        public async Task<List<StreamailHeaders>> GetStreamails(string type, string ownerUsername)
        {
            using (var db = dbManager.GetDbContext(ownerUsername))
            {
                var res = await db.Headers.Where(x => x.StreamailType == type).ToListAsync();
                return res;
            }
        }

        public async Task<StreamailHeaders> GetStreamailHeadersFromMessageID(string messageID, string ownerUsername)
        {
            using (var db = dbManager.GetDbContext(ownerUsername))
            {
                var res = await db.Messages.Where(x => x.ID == messageID).SingleOrDefaultAsync();
                if (res != null)
                {
                    var streamail = await db.Headers.Where(x => x.ID == res.StreamailID).SingleOrDefaultAsync();
                    if (streamail != null)
                    {
                        return streamail;
                    }

                }
            }
            return null;
        }
    }
}
