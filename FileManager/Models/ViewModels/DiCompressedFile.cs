using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Models.ViewModels
{
    public class DiCompressedFile
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public int Progress { get; set; }
        public long Size { get; set; }
    }
}
