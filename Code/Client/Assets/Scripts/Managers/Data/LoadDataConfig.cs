using System;
using System.IO;
using System.Text;

public class LoadDataConfig : Singleton<LoadDataConfig>
{
    private string m_sLoadPath;
//    private GemConfig mConfig;

    public static string LoadPath
    {
        get
        {
#if !(UNITY_WEBPLAYER || UNITY_FLASH)
            if ((Singleton<LoadDataConfig>.Instance.m_sLoadPath == null) || (Singleton<LoadDataConfig>.Instance.m_sLoadPath == string.Empty))
            {
                string path = Directory.GetCurrentDirectory() + "/DataConfig/Config.txt";
                if (File.Exists(path))
				{
                    string[] strArray = File.ReadAllLines(path, Encoding.UTF8);
                    Singleton<LoadDataConfig>.Instance.m_sLoadPath = strArray[0];
                }
            }
#endif
            return Singleton<LoadDataConfig>.Instance.m_sLoadPath;
        }
    }
}

