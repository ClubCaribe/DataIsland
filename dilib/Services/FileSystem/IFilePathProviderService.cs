using System;
namespace dataislandcommon.Services.FileSystem
{
    public interface IFilePathProviderService
    {
        string GetMainDatabasePath();
		string GetMainDataPath();
        string GetUserPath(string username);
        string GetUserDataPath(string username);
        string GetUserDatabasesPath(string username);
        bool CheckUserDirectoryExists(string username);
        string GetUserUploadFolder(string username);
        string GetRootConfigPath();
        string GetConfigPath(string section);
        string GetUserFilesPath(string username);
        string GetUserTempPath(string username);
        string GetUsersDirectory();

    }
}
