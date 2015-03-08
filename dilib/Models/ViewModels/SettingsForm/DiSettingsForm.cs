using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dataislandcommon.Models.ViewModels.SettingsForm
{
    public class DiSettingsForm
    {
        public List<string> Scopes { get; set; }
        public DiSettingsFormData Form { get; set; }
    }
}
