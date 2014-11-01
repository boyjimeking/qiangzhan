using UnityEngine;
using System.Collections;

public class QiangLinDanYuSceneInitParam : ActivitySceneInitParam
{
}

public class QiangLinDanYuScene : ActivityScene
{
    private Scene_QiangLinDanYuSceneTableItem mSubRes = null;
    private uint mScore = 0;
    private QiangLinDanYuReportScoreActionParam mParam = new QiangLinDanYuReportScoreActionParam();

    public QiangLinDanYuScene()
    {

    }

    override public bool Init(BaseSceneInitParam param)
	{
        mSubRes = DataManager.SceneTable[param.res_id] as Scene_QiangLinDanYuSceneTableItem;
        if (mSubRes == null)
            return false;

        if (!base.Init(param))
            return false;

		mReportInterval = 500;

		return true;
	}

	override protected void OnStateChangeToWorking()
	{
		base.OnStateChangeToWorking();

		ResetLogicRunTime();
		SceneManager.Instance.SetCountDown((int)mMaxLogicTime);

		mBattleUIModule.ShowTimer(true);
	}

	override protected void OnStateChangeToClosing()
	{
		base.OnStateChangeToClosing();

		Finish();
	}

	override public void OnMainPlayerDie()
	{
        Finish();
	}

	protected override void OnSceneInited()
	{
		base.OnSceneInited();

        WindowManager.Instance.OpenUI("qianglindanyu");
	}

    protected override void OnSceneDestroy()
    {
        base.OnSceneDestroy();

        WindowManager.Instance.CloseUI("qianglindanyu");
    }

    public override SceneType getType()
    {
        return SceneType.SceneType_QiangLinDanYu;
    }

    public override void OnKillEnemy(ObjectBase sprite, uint killId)
    {
        base.OnKillEnemy(sprite, killId);

        Npc n = sprite as Npc;
        if (n == null)
            return;

        int score = n.GetQiangLinDanYuScore();
        if (score < 0)
            return;

        mScore += (uint)score;

        QiangLinDanYuKillEnemyEvent bue = new QiangLinDanYuKillEnemyEvent(QiangLinDanYuKillEnemyEvent.QIANGLINDANYU_KILL_ENEMY_EVENT);
        bue.msg = sprite.GetPosition();
        EventSystem.Instance.PushEvent(bue);
    }

    override protected void Report()
    {
        mParam.Score = mScore;

        Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_QIANGLINDANYU_REPORT_SCORE, mParam, false);
    }

    public void Finish()
    {
        mCompleted = true;

		mBattleUIModule.ShowTimer(false);

        WindowManager.Instance.CloseUI("qianglindanyu");

        QiangLinDanYuOverActionParam param = new QiangLinDanYuOverActionParam();
        param.Score = mScore;

        Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_QIANGLINDANYU_OVER, param);
    }
}
