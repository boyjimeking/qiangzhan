using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using FantasyEngine;
using System;

public class SceneManager
{
	private static SceneManager instance;
 
	// 当前场景
	private BaseScene mCurScene  = null;

	// 摄像机控制器
	private CameraController mCameraControl = null;

	// 场景工厂
	private Dictionary<SceneType, SceneFactory> mFactorys = null;

	// 场景是否加载中
	private bool mSceneLoading = false;

	// 上个城镇
	private int mLastCityResId = -1;

	// 时间缩放计时
	private int mTimeScaleTimer = 0;

	// 原时间缩放设置
	private float mRestoreTimeScale = 1.0f;

    /// <summary>
    /// 倒计时时间
    /// </summary>
    private int mCountDownTime;

  
	public SceneManager()
	{
		instance = this;

		mCameraControl = new CameraController();

        InitSceneFacrotys();

        InitEventListeners();

	}

	public static SceneManager Instance
	{
		get
		{
			return instance;
		}
	}

    public bool IsCurSceneType(SceneType type)
    {
         if (mCurScene == null)
             return false;

         return mCurScene.getType() == type;
    }

    public bool RequestEnterScene(int resID)
    {
        //出现逻辑错误
        if (mSceneLoading)
            return false;

        if (!DataManager.SceneTable.ContainsKey(resID))
            return false;

        if (DataManager.Scene_StageSceneTable.ContainsKey(resID))
        {
            if (!StageDataManager.Instance.CheckEnterStage(resID))
                return false;

			if (!CheckSuitableFC(resID))
				return false;

			SendEnterStageRequest(resID);

            return true;
        }
       
        EnterScene(resID);
        return true;
    }

