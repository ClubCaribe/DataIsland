using FileManager.Models.ViewModels;
using FileManager.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace FileManager.Services.FileManager
{
    public interface IFileService
    {
        byte[] GenerateThumbnailToByteArray(string prefix, string vPath);
        Bitmap GenerateThumbnailToImage(string prefix, string vPath);
        byte[] GetFilePreview(string prefix, string vPath);
        void DeleteFile(string prefix, string previewPrefix, string vFileName);
        byte[] GetFile(string prefix, string vFileName);
        Task<byte[]> GetFileAsync(string prefix, string vFileName);
        DiFileInfo GetFileInfo(string prefix, string vFileName);
        bool CompressFiles(string prefix, List<string> filenames, string destinationFileName, IProgressManager progressManager);
        int CountAllFiles(string prefix, List<string> filenames);
    }
}
