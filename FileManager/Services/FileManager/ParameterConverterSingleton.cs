using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FileManager.Models.ViewModels;

namespace FileManager.Services.FileManager
{
    public class ParameterConverterSingleton : IParameterConverterSingleton
    {
        public ParameterConverterSingleton()
        {

        }

        public DiDirectoryInfo PopulateDiDirectoryInfo(DirectoryInfo dirinfo, string virtualPath)
        {
            DiDirectoryInfo diDirectoryInfo = new DiDirectoryInfo();
            diDirectoryInfo.CreationTime = dirinfo.CreationTime;
            diDirectoryInfo.CreationTimeUtc = dirinfo.CreationTimeUtc;
            diDirectoryInfo.Exists = dirinfo.Exists;
            diDirectoryInfo.Extension = dirinfo.Extension;
            diDirectoryInfo.FullName = ((!string.IsNullOrEmpty(virtualPath)) ? virtualPath.Replace("//", "/") : "");
            diDirectoryInfo.LastAccessTime = dirinfo.LastAccessTime;
            diDirectoryInfo.LastAccessTimeUtc = dirinfo.LastAccessTimeUtc;
            diDirectoryInfo.LastWriteTime = dirinfo.LastWriteTime;
            diDirectoryInfo.LastWriteTimeUtc = dirinfo.LastWriteTimeUtc;
            diDirectoryInfo.Name = (((!string.IsNullOrEmpty(virtualPath) && virtualPath != "/")) ? dirinfo.Name : "");

            return diDirectoryInfo;
        }

        public DiFileInfo PopulateDiFileInfo(FileInfo flInfo, string virtualPath)
        {
            DiFileInfo finf = new DiFileInfo();
            finf.CreationTime = flInfo.CreationTime;
            finf.CreationTimeUtc = flInfo.CreationTimeUtc;
            finf.DirectoryName = flInfo.DirectoryName;
            finf.Exists = flInfo.Exists;
            finf.Extension = flInfo.Extension;
            finf.FullName = ((!string.IsNullOrEmpty(virtualPath)) ? virtualPath.Replace("//", "/") + "/" + flInfo.Name : "/" + flInfo.Name);
            finf.IsReadOnly = flInfo.IsReadOnly;
            finf.LastAccessTime = flInfo.LastAccessTime;
            finf.LastAccessTimeUtc = flInfo.LastAccessTimeUtc;
            finf.LastWriteTime = flInfo.LastWriteTime;
            finf.LastWriteTimeUtc = flInfo.LastWriteTimeUtc;
            finf.Length = flInfo.Length;
            finf.Name = flInfo.Name;

            return finf;
        }

        public DiDirectoryListingEntry PopulateDirectoryListingEntry(DiDirectoryInfo dirInfo)
        {
            DiDirectoryListingEntry entry = new DiDirectoryListingEntry();
            entry.Extension = dirInfo.Extension;
            entry.FileSystemObject = dirInfo;
            entry.FullName = dirInfo.FullName;
            entry.IsDirectory = true;
            entry.Name = dirInfo.Name;
            entry.Size = 0;
            return entry;
        }

        public DiDirectoryListingEntry PopulateDirectoryListingEntry(DiFileInfo fileInfo)
        {
            DiDirectoryListingEntry entry = new DiDirectoryListingEntry();
            entry.Extension = fileInfo.Extension;
            entry.FileSystemObject = fileInfo;
            entry.FullName = fileInfo.FullName;
            entry.IsDirectory = false;
            entry.Name = fileInfo.Name;
            entry.Size = fileInfo.Length;
            return entry;
        }
    }
}
