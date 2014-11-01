using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FantasyEngine;
using LitJson;
using Object = UnityEngine.Object;

public class DownAtlasAssetParam : AssetParam
{
    public int max_count = 0;
    public int down_count = 0;
}

public class AtlasReferenceData
{
    public int reference_count = 0;

    public List<AssetPtr> resources = new List<AssetPtr>();
}

public class WindowReferenceData
{
    public int reference_count = 0;

    public AssetPtr ptr = null;
}

//UI资源管理器
public class UIResourceManager 
{
    //
    private static string FONTS_ASSET_FILE = "UI/Font/fonts.ab";
    private static string UI_CONFIG_FILE = "UI/config.config";

    private static string UI_PATH = "UI/";

    private static UIResourceManager instance = null;

    public delegate void FunctionCallback();
    public delegate void LoadUICallback(GameObject obj);
    public delegate void LoadAnimationCallback(string aniname);

    //private Hashtable mLoads = new Hashtable();

    private Dictionary<string, DownAtlasAssetParam> mDownAtlasCaches = new Dictionary<string, DownAtlasAssetParam>();

    private Dictionary<string, ArrayList> mLoadCallbacks = new Dictionary<string, ArrayList>();

    //界面图集缓存
    private Dictionary<string, UIAtlas> mAtlasCaches = new Dictionary<string, UIAtlas>();

    //字体的图集缓存
    private Dictionary<string, UIAtlas> mFontAtlasCaches = new Dictionary<string, UIAtlas>();

    private FunctionCallback mInitCallback = null;


    private UIConfig mConfig = null;        //UI/config.config

    private Dictionary<string, AssetPtr> mTempAssetsHolders = new Dictionary<string, AssetPtr>();

    private Dictionary<string, AtlasReferenceData> mAtlasReferences = new Dictionary<string, AtlasReferenceData>();

    private Dictionary<string, WindowReferenceData> mWindowReferences = new Dictionary<string, WindowReferenceData>();

    private List<AssetPtr> mDynamicFonts = new List<AssetPtr>();

    public static UIResourceManager Instance
    {
        get
        {
            return instance;
        }
    }
	public UIResourceManager()
    {
        instance = this;
    }
    public void Init(FunctionCallback calback)
    {
        mInitCallback = calback;
        BehaviourUtil.StartCoroutine(InitConfigAndFonts());
    }
    IEnumerator InitConfigAndFonts()
    {
        AssetParam param = new AssetParam();
        yield return AssetManager.Instance.LoadResource(UI_CONFIG_FILE, param);

        if (param.asset != null)
        {
            mConfig = JsonMapper.ToObject<UIConfig>(param.asset.Data.text);
            if (!mTempAssetsHolders.ContainsKey(param.asset.Data.url))
                mTempAssetsHolders.Add(param.asset.Data.url, param.asset);
            if (mConfig == null)
            {
                GameDebug.LogError("界面配置信息文件 " + UI_CONFIG_FILE + "读取失败");
            }
        }
        List<AssetPtr> depresList = new List<AssetPtr>();
        ///加载字体依赖的贴图
        foreach(KeyValuePair<string,AtlasData> fontpair in mConfig.AtlasDatas)
        {
            if(fontpair.Value.fontAtlas)
            {
                AssetParam atlasparam = new AssetParam();

                foreach(string texdep in fontpair.Value.texs)
                {
                    yield return AssetManager.Instance.LoadResource(UI_PATH+"Texture/"+texdep+AssetConfig.AssetSuffix, atlasparam);
                    depresList.Add(atlasparam.asset);
                }
            }
        }


        foreach (string fontres in mConfig.fontRes)
        {

            AssetParam fontresparam = new AssetParam();

            yield return AssetManager.Instance.LoadResource(UI_PATH + "Texture/" + fontres + AssetConfig.AssetSuffix, fontresparam);

            //depresList.Add(fontresparam.asset);
            mDynamicFonts.Add(fontresparam.asset);

        }

        yield return AssetManager.Instance.LoadResource(FONTS_ASSET_FILE, param);

        if (param.asset != null)
        {
            if (!mTempAssetsHolders.ContainsKey(param.asset.Data.url))
                mTempAssetsHolders.Add(param.asset.Data.url, param.asset);
            Object[] objs = param.asset.Data.LoadAll();

            //字体里的纹理集 不走界面纹理集的引用销毁机制
            if( objs != null )
            {
                for (int i = 0; i < objs.Length; ++i)
                {
                    Object o = objs[i];
                    if (o is GameObject)
                    {
                        GameObject go = o as GameObject;
                        UIAtlas uias = go.GetComponent<UIAtlas>();
                        if (uias == null)
                        {
                            continue;
                        }
                        if (mFontAtlasCaches.ContainsKey(uias.name))
                        {
                            continue;
                        }
                        mFontAtlasCaches.Add(uias.name, uias);
                    }
                }
            }
        }

        foreach (AssetPtr ptr in depresList)
        {
            if (!mTempAssetsHolders.ContainsKey(ptr.Data.url))
                mTempAssetsHolders.Add(ptr.Data.url, ptr);
            ptr.Data.UnloadWebStream();
        }
        depresList.Clear();
        if( mInitCallback != null )
        {
            mInitCallback();
        }
    }

