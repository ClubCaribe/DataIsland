using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity; 

using dataislandcommon.Models.ViewModels;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using dataislandcommon.Models.userdb;
using dataislandcommon.Services.Utilities;
using dataislandcommon.Services.db;
using dataislandcommon.Services.FileSystem;
using dimain.Services.Communication;
using dataislandcommon.Utilities.enums;
using dataislandcommon.Utilities;
using dimain.Services.System;


namespace dataislandcommon.Services.System
{
    public class UserService : dataislandcommon.Services.System.IUserService
    {

        private readonly IUserDatabaseManagerSingleton _dbManager;

        
        public ICryptographySingleton Cryptography { get; set; }

        
        public IMainDiCommandsService MainDiCommands { get; set; }

        
        public IFilePathProviderService FilePathProvider { get; set; }

        
        public IImageUtilitiesSingleton ImageUtilities { get; set; }

        
        public IDiUserService DiUserService { get; set; }

        public UserService(IUserDatabaseManagerSingleton dbManager)
        {
            _dbManager = dbManager;
        }

        public async Task<bool> CheckUserExists(string username)
        {
            if (string.IsNullOrEmpty(username) || username.Length < 3)
            {
                return true;
            }

            if ((username.ToLower() == "admin") || (username.ToLower() == "su"))
            {
                return true;
            }
            if (this.FilePathProvider.CheckUserDirectoryExists(username))
            {
                return true;

            }
            else
            {
                var res = await MainDiCommands.CheckUserExists(username);
                return res;
            }
        }

        public async Task<bool> CheckUserExistsLocally(string username)
        {
            if (this.FilePathProvider.CheckUserDirectoryExists(username))
            {
                return true;
            }

            return false;
        }

        public bool CheckUserExistsLocallyNonAsync(string username)
        {
            if (this.FilePathProvider.CheckUserDirectoryExists(username))
            {
                return true;
            }

            return false;
        }

