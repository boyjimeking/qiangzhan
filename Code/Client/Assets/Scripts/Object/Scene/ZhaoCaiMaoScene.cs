using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZhaoCaiMaoSceneInitParam : ActivitySceneInitParam
{
}

public class ZhaoCaiMaoScene : ActivityScene
{
	private Scene_ZhaoCaiMaoSceneTableItem mSubRes = null;

	private ZhaoCaiMaoReportActionParam mParam = new ZhaoCaiMaoReportActionParam();

	private int mRequestTimer = 0;

	private int mCreateTimer = 2000;

	private int mDamage = 0;

	private int mUpdateInterval = 500;

	private int mAwardTimer = -1;

	private bool mShowAlert = false;

	private Npc mBoss = null;

//	private List<Ghost> mGhostList = new List<Ghost>();

	public ZhaoCaiMaoScene()
    {

    }

    override public bool Init(BaseSceneInitParam param)
	{
		mSubRes = DataManager.SceneTable[param.res_id] as Scene_ZhaoCaiMaoSceneTableItem;
        if (mSubRes == null)
            return false;

        if (!base.Init(param))
            return false;

		mReportInterval = 500;

		return true;
	}

	override public bool LogicUpdate(uint elapsed)
	{
		if (!base.LogicUpdate(elapsed))
			return false;

		if (mUpdateInterval > 0)
		{
			if(!mCompleted)
			{
				mUpdateInterval -= (int)elapsed;
				if(mUpdateInterval <= 0)
				{
					UpdateDamage();
					mUpdateInterval = 500;
				}
			}
		}

		if(mRequestTimer > 0)
		{
			mRequestTimer -= (int)elapsed;
			if(mRequestTimer <= 0)
			{
				RequestPartner();
			}
		}

		if(mAwardTimer > 0)
		{
			mAwardTimer -= (int)elapsed;
			if(mAwardTimer <= 0)
			{
				RequestRankingAward();
			}
		}

		int partnerCount = ActivityManager.Instance.PartnerList.Count;
		if(partnerCount > 0)
		{
			if(mCreateTimer > 0)
			{
				mCreateTimer -= (int)elapsed;
				if(mCreateTimer <= 0)
				{
					GhostInitParam initParam = new GhostInitParam();
					initParam.init_pos = GetInitPosByName("player");
					initParam.init_dir = GetInitDirByName("player");
					initParam.ghost_data.SyncProperty(ActivityManager.Instance.PartnerList[partnerCount - 1]);
					initParam.league = LeagueDef.Red;
					initParam.main_player = false;
					CreateSprite(initParam);
//					mGhostList.Add(CreateSprite(initParam) as Ghost);
					ActivityManager.Instance.PartnerList.RemoveAt(partnerCount - 1);

					mCreateTimer = 2000;
				}
			}
		}

		UpdateBoss(elapsed);

		return true;
	}

	protected override void OnStateChangeToClosing()
	{
		base.OnStateChangeToClosing();

		mBattleUIModule.ShowTimer(false);

		mBoss.Die(new AttackerAttr(mBoss));

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

		WindowManager.Instance.OpenUI("zcmrankinfo");
	}

    protected override void OnSceneDestroy()
    {
        base.OnSceneDestroy();

		WindowManager.Instance.CloseUI("zcmrankinfo");
    }

    public override SceneType getType()
    {
        return SceneType.SceneType_ZhaoCaiMao;
    }