    public void CacheUIAtlas(Object[] objs)
    {
        if( objs == null )
        {
            return;
        }
        //将纹理集缓存起来 备用
        for (int i = 0; i < objs.Length; ++i)
        {
            Object o = objs[i];
            if (o is GameObject)
            {
                GameObject go = o as GameObject;
                UIAtlas uias = go.GetComponent<UIAtlas>();
                if (uias == null)
                {
                    continue;
                }
                if (mAtlasCaches.ContainsKey(uias.name))
                {
                    continue;
                }
                mAtlasCaches.Add(uias.name, uias);
            }
        }
    }

    public UIAtlas GetAtlas(string atlasName)
    {
        if (mAtlasCaches.ContainsKey(atlasName))
            return mAtlasCaches[atlasName];
        if (mFontAtlasCaches.ContainsKey(atlasName))
            return mFontAtlasCaches[atlasName];
        return null;
    }
    private bool CheckAtlasComplete(string atlas)
    {
        int start = atlas.LastIndexOf("/");
        int end = atlas.LastIndexOf( '.' );
        string name = atlas.Substring(start + 1, end - start - 1);

        if (mAtlasCaches.ContainsKey(name))
            return true;
        return false;
    }

    private void RemoveAtlas(string atlasName)
    {
        int start = atlasName.LastIndexOf("/");
        int end = atlasName.LastIndexOf('.');
        string name = atlasName.Substring(start + 1, end - start - 1);

        if (!mAtlasCaches.ContainsKey(name))
            return ;
        mAtlasCaches.Remove(name);
    }

