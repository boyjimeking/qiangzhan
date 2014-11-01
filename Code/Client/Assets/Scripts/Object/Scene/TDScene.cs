using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TDSceneInitParam : StageSceneInitParam
{

}

public class TDScene : StageScene
{
	private class NpcDeadTimer
	{
		public Npc npc = null;
		public uint timer = 0;

		public NpcDeadTimer(Npc n, uint t = 0)
		{
			npc = n;
			timer = t;
		}
	}

    //private Scene_TDSceneTableItem mSubRes = null;

	private AnimActionPlayAnim mAnimParam = null;

	private List<NpcDeadTimer> mList = new List<NpcDeadTimer>();

	private static readonly uint mWaitTime = 2000;
    
	private int mLife = 0;

    public TDScene()
    {

    }

    override public bool Init(BaseSceneInitParam param)
	{
        mSubRes = DataManager.SceneTable[param.res_id] as Scene_TDSceneTableItem;
        if (mSubRes == null)
            return false;

        if (!base.Init(param))
            return false;

		mLife = GameConfig.TDLifeCount;
		mBalanceComponent = new TDBalanceComponent(this);

		mAnimParam = AnimActionFactory.Create(AnimActionFactory.E_Type.PlayAnim) as AnimActionPlayAnim;
		mAnimParam.AnimName = "Base Layer.huanhu";

		return true;
	}

	override public bool LogicUpdate(uint elapsed)
    {
        if(!base.LogicUpdate(elapsed))
        {
            return false;
		}

		for(int i = mList.Count - 1; i >= 0; --i)
		{
			NpcDeadTimer ndt = mList[i];
			if (ndt.npc == null)
			{
				mList.RemoveAt(i);
				continue;
			}

			ndt.timer += elapsed;
			if (ndt.timer > mWaitTime)
			{
				RemoveObject(ndt.npc.InstanceID);
				mList.RemoveAt(i);
				continue;
			}
		}

		return true;
	}

	protected override void OnSceneInited()
	{
		base.OnSceneInited();

		WindowManager.Instance.OpenUI("tdinfo");
	}

    protected override void OnSceneDestroy()
    {
        base.OnSceneDestroy();

        WindowManager.Instance.CloseUI("tdinfo");
    }

    public override SceneType getType()
    {
        return SceneType.SceneType_TD;
    }

	override public void OnMainPlayerDie()
	{
		pass();
	}

	override public void OnSpriteEnterZone(ObjectBase sprite, Zone zone)
	{
		base.OnSpriteEnterZone(sprite, zone);

		Npc npc = sprite as Npc;
		if (npc == null)
			return;

		if (string.Compare(npc.GetAlias(), "monster") != 0)
			return;

		npc.GetStateController().DoAction(mAnimParam);
		npc.SetWudi(true);

		NpcDeadTimer ndt = new NpcDeadTimer(npc, 0);
		mList.Add(ndt);

		mLife--;

		PopTipManager.Instance.AddNewTip(string.Format(StringHelper.GetString("td_notify", FontColor.Red), mLife));
		//PromptUIManager.Instance.AddNewPrompt(string.Format(StringHelper.GetString("td_notify"), mLife));

		Vector3 pos = new Vector3(GameConfig.TDEffectPosX, 0.0f, GameConfig.TDEffectPosZ);
		pos.y = GetHeight(pos.x, pos.z);
		CreateEffect(GameConfig.TDEffectId, Vector3.one, pos);

		EventSystem.Instance.PushEvent(new TDSceneLifeUpdateEvent(mLife));

		if(mLife < 1)
			pass();
	}

	override protected void OnStateChangeToWorking()
	{
		base.OnStateChangeToWorking();

		ResetLogicRunTime();
		SceneManager.Instance.SetCountDown((int)mSceneRes.mLogicTime);

		mBattleUIModule.ShowTimer(true);
	}

	override protected void OnStateChangeToClosing()
	{
		base.OnStateChangeToClosing();

		mBattleUIModule.ShowTimer(false);

		SetResult(1);
		pass();
	}

	protected override bool MayDisplayLianJi()
	{
		return false;
	}
}
