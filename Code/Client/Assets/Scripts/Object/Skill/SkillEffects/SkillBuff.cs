using System.Collections.Generic;

public class SkillBuffInitParam : SkillEffectInitParam
{
	public SkillBuffInitParam(BattleUnit owner, AttackerAttr attr, uint buffResID)
		: base(owner, attr)
	{
		buffResource = DataManager.BuffTable[buffResID] as SkillBuffTableItem;
		if (buffResource == null)
			SkillUtilities.ResourceNotFound("buff", buffResID);
	}

	public SkillBuffTableItem buffResource;
}

public class SkillBuff : SkillEffect
{
	// 接SkillEffect.StateAwake, 不能超过SkillEffect.StateCount.
	private static readonly int StateEnabled = SkillEffect.StateAwake + 1;

	SkillBuffTableItem mBuffRes;

	uint mLastUseWeapon = uint.MaxValue;
	
	// buff的叠加层数.
	private uint mStackCount = 0;

	// buff的生命时间.
	private uint mLifeTime = 0;

	// buff的创建时间.
	private ulong mCreatedTime = ulong.MaxValue;

	private SkillUtilities.TaskManager mTaskManager = null;

	// 当前buff是否对owner进行了控制.
	private bool mStunEnabled = false;

	// buff改变的属性.
	List<Pair<int, int>> mProperties = null;

	List<Pair<uint, string>> mSkillTransform = null;

	// buff的前端表现.
	ClientBehaviourIdContainer mClientBehaviourIdContainer = new ClientBehaviourIdContainer();

	public override bool Initialize(SkillEffectInitParam param)
	{
		SkillBuffInitParam buffInit = param as SkillBuffInitParam;

		mBuffRes = buffInit.buffResource;

		return mBuffRes != null && base.Initialize(param);
	}

	protected override ErrorCode doStart(SkillEffectInitParam param)
	{
		// 如果单位免疫控制, 且该buff为控制类buff, 那么, 该buff不能被添加,
		// 无论该buff有什么其它的功能.
		if (!mOwner.CanBeStuned() && mBuffRes.IsStunBuff)
			return ErrorCode.AddEffectFailedSkillEffectImmunity;

		if (mBuffRes.dotEffectTimeInterval != uint.MaxValue)
			mTaskManager = new SkillUtilities.TaskManager(onTaskFinished);

		mProperties = SkillUtilities.ParseProperties(mBuffRes.properties);
		mSkillTransform = SkillUtilities.ParseSkillTransform(mBuffRes.skillTransform);
		return base.doStart(param);
	}

	protected override void onStarted()
	{
        mCreatedTime = TimeUtilities.GetNow();

		ErrorHandler.Parse(
			enable(true, SkillEffectStopReason.Invalid)
			);

		// 无论buff是否enable, dot效果的计时不会停止.
		if (mTaskManager != null)
		{
			// 第一次DOT效果立即产生, 之后的按照配置中的间隔产生.
			SkillUtilities.Task task = new SkillUtilities.Task(0, buffDotEffect, null);
			mTaskManager.AddTask(task);
			mTaskManager.Start();
		}

		base.onStarted();
	}

	protected override void onStopped(SkillEffectStopReason stopReason)
	{
		if (getState(StateEnabled))
			ErrorHandler.Parse(enable(false, stopReason));

		// buff被回收时, 不会再继续触发效果.
		if (stopReason != SkillEffectStopReason.Recycled)
		{
			// 消失时的特效.
			SkillClientBehaviour.AddEffect2Object(mOwner, mBuffRes.endEffectID, mBuffRes.endEffectBindpoint, 0f);

			// buff死亡创建.
			// 对buff拥有者产生效果.
			SkillDetails.AddSkillEffectByResource(mAttackerAttr, mOwner, mBuffRes.effect2OwnerOnExpired);

			// 对buff拥有者周围产生效果.
			SkillDetails.SelectTargetAndAddSkillEffect(mAttackerAttr, mOwner.GetPosition(),
				mOwner.GetDirection(),
				mBuffRes.targetSelectionOnExpired, mBuffRes.effect2OthersOnExpired
				);

			// 在buff拥有者周围进行创建.
			SkillDetails.CreateCreationAround(mAttackerAttr, mBuffRes.creationAroundOwnerOnExpired, mOwner.GetPosition(), mOwner.GetDirection());
		}

		base.onStopped(stopReason);
	}

	/// <summary>
	/// buff开始作用.
	/// </summary>
	protected ErrorCode enable(bool bEnabled, SkillEffectStopReason stopReason)
	{
		if (getState(StateEnabled) == bEnabled)
			return ErrorCode.Succeeded;

		setState(StateEnabled, bEnabled);

		onEnabled(bEnabled, stopReason);

		return ErrorCode.Succeeded;
	}

