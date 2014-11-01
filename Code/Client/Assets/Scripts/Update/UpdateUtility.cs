//#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY)
//#define MOBILE
//#endif

using FantasyEngine;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
/// <summary>
/// 版本更新控制器
/// </summary>
public class UpdateUtility : Singleton<UpdateUtility>
{
    public delegate void VOIDDelegate();

    public VOIDDelegate CompleteDelegate;
    public VOIDDelegate InstallDelegate;

   ///本地文件夹
    public string mLocalDataFolder = string.Empty;
    ///服务器及本地资源列表
    private Filelist mServerFileListInfo;
    public  Filelist mLocalDataListInfo;

    ///待移除资源
    public List<ResData> mRemoveData = new List<ResData>();

    private ResVersion mServerRes;


    public static string DataPath = "Data";

    private PlatformUpdate mPlatform;
   
    /// <summary>
    /// 开始更新
    /// </summary>
    public void StartUpdate()
    {
        //TODO:先检测主版本号，如果不匹配
        if(true)
        {
            BehaviourUtil.StartCoroutine(LoadFileListFromServer());
        }
        else
        {
            //TODO:Dosomething
            //CheckResource();
        }
    }

    public PlatformUpdate GetPlatformUpdate()
    {
        return mPlatform;
    }
    public void PlatformUpdate()
    {
		NotifyProcess (0,"检查版本更新");
        //TODO:检测平台环境
        mPlatform = new QQPlatformUpdate();
        mPlatform.CompleteDelegate = DetectDatas;
        mPlatform.CheckNeedUpdate();

    }

    /// <summary>
    /// 检测资源是否已拷贝完毕
    /// </summary>
    public void DetectDatas()
    {
       mLocalDataFolder = AssetManager.GetFileAssetBase(AssetManager.PathType.Path_Local);

#if UNITY_ANDROID && (!UNITY_EDITOR)
       ///Apk中是否存在Data文件夹,如果存在则此App是一个完整的App
       ///本地文件夹中存储的版本号信息应与此App一致，如果不一致
       ///则将APK中的资源更新到本地
       ///当本地无法找到filelist文件时也需要更新资源
       
        string installFlag = AppSystemInfo.appInfo.versionName + AppSystemInfo.appInfo.lastUpdateTime;
        if(PlayerPrefs.HasKey(GameConfig.appInstallFlag) && PlayerPrefs.GetString(GameConfig.appInstallFlag) != installFlag)
        {
            //删除文件夹
            if(Directory.Exists(mLocalDataFolder))
                Directory.Delete(mLocalDataFolder,true);
        }

       bool isFullApp = ApplicationUtility.instance.FileExist(DataPath+ "/"+GameConfig.FileListPath);
       //如果版本号不相同
       string filelist = mLocalDataFolder + GameConfig.FileListPath;

       if (isFullApp && PlayerPrefs.GetInt(GameConfig.VersionID) != AppVersion.VersionID || !File.Exists(filelist))
       {
           InstallDelegate = StartUpdate;
           //将文件拷贝到sdCard中
           CopyFileList();
       }
       else
       {
           StartUpdate();
       }
#else
       StartUpdate();
#endif
    }

 #if UNITY_ANDROID && (!UNITY_EDITOR)
    public void CopyFileList()
    {
        BehaviourUtil.StartCoroutine(InstallData(true, true));
    }

