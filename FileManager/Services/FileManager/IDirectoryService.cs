using FileManager.Models.ViewModels;
using System;
using System.Collections.Generic;
namespace FileManager.Services.FileManager
{
    public interface IDirectoryService
    {
        DiDirectoryInfo CreateDirectory(string prefixPath, string directoryPath);
        void DeleteDirectory(string prefixPath, string filePreviewPath, string virtualPath, bool recursive);
        System.Collections.Generic.List<DiDirectoryInfo> GetDirectories(string prefixPath, string virtualPath);
        DiDirectoryInfo GetDirectoryInfo(string prefixPath, string virtualPath);
        List<DiFileInfo> GetFiles(string prefixPath, string virtualPath);
        DiDirectoryInfo GetParent(string prefixPath, string virtualPath);
        List<DiDirectoryListingEntry> ListDirectory(string prefixPath, string virtualPath);
        List<DiDirectoryListingEntry> ListDirectory(string prefixPath, string virtualPath, string searchPhrase);
        void MoveDirectory(string prefixPath, string virtualPath, string destinationPath);
    }
}
