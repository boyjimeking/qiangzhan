

// 复活组件

using Message;
using UnityEngine;

public class SceneReliveComponent
{
	virtual public void requestRelive()
	{

	}

	virtual public void Destroy()
	{

	}
}

// 主线本复活
public class StageSceneReliveComponent : SceneReliveComponent
{
	// 已复活次数
	protected int mReliveTimes = 0;

	// 场景
	private StageScene mScene = null;

	// 表
	Scene_StageSceneTableItem mRes = null;

	private StageReliveModule mModule = ModuleManager.Instance.FindModule<StageReliveModule>();

	public StageSceneReliveComponent(StageScene scene)
    {
		mScene = scene;

		mRes = mScene.GetStageRes();

		EventSystem.Instance.addEventListener(StageReliveEvent.STAGE_RELIVE_REQUEST, onReliveRequest);
		EventSystem.Instance.addEventListener(StageReliveEvent.STAGE_RELIVE_RESPOND, onReliveRespond);
    }

	// 玩家死亡
	public override void requestRelive()
	{
		base.requestRelive();

		if (mRes.mReliveTimes >= 0 && mReliveTimes >= mRes.mReliveTimes)
		{
			mScene.SetResult(0);
			mScene.pass();

			return;
		}

		StageReliveModule module = ModuleManager.Instance.FindModule<StageReliveModule>();
		if (module == null)
		{
			return;
		}

		module.setReliveData(mRes.mReliveTimes - mReliveTimes, ConditionManager.Instance.GetConditionRequiredValue(mRes.mReliveCostId0), ConditionManager.Instance.GetConditionIcon(mRes.mReliveCostId0),
			ConditionManager.Instance.GetConditionRequiredValue(mRes.mReliveCostId1), ConditionManager.Instance.GetConditionIcon(mRes.mReliveCostId1));

		OpenReliveUI();
	}

	// 打开复活界面
	private void OpenReliveUI()
	{
		WindowManager.Instance.OpenUI("stagerelive");
	}

	// 请求复活
	private void onReliveRequest(EventBase evt)
	{
		StageReliveEvent e = evt as StageReliveEvent;
		if(e == null)
		{
			return;
		}

		if(e.mReliveType == ReliveType.ReliveType_Normal)
		{
			if (!ConditionManager.Instance.CheckCondition(mRes.mReliveCostId0))
			{
				PopTipManager.Instance.AddNewTip(StringHelper.GetErrorString(ERROR_CODE.ERR_SCENE_RELIVE_FAILED_NOCOST, FontColor.Red));
				return;
			}

		}
		else if(e.mReliveType == ReliveType.ReliveType_Extra)
		{
			if (!ConditionManager.Instance.CheckCondition(mRes.mReliveCostId1))
			{
				PopTipManager.Instance.AddNewTip(StringHelper.GetErrorString(ERROR_CODE.ERR_SCENE_RELIVE_FAILED_NOCOST, FontColor.Red));
				return;
			}
		}

		mModule.WaitRelive = true;

		WindowManager.Instance.CloseUI("stagerelive");

		ReliveActionParam param = new ReliveActionParam();
		param.scenetype = SceneType.SceneType_Stage;
		param.sceneid = mRes.resID;
		param.relivetype = (int)e.mReliveType;
		Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_SCENE_RELIVE, param);
	}

	// 复活答复
	private void onReliveRespond(EventBase evt)
	{
		StageReliveEvent e = evt as StageReliveEvent;
		if (e == null)
		{
			return;
		}

		Player player = PlayerController.Instance.GetControlObj() as Player;
		if (player == null)
		{
			return;
		}

		mReliveTimes++;
		if(e.mReliveType == ReliveType.ReliveType_Normal)
		{
			player.Relive((int)(player.GetMaxHP() * GameConfig.NormalReliveRate), (int)(player.GetMaxMana() * GameConfig.NormalReliveRate));
		}
		else if(e.mReliveType == ReliveType.ReliveType_Extra)
		{
			player.Relive((int)(player.GetMaxHP() * GameConfig.ExtraReliveRate), (int)(player.GetMaxMana() * GameConfig.ExtraReliveRate));
		}
	}

    //重设复活次数
    public void ResetReliveTime()
    {
        mReliveTimes = 0;
    }

	// 销毁
	public override void Destroy()
	{
		mScene = null;

		mRes = null;

		EventSystem.Instance.removeEventListener(StageReliveEvent.STAGE_RELIVE_REQUEST, onReliveRequest);
		EventSystem.Instance.removeEventListener(StageReliveEvent.STAGE_RELIVE_RESPOND, onReliveRespond);
	}
}

// 佣兵复活
public class CropsReliveComponent : SceneReliveComponent
{ 
    // 主佣兵已复活次数
	protected int mMainCropsReliveTimes = 0;

