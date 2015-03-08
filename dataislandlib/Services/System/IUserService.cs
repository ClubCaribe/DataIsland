
using dataislandcommon.Models.userdb;
using dataislandcommon.Models.ViewModels;
using dataislandcommon.Utilities.enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dataislandcommon.Services.System
{
    public interface IUserService
    {
        Task AddClaim(string username, string type, string value);
        Task<bool> AddExternalLogin(string username, string LoginProvider, string ProviderKey);
        Task<bool> CheckExternalLoginExists(string username, string ProviderKey);
        Task<bool> CheckUserExists(string username);
        Task<bool> CheckUserExistsLocally(string username);
        bool CheckUserExistsLocallyNonAsync(string username);
        Task DeleteClaim(string username, string type, string value);
        Task DeleteExternalLogin(string username, string ProviderKey);
        Task<bool> DeleteUser(string username, string dataislandId);
        Task<List<UserExternalLogin>> GetExternalLogins(string username);
        Task<UserAccount> GetUserAccount(string username);
        byte[] GetUserAvatar(string username, int size, string type);
        Task<byte[]> GetUserAvatarFromUserId(string userId, int size, string type);
        Task<List<UserClaim>> GetUserClaims(string username);
        Task<UserDetails> GetUserDetails(string username);
        Task<Dictionary<string, dataislandcommon.Models.ViewModels.UserSetting>> GetUserSettings(string username);
        Task<bool> LoginUser(string username, string password);
        Task<UserRegistrationResult> RegisterUser(string id, string username, string password, string email, string securityStamp, string roles, string language, string dataislandID);
        bool SaveUserAvatar(string username, byte[] avatarData);
        Task<bool> SaveUserSettings(string username, Dictionary<string, dataislandcommon.Models.ViewModels.UserSetting> settings);
        Task SetEmail(string username, string email);
        Task SetEmailConfirmed(string username, bool confirmed);
        Task<bool> SetPassword(string username, string newpassword);
        Task<bool> UpdateUser(dataislandcommon.Models.userdb.UserAccount acc);
        Task<bool> VerifyCredentials(string username, string password);
        bool VerifyCredentialsNonAsync(string username, string password);
    }
}
