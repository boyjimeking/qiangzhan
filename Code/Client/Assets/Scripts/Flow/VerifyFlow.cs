using FantasyEngine;
using UnityEngine;

//版本检测流程

public class VerifyFlow : BaseFlow 
{
    private bool mGoToNext = false;


    bool BaseFlow.Init()
    {
        LoadingManager.Instance.ShowLoading();

#if UNITY_ANDROID && !UNITY_EDITOR
        AssetManager.useLocal = true;

        //进行资源校验
        UpdateUtility.Instance.CompleteDelegate = OnUpdateComplete;
        UpdateUtility.Instance.PlatformUpdate();
#else
        OnUpdateComplete();
#endif


    

        return false;
    }

    /// <summary>
    /// 资源更新完成
    /// </summary>
    void OnUpdateComplete()
    {
        NotifyProcess(0,"加载数据资源");
        GameConfig.SyncInit(onConfigCallback);
        EngineInitialize.Init();

        LightShadowManager.Instance.InitRoleLight();

        CustomMaterialManager.Instance.Init();
    }

    void onConfigCallback()
    {
        NotifyProcess(0.3f, "加载数据资源");
        //加载资源数据表
        DataManager.Instance.SyncInit(onDataCallback);
    }

    void onDataCallback()
    {
        NotifyProcess(0.7f, "加载数据资源");
        InitDataStruct();

        UIResourceManager.Instance.Init(onFontsInit);
    }

    void onFontsInit()
    {
        mGoToNext = true;

        NotifyProcess(0.9f, "加载数据资源");
        WindowManager.Instance.CloseUI("loading");
    }

    private void InitDataStruct()
    {
        DropManager.Instance.InitDataStruct();
        StageDataManager.Instance.InitDataStruct();
    }

    public void NotifyProcess(float value, string showname)
    {
        LoadingEvent evt = new LoadingEvent(LoadingEvent.LOADING_PROGRESS);
        evt.progress = (int)(value * 100);
        evt.showname = showname;

        EventSystem.Instance.PushEvent(evt);
    }

    bool BaseFlow.Term()
    {
        return false;
    }
    GAME_FLOW_ENUM BaseFlow.GetFlowEnum()
    {
        return GAME_FLOW_ENUM.GAME_FLOW_VERIFY;
    }
    FLOW_EXIT_CODE BaseFlow.Update(uint elapsed)
    {
        if (mGoToNext)
        {
            mGoToNext = false;
            return FLOW_EXIT_CODE.FLOW_EXIT_CODE_NEXT;
        }
        return FLOW_EXIT_CODE.FLOW_EXIT_CODE_NONE;
    }
}
