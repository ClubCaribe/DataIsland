using dimain.Models.oauth;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace dimain.Services.System
{
    public interface IDiRefreshTokenService
    {
        Task<bool> AddClient(string clientid, string secret, string name, ApplicationTypes applicationType, bool isActive, int refreshTokenLifeTime, string allowedOrigin);
        Task<bool> AddRefreshToken(RefreshToken token);
        Client FindClient(string clientId);
        Task<RefreshToken> FindRefreshToken(string refreshTokenId);
        List<RefreshToken> GetAllRefreshTokens();
        Task<bool> RemoveRefreshToken(RefreshToken refreshToken);
        Task<bool> RemoveRefreshToken(string refreshTokenId);
        string GetHash(string input);
    }
}
