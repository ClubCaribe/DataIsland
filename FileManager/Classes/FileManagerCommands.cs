using dataislandcommon.Classes.Attributes;
using dataislandcommon.Interfaces.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Classes
{
    [DataIslandCommands]
    public class FileManagerCommands : IDataIslandCommandHub
    {
        public FileManagerCommands()
        {

        }

        public string Username { get; set; }
        public string UserId { get; set; }
        public string SenderId
        {
            get;
            set;
        }

        public int CommandVersion
        {
            get;
            set;
        }

        public void TestCommand(int counter, string strvalue, float flvalue)
        {
            int i;
            i = 10;
        }
    }
}
