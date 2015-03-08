using dataislandcommon.Models.System;
using dataislandcommon.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace dataislandcommon.Services.FileSystem
{
    public class FilePathProviderService : dataislandcommon.Services.FileSystem.IFilePathProviderService
    {
        private readonly IFileSystemUtilitiesService _utilities;
        private readonly IDataProviderSingleton _dataProvider;

        public FilePathProviderService(IFileSystemUtilitiesService utilities, IDataProviderSingleton dataProvider)
        {
             _utilities = utilities;
             _dataProvider = dataProvider;
        }

        public string GetMainDatabasePath()
        {
            string path = ((DataIslandState)_dataProvider.GetModel(DiConsts.DataIslandSate)).RootFilePath ;
            return _utilities.GetOrCreateDirectory(path) + "main.db";
            
        }

		public string GetMainDataPath()
		{
            string path = ((DataIslandState)_dataProvider.GetModel(DiConsts.DataIslandSate)).RootFilePath;
            return _utilities.GetOrCreateDirectory(path);
		}

        public string  GetRootConfigPath()
        {
            string path = ((DataIslandState)_dataProvider.GetModel(DiConsts.DataIslandSate)).RootFilePath + "/settings/configs/";
            return _utilities.GetOrCreateDirectory(path);
        }

        public string GetConfigPath(string section)
        {
            return _utilities.GetOrCreateDirectory(GetRootConfigPath()+section+"/");
        }

        public string GetUserDataPath(string username)
        {
            string userDirectory = ((DataIslandState)_dataProvider.GetModel(DiConsts.DataIslandSate)).RootFilePath + "/users/" + username + "/userdata/";
            _utilities.GetOrCreateDirectory(userDirectory);
            return userDirectory;
        }

        public string GetUserFilesPath(string username)
        {
            string userDirectory = ((DataIslandState)_dataProvider.GetModel(DiConsts.DataIslandSate)).RootFilePath + "/users/" + username + "/files/";
            _utilities.GetOrCreateDirectory(userDirectory);
            return userDirectory;
        }

        public string GetUserDatabasesPath(string username)
        {
            string databasesDirectory = GetUserPath(username) + "databases/";
            _utilities.GetOrCreateDirectory(databasesDirectory);
            return databasesDirectory;
        }

        public string GetUserPath(string username)
        {
            string userDirectory = ((DataIslandState)_dataProvider.GetModel(DiConsts.DataIslandSate)).RootFilePath + "/users/" + username + "/";
            _utilities.GetOrCreateDirectory(userDirectory);
            return userDirectory;
        }

        public string GetUserTempPath(string username)
        {
            string userDirectory = ((DataIslandState)_dataProvider.GetModel(DiConsts.DataIslandSate)).RootFilePath + "/users/" + username + "/temp/";
            _utilities.GetOrCreateDirectory(userDirectory);
            return userDirectory;
        }

        public bool CheckUserDirectoryExists(string username)
        {
            string userDirectory = ((DataIslandState)_dataProvider.GetModel(DiConsts.DataIslandSate)).RootFilePath + "/users/" + username + "/";
            return Directory.Exists(userDirectory);
        }

        public string GetUserUploadFolder(string username)
        {
            string uploadsDirectory = GetUserPath(username) + "uploads/";
            _utilities.GetOrCreateDirectory(uploadsDirectory);
            return uploadsDirectory;
        }

        public string GetUsersDirectory()
        {
            string usersDirectory = ((DataIslandState)_dataProvider.GetModel(DiConsts.DataIslandSate)).RootFilePath + "/users/";
            _utilities.GetOrCreateDirectory(usersDirectory);
            return usersDirectory;
        }
    }
}