    /// <summary>
    /// 安装数据
    /// </summary>
    /// <param name="isInstall"></param>
    /// <param name="isCopyFile"></param>
    /// <returns></returns>
    protected IEnumerator InstallData(bool isInstall, bool isCopyFile)
    {
        yield return BehaviourUtil.StartCoroutine(AndroidCopy(isCopyFile));

        if (InstallDelegate != null)
            InstallDelegate();
    }
    /// <summary>
    /// android下的文件拷贝
    /// </summary>
    /// <param name="copyFile"></param>
    /// <returns></returns>
    private IEnumerator AndroidCopy(bool copyFile)
    {
        string sdcardPath = AssetManager.GetFileAssetBase(AssetManager.PathType.Path_Local);

        NotifyProcess(0,"初始化本地资源...");
        yield return 2;
        ///将数据放在这里以便于通知进度
        //先不让所有资源拷贝出去
#region 拷贝资源到外部
            //AssetParam param = new AssetParam();
            //param.ptype = AssetManager.PathType.Path_Streaming;

            //yield return AssetManager.instance.LoadResource(GameConfig.FileListPath,param);

            //if(param.asset == null || param.asset.Data == null)
            //{
            //    //程序是安装包
            //    NotifyProcess(1, "本地资源初始化成功");
            //    yield break;
            //}

            //string text = param.asset.Data.text;
            //Filelist flist = JsonMapper.ToObject<Filelist>(text);

            //if(flist == null || flist.filelist == null)
            //{
            //    NotifyProcess(1, "本地资源初始化失败");
            //    yield break;
            //}

            //int fileCount = flist.filelist.Count;

            //int flag = 0;
            //foreach (KeyValuePair<string, ResData> fData in flist.filelist)
            //{
            //    string filePath = DataPath + "/" + fData.Key;
            //    string destPath = sdcardPath + fData.Key;
       
            //    if (copyFile)
            //    {
            //        if (!File.Exists(destPath))
            //            ApplicationUtility.instance.CopyAFile(filePath, destPath);
            //    }
            //    flag++;
            //    if (flag % 10 == 0)
            //    {
            //        NotifyProcess((float)flag / fileCount, "初始化本地资源...(不消耗流量哦O(∩_∩)O~");
            //        yield return 1;
            //    }
            //}
#endregion
        //拷贝完资源之后将 filelist 以及versiondata拷贝过去
        ApplicationUtility.instance.CopyAFile(DataPath+"/"+GameConfig.FileListPath, sdcardPath + GameConfig.FileListPath);
        ApplicationUtility.instance.CopyAFile(DataPath + "/" + GameConfig.DataversionPath, sdcardPath + GameConfig.DataversionPath);

        //拷贝完之后将当前的app版本号写入记录中
        PlayerPrefs.SetInt(GameConfig.VersionID,AppVersion.VersionID);
        //拷贝完之后将版本+时间信息写入记录中
        string installFlag = AppSystemInfo.appInfo.versionName + AppSystemInfo.appInfo.lastUpdateTime;
        PlayerPrefs.SetString(GameConfig.appInstallFlag,installFlag);
    }
	public void ClientUpdateComplete(bool success,string filepath)
	{
		if(success)
		{
			ApplicationUtility.instance.InstallApp(filepath);
            //Application.Quit();
		}
		else
		{
			//TODO:处理失败
			GameDebug.LogError("客户端更新失败");
		}
	}
#endif
    //------------------------------------------------------------------------------------------
    /// <summary>
    /// 检测资源
    /// </summary>
    //public void CheckResource()
    //{
    //    string path = mLocalDataFolder + "/filelist.config";
    //    if (!File.Exists(path))
    //    {
    //        ///本地目录没有filelist，从服务器下载
    //        BehaviourUtil.StartCoroutine(LoadFileListFromServer());
    //    }
    //    else
    //    {
    //        FileStream stream = new FileStream(path, FileMode.Open);
    //        StreamReader reader = new StreamReader(stream);
    //        try
    //        {
    //            mLocalDataListInfo = JsonMapper.ToObject<Filelist>(reader.ReadToEnd());
    //        }
    //        catch (Exception exception)
    //        {
    //            string str2 = reader.ReadToEnd();
    //            if (str2 == null)
    //            {
    //                GameDebug.Log("reader.ReadToEnd() = null");
    //            }
    //            else
    //            {
    //                GameDebug.Log("reader.ReadToEnd().Len = " + str2.Length);
    //            }
    //            GameDebug.Log(reader.ReadToEnd());
    //            GameDebug.Log(exception.ToString());
    //            reader.Close();
    //            stream.Close();
    //            BehaviourUtil.StartCoroutine(LoadFileListFromServer());
    //            return;
    //        }
    //        reader.Close();
    //        stream.Close();

