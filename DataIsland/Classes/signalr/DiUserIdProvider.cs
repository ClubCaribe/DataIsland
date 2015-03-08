using DataIsland.App_Start;
using dataislandcommon.Utilities;
using Microsoft.AspNet.SignalR;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataIsland.Classes.signalr
{
    public class DiUserIdProvider : IUserIdProvider
    {
        public string GetUserId(IRequest request)
        {
            return request.User.Identity.Name;
        }
    }
}