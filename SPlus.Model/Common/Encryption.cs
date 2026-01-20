using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SPlus.Model.Common
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
        public static string GenerateSecurityToken(string username, string passord, string delegator = "")
        {
            string result = string.Format("{0}:{1}:{2}", username, passord, delegator);
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
                if (arr.Count() != 3)
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
                if (arr.Count() != 3)
                    throw new Exception("Wrong Security Token Format");

                if (arr[2] != "") // Keep it as empty string 
                    userName = arr[2]; // Delegation mode
                else
                    userName = arr[0]; // Normal mode

                return userName;
            }
            catch (Exception ex)
            {
                throw new Exception("Wrong Security Token Format");
            }
        }
    }
}