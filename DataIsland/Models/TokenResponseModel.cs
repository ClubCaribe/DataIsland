﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataIsland.Models
{
    public class TokenResponseModel
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string userName { get; set; }
        public string refresh_token { get; set; }
        public string asClientId { get; set; }

    }
}