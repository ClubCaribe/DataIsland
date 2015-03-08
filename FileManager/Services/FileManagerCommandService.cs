using dilib.Services.Communication;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Services
{
    public class FileManagerCommandService : IFileManagerCommandService
    {
        
        public IDICommandsService Commands { get; set; }
        public FileManagerCommandService()
        {

        }

        public bool TestCommand()
        {
            Commands.User("test", "test").TestCommand(0, "23", 45.4);
            return true;
        }
    }
}
