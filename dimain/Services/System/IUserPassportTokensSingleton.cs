using dimain.Models.maindb;
using System;
using System.Threading.Tasks;
namespace dimain.Services.System
{
    public interface IUserPassportTokensSingleton
    {
        string GetUserID(string tokenID);
        Task InitTokens();
        Task<DiUserPassportToken> IssueNewPassportToken(string passportString);
        Task<DiUserPassportToken> GeneratePassportToken(string userId);
        string GetUserIdFromQueryToken(string query);
    }
}
