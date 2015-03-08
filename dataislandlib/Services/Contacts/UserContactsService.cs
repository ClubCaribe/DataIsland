using dataislandcommon.Models.userdb;
using dataislandcommon.Models.ViewModels.Contacts;
using dataislandcommon.Services.db;
using dataislandcommon.Services.Utilities;
using dimain.Services.Communication;

using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Linq;
using dilib.Services.Communication;
using dimain.Services.System;
using Microsoft.AspNet.SignalR;
using dataislandcommon.Classes.Communication;
using dataislandcommon.Services.FileSystem;
using System.IO;
using System.Drawing;
using dataislandcommon.Services.Contacts;

namespace dataislandcommon.Services.System
{
    public class UserContactsService : dataislandcommon.Services.System.IUserContactsService
    {
        private readonly IUserDatabaseManagerSingleton _dbManager;

        
        public IMainDiCommandsService MainCommands { get; set; }

        
        public dimain.Services.System.IDataIslandService DiUtils { get; set; }

        
        public IUtilitiesSingleton Utilities { get; set; }

        
        public IDICommandsService Commands { get; set; }

        
        public IDiUserService DiUsers { get; set; }

        
        public IUserService User { get; set; }

        
        public IDataIslandService DataIslandService { get; set; }

        
        public IFilePathProviderService FilePathProvider { get; set; }

        
        public IFileSystemUtilitiesService FileSystemUtilities { get; set; }

        
        public IImageUtilitiesSingleton ImageUtilities { get; set; }

        public IContactsNotificationsService Notifications { get; set; }

        public UserContactsService(IUserDatabaseManagerSingleton dbManager)
        {
            _dbManager = dbManager;
        }

        public async Task<List<UserExternalContact>> FindExternalUsers(string searchPhrase, string username)
        {
            List<UserExternalContact> results = new List<UserExternalContact>();
            if (searchPhrase.Length > 2)
            {
                List<string> searchResults = await this.MainCommands.FindUsers(searchPhrase);
                if ((searchResults != null) && (searchResults.Count > 0))
                {
                    List<string> contactsUsernames = await this.GetContactsUsernames(username);
                    for (int i = 0; i < searchResults.Count; i++)
                    {

                        UserExternalContact cnt = new UserExternalContact();
                        cnt.Username = searchResults[i];
                        cnt.UserId = searchResults[i + 1];
                        if (contactsUsernames.BinarySearch(cnt.Username) < 0 && cnt.Username != username)
                        {
                            cnt.AvatarPath = await this.DiUtils.GetDataislandUrl(searchResults[i + 2]) + "utilities/useridavatar/" + this.Utilities.EscapeUserId(cnt.UserId) + "/44/sqr";
                            cnt.DataislandId = searchResults[i + 2];
                            results.Add(cnt);
                        }
                        i = i + 2;
                    }
                }
            }
            return results;
        }

        public async Task<List<string>> GetContactsUsernames(string username)
        {
            List<string> usernames = new List<string>();
            using(var db = this._dbManager.GetUserContext(username))
            {
                try
                {
                    var res = await db.UserContacts.ToListAsync();
                    foreach (UserContact user in res)
                    {
                        usernames.Add(user.Username);
                    }
                }
                catch
                {

                }
            }
            usernames.Sort();
            return usernames;
        }

        public async Task<bool> CheckContactExists(string userId,string ownerUsername)
        {
            using(var db = this._dbManager.GetUserContext(ownerUsername))
            {
                try
                {
                    var res = await db.UserContacts.Where(x => x.UserId == userId).ToListAsync();
                    if (res != null && res.Count > 0)
                    {
                        return true;
                    }
                }
                catch
                { 
                }
            }

            return false;
        }

