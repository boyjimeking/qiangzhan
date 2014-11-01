using FantasyEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class UpdateTool : Singleton<UpdateTool>
{
    public delegate void UpdateDeglate(bool saveResVer = true);
    public UpdateDeglate OnUpdateComplete;

    public string serverurl;
    public string mLocalDataFolder;

    private int mTotalSize = 0;
    protected ResData[] mDownLoadingDataInfoArray;
    public List<ResData> mDownData = new List<ResData>();



    private int mDownLoadedIndex;
    //最大的下载次数
    public int mMaxDownLoadingCount = 4;
    protected int mHttpDownLoadCount;
    private int mDonwloadedCount;

    public int nTotalDownCount;


    private string mCachePath = string.Empty;

	private void CheckPoint(string name)
	{
		Debug.Log(name);
	}
#if !UNITY_EDITOR && UNITY_ANDROID
	/// <summary>
	/// 下载程序文件
	/// </summary>
	/// <param name="url"></param>
	public void UpdateClient(string url)
	{
		BehaviourUtil.StartCoroutine(HttpDownload(url));
	}

    public void ClientUpdateComplete(bool success,string filepath)
    {
        UpdateUtility.instance.ClientUpdateComplete(success, filepath);
    }

    /// <summary>
    /// HTTP下载文件，支持断点续传
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    IEnumerator HttpDownload(string url)
    {
        string strUpdatepositionName = "updateposition_" + AppVersion.VersionID;

        string curPosName = PlayerPrefs.GetString(GameConfig.appUpdateFlag);

        string filename = Path.GetFileName(url);
		string filepath = string.Format("{0}{1}", GameConfig.CachePath, filename);



        if(strUpdatepositionName != curPosName && PlayerPrefs.HasKey(curPosName))
        {
            PlayerPrefs.DeleteKey(curPosName);
            if (File.Exists(filepath))
                File.Delete(filepath);
        }
   

        int updateposition = PlayerPrefs.GetInt(strUpdatepositionName);

        if (updateposition > 0 && !File.Exists(filepath))
            updateposition = 0;
		if(updateposition == 0 && File.Exists(filepath))
			File.Delete(filepath);

        NotifyProgress(0, "开始更新客户端");

        yield return 1;

		Uri appUri = new Uri(Uri.EscapeUriString(url));
		HttpWebRequest request = WebRequest.Create(appUri) as HttpWebRequest;

        yield return request;

		HttpWebResponse response = request.GetResponse() as HttpWebResponse;
		yield return response;

		mTotalSize = (int)response.ContentLength;
		request.Abort();
		response.Close();

		CheckPoint(mTotalSize.ToString());
		//总大小
		if(mTotalSize == updateposition)
		{
			CheckPoint(request.ContentLength.ToString());
			//已经更新完成了提示用户安装即可
			//TODO:验证安装包的MD5

			NotifyProgress(1, "客户端更新完毕");
			ClientUpdateComplete(true,filepath);
			yield break;
		}
		//重新申请资源更新
		request = WebRequest.Create(appUri) as HttpWebRequest;
		response = request.GetResponse() as HttpWebResponse;
        request.AddRange(updateposition);
        response = request.GetResponse() as HttpWebResponse;
        yield return response;


		//mTotalSize = (int)response.ContentLength + updateposition;

        if(mTotalSize == 0)
        {
           GameDebug.LogError(string.Format("客户端版本{0}更新出现错误",AppVersion.VersionID));
           ClientUpdateComplete(false,filepath);
            yield break;
        }
        NotifyProgress((float)updateposition/mTotalSize, string.Format("更新客户端{0}byte/{1}byte..",updateposition,mTotalSize));
        yield return 1;

        byte[] bBuffer = new byte[65536];
        //开始接收

        Stream streambuff = response.GetResponseStream();

        //读取buffer
        while(true)
        {
            int nRealReadCount = streambuff.Read(bBuffer, 0, 65536);


            if (nRealReadCount <= 0)
            {
                ///无读取内容时
                if (updateposition == mTotalSize)
                {
                    ClientUpdateComplete(true,filepath);
                    break;
                }

                yield return 1;
            }


            string folder =  Path.GetDirectoryName(filepath);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }


            FileStream fileStream = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            try
            {
                fileStream.Position = updateposition;
                fileStream.Write(bBuffer, 0, nRealReadCount);
                fileStream.Flush();
                fileStream.Close();
                updateposition += nRealReadCount;
                //保存进度
                PlayerPrefs.SetInt(strUpdatepositionName, updateposition);


                NotifyProgress((float)updateposition / mTotalSize, string.Format("更新客户端{0}byte/{1}byte..", updateposition, mTotalSize));
                yield return 1;
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Dispose();
                }
            }

        }
    }
