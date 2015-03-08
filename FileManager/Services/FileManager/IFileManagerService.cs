using FileManager.Models.ViewModels;
using System;
using System.Collections.Generic;
namespace FileManager.Services.FileManager
{
    public interface IFileManagerService
    {
        void ZipFilesForDownload(string userId, string pathprefix, List<string> filenames, string zipFileName);
        List<DiCompressedFile> GetCompressedFiles(string path);
        bool RotateFlipPreview(string user, string path, string transform);
    }
}
