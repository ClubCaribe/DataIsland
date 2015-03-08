using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dataislandcommon.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class DiAuthorization : Attribute
    {
        public DiAuthorization()
        {

        }


        private string _role = "user";
        public string Role
        {
            get { return _role; }
            set { _role = value; }
        }
    }

}
