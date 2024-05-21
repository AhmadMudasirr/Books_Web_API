using System;
using System.IO;
using System.Security.Cryptography;


namespace BooksAPI.Encryption
{
    public class DataEncryption
    {
        private static readonly byte[] ProcessKey = new byte[32];
        private static readonly byte[] ProcessIV = new byte[16];
        public void GenerateKeys()
        {
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(ProcessKey);
            rng.GetBytes(ProcessIV);
        }



        public static string Encrypt(string text)
        {

            if (text == null)
            {
                return null;

            }
            else
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = ProcessKey;
                    aes.IV = ProcessIV;

                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    using (MemoryStream msEncrypt = new MemoryStream())
                    {

                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {

                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {

                                swEncrypt.Write(text);
                            }
                            return Convert.ToBase64String(msEncrypt.ToArray());
                        }
                    }
                }
            }
        }



        public static string Decrypt(byte[] cipherText)
        {
            using (Aes aes = Aes.Create())
            {

                aes.Key = ProcessKey;
                aes.IV = ProcessIV;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {

                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {

                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            return srDecrypt.ReadToEnd();

                        }
                    }

                }
            }
        }
    }
}
