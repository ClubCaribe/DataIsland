using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dimain.Models.oauth
{
    
    public class OAuthClient
    {
        [Key]
        public int ClientId { get; set; }


        public string ClientIdentifier { get; set; }


        public string ClientSecret { get; set; }


        public string Callback { get; set; }


        public string Name { get; set; }

        public int ClientType { get; set; }
    }
}
