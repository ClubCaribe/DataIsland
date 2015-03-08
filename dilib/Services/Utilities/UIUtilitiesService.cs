using dataislandcommon.Models.ViewModels.SettingsForm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace dataislandcommon.Services.Utilities
{
    public class UIUtilitiesService : IUIUtilitiesService
    {
        public UIUtilitiesService()
        {

        }

        public DiSettingsForm GetSettingsForm(string configPath, Dictionary<string, object> values)
        {
            string config = File.ReadAllText(configPath);
            DiSettingsForm form = JsonConvert.DeserializeObject<DiSettingsForm>(config);
            foreach (var entry in form.Form.Entries)
            {
                if(values.Keys.Contains(entry.Name))
                {
                    entry.Value = values[entry.Name];
                }
            }
            return form;
        }

        public bool ValidateSettingsForm(DiSettingsFormData form)
        {
            bool isValid = true;
            foreach(DiSettingsFormEntry entry in form.Entries)
            {
                if(entry.Validators.Required && ((entry.Value==null)||(string.IsNullOrEmpty(entry.Value.ToString()))))
                {
                    isValid = false;
                }
            }
            return isValid;
        }
    }
}
