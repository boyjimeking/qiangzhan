using FantasyEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


/// <summary>
/// 光影管理器
/// </summary>
public class LightShadowManager : Singleton<LightShadowManager>
{

    private GameObject mRoleLight;

    public void InitRoleLight()
    {

        //AssetParam param = new AssetParam();
        //param.listener += OnRoleLightComplete;
        //ResourceManager.instance.LoadResource(AssetConfig.RolightPath, param);

    }

    private void OnRoleLightComplete(AssetData asset)
    {
       UnityEngine.Object lightObj =  asset.Instantiate();
       mRoleLight = lightObj as GameObject;
       if (mRoleLight != null)
           UnityEngine.Object.DontDestroyOnLoad(mRoleLight);
    }

}

