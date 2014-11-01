using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QualifyingSceneInitParam : GameSceneInitParam
{

}

public class QualifyingScene : GameScene
{
    private Scene_QualifyingSceneTableItem mSubRes = null;

	private QualifyingEndActionParam mParam = new QualifyingEndActionParam();

	private AnimActionPlayAnim mAnimParam = null;

	private QualifyingModule mModule = ModuleManager.Instance.FindModule<QualifyingModule>();

	private Ghost mMainGhost = null;
	private Ghost mOtherGhost = null;

	public QualifyingScene()
    {

    }

	override public bool Init(BaseSceneInitParam param)
	{
		mSubRes = DataManager.SceneTable[param.res_id] as Scene_QualifyingSceneTableItem;
        if (mSubRes == null)
            return false;

        if (!base.Init(param))
            return false;

		mResult = -1;
		mAnimParam = AnimActionFactory.Create(AnimActionFactory.E_Type.PlayAnim) as AnimActionPlayAnim;
		mAnimParam.AnimName = "Base Layer.huanhu";

		return true;
	}

    public override SceneType getType()
    {
        return SceneType.SceneType_Qualifying;
    }

	override public ObjectBase CreateMainPlayer()
	{
		GhostInitParam initParam = new GhostInitParam();
		initParam.init_pos = GetInitPos();
		initParam.init_pos.y = GetHeight(initParam.init_pos.x, initParam.init_pos.z);
		initParam.init_dir = GetInitDir();
		initParam.ghost_data.SyncProperty(PlayerDataPool.Instance.MainData);
		initParam.league = LeagueDef.Red;
		initParam.main_player = true;

		ObjectBase playerGhost = CreateSprite(initParam);
		if (playerGhost == null)
			return null;

		SetOwner(playerGhost);

		PlayerController.Instance.SetControl(playerGhost.InstanceID);//uint.MaxValue);

		return playerGhost;
	}

	override protected void OnGameStart()
	{
		base.OnGameStart();

		if (mModule.GetGhostGuid() == null || mModule.GetGhostData() == null)
		{
			SceneManager.Instance.RequestEnterLastCity();
			return;
		}

		GhostInitParam initParam = new GhostInitParam();
		initParam.init_pos = GetInitPosByName("ghost");
		initParam.init_pos.y = GetHeight(initParam.init_pos.x, initParam.init_pos.z);
		initParam.init_dir = GetInitDirByName("ghost");
		initParam.ghost_data = mModule.GetGhostData();
		initParam.league = LeagueDef.Blue;
		initParam.main_player = false;
		CreateSprite(initParam);

        if (Vector3.zero != GetInitPosByName("ghostcrops1") && null != mModule.MainCropsInfo && -1 != mModule.MainCropsInfo.mCropsId)
        {
            //创建玩家拥有的佣兵
            CropsInitParam cropsParam = new CropsInitParam();
            cropsParam.crops_res_id = mModule.MainCropsInfo.mCropsId;
            cropsParam.init_pos = GetInitPosByName("ghostcrops1");
            cropsParam.init_pos.y = GetHeight(cropsParam.init_pos.x, cropsParam.init_pos.z);
            cropsParam.init_dir = GetInitDirByName("ghostcrops1");
            cropsParam.league = LeagueDef.Blue;
            cropsParam.cropsinfo = mModule.MainCropsInfo;

            CreateSprite(cropsParam);

            if (Vector3.zero != GetInitPosByName("ghostcrops2") && null != mModule.SubCropsInfo && -1 != mModule.SubCropsInfo.mCropsId)
            {
                cropsParam.crops_res_id = mModule.SubCropsInfo.mCropsId;
                cropsParam.init_pos = GetInitPosByName("ghostcrops2");
                cropsParam.init_pos.y = GetHeight(cropsParam.init_pos.x, cropsParam.init_pos.z);
                cropsParam.init_dir = GetInitDirByName("ghostcrops2");
                cropsParam.league = LeagueDef.Blue;
                cropsParam.cropsinfo = mModule.SubCropsInfo;

                CreateSprite(cropsParam);
            }
        }

		WindowManager.Instance.OpenUI("fight321");
	}