    //        if (!UpdateTool.CheckAllRes(mLocalDataListInfo))
    //        {
    //            BehaviourUtil.StartCoroutine(LoadFileListFromServer());
    //        }
    //        else
    //        {
    //            UpdateComplete(false);
    //        }
    //    }
    //}


    /// <summary>
    /// 从服务器下载文件列表
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadFileListFromServer()
    {
#region 更新miniClient

#if !UNITY_EDITOR && UNITY_ANDROID && False
        //鉴于应用宝平台有增量更新设置，此功能暂时屏蔽
        string appversion = GameConfig.ServerAppURL + GameConfig.AppVersionPath;

        WWW appvWWW = new WWW(appversion);
        yield return appvWWW;

        if (string.IsNullOrEmpty(appvWWW.error))
        {
            int serverversion = AppVersion.VersionID;
            bool versionok = true;

            serverversion = Convert.ToInt32(appvWWW.text);
			AppVersion.ServerAppVersion = serverversion;
            if (versionok)
            {
                if (serverversion != AppVersion.VersionID)
                {
                    //更新客户端
                    string appURL = GameConfig.ServerAppURL + GameConfig.AppPath;
                    UpdateTool.Instance.UpdateClient(appURL);
                    yield break;
                }
            }
        }
#endif
#endregion

        WWW filelistWWW = null;
        //获取服务器上当前的版本信息
        string sVPath = GameConfig.AssetServerURL + GameConfig.AssetVersion;

        filelistWWW = new WWW(sVPath);
        yield return filelistWWW;

        if (!string.IsNullOrEmpty(filelistWWW.error))
        {
            UpdateFailed();
            yield break;
        }
        AppVersion.assetVersion = JsonMapper.ToObject<AssetVersion>(filelistWWW.text);

        if (AppVersion.assetVersion == null || string.IsNullOrEmpty(AppVersion.assetVersion.version))
            AppVersion.assetVersion.version = AppSystemInfo.appInfo.versionName;


        string versiondata = GameConfig.GetRealSrvDataPath(AppVersion.assetVersion.version) + GameConfig.DataversionPath;
        filelistWWW = new WWW(versiondata);
        yield return filelistWWW;

        if (!string.IsNullOrEmpty(filelistWWW.error))
        {
            UpdateFailed();
            yield break;
        }

        //加载本地的config

        mServerRes = JsonMapper.ToObject<ResVersion>(filelistWWW.text);
        AssetParam param = new AssetParam();
        yield return AssetManager.instance.LoadResource("dataversion.config", param);
        if (param.asset == null || param.asset.Data == null) 
		{
			UpdateFailed();
			yield break;
		}

        string text = param.asset.Data.text;
        ResVersion localresv = JsonMapper.ToObject<ResVersion>(text);

        if(localresv == null || mServerRes == null)
        {
            UpdateComplete(false);
            NotifyProcess(1, "资源更新失败...");
            yield break;
        }
        //测试：走下面的流程
        if (mServerRes.md5 == localresv.md5) 
		{
			UpdateComplete(false);
			yield break;
		}


        NotifyProcess(0,"检查资源更新...");

        //服务器上的filelist
        string filelistpath = string.Format("{0}filelist.config", GameConfig.GetRealSrvDataPath(AppVersion.assetVersion.version));

        filelistWWW = new WWW(filelistpath);
        yield return filelistWWW;

        if(!string.IsNullOrEmpty(filelistWWW.error))
        {
            //TODO:处理加载失败

            UpdateFailed();
            yield break;
        }

        string configdata = filelistWWW.text;

        if(string.IsNullOrEmpty(configdata))
        {
            //有错误
            UpdateFailed(string.Empty);
            yield break;
        }

        mServerFileListInfo = JsonMapper.ToObject<Filelist>(configdata);

        filelistWWW.Dispose();


        if (mServerFileListInfo != null)
            ParseDownLoadAndUpdateData();
        else
            UpdateFailed(string.Empty);
    }

