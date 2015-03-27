using dimain.Services.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using dimain.Models.maindb;

using dataislandcommon.Services.Utilities;
using dimain.Models.ViewModels;
using dataislandcommon.Utilities;
using Newtonsoft.Json;
using dimain.Services.Communication;
using dimain.Services.System.Cache;

namespace dimain.Services.System
{
    public class DiUserService : dimain.Services.System.IDiUserService
    {
        
        public IUserRoleService RoleService { get; set; }
        
        public IDataIslandSettingsService DiSettings { get; set; }
        
        public ICryptographySingleton Cryptography { get; set; }
        
        public IMainDiCommandsService MainDiCommands { get; set; }
        
        public IUserDataMemoryCacheService UserDataMemoryCache { get; set; }

        private IMainDatabaseManagerSingleton DbManager;
        public DiUserService(IMainDatabaseManagerSingleton _dbManager)
        {
            DbManager = _dbManager;
        }

        public async Task<List<DiUser>> GetUsers(int page, int numOfElements, string searchPhrase)
        {
            List<DiUser> users = new List<DiUser>();

            try
            {
                using (var db = DbManager.GetMainDatabaseConnection())
                {
                    if (string.IsNullOrEmpty(searchPhrase))
                    {
                        var lst = await db.DiUser.Include(x => x.UserRoles).OrderBy(x => x.Username).Skip((page - 1) * numOfElements).Take(numOfElements).ToListAsync();
                        if (lst.Count > 0)
                        {
                            return lst;
                        }
                    }
                    else
                    {
                        var lst = await db.DiUser.Where(x => x.Username.ToLower().Contains(searchPhrase.ToLower()) || x.Name.ToLower().Contains(searchPhrase.ToLower())).Include(x => x.UserRoles).OrderBy(x => x.Username).Skip((page - 1) * numOfElements).Take(numOfElements).ToListAsync();

                        if (lst.Count > 0)
                        {
                            return lst;
                        }
                    }
                }
            }
            catch
            {
            }
            return users;
        }

        public async Task<List<DiUser>> GetUsers()
        {
            using (var db = DbManager.GetMainDatabaseConnection())
            {
                var lst = await db.DiUser.ToListAsync();
                return lst;
            }
        }

        public async Task<int> GetUsersCount(string searchPhrase)
        {
            using (var db = DbManager.GetMainDatabaseConnection())
            {
                var res = await db.UserRole.Where(x => x.Username.Contains(searchPhrase)).OrderBy(x => x.Username).CountAsync();

                return res;
            }
        }

