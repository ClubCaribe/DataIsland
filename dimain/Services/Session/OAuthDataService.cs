using dimain.Services.db;
using dimain.Models.oauth;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dimain.Services.Session
{
    public class OAuthDataService : IOAuthDataService
    {
        private readonly IMainDatabaseManagerSingleton _dbManager;
        public OAuthDataService(IMainDatabaseManagerSingleton dbManager)
        {
            _dbManager = dbManager;
        }

        #region INonceStore
        public bool StoreNonce(string context, string nonce, DateTime timestampUtc)
        {
            try
            {
                OAuthNonce oauthNonce = new OAuthNonce { Code = nonce, Context = context, Timestamp = timestampUtc };
                var db = _dbManager.GetMainDatabaseConnection();
				db.OAuthNonce.Add(oauthNonce);
				db.SaveChanges();
                return true;
            }
            catch
            {
            }
            return false;
        }

        public bool StoreNonceNonAsync(string context, string nonce, DateTime timestampUtc)
        {
            try
            {
                OAuthNonce oauthNonce = new OAuthNonce { Code = nonce, Context = context, Timestamp = timestampUtc };
                var db = _dbManager.GetMainDatabaseConnectionNonAsync();
				db.OAuthNonce.Add(oauthNonce);
				db.SaveChanges();
                return true;
            }
            catch
            {
            }
            return false;
        }
        #endregion


        #region ICryptoKeyStore
        public async Task<OAuthSymmetricCryptoKey> GetKey(string bucket, string handle)
        {
            var db = _dbManager.GetMainDatabaseConnection();

			var lst = await db.OAuthSymmetricCryptoKey.Where(t => t.Bucket == bucket && t.Handle == handle).ToListAsync();
            if (lst != null && lst.Count > 0)
            {
                lst[0].Secret = Convert.FromBase64String(lst[0].SecretBase64);
                return lst[0];
            }
            return null;
        }

        public  OAuthSymmetricCryptoKey GetKeyNonAsync(string bucket, string handle)
        {
            var db = _dbManager.GetMainDatabaseConnectionNonAsync();
            var lst = db.OAuthSymmetricCryptoKey.Where(t => t.Bucket == bucket && t.Handle == handle).ToList();
            if (lst != null && lst.Count > 0)
            {
                lst[0].Secret = Convert.FromBase64String(lst[0].SecretBase64);
                return lst[0];
            }
            return null;
        }

        public async Task<List<OAuthSymmetricCryptoKey>> GetKeys(string bucket)
        {
            var db = _dbManager.GetMainDatabaseConnection();
            var list = await db.OAuthSymmetricCryptoKey.Where(t => t.Bucket == bucket).ToListAsync();
            if (list != null && list.Count > 0)
            {
                foreach (var key in list)
                {
                    key.Secret = Convert.FromBase64String(key.SecretBase64);
                }
            }
            return list;
        }

        public List<OAuthSymmetricCryptoKey> GetKeysNonAsync(string bucket)
        {
            var db = _dbManager.GetMainDatabaseConnectionNonAsync();
            var list = db.OAuthSymmetricCryptoKey.Where(t => t.Bucket == bucket).ToList();
            if (list != null && list.Count > 0)
            {
                foreach (var key in list)
                {
                    key.Secret = Convert.FromBase64String(key.SecretBase64);
                }
            }
            return list;
        }

        public async Task RemoveKey(string bucket, string handle)
        {
            var db = _dbManager.GetMainDatabaseConnection();
            await db.Database.ExecuteSqlCommandAsync(String.Format("DELETE FROM OAuthSymmetricCryptoKey WHERE Bucket={0} AND Handle = {1}", bucket, handle),null);
        }

        public void RemoveKeyNonAsync(string bucket, string handle)
        {
            var db = _dbManager.GetMainDatabaseConnectionNonAsync();
            db.Database.ExecuteSqlCommand(String.Format("DELETE FROM OAuthSymmetricCryptoKey WHERE Bucket={0} AND Handle = {1}", bucket, handle), null);
        }

        public void StoreKey(OAuthSymmetricCryptoKey key)
        {
            var db = _dbManager.GetMainDatabaseConnection();
            key.SecretBase64 = Convert.ToBase64String(key.Secret);
			db.OAuthSymmetricCryptoKey.Add(key);
			db.SaveChanges();
        }

        public void StoreKeyNonAsync(OAuthSymmetricCryptoKey key)
        {
            var db = _dbManager.GetMainDatabaseConnectionNonAsync();
            key.SecretBase64 = Convert.ToBase64String(key.Secret);
			db.OAuthSymmetricCryptoKey.Add(key);
			db.SaveChanges();
        }
        #endregion

        public async Task<OAuthClient> GetClient(string clientIdentifier)
        {
            var db = _dbManager.GetMainDatabaseConnection();
            var list = await db.OAuthClient.Where(t => t.ClientIdentifier == clientIdentifier).ToListAsync();
            if (list != null && list.Count > 0)
            {
                return list[0];
            }
            return null;
        }

        public OAuthClient GetClientNonAsync(string clientIdentifier)
        {
            var db = _dbManager.GetMainDatabaseConnectionNonAsync();
            var list = db.OAuthClient.Where(t => t.ClientIdentifier == clientIdentifier).ToList();
            if (list != null && list.Count > 0)
            {
                return list[0];
            }
            return null;
        }

        //public async Task<string> StoreClient

        public async Task<List<OAuthClientAuthorization>> GetClientAuthorization(DateTime issuedTime, string clientIdentifier, string username)
        {
            var db = _dbManager.GetMainDatabaseConnection();
            OAuthClient client = await GetClient(clientIdentifier);
            if (client.Name == username)
            {
                if (client != null)
                {
                    var list = await db.OAuthClientAuthorization.Where(t => t.ClientId == client.ClientId && t.CreatedOnUtc <= issuedTime && t.ExpirationDateUtc >= DateTime.Now).ToListAsync();
                    if (list != null && list.Count > 0)
                    {
                        return list;
                    }
                }
            }
            return null;
        }

        public List<OAuthClientAuthorization> GetClientAuthorizationNonAsync(DateTime issuedTime, string clientIdentifier, string username)
        {
            var db = _dbManager.GetMainDatabaseConnectionNonAsync();
            OAuthClient client = GetClientNonAsync(clientIdentifier);
            if (client.Name == username)
            {
                if (client != null)
                {
                    var list = db.OAuthClientAuthorization.Where(t => t.ClientId == client.ClientId && t.CreatedOnUtc <= issuedTime && t.ExpirationDateUtc >= DateTime.Now).ToList();
                    if (list != null && list.Count > 0)
                    {
                        return list;
                    }
                }
            }
            return null;
        }
    }
}