    private void Finish()
    {
		mCompleted = true;
		mAwardTimer = 7000;

		WindowManager.Instance.CloseUI("zcmrankinfo");

		ZhaoCaiMaoOverActionParam param = new ZhaoCaiMaoOverActionParam();
		param.damage = mDamage;

		Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_ZHAOCAIMAO_OVER, param);
    }

	override protected void Report()
	{
		System.DateTime nowTime = System.DateTime.Now;
		mParam.damage = mDamage;

		Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_ZHAOCAIMAO_REPORT, mParam, false);
	}

	override protected bool InitActivityParam()
	{
		SceneActivityParam param = ActivityManager.Instance.Param;
		if (param == null || param.mSceneId != mSubRes.resID)
			return false;

		System.DateTime nowTime = System.DateTime.Now;

		uint startMS = (uint)(nowTime.TimeOfDay.TotalMilliseconds - (param.mStartTime * 1000));
		if(startMS >= (GameConfig.ZhaoCaiMaoRequestTime + 5000))
		{
			RequestPartner();
		}
		else
		{
			mRequestTimer = (int)(GameConfig.ZhaoCaiMaoRequestTime + 5000 - startMS);
		}

		if(startMS >= GameConfig.ZhaoCaiMaoReadyTime)
		{
			mMaxReadyTime = 3000;
			mShowAlert = false;
		}
		else
		{
			mMaxReadyTime = GameConfig.ZhaoCaiMaoReadyTime - startMS;
			if (mMaxReadyTime < 3000)
				mMaxReadyTime = 3000;

			mShowAlert = true;
		}

		uint endMS = (uint)((param.mOverTime * 1000) - nowTime.TimeOfDay.TotalMilliseconds);
		if(endMS < mMaxReadyTime)
		{
			ActivityManager.Instance.Param = null;
			return false;
		}

		if (endMS < (mSubRes.mLogicTime + mMaxReadyTime))
		{
			mMaxLogicTime = endMS - mMaxReadyTime;
		}
			
		ActivityManager.Instance.Param = null;

		return true;
	}

	private void CreateBoss(uint time)
	{
		NpcInitParam initParam = new NpcInitParam();
		initParam.npc_res_id = GameConfig.ZhaoCaiMaoId;
		initParam.init_pos = new Vector3(GameConfig.ZhaoCaiMaoPosX, 0.0f, GameConfig.ZhaoCaiMaoPosY);
		initParam.init_pos.y = GetHeight(initParam.init_pos.x, initParam.init_pos.z);
		initParam.init_dir = GameConfig.ZhaoCaiMaoDir;
		initParam.alias = "zhaocaimao";
		mBoss = CreateSprite(initParam) as Npc;
	}

	private void UpdateBoss(uint elapsed)
	{
		if (mBoss == null)
			return;

		if (!IsWorkingState())
			return;

		uint mLeftTime = mMaxLogicTime - mLogicRunTime;
		if (mLeftTime <= 60000)
		{
			if (mLeftTime + elapsed > 60000)
			{
				mBoss.AddSkillEffect(new AttackerAttr(mBoss), SkillEffectType.Buff, 1453);
			}
		}
		else if (mLeftTime <= 120000)
		{
			if (mLeftTime + elapsed > 120000)
			{
				mBoss.AddSkillEffect(new AttackerAttr(mBoss), SkillEffectType.Buff, 1451);
			}
		}
	}

	private void RequestPartner()
	{
		Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_ZHAOCAIMAO_PARTNER, null);
	}

	private void UpdateDamage()
	{
		EventSystem.Instance.PushEvent(new ZhaoCaiMaoUpdateDamageEvent(mDamage));
	}

	protected override void OnStateChangeToReady()
	{
		base.OnStateChangeToReady();

		AddAllActionFlag();

		mLogicRunTime = 0;

		if (mMaxReadyTime > 0 && mShowAlert)
		{
			WindowManager.Instance.OpenUI("maoalert", (int)mMaxReadyTime);
		}
	}

	override protected void OnStateChangeToWorking()
	{
		base.OnStateChangeToWorking();

// 		foreach(Ghost ghost in mGhostList)
// 		{
// 			ghost.CreateGhostAI();
// 		}

		ResetLogicRunTime();
		SceneManager.Instance.SetCountDown((int)mMaxLogicTime);

		mBattleUIModule.ShowTimer(true);

		WindowManager.Instance.CloseUI("maoalert");

		CreateBoss(mLogicRunTime);
	}

	public override void onDamage(BattleUnit damageTo, DamageInfo damage, AttackerAttr attackerAttr)
	{
		if (damage.Value < 0)
		{
			if (string.Compare(damageTo.GetAlias(), "zhaocaimao") != 0)
				return;

			if (attackerAttr.AttackerID == PlayerController.Instance.GetControlObj().InstanceID)
				mDamage -= damage.Value;
		}
	}

	private void RequestRankingAward()
	{
		Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_ZHAOCAIMAO_RANKING, null);
	}

	public override void onRoleModelLoaded(Role role)
	{
		base.onRoleModelLoaded(role);

		if (string.Compare(role.GetAlias(), "zhaocaimao") != 0)
			return;

		if (!IsWorkingState())
			return;

		uint mLeftTime = mMaxLogicTime - mLogicRunTime;
		if (mLeftTime <= 60000)
		{
			mBoss.AddSkillEffect(new AttackerAttr(mBoss), SkillEffectType.Buff, 1453);
		}
		else if (mLeftTime <= 120000)
		{
			mBoss.AddSkillEffect(new AttackerAttr(mBoss), SkillEffectType.Buff, 1451);
		}
	}
}
