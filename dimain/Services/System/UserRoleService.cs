using dimain.Services.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using dimain.Models.maindb; 

namespace dimain.Services.System
{
    public class UserRoleService : dimain.Services.System.IUserRoleService
    {
        private readonly IMainDatabaseManagerSingleton DbManager;

        public UserRoleService(IMainDatabaseManagerSingleton _dbManager)
        {
            DbManager = _dbManager;
        }

        public async Task<List<string>> GetUserRoles(string username)
        {
            List<string> userRoles = new List<string>();

            using (var db = DbManager.GetMainDatabaseConnection())
            {
                var roles = await db.UserRole.Where(x => x.Username == username).ToListAsync();
                if (roles.Count > 0)
                {
                    foreach (var role in roles)
                    {
                        userRoles.Add(role.Role);
                    }
                }
                return userRoles;
            }
        }

        public async Task<bool> CheckUserRoleExist(string username, string role)
        {
            using (var db = DbManager.GetMainDatabaseConnection())
            {
                var userRole = await db.UserRole.Where(x => x.Username == username && x.Role.ToLower() == role.ToLower()).ToListAsync();
                if (userRole.Count > 0)
                {
                    return true;
                }
                return false;
            }
        }

        public async Task<bool> AddUserRole(string username, string role)
        {
            if (!await CheckUserRoleExist(username, role))
            {
                using (var db = DbManager.GetMainDatabaseConnection())
                {
                    UserRole userRole = new UserRole();
                    userRole.ID = Guid.NewGuid().ToString();
                    userRole.Role = role;
                    userRole.Username = username;
                    db.UserRole.Add(userRole);
                    await db.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> AddUserRoles(string username, List<string> roles)
        {
            using (var db = DbManager.GetMainDatabaseConnection())
            {
                bool saveChanges = false;
                foreach (var role in roles)
                {
                    if (!await this.CheckUserRoleExist(username, role))
                    {
                        UserRole userRole = new UserRole();
                        userRole.ID = Guid.NewGuid().ToString();
                        userRole.Role = role;
                        userRole.Username = username;
                        db.UserRole.Add(userRole);
                        saveChanges = true;
                    }
                }
                if (saveChanges && await db.SaveChangesAsync() > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> DeleteUserRole(string username, string role)
        {
            using (var db = DbManager.GetMainDatabaseConnection())
            {
                var lst = await db.UserRole.Where(x => x.Username == username && x.Role == role).ToListAsync();
                if (lst.Count > 0)
                {
                    db.UserRole.RemoveRange(lst);
                    await db.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> DeleteUserRoles(string username, IEnumerable<string> roles)
        {
            using (var db = DbManager.GetMainDatabaseConnection())
            {
                var lst = await db.UserRole.Where(x => roles.Contains(x.Role) && x.Username == username).ToListAsync();
                if (lst.Count > 0)
                {
                    db.UserRole.RemoveRange(lst);
                    await db.SaveChangesAsync();
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> DeleteUserRoles(string username)
        {
            using (var db = DbManager.GetMainDatabaseConnection())
            {
                var lst = await db.UserRole.Where(x => x.Username == username).ToListAsync();
                if (lst.Count > 0)
                {
                    db.UserRole.RemoveRange(lst);
                    await db.SaveChangesAsync();
                    return true;
                }
            }

            return false;
        }

    }
}
