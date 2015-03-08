using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace dimain.Services.Communication
{
    public interface IMainDiCommandsService
    {
        Task<bool> DeleteUser(DeleteUserArgs args);
        Task<bool> DeleteUserEmail(string email);
        Task<bool> RegisterDataIsland(RegisterDataiSlandArgs args);
        Task<bool> RegisterUser(scutils.maindicommands.RegisterUserArgs args);
        Task<bool> RegisterUserEmail(scutils.maindicommands.AddUserEmailArgs args);
        Task<bool> SetDataIslandData(SetDataIslandDataArgs dat);
        Task<bool> SetDataIslandIp(string dataislandid);
        Task<bool> CheckUserExists(string username);
        Task<bool> ExtendProAccount(string username, int months, string key);
        Task<string> GetUserDataIslandID(string username);
        Task<string> GetUserDataIslandIDFromUserId(string userId);
        Task<string> GetUserDataIslandPublicKey(string username);
        Task<string> GetUserDataIslandPublicKeyFromUserId(string userId);
        Task<DateTime> GetUserProAccountExpirationDate(string username);
        Task<string> GetUserPublicKey(string username);
        Task<string> GetUserPublicKeyFromUserId(string userId);
        Task<string> GetDataIslandIp(string dataislandid);
        Task<string> GetUserID(string username);
        Task<List<string>> FindUsers(string searchPhrase);
    }

    public class RegisterDataiSlandArgs
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string PublicKey { get; set; }
        public string WebPage { get; set; }
        public bool IsPublic { get; set; }
        public string Description { get; set; }
        public string Domain { get; set; }
        public string DataIslandID { get; set; }
    }

    public class SetDataIslandDomainArgs
    {
        public string ID { get; set; }
        public string Domain { get; set; }
        public bool IsPublic { get; set; }
    }

    public class SetDataIslandDataArgs
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Webpage { get; set; }
    }

    public class DeleteUserArgs
    {
        public string DataIslandID { get; set; }
        public string Username { get; set; }
    }

    public class UserPublicDataArgs
    {
        public string Username { get; set; }
        public string Name { get; set; }
        public string DataislandID { get; set; }
        public string PublicKey { get; set; }
        public string UserId { get; set; }
    }
}
