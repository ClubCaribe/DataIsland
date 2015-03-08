using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dataislandcommon.Interfaces.Communication
{
    public interface IDataIslandCommandHub
    {
        string Username { get; set; }
        string UserId { get; set; }
        string SenderId { get; set; }
        int CommandVersion { get; set; }
    }
}
