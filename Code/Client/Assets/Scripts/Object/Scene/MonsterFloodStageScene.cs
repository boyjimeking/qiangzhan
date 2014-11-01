using UnityEngine;
using System.Collections;

public class MonsterFloodStageSceneInitParam : StageSceneInitParam 
{
}

public class MonsterFloodStageScene : StageScene
{
 
    public MonsterFloodStageScene()
    {

    }

    override public bool Init(BaseSceneInitParam param)
	{
		if (!base.Init(param))	
		   return false;

        mBalanceComponent = new MonsterFloodBlanceComponent(this);
		return true;
	}

	protected override void OnSceneDestroy ()
	{
		base.OnSceneDestroy ();

	}
	protected override void OnSceneInited ()
	{
		base.OnSceneInited();

		MonsterFloodModule mfm = ModuleManager.Instance.FindModule<MonsterFloodModule>();
        mfm.Reset();

	}
	
	public override void OnKillEnemy (ObjectBase sprite, uint killId)
	{
		base.OnKillEnemy (sprite, killId);

		ModuleManager.Instance.FindModule<MonsterFloodModule>().TempMoney += 50;
		BattleUIEvent bu3 = new BattleUIEvent(BattleUIEvent.BATTLE_UI_PICK_TEMPMONEY);
		EventSystem.Instance.PushEvent(bu3);

	}

	public override void OnSpriteModelLoaded (uint instanceid)
	{
		base.OnSpriteModelLoaded (instanceid);

		Player ply = PlayerController.Instance.GetControlObj() as Player;
		if(ply.InstanceID == instanceid)
		{
			ply.AddSkillEffect(new AttackerAttr(ply),SkillEffectType.Buff,527);
			Debug.Log("加buff 527");
		}


	}

    public override bool isSafeScene()
    {
        return false;
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

		if(pick.GetAlias()=="pick_buff")
		{
			ModuleManager.Instance.FindModule<MonsterFloodModule>().TempMoney -= 50;
			GameDebug.Log("拾取pick_buff:" + pti.resID);
			RemoveObjsByAlias<Pick>(MonsterFloodModule.Pick_Buff);
			BattleUIEvent bu4 = new BattleUIEvent(BattleUIEvent.BATTLE_UI_PICK_TEMPMONEY);
			bu4.msg = pick.GetPosition();
			EventSystem.Instance.PushEvent(bu4);
		}

	}

    public override SceneType getType()
    {
        return SceneType.SceneType_MonsterFlood;
    }
}
