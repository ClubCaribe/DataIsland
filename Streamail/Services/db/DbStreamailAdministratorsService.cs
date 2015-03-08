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
    public class DbStreamailAdministratorsService : Streamail.Services.db.IDbStreamailAdministratorsService
    {
        private readonly IStreamailDatabaseManagerSingleton dbManager;
        public DbStreamailAdministratorsService(IStreamailDatabaseManagerSingleton manager)
        {
            this.dbManager = manager;
        }

        private async Task<bool> CheckAdministratorExists(string streamailId, string participantId, DiStreamailContext db)
        {
            var res = await db.Administrators.Where(x => x.StreamailID == streamailId && x.ParticipantID == participantId).ToListAsync();
            if(res.Count>0)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> AddAdministrator(string streamailId, string participantId, string participantName, StreamailAdministrationRole role, string ownerUsername)
        {
            using(var db = this.dbManager.GetDbContext(ownerUsername))
            {
                if(!await this.CheckAdministratorExists(streamailId,participantId,db))
                {
                    StreamailAdministrator adm = new StreamailAdministrator();
                    adm.ID = Guid.NewGuid().ToString();
                    adm.ParticipantID = participantId;
                    adm.ParticipantName = participantName;
                    adm.Role = role;
                    adm.StreamailID = streamailId;
                    db.Administrators.Add(adm);

                    await db.SaveChangesAsync();

                    return true;
                }
            }
            return false;
        }

        public async Task<bool> AddAdministrators(List<StreamailAdministrator> administrators, string streamailId, string ownerUsername)
        {
            using(var db = this.dbManager.GetDbContext(ownerUsername))
            {
                bool added = false;
                foreach(var admin in administrators)
                {
                    if(!await this.CheckAdministratorExists(streamailId,admin.ParticipantID,db))
                    {
                        admin.ID = Guid.NewGuid().ToString();
                        admin.StreamailID = streamailId;
                        db.Administrators.Add(admin);
                        added = true;
                    }
                }

                if(added)
                {
                    await db.SaveChangesAsync();
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> AddAdministrators(List<StreamailAdministrator> administrators, string streamailId, DiStreamailContext db)
        {
            bool added = false;
            foreach (var admin in administrators)
            {
                if (!await this.CheckAdministratorExists(streamailId, admin.ParticipantID, db))
                {
                    admin.ID = Guid.NewGuid().ToString();
                    admin.StreamailID = streamailId;
                    db.Administrators.Add(admin);
                    added = true;
                }
            }
            if (added)
            {
                return true;
            }
            return false;
        }

        public async Task<List<StreamailAdministrator>> GetAdministrators(string streamailId, string ownerUsername)
        {
            using(var db = this.dbManager.GetDbContext(ownerUsername))
            {
                var res = await db.Administrators.Where(x => x.StreamailID == streamailId).ToListAsync();
                return res;
            }
        }

        public async Task<bool> SetAdministratorRole(string streamailId, string adminid, StreamailAdministrationRole role, string ownerUsername)
        {
            using (var db = this.dbManager.GetDbContext(ownerUsername))
            {
                var admin = await db.Administrators.Where(x => x.StreamailID == streamailId && x.ParticipantID == adminid).SingleOrDefaultAsync();
                if(admin!=null)
                {
                    admin.Role = role;
                    await db.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> DeleteAdministrator(string streamailId, string adminId, string ownerUsername)
        {
            using (var db = this.dbManager.GetDbContext(ownerUsername))
            {
                var num = await db.Database.ExecuteSqlCommandAsync("DELETE FROM Administrators WHERE StreamailID=\"" + streamailId + "\" AND ParticipantID=\"" + adminId + "\"");
                if(num>0)
                {
                    await db.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> DeleteAdministrators(string streamailId, string ownerUsername)
        {
            using (var db = this.dbManager.GetDbContext(ownerUsername))
            {
                var num = await db.Database.ExecuteSqlCommandAsync("DELETE FROM Administrators WHERE StreamailID=\"" + streamailId + "\"");
                if (num > 0)
                {
                    await db.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }


    }
}
