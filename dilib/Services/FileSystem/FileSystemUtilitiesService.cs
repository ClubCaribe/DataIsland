using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace dataislandcommon.Services.FileSystem
{
    public class FileSystemUtilitiesService : dataislandcommon.Services.FileSystem.IFileSystemUtilitiesService
    {
        public FileSystemUtilitiesService()
        {

        }

        public string GetOrCreateDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return directory;
        }

        
    }
}
