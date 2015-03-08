using System;
using System.Collections.Generic;
using System.Security.Cryptography;
namespace dataislandcommon.Services.Utilities
{
    public interface ICryptographySingleton
    {
        string ChangePrivateKeyPassword(string privatekey, string oldpassword, string newpassword);
        byte[] DecryptAES256(byte[] encrypted, string password);
        string DecryptAES256(string encryptedBase64Text, string password);
        byte[] DecryptRsa(byte[] data, string privatekey, string passphrase);
        byte[] EncryptAES256(byte[] plainTextBytes, string password);
        string EncryptAES256(string plainText, string password);
        byte[] EncryptRsa(byte[] data, string publickey);
        List<string> GenerateRsaKeys(string passphrase);
        string GetSha1AsBase64String(string tosign);
        byte[] SignMessage(byte[] message, string privatekey, string passphrase);
        string SignMessage(string message, string privatekey, string passphrase);
        bool VerifySignature(byte[] message, byte[] signature, string publickey);
        RSAParameters GetRsaParametersFromKey(string pemKey);
        RSAParameters GetRsaParametersFromKey(string pemKey, string password);
        RSACryptoServiceProvider GetRsaServiceProviderFromPemKey(string pemKey);
        RSACryptoServiceProvider GetRsaServiceProviderFromPemKey(string pemKey, string password);
    }
}
