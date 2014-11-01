using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using System.Text;
using FantasyEngine;

public class ResourceManager : Singleton<ResourceManager>
{

    public enum PathType
    {
        Path_Local,//本地目录
        Path_Streaming,//AssetBundle目录
    }

    //索引文件位置  对比FILELIST 后获取的最新文件位置
    private Hashtable mFiles = new Hashtable();

    private Hashtable mPrefabCaches = new Hashtable();

    public delegate void ReadTextCallback(string path, byte[] bytes);
    public delegate void LoadSceneCallback();

    public static string DownLoadPath
    {
        get
        {
            return Application.persistentDataPath + "/cache";
        }
    }

    //	public static string AssetPath
    //	{
    //		get
    //		{
    //			//编辑器模式
    //			#if UNITY_EDITOR
    //			return Application.dataPath + "/../../../Bin/StreamingAssets/";
    //			#elif UNITY_STANDALONE_WIN
    //			return Application.dataPath + "/../StreamingAssets/";
    //			#elif UNITY_ANDROID
    //			return Application.dataPath + "!/assets/";
    //			#elif UNITY_IPHONE
    //			return Application.dataPath + "/Raw/";
    //			#endif
    //		}
    //	}

    public static string CommonStreamingPath(string path)
    {

            //编辑器模式
#if UNITY_EDITOR
            return "file://" + Application.dataPath + "/../../../Bin/common/" + path;
#elif UNITY_STANDALONE_WIN
			return "file://" + Application.dataPath + "/../../common/" + path;
#elif UNITY_ANDROID
		return  AssetManager.GetRealPath("common/" + path);
#elif UNITY_IPHONE
		return AssetManager.GetRealPath("common/" + path);
#else
            return "";
#endif
    }
    private string CompositePath(string path)
    {
        string _path = AssetManager.GetRealPath(path);
        if (path[0] == '@' && path[1] == '/')
        {
            _path = CommonStreamingPath(path.Substring(2));
        }
        return _path;
    }


    public void LoadBytes(string path, ReadTextCallback callback)
    {
        BehaviourUtil.StartCoroutine(LoadBytes_impl(path, callback));
    }


    public IEnumerator LoadBytes_impl(string path, ReadTextCallback callback)
    {
        string pathName = CompositePath(path);

        WWW www = new WWW(pathName);
        yield return www;
        if (www.error != null)
        {
            //日志记录
            GameDebug.Log("File:" + pathName + "-Error:" + www.error);
            www.Dispose();

        }
        else
        {
            if (www.isDone)
            {
                if (callback != null)
                {
                    callback(path, www.bytes);
                }
            }

            www.Dispose();
        }
    }
    public GameObject LoadUI(string prefabName, Vector3 position)
    {
        Object prefab = null;
        if (!mPrefabCaches.ContainsKey(prefabName))
        {
            prefab = Resources.Load(prefabName);
            mPrefabCaches.Add(prefabName, prefab);
        }
        else
        {
            prefab = mPrefabCaches[prefabName] as Object;
        }

        if (prefab == null)
            return null;

        return (GameObject)UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity);
    }
    public GameObject LoadUI(string prefabName)
    {
        Object prefab = null;
        if (!mPrefabCaches.ContainsKey(prefabName))
        {
            prefab = Resources.Load(prefabName);
            mPrefabCaches.Add(prefabName, prefab);
        }
        else
        {
            prefab = mPrefabCaches[prefabName] as Object;
        }

        if (prefab == null)
            return null;

        return (GameObject)UnityEngine.Object.Instantiate(prefab);
    }

    public UIAtlas LoadAtlas(string prefabName)
    {
        UIAtlas atlas = Resources.Load(prefabName, typeof(UIAtlas)) as UIAtlas;
        return atlas;
    }

    public AudioClip LoadSoundClip(string path)
    {

        AudioClip audio = Resources.Load(("Sound/" + path).Replace("\\", "/")) as AudioClip;
        return audio;
    }
}
