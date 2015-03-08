using FileManager.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Services.FileManager
{
    public class TaskDispatcherSingleton : ITaskDispatcherSingleton
    {
        public Dictionary<string,List<IProgressManager>> CompressingFiles { get; set; }

        public TaskDispatcherSingleton()
        {
            CompressingFiles = new Dictionary<string, List<IProgressManager>>();

        }

        public bool AddCompressingProgressManager(string username, IProgressManager manager)
        {
            if(!CompressingFiles.ContainsKey(username))
            {
                CompressingFiles[username] = new List<IProgressManager>();
            }

            CompressingFiles[username].Add(manager);
            return true;
        }

        public bool RemoveCompressingProgressManager(string username, IProgressManager manager)
        {
            if (CompressingFiles.ContainsKey(username))
            {
                CompressingFiles[username].Remove(manager);
            }
            return false;
        }

        public IProgressManager GetCompressingProgressManager(string username, string filename)
        {
            if (CompressingFiles.ContainsKey(username))
            {
                foreach (IProgressManager manager in CompressingFiles[username])
                {
                    if ((string)manager.Properties["ZipFileName"] == filename)
                    {
                        return manager;
                    }
                }
            }
            return null;
        }
    }
}
