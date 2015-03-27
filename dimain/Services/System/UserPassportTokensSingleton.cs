using dimain.Models.maindb;
using dimain.Models.ViewModels;
using dimain.Services.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace dimain.Services.System
{
    public class UserPassportTokensSingleton : dimain.Services.System.IUserPassportTokensSingleton
    {
        public ISystemSecurityService SystemSecurity { get; set; }

        public Dictionary<string,DiUserPassportToken> Tokens { get; set; }

        private readonly IMainDatabaseManagerSingleton DbManager;
        private bool IsCleaningProccessRuning = false;
        public UserPassportTokensSingleton(IMainDatabaseManagerSingleton dbmanager)
        {
            this.DbManager = dbmanager;
            Tokens = new Dictionary<string, DiUserPassportToken>();
        }

        public async Task DeleteOverdueTokens()
        {
            if (!IsCleaningProccessRuning)
            {
                IsCleaningProccessRuning = true;
                DateTime timeTreshold = DateTime.UtcNow;
                bool saveChanges = false;
                using (var db = DbManager.GetMainDatabaseConnection())
                {
                    try
                    {
                        for (int i = Tokens.Count - 1; i > -1; i--)
                        {
                            var element = Tokens.ElementAt(i);
                            if (element.Value.ExpirationTime < timeTreshold)
                            {
                                Tokens.Remove(element.Key);
                                await db.Database.ExecuteSqlCommandAsync("DELETE FROM DiUserPassportToken WHERE ID=\"" + element.Key + "\"");
                                saveChanges = true;
                            }
                        }
                    }
                    catch
                    {
                    }

                    if(saveChanges)
                    {
                        await db.SaveChangesAsync();
                    }
                }
                IsCleaningProccessRuning = false;
            }
        }

        public async Task<DiUserPassportToken> IssueNewPassportToken(string passportString)
        {
            UserPassportModel passport = await this.SystemSecurity.ValidateUserPassport(passportString);
            if (passport != null)
            {
                using (var db = this.DbManager.GetMainDatabaseConnection())
                {
                    DiUserPassportToken token = new DiUserPassportToken();
                    token.ExpirationTime = passport.ExpiryDate;
                    token.ID = Guid.NewGuid().ToString();
                    token.UserID = passport.UserId;
                    db.UserPassportTokens.Add(token);
                    await db.SaveChangesAsync();
                    this.Tokens[token.ID] = token;
                    Task.Factory.StartNew(() => this.DeleteOverdueTokens());
                    return token;
                }
            }
            return null;
        }

        public async Task<DiUserPassportToken> GeneratePassportToken(string userId)
        {
            using (var db = this.DbManager.GetMainDatabaseConnection())
            {
                DiUserPassportToken token = new DiUserPassportToken();
                token.ExpirationTime = DateTime.UtcNow.AddMinutes(3);
                token.ID = Guid.NewGuid().ToString();
                token.UserID = userId;
                db.UserPassportTokens.Add(token);
                await db.SaveChangesAsync();
                this.Tokens[token.ID] = token;
                Task.Factory.StartNew(() => this.DeleteOverdueTokens());
                return token;
            }
            return null;
        }

        public string GetUserID(string tokenID)
        {
            if (this.Tokens.ContainsKey(tokenID))
            {
                DiUserPassportToken token = this.Tokens[tokenID];
                if (token.ExpirationTime > DateTime.UtcNow)
                {
                    return token.UserID;
                }
            }
            return String.Empty;
        }

        public async Task InitTokens()
        {
            using(var db = this.DbManager.GetMainDatabaseConnection())
            {
                var res = await db.UserPassportTokens.ToListAsync();
                foreach(var token in res)
                {
                    this.Tokens[token.ID] = token;
                }
            }
        }

        public string GetUserIdFromQueryToken(string query)
        {
            string wQuery = query;
            if (wQuery.StartsWith("?"))
            {
                wQuery = wQuery.Substring(1);
            }
            wQuery = wQuery.Replace("&&", "&");
            string[] parts = wQuery.Split("&".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string token = string.Empty;
            for (int i = 0; i < parts.Length; i++)
            {
                string[] vals = parts[i].Split("=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (vals.Length > 1 && vals[0].ToLower() == "diupt")
                {
                    token = vals[1];
                }
            }
            if (!string.IsNullOrEmpty(token))
            {
                string userId = this.GetUserID(token);
                return userId;
            }

            return string.Empty;
        }

    }
}
