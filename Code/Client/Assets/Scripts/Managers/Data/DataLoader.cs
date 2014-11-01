using System;
using System.IO;
using UnityEngine;

public class DataLoader : Singleton<DataLoader>
{
    public WWW mWWWdata;

    public void CloseData()
    {
        if (this.mWWWdata != null)
        {
            this.mWWWdata.assetBundle.Unload(true);
            this.mWWWdata = null;
        }
    }

    public static string GetDataPath()
    {
        string path = string.Empty;
        string loadPath = LoadDataConfig.LoadPath;
        if (loadPath != null)
        {
            path = "file:///" + loadPath;
            if (!File.Exists(path))
            {
                path = string.Empty;
            }
        }
        if (path == string.Empty)
        {
            path = Application.persistentDataPath;
            path = "file:///" + path + "/data.pkg";
        }
        return path;
    }

    public static string GetRealDataPath()
    {
        return GetDataPath().Substring(8);
    }

    public object Load(string filename)
    {
        if (this.mWWWdata == null)
        {
            return null;
        }
        return this.mWWWdata.assetBundle.Load(filename);
    }
	
	public object loadFromLocal(string filename)
	{
		return Resources.Load(filename);
	}

    public void RemoveData()
    {
        this.CloseData();
        string realDataPath = GetRealDataPath();
        if (File.Exists(realDataPath))
        {
            File.Delete(realDataPath);
        }
    }

    public void UnLoad()
    {
        this.mWWWdata.assetBundle.Unload(true);
    }
}

