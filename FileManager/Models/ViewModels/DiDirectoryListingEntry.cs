using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Models.ViewModels
{
    public class DiDirectoryListingEntry
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Extension { get; set; }
        public long Size { get; set; }
        public bool IsDirectory { get; set; }
        public object FileSystemObject { get; set; }
    }
}
