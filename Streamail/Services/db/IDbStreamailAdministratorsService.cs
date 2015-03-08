using Streamail.Classes.db;
using Streamail.Models.db;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Streamail.Services.db
{
    public interface IDbStreamailAdministratorsService
    {
        Task<bool> AddAdministrator(string streamailId, string participantId, string participantName, StreamailAdministrationRole role, string ownerUsername);
        Task<bool> AddAdministrators(List<StreamailAdministrator> administrators, string streamailId, string ownerUsername);
        Task<bool> AddAdministrators(List<StreamailAdministrator> administrators, string streamailId, DiStreamailContext db);
        Task<bool> DeleteAdministrator(string streamailId, string adminId, string ownerUsername);
        Task<bool> DeleteAdministrators(string streamailId, string ownerUsername);
        Task<List<StreamailAdministrator>> GetAdministrators(string streamailId, string ownerUsername);
        Task<bool> SetAdministratorRole(string streamailId, string adminid, StreamailAdministrationRole role, string ownerUsername);
    }
}
