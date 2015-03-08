using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dimain.Models.oauth
{

    /*
      <Column Name="Context" Type="System.String" IsPrimaryKey="true" CanBeNull="false" />
      <Column Name="Code" Type="System.String" IsPrimaryKey="true" CanBeNull="false" />
      <Column Name="Timestamp" Type="System.DateTime" IsPrimaryKey="true" CanBeNull="false" />
     */
     
    public class OAuthNonce
    {
        [Key]
        public string ID { get; set; }

        public string Context { get; set; }

        public string Code { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