    IEnumerator DownloadUI(string prefabPath, string prefabName, List<string> refAtlas)
    {
        AssetParam refParam = new AssetParam();


        List<string> wtoa = new List<string>();
        foreach (string atlas in refAtlas)
        {
            if (!mAtlasReferences.ContainsKey(atlas))
            {
                mAtlasReferences.Add(atlas, new AtlasReferenceData());
            }

            //引用计数 +1
            mAtlasReferences[atlas].reference_count += 1;



            wtoa.Add(UI_PATH + "Atlas/" + atlas + ".ab");

            if( CheckAtlasComplete( atlas ) )
            {
                continue;
            }

            AtlasData atlasData = mConfig.AtlasDatas[atlas];
            List<AssetPtr> depresList = new List<AssetPtr>();
            ///加载字体依赖的贴图

            AssetParam atlasparam = new AssetParam();

            foreach (string texdep in atlasData.texs)
            {
                yield return AssetManager.Instance.LoadResource(UI_PATH + "Texture/" + texdep + AssetConfig.AssetSuffix, atlasparam);
                depresList.Add(atlasparam.asset);
            }

            yield return AssetManager.Instance.LoadResource(UI_PATH + "Atlas/" + atlas + ".ab", refParam);
            if (refParam.asset != null)
            {
                mAtlasReferences[atlas].resources.Add(refParam.asset);

                UnityEngine.Object[] objs = refParam.asset.Data.LoadAll();
                CacheUIAtlas(objs);
            }

            foreach(AssetPtr ptr in depresList)
            {
                ptr.Data.UnloadWebStream();

                mAtlasReferences[atlas].resources.Add( ptr );
            }
        }

        string downName = UI_PATH + prefabPath + prefabName + ".ab";


        AssetManager.Instance.RecordAssociate(downName, wtoa.ToArray());

        AssetParam param = new AssetParam();
        yield return AssetManager.Instance.LoadResource(downName, param);
        if (param.asset == null)
            yield break;

        if (!mWindowReferences.ContainsKey( prefabName ))
        {
            WindowReferenceData data = new WindowReferenceData();
            data.ptr = param.asset;
            mWindowReferences.Add(prefabName, data);
        }

//         if (!mTempAssetsHolders.ContainsKey(param.asset.Data.url))
//             mTempAssetsHolders.Add(param.asset.Data.url, param.asset);
        param.asset.Data.LoadAll();
        OnLoadComplete(prefabName, param);
    }

    IEnumerator DownloadAnimationRes(string prefabName, AnimationData data, LoadAnimationCallback callback, string aniName)
    {
        AssetParam refParam = new AssetParam();
        foreach (string atlas in data.refAtlas)
        {
            if (!mAtlasReferences.ContainsKey(atlas))
            {
                mAtlasReferences.Add(atlas, new AtlasReferenceData());
            }

            //引用计数 +1
            mAtlasReferences[atlas].reference_count += 1;

            if (CheckAtlasComplete(atlas))
            {
                continue;
            }

            AtlasData atlasData = mConfig.AtlasDatas[atlas];
            List<AssetPtr> depresList = new List<AssetPtr>();
            ///加载字体依赖的贴图

            AssetParam atlasparam = new AssetParam();

            foreach (string texdep in atlasData.texs)
            {
                yield return AssetManager.Instance.LoadResource(UI_PATH + "Texture/" + texdep + AssetConfig.AssetSuffix, atlasparam);
                depresList.Add(atlasparam.asset);
            }


            yield return AssetManager.Instance.LoadResource(UI_PATH + "Atlas/" + atlas + ".ab", refParam);
            if (refParam.asset != null)
            {
                mAtlasReferences[atlas].resources.Add(refParam.asset);

//                 if (!mTempAssetsHolders.ContainsKey(refParam.asset.Data.url))
//                     mTempAssetsHolders.Add(refParam.asset.Data.url, refParam.asset);

                UnityEngine.Object[] objs = refParam.asset.Data.LoadAll();
                CacheUIAtlas(objs);
            }

            foreach (AssetPtr ptr in depresList)
            {
                ptr.Data.UnloadWebStream();
                mAtlasReferences[atlas].resources.Add(ptr);

//                 if (!mTempAssetsHolders.ContainsKey(ptr.Data.url))
//                     mTempAssetsHolders.Add(ptr.Data.url, ptr);
            }
        }
        OnAniLoadComplete(prefabName, callback, aniName);
    }

    private void OnLoadComplete(string prefabName, AssetParam param)
    {
        if (!mLoadCallbacks.ContainsKey(prefabName))
            return;
        ArrayList list = mLoadCallbacks[prefabName];

        for (int i = 0; i < list.Count; ++i )
        {
            LoadUICallback callback = list[i] as LoadUICallback;
            if( callback != null )
            {
                GameObject obj = param.asset.Data.Instantiate() as GameObject;
                obj.SetActive(false);
                callback( obj );
            }
        }
        mLoadCallbacks[prefabName].Clear();
        mLoadCallbacks.Remove(prefabName);
    }

