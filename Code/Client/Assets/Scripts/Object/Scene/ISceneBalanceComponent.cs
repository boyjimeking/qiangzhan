
using UnityEngine;
using System.Collections;


public class SceneBalanceComponent
{
    virtual protected void GenerateAward()
    {

    }

	virtual public void Balance()
	{

	}

    virtual protected void OpenBalanceUI()
    {
		
    }

	virtual protected void CloseBalanceUI()
    {
		
    }

	virtual public void Destroy()
	{

	}
}

// 主线结算
public class StageSceneBalanceComponent : SceneBalanceComponent
{
	// 场景
	private StageScene mScene = null;

    // stage表
    private Scene_StageSceneTableItem mRes = null;

	// 结算统计
	private StageEndModule mEndModule = ModuleManager.Instance.FindModule<StageEndModule>();

	// 翻牌
	private StageBalanceModule mBalanceModule = ModuleManager.Instance.FindModule<StageBalanceModule>();

    public StageSceneBalanceComponent(StageScene scene)
    {
		mScene = scene;

		mRes = mScene.GetStageRes();

		EventSystem.Instance.addEventListener(StageEndUIEvent.STAGE_END_FINISH, OnStageEndFinish);
    }

	public override void Destroy()
	{
		mScene = null;
		mRes = null;
		mEndModule = null;

		EventSystem.Instance.removeEventListener(StageEndUIEvent.STAGE_END_FINISH, OnStageEndFinish);
	}

    override protected void GenerateAward()
    {

    }

	override public void Balance()
	{
		if(mScene.GetResult() > 0)
		{
			mScene.DropPassAward();

			uint passtime = mScene.GetLogicRunTime();

			StageGrade grade = StageGrade.StageGrade_Invalid;

			if (passtime < mRes.mTimeS)
			{
				grade = StageGrade.StageGrade_S;
			}
			else if (passtime < mRes.mTimeA)
			{
				grade = StageGrade.StageGrade_A;
			}
			else if (passtime < mRes.mTimeB)
			{
				grade = StageGrade.StageGrade_B;
			}
			else
			{
				grade = StageGrade.StageGrade_C;
			}

			mEndModule.SetPassTime(passtime);
			mEndModule.SetGrade(grade);
			mEndModule.SetExp(mRes.mAwardExp);
			mBalanceModule.SetPassTime(passtime);
			mBalanceModule.SetGrade(grade);

			OpenEndUI();
		}
		else
		{
			OpenFailedUI();
		}

		SceneManager.Instance.StartTimeScale(0.2f, 3000);
		CameraController.Instance.PlayCameraEffect(6.0f, mScene.GetCameraInfo().z, 0.8f);
	}

	private void OpenEndUI()
	{
		WindowManager.Instance.OpenUI("stageend");
	}

	private void CloseEndUI()
	{
		WindowManager.Instance.CloseUI("stageend");
	}

	private void OpenFailedUI()
	{
		WindowManager.Instance.OpenUI("stagefailed");
	}

    override protected void CloseBalanceUI()
    {
		WindowManager.Instance.CloseUI("stagebalance");
    }

	override protected void OpenBalanceUI()
	{
		WindowManager.Instance.OpenUI("stagebalance");
    }

	private void OnStageEndFinish(EventBase evt)
	{
		CloseEndUI();
		OpenBalanceUI();
	}
}

public class TowerSceneBlanceComponent : SceneBalanceComponent
{
	// 场景
	private StageScene mScene = null;

    // stage表
    private Scene_StageSceneTableItem mRes = null;

    public TowerSceneBlanceComponent(StageScene scene)
    {
		mScene = scene;
        mRes = mScene.GetStageRes();
		EventSystem.Instance.addEventListener(StageEndUIEvent.STAGE_END_FINISH, onGameOver);
    }

	public override void Destroy()
	{
		mScene = null;
		mRes = null;
		EventSystem.Instance.removeEventListener(StageEndUIEvent.STAGE_END_FINISH, onGameOver);
	}

    public void onGameOver(EventBase ev)
    {

    }

	override public void Balance()
	{
        var scn = SceneManager.Instance.GetCurScene() as StageScene;
        int result = scn.GetResult();
      
        //0失败，1 成功
        var mModule = ModuleManager.Instance.FindModule<ChallengeModule>();
        scn.StopTrigger("tf" + mModule.GetDoingFloor());
        WindowManager.Instance.CloseUI("challengecountdown");
        //WindowManager.Instance.CloseUI("countDown");

        if (result == 0)
        {
            ModuleManager.Instance.FindModule<ChallengeModule>().ChallengeFail();
        }
        else
        {
            var evt = new TowerPassEvent(TowerPassEvent.TOWER_PASS) { mfloor = mModule.GetDoingFloor() };
            EventSystem.Instance.PushEvent(evt);

            scn.StopBgSound();
            //重置复活次数
            scn.ResetReliveTimes();
            //重置佣兵复活次数
            scn.ResetCropsReliveTimes();
            //设置定身
            PlayerController.Instance.SetFreeze(true);

            ChallengeStageOverStageActionParam param = new ChallengeStageOverStageActionParam();
            param.Floor = (uint)ModuleManager.Instance.FindModule<ChallengeModule>().GetDoingFloor();
            param.ConsumeTime = scn.GetLogicRunTime();
            Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_CHALLENGE_STAGE_END, param);

            mModule.SetDoingFloor(mModule.GetDoingFloor() + 1);
        }
      
	}

    protected override void CloseBalanceUI()
    {
        WindowManager.Instance.CloseUI("challengeDrop");
    }

    protected override void OpenBalanceUI()
    {
       
    }

   
    protected override void GenerateAward()
    {
        
    }

   
}


public class MonsterFloodBlanceComponent : StageSceneBalanceComponent
{
    public MonsterFloodBlanceComponent(StageScene scene) : base(scene)
    {
    }

    public override void Balance()
    {
        base.Balance();
        ModuleManager.Instance.FindModule<MonsterFloodModule>().Reset();
    }
}

// 塔防结算
public class TDBalanceComponent : StageSceneBalanceComponent
{
	public TDBalanceComponent(StageScene scene) : base(scene)
	{

	}

	override public void Balance()
	{
		WindowManager.Instance.CloseUI("tdinfo");

		base.Balance();
	}
}


// 压制邪恶结算
public class YaZhiXieESceneBalanceComponent : SceneBalanceComponent
{
	// 场景
	private YaZhiXieEScene mScene = null;

	// scene表
	private SceneTableItem mRes = null;

	public YaZhiXieESceneBalanceComponent(YaZhiXieEScene scene)
	{
		mScene = scene;

		mRes = mScene.GetSceneRes();
	}

	public override void Destroy()
	{
		mScene = null;
		mRes = null;
	}

	override public void Balance()
	{
		WindowManager.Instance.CloseUI("yzxerankinfo");
	}
}
