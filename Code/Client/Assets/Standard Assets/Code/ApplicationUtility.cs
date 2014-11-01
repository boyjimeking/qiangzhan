#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY)
#define MOBILE
#endif


using System;
using System.IO;
using UnityEngine;
/// <summary>
/// 应用程序的拓展类
/// </summary>
public class ApplicationUtility
{
   
    protected static ApplicationUtility mInst;

#if !UNITY_EDITOR && UNITY_ANDROID
    private AndroidJavaObject m_Obj_Activity;

    public void CopyAssets(string[] files, string sdcardPath,bool copyFile)
    {
        if (files == null)
            return;

        foreach(string file in  files)
        {
            int fileCount = ApplicationUtility.instance.BeginCopyDataFiles(file);

            int flag = 0;
            while (flag < fileCount)
            {
                string filePath = ApplicationUtility.instance.GetCopyFile(flag);

                string destPath = sdcardPath + filePath;

                if (copyFile)
                {
                    if (!File.Exists(destPath))
                        ApplicationUtility.instance.CopyAFile(filePath, destPath);
                }
				flag++;
            }
        }
    }

    public int BeginCopyDataFiles(string assetDir)
    {
        if (m_Obj_Activity == null)
        {
            this.m_Obj_Activity = new AndroidJavaObject("com.tencent.tmgp.ttdq.ApplicationUtility", new object[0]);
            Debug.Log("Debug.Log( m_Obj_Activity )");
        }
        object[] args = new object[] { assetDir };
        return this.m_Obj_Activity.Call<int>("BeginCopyDataFiles", args);
    }

    public int CheckFont()
    {
        return this.m_Obj_Activity.Call<int>("GetFontResult", new object[0]);
    }

    /// <summary>
    /// 拷贝一个文件
    /// </summary>
    /// <param name="srcFile"></param>
    /// <param name="destFile"></param>
    public void CopyAFile(string srcFile, string destFile)
    {
        if (m_Obj_Activity == null)
        {
            m_Obj_Activity = new AndroidJavaObject("com.tencent.tmgp.ttdq.ApplicationUtility", new object[0]);
            Debug.Log("Debug.Log( m_Obj_Activity )");
        }
        object[] args = new object[] { srcFile, destFile };
        this.m_Obj_Activity.Call("CopyAFile", args);
    }

    public void CopyAssets(string assetDir, string destDir)
    {
        if (this.m_Obj_Activity == null)
        {
            this.m_Obj_Activity = new AndroidJavaObject("com.tencent.tmgp.ttdq.ApplicationUtility", new object[0]);
            Debug.Log("Debug.Log( m_Obj_Activity )");
        }
        object[] args = new object[] { assetDir, destDir };
        this.m_Obj_Activity.Call("CopyAssets", args);
    }

    public bool FileExist(string srcPath)
    {
        if (this.m_Obj_Activity == null)
        {
            this.m_Obj_Activity = new AndroidJavaObject("com.tencent.tmgp.ttdq.ApplicationUtility", new object[0]);
            Debug.Log("Debug.Log( m_Obj_Activity )");
        }
        object[] args = new object[] { srcPath };
        return this.m_Obj_Activity.Call<bool>("FileExist", args);
    }

    public string GetAndroidResPath()
    {
        string bundleId = this.GetVersion();
        bundleId = bundleId.Substring(bundleId.LastIndexOf(".") + 1);
        return string.Format("{0}/youxigu/{1}/", GetSDCardPath(), bundleId);
    }

    public string PackageInfo()
    {
        if (m_Obj_Activity == null)
        {
            m_Obj_Activity = new AndroidJavaObject("com.tencent.tmgp.ttdq.ApplicationUtility", new object[0]);
            Debug.Log("Debug.Log( m_Obj_Activity )");
        }
        return m_Obj_Activity.Call<string>("PackageInfo", new object[0]);
    }

    public string GetAppName()
    {
        if (m_Obj_Activity == null)
        {
            m_Obj_Activity = new AndroidJavaObject("com.tencent.tmgp.ttdq.ApplicationUtility", new object[0]);
            Debug.Log("Debug.Log( m_Obj_Activity )");
        }
        return this.m_Obj_Activity.Call<string>("GetAppName", new object[0]);
    }

    public string GetVersion()
    {
        if (m_Obj_Activity == null)
        {
            m_Obj_Activity = new AndroidJavaObject("com.tencent.tmgp.ttdq.ApplicationUtility", new object[0]);
            Debug.Log("Debug.Log( m_Obj_Activity )");
        }
        return this.m_Obj_Activity.Call<string>("GetVersion", new object[0]);
    }

    public string GetCopyFile(int index)
    {
        if (this.m_Obj_Activity == null)
        {
            this.m_Obj_Activity = new AndroidJavaObject("com.tencent.tmgp.ttdq.ApplicationUtility", new object[0]);
            Debug.Log("Debug.Log( m_Obj_Activity )");
        }
        object[] args = new object[] { index };
        return this.m_Obj_Activity.Call<string>("GetNeedCopyFileName", args);
    }

