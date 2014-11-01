using System.Collections.Generic;
using UnityEngine;

public class ActionSkillInitParam : ActionInitParam
{
	public BattleUnitSkill skill;
	public Vector3 targetPosition;

	public override ActionTypeDef ActionType
	{
		get { return ActionTypeDef.ActionTypeSkill; }
	}
}

internal struct EffectCreateStructure
{
	public string bindpoint;
	public uint effectID;
	public bool loop;
}

public class ActionSkill : Action
{
	public class Allocator : ActionAllocator
	{
		public Action Allocate()
		{
			return new ActionSkill();
		}
	}

	private enum ActionSkillState : int
	{
		// 无效状态.
		Invalid = -1,
		// 技能准备状态.
		Charging = 0,
		// 技能使用状态.
		Using = 1,
	}

	ActionSkillState mState = ActionSkillState.Invalid;

	// 当前正在使用的技能.
	BattleUnitSkill mSkillUsing;

	// 目标点距离当前发起者的距离(不保存目标点, 每次使用目标点时, 从当前位置根据方向和距离重新计算).
	float mTargetPositionDistance = 0f;

	// 当前技能使用阶段经过的时间.
	uint mStateTimer = 0;

	// 蓄力了多久.
	uint mCharge = 0;

	// 管理技能使用过程中, 前端表现, 技能效果等的延迟.
	// 通过把需要执行的方法加入到该容器中, 在时间到达时, 回调执行.
	SkillUtilities.TaskManager mTaskManager = null;

	// 所有循环的前段表现的运行时ID.
	ClientBehaviourIdContainer mLoopClientBehaviours = new ClientBehaviourIdContainer();

	// 技能使用阶段的剩余的循环次数.
	uint mSkillUseStateLoopLeft = 1;

	public override ActionTypeDef ActionType
	{
		get { return ActionTypeDef.ActionTypeSkill; }
	}

	protected override ErrorCode canStart(ActionInitParam param)
	{	// 距离等检测放到技能使用时.
		ActionSkillInitParam skillParam = param as ActionSkillInitParam;

		if(skillParam == null)
			return ErrorCode.LogicError;

        return ErrorCode.Succeeded;
	}

	/// <summary>
	/// 检查可否从当前状态转移到otherState.
	/// 准备阶段不检查距离, 使用阶段检查.
	/// </summary>
    private ErrorCode canEnterState(ActionSkillState otherState, Vector3 targetPosition)
    {
		if (otherState == mState)
            return ErrorCode.LogicError;

        // 只能从invalid状态进入charge.
        if (otherState == ActionSkillState.Charging && mState != ActionSkillState.Invalid)
            return ErrorCode.LogicError;

		// 
		return ErrorCode.Succeeded;
    }

	/// <summary>
	/// 技能进入使用状态(除逻辑错误外, 不会失败).
	/// </summary>
	public ErrorCode EnterUseState(Vector3 targetPosition)
	{
        ErrorCode err = canEnterState(ActionSkillState.Using, targetPosition);
        if (err != ErrorCode.Succeeded)
            return err;

		// 技能可被打断, 表示是可以蓄力的.
		if (Interruptable)
			mCharge = mStateTimer;

		SkillTargetPosition = targetPosition;
		switch2State(ActionSkillState.Using);

		loadUseStateTasks();
		
		mTaskManager.Start();

		// 置CD.
		SkillDetails.OnActionEnterUseState(mOwner, mSkillUsing);
		
		// 进入使用状态时不会再设置朝向.

		return ErrorCode.Succeeded;
	}

	void loadUseStateTasks()
	{
		loadClientBehaviourTask(mSkillUsing.skillRes.useBehaviour);

		mTaskManager.AddTask(new SkillUtilities.Task(
			mSkillUsing.skillRes.skillEffectStartTime, skillTakeEffect,
			null
			));

		// 技能覆盖时间结束之后, 通知.
		// 由于不能在taskHandler中, 添加task, 所以不能直接加入重新进入UseState的task.
		mTaskManager.AddTask(new SkillUtilities.Task(mSkillUsing.skillRes.skillUseStateDuration, null, null));
	}

