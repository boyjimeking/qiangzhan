using System;
using UnityEngine;
public class ActionDieInitParam : ActionInitParam
{
	public AttackerAttr attackerAttr;
	public ImpactDamageType damageType = ImpactDamageType.Invalid;
    public ActionDieInitParam()
    {
    }

	public override ActionTypeDef ActionType
	{
		get { return ActionTypeDef.ActionTypeDie; }
	}
}

/// <summary>
/// 死亡特效, 动作等.
/// 不处理IsDeath标记!
/// </summary>
public class ActionDie : Action
{
	public class Allocator : ActionAllocator
	{
		public Action Allocate()
		{
			return new ActionDie();
		}
	}

	DeathBehaviour mDeathBehaviour = null;

	public override ActionTypeDef ActionType
	{
		get { return ActionTypeDef.ActionTypeDie; }
	}

	protected override ErrorCode doStart(ActionInitParam param)
	{
		ActionDieInitParam initParam = param as ActionDieInitParam;

		if (!mOwner.IsDead())
			return ErrorCode.LogicError;

		// 禁止攻击/移动/使用技能, 并停止当前的此类Action.
		mOwner.AddActiveFlag(ActiveFlagsDef.DisableAttack, true, true);
		mOwner.AddActiveFlag(ActiveFlagsDef.DisableMovement, true, true);
		mOwner.AddActiveFlag(ActiveFlagsDef.DisableSkillUse, true, true);
		mOwner.AddActiveFlag(ActiveFlagsDef.DisableRotate, true, true);
		mOwner.AddActiveFlag(ActiveFlagsDef.Inviolability, true, false);

		mDeathBehaviour = new DeathBehaviour();
		mDeathBehaviour.Start(mOwner, initParam.damageType, initParam.attackerAttr);

		return base.doStart(param);
	}

	protected override UpdateRetCode onUpdate(uint elapsed)
	{
		mDeathBehaviour.Update(elapsed);
		return base.onUpdate(elapsed);
	}

	protected override ErrorCode doStop(bool finished)
	{
		// ActionDie只能被终止, 不会自然停止!
		if (finished)
			return ErrorCode.LogicError;

		mOwner.AddActiveFlag(ActiveFlagsDef.DisableAttack, false, true);
		mOwner.AddActiveFlag(ActiveFlagsDef.DisableMovement, false, true);
		mOwner.AddActiveFlag(ActiveFlagsDef.DisableSkillUse, false, true);
		mOwner.AddActiveFlag(ActiveFlagsDef.DisableRotate, false, true);
		mOwner.AddActiveFlag(ActiveFlagsDef.Inviolability, false, true);

		mDeathBehaviour.Stop(false);

		return base.doStop(finished);
	}

	protected override void onStopped(bool finished)
	{
		base.onStopped(finished);
	}

	void onDeathBehaviourStopped()
	{ 
		
	}
}

/// <summary>
/// 死亡的前端表现.
/// </summary>
class DeathBehaviour
{
	protected BattleUnit mOwner = null;

	int mDieAnimationHashCode = 0;

	protected bool IsRunning {private set; get;}

	public void Start(BattleUnit owner, ImpactDamageType damageType, AttackerAttr killerAttr)
	{
		if (!IsRunning)
		{
			IsRunning = true;
			mOwner = owner;

			createDeathClientBehaviour(damageType, killerAttr);

			onStarted(killerAttr);
		}
	}

	protected virtual void onStarted(AttackerAttr killerAttr)
	{ }

	public void Stop(bool finished)
	{
		if (IsRunning)
		{
			IsRunning = false;

			// 如果该死亡动作被强制终止, 立即停止当前的动画.
			//if (!finished)
			mOwner.GetStateController().FinishCurrentState(mDieAnimationHashCode);

			onStopped(finished);
		}
	}

	protected virtual void onStopped(bool finished)
	{ }

	public void Update(uint elapsed) {
		if (IsRunning && !onUpdate(elapsed))
			Stop(true);
	}

	protected virtual bool onUpdate(uint elapsed)
	{
		return true;
	}

	/// <summary>
	/// 死亡的前端表现.
	/// </summary>
	private void createDeathClientBehaviour(ImpactDamageType damageType, AttackerAttr killerAttr)
	{
		// 材质死亡(特效).
		ErrorHandler.Parse(
			mOwner.AddMaterialBehaviour(MaterialBehaviourDef.OnMaterialDie, damageType, killerAttr.EffectStartDirection),
			"failed to add death material effect"
			);

		// 死亡动作.
		MecanimStateController stateControl = mOwner.GetStateController();
		AnimatorProperty animSet = stateControl.AnimSet;
		if (animSet != null && !string.IsNullOrEmpty(mOwner.GetDeathAnimation()))//animSet.NumOfDie > 0)
		{
            AnimActionDeath death = AnimActionFactory.Create(AnimActionFactory.E_Type.Death) as AnimActionDeath;
			death.dieAnim = mOwner.GetDeathAnimation();
            if (stateControl.AnimSet != null)
                mDieAnimationHashCode = stateControl.AnimSet.GetStateHash(death.dieAnim);
            stateControl.DoAction(death);
		}


        //mOwner.SetDeathMaterial("dssipate");
        //mOwner.SetDeathMaterial("burn_out");
		// 死亡声音.
		if (mOwner.GetDeadSound() != -1)
		{
			SoundManager.Instance.Play(mOwner.GetDeadSound());
		}
	}
}