        public async Task<bool> LoginUser(string username, string password)
        {
            if (this.CheckUserExistsLocallyNonAsync(username))
            {
                _dbManager.UpdateDatabase(username);
                using (var db = _dbManager.GetUserContext(username))
                {
                    var users = await db.UserAccount.Where(x => x.UserName == username).ToListAsync();
                    if (users != null && users.Count > 0)
                    {
                        dataislandcommon.Models.userdb.UserAccount user = users[0];
                        if (user.Password == Cryptography.GetSha1AsBase64String(password))
                        {
                            user.LastLoginTime = DateTime.Now;
                            await db.SaveChangesAsync();
                            user.Password = password;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public async Task<bool> VerifyCredentials(string username, string password)
        {
            try
            {
                using (var db = _dbManager.GetUserContext(username))
                {
                    var users = await db.UserAccount.Where(x => x.UserName == username).ToListAsync();
                    if (users != null && users.Count > 0)
                    {
                        dataislandcommon.Models.userdb.UserAccount user = users[0];
                        if (user.Password == Cryptography.GetSha1AsBase64String(password))
                        {
                            return true;
                        }
                    }
                }
            }
            catch
            {
            }
            return false;
        }

        public bool VerifyCredentialsNonAsync(string username, string password)
        {
            try
            {
                using (var db = _dbManager.GetUserContext(username))
                {
                    var users = db.UserAccount.Where(x => x.UserName == username).ToList();
                    if (users != null && users.Count > 0)
                    {
                        UserAccount user = users[0];
                        if (user.Password == Cryptography.GetSha1AsBase64String(password))
                        {
                            return true;
                        }
                    }
                }
            }
            catch
            {
            }
            return false;
        }

        public async Task<UserRegistrationResult> RegisterUser(string id, string username, string password, string email, string securityStamp, string roles, string language, string dataislandID)
        {
            if (!await CheckUserExists(username))
            {
                string passwordhash = password;
                List<string> rsaKeys = Cryptography.GenerateRsaKeys(passwordhash);
                UserAccount acc = new UserAccount();
                acc.Id = id;
                acc.Email = email;
                acc.LanguageDirection = "ltr";
                acc.LastLoginTime = DateTime.Now;
                acc.Name = username;
                acc.Password = passwordhash;
                acc.PrivateKey = rsaKeys[1];
                acc.ProAccountExpirationTime = DateTime.Now;
                acc.PublicKey = rsaKeys[0];
                acc.RegisterDate = DateTime.Now;
                acc.Roles = roles;
                acc.UiLanguage = language;
                acc.UserName = username;
                acc.EmailConfirmed = false;
                acc.SecurityStamp = securityStamp;

                scutils.maindicommands.RegisterUserArgs registeruserargs = new scutils.maindicommands.RegisterUserArgs();
                registeruserargs.DataIslandID = dataislandID;
                registeruserargs.Emailaddress = email;
                registeruserargs.Name = username;
                registeruserargs.PublicKey = acc.PublicKey;
                registeruserargs.Username = username;

                if (await MainDiCommands.RegisterUser(registeruserargs))
                {
                    _dbManager.UpdateDatabase(username);
                    using (var db = _dbManager.GetUserContext(username))
                    {
                        db.UserAccount.Add(acc);
                        int count = await db.SaveChangesAsync();
                        if (count > 0)
                        {
                            return UserRegistrationResult.Success;
                        }
                        else
                        {
                            return UserRegistrationResult.DatabaseError;
                        }
                    }
                }
                else
                {
                    return UserRegistrationResult.MainServerError;
                }
            }
            return UserRegistrationResult.UserExists;
        }

		public async Task<UserAccount> GetUserAccount(string username)
		{
            if (await CheckUserExistsLocally(username))
            {
                using (var db = _dbManager.GetUserContext(username))
                {
                    var res = await db.UserAccount.Where(t => t.UserName == username).SingleOrDefaultAsync();
                    if (res != null)
                    {
                        return res;
                    }
                }
            }
			return null;
		}

		public async Task<UserDetails> GetUserDetails(string username)
		{
            if (await CheckUserExistsLocally(username))
            {
                UserDetails userDetails = new UserDetails();
                UserAccount account = await GetUserAccount(username);
                if (account != null)
                {
                    userDetails.LastLoginTime = account.LastLoginTime;
                    userDetails.Name = ((String.IsNullOrEmpty(account.Name)) ? username : account.Name);
                    userDetails.ProAccountExpirationTime = account.ProAccountExpirationTime.Date;
                    userDetails.Username = username;
                    userDetails.Email = account.Email;

                }

                return userDetails;
            }
            return null;
		}

        public async Task<Dictionary<string,dataislandcommon.Models.ViewModels.UserSetting>> GetUserSettings(string username)
        {
                Dictionary<string, dataislandcommon.Models.ViewModels.UserSetting> settings = new Dictionary<string, Models.ViewModels.UserSetting>();
                try
                {
                    if (await CheckUserExistsLocally(username))
                    {
                        using (var db = _dbManager.GetUserContext(username))
                        {
                            var res = await db.UserSettings.ToListAsync();
                            foreach (string setting in DiConsts.UserAccountPersonalSettings)
                            {
                                bool dbRecordExist = false;
                                if (res.Count > 0)
                                {
                                    foreach (var dbsetting in res)
                                    {
                                        if (dbsetting.Name == setting)
                                        {
                                            dataislandcommon.Models.ViewModels.UserSetting stt = new Models.ViewModels.UserSetting();
                                            stt.Name = dbsetting.Name;
                                            stt.Value = dbsetting.Value;
                                            stt.IsPublic = dbsetting.Visible;
                                            settings[setting] = stt;
                                            dbRecordExist = true;
                                        }
                                    }
                                }
                                if (!dbRecordExist)
                                {
                                    dataislandcommon.Models.ViewModels.UserSetting stt = new Models.ViewModels.UserSetting();
                                    stt.Name = setting;
                                    stt.Value = "";
                                    stt.IsPublic = true;
                                    stt.Category = "General";
                                    settings[setting] = stt;
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
                return settings;
            
        }

        public async Task<bool> SaveUserSettings(string username, Dictionary<string,dataislandcommon.Models.ViewModels.UserSetting> settings)
        {
            if (await CheckUserExistsLocally(username))
            {
                using (var db = _dbManager.GetUserContext(username))
                {
                    var userdetails = db.UserAccount.Where(x => x.UserName == username).SingleOrDefault();
                    foreach (var stt in settings)
                    {

                        if (stt.Value.Name == "Name")
                        {
                            userdetails.Name = stt.Value.Value;
                        }
                        else if (stt.Value.Name == "Email")
                        {
                            userdetails.Email = stt.Value.Value;
                        }
                        else
                        {
                            var res = await db.UserSettings.Where(t => t.Name == stt.Value.Name).SingleOrDefaultAsync();
                            if (res != null)
                            {
                                res.Value = stt.Value.Value;
                                res.Visible = stt.Value.IsPublic;
                            }
                            else
                            {
                                dataislandcommon.Models.userdb.UserSetting setting = new Models.userdb.UserSetting();
                                setting.Category = stt.Value.Category;
                                setting.Name = stt.Value.Name;
                                setting.Value = stt.Value.Value;
                                setting.Visible = stt.Value.IsPublic;
                                db.UserSettings.Add(setting);
                            }
                        }
                    }
                    int rowcount = await db.SaveChangesAsync();
                    if (rowcount > -1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public byte[] GetUserAvatar(string username, int size, string type)
        {
            
                string avatarPath = FilePathProvider.GetUserDataPath(username) + "avatar.png";
                if (File.Exists(avatarPath))
                {
                    using (Image img = Image.FromFile(avatarPath))
                    {
                        using (Bitmap resizedImage = ImageUtilities.TransformPicture((Bitmap)img, size, size, type))
                        {
                            if (resizedImage != null)
                            {
                                byte[] outputData = ImageUtilities.TransformImageToByte(resizedImage, "png");
                                if (outputData != null)
                                {
                                    return outputData;
                                }
                            }
                        }
                    }
                }
            return null;
        }

        public async Task<byte[]> GetUserAvatarFromUserId(string userId, int size, string type)
        {
            var userData = await this.DiUserService.GetUserByUserId(userId);
            if (userData != null)
            {
                string avatarPath = FilePathProvider.GetUserDataPath(userData.Username) + "avatar.png";
                if (File.Exists(avatarPath))
                {
                    using (Image img = Image.FromFile(avatarPath))
                    {
                        using (Bitmap resizedImage = ImageUtilities.TransformPicture((Bitmap)img, size, size, type))
                        {
                            if (resizedImage != null)
                            {
                                byte[] outputData = ImageUtilities.TransformImageToByte(resizedImage, "png");
                                if (outputData != null)
                                {
                                    return outputData;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        public bool SaveUserAvatar(string username, byte[] avatarData)
        {
            using (MemoryStream ms = new MemoryStream(avatarData))
            {
                using (Image img = Image.FromStream(ms))
                {
                    using (Bitmap resizedImage = ImageUtilities.ResizePictureIfLarger((Bitmap)img, 1200, 1200,Color.Transparent))
                    {
                        if (resizedImage != null)
                        {
                            byte[] outputData = ImageUtilities.TransformImageToByte(resizedImage, "png");
                            if (outputData != null)
                            {
                                if (Directory.Exists(FilePathProvider.GetUserDataPath(username)))
                                {
                                    string avatarPath = FilePathProvider.GetUserDataPath(username) + "avatar.png";
                                    File.WriteAllBytes(avatarPath, outputData);
                                    
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        public async Task<bool> SetPassword(string username, string newpassword)
        {
            if (await CheckUserExistsLocally(username))
            {
                using (var db = _dbManager.GetUserContext(username))
                {
                    var res = await db.UserAccount.Where(t => t.UserName == username).SingleOrDefaultAsync();
                    if (res != null)
                    {
                        string passwordhash = Cryptography.GetSha1AsBase64String("password");
                        if (passwordhash != res.Password)
                        {
                            passwordhash = res.Password;
                        }
                        res.PrivateKey = Cryptography.ChangePrivateKeyPassword(res.PrivateKey, passwordhash, newpassword);
                        res.Password = newpassword;
                        await db.SaveChangesAsync();
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> DeleteUser(string username, string dataislandId)
        {
            try
            {
                if (await CheckUserExistsLocally(username))
                {
                    DeleteUserArgs delArgs = new DeleteUserArgs();
                    delArgs.DataIslandID = dataislandId;
                    delArgs.Username = username;
                    await MainDiCommands.DeleteUser(delArgs);
                    try
                    {
                        Directory.Delete(FilePathProvider.GetUserPath(username), true);
                    }
                    catch
                    {

                    }
                    return true;
                }
            }
            catch
            {
            }

            return false;
        }

        public async Task<bool> UpdateUser(UserAccount acc)
        {
            if (await CheckUserExistsLocally(acc.UserName))
            {
                using (var db = _dbManager.GetUserContext(acc.UserName))
                {
                    var original = await db.UserAccount.FindAsync(acc.Id);

                    if (original != null)
                    {
                        db.Entry(original).CurrentValues.SetValues(acc);
                        await db.SaveChangesAsync();
                        return true;
                    }
                }
            }
            return false;
        }

        public async Task<bool> AddExternalLogin(string username, string LoginProvider, string ProviderKey)
        {
            if (await CheckUserExistsLocally(username))
            {
                if (!await this.CheckExternalLoginExists(username, ProviderKey))
                {
                    using (var db = _dbManager.GetUserContext(username))
                    {
                        UserExternalLogin exlogin = new UserExternalLogin();
                        exlogin.LoginProvider = LoginProvider;
                        exlogin.ProviderKey = ProviderKey;
                        db.ExternalLogins.Add(exlogin);
                        await db.SaveChangesAsync();

                        return true;
                    }
                }
            }
            return false;
        }

        public async Task<bool> CheckExternalLoginExists(string username, string ProviderKey)
        {
            if (await CheckUserExistsLocally(username))
            {
                using (var db = _dbManager.GetUserContext(username))
                {
                    var res = await db.ExternalLogins.Where(x => x.ProviderKey == ProviderKey).ToListAsync();
                    if (res != null && res.Count > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public async Task<List<UserExternalLogin>> GetExternalLogins(string username)
        {
            if (await CheckUserExistsLocally(username))
            {
                using (var db = _dbManager.GetUserContext(username))
                {
                    var res = await db.ExternalLogins.ToListAsync();
                    return res;
                }
            }
            return null;
        }

        public async Task DeleteExternalLogin(string username, string ProviderKey)
        {
            if (await CheckUserExistsLocally(username))
            {
                using (var db = _dbManager.GetUserContext(username))
                {
                    var lst = await db.ExternalLogins.Where(x => x.ProviderKey == ProviderKey).ToListAsync();
                    db.ExternalLogins.RemoveRange(lst);
                    await db.SaveChangesAsync();
                }
            }
        }

        public async Task AddClaim(string username, string type, string value)
        {
            if (await CheckUserExistsLocally(username))
            {
                using (var db = _dbManager.GetUserContext(username))
                {
                    var existingClaim = await db.UserClaims.Where(x => x.Type == type).ToListAsync();
                    if (existingClaim != null && existingClaim.Count > 0)
                    {
                        existingClaim[0].Value = value;
                    }
                    else
                    {
                        UserClaim cl = new UserClaim();
                        cl.Id = Guid.NewGuid().ToString();
                        cl.Type = type;
                        cl.Value = value;
                        db.UserClaims.Add(cl);
                    }
                    await db.SaveChangesAsync();
                }
            }
        }

        public async Task<List<UserClaim>> GetUserClaims(string username)
        {
            if (await CheckUserExistsLocally(username))
            {
                using (var db = _dbManager.GetUserContext(username))
                {
                    var res = await db.UserClaims.ToListAsync();
                    return res;
                }
            }
            return null;
        }

        public async Task DeleteClaim(string username, string type, string value)
        {
            if (await CheckUserExistsLocally(username))
            {
                using (var db = _dbManager.GetUserContext(username))
                {
                    var lst = await db.UserClaims.Where(x => x.Type == type && x.Value == value).ToListAsync();
                    db.UserClaims.RemoveRange(lst);
                    await db.SaveChangesAsync();
                }
            }
        }

        public async Task SetEmail(string username, string email)
        {
            if (await CheckUserExistsLocally(username))
            {
                using (var db = _dbManager.GetUserContext(username))
                {
                    var user = await db.UserAccount.Where(x => x.UserName == username).SingleAsync();
                    if (user != null)
                    {
                        user.Email = email;
                        await db.SaveChangesAsync();
                    }
                }
            }
        }

        public async Task SetEmailConfirmed(string username, bool confirmed)
        {
            if (await CheckUserExistsLocally(username))
            {
                using (var db = _dbManager.GetUserContext(username))
                {
                    var user = await db.UserAccount.Where(x => x.UserName == username).SingleAsync();
                    if (user != null)
                    {
                        user.EmailConfirmed = confirmed;
                        await db.SaveChangesAsync();
                    }
                }
            }
        }

    }
}