    private void OnAniLoadComplete(string prefabName, LoadAnimationCallback callback, string aniName)
    {
        if( callback != null )
        {
            callback(aniName);
        }
    }

    public bool LoadAnimObject(string _prefab , LoadUICallback callback)
    {
        string prefabName = _prefab + ".prefab";
        if (!mConfig.WinDatas.ContainsKey(prefabName))
        {
            //这里尝试使用 Resource.Load 来载入Resource下的预设
            GameDebug.Log("包里没有找到UI " + prefabName);
            return false;
        }

        //加入等待回调
        if (!mLoadCallbacks.ContainsKey(prefabName))
        {
            mLoadCallbacks.Add(prefabName, new ArrayList());
            mLoadCallbacks[prefabName].Add(callback);

            WindowData data = mConfig.WinDatas[prefabName] as WindowData;

            BehaviourUtil.StartCoroutine(DownloadUI("Anim/", prefabName, data.refAtlas));
        }
        else
        {
            mLoadCallbacks[prefabName].Add(callback);
        }
        return true;
    }

    //下载UI  会将引用的纹理集载入
    public bool LoadUI(string _prefab ,LoadUICallback callback )
    {
        string prefabName = _prefab + ".prefab";

        if (!mConfig.WinDatas.ContainsKey(prefabName))
        {
            //这里尝试使用 Resource.Load 来载入Resource下的预设
            GameDebug.Log("包里没有找到UI " + prefabName);
            return false;
        }

        //加入等待回调
        if (!mLoadCallbacks.ContainsKey(prefabName))
        {
            mLoadCallbacks.Add(prefabName, new ArrayList());
            mLoadCallbacks[prefabName].Add(callback);

            WindowData data = mConfig.WinDatas[prefabName] as WindowData;

            BehaviourUtil.StartCoroutine(DownloadUI("Window/", prefabName, data.refAtlas));
        }else
        {
            mLoadCallbacks[prefabName].Add(callback);
        }
        return true;
    }

    public bool LoadAnimationRes(string aniName, LoadAnimationCallback callback)
    {
        string prefabName = aniName + ".prefab";

        if (!mConfig.AniDatas.ContainsKey(prefabName))
        {
            GameDebug.Log("包里没有找到Animation " + prefabName);
            return false;
        }

        //加入等待回调
//         if (!mAniLoadCallbacks.ContainsKey(prefabName))
//         {
//             mAniLoadCallbacks.Add(prefabName, new ArrayList());
//         }
//         mAniLoadCallbacks[prefabName].Add(callback);

        AnimationData data = mConfig.AniDatas[prefabName] as AnimationData;

        BehaviourUtil.StartCoroutine(DownloadAnimationRes(prefabName, data, callback, aniName));

        return true;
    }


    public bool UnLoadUI(string _prefab)
    {
        string prefabName = _prefab + ".prefab";

        if (!mConfig.WinDatas.ContainsKey(prefabName))
        {
            GameDebug.Log("UnLoadUI 包里没有找到UI " + prefabName);
            return false;
        }

        //清理资源
        WindowData data = mConfig.WinDatas[prefabName] as WindowData;
        foreach (string atlas in data.refAtlas)
        {
            if (!mAtlasReferences.ContainsKey(atlas))
            {
                continue;
            }
            mAtlasReferences[atlas].reference_count -= 1;
            if (mAtlasReferences[atlas].reference_count <= 0)
            {
                mAtlasReferences[atlas].resources.Clear();
                mAtlasReferences.Remove(atlas);

                RemoveAtlas(atlas);
            }
        }

        //清理UI
        if (mWindowReferences.ContainsKey(prefabName))
        {
            mWindowReferences.Remove(prefabName);
        }

        return true;
    }

    public GameObject CloneGameObject(GameObject obj)
    {
        GameObject clone = (GameObject)GameObject.Instantiate(obj);
        return clone;
    }
    
}