	/// <summary>
	/// buff开始作用. 添加属性, 动态标记等.
	/// </summary>
	protected virtual void onEnabled(bool bEnabled, SkillEffectStopReason stopReason)
	{
		// 特效
		if (bEnabled)
		{
			SkillClientBehaviour.AddEffect2Object(mOwner, mBuffRes._3DEffectID, mBuffRes._3DEffectBindpoint, 0f, mBuffRes.loop3DEffect, mClientBehaviourIdContainer);
			SkillClientBehaviour.PlayAnimation(mOwner, mBuffRes.animationName, mBuffRes.loopAnimation, mClientBehaviourIdContainer);
		}
		else
		{
			SkillClientBehaviour.RemoveAll(mOwner, mClientBehaviourIdContainer);
		}

		// 检查目标是否可以被控制, 并保存标记, 表示该buff是否对目标产生了控制.
		if ((bEnabled && mOwner.CanBeStuned()) || (!bEnabled && mStunEnabled))
		{
			enableStun(bEnabled);
			mStunEnabled = bEnabled;
		}

		if (mBuffRes.stunImmunity)
			mOwner.AddActiveFlag(ActiveFlagsDef.StunImmunity, bEnabled, true);

		if (mBuffRes.damageImmunity)
			mOwner.AddActiveFlag(ActiveFlagsDef.DamageImmunity, bEnabled, true);

		if (mBuffRes.inviolability)
			mOwner.AddActiveFlag(ActiveFlagsDef.Inviolability, bEnabled, true);

		if (mBuffRes.magneticEffect)
			mOwner.AddActiveFlag(ActiveFlagsDef.MagneticEffect, bEnabled, true);

		if (mBuffRes.strokeEffect)
			mOwner.AddActiveFlag(ActiveFlagsDef.StrokeEffect, bEnabled, true);

		if (mBuffRes.randEvent != uint.MaxValue)
		{
			if (bEnabled)
				mOwner.AddRandEvent(mBuffRes, mAttackerAttr);
			else
				mOwner.RemoveRandEvent(mBuffRes, stopReason);
		}

		if (bEnabled) // mStackCount == 0.
		{
			AddStack();
			if (mBuffRes.newModelID != uint.MaxValue)
				mOwner.Transform((int)mBuffRes.newModelID);
			if (mBuffRes.newWeaponID != uint.MaxValue)
			{
				mOwner.ChangeWeapon((int)mBuffRes.newWeaponID);
				mOwner.AddActiveFlag(ActiveFlagsDef.DisableChangeWeaponModel, true, false);
			}

			if (mBuffRes.superNewWeaponID != uint.MaxValue)
			{
				if (mOwner is Player)
				{
					if ((mOwner as Player).HasSuperWeapon())
						(mOwner as Player).UnEquipSuperWeapon();

					mLastUseWeapon = (uint)mOwner.GetMainWeaponID();
					(mOwner as Player).SceneChangeWeapon((int)mBuffRes.superNewWeaponID);
					mOwner.AddActiveFlag(ActiveFlagsDef.DisableChangeWeapon, true, false);
				}
				else
				{
					GameDebug.LogError("只有玩家支持使用buff更换武器");
				}
			}

			if (mSkillTransform != null)
				mOwner.SkillTransform(mSkillTransform);
		}
		else
		{
			removeAllStacks();
			if (mBuffRes.newModelID != uint.MaxValue)
				mOwner.UndoTransform();
			if (mBuffRes.newWeaponID != uint.MaxValue && mOwner is Player)
			{
				mOwner.AddActiveFlag(ActiveFlagsDef.DisableChangeWeaponModel, false, false);
				mOwner.ChangeWeapon(mOwner.GetMainWeaponID());
			}

			if (mBuffRes.superNewWeaponID != uint.MaxValue)
			{
				if (mOwner is Player)
				{
					mOwner.AddActiveFlag(ActiveFlagsDef.DisableChangeWeapon, false, false);
					(mOwner as Player).SceneChangeWeapon((int)mLastUseWeapon);
				}
				else
				{
					GameDebug.LogError("只有玩家支持使用buff更换武器");
				}
			}

			if (mSkillTransform != null)
				mOwner.SkillTransform(null);
		}
	}

	/// <summary>
	/// 开启/关闭该buff所带的控制标记.
	/// </summary>
	/// <param name="bEnabled"></param>
	void enableStun(bool bEnabled)
	{
		if (mBuffRes.disableAttack)
			mOwner.AddActiveFlag(ActiveFlagsDef.DisableAttack, bEnabled, true);

		if (mBuffRes.disableMovement)
			mOwner.AddActiveFlag(ActiveFlagsDef.DisableMovement, bEnabled, true);

		if (mBuffRes.disableSkillUse)
			mOwner.AddActiveFlag(ActiveFlagsDef.DisableSkillUse, bEnabled, true);

		if (mBuffRes.disableRotate)
			mOwner.AddActiveFlag(ActiveFlagsDef.DisableRotate, bEnabled, true);
	}

