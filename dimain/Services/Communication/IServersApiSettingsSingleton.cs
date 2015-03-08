using System;
namespace dimain.Services.Communication
{
    public interface IServersApiSettingsSingleton
    {
        scutils.transportprotocol.MainServerApiSettings MainServerApiKey { get; set; }
    }
}
