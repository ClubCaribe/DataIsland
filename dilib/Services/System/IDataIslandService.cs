
using dataislandcommon.Models.System;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
namespace dimain.Services.System
{
    public interface IDataIslandService
    {
        void InitDataIslandStateObject();
        DataIslandState GetDataIslandStateObject();
        void SetRootDocumentPath(string path);
        Task<string> GetDataIslandID();
        Task<bool> RegisterDataIsland(string name, string description, string domain, string webpage, bool isPublic, string adminEmail);
        Task StartDataIsland();
        bool IsDataIslandInitialised();
        Task<RSACryptoServiceProvider> GetDataIslandPublicRsaCryproProvider();
        RSACryptoServiceProvider GetDataIslandPublicRsaCryproProviderNonAsync();
        Task<RSACryptoServiceProvider> GetDataIslandPrivateRsaCryptoProvider();
        RSACryptoServiceProvider GetDataIslandPrivateRsaCryptoProviderNonAsync();
        Task<string> GetDataIslandPublicKey(string dataislandid);
        Task<string> GetDataislandUrl(string dataislandId);
    }
}
