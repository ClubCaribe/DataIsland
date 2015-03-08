using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dataislandcommon.Models.userdb
{
    public class ContactCategory
    {
        [Key]
        public string ContactCategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ParentCategoryId { get; set; }
    }
}
