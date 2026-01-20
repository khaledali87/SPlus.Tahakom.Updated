using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;


namespace SPlus.Helper
{
    public class Encryption
    {
        public static string key = "secret";
        public static string Encrypt(string clearText)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
        public static string Decrypt(string cipherText)
        {
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
        public static string GenerateSecurityToken(string username, string passord, Guid sessionid, string delegator = "")
        {
            string result = string.Format("{0}:{1}:{2}:{3}", username, passord, delegator, sessionid);
            result = Convert.ToBase64String(Encoding.ASCII.GetBytes(result));

            return result.ToEcryptedString();
        }
        public static string GenerateSecurityTokenSSO(string username, Guid UserSessionID, string delegator = "")
        {
            string result = string.Format("{0}::{1}:{2}", username, delegator, UserSessionID);
            result = Convert.ToBase64String(Encoding.ASCII.GetBytes(result));

            return result.ToEcryptedString();
        }
        public static string[] GetCredentialsFromSecurityToken(string token)
        {
            try
            {
                string result = token.ToDecryptedString();
                result = Encoding.ASCII.GetString(Convert.FromBase64String(result));

                string[] arr = result.Split(new char[] { ':' });
                if (arr.Count() != 4)
                    throw new Exception("Wrong Security Token Format");

                return arr;
            }
            catch (Exception ex) 
            {
                throw new Exception("Wrong Security Token Format");

            }
        }

        public static string GetCurrentUser(string token)
        {
            try
            {
                string userName = string.Empty;
                string result = token.ToDecryptedString();
                result = Encoding.ASCII.GetString(Convert.FromBase64String(result));

                string[] arr = result.Split(new char[] { ':' });
                if (arr.Count() != 4)
                    throw new Exception("Wrong Security Token Format");

                if (arr[2] != "") // Keep it as empty string 
                    userName = arr[2]; // Delegation mode
                else
                    userName = arr[0]; // Normal mode

                return userName.ToLower();
            }
            catch (Exception ex)
            {
                throw new Exception("Wrong Security Token Format");
            }
        }

        public static string DecryptPassword(string cipherText)
        {
            string privateKey = @"-----BEGIN RSA PRIVATE KEY-----
        MIICXAIBAAKBgQC7pbDRvuqRL0EsKjYGqPZWu4VFKI1iilisb3+yW5guIP4I+T8q
        hP8dcUugiwa2fDwGgl3Y+PHASGdCfvYOfMefWlNUoGeLv4Tx6nqvq2OcVzRhYjUf
        IpRE7z81xhmKtvxxLTNzTAoHN6rWqarKYMFydR/sD+oJs7LnnJoWieh1QQIDAQAB
        AoGAPeQ4nfXKiHh9loOVrjysg472NglaGNZoyPc9tyZe21gmce9D1lJnkt57g0hX
        vnjbk4oMSjRSCInZBSW7IqwlauiYwJra5O7MbLFaCTWDDnwFB+Zcz9E2PbPdFQFl
        jyYfh0gm9zvo79oyqESXtfYaMfx45zPbJGOkhiPcjZDsZcECQQDyy2qz6L6JALIY
        L1CtY6NTSm33rXv6Y+RzTBZovkK9OvMODvklsOhbcNQx0o7vQ5600dbHORwteCcm
        cHMgFDGnAkEAxdpsD2omWPzRYCRmk72aqRtccp9AE9eE75mUG6sMt3NDqwGeTuWV
        EH70XC3yIwqFmkc3zzNjZqsxTEDtuRtu1wJBAJoAHpko2poJv+0JLfIczf7JqgC8
        oHPMop3jOB+N9sUSPBLBupSGpotBgMZtWM44pNTqeIH7Hn1UxfhiwRMq2+cCQDwZ
        K8fG450WNncwt2PbLRZ+9CbxDqK4TW4GRYHeBD/ZKE3ScQbgH9Zh6dHyNuHD+W8y
        gNZUcrYl/BSAiHU4ywMCQDok1QgvVmc8jma9qK2dxmd4IR8varRPti4VQAMumdaE
        4s1Ofdw+8Bl2FATkdQZDLUQbWat4E5l8uGLfYZAHSow=
        -----END RSA PRIVATE KEY-----";
            RSACryptoServiceProvider RSAprivateKey = ImportPrivateKey(privateKey);
            var bytesCypherText = Convert.FromBase64String(cipherText);
            var bytesPlainTextData = RSAprivateKey.Decrypt(bytesCypherText, false);
            return System.Text.Encoding.UTF8.GetString(bytesPlainTextData);
        }

        private static RSACryptoServiceProvider ImportPrivateKey(string pem)
        {
            PemReader pr = new PemReader(new StringReader(pem));
            AsymmetricCipherKeyPair KeyPair = (AsymmetricCipherKeyPair)pr.ReadObject();
            RSAParameters rsaParams = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)KeyPair.Private);

            RSACryptoServiceProvider csp = new RSACryptoServiceProvider();// cspParams);
            csp.ImportParameters(rsaParams);
            return csp;
        }

    }
}