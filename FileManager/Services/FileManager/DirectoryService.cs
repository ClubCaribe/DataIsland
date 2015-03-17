using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using FileManager.Models.ViewModels;
using FileManager.Services.db;
using FileManager.Models.db;

namespace FileManager.Services.FileManager
{
    public class DirectoryService : IDirectoryService
    {
        public IFileDatabaseManagerSingleton DbManager { get; set; }
        public IDbSharedResourcesService SharedResources { get; set; }

        private readonly IParameterConverterSingleton ParameterConverter;

        public DirectoryService(IParameterConverterSingleton _parameterConverter)
        {
            ParameterConverter = _parameterConverter;
        }

        public DiDirectoryInfo GetDirectoryInfo(string prefixPath, string virtualPath)
        {
            DirectoryInfo dirinfo = new DirectoryInfo(prefixPath + virtualPath);
            
            if (dirinfo.Exists)
            {
                DiDirectoryInfo diDirectoryInfo = ParameterConverter.PopulateDiDirectoryInfo(dirinfo, virtualPath);

                return diDirectoryInfo;
            }
            return null;
        }

        public DiDirectoryInfo CreateDirectory(string prefixPath, string directoryPath)
        {
            DirectoryInfo dirInfo = Directory.CreateDirectory(prefixPath + directoryPath);
            return ParameterConverter.PopulateDiDirectoryInfo(dirInfo, directoryPath);
        }

        public void DeleteDirectory(string prefixPath, string filePreviewPath, string virtualPath, bool recursive)
        {
            Directory.Delete(prefixPath + virtualPath, recursive);
            Directory.Delete(filePreviewPath + virtualPath, recursive);
        }

        public DiDirectoryInfo GetParent(string prefixPath, string virtualPath)
        {
            if (!string.IsNullOrEmpty(virtualPath) && virtualPath != "/")
            {
                DirectoryInfo dirInfo = Directory.GetParent(prefixPath + virtualPath);
                return ParameterConverter.PopulateDiDirectoryInfo(dirInfo, virtualPath);
            }
            return null;
        }

        public void MoveDirectory(string prefixPath, string virtualPath, string destinationPath)
        {
            Directory.Move(prefixPath + virtualPath, prefixPath + destinationPath);
        }

        public List<DiDirectoryInfo> GetDirectories(string prefixPath, string virtualPath)
        {
            List<DiDirectoryInfo> directoriesList = new List<DiDirectoryInfo>();

            DirectoryInfo dinf = new DirectoryInfo(prefixPath + virtualPath);
            foreach(DirectoryInfo childDir in dinf.GetDirectories().ToList())
            {
                directoriesList.Add(ParameterConverter.PopulateDiDirectoryInfo(childDir, "/" + virtualPath + "/" + childDir.Name));
            }
            return directoriesList;
        }

        public List<DiFileInfo> GetFiles(string prefixPath, string virtualPath)
        {
            List<DiFileInfo> files = new List<DiFileInfo>();

            DirectoryInfo dinf = new DirectoryInfo(prefixPath + virtualPath);
            foreach(FileInfo finf in dinf.GetFiles().ToList())
            {
                files.Add(ParameterConverter.PopulateDiFileInfo(finf, virtualPath));
            }

            return files;
        }

        public async Task<List<DiDirectoryListingEntry>> ListDirectory(string prefixPath, string virtualPath, string ownerUsername)
        {
            return await ListDirectory(prefixPath, virtualPath, "", ownerUsername);
        }

        public async Task<List<DiDirectoryListingEntry>> ListDirectory(string prefixPath, string virtualPath, string searchPhrase,string ownerUsername)
        {
            List<DiDirectoryInfo> directories = GetDirectories(prefixPath, virtualPath);
            if(directories.Count>0)
            {
                if (!string.IsNullOrEmpty(searchPhrase))
                {
                    string search = searchPhrase.ToLower();
                    for (int i = directories.Count - 1; i > -1; i--)
                    {
                        if(!directories[i].Name.ToLower().Contains(search))
                        {
                            directories.RemoveAt(i);
                        }
                    }
                }
            }
            List<DiFileInfo> files = GetFiles(prefixPath, virtualPath);
            if (files.Count > 0)
            {
                if (!string.IsNullOrEmpty(searchPhrase))
                {
                    string search = searchPhrase.ToLower();
                    for (int i = files.Count - 1; i > -1; i--)
                    {
                        if (!files[i].Name.ToLower().Contains(search))
                        {
                            files.RemoveAt(i);
                        }
                    }
                }
            }

            List<DiDirectoryListingEntry> listing = new List<DiDirectoryListingEntry>();
            foreach(DiDirectoryInfo dirInfo in directories)
            {
                listing.Add(ParameterConverter.PopulateDirectoryListingEntry(dirInfo));
            }
            foreach (DiFileInfo fileInfo in files)
            {
                listing.Add(ParameterConverter.PopulateDirectoryListingEntry(fileInfo));
            }

            using (var db = this.DbManager.GetDbContext(ownerUsername))
            {
                foreach(var entry in listing)
                {
                    SharedResource res = await this.SharedResources.GetResource(((entry.IsDirectory)?"":"/")+entry.FullName, db);
                    if (res != null)
                    {
                        entry.IsShared = true;
                        entry.IsSharedAsPublic = res.IsPublic;
                        entry.SharedPublicPath = "/file/" + res.ID;
                    }
                }
            }
            return listing;
        }
    }
}
