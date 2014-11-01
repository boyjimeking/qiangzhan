
//更新状态说明

//enum TMSelfUpdateSDKUpdateInfo
//{
//    STATUS_OK = 0,  // 后台有更新包
//    STATUS_CHECKUPDATE_FAILURE = 1, // 检查失败
//    STATUS_CHECKUPDATE_RESPONSE_IS_NULL = 2 //
//};

/// <summary>
/// QQ检查更新回调结构
///  * @param newApkSize apk文件大小(全量包)
// 	  @param newFeature 新特性说明
// 	  @param patchSize 省流量升级包大小
// 	  @param status 值为TMSelfUpdateSDKUpdateInfo(WGPublicDefine.h中有定义), 游戏根据此值来确定是否弹窗提示用户更新
// 	  @param updateDownloadUrl 更新包的下载链接
// 	  @param updateMethod

public class QQNeedUpdateInfo
{
   public int newApkSize; //新的apk的大小
   public int newFeature;//新特性描述
   public int patchSize;//更新包大小
   public int status;
   public string updateDownloadUrl;
   public int updateMethod;

}

public class QQUpdateProcess
{
    public int receiveDataLen;
    public int totalDataLen;
}

public class QQDownAppState
{
    public int state;
    public int errorCode;
    public string errorMsg;
}

public class YYBStateChange
{
    public string url;
    public int state;
    public int errorCode;
    public string errorMsg;

}
