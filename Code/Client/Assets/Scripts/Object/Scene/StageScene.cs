using Message;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StageSceneInitParam : GameSceneInitParam
{
}

public class StageScene : GameScene
{
    // StageScene子表
    protected Scene_StageSceneTableItem mSubRes = null;

	protected BattleUIModule mBattleUIModule = ModuleManager.Instance.FindModule<BattleUIModule>();

    public StageScene()
    {

    }

    override public bool Init(BaseSceneInitParam param)
	{
		StageSceneInitParam stageParam = param as StageSceneInitParam;

        if (!DataManager.SceneTable.ContainsKey(stageParam.res_id))
            return false;

        mSubRes = DataManager.SceneTable[stageParam.res_id] as Scene_StageSceneTableItem;
		if (!base.Init(stageParam))	
		   return false;

        mBalanceComponent = new StageSceneBalanceComponent(this);
        mShowPickGuide = false;

		mReliveComponent = new StageSceneReliveComponent(this);
		return true;
	}

	public Scene_StageSceneTableItem GetStageRes()
	{
		return mSubRes;
	}

	public override uint GetSceneScore()
	{
		if(mSubRes == null)
		{
			return 0;
		}

		if(mSubRes.mLogicTime == uint.MinValue || mSubRes.mLogicTime == uint.MaxValue)
		{
			return mSubRes.mTotalScore;
		}

		return (mSubRes.mTotalScore * mLogicRunTime) / mSubRes.mLogicTime;
	}

	public override StageGrade GetSceneGrade()
	{
		if(mSubRes == null)
		{
			return StageGrade.StageGrade_Invalid;
		}

		if(mLogicRunTime < mSubRes.mTimeS)
		{
			return StageGrade.StageGrade_S;
		}
		else if(mLogicRunTime < mSubRes.mTimeA)
		{
			return StageGrade.StageGrade_A;
		}
		else if(mLogicRunTime < mSubRes.mTimeB)
		{
			return StageGrade.StageGrade_B;
		}

		return StageGrade.StageGrade_C;
	}

	override public void OnMainPlayerDie()
	{      
		if(mReliveComponent != null)
		{    
			mReliveComponent.requestRelive();
		}
	}

	protected override void OnSceneInited ()
	{
		base.OnSceneInited();
	}

	public override void OnPick (ObjectBase pick, ObjectBase picker)
	{
		base.OnPick (pick, picker);
	}

    //重置复活次数
    public void ResetReliveTimes()
    {
       // mReliveComponent();
      var ssrc=  mReliveComponent as StageSceneReliveComponent;
      ssrc.ResetReliveTime();
    }

    public override SceneType getType()
    {
        return SceneType.SceneType_Stage;
    }

	// 掉落首次&&通关&&Boss奖励
	public void DropPassAward()
	{
		ObjectBase obj = PlayerController.Instance.GetControlObj();
		if (obj == null)
			return;

		// 首次通关掉落
		PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
		if(!module.IsStageHasPassed(mSubRes.resID))
		{
			List<role_dropaward> fpList = StageDataManager.Instance.GetFirstPassAwards();
			if(fpList != null)
			{
				for(int i = 0; i < fpList.Count; ++i)
				{
					role_dropaward fpAward = fpList[i];
					if(fpAward != null)
					{
						if(fpAward.drop_id_type == 0)
						{
							GameDebug.LogWarning("掉落物的Dropbox表不使用ConditionId了.");
						}
						else
						{
							List<PickInitParam> paramList = new List<PickInitParam>();
							if (SceneObjManager.CreatePickInitParam(Pick.PickType.ITEM, -1, fpAward.drop_id, obj.GetPosition(), 0, out paramList, true, Pick.FlyType.DROP_DOWN, false))
							{
								foreach (PickInitParam param in paramList)
								{
									CreateSprite(param);
								}
							}
						}
					}
				}
			}
		}

		Vector3 pos = BossDeadPos != Vector3.zero ? BossDeadPos : obj.GetPosition();
		if (pos == null)
			return;

		// 通关随机掉落
		List<role_dropaward> dropList = StageDataManager.Instance.GetBossKillAwards();
		if(dropList != null && dropList.Count > 0)
		{
			for (int i = 0; i < dropList.Count; ++i)
			{
				role_dropaward dropAward = dropList[i];
				if (dropAward != null)
				{
					if (dropAward.drop_id_type == 0)
					{
						GameDebug.LogWarning("掉落物的Dropbox表不使用ConditionId了.");
					}
					else
					{
						List<PickInitParam> paramList = new List<PickInitParam>();
						if (SceneObjManager.CreatePickInitParam(Pick.PickType.ITEM, -1, dropAward.drop_id, obj.GetPosition(), 0, out paramList, true, Pick.FlyType.FLY_OUT, false))
						{
							foreach (PickInitParam param in paramList)
							{
								CreateSprite(param);
							}
						}
					}
				}
			}
		}

		// 金币随机掉落
		uint gold = StageDataManager.Instance.GetGoldAwards();
		if(gold != 0 && gold != uint.MaxValue)
		{
			List<PickInitParam> paramList = new List<PickInitParam>();
            if (SceneObjManager.CreatePickInitParam(Pick.PickType.MONEY, 3, (int)gold, pos, 0, out paramList, true, Pick.FlyType.FLY_OUT, false))
            {
                foreach (PickInitParam param in paramList)
                {
                    CreateSprite(param);
                }
            }
		}
	}

	override public void OnKillEnemy(ObjectBase sprite, uint killId)
	{
		base.OnKillEnemy(sprite, killId);

// 		if(PlayerController.Instance.GetControl() == killId 
// 		|| PlayerController.Instance.GetMainCropsControl() == killId
// 		|| PlayerController.Instance.GetSubCropsControl() == killId)
// 		{
		mBattleUIModule.OnKillEnemy(sprite);
//		}
	}
}
