using System;
using System.Text;

public abstract class MD5Utils
{
    public static string Encrypt(byte[] buffer)
    {
        System.Security.Cryptography.MD5 alg = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] data = alg.ComputeHash(buffer);
        StringBuilder sBuilder = new StringBuilder();
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }
        return sBuilder.ToString();
    }

    public static string Encrypt(string src)
    {
        byte[] buffer = new byte[src.Length];
        for(int i = 0; i < src.Length; i++)
        {
            buffer[i] = (byte)src[i];
        }
        return Encrypt(buffer);
    }
}