	/// <summary>
	/// 通过前端表现的资源, 转换为task, 加入到taskManager中.
	/// </summary>
	private void loadClientBehaviourTask(uint clientBehaviourResID)
	{
		SkillClientBehaviourItem resource = (clientBehaviourResID != uint.MaxValue)
			? DataManager.SkillClientBehaviourTable[clientBehaviourResID] as SkillClientBehaviourItem
			: null;

		if (resource == null) return;

		// skillclientbehaviour中的换武器, 拉近镜头, 循环的(特效, 声音, 动作), 只在技能第一次进入使用阶段时播放. 
		// 其他情况下, 在任意一次进入使用阶段都会播放.
		bool chargingOrFirstUseState = (mState == ActionSkillState.Charging || FirstUseState);

		if (!string.IsNullOrEmpty(resource.soundResID) && (chargingOrFirstUseState || !resource.loopSound))
			mTaskManager.AddTask(new SkillUtilities.Task(0, playSound, resource));

		if (!string.IsNullOrEmpty(resource.userAnimationName) && (chargingOrFirstUseState || !resource.loopUserAnimation))
			mTaskManager.AddTask(new SkillUtilities.Task(0, playAnimation, resource));

		if (!string.IsNullOrEmpty(resource.weaponAnimationName) && (chargingOrFirstUseState || !resource.loopWeaponAnimation))
			mTaskManager.AddTask(new SkillUtilities.Task(0, playWeaponAnimation, resource));

		if (resource.cameraDist2Player >= 0f && chargingOrFirstUseState)
			mTaskManager.AddTask(new SkillUtilities.Task(0, playCameraEffect, resource));

		if (resource.effectID_0 != uint.MaxValue && (chargingOrFirstUseState || !resource.loopEffect_0))
		{
			mTaskManager.AddTask(new SkillUtilities.Task(resource.effectStartTime_0, create3DEffect, new EffectCreateStructure()
			{
				bindpoint = resource.effectBp_0,
				effectID = resource.effectID_0,
				loop = resource.loopEffect_0
			}));
		}

		if (resource.effectID_1 != uint.MaxValue && (chargingOrFirstUseState || !resource.loopEffect_1))
		{
			mTaskManager.AddTask(new SkillUtilities.Task(resource.effectStartTime_1, create3DEffect, new EffectCreateStructure()
			{
				bindpoint = resource.effectBp_1,
				effectID = resource.effectID_1,
				loop = resource.loopEffect_1
			}));
		}
	}

	/// <summary>
	/// 任何一个新的技能都从该方法进入实际的状态.
	/// </summary>
	private ErrorCode startSkill(Vector3 targetPosition)
	{
		SkillDetails.OnSkillStarted(mOwner, mSkillUsing);

		if(mSkillUsing.skillRes.chargeTime != 0)
		{
			SkillTargetPosition = targetPosition;
			switch2State(ActionSkillState.Charging);
			loadClientBehaviourTask(mSkillUsing.skillRes.chargeBehaviour);
			mTaskManager.Start();

			return ErrorCode.Succeeded;
		}

		// 没有准备时间, 直接进入使用状态.
		return EnterUseState(targetPosition);
	}

	/// <summary>
	/// 使使用者面朝targetPosition.
	/// </summary>
	private void ownerLookAt(Vector3 targetPosition)
	{
		mOwner.SetDirection(
			Utility.Vector3ToRadian(targetPosition - mOwner.GetPosition(),
			mOwner.GetDirection())	// 如果向当前位置使用技能, 那么不改变朝向.
			);
	}

	public bool SameSkill(BattleUnitSkill skill)
	{
		return mSkillUsing.skillRes.resID == skill.skillRes.resID;
	}

	protected override ErrorCode doStart(ActionInitParam param)
	{
		ActionSkillInitParam asInit = param as ActionSkillInitParam;
		
		mSkillUsing = asInit.skill;

		mSkillUseStateLoopLeft = mSkillUsing.skillRes.skillUseStateLoopLeft;
		
		mTaskManager = new SkillUtilities.TaskManager(onTaskFinished);

		mOwner.EnterFightState();

        if(IsRegularAttack)
        {
            //如果是普通攻击，武器播放射击动画
            mOwner.PlayWeaponAnim(AnimationNameDef.WeaponFire);
        }

		if (mOwner.IsCanRotation() && mSkillUsing.skillRes.autoAim)
			ownerLookAt(asInit.targetPosition);

		SkillTargetPosition = asInit.targetPosition;
		switch2State(ActionSkillState.Invalid);

		return canEnterState(ActionSkillState.Charging, asInit.targetPosition);
	}

	protected override void onStarted()
	{
		startSkill(SkillTargetPosition);

		// 此处添加的buff, 可以具有`不能使用技能, 不能攻击`等限制属性, 而不会打断当前的技能.
		// 因为当前的技能尚未加入管理器内部的容器.
		if (mSkillUsing.skillRes.buffToSkillUser != uint.MaxValue)
			ErrorHandler.Parse(
				mOwner.AddSkillEffect(new AttackerAttr(mOwner, (uint)mSkillUsing.skillRes.resID),
				SkillEffectType.Buff, mSkillUsing.skillRes.buffToSkillUser),
				"failed to add skillbuff on skill started"
			);
	}

