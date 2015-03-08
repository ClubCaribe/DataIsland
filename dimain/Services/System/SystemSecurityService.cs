using dataislandcommon.Services.Utilities;
using dataislandcommon.Utilities;
using dimain.Models.maindb;
using dimain.Models.ViewModels;
using dimain.Services.Communication;
using dimain.Services.db;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace dimain.Services.System
{
    public class SystemSecurityService : ISystemSecurityService
    {

        
        public IDataIslandSettingsService DiSettings { get; set; }

        
        public ICryptographySingleton Cryptography { get; set; }

        
        public IMainDiCommandsService MainDiCommands { get; set; }

        
        public IDiUserService UserService { get; set; }

        
        public IDataIslandService DataIslandService { get; set; }

        private IMainDatabaseManagerSingleton DbManager;

        public SystemSecurityService(IMainDatabaseManagerSingleton _dbManager)
        {
            DbManager = _dbManager;
        }

        public async Task<string> IssueUserPassport(string username, int validFormMinutes)
        {
            string passportString = "";
            object oDataIslandPrivateKey = await DiSettings.GetSetting(DiConsts.PrivateKey);
            if (oDataIslandPrivateKey != null)
            {
                object diPassword = await DiSettings.GetSetting(DiConsts.DataIslandPassword);
                if (diPassword != null)
                {
                    UserPassportModel passport = new UserPassportModel();
                    passport.UserId = Cryptography.GetSha1AsBase64String(username.ToLower());
                    passport.ExpiryDate = DateTime.UtcNow.AddMinutes(validFormMinutes);
                    passport.Signature = Cryptography.SignMessage(passport.UserId + passport.ExpiryDate, (string)oDataIslandPrivateKey, (string)diPassword);
                    passportString = JsonConvert.SerializeObject(passport);
                    passportString = Convert.ToBase64String(Encoding.UTF8.GetBytes(passportString));
                }
            }

            return passportString;
        }

        public async Task<UserPassportModel> ValidateUserPassport(string passportString)
        {
            string passportHashId = Cryptography.GetSha1AsBase64String(passportString);
            using (var db = DbManager.GetMainDatabaseConnection())
            {
                var existingPassport = await db.DiUserPassport.Where(x => x.Id == passportHashId).ToListAsync();
                if (existingPassport.Count > 0)
                {
                    if (existingPassport[0].ExpireDate > DateTime.UtcNow)
                    {
                        return (UserPassportModel)JsonConvert.DeserializeObject(Encoding.UTF8.GetString(Convert.FromBase64String(passportString)));
                    }
                    else
                    {
                        db.DiUserPassport.RemoveRange(existingPassport);
                    }
                }

                UserPassportModel userPassport = (UserPassportModel)JsonConvert.DeserializeObject(Encoding.UTF8.GetString(Convert.FromBase64String(passportString)));
                DiUserData userData = await this.UserService.GetDIUserDataFromUserId(userPassport.UserId);
                if (userData != null)
                {
                    string userDataIslandPublicKey = await this.DataIslandService.GetDataIslandPublicKey(userData.DatIslandId);

                    if (!string.IsNullOrEmpty(userDataIslandPublicKey))
                    {
                        if (Cryptography.VerifySignature(Encoding.UTF8.GetBytes(userPassport.UserId + userPassport.ExpiryDate), Convert.FromBase64String(userPassport.Signature), userDataIslandPublicKey))
                        {
                            DiUserPassport diPassport = new DiUserPassport();
                            diPassport.ExpireDate = userPassport.ExpiryDate;
                            diPassport.Id = passportHashId;
                            diPassport.PassportData = passportString;
                            db.DiUserPassport.Add(diPassport);
                            await db.SaveChangesAsync();
                            return userPassport;
                        }
                    }
                }
            }


            return null;
        }
    }
}
