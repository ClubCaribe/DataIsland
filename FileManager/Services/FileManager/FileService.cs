using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.ShellExtensions;
using System.Drawing;
using dataislandcommon.Services.Utilities;

using System.IO;
using System.Windows.Media.Imaging;
using FileManager.Models.ViewModels;
using System.IO.Compression;
using Microsoft.AspNet.SignalR;
using FileManager.Utilities;

namespace FileManager.Services.FileManager
{
    public class FileService : IFileService
    {
        
        public IImageUtilitiesSingleton ImageUtilities { get; set; }

        
        public IParameterConverterSingleton ParameterConverter { get; set; }

        public FileService()
        {

        }

        public byte[] GenerateThumbnailToByteArray(string prefix, string vPath)
        {
            BitmapSource thumbnail = null;
            if (File.Exists(prefix + vPath))
            {
                ShellFile shellFile = ShellFile.FromFilePath(prefix + vPath);
                thumbnail = shellFile.Thumbnail.ExtraLargeBitmapSource;
            }
            else if (Directory.Exists(prefix + vPath))
            {
                thumbnail = ShellFileSystemFolder.FromFolderPath(prefix + vPath).Thumbnail.ExtraLargeBitmapSource;
            }

            if(thumbnail!=null)
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    encoder.Frames.Add(BitmapFrame.Create(thumbnail));
                    encoder.Save(memoryStream);

                    return memoryStream.ToArray();
                }
            }

            return null;
        }

        public Bitmap GenerateThumbnailToImage(string prefix, string vPath)
        {
            BitmapSource thumbnail = null;
            if (File.Exists(prefix + vPath))
            {
                ShellFile shellFile = ShellFile.FromFilePath(prefix + vPath);
                thumbnail = shellFile.Thumbnail.ExtraLargeBitmapSource;
            }
            else if (Directory.Exists(prefix + vPath))
            {
                thumbnail = ShellFileSystemFolder.FromFolderPath(prefix + vPath).Thumbnail.ExtraLargeBitmapSource;
            }

            if(thumbnail!=null)
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    encoder.Frames.Add(BitmapFrame.Create(thumbnail));
                    encoder.Save(memoryStream);

                    Bitmap bmp = (Bitmap)Image.FromStream(memoryStream);
                    return bmp;
                }
            }

            return null;
        }

        public byte[] GetFilePreview(string prefix, string vPath)
        {
            byte[] preview = ImageUtilities.GenerateImagePreviewAndGetImageData(prefix + vPath,"png");
            if (preview == null)
            {
                preview = this.GenerateThumbnailToByteArray(prefix, vPath);
            }
            return preview;
        }

        public void DeleteFile(string prefix, string previewPrefix, string vFileName)
        {
            File.Delete(prefix + vFileName);
            string previewPath = previewPrefix + vFileName;
            previewPath = previewPath.Substring(0, previewPath.LastIndexOf(".")) + ".png";
            File.Delete(previewPrefix + vFileName);
            File.Delete(previewPath);
        }

        public byte[] GetFile(string prefix, string vFileName)
        {
            
            return File.ReadAllBytes(prefix + vFileName);
        }

        public async Task<byte[]> GetFileAsync(string prefix, string vFileName)
        {
            byte[] data = null;
            using (FileStream fs = new FileStream(prefix + vFileName,FileMode.Open,FileAccess.Read))
            {
                data = new byte[(int)fs.Length];
                await fs.ReadAsync(data, 0, data.Length);
                fs.Close();
            }
            return data;
        }

        public bool CompressFiles(string prefix, List<string> filenames, string destinationFileName, IProgressManager progressManager)
        {
            using(ZipArchive archive = ZipFile.Open(destinationFileName,ZipArchiveMode.Create))
            {
                foreach(string filename in filenames)
                {
                    if(progressManager.IsCanceled())
                    {
                        return false;
                    }
                    string fullPath = prefix + filename;
                    if (File.Exists(fullPath))
                    {
                        archive.CreateEntryFromFile(fullPath, Path.GetFileName(fullPath));
                        if (progressManager != null)
                        {
                            progressManager.Increment();
                        }
                    }
                    if(Directory.Exists(fullPath))
                    {
                        DirectoryInfo dinf = new DirectoryInfo(fullPath);
                        if(!this.CompressDirectory(archive, fullPath, dinf.Name + "/", progressManager))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        public bool CompressDirectory(ZipArchive archive, string directoryPath, string zipVirtualPath, IProgressManager progressManager)
        {
            
            DirectoryInfo dirinfo = new DirectoryInfo(directoryPath);
            foreach(DirectoryInfo dir in dirinfo.GetDirectories().ToArray())
            {
                CompressDirectory(archive, dir.FullName, zipVirtualPath + dir.Name + "/", progressManager);
            }
            foreach (FileInfo finfo in dirinfo.GetFiles().ToArray())
            {
                if (progressManager.IsCanceled())
                {
                    return false;
                }
                archive.CreateEntryFromFile(finfo.FullName, zipVirtualPath+finfo.Name);
                if (progressManager != null)
                {
                    progressManager.Increment();
                }
            }
            return true;
        }

        public int CountAllFiles(string prefix, List<string> filenames)
        {
            int count = 0;
            foreach (string filename in filenames)
            {

                string fullPath = prefix + filename;
                if (File.Exists(fullPath))
                {
                    count++;
                }
                if (Directory.Exists(fullPath))
                {
                    count = count + CountFilesInDirectory(fullPath);
                }
            }
            return count;
        }

        public int CountFilesInDirectory(string directoryPath)
        {
            int count = 0;
            DirectoryInfo dirinfo = new DirectoryInfo(directoryPath);
            foreach (DirectoryInfo dir in dirinfo.GetDirectories().ToArray())
            {
                count = count + CountFilesInDirectory(dir.FullName);
            }
            foreach (FileInfo finfo in dirinfo.GetFiles().ToArray())
            {
                count++;
            }
            return count;
        }

        public DiFileInfo GetFileInfo(string prefix, string vFileName)
        {
            FileInfo finf = new FileInfo(prefix + vFileName);
            DiFileInfo info = ParameterConverter.PopulateDiFileInfo(finf, vFileName);
            return info;
        }

    }
}
