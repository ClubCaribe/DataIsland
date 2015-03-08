using dimain.Models.maindb;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace dimain.Services.System
{
    public interface IDiUserService
    {
        Task<List<DiUser>> GetUsers();
        Task<List<DiUser>> GetUsers(int page, int numOfElements, string searchPhrase);
        Task<int> GetUsersCount(string searchPhrase);
        Task<bool> CheckUserExists(string username);
        Task<bool> CheckUserIdExists(string userId);
        Task<bool> AddUser(string username, string id);
        Task<int> UpdateUsersIds();
        Task<DiUser> GetUserById(string id);
        Task<DiUser> GetUserByUserId(string userId);
        Task<DiUser> GetUserByUsername(string username);
        Task<string> GetUserIdByFromUsername(string username);
        Task<string> GetUsernameFromUserId(string userId);
        Task<bool> DeleteUser(string username);
        Task<bool> ChangeUserName(string username, string name);
        Task<bool> SetProAccountExpirationDate(string username, DateTime proAccountExpirationDate);
        Task<bool> AddExternalLogin(string username, string ProviderKey);
        Task<bool> CheckExternalLoginExists(string providerKey);
        Task<string> GetExternalLoginUsername(string providerKey);
        Task DeleteExternalLogin(string providerKey);
        Task SetEmail(string username, string email);
        Task<string> GetUsenameByEmail(string email);
        Task<DiUserData> GetDIUserDataFromUserId(string userId);
    }
}
