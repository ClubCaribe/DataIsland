using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Org.BouncyCastle;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Math;
using System.IO;
using Org.BouncyCastle.Crypto.Generators;
using System.Security.Cryptography;
using System.Security;

namespace dataislandcommon.Services.Utilities
{
    public class Password
            : IPasswordFinder
    {
        private readonly char[] password;

        public Password(
            char[] word)
        {
            this.password = (char[])word.Clone();
        }

        public char[] GetPassword()
        {
            return (char[])password.Clone();
        }
    }

    public class CryptographySingleton : dataislandcommon.Services.Utilities.ICryptographySingleton
    {
        public byte[] SignMessage(byte[] message, string privatekey, string passphrase)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(privatekey)))
                {
                    using (StreamReader sr = new StreamReader(ms))
                    {
                        IPasswordFinder pGet = new Password(passphrase.ToCharArray());
                        PemReader pr = new PemReader(sr, pGet);

                        var pemKeyParams = (AsymmetricCipherKeyPair)pr.ReadObject();

                        ISigner sig = SignerUtilities.GetSigner("SHA1withRSA");
                        sig.Init(true, pemKeyParams.Private);

                        sig.BlockUpdate(message, 0, message.Length);

                        byte[] signature = sig.GenerateSignature();
                        return signature;
                    }
                }

            }
            catch
            {
            }
            return null;
        }



        public string SignMessage(string message, string privatekey, string passphrase)
        {
            try
            {
                byte[] tosign = Encoding.UTF8.GetBytes(message);
                byte[] outcome = SignMessage(tosign, privatekey, passphrase);
                if (outcome != null)
                {
                    return Convert.ToBase64String(outcome);
                }
            }
            catch
            {
            }
            return null;
        }

        public bool VerifySignature(byte[] message, byte[] signature, string publickey)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(publickey)))
                {
                    using (StreamReader sr = new StreamReader(ms))
                    {
                        PemReader pr = new PemReader(sr);
                        var pemKeyParams = (RsaKeyParameters)pr.ReadObject();
                        var cypherParams = new RsaKeyParameters(false, pemKeyParams.Modulus, pemKeyParams.Exponent);

                        ISigner sig = SignerUtilities.GetSigner("SHA1withRSA");
                        sig.Init(false, cypherParams);

                        sig.BlockUpdate(message, 0, message.Length);
                        bool result = sig.VerifySignature(signature);


                        return result;
                    }
                }
            }
            catch
            {
            }
            return false;
        }

        public byte[] DecryptRsa(byte[] data, string privatekey, string passphrase)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(privatekey)))
                {
                    using (StreamReader sr = new StreamReader(ms))
                    {
                        IPasswordFinder pGet = new Password(passphrase.ToCharArray());
                        PemReader pr = new PemReader(sr, pGet);
                        var pemKeyParams = (AsymmetricCipherKeyPair)pr.ReadObject();
                        //IAsymmetricBlockCipher eng = new Pkcs1Encoding(new RsaEngine());
                        IAsymmetricBlockCipher eng = new OaepEncoding(new RsaEngine());
                        eng.Init(false, pemKeyParams.Private);
                        byte[] encdata = data;
                        encdata = eng.ProcessBlock(encdata, 0, encdata.Length);


                        return encdata;
                    }
                }
            }
            catch
            {
            }
            return null;
        }

        public byte[] EncryptRsa(byte[] data, string publickey)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(publickey)))
                {
                    using (StreamReader sr = new StreamReader(ms))
                    {
                        PemReader pr = new PemReader(sr);
                        var pemKeyParams = (RsaKeyParameters)pr.ReadObject();
                        var cypherParams = new RsaKeyParameters(false, pemKeyParams.Modulus, pemKeyParams.Exponent);
                        //IAsymmetricBlockCipher eng = new Pkcs1Encoding(new RsaEngine());
                        IAsymmetricBlockCipher eng = new OaepEncoding(new RsaEngine());

                        eng.Init(true, cypherParams);
                        byte[] encdata = data;
                        encdata = eng.ProcessBlock(encdata, 0, encdata.Length);
                        return encdata;
                    }
                }
            }
            catch
            {
            }
            return null;
        }

        public List<string> GenerateRsaKeys(string passphrase)
        {
            try
            {
                using (TextWriter ptwr = new StringWriter())
                {
                    PemWriter ppwr = new PemWriter(ptwr);

                    using (TextWriter prtwr = new StringWriter())
                    {
                        PemWriter prpwr = new PemWriter(prtwr);

                        SecureRandom secrandom = new SecureRandom();
                        RsaKeyPairGenerator r = new RsaKeyPairGenerator();
                        r.Init(new KeyGenerationParameters(secrandom, 2048));
                        AsymmetricCipherKeyPair keys = r.GenerateKeyPair();

                        ppwr.WriteObject(keys.Public);

                        prpwr.WriteObject(keys.Private, "DES-EDE3-CBC", passphrase.ToCharArray(), secrandom);
                        ppwr.Writer.Flush();
                        prpwr.Writer.Flush();

                        List<string> lst = new List<string>();
                        lst.Add(ptwr.ToString());
                        lst.Add(prtwr.ToString());

                        return lst;
                    }
                }

            }
            catch
            {
            }
            return null;

        }

        public string ChangePrivateKeyPassword(string privatekey, string oldpassword, string newpassword)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(privatekey)))
                {
                    SecureRandom secrandom = new SecureRandom();
                    using (StreamReader sr = new StreamReader(ms))
                    {
                        IPasswordFinder pGet = new Password(oldpassword.ToCharArray());
                        PemReader pr = new PemReader(sr, pGet);
                        var pemKeyParams = (AsymmetricCipherKeyPair)pr.ReadObject();

                        TextWriter prtwr = new StringWriter();
                        PemWriter prpwr = new PemWriter(prtwr);

                        prpwr.WriteObject(pemKeyParams.Private, "DES-EDE3-CBC", newpassword.ToCharArray(), secrandom);
                        prpwr.Writer.Flush();

                        return prtwr.ToString();
                    }

                }
            }
            catch
            {
            }
            return "";
        }

        public RSAParameters GetRsaParametersFromKey(string pemKey)
        {
            using (MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(pemKey)))
            {
                using (TextReader txtRD = new StreamReader(ms))
                {
                    PemReader pemRD = new PemReader(txtRD);
                    RsaKeyParameters rsaKeyParameters = (RsaKeyParameters)pemRD.ReadObject();
                    RSAParameters rsaParameters = new global::System.Security.Cryptography.RSAParameters();
					rsaParameters.Modulus = rsaKeyParameters.Modulus.ToByteArrayUnsigned();
					rsaParameters.Exponent = rsaKeyParameters.Exponent.ToByteArrayUnsigned();
                    return rsaParameters;
                }
            }
        }

        public RSAParameters GetRsaParametersFromKey(string pemKey, string password)
        {
            using (MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(pemKey)))
            {
                using (TextReader txtRD = new StreamReader(ms))
                {
                    IPasswordFinder pGet = new Password(password.ToCharArray());

                    PemReader pemRD = new PemReader(txtRD,pGet);
                    AsymmetricCipherKeyPair rsaKeyParameters = (AsymmetricCipherKeyPair)pemRD.ReadObject();
                    RsaPrivateCrtKeyParameters rpckp = ((RsaPrivateCrtKeyParameters)rsaKeyParameters.Private);

                    RSAParameters parms = new global::System.Security.Cryptography.RSAParameters();
                    parms.Modulus = rpckp.Modulus.ToByteArrayUnsigned();
                    parms.P = rpckp.P.ToByteArrayUnsigned();
                    parms.Q = rpckp.Q.ToByteArrayUnsigned();
                    parms.DP = rpckp.DP.ToByteArrayUnsigned();
                    parms.DQ = rpckp.DQ.ToByteArrayUnsigned();
                    parms.InverseQ = rpckp.QInv.ToByteArrayUnsigned();
                    parms.D = rpckp.Exponent.ToByteArrayUnsigned();
                    parms.Exponent = rpckp.PublicExponent.ToByteArrayUnsigned();
                    
                    return parms;
                }
            }
        }

        public RSACryptoServiceProvider GetRsaServiceProviderFromPemKey(string pemKey)
        {
            RSAParameters rsaParameters = GetRsaParametersFromKey(pemKey);
            RSACryptoServiceProvider rsaProvider = new global::System.Security.Cryptography.RSACryptoServiceProvider();
            rsaProvider.ImportParameters(rsaParameters);
            return rsaProvider;
        }

        public RSACryptoServiceProvider GetRsaServiceProviderFromPemKey(string pemKey, string password)
        {
            RSAParameters rsaParameters = GetRsaParametersFromKey(pemKey,password);
            RSACryptoServiceProvider rsaProvider = new global::System.Security.Cryptography.RSACryptoServiceProvider();
            rsaProvider.ImportParameters(rsaParameters);
            return rsaProvider;
        }


        private byte[] randomBytes(int size)
        {
            byte[] array = new byte[size];
            new Random().NextBytes(array);
            return array;
        }


        ///Text to be encrypted
        ///Password to encrypt with
        /// An encrypted string encoded with Base64
        public string EncryptAES256(string plainText, string password)
        {
            try
            {
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] encryptedBytes = EncryptAES256(plainTextBytes, password);
                return Convert.ToBase64String(encryptedBytes);
            }
            catch
            {
            }
            return "";
        }

        public byte[] EncryptAES256(byte[] plainTextBytes, string password)
        {
            try
            {
                byte[] salt = randomBytes(8);
                // if salt is same during every encryption, same key is used. May be useful for testing, must not be used in production code
                //salt = new byte[8]; //set {0,0...}

                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                using (MD5 md5 = MD5.Create())
                {

                    int preKeyLength = password.Length + salt.Length;
                    byte[] preKey = new byte[preKeyLength];

                    Buffer.BlockCopy(passwordBytes, 0, preKey, 0, passwordBytes.Length);
                    Buffer.BlockCopy(salt, 0, preKey, passwordBytes.Length, salt.Length);

                    byte[] key = md5.ComputeHash(preKey);

                    int preIVLength = key.Length + preKeyLength;
                    byte[] preIV = new byte[preIVLength];

                    Buffer.BlockCopy(key, 0, preIV, 0, key.Length);
                    Buffer.BlockCopy(preKey, 0, preIV, key.Length, preKey.Length);

                    byte[] iv = md5.ComputeHash(preIV);

                    md5.Clear();

                    using (AesManaged aes = new AesManaged())
                    {
                        aes.Mode = CipherMode.CBC;
                        aes.Padding = PaddingMode.PKCS7;
                        aes.KeySize = 128;
                        aes.BlockSize = 128;
                        aes.Key = key;
                        aes.IV = iv;

                        byte[] encrypted = null;

                        using (ICryptoTransform Encryptor = aes.CreateEncryptor())
                        {
                            using (MemoryStream MemStream = new MemoryStream())
                            {
                                using (CryptoStream CryptoStream = new CryptoStream(MemStream, Encryptor, CryptoStreamMode.Write))
                                {
                                    CryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                    CryptoStream.FlushFinalBlock();

                                    encrypted = MemStream.ToArray();
                                    CryptoStream.Close();
                                }
                                MemStream.Close();
                            }
                        }
                        aes.Clear();

                        int resultLength = encrypted.Length + 8 + 8;
                        byte[] salted = Encoding.UTF8.GetBytes("Salted__");
                        byte[] result = new byte[resultLength];

                        Buffer.BlockCopy(salted, 0, result, 0, salted.Length);
                        Buffer.BlockCopy(salt, 0, result, 8, salt.Length);
                        Buffer.BlockCopy(encrypted, 0, result, 16, encrypted.Length);

                        return result;
                    }
                }
            }
            catch
            {
            }
            return null;
        }

        private bool IsDataEqual(byte[] a, int a_offset, byte[] b, int b_offset, int length)
        {
            if (a.Length - a_offset < length)
            {
                return false;
            }
            if (b.Length - b_offset < length)
            {
                return false;
            }
            for (int i = 0; i < length; i++) { if (a[i + a_offset] != b[i + b_offset]) { return false; } } return true;
        }

        public string DecryptAES256(string encryptedBase64Text, string password)
        {
            try
            {
                byte[] encrypted = Convert.FromBase64String(encryptedBase64Text);
                byte[] plaintext = DecryptAES256(encrypted, password);
                return Encoding.UTF8.GetString(plaintext);
            }
            catch
            {
            }
            return "";
        }

        public byte[] DecryptAES256(byte[] encrypted, string password)
        {
            try
            {
                bool isSalted = false; byte[] salt = null; if (encrypted.Length > 16)
                {
                    byte[] salted = Encoding.UTF8.GetBytes("Salted__");

                    if (IsDataEqual(encrypted, 0, salted, 0, 8))
                    {
                        isSalted = true;

                        salt = new byte[8];
                        Buffer.BlockCopy(encrypted, 8, salt, 0, 8);
                    }
                }

                byte[] aesData;

                if (isSalted)
                {
                    int aesDataLength = encrypted.Length - 16;
                    aesData = new byte[aesDataLength];
                    Buffer.BlockCopy(encrypted, 16, aesData, 0, aesDataLength);
                }
                else
                {
                    salt = new byte[0];
                    aesData = encrypted;
                }

                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                using (MD5 md5 = MD5.Create())
                {

                    int preKeyLength = password.Length + salt.Length;
                    byte[] preKey = new byte[preKeyLength];

                    Buffer.BlockCopy(passwordBytes, 0, preKey, 0, passwordBytes.Length);
                    Buffer.BlockCopy(salt, 0, preKey, passwordBytes.Length, salt.Length);

                    byte[] key = md5.ComputeHash(preKey);

                    int preIVLength = key.Length + preKeyLength;
                    byte[] preIV = new byte[preIVLength];

                    Buffer.BlockCopy(key, 0, preIV, 0, key.Length);
                    Buffer.BlockCopy(preKey, 0, preIV, key.Length, preKey.Length);

                    byte[] iv = md5.ComputeHash(preIV);

                    md5.Clear();

                    using (AesManaged aes = new AesManaged())
                    {
                        aes.Mode = CipherMode.CBC;
                        aes.Padding = PaddingMode.PKCS7;
                        aes.KeySize = 128;
                        aes.BlockSize = 128;
                        aes.Key = key;
                        aes.IV = iv;

                        byte[] clearData = null;

                        using (ICryptoTransform Decryptor = aes.CreateDecryptor())
                        {
                            using (MemoryStream MemStream = new MemoryStream())
                            {
                                using (CryptoStream CryptoStream = new CryptoStream(MemStream, Decryptor, CryptoStreamMode.Write))
                                {
                                    CryptoStream.Write(aesData, 0, aesData.Length);
                                    CryptoStream.FlushFinalBlock();

                                    clearData = MemStream.ToArray();
                                    CryptoStream.Close();
                                }
                                MemStream.Close();
                            }
                        }
                        aes.Clear();

                        return clearData;
                    }
                }
            }
            catch
            {
            }
            return null;
        }

        public string GetSha1AsBase64String(string tosign)
        {
            using (SHA1Managed sh = new SHA1Managed())
            {
                sh.Initialize();
                byte[] buff = null;
                buff = Encoding.UTF8.GetBytes(tosign);
                if (buff != null)
                {
                    byte[] result;
                    result = sh.ComputeHash(buff);
                    string sres = "";
                    if (result != null)
                    {
                        sres = Convert.ToBase64String(result);
                        return sres;
                    }
                }
                return "";
            }
        }
    }
}
