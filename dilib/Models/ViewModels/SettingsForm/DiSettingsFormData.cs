using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dataislandcommon.Models.ViewModels.SettingsForm
{
    public class DiSettingsFormData
    {
        public string Title { get; set; }
        public string FormName { get; set; }
        public List<DiSettingsFormEntry> Entries { get; set; }
        public string SubmitButtonText { get; set; }

    }
}