        public async Task<bool> AddContact(UserExternalContact exContact,bool isAccepted, string ownerusername)
        {
            if (!await this.CheckContactExists(exContact.UserId, ownerusername))
            {
                UserContact cnt = new UserContact();
                cnt.UserId = exContact.UserId;
                cnt.Accepted = isAccepted;
                cnt.InitialMessage = "";
                cnt.Name = exContact.Username;
                cnt.Username = exContact.Username;
                cnt.DataIslandId = exContact.DataislandId;
                cnt.RequestToAccept = false;
                cnt.IsFavourite = false;

                using (var db = this._dbManager.GetUserContext(ownerusername))
                {
                    db.UserContacts.Add(cnt);
                    await db.SaveChangesAsync();

                    IHubContext _hubContext = GlobalHost.ConnectionManager.GetHubContext<UserContactsHub>();
                    if (_hubContext != null)
                    {
                        _hubContext.Clients.User(ownerusername).AddOrUpdateContact(cnt);
                    }
                }

                if (!isAccepted)
                {
                    string senderId = await this.DiUsers.GetUserIdByFromUsername(ownerusername);
                    var userdetails = await this.User.GetUserDetails(ownerusername);
                    
                    UserContact senderContact = new UserContact();
                    senderContact.Accepted = true;
                    senderContact.DataIslandId = await this.DataIslandService.GetDataIslandID();
                    senderContact.Name = ((string.IsNullOrEmpty(userdetails.Name))?ownerusername:userdetails.Name);
                    senderContact.RequestToAccept = true;
                    senderContact.UserId = senderId;
                    senderContact.Username = ownerusername;
                    senderContact.IsFavourite = false;

                    this.Commands.User(senderId, exContact.UserId).SendContactRequest(senderContact);
                    byte[] userAvatar = this.User.GetUserAvatar(ownerusername, 200, "sqr");
                    this.Commands.User(senderId, exContact.UserId).UpdateContactAvatar(senderId, userAvatar);

                }

                return true;
            }
            return false;
        }

