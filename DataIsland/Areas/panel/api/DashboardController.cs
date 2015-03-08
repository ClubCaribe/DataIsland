using Streamail.Models.db;
using Streamail.Services;
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
    [RoutePrefix("api/panel/dashboard")]
    public class DashboardController : ApiController
    {

        private readonly IStreamailService Streamail;

        public DashboardController(IStreamailService streamailService)
        {
            this.Streamail = streamailService;
        }

        [Route("getnotifications/{pageNum}")]
        [HttpGet]
        public async Task<List<StreamailMessage>> GetChatMessages(int pageNum)
        {
            return await this.Streamail.GetMessages("notificator", pageNum, 20, this.User.Identity.Name);
        }
    }
}
