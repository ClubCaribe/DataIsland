using System;
namespace dimain.Services.System.Cache
{
    public interface IUserDataMemoryCacheService
    {
        bool CheckUserExists(string username);
        bool CheckUserExistsByUserId(string userId);
        string GetUserIdByUsername(string username);
        string GetUsernameFromUserId(string userId);
        bool CheckUserExistsLocallyByUserId(string userId);
        void SetUserData(dimain.Models.ViewModels.UserDataCache data);
        dimain.Models.ViewModels.UserDataCache this[string userid] { get; set; }
    }
}
