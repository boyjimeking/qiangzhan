using FantasyEngine;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomMaterialManager : Singleton<CustomMaterialManager>
{

    private Dictionary<string, Material> materials = new Dictionary<string,Material>();

    private AssetPtr mMaterialAssetPtr;
    public void Init()
    {

        AssetParam param = new AssetParam();
        param.listener += OnAssetComplete;

        AssetManager.instance.LoadResource(AssetConfig.CustomMaterialPath, param);
    }
    private void OnAssetComplete(AssetPtr asset)
    {
        mMaterialAssetPtr = asset;
        UnityEngine.Object[] objs = asset.Data.LoadAll();

        foreach(UnityEngine.Object obj in objs)
        {
            if(obj is Material)
            {
                materials.Add(obj.name, obj as Material);
            }
        }

        asset.Data.UnloadWebStream();
    }

    public Material GetMaterial(string name)
    {
        if(string.IsNullOrEmpty(name))
            return null;
        if (!materials.ContainsKey(name))
            return null;
        return materials[name];
    }
}

