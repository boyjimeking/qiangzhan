using UnityEngine;
using System.Collections;

/// <summary>
/// 资源本副本结算;
/// 1.活动力扣除是在成功进入副本就扣;
/// 2.进入次数扣除是在成功通关副本，进入结算界面才扣(失败不扣);
/// </summary>
public class ActiveSceneBalanceComponent : SceneBalanceComponent
{
    // 场景;
    protected StageScene mScene = null;

    // stage表;
    protected Scene_StageSceneTableItem mRes = null;

    // 结算统计;
    protected StageEndModule mEndModule = ModuleManager.Instance.FindModule<StageEndModule>();

    // 翻牌;
    protected StageBalanceModule mBalanceModule = ModuleManager.Instance.FindModule<StageBalanceModule>();

    public ActiveSceneBalanceComponent(StageScene scene)
    {
        mScene = scene;
        mRes = mScene.GetStageRes();

        EventSystem.Instance.addEventListener(StageEndUIEvent.STAGE_END_FINISH, onGameOver);

        Init();
    }

    public override void Balance()
    {
        base.Balance();

        var scn = SceneManager.Instance.GetCurScene() as StageScene;

        uint score = scn.GetSceneScore();
        int result = scn.GetResult();
        scn.StopBgSound();

        //设置定身;
        PlayerController.Instance.SetFreeze(true);
        //清怪;
        //scn.DestroyCurGrowthTrigger();

        if (mScene.GetResult() > 0)
        {
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
    }

    protected override void CloseBalanceUI()
    {
        WindowManager.Instance.CloseUI("stagebalance");
    }

    protected override void OpenBalanceUI()
    {
        WindowManager.Instance.OpenUI("stagebalance");
    }

    protected override void GenerateAward()
    {

    }

    public override void Destroy()
    {
        mScene = null;
        mRes = null;
        mEndModule = null;
        EventSystem.Instance.removeEventListener(StageEndUIEvent.STAGE_END_FINISH, onGameOver);
    }

    public virtual void Init()
    {
 
    }

    public virtual void OpenEndUI()
    {
        WindowManager.Instance.OpenUI("stageend");
    }

    public virtual void CloseEndUI()
    {
        WindowManager.Instance.CloseUI("stageend");
    }

    public virtual void OpenFailedUI()
    {
        WindowManager.Instance.OpenUI("stagefailed");
    }

    public virtual void onGameOver(EventBase ev)
    {
        CloseEndUI();
        OpenBalanceUI();
        //		WindowManager.Instance.CloseUI("countDown");
    }
}
