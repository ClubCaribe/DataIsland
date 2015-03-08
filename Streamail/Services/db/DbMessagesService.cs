using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Streamail.Classes.db;
using Streamail.Models.db;


namespace Streamail.Services.db
{
    public class DbMessagesService : Streamail.Services.db.IDbMessagesService
    {
        public IDbReadStatusesService ReadStatuses { get; set; }
        public IDbMessageCustomFiledsService CustomFields { get; set; }

        private readonly IStreamailDatabaseManagerSingleton dbManager;
        public DbMessagesService(IStreamailDatabaseManagerSingleton manager)
        {
            dbManager = manager;
        }

        public async Task<bool> CheckMessageExists(string id, string ownerUsername)
        {
            using (var db = dbManager.GetDbContext(ownerUsername))
            {
                var res = await db.Messages.Where(x => x.ID == id).ToListAsync();
                if (res.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> CheckMessageExists(string id, DiStreamailContext db)
        {
            var res = await db.Messages.Where(x => x.ID == id).ToListAsync();
            if (res.Count > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> AddMessage(string id, string streamailId, string renderedMessage, string rawMessage, DateTime sendTime, string parentMessageId, string rootMessageId, string rendererName, string ownerUsername)
        {
            using (var db = dbManager.GetDbContext(ownerUsername))
            {
                if (!await this.CheckMessageExists(id, db))
                {
                    StreamailMessage msg = new StreamailMessage();
                    msg.ID = id;
                    msg.StreamailID = streamailId;
                    msg.IsRead = false;
                    msg.ParentMessageID = parentMessageId;
                    msg.ReceiveTime = DateTime.UtcNow;
                    msg.Message = renderedMessage;
                    msg.RendererName = rendererName;
                    msg.SendTime = sendTime;
                    msg.RootMessageID = rootMessageId;
                    db.Messages.Add(msg);

                    RawMessage rawMsg = new RawMessage();
                    rawMsg.Message = rawMessage;
                    rawMsg.MessageId = id;
                    db.RawMessages.Add(rawMsg);

                    await db.SaveChangesAsync();
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> AddMessage(StreamailMessage msg, string ownerUsername)
        {
            using (var db = dbManager.GetDbContext(ownerUsername))
            {
                if (!await this.CheckMessageExists(msg.ID, db))
                {
                    db.Messages.Add(msg);

                    await db.SaveChangesAsync();
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> AddRawMessage(string id,string streamailId, string rawMessage, DateTime sendTime, string parentMessageId, string rootMessageId, string rendererName, string ownerUsername)
        {
            using (var db = dbManager.GetDbContext(ownerUsername))
            {
                if (!await this.CheckMessageExists(id, db))
                {
                    StreamailMessage msg = new StreamailMessage();
                    msg.ID = id;
                    msg.StreamailID = id;
                    msg.IsRead = false;
                    msg.ParentMessageID = parentMessageId;
                    msg.ReceiveTime = DateTime.UtcNow;
                    msg.Message = "";
                    msg.RendererName = rendererName;
                    msg.SendTime = sendTime;
                    msg.RootMessageID = rootMessageId;
                    db.Messages.Add(msg);

                    RawMessage rawMsg = new RawMessage();
                    rawMsg.Message = rawMessage;
                    rawMsg.MessageId = id;
                    db.RawMessages.Add(rawMsg);

                    await db.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> AddRawMessage(string id, string rawMessage, string ownerUsername)
        {
            using (var db = dbManager.GetDbContext(ownerUsername))
            {
                if (!await this.CheckMessageExists(id, db))
                {
                    RawMessage rawMsg = new RawMessage();
                    rawMsg.Message = rawMessage;
                    rawMsg.MessageId = id;
                    db.RawMessages.Add(rawMsg);

                    await db.SaveChangesAsync();
                }
            }
            return false;
        }

        public async Task<bool> UpdateRenderedMessage(string id, string renderedMessage, string ownerUsername)
        {
            using (var db = dbManager.GetDbContext(ownerUsername))
            {
                if (await this.CheckMessageExists(id, db))
                {
                    var msg = await db.Messages.Where(x => x.ID == id).SingleOrDefaultAsync();
                    if (msg != null)
                    {
                        msg.RendererName = renderedMessage;
                        await db.SaveChangesAsync();
                        return true;
                    }
                }
            }
            return false;
        }

        public async Task<bool> UpdateReadStatus(string id, bool isRead, string ownerUsername)
        {
            using (var db = dbManager.GetDbContext(ownerUsername))
            {
                if (await this.CheckMessageExists(id, db))
                {
                    var msg = await db.Messages.Where(x => x.ID == id).SingleOrDefaultAsync();
                    if (msg != null)
                    {
                        msg.IsRead = isRead;
                        await db.SaveChangesAsync();
                        return true;
                    }
                }
            }
            return false;
        }

        public async Task<StreamailMessage> GetMessage(string id, string ownerUsername)
        {
            using (var db = dbManager.GetDbContext(ownerUsername))
            {
                var msg = await db.Messages.Where(x => x.ID == id).SingleOrDefaultAsync();
                if (msg != null)
                {
                    msg.ReadStatuses = await this.ReadStatuses.GetStatuses(msg.ID, db);
                    msg.CustomFields = await this.CustomFields.GetCustomFields(msg.ID, db);
                    return msg;
                }
            }
            return null;
        }

        public async Task<List<StreamailMessage>> GetMessages(string streamailId,string ownerUsername)
        {
            using (var db = dbManager.GetDbContext(ownerUsername))
            {
                var res = await db.Messages.Where(x => x.StreamailID == streamailId && String.IsNullOrEmpty(x.ParentMessageID)).OrderBy(x=>x.SendTime) .ToListAsync();
                foreach (var msg in res)
                {
                    msg.ReadStatuses = await this.ReadStatuses.GetStatuses(msg.ID, db);
                    msg.CustomFields = await this.CustomFields.GetCustomFields(msg.ID, db);
                    await this.LoadChildrenMessages(msg, db);
                }
                return res;
            }
        }

        public async Task<List<StreamailMessage>> GetMessages(string streamailId, int pageNum , int pageSize, string ownerUsername)
        {
            using (var db = dbManager.GetDbContext(ownerUsername))
            {
                var res = await db.Messages.Where(x => x.StreamailID == streamailId && String.IsNullOrEmpty(x.ParentMessageID)).OrderByDescending(x => x.SendTime).ToListAsync();
                var pageres = res.Skip((pageNum - 1) * pageSize).Take(pageSize);
                foreach (var msg in pageres)
                {
                    msg.ReadStatuses = await this.ReadStatuses.GetStatuses(msg.ID, db);
                    msg.CustomFields = await this.CustomFields.GetCustomFields(msg.ID, db);
                    await this.LoadChildrenMessages(msg, db);
                }
                return pageres.ToList();
            }
        }

        public async Task<bool> MarkAllMessagesAsRead(string streamailId, string ownerUsername)
        {
            using (var db = dbManager.GetDbContext(ownerUsername))
            {
                var res = await db.Messages.Where(x => x.StreamailID == streamailId).ToListAsync();
                foreach (var msg in res)
                {
                    msg.IsRead = true;
                }
                if(res.Count>0)
                {
                    await db.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> MarkMessageAsRead(string messageId, string ownerUsername)
        {
            using (var db = dbManager.GetDbContext(ownerUsername))
            {
                var res = await db.Messages.Where(x => x.ID == messageId).SingleOrDefaultAsync();
                if(res!=null)
                {
                    res.IsRead = true;
                    await db.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }

        public async Task<string> GetMessageSenderID(string messageId, string ownerUsername)
        {
            using (var db = dbManager.GetDbContext(ownerUsername))
            {
                var res = await db.Messages.Where(x => x.ID == messageId).SingleOrDefaultAsync();
                if (res != null)
                {
                    return res.SenderID;
                }
            }
            return String.Empty;
        }

        public async Task<string> GetStreamailID(string messageId, string ownerUsername)
        {
            using (var db = dbManager.GetDbContext(ownerUsername))
            {
                var res = await db.Messages.Where(x => x.ID == messageId).SingleOrDefaultAsync();
                if (res != null)
                {
                    return res.StreamailID;
                }
            }
            return String.Empty;
        }

        public async Task<List<StreamailMessage>> FindMessages(string streamailId, string searchPhrase, string ownerUsername)
        {
            using (var db = dbManager.GetDbContext(ownerUsername))
            {
                var res = await db.Messages.Where(x => x.Message.IndexOf(searchPhrase) > -1).OrderBy(x => x.SendTime).ToListAsync();
                foreach (StreamailMessage msg in res)
                {
                    msg.ReadStatuses = await this.ReadStatuses.GetStatuses(msg.ID, db);
                    msg.CustomFields = await this.CustomFields.GetCustomFields(msg.ID, db);
                }
                return res;
            }
        }

        public async Task LoadChildrenMessages(StreamailMessage msg,  DiStreamailContext db)
        {
            var children = await db.Messages.Where(x => x.ParentMessageID == msg.ID).OrderBy(x=>x.SendTime).ToListAsync();
            foreach(var child in children)
            {
                child.ReadStatuses = await this.ReadStatuses.GetStatuses(child.ID, db);
                msg.CustomFields = await this.CustomFields.GetCustomFields(msg.ID, db);
                await this.LoadChildrenMessages(child, db);
            }
            msg.ChildrenMessages = children;
        }

        public async Task<List<StreamailMessage>> GetMessagesFromGivenID(string messageId, string ownerUsername)
        {
            using (var db = dbManager.GetDbContext(ownerUsername))
            {
                var firstMessage = await db.Messages.Where(x => x.ID == messageId).SingleOrDefaultAsync();
                if(firstMessage!=null)
                {
                    var messages = await db.Messages.Where(x => x.SendTime >= firstMessage.SendTime && String.IsNullOrEmpty(x.ParentMessageID)).OrderByDescending(x => x.SendTime).ToListAsync();
                    foreach(var message in messages)
                    {
                        message.ReadStatuses = await this.ReadStatuses.GetStatuses(message.ID, db);
                        message.CustomFields = await this.CustomFields.GetCustomFields(message.ID, db);
                        await this.LoadChildrenMessages(message, db);
                    }
                    return messages;
                }
            }
            return null;
        }

        public async Task DeleteMessage(string id, string ownerUsername)
        {
            using(var db = dbManager.GetDbContext(ownerUsername))
            {
                await db.Database.ExecuteSqlCommandAsync("DELETE FROM Messages WHERE ID=\"" + id + "\"");
                await this.ReadStatuses.DeleteReadStatuses(id, db);
                await this.CustomFields.DeleteCustomFileds(id, db);
                await db.SaveChangesAsync();
            }
        }

        public async Task DeleteMessagesFromStreamail(string streamailId, string ownerUsername)
        {
            using(var db = dbManager.GetDbContext(ownerUsername))
            {
                await db.Database.ExecuteSqlCommandAsync("DELETE FROM Messages WHERE StreamailID=\"" + streamailId + "\"");
                await db.SaveChangesAsync();
            }
        }

        public async Task<int> GetNumberOfUnreadMessages(string streamailID, string ownerUsername)
        {
            using (var db = dbManager.GetDbContext(ownerUsername))
            {
                int count = await db.Messages.Where(x => x.StreamailID == streamailID && x.IsRead == false).CountAsync();
                return count;
            }
            return 0;
        }
    }
}
