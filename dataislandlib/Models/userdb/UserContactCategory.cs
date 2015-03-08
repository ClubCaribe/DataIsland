using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dataislandcommon.Models.userdb
{
    public class UserContactCategory
    {
        [Key]
        public string Id { get; set; }
        public string ContactCategoryId { get; set; }
        public string UserId { get; set; }

    }
}
