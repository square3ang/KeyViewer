using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace KeyViewer.Utils
{
    public static class CryptoUtils
    {
        public const string DefaultKey = "Suckyoubus Chan~!! Daiski~♥♥";
        public static byte[] EncryptAes(string data, string key)
        {
            byte[] rawKey = Encoding.UTF8.GetBytes(key);
            byte[] padKey = new byte[32];
            Array.Copy(rawKey, 0, padKey, 0, Math.Min(rawKey.Length, 32));
            using (Aes aes = Aes.Create())
            {
                aes.Key = padKey;
                aes.IV = new byte[16];
                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter streamWriter = new StreamWriter(cs))
                        streamWriter.Write(data);
                    return ms.ToArray();
                }
            }
        }
        public static string DecryptAes(byte[] data, string key)
        {
            byte[] rawKey = Encoding.UTF8.GetBytes(key);
            byte[] padKey = new byte[32];
            Array.Copy(rawKey, 0, padKey, 0, Math.Min(rawKey.Length, 32));
            using (Aes aes = Aes.Create())
            {
                aes.Key = padKey;
                aes.IV = new byte[16];
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new MemoryStream(data))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    using (StreamReader streamReader = new StreamReader(cs))
                        return streamReader.ReadToEnd();
                }
            }
        }
        public static string Xor(string data, string key)
        {
            char[] charKey = key.ToCharArray();
            char[] rawData = data.ToCharArray();
            int charKeyLen = charKey.Length;
            int rawDataLen = rawData.Length;
            StringBuilder sb = new StringBuilder(rawDataLen);
            for (int i = 0; i < rawDataLen; i++)
                sb.Append((char)(rawData[i] ^ charKey[i % charKeyLen]));
            return sb.ToString();
        }
        public static byte[] Xor(byte[] data, byte[] key)
        {
            int keyLen = key.Length;
            int dataLen = data.Length;
            byte[] result = new byte[dataLen];
            for (int i = 0; i < dataLen; i++)
                result[i] = (byte)(data[i] ^ key[i % keyLen]);
            return result;
        }
    }
}
