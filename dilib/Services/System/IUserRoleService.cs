using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dimain.Services.System
{
    public interface IUserRoleService
    {
        Task<bool> AddUserRole(string username, string role);
        Task<bool> AddUserRoles(string username, List<string> roles);
        Task<bool> CheckUserRoleExist(string username, string role);
        Task<bool> DeleteUserRole(string username, string role);
        Task<bool> DeleteUserRoles(string username, IEnumerable<string> roles);
        Task<List<string>> GetUserRoles(string username);
        Task<bool> DeleteUserRoles(string username);
    }
}