    // 副佣兵已复活次数
    protected int mSubCropsReliveTimes = 0;

	// 场景
	private BaseScene mScene = null;

	// 表
	SceneTableItem mRes = null;

    public CropsReliveComponent(GameScene scene)
    {
		mScene = scene;

        mRes = mScene.GetSceneRes();

        EventSystem.Instance.addEventListener(CropsEvent.MAIN_CROPS_RELIVE_REQUEST, onMainCropsReliveRequest);
        EventSystem.Instance.addEventListener(CropsEvent.SUB_CROPS_RELIVE_REQUEST, onSubCropsReliveRequest);
		EventSystem.Instance.addEventListener(CropsEvent.MAIN_CROPS_RELIVE_RESPOND, onMainCropsReliveRespond);
        EventSystem.Instance.addEventListener(CropsEvent.SUB_CROPS_RELIVE_RESPOND, onSubCropsReliveRespond);
    }

	// 佣兵死亡
	public override void requestRelive()
	{
		base.requestRelive();
        if ((PlayerController.Instance.GetMainCropsControlObj() as Crops).IsDead())
            EventSystem.Instance.PushEvent(new CropsEvent(CropsEvent.MAIN_CROPS_RELIVE_TIME_DOWN));
        if ((PlayerController.Instance.GetSubCropsControlObj() as Crops).IsDead())
            EventSystem.Instance.PushEvent(new CropsEvent(CropsEvent.SUB_CROPS_RELIVE_TIME_DOWN));
	}

	// 请求复活
    private void onMainCropsReliveRequest(EventBase evt)
	{
        EventSystem.Instance.PushEvent(new CropsEvent(CropsEvent.MAIN_CROPS_RELIVE_RESPOND));
	}

    private void onSubCropsReliveRequest(EventBase evt)
    {
        EventSystem.Instance.PushEvent(new CropsEvent(CropsEvent.SUB_CROPS_RELIVE_RESPOND));
    }

	// 复活答复
    private void onMainCropsReliveRespond(EventBase evt)
    {
        CropsEvent e = evt as CropsEvent;
        if (e == null)
        {
            return;
        }

        Crops mMainCrops = PlayerController.Instance.GetMainCropsControlObj() as Crops;
        if (mMainCrops == null)
        {
            return;
        }
        if (mMainCropsReliveTimes < mRes.mCropsReliveTimes)
        {
            mMainCrops.Relive((int)mMainCrops.GetMaxHP(),(int)mMainCrops.GetMaxMana());
            mMainCropsReliveTimes++;
        }
    }

    private void onSubCropsReliveRespond(EventBase evt)
    {
        CropsEvent e = evt as CropsEvent;
        if (e == null)
        {
            return;
        }

        Crops mSubCrops = PlayerController.Instance.GetSubCropsControlObj() as Crops;
        if (mSubCrops == null)
        {
            return;
        }

        if (mSubCropsReliveTimes < mRes.mCropsReliveTimes)
        {
            mSubCrops.Relive((int)mSubCrops.GetMaxHP(), (int)mSubCrops.GetMaxMana());
            mSubCropsReliveTimes++;
        }

    }
    //重设复活次数
    public void ResetReliveTime()
    {
        mMainCropsReliveTimes = 0;
        mSubCropsReliveTimes = 0;
    }

	// 销毁
	public override void Destroy()
	{
		mScene = null;

		mRes = null;

        EventSystem.Instance.removeEventListener(CropsEvent.MAIN_CROPS_RELIVE_REQUEST, onMainCropsReliveRequest);
        EventSystem.Instance.removeEventListener(CropsEvent.SUB_CROPS_RELIVE_REQUEST, onSubCropsReliveRequest);
        EventSystem.Instance.removeEventListener(CropsEvent.MAIN_CROPS_RELIVE_RESPOND, onMainCropsReliveRespond);
        EventSystem.Instance.removeEventListener(CropsEvent.SUB_CROPS_RELIVE_RESPOND, onSubCropsReliveRespond);
	}

    public bool CropsCanRelive()
    {
        return MainCropsCanRelive() || SubCrosCanRelive();
    }

    public bool MainCropsCanRelive()
    {
        Crops mMainCrops = PlayerController.Instance.GetMainCropsControlObj() as Crops;
        if (mMainCrops == null)
        {
            return false;
        }
        if (mMainCropsReliveTimes < mRes.mCropsReliveTimes)
        {
            return true;
        }

        return false;
    }

    public bool SubCrosCanRelive()
    {
        Crops mSubCrops = PlayerController.Instance.GetSubCropsControlObj() as Crops;
        if (mSubCrops == null)
        {
            return false;
        }
        if (mSubCropsReliveTimes < mRes.mCropsReliveTimes)
        {
            return true;
        }

        return false;
    }
}
