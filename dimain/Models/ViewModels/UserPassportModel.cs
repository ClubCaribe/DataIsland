using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dimain.Models.ViewModels
{
    public class UserPassportModel
    {
        public string UserId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Signature { get; set; }
    }
}
