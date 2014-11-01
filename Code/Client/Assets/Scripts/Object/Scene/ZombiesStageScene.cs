using UnityEngine;
using System.Collections;

public class ZombiesStageSceneInitParam : StageSceneInitParam
{
}

public class ZombiesStageScene : StageScene
{

    public ZombiesStageScene()
    {

    }

    override public bool Init(BaseSceneInitParam param)
	{
		if (!base.Init(param))	
		   return false;

		mBalanceComponent = new ActiveSceneBalanceComponent(this);
		mShowPickGuide = true;
		
		return true;
	}

	protected override void OnSceneInited ()
	{
		base.OnSceneInited();

		mBattleUIModule.ShowTimer(true);

        ZombiesStageModule zsm = ModuleManager.Instance.FindModule<ZombiesStageModule>();
        if (zsm != null)
            zsm.SetBeginGoldNum();
	}

    protected override void OnSceneDestroy()
    {
        base.OnSceneDestroy();
    }

	public override void OnPick (ObjectBase pick, ObjectBase picker)
	{
		base.OnPick (pick, picker);

		if(pick == null || picker == null)
			return;

		Pick obj = pick as Pick;

		if(obj == null)
			return;

		PickTableItem pti = obj.GetCurPickTableItem();
		if(pti == null)
			return;

        //switch(pti.resID)
        //{
        //    case 5:             //齿轮碎片;
        //        BattleUIEvent bue1 = new BattleUIEvent(BattleUIEvent.BATTLE_UI_PICK_GEAR);
        //        bue1.msg = pick.GetPosition();
        //        EventSystem.Instance.PushEvent(bue1);
        //        break;
        //    case 3:             //捡到金币;
        //        BattleUIEvent bue2 = new BattleUIEvent(BattleUIEvent.BATTLE_UI_PICK_GOLD);
        //        bue2.msg = pick.GetPosition();
        //        EventSystem.Instance.PushEvent(bue2);
        //        break;
        //}
        int pickId = (int)ConfigManager.GetVal<int>(ConfigItemKey.ZOMBIE_PICK_ID);
        //Debug.LogError(pti.resID);
        if (pti.resID == pickId)
        {
            BattleUIEvent bue1 = new BattleUIEvent(BattleUIEvent.BATTLE_UI_ZOMBIE_PICK1);
            //bue1.msg = pick.GetPosition();
            EventSystem.Instance.PushEvent(bue1);
        }
	}

    public override void OnKillEnemy(ObjectBase sprite, uint killId)
    {
        base.OnKillEnemy(sprite, killId);

        ZombiesStageModule zsm = ModuleManager.Instance.FindModule<ZombiesStageModule>();
        
        if (zsm == null)
            return;

        if (zsm.KillEnermy())
        {
            SetResult(1);
            pass();
        }
    }

    public override SceneType getType()
    {
        return SceneType.SceneType_Zombies;
    }

	protected override void OnStateChangeToClosing()
	{
		base.OnStateChangeToClosing();

		mBattleUIModule.ShowTimer(false);
	}
}
