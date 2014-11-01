using UnityEngine;

public abstract class ActionQuickMoveInitParam : ActionInitParam
{
	public Vector3 targetPosition;
	public AttackerAttr mAttackerAttr;
}

public abstract class ActionQuickMove : Action
{
	// 用来保存循环动作和特效.
	ClientBehaviourIdContainer mLoopClientBehaviours = new ClientBehaviourIdContainer();

	// 技能发起者的属性.
	protected AttackerAttr mAttackerAttr;

	protected SkillUtilities.QuickMoveController mController = null;

	protected Vector3 mQuickMoveDirection = Vector3.zero;

	protected override ErrorCode doStart(ActionInitParam param)
	{
		ActionQuickMoveInitParam quickMoveInit = param as ActionQuickMoveInitParam;
		if (quickMoveInit == null)
			return ErrorCode.LogicError;

		mAttackerAttr = quickMoveInit.mAttackerAttr;

		mQuickMoveDirection = quickMoveInit.targetPosition - mOwner.GetPosition();
		mQuickMoveDirection.y = 0;

		mController = new SkillUtilities.QuickMoveController(mOwner.GetPosition(), mQuickMoveDirection, mQuickMoveDirection.magnitude);

		return ErrorCode.Succeeded;
	}

	protected override void onStarted()
	{
		// 如果该位移会打断当前技能, 那么需要将动态标记的改变应用到当前的action上.
		// 否则, 只记录标记, 保证过程中不能施放新的技能即可.
		mOwner.AddActiveFlag(ActiveFlagsDef.DisableAttack, true, InterruptSkillUsing);
		mOwner.AddActiveFlag(ActiveFlagsDef.DisableSkillUse, true, InterruptSkillUsing);
		mOwner.AddActiveFlag(ActiveFlagsDef.DisableMovement, true, true);
		mOwner.AddActiveFlag(ActiveFlagsDef.DisableRotate, true, true);

		// 特效和动作.
		SkillClientBehaviour.PlayAnimation(mOwner, AnimationName, LoopAnimation, mLoopClientBehaviours);

		SkillClientBehaviour.AddEffect2Object(mOwner, EffectID, EffectBindPoint, 0f,
			LoopEffect, mLoopClientBehaviours
			);

		base.onStarted();
	}

	protected override void onStopped(bool finished)
	{
		SkillClientBehaviour.RemoveAll(mOwner, mLoopClientBehaviours);

		mOwner.AddActiveFlag(ActiveFlagsDef.DisableAttack, false, InterruptSkillUsing);
		mOwner.AddActiveFlag(ActiveFlagsDef.DisableSkillUse, false, InterruptSkillUsing);
		mOwner.AddActiveFlag(ActiveFlagsDef.DisableMovement, false, true);
		mOwner.AddActiveFlag(ActiveFlagsDef.DisableRotate, false, true);

		base.onStopped(finished);
	}

	protected override UpdateRetCode onUpdate(uint elapsed)
	{
		UpdateRetCode ret = base.onUpdate(elapsed);
		if (ret != UpdateRetCode.Continue)
			return ret;

		Vector3 targetPosition = Vector3.zero;
		
		if (!mController.Update(elapsed, AbsoluteSpeed, out targetPosition))
			ret = UpdateRetCode.Finished;

		if (mOwner.Scene.IsBarrierRegion(mOwner, targetPosition.x, targetPosition.z))
			ret = UpdateRetCode.Aborted;
		else
			mOwner.SetPosition(targetPosition);

		return ret;
	}

	public override bool OnActiveFlagsStateChanged(ActiveFlagsDef flagName, bool increased)
	{
		if (!increased)
			return true;
		switch (flagName)
		{
			// displace的中途, 被控制移动, 那么停止displace.
			case ActiveFlagsDef.DisableMovement:
				return false;
			default:
				break;
		}
		return true;
	}

	#region AbstractProperties
	/// <summary>
	/// 位移类型.
	/// </summary>
	protected abstract SkillDisplacementType QuickMoveType { get; }
	/// <summary>
	/// 位移是否打断当前正在使用的技能.
	/// </summary>
	protected abstract bool InterruptSkillUsing { get; }
	/// <summary>
	/// 位移的速度绝对值.
	/// </summary>
	protected abstract float AbsoluteSpeed { get; }
	#endregion AbstractProperties

	#region OverrideProperties
	/// <summary>
	/// 位移时的动画.
	/// </summary>
	protected virtual string AnimationName { get { return null; } }
	/// <summary>
	/// 位移时的动画是否循环.
	/// </summary>
	protected virtual bool LoopAnimation { get { return false; } }
	/// <summary>
	/// 位移时的特效.
	/// </summary>
	protected virtual uint EffectID { get { return uint.MaxValue; } }
	/// <summary>
	/// 位移时的特效帮点.
	/// </summary>
	protected virtual string EffectBindPoint { get { return null; } }
	/// <summary>
	/// 位移时的特效是否循环.
	/// </summary>
	protected virtual bool LoopEffect { get { return false; } }
	#endregion OverrideProperties
}
