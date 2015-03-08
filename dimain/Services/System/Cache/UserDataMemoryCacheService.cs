using dimain.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dimain.Services.System.Cache
{
    public class UserDataMemoryCacheService : dimain.Services.System.Cache.IUserDataMemoryCacheService
    {
        private readonly IMemoryCacheSingleton MemoryCache;
        public UserDataMemoryCacheService(IMemoryCacheSingleton memoryCache)
        {
            MemoryCache = memoryCache;
        }

        public UserDataCache this[string userid]
        {
            get
            {
                Dictionary<string, UserDataCache> users = (Dictionary<string, UserDataCache>)(MemoryCache["users"] ?? new Dictionary<string, UserDataCache>());
                if(users.ContainsKey(userid))
                {
                    return users[userid];
                }
                return null;
            }

            set
            {
                Dictionary<string, UserDataCache> users = null;
                if (MemoryCache["users"] == null)
                {
                    users = new Dictionary<string, UserDataCache>();
                    MemoryCache["users"] = users;
                }
                else
                {
                    users = (Dictionary<string, UserDataCache>)MemoryCache["users"];
                }

                users[userid] = value;
            }
        }

        public bool CheckUserExists(string username)
        {
            Dictionary<string, UserDataCache> users = (Dictionary<string, UserDataCache>)(MemoryCache["users"] ?? new Dictionary<string,UserDataCache>());
            foreach(var user in users)
            {
                if(user.Value.Username == username)
                {
                    return true;
                }
            }

            return false;
        }

        public string GetUserIdByUsername(string username)
        {
            Dictionary<string, UserDataCache> users = (Dictionary<string, UserDataCache>)(MemoryCache["users"] ?? new Dictionary<string, UserDataCache>());
            foreach (var user in users)
            {
                if (user.Value.Username == username)
                {
                    return user.Value.UserId;
                }
            }

            return String.Empty;
        }

        public string GetUsernameFromUserId(string userId)
        {
            UserDataCache user = this[userId];
            if(user!=null)
            {
                return user.Username;
            }
            return String.Empty;
        }

        public bool CheckUserExistsLocallyByUserId(string userId)
        {

            UserDataCache user = this[userId];
            if(user!=null && user.UserIsLocal)
            {
                return true;
            }

            return false;
        }

        public bool CheckUserExistsByUserId(string userId)
        {
            Dictionary<string, UserDataCache> users = (Dictionary<string, UserDataCache>)(MemoryCache["users"] ?? new Dictionary<string, UserDataCache>());
            return users.ContainsKey(userId);
        }

        public void SetUserData(UserDataCache data)
        {
            Dictionary<string, UserDataCache> users = null;
            if(MemoryCache["users"] == null)
            {
                users = new Dictionary<string, UserDataCache>();
                MemoryCache["users"] = users;
            }
            else
            {
                users = (Dictionary<string, UserDataCache>)MemoryCache["users"];
            }

            users[data.UserId] = data;
        }

        
    }
}
