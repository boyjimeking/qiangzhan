
using LitJson;
/// <summary>
/// 应用程序的系统信息
/// </summary>
using System;
public sealed class AppSystemInfo
{
    private static string _appConfiguration = "";
    private static string _appIdentifier = "";
    private static string _appVersion = "";

    private static PackageInfo _appInfo;

    public static void InitSysInfos()
    {
        _appConfiguration = "release";

        _appInfo = new PackageInfo();
#if !UNITY_EDITOR && UNITY_ANDROID
        _appInfo.Parse(ApplicationUtility.instance.PackageInfo());
        _appIdentifier = ApplicationUtility.instance.GetAppName();
        _appVersion = ApplicationUtility.instance.GetVersion();
#endif
    }

    public static PackageInfo appInfo
    {
        get
        {
            return _appInfo;
        }
    }

    public static string GetAppVersion()
    {
        if( _appInfo == null )
        {
            return "test";
        }
        return _appInfo.versionName;
    }
    public static int GetAppVersionToInt()
    {
        if (string.IsNullOrEmpty(_appInfo.versionName))
        {
            return -1;
        }
        char[] separator = new char[] { '.' };
        string[] strArray = _appInfo.versionName.Split(separator);
        if ((strArray == null) || (strArray.Length < 3))
        {
            return -1;
        }
        int num = -1;
        try
        {
            int num2 = Convert.ToInt32(strArray[0]);
            int num3 = Convert.ToInt32(strArray[1]);
            int num4 = Convert.ToInt32(strArray[2]);
            num = ((num2 * 10000) + (num3 * 100)) + num4;
        }
        catch
        {
            return -1;
        }
        return num;
    }
}


public sealed class PackageInfo
{
    public string packageName;
    public int versionCode;
    public string versionName;
    public long firstInstallTime;
    public long lastUpdateTime;
    public void Parse(string data)
    {
       JsonData jdata = JsonMapper.ToObject(data);
       if (jdata == null)
           return;
       packageName = (String)jdata["packageName"];
       versionCode = (int)jdata["versionCode"];
       versionName = (String)jdata["versionName"];
       firstInstallTime = (long)jdata["firstInstallTime"];
       lastUpdateTime = (long)jdata["lastUpdateTime"];

    }
}