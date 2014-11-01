using System.Collections;
using System.Collections.Generic;
public class SkillEffectInitParam
{
	public SkillEffectInitParam(BattleUnit owner, AttackerAttr attr)
	{
		attackerAttr = attr;
		this.owner = owner;
	}

	/// <summary>
	/// 效果的拥有者.
	/// </summary>
	public BattleUnit owner;

	/// <summary>
	/// 效果发起者的属性.
	/// </summary>
	public AttackerAttr attackerAttr;
}

/// <summary>
/// 技能效果的基类.
/// </summary>
public abstract class SkillEffect
{
	/// <summary>
	/// 攻击者的属性.
	/// </summary>
	protected AttackerAttr mAttackerAttr;
	
	private static readonly int StateCount = 4;
	private BitArray mState = new BitArray(StateCount);

	/// <summary>
	/// 是有为有效状态.
	/// </summary>
	protected static readonly int StateAwake = 0;

	protected BattleUnit mOwner = null;

	public virtual bool Initialize(SkillEffectInitParam param)
	{
		mAttackerAttr = param.attackerAttr;
		mOwner = param.owner;
		return true;
	}

	public ErrorCode Start(SkillEffectInitParam param)
	{
		ErrorCode err = doStart(param);
		if (err != ErrorCode.Succeeded)
			return err;

		setState(StateAwake, true);

		onStarted();
		return ErrorCode.Succeeded;
	}

	/// <summary>
	///返回当skilleffect目标的发生ownerEvent事件时, 该skilleffect是否需要移除.
	/// </summary>
	/// <param name="ownerEvent"></param>
	/// <returns></returns>
	public virtual bool NeedRemoveOnOwnerEvent(SkillEffectOwnerEventDef ownerEvent)
	{
		return false;
	}

	protected virtual ErrorCode doStart(SkillEffectInitParam param)
	{
		return ErrorCode.Succeeded;
	}

	protected virtual void onStarted()
	{
	}

	protected void setState(int state, bool value)
	{
		mState.Set(state, value);
	}

	protected bool getState(int pos)
	{ 
		return mState.Get(pos);
	}

	/// <summary>
	/// 结束一个技能效果.
	/// </summary>
	public ErrorCode Stop(SkillEffectStopReason stopReason)
	{
		if (!getState(StateAwake))
			return ErrorCode.Succeeded;

		setState(StateAwake, false);

		ErrorCode err = doStop();
		if (err != ErrorCode.Succeeded)
			return err;

		onStopped(stopReason);
		return ErrorCode.Succeeded;
	}

	protected virtual ErrorCode doStop()
	{
		return ErrorCode.Succeeded;
	}

	protected virtual void onStopped(SkillEffectStopReason stopReason)
	{
	}

	public UpdateRetCode Update(uint elapsed)
	{
		return IsAwake ? onUpdate(elapsed) : UpdateRetCode.Aborted;
	}

	protected virtual UpdateRetCode onUpdate(uint elapsed)
	{
		return UpdateRetCode.Continue;
	}

	/// <summary>
	/// 技能效果是否需要更新.
	/// </summary>
	public abstract bool NeedUpdate { get; }

	/// <summary>
	/// 技能效果类型.
	/// </summary>
	public abstract SkillEffectType Type { get; }

	/// <summary>
	/// 技能效果资源ID.
	/// </summary>
	public abstract uint ResID { get; }

	/// <summary>
	/// 返回该effect现在是否有效.
	/// </summary>
	public bool IsAwake { get { return getState(StateAwake); } }
}
