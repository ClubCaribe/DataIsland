using System;
namespace dataislandcommon.Services.FileSystem
{
    public interface IFileSystemUtilitiesService
    {
        string GetOrCreateDirectory(string directory);
    }
}
