using UnityEngine;
using System.Collections;

public class ArenaSceneInitParam : GameSceneInitParam
{
	GUID guid;
}

public class ArenaScene : GameScene
{
    private Scene_ArenaSceneTableItem mSubRes = null;

	private ArenaEndActionParam mParam = new ArenaEndActionParam();

	private AnimActionPlayAnim mAnimParam = null;

	private ArenaModule mModule = ModuleManager.Instance.FindModule<ArenaModule>();

	private Ghost mGhost = null;

	public ArenaScene()
    {

    }

	override public bool Init(BaseSceneInitParam param)
	{
        mSubRes = DataManager.SceneTable[param.res_id] as Scene_ArenaSceneTableItem;
        if (mSubRes == null)
            return false;

        if (!base.Init(param))
            return false;

		mResult = -1;
		mAnimParam = AnimActionFactory.Create(AnimActionFactory.E_Type.PlayAnim) as AnimActionPlayAnim;
		mAnimParam.AnimName = "Base Layer.huanhu";

		return true;
	}

	override public void OnMainPlayerDie()
	{
		mResult = 0;

		pass();
	}

    public override SceneType getType()
    {
        return SceneType.SceneType_Arena;
    }

    public override void OnKillEnemy(ObjectBase sprite, uint killId)
    {
        base.OnKillEnemy(sprite, killId);

        Ghost n = sprite as Ghost;
        if (n == null)
            return;

		Player player = PlayerController.Instance.GetControlObj() as Player;
		if (player == null || player.IsDead())
			return;

		mResult = 1;

		pass();
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

	override public void OnSpriteModelLoaded(uint instanceid)
	{
		base.OnSpriteModelLoaded(instanceid);

		BattleUnit unit = FindObject(instanceid) as BattleUnit;
		if (unit == null)
			return;

		if (PlayerController.Instance.GetMainCropsControl() == instanceid || 
			PlayerController.Instance.GetSubCropsControl() == instanceid || 
			PlayerController.Instance.GetControl() == instanceid)
		{
			unit.AddEffect(528, null);
			unit.AddSkillEffect(new AttackerAttr(unit), SkillEffectType.Buff, GameConfig.PvpBuffId);
		}
		else
		{
			Ghost ghost = unit as Ghost;
			if (ghost == null)
				return;

			mGhost = ghost;
			ghost.AddSkillEffect(new AttackerAttr(ghost), SkillEffectType.Buff, GameConfig.PvpBuffId);
		}
	}

	override public void Destroy()
	{
		base.Destroy();
	}

	protected override void OnStateChangeToWorking()
	{
		base.OnStateChangeToWorking();

		if (mGhost != null)
		{
			mGhost.CreateGhostAI();
		}

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

		if (mResult < 0)
		{
			BattleUnit unit = PlayerController.Instance.GetControlObj() as BattleUnit;
			if(unit == null)
			{
				SceneManager.Instance.RequestEnterLastCity();
				return;
			}

			float mMainRate = (float)unit.GetHP() / (float)unit.GetMaxHP();
			float mGhostRate = (float)mGhost.GetHP() / (float)mGhost.GetMaxHP();
			if (mMainRate > mGhostRate)
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

			if (mGhost != null && !mGhost.IsDead())
			{
				mGhost.GetStateController().DoAction(mAnimParam);
			}
		}
		else if (mResult == 1)
		{
			mModule.Win = true;

			mResult = 1;

			Player player = PlayerController.Instance.GetControlObj() as Player;
			if (player != null && !player.IsDead())
			{
				player.GetStateController().DoAction(mAnimParam);
			}
		}

		mParam.result = mResult;
		Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_ARENA_END, mParam, false);

		WindowManager.Instance.OpenUI("pvpbalance", 0);
	}
}