    protected void UpdateFailed(string param = "")
    {
        UpdateComplete(false);

    }

    public void UpdateComplete(bool saveResVer = true)
    {

        if (saveResVer && mServerRes != null)
        {
            string resversionPath = AssetManager.GetFileAssetBase(AssetManager.PathType.Path_Local) + GameConfig.DataversionPath;
            //要保存资源版本号:
            string localDataPath = Path.GetDirectoryName(resversionPath);
            if (!Directory.Exists(localDataPath))
                Directory.CreateDirectory(localDataPath);

            File.WriteAllText(resversionPath, JsonMapper.ToJson(mServerRes));
        }

        if (CompleteDelegate != null)
            CompleteDelegate();      
    }

    /// <summary>
    /// 解析下载并更新数据
    /// </summary>
    protected void ParseDownLoadAndUpdateData()
    {
        ///加载本地的.config
        string path = mLocalDataFolder + "filelist.config";

        if (!File.Exists(path))
        {
            if (mLocalDataListInfo == null)
            {
                mLocalDataListInfo = new Filelist();
            }
        }
        else
        {
            FileStream stream = new FileStream(path, FileMode.Open);
            StreamReader reader = new StreamReader(stream);
            try
            {
                mLocalDataListInfo = JsonMapper.ToObject<Filelist>(reader.ReadToEnd());
                reader.Close();
                stream.Close();
            }
            catch (Exception exception)
            {
                Debug.Log(exception.Message);
                string str2 = reader.ReadToEnd();
                if (str2 == null)
                {
                    Debug.Log("reader.ReadToEnd() = null");
                }
                else
                {
                    Debug.Log("reader.ReadToEnd().Len = " + str2.Length);
                }
                reader.Close();
                stream.Close();
                mLocalDataListInfo = new Filelist();
            }
        }

        ///对比本地文件列表与服务器列表数据
        UpdateTool.GetDiff(ref UpdateTool.Instance.mDownData, ref mRemoveData, mLocalDataListInfo, mServerFileListInfo);
        //统计总下载数据大小
        uint num = 0;
        for (int i = 0; i < UpdateTool.Instance.mDownData.Count; i++)
        {
            num += UpdateTool.Instance.mDownData[i].mSize;
        }

        ContinueDownLoad(num.ToString());
        
    }

    /// <summary>
    /// 继续下载
    /// </summary>
    /// <param name="param">需要下载的文件的大小</param>
    private void ContinueDownLoad(string param)
    {
        uint parameter = Convert.ToUInt32(param);
        int count = UpdateTool.Instance.mDownData.Count;
        if (count > 0)
        {
            //TODO:通知下载进度
            UpdateTool.Instance.cachePath = mLocalDataFolder + "temp/";
            UpdateTool.Instance.serverurl = GameConfig.GetRealSrvDataPath(AppVersion.assetVersion.version);
            UpdateTool.Instance.mLocalDataFolder = mLocalDataFolder;

            UpdateTool.Instance.OnUpdateComplete = new UpdateTool.UpdateDeglate(UpdateComplete);
            UpdateTool.Instance.nTotalDownCount = count;

            UpdateTool.Instance.DoUpdateSelf();
        }
        else
        {
            UpdateComplete(true);
        }
    }



    public void NotifyProcess(float value,string showname)
    {
        LoadingEvent evt = new LoadingEvent(LoadingEvent.LOADING_PROGRESS);
        evt.progress = (int)(value * 100);
        evt.showname = showname;

        EventSystem.Instance.PushEvent(evt);
    }

    public void SaveFileList()
    {
        GameDebug.Log("InstallData SaveFileList");
		string str = mLocalDataFolder + "filelist.config";
        if (mLocalDataListInfo != null)
        {
            string str2 = JsonMapper.ToJson(mLocalDataListInfo);
            FileStream stream = new FileStream(str, FileMode.Create);
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(str2);
            writer.Flush();
            writer.Close();
            stream.Close();
        }
    }
}