	/// <summary>
	/// 修改owner的属性.
	/// </summary>
	/// <param name="addProperty">true表示作用buff资源中的属性, false表示反作用</param>
	private void modifyProperty(bool addProperty)
	{
		if (mProperties == null)
			return;

		// 增加时, 只增加一层.
		// 减少时, 需要将之前叠加的全部移除.
		int stack = (int)(addProperty ? 1 : mStackCount);
		
		if (!addProperty) stack = -stack;

		foreach (Pair<int, int> value in mProperties)
		{
			int property = value.second * (int)stack;
			mOwner.ModifyPropertyValue(value.first, property);
		}
	}

	/// <summary>
	/// 重置buff的生命时间(不修改创建时间).
	/// </summary>
	private void resetBuffLifeTime()
	{
		mLifeTime = mBuffRes.lifeMilliseconds;
	}

	/// <summary>
	/// buff的dot效果, 每隔指定时间, 产生一次效果.
	/// </summary>
	/// <param name="param"></param>
	private void buffDotEffect(object param)
	{
		// dot的计时不会停止, 但是到了产生效果的时间时, 需要检查当前buff是否启用.
		if (getState(StateEnabled))
		{
			ErrorHandler.Parse(
				SkillDetails.AddSkillEffectByResource(mAttackerAttr, mOwner, mBuffRes.dotEffect2Owner),
				"failed to add dot effect to buff owner"
			);

			SkillDetails.SelectTargetAndAddSkillEffect(mAttackerAttr, mOwner.GetPosition(),
				mOwner.GetDirection(),
				mBuffRes.dotEffectTargetSelection,
				mBuffRes.dotEffect2Others
				);
		}
	}

	private void onTaskFinished()
	{
		if (mTaskManager != null)
		{
			mTaskManager.AddTask(new SkillUtilities.Task(mBuffRes.dotEffectTimeInterval, buffDotEffect, null));
			mTaskManager.Start();
		}
	}

	protected override UpdateRetCode onUpdate(uint elapsed)
	{
		if (!getState(StateAwake))
		{
			ErrorHandler.Parse(ErrorCode.ConfigError, "skillbuff isn't awake");
			return UpdateRetCode.Aborted;
		}

		if (mTaskManager != null)
			mTaskManager.Update(elapsed);

		// buff本身所带的效果可能终止自身.
		if (!getState(StateAwake))
			return UpdateRetCode.Aborted;

		if (mBuffRes.lifeMilliseconds == uint.MaxValue)
			return UpdateRetCode.Continue;

		if (mLifeTime < elapsed || (mLifeTime -= elapsed) == 0)
		{
			return UpdateRetCode.Finished;
		}

		return UpdateRetCode.Continue;
	}

	/// <summary>
	/// 检查当前buff是否与mutex互斥类型互斥.
	/// </summary>
	public bool IsMutuallyExclusive(uint mutex)
	{
		return (mutex != uint.MaxValue && mBuffRes.mutex == mutex);
	}

	public bool SameGroup(uint groupID)
	{
		return mBuffRes != null && mBuffRes.group == groupID;
	}

	public override bool NeedRemoveOnOwnerEvent(SkillEffectOwnerEventDef ownerEvent)
	{
		return mBuffRes == null || containsBit((uint)mBuffRes.removeCondition, (int)ownerEvent);
	}

	/// <summary>
	/// number的第bitPos位, 是否为非0.
	/// </summary>
	private bool containsBit(uint number, int bitPos)
	{
		return (number & (1 << bitPos)) != 0;
	}

	/// <summary>
	/// 增加buff属性的叠加层数, 并重置buff作用时间(不会改变buff的创建时间).
	/// </summary>
	/// <returns></returns>
	public ErrorCode AddStack()
	{
		// 无论buff叠加次数是否达到最大值, 都重置时间.
		// 最大叠加次数不可以为0.
		resetBuffLifeTime();

		if (mStackCount != 0 && mBuffRes.randEvent != uint.MaxValue) {
			EventSystem.Instance.PushEvent(new ResetRandEventDurationEvent(mOwner.InstanceID, (uint)mBuffRes.resID));
		}

		if (mStackCount >= mBuffRes.stackCountMax)
			return ErrorCode.MaxStackCount;

		modifyProperty(true);

		++mStackCount;
		
		return ErrorCode.Succeeded;
	}

	/// <summary>
	/// 移除所有叠加的属性, 并清空叠加次数.
	/// </summary>
	/// <returns></returns>
	private void removeAllStacks()
	{
		modifyProperty(false);
		mStackCount = 0;
	}

	public override bool NeedUpdate
	{
		get { return true; }
	}

	public override SkillEffectType Type
	{
		get { return SkillEffectType.Buff; }
	}

	public override uint ResID
	{
		get { return (uint)mBuffRes.resID; }
	}

	/// <summary>
	/// 该buff是否带有控制效果.
	/// </summary>
	public bool HasStunEffect
	{
		get {
			return mStunEnabled && mBuffRes.IsStunBuff; 
		}
	}
}
