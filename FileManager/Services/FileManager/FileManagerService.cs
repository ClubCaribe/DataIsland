using dataislandcommon.Services.FileSystem;
using dataislandcommon.Services.Utilities;
using FileManager.Models.ViewModels;
using FileManager.Utilities;
using Microsoft.AspNet.SignalR;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Services.FileManager
{
    public class FileManagerService : IFileManagerService
    {
    
        
        public IFileService FileService { get; set; }

        
        public ITaskDispatcherSingleton TaskDispatcher { get; set; }

        
        public IImageUtilitiesSingleton ImageUtilities { get; set; }

        
        public IFilePathProviderService PathProvider { get; set; }

        public FileManagerService()
        {

        }

        public void ZipFilesForDownload(string userId, string pathprefix, List<string> filenames, string zipFileName)
        {
            string zipName = Path.GetFileName(zipFileName);
            IHubContext _hubContext = GlobalHost.ConnectionManager.GetHubContext<FileManagerHub>();
            ProgressManager prManager = new ProgressManager();
            prManager.NumOfFiles = FileService.CountAllFiles(pathprefix, filenames);
            prManager.hubContext = _hubContext;
            prManager.CurrentValue = 0;
            prManager.User = userId;
            prManager.ZipFileName = zipName;
            TaskDispatcher.AddCompressingProgressManager(userId, prManager);

            if (FileService.CompressFiles(pathprefix, filenames, zipFileName, prManager))
            {
                string compressedFileName = zipFileName;
                if(!string.IsNullOrEmpty(prManager.NewFileName) && prManager.NewFileName!=zipName)
                {
                    File.Move(zipFileName, Path.GetDirectoryName(zipFileName) + "/" + prManager.NewFileName);
                    compressedFileName = Path.GetDirectoryName(zipFileName) + "/" + prManager.NewFileName;
                    zipName = Path.GetFileName(compressedFileName);
                    
                }
                FileInfo finf = new FileInfo(compressedFileName);

                _hubContext.Clients.User(userId).FileZipIsReadyForDownload(zipName, finf.Length);
            }
            else
            {
                _hubContext.Clients.User(userId).RemoveFileFromCompressionFiles(zipName);
                File.Delete(zipFileName);
            }

            TaskDispatcher.RemoveCompressingProgressManager(userId, prManager);
        }

        public List<DiCompressedFile> GetCompressedFiles(string path)
        {
            List<DiCompressedFile> files = new List<DiCompressedFile>();
            DirectoryInfo dinf = new DirectoryInfo(path);
            foreach (FileInfo finf in dinf.GetFiles().ToArray())
            {
                DiCompressedFile cmpFile = new DiCompressedFile();
                cmpFile.Name = finf.Name;
                cmpFile.DisplayName = finf.Name;
                cmpFile.Progress = 100;
                cmpFile.Size = finf.Length;
                files.Add(cmpFile);
            }
            return files;
        }

        public bool RotateFlipPreview(string user, string path, string transform)
        {
            string previewPath = PathProvider.GetUserDataPath(user) + FileManagerConsts.FilesPreviewsPath + path;
            previewPath = Path.GetDirectoryName(previewPath) + "/" + Path.GetFileNameWithoutExtension(previewPath) + ".png";
            if (File.Exists(previewPath))
            {
                using (Bitmap img = (Bitmap)Image.FromFile(previewPath))
                {
                    if (img != null)
                    {
                        RotateFlipType transformType = RotateFlipType.RotateNoneFlipNone;
                        switch (transform)
                        {
                            case FileManagerConsts.Rotate90NoFlip:
                                {
                                    transformType = RotateFlipType.Rotate90FlipNone;
                                }
                                break;
                            case FileManagerConsts.Rotate270NoFlip:
                                {
                                    transformType = RotateFlipType.Rotate270FlipNone;
                                }
                                break;
                            case FileManagerConsts.RotateNoneFlipX:
                                {
                                    transformType = RotateFlipType.RotateNoneFlipX;
                                }
                                break;
                            case FileManagerConsts.RotateNoneFlipY:
                                {
                                    transformType = RotateFlipType.RotateNoneFlipY;
                                }
                                break;
                        }
                        ImageUtilities.RotateFlip(img, transformType);
                        img.Save(previewPath, ImageUtilities.GetImageFormatFromExtension(Path.GetExtension(previewPath)));
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