#endif
    protected ResData[] downLoadingDataInfoArray
    {
        get
        {
            if ((mDownLoadingDataInfoArray == null) || (mDownLoadingDataInfoArray.Length == 0))
            {
                mDownLoadingDataInfoArray = new ResData[mMaxDownLoadingCount];
                for (int i = 0; i < this.mMaxDownLoadingCount; i++)
                {
                    mDownLoadingDataInfoArray[i] = null;
                }
            }
            return mDownLoadingDataInfoArray;
        }
    }

    /// <summary>
    /// 是否正在下载
    /// </summary>
    private bool isDownLoading
    {
        get
        {
            for (int i = 0; i < downLoadingDataInfoArray.Length; i++)
            {
                if (downLoadingDataInfoArray[i] != null)
                {
                    return true;
                }
            }
            return false;
        }
    }


    private int downLoadIndex
    {
        get
        {
            for (int i = 0; i < downLoadingDataInfoArray.Length; i++)
            {
                if (downLoadingDataInfoArray[i] == null)
                {
                    return i;
                }
            }
            return -1;
        }
    }

    /// <summary>
    /// 是否全速加载
    /// </summary>
    private bool fullDownLoading
    {
        get
        {
            for (int i = 0; i < downLoadingDataInfoArray.Length; i++)
            {
                if (downLoadingDataInfoArray[i] == null)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public string cachePath
    {
        set
        {
            if (mCachePath != value)
            {
                mCachePath = value;
            }
        }
    }

    public void DoUpdateSelf()
    {
        if (mDownLoadedIndex >= 10)
        {
            //没下载x个文件保存一下列表防止中途中断更新
            mDownLoadedIndex = 0;
            SaveFileList();
        }
        if ((mDownData.Count <= 0) && !isDownLoading)
        {
            SaveFileList();
            if (OnUpdateComplete != null)
            {
                OnUpdateComplete(true);
                OnUpdateComplete = null;
            }
        }
        else
        {
            if (!Directory.Exists(mCachePath))
            {
                Directory.CreateDirectory(mCachePath);
            }
          
            while ((mDownData.Count > 0) && !fullDownLoading)
            {
                ///如果列表里有需要下载的东西，并且非全速下载
                ///执行一次下载
                int index = downLoadIndex;
                if (index == -1)
                {
                    Debug.LogError("DoUpdateSelf Error");
                    return;
                }
                ResData info = mDownData[0];
                if (info == null)
                {
                    Debug.LogError("DoUpdateSelf Error2");
                    return;
                }
                downLoadingDataInfoArray[index] = info;
                mDownData.RemoveAt(0);

                BehaviourUtil.StartCoroutine(WWWDownLoad(info.mDataPath, string.Format("{0}tmp{1}.ttdq", mCachePath, index), index));
            }
        }
    }

    private IEnumerator WWWDownLoad(string url, string filePath, int index)
    {
        ResData downLoadDataInfo = downLoadingDataInfoArray[index];
        if (downLoadDataInfo != null)
        {
            float time = Time.realtimeSinceStartup;
            string downUrl = serverurl + url;

            WWW www = new WWW(Uri.EscapeUriString(downUrl));
            yield return www;

            mDownLoadedIndex++;

            if (string.IsNullOrEmpty(www.error))
            {
                try
                {
                    byte[] bBuffer = www.bytes;
                    int nRealReadCount = bBuffer.Length;
                    FileStream filestream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                    try
                    {
                        filestream.Position = 0L;
                        filestream.Write(bBuffer, 0, nRealReadCount);
                        filestream.Flush();
                        filestream.Close();
                    }
                    finally
                    {
                        if (filestream != null)
                        {
                            filestream.Dispose();
                        }
                    }

                    //下载完之后开始文件拷贝
                    FileInfo finfo = new FileInfo(filePath);
                    string targetpath = string.Format("{0}{1}", mLocalDataFolder, downLoadDataInfo.mDataPath);
                    string directoryName = Path.GetDirectoryName(targetpath);
                    if (!Directory.Exists(directoryName))
                    {
                        Directory.CreateDirectory(directoryName);
                    }
                    if (File.Exists(targetpath))
                    {
                        File.Delete(targetpath);
                    }
                    finfo.MoveTo(targetpath);
                    FileDownLoaded(index.ToString());
                    GameDebug.Log("copy time = " + ((Time.realtimeSinceStartup - time)).ToString("0.000") + "s, url = " + downUrl);
                }
                catch (Exception exception)
                {
                    GameDebug.LogError("url = " + url + ", filePath = " + filePath + ",  " + exception.ToString());
                }
            }

        }
    }

    /// <summary>
    /// 文件下载成功
    /// </summary>
    /// <param name="param"></param>
    private void FileDownLoaded(string param)
    {
        int index = 0;
        try
        {
            index = Convert.ToInt32(param);
        }
        catch
        {
            GameDebug.LogError("FileDownLoaded Error1 param = " + param);
            return;
        }
        if ((index < 0) || (index >= mDownLoadingDataInfoArray.Length))
        {
            GameDebug.LogError("FileDownLoaded Error2 param = " + param);
        }
        else
        {
            ResData info = mDownLoadingDataInfoArray[index];
            if (downLoadingDataInfoArray == null)
            {
                Debug.LogError("FileDownLoaded Error3 param = " + param);
            }
            else
            {
                UpdateUtility.Instance.mLocalDataListInfo.AddResData(info);
                mDownLoadingDataInfoArray[index] = null;
                mHttpDownLoadCount = 0;
                mDonwloadedCount++;

                NotifyProgress((float)mDonwloadedCount / nTotalDownCount,string.Format("检查资源更新{0}/{1}..." ,mDonwloadedCount,nTotalDownCount));

                //TODO:发送通知

                DoUpdateSelf();
            }
        }
    }

    //public static bool CheckAllRes(Filelist localListInfo)
    //{
    //    string ResPath = AssetManager.StreamingBase;
    //    if (localListInfo == null)
    //    {
    //        return false;
    //    }

    //    Dictionary<string, ResData> dataList = localListInfo.filelist;
    //    foreach (KeyValuePair<string, ResData> respair in dataList)
    //    {
    //        if (!File.Exists(ResPath + respair.Value.mDataPath))
    //        {
    //            return false;
    //        }
    //    }

    //    return true;
    //}


    /// <summary>
    /// 文件差异
    /// </summary>
    /// <param name="downLoadList"></param>
    /// <param name="removeList"></param>
    /// <param name="localListInfo"></param>
    /// <param name="serverListInfo"></param>
    public static void GetDiff(ref List<ResData> downLoadList, ref List<ResData> removeList, Filelist localListInfo, Filelist serverListInfo)
    {

        ///检查更新
        foreach (KeyValuePair<string, ResData> serverfile in serverListInfo.filelist)
        {
            if (!localListInfo.filelist.ContainsKey(serverfile.Key))
            {
                downLoadList.Add(serverfile.Value);
                continue;
            }
            if (!localListInfo.filelist[serverfile.Key].mDataMD5.Equals(serverfile.Value.mDataMD5))
            {
                downLoadList.Add(serverfile.Value);
                continue;
            }
        }

        //检查需要移除的资源
        foreach (KeyValuePair<string, ResData> localfile in localListInfo.filelist)
        {
            if (!serverListInfo.filelist.ContainsKey(localfile.Key))
            {
                removeList.Add(localfile.Value);
            }
        }

    }

    /// <summary>
    /// 保存文件列表
    /// </summary>
    private void SaveFileList()
    {

        UpdateUtility.Instance.SaveFileList();
    }

    public void NotifyProgress(float value,string showname)
    {
		UpdateUtility.instance.NotifyProcess(value, showname);
    }

    public void NotifyUpdateComplete()
    {

        if (OnUpdateComplete != null)
        {
            OnUpdateComplete(true);
            OnUpdateComplete = null;
        }
     
    }
}

