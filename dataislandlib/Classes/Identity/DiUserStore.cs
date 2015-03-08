using dataislandcommon.Models.userdb;
using dataislandcommon.Services.System;
using dataislandcommon.Services.FileSystem;
using dataislandcommon.Utilities;
using dataislandcommon.Utilities.enums;
using dimain.Services.System;
using Microsoft.AspNet.Identity;
using Autofac;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;

namespace dataislandcommon.Classes.Identity
{
    public class DiUserStore :
        IUserStore<DiUser>,
        IUserPasswordStore<DiUser>,
        IUserSecurityStampStore<DiUser>,
        IUserRoleStore<DiUser>,
        IUserLoginStore<DiUser>,
        IUserClaimStore<DiUser>,
        IUserEmailStore<DiUser>,
        IUserLockoutStore<DiUser,string>,
        IUserTwoFactorStore<DiUser,string>
    {

        #region IUserStore
        public async Task CreateAsync(DiUser user)
        {
            using (var container = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                IUserService userService = container.Resolve<IUserService>();
                IDataIslandService DataIsland = container.Resolve<IDataIslandService>();
                IDiUserService DiUsers = container.Resolve<IDiUserService>();
                IUserRoleService UserRoles = container.Resolve<IUserRoleService>();


                string dataIslandID = await DataIsland.GetDataIslandID();
                UserRegistrationResult res = await userService.RegisterUser(user.Id, user.UserName, user.PasswordHash, user.userAccount.Email, user.SecurityStamp, "", "", dataIslandID);

                if (res == UserRegistrationResult.Success)
                {
                    await DiUsers.AddUser(user.UserName, user.Id);
                    return;
                }

            }

        }

        public async Task DeleteAsync(DiUser user)
        {
            using (var container = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                IUserService userService = container.Resolve<IUserService>();
                IDataIslandService DataIsland = container.Resolve<IDataIslandService>();
                IDiUserService DiUsers = container.Resolve<IDiUserService>();
                IUserRoleService UserRoles = container.Resolve<IUserRoleService>();

                string dataIslandID = await DataIsland.GetDataIslandID();

                bool result = await userService.DeleteUser(user.UserName, dataIslandID);
                if (result)
                {
                    await DiUsers.DeleteUser(user.UserName);
                }
            }

        }

        public async Task<DiUser> FindByIdAsync(string userId)
        {
            using (var container = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                IDiUserService DiUsers = container.Resolve<IDiUserService>();
                IFilePathProviderService FilePathProvider = container.Resolve<IFilePathProviderService>();
                IUserService userService = container.Resolve<IUserService>();

                dimain.Models.maindb.DiUser mainuser = await DiUsers.GetUserById(userId);

                if (mainuser != null)
                {
                    UserAccount acc = await userService.GetUserAccount(mainuser.Username);
                    if (acc != null)
                    {
                        DiUser user = new DiUser();
                        user.userAccount = acc;
                        return user;
                    }
                }
                return null;
            }
        }

        public async Task<DiUser> FindByNameAsync(string userName)
        {
            using (var container = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                IUserService userService = container.Resolve<IUserService>();
                UserAccount acc = await userService.GetUserAccount(userName);
                if (acc != null)
                {
                    DiUser user = new DiUser();
                    user.userAccount = acc;
                    return user;
                }
                return null;
            }
        }

        public async Task UpdateAsync(DiUser user)
        {
            using (var container = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                IUserService userService = container.Resolve<IUserService>();
                await userService.UpdateUser(user.userAccount);
            }
        }

        public void Dispose()
        {
        }
        #endregion

        #region IUserPasswordStore
        public async Task<string> GetPasswordHashAsync(DiUser user)
        {
            using (var container = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                IUserService userService = container.Resolve<IUserService>();
                UserAccount account = await userService.GetUserAccount(user.UserName);
                return account.Password;
            }
        }

        public async Task<bool> HasPasswordAsync(DiUser user)
        {
            using (var container = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                IUserService userService = container.Resolve<IUserService>();
                UserAccount account = await userService.GetUserAccount(user.UserName);
                return !string.IsNullOrEmpty(account.Password);
            }
        }

        public Task SetPasswordHashAsync(DiUser user, string passwordHash)
        {
            user.PasswordHash = passwordHash;

            return Task.FromResult<Object>(null);
        } 
        #endregion

        #region IUserSecurityStampStore
        public async Task<string> GetSecurityStampAsync(DiUser user)
        {
            using (var container = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                IUserService userService = container.Resolve<IUserService>();
                UserAccount acc = await userService.GetUserAccount(user.UserName);
                return acc.SecurityStamp;
            }
        }

        public Task SetSecurityStampAsync(DiUser user, string stamp)
        {
            user.SecurityStamp = stamp;

            return Task.FromResult(0);
        } 
        #endregion

        #region IUserRoleStore
        public async Task AddToRoleAsync(DiUser user, string roleName)
        {
            using (var container = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                IUserRoleService RoleService = container.Resolve<IUserRoleService>();
                await RoleService.AddUserRole(user.UserName, roleName);
            }
        }

        public async Task<IList<string>> GetRolesAsync(DiUser user)
        {
            using (var container = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                IUserRoleService RoleService = container.Resolve<IUserRoleService>();
                return await RoleService.GetUserRoles(user.UserName);
            }
        }

        public async Task<bool> IsInRoleAsync(DiUser user, string roleName)
        {
            using (var container = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                IUserRoleService RoleService = container.Resolve<IUserRoleService>();
                List<string> roles = await RoleService.GetUserRoles(user.UserName);
                if (roles != null && roles.Count > 0)
                {
                    foreach (var role in roles)
                    {
                        if (role.ToLower() == "all" || role.ToLower() == roleName.ToLower())
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public async Task RemoveFromRoleAsync(DiUser user, string roleName)
        {
            using (var container = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                IUserRoleService RoleService = container.Resolve<IUserRoleService>();
                await RoleService.DeleteUserRole(user.UserName, roleName);
            }
        } 
        #endregion

        #region IUserLoginStore
        public async Task AddLoginAsync(DiUser user, UserLoginInfo login)
        {
            using (var container = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                IUserService userService = container.Resolve<IUserService>();
                IDiUserService DiUserService = container.Resolve<IDiUserService>();
                if (await userService.AddExternalLogin(user.UserName, login.LoginProvider, login.ProviderKey))
                {
                    await DiUserService.AddExternalLogin(user.UserName, login.ProviderKey);
                }
            }
        }

        public async Task<DiUser> FindAsync(UserLoginInfo login)
        {
            using (var container = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                IUserService userService = container.Resolve<IUserService>();
                IDiUserService DiUserService = container.Resolve<IDiUserService>();
                string userName = await DiUserService.GetExternalLoginUsername(login.ProviderKey);
                if (!string.IsNullOrEmpty(userName))
                {
                    UserAccount acc = await userService.GetUserAccount(userName);
                    DiUser user = new DiUser();
                    user.userAccount = acc;
                    return user;
                }
                return null;
            }
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(DiUser user)
        {
            using (var container = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                IUserService userService = container.Resolve<IUserService>();
                List<UserExternalLogin> extLogins = await userService.GetExternalLogins(user.UserName);
                List<UserLoginInfo> logins = new List<UserLoginInfo>();
                foreach (var login in extLogins)
                {
                    UserLoginInfo log = new UserLoginInfo(login.LoginProvider, login.ProviderKey);
                    logins.Add(log);
                }
                return logins;
            }
        }

        public async Task RemoveLoginAsync(DiUser user, UserLoginInfo login)
        {
            using (var container = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                IUserService userService = container.Resolve<IUserService>();
                IDiUserService DiUserService = container.Resolve<IDiUserService>();
                await userService.DeleteExternalLogin(user.UserName, login.ProviderKey);
                await DiUserService.DeleteExternalLogin(login.ProviderKey);
            }
        } 
        #endregion

        #region IUserClaimStore
        public async Task AddClaimAsync(DiUser user, Claim claim)
        {
            using (var container = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                IUserService userService = container.Resolve<IUserService>();
                await userService.AddClaim(user.UserName, claim.Type, claim.Value);
            }
        }

        public async Task<IList<System.Security.Claims.Claim>> GetClaimsAsync(DiUser user)
        {
            using (var ioc = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                IUserService userService = ioc.Resolve<IUserService>();
                List<Claim> claims = new List<Claim>();
                foreach (var claim in await userService.GetUserClaims(user.UserName))
                {
                    Claim cl = new Claim(claim.Type, claim.Value);
                    claims.Add(cl);
                }
                return claims;
            }
        }

        public async Task RemoveClaimAsync(DiUser user, System.Security.Claims.Claim claim)
        {
            using (var ioc = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                IUserService userService = ioc.Resolve<IUserService>();
                await userService.DeleteClaim(user.UserName, claim.Type, claim.Value);
            }
        } 
        #endregion

        #region IUserEmailStore
        public async Task<DiUser> FindByEmailAsync(string email)
        {
            using (var ioc = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                IUserService userService = ioc.Resolve<IUserService>();
                IDiUserService DiUserService = ioc.Resolve<IDiUserService>();
                string username = await DiUserService.GetUsenameByEmail(email);
                if (!string.IsNullOrEmpty(username))
                {
                    UserAccount acc = await userService.GetUserAccount(username);
                    DiUser usr = new DiUser();
                    usr.userAccount = acc;
                    return usr;
                }
                return null;
            }
        }

        public async Task<string> GetEmailAsync(DiUser user)
        {
            using (var ioc = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                IUserService userService = ioc.Resolve<IUserService>();
                UserAccount acc = await userService.GetUserAccount(user.UserName);
                if (acc != null)
                {
                    return acc.Email;
                }
                return "";
            }
        }

        public Task<bool> GetEmailConfirmedAsync(DiUser user)
        {
            return Task.FromResult<bool>(true);
            //IUserService userService = UnityConfig.GetConfiguredContainer().Resolve<IUserService>();
            //UserAccount acc = await userService.GetUserAccount(user.UserName);
            //if (acc != null)
            //{
            //    return acc.EmailConfirmed;
            //}
            //return false;
        }

        public async Task SetEmailAsync(DiUser user, string email)
        {
            using (var ioc = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                IUserService userService = ioc.Resolve<IUserService>();
                await userService.SetEmail(user.UserName, email);
            }
        }

        public async Task SetEmailConfirmedAsync(DiUser user, bool confirmed)
        {
            using (var ioc = AutofacConfig.GetConfiguredContainer().BeginLifetimeScope())
            {
                IUserService userService = ioc.Resolve<IUserService>();
                await userService.SetEmailConfirmed(user.UserName, confirmed);
            }
        } 
        #endregion

        #region IUserLockoutStore
        public Task<int> GetAccessFailedCountAsync(DiUser user)
        {
            return Task.FromResult<int>(0);
        }

        public Task<bool> GetLockoutEnabledAsync(DiUser user)
        {
            return Task.FromResult<bool>(true);
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(DiUser user)
        {
            DateTimeOffset dtOffset = new DateTimeOffset(DateTime.Now.AddDays(-1));
            return Task.FromResult<DateTimeOffset>(dtOffset);
        }

        public Task<int> IncrementAccessFailedCountAsync(DiUser user)
        {
            return Task.FromResult<int>(0);
        }

        public Task ResetAccessFailedCountAsync(DiUser user)
        {
            return Task.FromResult<object>(null);
        }

        public Task SetLockoutEnabledAsync(DiUser user, bool enabled)
        {
            return Task.FromResult<object>(null);
        }

        public Task SetLockoutEndDateAsync(DiUser user, DateTimeOffset lockoutEnd)
        {
            return Task.FromResult<object>(null);
        } 
        #endregion

        #region IUserTwoFactorStore
        public Task<bool> GetTwoFactorEnabledAsync(DiUser user)
        {
            return Task.FromResult<bool>(false);
        }

        public Task SetTwoFactorEnabledAsync(DiUser user, bool enabled)
        {
            return Task.FromResult<object>(null);
        } 
        #endregion
    }
}
