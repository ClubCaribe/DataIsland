using dataislandcommon.Models.userdb;
using dataislandcommon.Models.ViewModels.Contacts;
using dataislandcommon.Services.Notifications;
using dataislandcommon.Services.System;
using dataislandcommon.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DataIsland.Areas.panel.api
{
    [Authorize]
    [RoutePrefix("api/panel/usercontacts")]
    public class UserContactsController : ApiController
    {
        public INotifierService Notificator { get; set; }

        private readonly IUserContactsService Contacts;
        public UserContactsController(IUserContactsService contacts)
        {
            this.Contacts = contacts;
        }


        [Route("findexternalusers/{searchPhrase}")]
        [HttpGet]
        public async Task<List<UserExternalContact>> FindExternalUsers(string searchPhrase)
        {
            List<UserExternalContact> users = await this.Contacts.FindExternalUsers(searchPhrase, this.User.Identity.Name);
            return users;
        }

        [Route("addcontact")]
        [HttpPost]
        public async Task<bool> AddContact(UserExternalContact contact)
        {
            try
            {
                bool res = await this.Contacts.AddContact(contact, false, this.User.Identity.Name);
                return res;
            }
            catch
            {
            }
            return false;
        }

        [Route("getcontact")]
        [HttpPost]
        public async Task<UserContact> GetContact(JObject jsonData)
        {
            dynamic postData = jsonData;
            string userId = postData.userId;
            return await this.Contacts.GetContact(userId, this.User.Identity.Name);
        }

        [Route("getcontacts")]
        [HttpGet]
        public async Task<List<UserContact>> GetContacts()
        {
            return await this.Contacts.GetUserContacts(this.User.Identity.Name);
        }

        [Route("deletecontact")]
        [HttpPost]
        public async Task<bool> DeleteContact(JObject jsonData)
        {
            dynamic postData = jsonData;
            string userId = postData.UserId;
            return await this.Contacts.DeleteContact(userId, this.User.Identity.Name);
        }

        [Route("setfavourite")]
        [HttpPost]
        public async Task<bool> SetFavourite(JObject jsonData)
        {
            dynamic postData = jsonData;
            string userId = postData.UserId;
            bool isFavourite = postData.IsFavourite;

            return await this.Contacts.SetFavourite(userId, isFavourite, this.User.Identity.Name);
        }

        [Route("acceptrequest")]
        [HttpPost]
        public async Task<bool> AcceptRequest(JObject jsonData)
        {
            dynamic postData = jsonData;
            string userId = postData.UserId;
            return await this.Contacts.AcceptContactRequest(userId, this.User.Identity.Name);
        }

        [Route("resendrequest")]
        [HttpPost]
        public async Task<bool> ResendContactRequest(JObject jsonData)
        {
            dynamic postData = jsonData;
            string userId = postData.UserId;
            return await this.Contacts.ResendContactRequest(userId, this.User.Identity.Name);
        }
    }
}