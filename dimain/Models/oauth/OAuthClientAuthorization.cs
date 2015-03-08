using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace dimain.Models.oauth
{
    
    public class OAuthClientAuthorization
    {
        [Key]
        public int AuthorizationId { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public int ClientId { get; set; }

        public int UserId { get; set; }

        public string Scope { get; set; }

        public DateTime ExpirationDateUtc { get; set; }
    }
}
