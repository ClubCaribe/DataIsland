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
    public class DbParticipantsService : Streamail.Services.db.IDbParticipantsService
    {
        private readonly IStreamailDatabaseManagerSingleton dbManager;
        public DbParticipantsService(IStreamailDatabaseManagerSingleton manager)
        {
            this.dbManager = manager;
        }

        private async Task<bool> CheckParticipantExists(string entityId, string participantId, DiStreamailContext db)
        {
            var res = await db.Participants.Where(x => x.EntityID == entityId && x.ParticipantID == participantId).ToListAsync();
            if(res.Count>0)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> AddSender(string entityId, string participantId, string participantName, string ownerUsername)
        {
            using(var db = this.dbManager.GetDbContext(ownerUsername))
            {
                if (!await this.CheckParticipantExists(entityId, participantId, db))
                {
                    Participant prt = new Participant();
                    prt.ID = Guid.NewGuid().ToString();
                    prt.IsSender = true;
                    prt.EntityID = entityId;
                    prt.IsRead = true;
                    prt.ParticipantID = participantId;
                    prt.ParticipantName = participantName;

                    db.Participants.Add(prt);
                    await db.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> AddParticipant(string entityId, string participantId, string participantName, bool isRead, string ownerUsername)
        {
            using (var db = this.dbManager.GetDbContext(ownerUsername))
            {
                if (!await this.CheckParticipantExists(entityId, participantId, db))
                {
                    Participant prt = new Participant();
                    prt.ID = Guid.NewGuid().ToString();
                    prt.IsSender = true;
                    prt.EntityID = entityId;
                    prt.IsRead = isRead;
                    prt.ParticipantID = participantId;
                    prt.ParticipantName = participantName;

                    db.Participants.Add(prt);
                    await db.SaveChangesAsync();
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> AddParticipants(string entityId, List<Participant> participants, string ownerUsername)
        {
            using (var db = this.dbManager.GetDbContext(ownerUsername))
            {
                foreach (var participant in participants)
                {
                    if (!await this.CheckParticipantExists(entityId, participant.ParticipantID, db))
                    {
                        participant.ID = Guid.NewGuid().ToString();
                        participant.EntityID = entityId;
                        db.Participants.Add(participant);
                    }
                }

                await db.SaveChangesAsync();
                return true;

            }
            return false;
        }

        public async Task<bool> AddParticipants(string entityId, List<Participant> participants, DiStreamailContext db)
        {
            foreach (var participant in participants)
            {
                if (!await this.CheckParticipantExists(entityId, participant.ParticipantID, db))
                {
                    participant.ID = Guid.NewGuid().ToString();
                    participant.EntityID = entityId;
                    db.Participants.Add(participant);
                }
            }
            return true;
        }

        public async Task<List<Participant>> GetEntityParticipants(string entityId, string ownerUsername)
        {
            using(var db = this.dbManager.GetDbContext(ownerUsername))
            {
                var res = await db.Participants.Where(x => x.EntityID == entityId).ToListAsync();
                return res;
            }
        }

        public async Task<Participant> GetEntitySender(string entityId, string ownerUsername)
        {
            using (var db = this.dbManager.GetDbContext(ownerUsername))
            {
                var res = await db.Participants.Where(x => x.EntityID == entityId && x.IsSender == true).SingleOrDefaultAsync();
                return res;
            }
        }

        public async Task<bool> SetReadStatus(string entityId, string participantId, bool isRead, string ownerUsername)
        {
            using(var db = this.dbManager.GetDbContext(ownerUsername))
            {
                var part = await db.Participants.Where(x => x.EntityID == entityId && x.ParticipantID == participantId).SingleOrDefaultAsync();
                if(part!=null)
                {
                    part.IsRead = isRead;
                    await db.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> DeleteEntityParticipant(string entityId, string participantId, string ownerUsername)
        {
            using (var db = this.dbManager.GetDbContext(ownerUsername))
            {
                int rows = await db.Database.ExecuteSqlCommandAsync("DELETE FROM MessageParticipants WHERE ParticipantID=\"" + participantId + "\" AND EntityID=\"" + entityId + "\"");
                if(rows>0)
                {
                    await db.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> DeleteEntityParticipants(string entityId, string ownerUsername)
        {
            using(var db = dbManager.GetDbContext(ownerUsername))
            {
                int rows = await db.Database.ExecuteSqlCommandAsync("DELETE FROM MessageParticipants WHERE EntityID=\"" + entityId + "\"");
                if (rows > 0)
                {
                    await db.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }
    }
}