	protected override ErrorCode doStop(bool finished)
	{
		mTaskManager.Stop();

        if (IsRegularAttack)
        {
            //如果是普通攻击,停止武器的射击动画
            mOwner.PlayWeaponAnim(AnimationNameDef.WeaponDefault);
        }

		SkillTargetPosition = Vector3.zero;
		switch2State(ActionSkillState.Invalid);

		mOwner.EnterFightState();
		return ErrorCode.Succeeded;
	}

	protected override void onStopped(bool finished)
	{
		if (mSkillUsing.skillRes.buffToSkillUser != uint.MaxValue)
			ErrorHandler.Parse(
			mOwner.RemoveSkillBuffByResID(mSkillUsing.skillRes.buffToSkillUser),
			"failed to remove skill buff on skill stopped"
			);

		if (finished)
			SkillDetails.OnSkillFinished(mOwner, mSkillUsing);

		mSkillUsing = null;
	}

	public override bool OnActiveFlagsStateChanged(ActiveFlagsDef flagName, bool increased)
	{
		if (!increased)
			return true;

		switch (flagName)
		{ 
			case ActiveFlagsDef.DisableAttack:
				return !mSkillUsing.IsRegularAttack;
			case ActiveFlagsDef.DisableSkillUse:
				return mSkillUsing.IsRegularAttack;
			default:
				break;
		}

		return true;
	}
	
	override protected UpdateRetCode onUpdate(uint elapsed)
	{
		if(mState == ActionSkillState.Invalid)
			return UpdateRetCode.Aborted;

		uint skillResID = dbgSkillResId;

		mTaskManager.Update(elapsed);

		// 技能效果可能把技能自身中断.
		if (mState == ActionSkillState.Invalid)
		{
			GameDebug.Log("skill " + skillResID + " interrupted by itself");
			return UpdateRetCode.Aborted;
		}

		UpdateRetCode retCode = UpdateRetCode.Continue;

		mStateTimer += elapsed;

		switch (mState)
		{
			case ActionSkillState.Charging:
				// 如果过了准备时间, 那么进入使用状态.
				if (mStateTimer > mSkillUsing.skillRes.chargeTime)
				{
					ErrorCode ec = EnterUseState(SkillTargetPosition);
					if (ec != ErrorCode.Succeeded)
						retCode = UpdateRetCode.Aborted;
				}
				break;

			case ActionSkillState.Using:
				if (mSkillUseStateLoopLeft == 0)
					retCode = UpdateRetCode.Finished;
				break;
		}

		return retCode;
	}

	/// <summary>
	/// 切换状态并清空参数.
	/// </summary>
	/// <param name="state"></param>
	private void switch2State(ActionSkillState state)
	{
		if (state != mState)
		{
			mState = state;
			mStateTimer = 0;

			SkillClientBehaviour.RemoveAll(mOwner, mLoopClientBehaviours);
			mTaskManager.Stop();
		}
	}

	#region Callbacks
	/// <summary>
	/// 所有任务正常完成时的回调.
	/// </summary>
	private void onTaskFinished()
	{
		if (mSkillUseStateLoopLeft == 0)
			ErrorHandler.Parse(ErrorCode.LogicError, "invalid loop count");
		else if (mState == ActionSkillState.Invalid)
			ErrorHandler.Parse(ErrorCode.LogicError, "invalid skill state");
		else if (mState == ActionSkillState.Using && --mSkillUseStateLoopLeft != 0)
		{
			Vector3 targetPosition = Vector3.zero;
			if (mOwner.IsCanRotation() && mSkillUsing.skillRes.autoAim
				&& (targetPosition = mOwner.GetAimTargetPos()) != Vector3.zero)
			{
				ownerLookAt(targetPosition);
				SkillTargetPosition = targetPosition;
			}

			loadUseStateTasks();
			mTaskManager.Start();
		}
	}

