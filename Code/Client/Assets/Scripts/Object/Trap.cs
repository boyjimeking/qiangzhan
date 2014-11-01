using System.Collections;
using UnityEngine;
public class TrapInitParam : RoleInitParam
{
	public uint trapResID = uint.MaxValue;

	public uint lifeTime = uint.MaxValue;

	// 召唤者的属性.
	public AttackerAttr summonerAttr;
}

/// <summary>
/// 地雷/陷阱, 没有属性和技能, 可以绑定buff, 触发爆炸等.
/// 该单位的阵营与召唤者的阵营一致. 如果由系统创建, 需要手动指定阵营.
/// </summary>
public class Trap : BattleUnit
{
	private TrapTableItem mTrapResource = null;
	private AttackerAttr mSummonerAttr;

	private SkillUtilities.TaskManager mTaskManager = null;

	/// <summary>
	/// 当trap死亡或者处在死亡倒计时时, trap不再检测附近敌人, 该变量为false.
	/// </summary>
	private bool mAwake = true;

	private ClientBehaviourIdContainer mLoopClientBehaviours = null;

	public override bool Init(ObjectInitParam param)
	{
		if (!base.Init(param))
			return false;

		TrapInitParam trapInit = (TrapInitParam)param;

		if (!DataManager.TrapTable.ContainsKey(trapInit.trapResID))
		{
			return false;
		}

		mTrapResource = DataManager.TrapTable[trapInit.trapResID] as TrapTableItem;
		mModelResID = (int)mTrapResource.modelID;
		mSummonerAttr = trapInit.summonerAttr;

		// 没有召唤者.
		if (mSummonerAttr.AttackerID == uint.MaxValue)
			mSummonerAttr = new AttackerAttr(this, mSummonerAttr.SkillCommonID, mSummonerAttr.StructMadeByRandEvent);

		mTaskManager = new SkillUtilities.TaskManager(null);

		// 生命时间结束后, 进入倒计时死亡.
		mTaskManager.AddTask(new SkillUtilities.Task(trapInit.lifeTime, explodeCountBackwards, null));

		// 倒计时死亡之后, 再经过爆炸延迟, 开始爆炸.
		uint explodeTime = uint.MaxValue;

		// 避免累加时溢出.
		if (trapInit.lifeTime != uint.MaxValue && mTrapResource.explodeDelay != uint.MaxValue)
			explodeTime = trapInit.lifeTime + mTrapResource.explodeDelay;

		mTaskManager.AddTask(new SkillUtilities.Task(explodeTime, explode, null));
		mTaskManager.Start();

		return true;
	}

	public override void OnEnterScene(BaseScene scene, uint instanceid)
	{
		mAwake = true;
		base.OnEnterScene(scene, instanceid);
		// trap由于自身没有属性, 因此使用召唤者的属性创建出生buff.
		if (mTrapResource.buffID != uint.MaxValue)
			AddBornSkillEffect(mSummonerAttr, SkillEffectType.Buff, mTrapResource.buffID);
	}

	/// <summary>
	/// trap爆炸死亡.
	/// </summary>
	private void explode(object param)
	{
		if (mLoopClientBehaviours != null)
			SkillClientBehaviour.RemoveAll(this, mLoopClientBehaviours);
		Die(new AttackerAttr(this));	
	}

	/// <summary>
	/// 开始进入爆炸倒计时.
	/// </summary>
	private void explodeCountBackwards(object param)
	{
		if (!mAwake) return;

		RemoveSkillBuffByResID(mTrapResource.buffID);

		if (mTrapResource.delayEffect != uint.MaxValue)
		{
			if (mTrapResource.loopDelayEffect)
				mLoopClientBehaviours = new ClientBehaviourIdContainer();

			SkillClientBehaviour.AddEffect2Object(this,
				mTrapResource.delayEffect,
				mTrapResource.delayEffectBindpoint,
				GetDirection(),
				mTrapResource.loopDelayEffect,
				mLoopClientBehaviours
				);
		}

		mAwake = false;
	}

	public override string dbgGetIdentifier()
	{
		return "trap: " + mTrapResource.resID;
	}

	public override int Type
	{
		get
		{
			return ObjectType.OBJ_TRAP;
		}
	}

	public override int GetObjectLayer()
	{
		return (int)ObjectLayerType.ObjectLayerPlayer;
	}

	public override string GetObjectTag()
	{
		return ObjectType.ObjectTagTrap;
	}

	protected override void onDie(AttackerAttr attackerAttr, ImpactDamageType impactDamageType)
	{
		SkillDetails.SelectTargetAndAddSkillEffect(mSummonerAttr, GetPosition(), GetDirection(),
			mTrapResource.targetSelectionOnExplode,
			mTrapResource.skillEffectOnExplode
			);

		SkillClientBehaviour.AddSceneEffect(mTrapResource._3DEffectOnExplode, GetPosition());

		base.onDie(attackerAttr, impactDamageType);
	}

	public override bool Update(uint elapsed)
	{
		if (IsDead())
			return false;

		mTaskManager.Update(elapsed);

		BaseScene scn = SceneManager.Instance.GetCurScene();
		if (scn == null)
			return false;

		// 接触时爆炸.
		if (mAwake && mTrapResource.collideRadius > 0)
		{
			Vector3 myPosition = GetPosition();
			ArrayList container = Scene.SearchBattleUnit(new Vector2f(myPosition.x, myPosition.z), mTrapResource.collideRadius);

			if (container != null && containsEnemy(container))
			{
				explodeCountBackwards(null);
				mTaskManager.Stop();
				mTaskManager.AddTask(new SkillUtilities.Task(mTrapResource.explodeDelay, explode, null));
				mTaskManager.Start();
			}
		}
		
		return base.Update(elapsed);
	}

	bool containsEnemy(ArrayList container)
	{
		foreach (BattleUnit x in container)
		{
			if (this.IsEnemy(x))
				return true;
		}
		
		return false;
	}
}
