using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Utilities
{
    public class ProgressManager : IProgressManager
    {

        Dictionary<string, object> properties;
        public ProgressManager()
        {
            properties = new Dictionary<string, object>();
            Cancel = false;
        }

        public string NewFileName { get; set; }

        public int NumOfFiles
        {
            get
            {
                return (int)properties["NumOfFiles"];
            }
            set
            {
                properties["NumOfFiles"] = value;
            }
        }

        public int CurrentValue
        {
            get
            {
                return (int)properties["CurrentValue"];
            }
            set
            {
                properties["CurrentValue"] = value;
            }
        }

        public string User {
            get
            {
                return (string)properties["User"];
            }
            set
            {
                properties["User"] = value;
            }
        }

        public string ZipFileName {
            get
            {
                return (string)properties["ZipFileName"];
            }
            set
            {
                properties["ZipFileName"] = value;
            }
        }

        public bool Cancel { get; set; }

        public IHubContext hubContext { get; set; }

        public void Increment()
        {
            CurrentValue = CurrentValue + 1;
            int percent = (int)(((float)CurrentValue / (float)NumOfFiles) * 100.0f);
            hubContext.Clients.User(User).SetFilesCompressionProgress(ZipFileName, percent);
        }


        public Dictionary<string, object> Properties
        {
            get
            {
                return properties;
            }
            
        }

        public void CancelOperation()
        {
            Cancel = true;
        }

        public bool IsCanceled()
        {
            return Cancel;
        }
    }
}
