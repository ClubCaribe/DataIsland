using dimain.Models.ViewModels;
using System;
using System.Threading.Tasks;
namespace dimain.Services.System
{
    public interface ISystemSecurityService
    {
        Task<string> IssueUserPassport(string username, int validFormMinutes);
        Task<UserPassportModel> ValidateUserPassport(string passportString);
    }
}
