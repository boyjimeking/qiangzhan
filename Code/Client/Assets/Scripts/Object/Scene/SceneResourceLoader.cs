
using FantasyEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneResourceLoader
{
    public string name;
    public SceneLoadComplete LoadComplete;

    private List<AssetPtr> mAssetList = new List<AssetPtr>();
    /// <summary>
    /// 加载场景
    /// </summary>
    /// <param name="name"></param>
    public void LoadScene(string name, SceneLoadComplete completedelegate,bool additive = false)
    {
        //Debug.Log("名字为" + name);
        SceneData data = SceneConfigManager.Instance.GetSceneData(name);
        if (data == null && completedelegate != null)
        {
            completedelegate();
            return;
        }

        LoadComplete = completedelegate;
        BehaviourUtil.StartCoroutine(LoadScene_impl(data, additive));
    }

    /// <summary>
    /// 加载场景
    /// </summary>
    /// <param name="data"></param>
    private IEnumerator LoadScene_impl(SceneData data, bool additive = false)
    {
        AssetParam param = null;
        //加载依赖资源
        foreach (string depres in data.resources)
        {

            param = new AssetParam();

            yield return AssetManager.Instance.LoadResource(depres, param);
            if (param.asset != null)
            {
                AssetBundle bundle = param.asset.Data.assetBundle;
                mAssetList.Add(param.asset);
            }

        }


        string scnFile = FantasyEngine.AssetConfig.LevelPath + data.name + FantasyEngine.AssetConfig.AssetSuffix;
        //param = new AssetParam();
        //yield return AssetManager.Instance.LoadResource(scnFile, param);

        //这里临时处理一下先不走AssetManger 现在的内存释放机制还没有做好
        WWW www = new WWW(AssetManager.GetRealPath(scnFile));
        yield return www;

        AssetBundle bundled = www.assetBundle;
        AsyncOperation sync = null;
        if (additive)
        {
            sync = Application.LoadLevelAdditiveAsync(data.name);
          
        }
        else
        {
            sync = Application.LoadLevelAsync(data.name);
        }

        yield return sync;

        if(www.assetBundle != null)
            www.assetBundle.Unload(false);
        www.Dispose();

        if (LoadComplete != null)
            LoadComplete();
    }
}
public delegate void SceneLoadComplete();
