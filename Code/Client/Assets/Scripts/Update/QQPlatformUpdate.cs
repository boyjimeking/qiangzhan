using LitJson;
using System;


public class QQPlatformUpdate : PlatformUpdate
{
    public override void CheckNeedUpdate()
    {

        PlatformSDK.Instance().CheckNeedUpdate();
    }
    public override void OnCheckNeedUpdateInfo(string param)
    {
        QQNeedUpdateInfo info = null;
        try
        {
            info = JsonMapper.ToObject<QQNeedUpdateInfo>(param);
        }
        catch (Exception e)
        {
            GameDebug.Log(e.Message);
        }

        if (info.status != 0)
        {
            //检查失败
            GameDebug.Log("应用程序检查失败");
        }

        if (info.newApkSize == 0)
        {
            //无更新
            //TODO:通知应用程序
            if(CompleteDelegate != null)
            {
                CompleteDelegate();
            }

            return;
        }
        //进行增量更新测试
        //TODO：通知应用程序 是否要增量更新，现在默认增量更新

        PlatformSDK.Instance().StartUpdate(0);

    }

    public override void OnDownloadAppProgressChanged(string param)
    {
        QQUpdateProcess proc = JsonMapper.ToObject<QQUpdateProcess>(param);

        //TODO:通知下载进度

    }
    public void OnDownloadAppStateChanged(string param)
    {
        QQDownAppState state = JsonMapper.ToObject<QQDownAppState>(param);

        //TODO:通知下载进度
    }

    public void OnDownloadYYBProgressChanged(string param)
    {
        QQDownAppState state = JsonMapper.ToObject<QQDownAppState>(param);

        //TODO:通知下载进度
    }

    public void OnDownloadYYBStateChanged(string param)
    {
        QQDownAppState state = JsonMapper.ToObject<QQDownAppState>(param);

        //TODO:通知下载进度
    }

    
}

