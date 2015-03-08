using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using dimain.Models.oauth;
namespace dimain.Services.Session
{
    public interface IOAuthDataService
    {
        Task<dimain.Models.oauth.OAuthClient> GetClient(string clientIdentifier);
        OAuthClient GetClientNonAsync(string clientIdentifier);
        Task<dimain.Models.oauth.OAuthSymmetricCryptoKey> GetKey(string bucket, string handle);
        OAuthSymmetricCryptoKey GetKeyNonAsync(string bucket, string handle);
        Task<List<dimain.Models.oauth.OAuthSymmetricCryptoKey>> GetKeys(string bucket);
        List<OAuthSymmetricCryptoKey> GetKeysNonAsync(string bucket);
        Task RemoveKey(string bucket, string handle);
        void RemoveKeyNonAsync(string bucket, string handle);
        void StoreKey(dimain.Models.oauth.OAuthSymmetricCryptoKey key);
        void StoreKeyNonAsync(OAuthSymmetricCryptoKey key);
        bool StoreNonce(string context, string nonce, DateTime timestampUtc);
        bool StoreNonceNonAsync(string context, string nonce, DateTime timestampUtc);
        Task<List<OAuthClientAuthorization>> GetClientAuthorization(DateTime issuedTime, string clientIdentifier, string username);
        List<OAuthClientAuthorization> GetClientAuthorizationNonAsync(DateTime issuedTime, string clientIdentifier, string username);
    }
}