    public string GetSDCardPath()
    {
        if (this.m_Obj_Activity == null)
        {
            this.m_Obj_Activity = new AndroidJavaObject("com.tencent.tmgp.ttdq.ApplicationUtility", new object[0]);
            Debug.Log("Debug.Log( m_Obj_Activity )");
        }
        return m_Obj_Activity.Call<string>("GetSDCardPath", new object[0]);
    }

    public bool HasDataFile(string path)
    {
        object[] args = new object[] { path };
        return m_Obj_Activity.Call<bool>("DataExist", args);
    }

    public void HitHard()
    {
        m_Obj_Activity.Call("HitHard", new object[0]);
    }

    public bool Init()
    {
        return InitAndroidPlugin();
    }

    private bool InitAndroidPlugin()
    {
        if (m_Obj_Activity == null)
        {
            m_Obj_Activity = new AndroidJavaObject("com.tencent.tmgp.ttdq.ApplicationUtility", new object[0]);
        }
        if (!m_Obj_Activity.Call<bool>("Init", new object[0]))
        {
            return false;
        }
        return true;
    }

    public void Install(string file)
    {
        object[] args = new object[] { file };
        this.m_Obj_Activity.Call("InstallApk", args);
    }

    public void InstallApp(string appPath)
    {
        if (this.m_Obj_Activity == null)
        {
            this.m_Obj_Activity = new AndroidJavaObject("com.tencent.tmgp.ttdq.ApplicationUtility", new object[0]);
            Debug.Log("Debug.Log( m_Obj_Activity )");
        }
        object[] args = new object[] { appPath };
        this.m_Obj_Activity.Call("InstallApp", args);
    }

    public void OpenUrl(string url)
    {
        if (m_Obj_Activity == null)
        {
            m_Obj_Activity = new AndroidJavaObject("com.tencent.tmgp.ttdq.ApplicationUtility", new object[0]);
            Debug.Log("Debug.Log( m_Obj_Activity )");
        }
        object[] args = new object[] { url };
        this.m_Obj_Activity.Call("OpenUrl", args);
    }

    public static ApplicationUtility instance
    {
        get
        {
            if (mInst == null)
            {
                mInst = new ApplicationUtility();
                mInst.m_Obj_Activity = new AndroidJavaObject("com.tencent.tmgp.ttdq.ApplicationUtility", new object[0]);
            }
            return mInst;
        }
    }



    public void AndroidCancleProgressDialog()
    {
        if (m_Obj_Activity == null)
        {
            m_Obj_Activity = new AndroidJavaObject("com.tencent.tmgp.ttdq.ApplicationUtility", new object[0]);
            Debug.Log("Debug.Log( m_Obj_Activity )");
        }
        m_Obj_Activity.Call("CancleProgressDialog", new object[0]);
    }

    public void AndroidMess(string titleInfo, string messageInfo, string positiveButton, string delFun)
    {
        if (this.m_Obj_Activity == null)
        {
            this.m_Obj_Activity = new AndroidJavaObject("com.tencent.tmgp.ttdq.ApplicationUtility", new object[0]);
            Debug.Log("Debug.Log( m_Obj_Activity )");
        }
        Debug.Log("Mess1");
        object[] args = new object[] { titleInfo, messageInfo, positiveButton, delFun };
        this.m_Obj_Activity.Call("Mess", args);
        Debug.Log("Mess2");
    }

    public void AndroidMessDoubleButton(string titleInfo, string messageInfo, string cancelButton, string otherButton, string cancelFun, string otherFun)
    {
        if (this.m_Obj_Activity == null)
        {
            this.m_Obj_Activity = new AndroidJavaObject("com.tencent.tmgp.ttdq.ApplicationUtility", new object[0]);
            Debug.Log("Debug.Log( m_Obj_Activity )");
        }
        Debug.Log(string.Format("MessDoubleButton1 cancelFun = {0}, otherFun = {1}", cancelFun, otherFun));
        object[] args = new object[] { titleInfo, messageInfo, cancelButton, otherButton, cancelFun, otherFun };
        this.m_Obj_Activity.Call("MessDoubleButton", args);
        Debug.Log("MessDoubleButton2");
    }

    public void AndroidShowProgressDialog(string titleInfo, string messageInfo)
    {
        if (this.m_Obj_Activity == null)
        {
            this.m_Obj_Activity = new AndroidJavaObject("com.tencent.tmgp.ttdq.ApplicationUtility", new object[0]);
            Debug.Log("Debug.Log( m_Obj_Activity )");
        }
        object[] args = new object[] { titleInfo, messageInfo };
        this.m_Obj_Activity.Call("ShowProgressDialog", args);
    }
#endif
}