	private void SendEnterStageRequest(object obj)
	{
		EnterSceneActionParam param = new EnterSceneActionParam();
		param.scenetype = SceneType.SceneType_Stage;
		param.sceneid = (int)obj;
		Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_SCENE_ENTER, param);
	}

	// 允许进入场景
	public BaseScene EnterScene(int resID)
	{
        if (!DataManager.SceneTable.ContainsKey(resID))
            return null;

        SceneTableItem scnRes = DataManager.SceneTable[resID] as SceneTableItem;    

		if (mCurScene != null)
		{
			mCurScene.Destroy();
			mCurScene = null;
		}

        BaseScene scn = CreateScene(SceneManager.GetSceneType(scnRes), resID);
		if (null == scn)
		{
			return null;
		}

        EngineInitialize.SwitchLevel();

		if (!scn.LoadScene())
		{
			return null;
		}

		mSceneLoading = true;

		mCurScene = scn;

        WindowManager.Instance.CloseAllUI();
		//打开加载界面
		WindowManager.Instance.OpenUI("loading");

        EventSystem.Instance.PushEvent(new StageEnterEvent(StageEnterEvent.STAGE_ENTER_RESPOND_EVENT, SceneManager.GetSceneType(scnRes), resID));

		return mCurScene;
	}
 
	//场景加载完成
    public void onSceneLoadCall(EventBase e)
    {
		mSceneLoading = false;

        if( !(mCurScene is CityScene) )
        {
            WindowManager.Instance.OpenUI("bloodstains");
        }

        //清理掉摄像机的上个场景的一些信息
        CameraController.Instance.Clear();

        //TODO:临时这样做
        //RenderSettings.ambientLight = new Color(177/255f,179/255f,186/255f,255);
        RenderSettings.ambientLight = Color.white;
        //获取场景中的摄像机动画信息
        CameraPathAnimator[] camerapaths = GameObject.FindObjectsOfType<CameraPathAnimator>();

        mCurScene.GetCameraPathManager().InitPath(camerapaths);


        DynamicAnimation[] dyanimation = GameObject.FindObjectsOfType<DynamicAnimation>();

		mCurScene.InitDynamicAnim(dyanimation);

        ObjectBase ply = mCurScene.CreateMainPlayer();
		if( ply == null )
        {
            GameDebug.Log("主角创建失败");
            return;
        }

        mCurScene.CreateCrops();

       
		//控制当前场景的摄像机
		GameObject camera = GameObject.Find("Main Camera");

		//控制当前主摄像机
        //TODO:什么时候需要再打开，名字自定义
        CameraController.Instance.SetControlCamera(camera,null/* mCurScene.GetCameraPathManager().GetPath("FollowPath")*/);
		//摄像机跟随玩家
		CameraController.Instance.SetLookAT(ply);

        Vector3 info = mCurScene.GetCameraInfo();
        CameraController.Instance.SetCameraInfo(info.x, info.y, info.z);
        GetCurScene().PlayNormalBgSound();
    }

    public BaseScene GetCurScene()
    {
        return mCurScene;
    }

	public void Update(uint elapsed)
	{
        elapsed = (uint)(elapsed * Time.timeScale);

		if(mTimeScaleTimer > 0)
		{
			mTimeScaleTimer -= (int)elapsed;
			if(mTimeScaleTimer <= 0)
			{
				ResumeTimeScale();
			}
		}
	     
		if( mCurScene != null )
		{
            mCurScene.Update(elapsed);
		}

		if( mCameraControl != null )
		{
			mCameraControl.Update();
		}
	}

    // 初始化场景工厂
    private void InitSceneFacrotys()
    {
		mFactorys = new Dictionary<SceneType, SceneFactory>();

        AddSceneFctory(new CitySceneFactory());
        AddSceneFctory(new StageSceneFactory());
        AddSceneFctory(new BattleSceneFactory());
        AddSceneFctory(new TowerStageSceneFactory());
        AddSceneFctory(new MonsterFloodStageSceneFactory());
        AddSceneFctory(new ZombiesStageSceneFactory());
        AddSceneFctory(new QiangLinDanYuSceneFactory());
		AddSceneFctory(new ArenaSceneFactory());
		AddSceneFctory(new QualifyingSceneFactory());
        AddSceneFctory(new MaoStageSceneFactory());
		AddSceneFctory(new WantedStageSceneFactory());
		AddSceneFctory(new TDSceneFactory());
		AddSceneFctory(new YaZhiXieESceneFactory());
		AddSceneFctory(new ZhaoCaiMaoSceneFactory());
    }

    // 初始化事件监听
    private void InitEventListeners()
    {
        EventSystem.Instance.addEventListener(SceneLoadEvent.SCENE_LOAD_COMPLETE, onSceneLoadCall);
    }

    // 添加一个场景工厂
    public void AddSceneFctory(SceneFactory factory)
    {
        if (mFactorys.ContainsKey(factory.GetSceneType()))
        {
            GameDebug.LogError("SceneManager contains same SceneFactory!");
            return;
        }

        mFactorys.Add(factory.GetSceneType(), factory);
    }

    // 移除一个场景工厂
    private void RemoveSceneFactory(SceneType type)
    {
        mFactorys.Remove(type);
    }

    // 创建场景
    public BaseScene CreateScene(SceneType type, int resId)
    {
        if (!mFactorys.ContainsKey(type))
            return null;

        return (mFactorys[type] as SceneFactory).CreateScene(resId);
    }

    public void DestroyCurrentScene()
    {
        if (mCurScene == null)
        {
            return;
        }
        mCurScene.Destroy();
        mCurScene = null;

    }

    public bool InvokeFunction(string name)
    {
        if(mCurScene == null)
        {
            return false;
        }

        return mCurScene.InvokeFunction(name);
    }

    public bool StartTrigger(string name)
    {
        if(mCurScene == null)
        {
            return false;
        }

        return mCurScene.StartTrigger(name);
    }


    public bool StopTrigger(string name)
    {
        if(mCurScene == null)
        {
            return false;
        }

        return mCurScene.StopTrigger(name);
    }

    public bool RestartTrigger(string name)
    {
        if (mCurScene == null)
        {
            return false;
        }

        return mCurScene.RestartTrigger(name);
    }

	public void SetLastCityResId(int id)
	{
		mLastCityResId = id;

		PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        PlayerPrefs.SetInt(module.getGUID().ToString() + "lastcityid",mLastCityResId);
	}

    public int GetLastCityResId()
    {
		PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();

		if (!PlayerPrefs.HasKey(module.getGUID().ToString() + "lastcityid"))
        {
            return GameConfig.BeginCityID;
        }

		mLastCityResId = PlayerPrefs.GetInt(module.getGUID().ToString() + "lastcityid");
        return mLastCityResId;
    }

    public void RequestEnterLastCity()
	{
		EnterScene(GetLastCityResId());
	}

    //获得关卡剩余时间
    public int GetLastTime()
    {
        var passTime= (GetCurScene() as GameScene).GetLogicRunTime();
        if (passTime > mCountDownTime)
        {
            return 0;
        }
        return (mCountDownTime - (int)passTime);
    }
    public void SetCountDown(int time)
    {
         mCountDownTime = time;
    }

    public int GetCountDown()
    {
        return mCountDownTime;
    }

	// 场景暂停
	public void LogicPause()
	{
// 		if(mCurScene == null)
// 		{
// 			return;
// 		}
// 
// 		if (!mCurScene.GetType().IsSubclassOf(typeof(GameScene)))
// 		{
// 			return;
// 		}
// 
// 		GameScene scene = mCurScene as GameScene;
// 		scene.LogicPause();
		SetTimeScale(0.0f);
	}

	// 场景恢复
	public void LogicResume()
	{
// 		if (mCurScene == null)
// 		{
// 			return;
// 		}
// 
// 		if (!mCurScene.GetType().IsSubclassOf(typeof(GameScene)))
// 		{
// 			return;
// 		}
// 
// 		GameScene scene = mCurScene as GameScene;
// 		scene.LogicResume();
		ResumeTimeScale();
	}

	// 启动一段计时缩放
	public void StartTimeScale(float scale, int time)
	{
		SetTimeScale(scale);

		mTimeScaleTimer = (int)(scale * time);
	}

	// 设置时间缩放
	public void SetTimeScale(float scale)
	{
		mRestoreTimeScale = Time.timeScale;
		Time.timeScale = scale;
	}

	// 恢复时间缩放
	public void ResumeTimeScale()
	{
		Time.timeScale = mRestoreTimeScale;
	}

   
    static public SceneType GetSceneType(int resId)
    {
		if (resId < 100000)
			return SceneType.SceneType_City;
		else if (resId < 200000)
			return SceneType.SceneType_Stage;
		else if (resId < 300000)
			return SceneType.SceneType_MonsterFlood;
		else if (resId < 400000)
			return SceneType.SceneType_Tower;
		else if (resId < 500000)
			return SceneType.SceneType_Zombies;
		else if (resId < 600000)
			return SceneType.SceneType_Battle;
		else if (resId < 700000)
			return SceneType.SceneType_QiangLinDanYu;
		else if (resId < 800000)
			return SceneType.SceneType_Arena;
		else if (resId < 900000)
			return SceneType.SceneType_Qualifying;
		else if (resId < 1000000)
			return SceneType.SceneType_Mao;
		else if (resId < 1100000)
			return SceneType.SceneType_Resource;
		else if (resId < 1200000)
			return SceneType.SceneType_HunNeng;
		else if (resId < 1300000)
			return SceneType.SceneType_Wanted;
		else if (resId < 1400000)
			return SceneType.SceneType_TD;
		else if (resId < 1500000)
			return SceneType.SceneType_YaZhiXieE;
		else if (resId < 1600000)
			return SceneType.SceneType_ZhaoCaiMao;
		else
			return SceneType.SceneType_Invaild;
    }

    static public SceneType GetSceneType(SceneTableItem res)
    {
        if (res == null)
            return SceneType.SceneType_Invaild;

        return GetSceneType(res.resID);
    }

	public bool CheckSuitableFC(int stageId)
	{
		PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
		if (module == null)
			return false;

		Scene_StageSceneTableItem ssti = DataManager.Scene_StageSceneTable[stageId] as Scene_StageSceneTableItem;
		if (ssti == null)
			return false;

		if (ssti.mSuitableFC > 0)
		{
			if ((int)module.GetGrade() < ssti.mSuitableFC)
			{
				YesOrNoBoxManager.Instance.ShowYesOrNoUI("危险警告", "您的战斗力小于推荐值，继续挑战可能会失败，是否继续？", SendEnterStageRequest, stageId);
				return false;
			}
		}

		return true;
	}
}
