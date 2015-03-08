using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dimain.Models.maindb
{
    public class DataCache
    {
        [Key]
        public string Id { get; set; }
        public string Data { get; set; }
    }
}