	/// <summary>
	/// 产生技能效果, 被task回调.
	/// </summary>
	private void skillTakeEffect(object obj)
	{
		// 将目标点缩放到最大距离和最小距离之间.
		Vector3 targetPosition = SkillUtilities.RoundTargetPosition(mSkillUsing.skillRes.minRange, mSkillUsing.skillRes.maxRange,
			mOwner.GetPosition(), SkillTargetPosition);

		uint skillId = dbgSkillResId;

		ErrorHandler.Parse(
			SkillDetails.SkillTakeEffect(mOwner, mSkillUsing.skillRes, targetPosition)
			);

		if (!IsRunning)
			ErrorHandler.Parse(ErrorCode.ConfigError, "技能" + skillId + "被自身所带效果异常终止!");
		else if (FirstUseState)
			SkillDetails.OnSkillEffected(mOwner, mSkillUsing);
	}

	void create3DEffect(object resource)
	{
		EffectCreateStructure argument = (EffectCreateStructure)resource;
		SkillClientBehaviour.AddEffect2Object(mOwner, argument.effectID, argument.bindpoint, float.NaN, argument.loop, mLoopClientBehaviours);
	}

	void playSound(object resource)
	{
		SkillClientBehaviourItem behaviour = resource as SkillClientBehaviourItem;
		string[] sounds = behaviour.soundResID.Split('|');
		string sound = sounds[UnityEngine.Random.Range(0, sounds.Length)];
		SkillClientBehaviour.PlaySound(uint.Parse(sound), behaviour.loopSound, mLoopClientBehaviours);
	}

	void playAnimation(object resource)
	{
		SkillClientBehaviourItem behaviour = resource as SkillClientBehaviourItem;
		SkillClientBehaviour.PlayAnimation(mOwner, behaviour.userAnimationName, behaviour.loopUserAnimation, mLoopClientBehaviours);
	}

	void playWeaponAnimation(object resource)
	{
		SkillClientBehaviourItem behaviour = resource as SkillClientBehaviourItem;
		SkillClientBehaviour.PlayWeaponAnimation(mOwner, behaviour.weaponAnimationName, behaviour.loopWeaponAnimation, mLoopClientBehaviours);
	}

	void playCameraEffect(object resource)
	{
		SkillClientBehaviourItem behaviour = resource as SkillClientBehaviourItem;
		CameraController.Instance.PlayCameraEffect(behaviour.cameraDist2Player, CameraController.Instance.Distance, behaviour.cameraMoveTime / 1000f);
	}
	#endregion

	#region Properties
	/// <summary>
	/// 技能在整个使用阶段的第一个循环内.
	/// </summary>
	bool FirstUseState
	{
		get { return mState == ActionSkillState.Using && mSkillUseStateLoopLeft == mSkillUsing.skillRes.skillUseStateLoopLeft; }
	}

	/// <summary>
	/// 获取目标点, 目标点是与技能发起者同平面的点.
	/// </summary>
	Vector3 SkillTargetPosition
	{
		get
		{
			return Utility.MoveVector3Towards(mOwner.GetPosition(), mOwner.GetDirection(), mTargetPositionDistance);
		}
		set
		{
			if (value != Vector3.zero)
				mTargetPositionDistance = Utility.Distance2D(value, mOwner.GetPosition());
		}
	}

	/// <summary>
	/// 技能在准备阶段时, 能否被使用者自身使用其它技能打断;
	/// 或者再次使用本技能, 通过二次使用技能而使技能进入使用阶段.
	/// </summary>
	/// <example>
	/// 可以使用在类似DotA光之守卫的冲击波的技能(第一次点击时, 开始蓄力; 第二次点击相同技能时, 释放).
	/// </example>
	public bool Interruptable
	{
		get {
			if(mSkillUsing == null)
			{
				ErrorHandler.Parse(ErrorCode.LogicError, "null skill resource");
				return true;
			}

			return mSkillUsing.skillRes.interruptable;
		}
	}

	/// <summary>
	/// 当前正在使用的技能的资源ID.
	/// </summary>
	uint dbgSkillResId
	{
		get {
			if(mSkillUsing == null)
			{
				ErrorHandler.Parse(ErrorCode.LogicError, "null skill resource");
				return uint.MaxValue;
			}

			return (uint)mSkillUsing.skillRes.resID;
		}
	}

	/// <summary>
	/// 当前是否在技能准备状态.
	/// </summary>
	public bool InChargingState
	{
		get {
			return mState == ActionSkillState.Charging;
		}
	}

	/// <summary>
	/// 是否为普通攻击, 普通攻击的优先级最低, 可以被其他技能打断.
	/// </summary>
	public bool IsRegularAttack
	{
		get { return mSkillUsing.skillRes.isRegularAttack; }
	}

	/// <summary>
	/// 当前是否在技能使用状态.
	/// </summary>
	public bool InUsingState
	{
		get {
			return mState == ActionSkillState.Using;
		}
	}
	#endregion
}
