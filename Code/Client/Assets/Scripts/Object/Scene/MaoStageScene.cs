using UnityEngine;
using System.Collections;

public class MaoStageSceneInitParam : StageSceneInitParam
{
}

public class MaoStageScene : StageScene
{
	private int mCurPickCount = 0;
	private int mMaxPickCount = 0;

	private int mGoldId1 = -1;
	private int mGoldId2 = -1;
	private int mGoldId3 = -1;

    public MaoStageScene()
    {

    }

    override public bool Init(BaseSceneInitParam param)
	{
		if (!base.Init(param))	
		   return false;

		mBalanceComponent = new ActiveSceneBalanceComponent(this);
		mShowPickGuide = true;
		mMaxPickCount = GameConfig.MaoMaxGoldCount;
		mGoldId1 = GameConfig.MaoGoldId1;
		mGoldId2 = GameConfig.MaoGoldId2;
		mGoldId3 = GameConfig.MaoGoldId3;

		return true;
	}

	protected override void OnSceneInited()
	{
		base.OnSceneInited();

        EventSystem.Instance.addEventListener(MaoStageSucceedEvent.MAO_STAGE_SCENE_SUCCEED_EVENT, OnSucceed);
        WindowManager.Instance.OpenUI("maostagebattle");
	}

    protected override void OnSceneDestroy()
    {
        base.OnSceneDestroy();

        EventSystem.Instance.removeEventListener(MaoStageSucceedEvent.MAO_STAGE_SCENE_SUCCEED_EVENT, OnSucceed);
    }

    private void OnSucceed(EventBase e)
    {
        SetResult(1);
        pass();
    }

	public override void OnPick(ObjectBase pick, ObjectBase picker)
	{
		base.OnPick (pick, picker);

        if (pick == null || picker == null)
            return;

        Pick obj = pick as Pick;

        if (obj == null)
            return;

        PickTableItem pti = obj.GetCurPickTableItem();
        if (pti == null)
            return;

		if (!ObjectType.IsPlayer(picker.Type))
			return;

		if (pti.resID == mGoldId1 || pti.resID == mGoldId2 || pti.resID == mGoldId3)
		{
			if (mCurPickCount >= mMaxPickCount)
				return;

			mCurPickCount++;

			MaoStageUpdateGoldEvent eUI = new MaoStageUpdateGoldEvent();
			eUI.CurrentGold = mCurPickCount;
			eUI.TotalGold = mMaxPickCount;
			eUI.PickPos = obj.GetPosition();
			EventSystem.Instance.PushEvent(eUI);

			if (mCurPickCount >= mMaxPickCount)
			{
				EventSystem.Instance.PushEvent(new MaoStageSucceedEvent());
			}
		}
		else
		{
			FindPickEvent e = new FindPickEvent();
			e.OwnerId = (int)picker.InstanceID;
			e.PickResId = pti.resID;
			e.Position = obj.GetPosition();
			EventSystem.Instance.PushEvent(e);
		}
    }

    public override SceneType getType()
    {
        return SceneType.SceneType_Mao;
    }

    protected override bool MayDisplayLianJi()
    {
        return false;
    }

    protected override bool MayDisplayGuideArrow()
    {
        return false;
    }

	protected override void OnStateChangeToWorking()
	{
		base.OnStateChangeToWorking();

		ResetLogicRunTime();
		SceneManager.Instance.SetCountDown((int)mSceneRes.mLogicTime);

		mBattleUIModule.ShowTimer(true);
	}

	protected override void OnStateChangeToClosing()
	{
		base.OnStateChangeToClosing();

		mBattleUIModule.ShowTimer(false);

		WindowManager.Instance.CloseUI("maostagebattle");
	}
}