        public async Task<bool> ResendContactRequest(string userId, string ownerUsername)
        {
            using(var db = this._dbManager.GetUserContext(ownerUsername))
            {
                var usr = await db.UserContacts.Where(x => x.UserId == userId).SingleOrDefaultAsync();
                if (usr != null && !usr.Accepted)
                {
                    string senderId = await this.DiUsers.GetUserIdByFromUsername(ownerUsername);
                    var userdetails = await this.User.GetUserDetails(ownerUsername);

                    UserContact senderContact = new UserContact();
                    senderContact.Accepted = true;
                    senderContact.DataIslandId = await this.DataIslandService.GetDataIslandID();
                    senderContact.Name = ((string.IsNullOrEmpty(userdetails.Name)) ? ownerUsername : userdetails.Name);
                    senderContact.RequestToAccept = true;
                    senderContact.UserId = senderId;
                    senderContact.Username = ownerUsername;
                    senderContact.IsFavourite = false;

                    this.Commands.User(senderId, userId).SendContactRequest(senderContact);
                    byte[] userAvatar = this.User.GetUserAvatar(ownerUsername, 200, "sqr");
                    this.Commands.User(senderId, userId).UpdateContactAvatar(senderId, userAvatar);
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> AddContactRequest(UserContact contact, string ownerUsername)
        {
            if (!await this.CheckContactExists(contact.UserId, ownerUsername))
            {
                contact.RequestToAccept = true;
                contact.Accepted = false;
                contact.IsFavourite = false;
                using (var db = this._dbManager.GetUserContext(ownerUsername))
                {
                    db.UserContacts.Add(contact);
                    await db.SaveChangesAsync();

                    this.Notifications.ContactRequestAdded(contact.Username, ownerUsername);

                    IHubContext _hubContext = GlobalHost.ConnectionManager.GetHubContext<UserContactsHub>();
                    if (_hubContext != null)
                    {
                        _hubContext.Clients.User(ownerUsername).AddOrUpdateContact(contact);
                    }
                    return true;
                }
            }
            else
            {
                return await this.AcceptContactRequest(contact.UserId, ownerUsername);
            }
        }

        public async Task<bool> AcceptContactRequest(string userId, string ownerUsername)
        {
            using(var db = this._dbManager.GetUserContext(ownerUsername))
            {
                var contact = await db.UserContacts.Where(x => x.UserId == userId).SingleOrDefaultAsync();
                if(contact !=null)
                {
                    contact.RequestToAccept = false;
                    contact.Accepted = true;
                    await db.SaveChangesAsync();

                    byte[] userAvatar = this.User.GetUserAvatar(ownerUsername, 200, "sqr");
                    var userdetails = await this.User.GetUserDetails(ownerUsername);
                    string senderId = await this.DiUsers.GetUserIdByFromUsername(ownerUsername);
                    this.Commands.User(senderId, userId).AcceptContact(senderId);
                    this.Commands.User(senderId, userId).UpdateContactAvatar(senderId, userAvatar);
                    if (userdetails != null)
                    {
                        this.Commands.User(senderId, userId).UpdateContactName(senderId, ((string.IsNullOrEmpty(userdetails.Name)) ? ownerUsername : userdetails.Name));
                    }
                    

                    IHubContext _hubContext = GlobalHost.ConnectionManager.GetHubContext<UserContactsHub>();
                    if (_hubContext != null)
                    {
                        _hubContext.Clients.User(ownerUsername).AddOrUpdateContact(contact);
                    }

                    return true;
                }
            }
            return false;
        }

        public async Task<List<UserContact>> GetUserContacts(string ownerUsername)
        {
            List<UserContact> contacts = new List<UserContact>();
            using (var db = this._dbManager.GetUserContext(ownerUsername))
            {
                contacts = await db.UserContacts.ToListAsync();
            }
            return contacts;
        }

        public async Task<bool> UpdateAcceptStatus(string userId, bool acceptSatatus, string ownerusername)
        {
            using(var db = this._dbManager.GetUserContext(ownerusername))
            {
                UserContact cnt = await db.UserContacts.Where(x => x.UserId == userId).SingleOrDefaultAsync();
                if(cnt!=null)
                {
                    cnt.Accepted = acceptSatatus;
                    await db.SaveChangesAsync();

                    if (acceptSatatus)
                    {
                        this.Notifications.ContactAccepted(cnt.Username, ownerusername);
                    }
                    else
                    {
                        this.Notifications.ContactDeleted(cnt.Username, ownerusername);
                    }

                    IHubContext _hubContext = GlobalHost.ConnectionManager.GetHubContext<UserContactsHub>();
                    if (_hubContext != null)
                    {
                        _hubContext.Clients.User(ownerusername).AddOrUpdateContact(cnt);
                    }

                    return true;
                }
            }
            return false;
        }

        public async Task<bool> UpdateContactName(string userId, string newName, string ownerUsername)
        {
            using (var db = this._dbManager.GetUserContext(ownerUsername))
            {
                UserContact cnt = await db.UserContacts.Where(x => x.UserId == userId).SingleOrDefaultAsync();
                if (cnt != null)
                {
                    cnt.Name = newName;
                    await db.SaveChangesAsync();

                    IHubContext _hubContext = GlobalHost.ConnectionManager.GetHubContext<UserContactsHub>();
                    if (_hubContext != null)
                    {
                        _hubContext.Clients.User(ownerUsername).AddOrUpdateContact(cnt);
                    }

                    return true;
                }
            }
            return false;
        }

        public async Task<bool> DeleteContact(string userId, string ownerUsername)
        {
            using(var db = this._dbManager.GetUserContext(ownerUsername))
            {
                var cnt = await db.UserContacts.Where(x=>x.UserId == userId).SingleOrDefaultAsync();
                if (cnt != null)
                {
                    db.UserContacts.Remove(cnt);
                    await db.SaveChangesAsync();
                    string senderId = await this.DiUsers.GetUserIdByFromUsername(ownerUsername);
                    this.Commands.User(senderId, userId).ContactDeleted(senderId);
                    string avatarpath = this.FileSystemUtilities.GetOrCreateDirectory(this.FilePathProvider.GetUserDataPath(ownerUsername) + "contactsavatars/") + this.Utilities.EscapeUserId(cnt.UserId) + ".png";
                    if (File.Exists(avatarpath))
                    {
                        File.Delete(avatarpath);
                    }
                }
            }
            return false;
        }

        public async Task<bool> UpdateContact(UserContact contact, string ownerUsername)
        {
            using(var db = this._dbManager.GetUserContext(ownerUsername))
            {
                var cnt = await db.UserContacts.Where(x => x.UserId == contact.UserId).SingleOrDefaultAsync();
                if(cnt!=null)
                {
                    cnt.Accepted = contact.Accepted;
                    cnt.DataIslandId = contact.DataIslandId;
                    cnt.InitialMessage = contact.InitialMessage;
                    cnt.Name = contact.Name;
                    await db.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }

        public async Task<UserContact> GetContact(string userId, string ownerUsername)
        {
            using(var db = this._dbManager.GetUserContext(ownerUsername))
            {
                var contact = await db.UserContacts.Where(x => x.UserId == userId).SingleOrDefaultAsync();
                return contact;
            }
        }
        public async Task<UserContact> GetContactByUsername(string username, string ownerUsername)
        {
            using (var db = this._dbManager.GetUserContext(ownerUsername))
            {
                var contact = await db.UserContacts.Where(x => x.Username == username).SingleOrDefaultAsync();
                return contact;
            }
        }

        public async Task<bool> SetFavourite(string userId,bool isFavourite, string ownerUsername)
        {
            using(var db = this._dbManager.GetUserContext(ownerUsername))
            {
                var contact = await db.UserContacts.Where(x => x.UserId == userId).SingleOrDefaultAsync();
                if(contact!=null)
                {
                    contact.IsFavourite = isFavourite;
                    await db.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }

        public void UpdateContactAvatar(string userId, byte[] avatar, string ownerUsername)
        {
            string avatarpath = this.FileSystemUtilities.GetOrCreateDirectory(this.FilePathProvider.GetUserDataPath(ownerUsername) + "contactsavatars/");
            if (avatar == null)
            {
                if (File.Exists(avatarpath + this.Utilities.EscapeUserId(userId) + ".png"))
                {
                    File.Delete(avatarpath + this.Utilities.EscapeUserId(userId) + ".png");
                }
            }
            else
            {
                File.WriteAllBytes(avatarpath +this.Utilities.EscapeUserId(userId) + ".png", avatar);
            }

            IHubContext _hubContext = GlobalHost.ConnectionManager.GetHubContext<UserContactsHub>();
            if (_hubContext != null)
            {
                _hubContext.Clients.User(ownerUsername).UpdateAvatar(userId);
            }
        }

        public async Task<byte[]> GetContactAvatar(string username, int size, string type, string ownerUsername)
        {
            UserContact cnt = await this.GetContactByUsername(username, ownerUsername);
            if (cnt != null)
            {
                string avatarpath = this.FileSystemUtilities.GetOrCreateDirectory(this.FilePathProvider.GetUserDataPath(ownerUsername) + "contactsavatars/") + this.Utilities.EscapeUserId(cnt.UserId) + ".png";
                if (File.Exists(avatarpath))
                {
                    using (Image img = Image.FromFile(avatarpath))
                    {
                        Bitmap resizedImage = this.ImageUtilities.TransformPicture((Bitmap)img, size, size, type);
                        return this.ImageUtilities.TransformImageToByte(resizedImage, "png");
                    }
                }
            }
            return null;
        }

        public async Task SendUpdateAvatarCommandToContacts(byte[] avatar, string ownerUsername)
        {
            string senderId = await this.DiUsers.GetUserIdByFromUsername(ownerUsername);
            List<UserContact> contacts = await this.GetUserContacts(ownerUsername);
            if (contacts != null && contacts.Count > 0)
            {
                using (MemoryStream ms = new MemoryStream(avatar))
                {
                    using (Image img = Image.FromStream(ms))
                    {
                        using (Bitmap resizedImage = ImageUtilities.ResizePictureIfLarger((Bitmap)img, 200, 200))
                        {
                            if (resizedImage != null)
                            {
                                byte[] outputData = ImageUtilities.TransformImageToByte(resizedImage, "png");
                                if (outputData != null)
                                {
                                    foreach (UserContact cnt in contacts)
                                    {
                                        this.Commands.User(senderId, cnt.UserId).UpdateContactAvatar(senderId, outputData);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

    }
}
