using dimain.Models.oauth;
using dimain.Services.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace dimain.Services.System
{
    public class DiRefreshTokenService : dimain.Services.System.IDiRefreshTokenService
    {
        private IMainDatabaseManagerSingleton DbManager;
        public DiRefreshTokenService(IMainDatabaseManagerSingleton _dbManager)
        {
            DbManager = _dbManager;
        }

        public async Task<bool> AddClient(string clientid, string secret, string name, ApplicationTypes applicationType, bool isActive, int refreshTokenLifeTime, string allowedOrigin )
        {
            using (var _ctx = DbManager.GetMainDatabaseConnection())
            {
                Client client = new Client
                {
                    Id = clientid,
                    Secret = GetHash(secret),
                    ApplicationType = applicationType,
                    Active = isActive,
                    RefreshTokenLifeTime = refreshTokenLifeTime,
                    AllowedOrigin = allowedOrigin,
                    Name = name
                };
                var cl = _ctx.Client.Find(clientid);
                if (cl == null)
                {
                    _ctx.Client.Add(client);
                    await _ctx.SaveChangesAsync();
                    return true;
                }

                return false;
            }

        }

        public Client FindClient(string clientId)
        {
            using (var _ctx = DbManager.GetMainDatabaseConnection())
            {
                var client = _ctx.Client.Find(clientId);

                return client;
            }
        }

        public async Task<bool> AddRefreshToken(RefreshToken token)
        {
            await RemoveRefreshToken(token);
            using (var _ctx = DbManager.GetMainDatabaseConnection())
            {
                _ctx.RefreshToken.Add(token);
                return await _ctx.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> RemoveRefreshToken(string refreshTokenId)
        {
            using (var _ctx = DbManager.GetMainDatabaseConnection())
            {
                var refreshToken = await _ctx.RefreshToken.FindAsync(refreshTokenId);

                if (refreshToken != null)
                {
                    _ctx.RefreshToken.Remove(refreshToken);
                    return await _ctx.SaveChangesAsync() > 0;
                }

                return false;
            }
        }

        public async Task<bool> RemoveRefreshToken(RefreshToken refreshToken)
        {
            try
            {
                using (var _ctx = DbManager.GetMainDatabaseConnection())
                {
                    var tokenToDelete = _ctx.RefreshToken.Where(r => r.Subject == refreshToken.Subject && r.ClientId == refreshToken.ClientId).SingleOrDefault();
                    if (tokenToDelete != null)
                    {
                        _ctx.RefreshToken.Remove(tokenToDelete);
                        return await _ctx.SaveChangesAsync() > 0;
                    }
                }
            }
            catch
            {

            }
            return false;
        }

        public async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
        {
            using (var _ctx = DbManager.GetMainDatabaseConnection())
            {
                var refreshToken = await _ctx.RefreshToken.FindAsync(refreshTokenId);

                return refreshToken;
            }
        }

        public List<RefreshToken> GetAllRefreshTokens()
        {
            using (var _ctx = DbManager.GetMainDatabaseConnection())
            {
                return _ctx.RefreshToken.ToList();
            }
        }

        public string GetHash(string input)
        {
            using (HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider())
            {

                byte[] byteValue = Encoding.UTF8.GetBytes(input);

                byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);

                return Convert.ToBase64String(byteHash);
            }
        }
    }
}
