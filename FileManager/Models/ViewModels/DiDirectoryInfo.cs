using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Models.ViewModels
{
    public class DiDirectoryInfo
    {
        public DateTime CreationTime { get; set; }
        public DateTime CreationTimeUtc { get; set; }
        public bool Exists { get; set; }
        public string Extension { get; set; }
        public string FullName { get; set; }
        public DateTime LastAccessTime { get; set; }
        public DateTime LastAccessTimeUtc { get; set; }
        public DateTime LastWriteTime { get; set; }
        public DateTime LastWriteTimeUtc { get; set; }
        public string Name { get; set; }

    }
}
