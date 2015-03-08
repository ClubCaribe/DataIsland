using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dataislandcommon.Models.ViewModels.SettingsForm
{
    public class DiSettingsFormEntry
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string ControlType { get; set; }
        public object Value { get; set; }
        public DiSettingsFormEntryValidator Validators { get; set; }
    }
}
