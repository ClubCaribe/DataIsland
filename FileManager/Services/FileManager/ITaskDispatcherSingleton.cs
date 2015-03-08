using FileManager.Utilities;
using System;
using System.Collections.Generic;
namespace FileManager.Services.FileManager
{
    public interface ITaskDispatcherSingleton
    {
        bool AddCompressingProgressManager(string username, IProgressManager manager);
        System.Collections.Generic.Dictionary<string, List<IProgressManager>> CompressingFiles { get; set; }
        IProgressManager GetCompressingProgressManager(string username, string filename);
        bool RemoveCompressingProgressManager(string username, IProgressManager manager);
    }
}
