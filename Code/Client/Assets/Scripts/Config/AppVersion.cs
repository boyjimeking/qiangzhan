
/// <summary>
/// 应用程序的版本信息
/// </summary>
public class AppVersion
{

    private static int _AppVersion = -1;

	public static int ServerAppVersion = -1;

    private static AssetVersion _assetVersion;//服务器当前启用的资源版本
    public static int VersionID
    {
        get
        {
            if (_AppVersion == -1)
                _AppVersion = AppSystemInfo.GetAppVersionToInt();
            return _AppVersion;
        }
    }

    public static AssetVersion assetVersion
    {
        get
        {
            return _assetVersion;
        }
        set
        {
            //只可在初始化时使用
            _assetVersion = value;
        }
    }
}

public class ResVersion
{
    public string md5;
    public int version;
}

public class AssetVersion
{
    public string version;
}

