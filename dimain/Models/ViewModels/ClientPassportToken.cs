using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dimain.Models.ViewModels
{
    public class ClientPassportToken
    {
        public string TokenID { get; set; }
        public string DataIslandID { get; set; }
        public DateTime ExpirationTime { get; set; }

    }
}