	public override void OnKillEnemy(ObjectBase sprite, uint killId)
	{
		base.OnKillEnemy(sprite, killId);

		Ghost n = sprite as Ghost;
		if (n == null)
			return;

		if(n.GetLeague() == LeagueDef.Red)
		{
			mResult = 0;
		}
		else
		{
			mResult = 1;
		}

		pass();
	}

	override public void OnSpriteModelLoaded(uint instanceid)
	{
		base.OnSpriteModelLoaded(instanceid);

		BattleUnit unit = FindObject(instanceid) as BattleUnit;
		if (unit == null)
			return;

		if(PlayerController.Instance.GetMainCropsControl() == instanceid || PlayerController.Instance.GetSubCropsControl() == instanceid)
		{
			unit.AddEffect(528, null);
			unit.AddSkillEffect(new AttackerAttr(unit), SkillEffectType.Buff, GameConfig.PvpBuffId);
		}
		else
		{
			Ghost ghost = unit as Ghost;
			if (ghost == null)
				return;

			if (ghost.IsMainPlayer())
			{
				mMainGhost = ghost;
				mMainGhost.AddEffect(528, null);
			}
			else
			{
				mOtherGhost = ghost;
			}

			ghost.AddSkillEffect(new AttackerAttr(ghost), SkillEffectType.Buff, GameConfig.PvpBuffId);
		}
	}

	protected override void OnStateChangeToWorking()
	{
		base.OnStateChangeToWorking();

		mMainGhost.CreateGhostAI();
		mOtherGhost.CreateGhostAI();

		ResetLogicRunTime();
		SceneManager.Instance.SetCountDown((int)mSceneRes.mLogicTime);

		mBattleUIModule.ShowTimer(true);
	}

	protected override void OnStateChangeToClosing()
	{
		base.OnStateChangeToClosing();

		mBattleUIModule.ShowTimer(false);

		RemoveAllActionFlag();

		mParam.guid = mModule.GetGhostGuid();
		if (mParam.guid == null)
		{
			SceneManager.Instance.RequestEnterLastCity();
			return;
		}

		if(mResult < 0)
		{
			float mMainRate = (mMainGhost != null) ? ((float)mMainGhost.GetHP() / (float)mMainGhost.GetMaxHP()) : 0.0f;
			float mGhostRate = (mOtherGhost != null) ? ((float)mOtherGhost.GetHP() / (float)mOtherGhost.GetMaxHP()) : 0.0f;

			if(mMainRate > mGhostRate)
			{
				mResult = 1;
			}
			else
			{
				mResult = 0;
			}
		}

		if (mResult == 0)
		{
			mModule.Win = false;

			if(mOtherGhost != null)
			{
				mOtherGhost.GetStateController().DoAction(mAnimParam);
			}
		}
		else if (mResult == 1)
		{
			mModule.Win = true;

			if(mMainGhost != null)
			{
				mMainGhost.GetStateController().DoAction(mAnimParam);
			}
		}

		mParam.result = mResult;
		Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_QUALIFYING_END, mParam, false);

		WindowManager.Instance.OpenUI("pvpbalance", 1);
	}

	protected override void OpenUI()
	{
		//base.OpenUI();

		UIBattleFormInitParam initParam = new UIBattleFormInitParam();
		initParam.DisplayLianJi = MayDisplayLianJi();
		initParam.DisplayerGuideArrow = MayDisplayGuideArrow();
		initParam.DisplayController = false;
		WindowManager.Instance.OpenUI("battle", initParam);
	}

	protected override void CloseUI()
	{
		//base.CloseUI();

		WindowManager.Instance.CloseUI("battle");
	}
}
