using UnityEngine;
using System.Collections;

public class YaZhiXieESceneInitParam : ActivitySceneInitParam
{
}

public class YaZhiXieEScene : ActivityScene
{
	private Scene_YaZhiXieESceneTableItem mSubRes = null;

    private uint mScore = 0;
	private uint mMonsterCount = 0;
    
    private YaZhiXieEReportActionParam mParam = new YaZhiXieEReportActionParam();

    public YaZhiXieEScene()
    {

    }

    override public bool Init(BaseSceneInitParam param)
	{
        mSubRes = DataManager.SceneTable[param.res_id] as Scene_YaZhiXieESceneTableItem;
        if (mSubRes == null)
            return false;

        if (!base.Init(param))
            return false;

		mReportInterval = 500;

		mBalanceComponent = new YaZhiXieESceneBalanceComponent(this);

		return true;
	}
   
	protected override void OnStateChangeToClosing()
	{
		base.OnStateChangeToClosing();

		Finish();
	}

	override public void OnMainPlayerDie()
	{
		SetResult(0);
		pass();
	}

	protected override void OnSceneInited()
	{
		base.OnSceneInited();

		SetResult(1);

		WindowManager.Instance.OpenUI("yzxerankinfo");
	}

    protected override void OnSceneDestroy()
    {
        base.OnSceneDestroy();

		WindowManager.Instance.CloseUI("yzxerankinfo");
    }

    public override SceneType getType()
    {
        return SceneType.SceneType_YaZhiXieE;
    }

    public override void OnKillEnemy(ObjectBase sprite, uint killId)
    {
        base.OnKillEnemy(sprite, killId);

        Npc n = sprite as Npc;
        if (n == null)
            return;

		if (string.Compare(n.GetAlias(), "monster") != 0)
			return;

		mMonsterCount--;

        uint score = n.GetYaZhiXieEScore();
        if (score == 0 || score == uint.MaxValue)
            return;

        mScore += score;

        EventSystem.Instance.PushEvent(new YaZhiXieEUpdateScoreEvent(mScore, mMonsterCount));
    }

    override protected void Report()
    {
        mParam.score = mScore;
		mParam.time_cost = GetRunTime();

        Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_YAZHIXIEE_REPORT, mParam, false);
    }

    private void Finish()
    {
        mCompleted = true;

		mBattleUIModule.ShowTimer(false);

		WindowManager.Instance.CloseUI("yzxerankinfo");

		YaZhiXieEOverActionParam param = new YaZhiXieEOverActionParam();
        param.score = mScore;
		param.time_cost = GetRunTime();

        Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_YAZHIXIEE_OVER, param);
    }

	override public void OnSpriteEnterScene(ObjectBase sprite)
	{
		Npc npc = sprite as Npc;
		if (npc == null)
			return;

		if (string.Compare(npc.GetAlias(), "monster") != 0)
			return;

		mMonsterCount++;

		EventSystem.Instance.PushEvent(new YaZhiXieEUpdateScoreEvent(mScore, mMonsterCount));

		if(mMonsterCount >= GameConfig.YZXEMonsterMaxCount)
		{
			SetResult(0);
			pass();
		}
	}

	override protected void OnStateChangeToWorking()
	{
		base.OnStateChangeToWorking();

		ResetLogicRunTime();
		SceneManager.Instance.SetCountDown((int)mMaxLogicTime);

		mBattleUIModule.ShowTimer(true);
	}

	override protected bool InitActivityParam()
	{
		SceneActivityParam param = ActivityManager.Instance.Param;
		if (param == null || param.mSceneId != mSubRes.resID)
			return false;

		uint ms = (uint)((param.mOverTime * 1000) - System.DateTime.Now.TimeOfDay.TotalMilliseconds);
		if (ms < mSubRes.mReadyTime)
			return false;

		if(ms < mSubRes.mReadyTime + mSubRes.mLogicTime)
			mMaxLogicTime = ms - mSubRes.mReadyTime;

		ActivityManager.Instance.Param = null;

		return true;
	}
}
