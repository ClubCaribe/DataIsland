using dataislandcommon.Models.ViewModels.SettingsForm;
using System;
using System.Collections.Generic;
namespace dataislandcommon.Services.Utilities
{
    public interface IUIUtilitiesService
    {
        DiSettingsForm GetSettingsForm(string configPath, Dictionary<string, object> values);
        bool ValidateSettingsForm(DiSettingsFormData form);
    }
}
