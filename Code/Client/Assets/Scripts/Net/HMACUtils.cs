using System;
using System.Text;
using System.Security.Cryptography;

public abstract class HMACUtils
{
    public static string HMacSha1Encrypt(byte[] buffer, byte[] key)
    {
        HMACSHA1 hmac = new HMACSHA1(key);
        byte[] data = hmac.ComputeHash(buffer);

        StringBuilder sBuilder = new StringBuilder();
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }
        return sBuilder.ToString();
    }

    public static string HMacSha1Encrypt(string src, string key)
    {
        byte[] buffer = new byte[src.Length];
        for(int i = 0; i < src.Length; i++)
        {
            buffer[i] = (byte)src[i];
        }

        byte[] key_buf = new byte[key.Length];
        for(int i = 0; i < key.Length; i++)
        {
            key_buf[i] = (byte)key[i];
        }

        return HMacSha1Encrypt(buffer, key_buf);
    }
}
