using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dimain.Models.oauth
{
    
    public class OAuthSymmetricCryptoKey
    {
        [Key]
        public string ID { get; set; } 

        public string Bucket { get; set; }

        public string Handle { get; set; }

        public DateTime ExpiresUtc { get; set; }

        public string SecretBase64 { get; set; }

		[NotMapped]
        public byte[] Secret { get; set; }
    }
}
