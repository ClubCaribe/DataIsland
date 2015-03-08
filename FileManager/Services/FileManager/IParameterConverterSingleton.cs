using FileManager.Models.ViewModels;
using System;
using System.IO;
namespace FileManager.Services.FileManager
{
    public interface IParameterConverterSingleton
    {
        DiDirectoryInfo PopulateDiDirectoryInfo(DirectoryInfo dirinfo, string virtualPath);
        DiFileInfo PopulateDiFileInfo(FileInfo flInfo, string virtualPath);
        DiDirectoryListingEntry PopulateDirectoryListingEntry(DiDirectoryInfo dirInfo);
        DiDirectoryListingEntry PopulateDirectoryListingEntry(DiFileInfo fileInfo);
    }
}