        public async Task<bool> CheckUserExists(string username)
        {
            using (var db = DbManager.GetMainDatabaseConnection())
            {
                var res = await db.DiUser.Where(x => x.Username.ToLower() == username.ToLower()).ToListAsync();
                if (res != null && res.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> CheckUserIdExists(string userId)
        {
            if(this.UserDataMemoryCache.CheckUserExistsLocallyByUserId(userId))
            {
                return true;
            }

            using (var db = DbManager.GetMainDatabaseConnection())
            {
                var res = await db.DiUser.Where(x => x.UserId == userId).ToListAsync();
                if (res.Count > 0)
                {
                    UserDataCache usercache = new UserDataCache();
                    usercache.UserIsLocal = true;
                    usercache.UserId = res[0].UserId;
                    usercache.Username = res[0].Username;
                    this.UserDataMemoryCache[userId] = usercache;
                    return true;
                }
            }

            return false;
        }

        public async Task<string> GetUserIdByFromUsername(string username)
        {
            string userId = this.UserDataMemoryCache.GetUserIdByUsername(username);

            if (String.IsNullOrEmpty(userId))
            {
                using (var db = DbManager.GetMainDatabaseConnection())
                {
                    var res = await db.DiUser.Where(x => x.Username == username).ToListAsync();
                    if (res.Count > 0)
                    {
                        UserDataCache usercache = new UserDataCache();
                        usercache.UserIsLocal = true;
                        usercache.UserId = res[0].UserId;
                        usercache.Username = res[0].Username;
                        this.UserDataMemoryCache[userId] = usercache;
                        userId = usercache.UserId;
                    }
                    else
                    {
                        userId = await this.MainDiCommands.GetUserID(username);
                        if (!string.IsNullOrEmpty(userId))
                        {
                            UserDataCache usercache = new UserDataCache();
                            usercache.UserIsLocal = false;
                            usercache.UserId = userId;
                            usercache.Username = username;
                            this.UserDataMemoryCache[userId] = usercache;
                            userId = usercache.UserId;
                        }
                    }
                }
            }

            return userId;
        }

        public async Task<string> GetUsernameFromUserId(string userId)
        {
            string username = this.UserDataMemoryCache.GetUsernameFromUserId(userId);

            if (String.IsNullOrEmpty(username))
            {
                using (var db = DbManager.GetMainDatabaseConnection())
                {
                    var res = await db.DiUser.Where(x => x.UserId == userId).ToListAsync();
                    if (res.Count > 0)
                    {
                        UserDataCache usercache = new UserDataCache();
                        usercache.UserIsLocal = true;
                        usercache.UserId = res[0].UserId;
                        usercache.Username = res[0].Username;
                        this.UserDataMemoryCache[userId] = usercache;
                        username = usercache.Username;
                    }
                }
            }

            return username;
        }

        public async Task<bool> AddUser(string username, string id)
        {
            if (!await CheckUserExists(username))
            {
                using (var db = DbManager.GetMainDatabaseConnection())
                {
                    DiUser usr = new DiUser();
                    usr.Username = username;
                    usr.Name = username;
                    usr.Id = id;
                    usr.UserId = this.Cryptography.GetSha1AsBase64String(username);
                    usr.ProAccountExpirationDate = DateTime.Now.AddDays(-1);
                    db.DiUser.Add(usr);
                    int count = await db.SaveChangesAsync();
                    if (count > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<int> UpdateUsersIds()
        {
            int count = 0;
            using (var db = DbManager.GetMainDatabaseConnection())
            {
                var users = await db.DiUser.ToListAsync();

                foreach (var user in users)
                {
                    if (string.IsNullOrEmpty(user.UserId))
                    {
                        user.UserId = this.Cryptography.GetSha1AsBase64String(user.Username);
                        count++;
                    }
                }
                if (count > 0)
                {
                    await db.SaveChangesAsync();
                }
            }

            return count;
        }

        public async Task<DiUser> GetUserById(string id)
        {
            using (var db = DbManager.GetMainDatabaseConnection())
            {
                var res = await db.DiUser.Where(x => x.Id == id).SingleOrDefaultAsync();
                if (res != null)
                {
                    return res;
                }
            }
            return null;
        }

        public async Task<DiUser> GetUserByUserId(string userId)
        {
            using (var db = DbManager.GetMainDatabaseConnection())
            {
                var res = await db.DiUser.Where(x => x.UserId == userId).SingleOrDefaultAsync();
                if (res != null)
                {
                    return res;
                }
            }
            return null;
        }

        public async Task<DiUser> GetUserByUsername(string username)
        {
            using (var db = DbManager.GetMainDatabaseConnection())
            {
                var res = await db.DiUser.Where(x => x.Username == username).SingleOrDefaultAsync();
                if (res != null)
                {
                    return res;
                }
            }
            return null;
        }


        public async Task<bool> DeleteUser(string username)
        {
            using (var db = DbManager.GetMainDatabaseConnection())
            {
                var lst = await db.DiUser.Where(x => x.Username == username).ToListAsync();
                db.DiUser.RemoveRange(lst);
                await db.SaveChangesAsync();
                await RoleService.DeleteUserRoles(username);
            }
            return true;
        }

        public async Task<bool> ChangeUserName(string username, string name)
        {
            using (var db = DbManager.GetMainDatabaseConnection())
            {
                var usr = await db.DiUser.Where(x => x.Username == username).SingleOrDefaultAsync();
                if (usr != null)
                {
                    usr.Name = name;
                    await db.SaveChangesAsync();
                    return true;
                }

                return false;
            }
        }

        public async Task<bool>SetProAccountExpirationDate(string username, DateTime proAccountExpirationDate)
        {
            using (var db = DbManager.GetMainDatabaseConnection())
            {
                var usr = await db.DiUser.Where(x => x.Username == username).SingleOrDefaultAsync();
                if (usr != null)
                {
                    usr.ProAccountExpirationDate = proAccountExpirationDate;
                    await db.SaveChangesAsync();
                    return true;
                }

                return false;
            }
        }

        public async Task<bool> AddExternalLogin(string username, string ProviderKey)
        {
            if (!await this.CheckExternalLoginExists(ProviderKey))
            {
                using (var db = DbManager.GetMainDatabaseConnection())
                {
                    DiUserExternalLogin login = new DiUserExternalLogin();
                    login.ProviderKey = ProviderKey;
                    login.UserName = username;
                    db.ExternalLogins.Add(login);
                    await db.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> CheckExternalLoginExists(string providerKey)
        {
            using (var db = DbManager.GetMainDatabaseConnection())
            {
                var res = await db.ExternalLogins.Where(x => x.ProviderKey == providerKey).ToListAsync();
                if (res != null && res.Count > 0)
                {
                    return true;
                }
                return false;
            }
        }

        public async Task<string> GetExternalLoginUsername(string providerKey)
        {
            using (var db = DbManager.GetMainDatabaseConnection())
            {
                var res = await db.ExternalLogins.Where(x => x.ProviderKey == providerKey).ToListAsync();
                if (res != null && res.Count > 0)
                {
                    return res[0].UserName;
                }
                return "";
            }
        }

        public async Task DeleteExternalLogin(string providerKey)
        {
            using (var db = DbManager.GetMainDatabaseConnection())
            {
                var lst = await db.ExternalLogins.Where(x => x.ProviderKey == providerKey).ToListAsync();
                db.ExternalLogins.RemoveRange(lst);
                await db.SaveChangesAsync();
            }
        }

        public async Task SetEmail(string username, string email)
        {
            using (var db = DbManager.GetMainDatabaseConnection())
            {
                var usr = await db.DiUser.Where(x => x.Username == username).SingleAsync();
                if (usr != null)
                {
                    usr.Email = email;
                    await db.SaveChangesAsync();
                }
            }
        }

        public async Task<string> GetUsenameByEmail(string email)
        {
            using (var db = DbManager.GetMainDatabaseConnection())
            {
                var usr = await db.DiUser.Where(x => x.Email.ToLower() == email.ToLower()).ToListAsync();
                if (usr != null && usr.Count > 0)
                {
                    return usr[0].Username;
                }
                return String.Empty;
            }
        }

        public async Task<DiUserData> GetDIUserDataFromUserId(string userId)
        {
            using (var db = DbManager.GetMainDatabaseConnection())
            {
                var usr = await db.DiUserData.Where(x => x.UserId == userId).ToListAsync();
                string userDataislandId;
                string userPublicKey;
                if (usr != null && usr.Count > 0)
                {
                    if (usr[0].LastUpdate.AddDays(1) > DateTime.Now)
                    {
                        return usr[0];
                    }
                    
                    userDataislandId = await MainDiCommands.GetUserDataIslandIDFromUserId(userId);
                    userPublicKey = await MainDiCommands.GetUserDataIslandPublicKeyFromUserId(userId);

                    if (userPublicKey != "err")
                    {
                        usr[0].LastUpdate = DateTime.Now;
                        usr[0].PublicKey = userPublicKey;
                        usr[0].DatIslandId = userDataislandId;

                        await db.SaveChangesAsync();

                        return usr[0];
                    }
                    else
                    {
                        return null;
                    }
                }


                userDataislandId = await MainDiCommands.GetUserDataIslandIDFromUserId(userId);
                userPublicKey = await MainDiCommands.GetUserDataIslandPublicKeyFromUserId(userId);


                if (userPublicKey != "err")
                {
                    DiUserData userData = new DiUserData();
                    userData.DatIslandId = userDataislandId;
                    userData.LastUpdate = DateTime.Now;
                    userData.PublicKey = userPublicKey;
                    userData.UserId = userId;

                    db.DiUserData.Add(userData);
                    await db.SaveChangesAsync();

                    return userData;
                }
            }

            return null;
        }

        

    }
}
