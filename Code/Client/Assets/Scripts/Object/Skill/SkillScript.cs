using System;
using UnityEngine;

public interface SkillScriptAllocator
{ 
	SkillScript Allocate(BattleUnit owner);
}

public struct SkillScriptStartArgument
{
	/// <summary>
	/// 添加该buff的单位的属性.
	/// </summary>
	public AttackerAttr buffCreaterAttr;
	/// <summary>
	/// 所属buff的资源.
	/// </summary>
	public SkillBuffTableItem buffRes;
	public int argument_0;
	public int argument_1;
	public int argument_2;
}

/// <summary>
/// 由skillrandevent调用的脚本效果, 与对应的skillrandevent一起触发.
/// </summary>
public abstract class SkillScript
{
	protected BattleUnit mOwner;
	/// <summary>
	/// Owner的属性.
	/// </summary>
	protected readonly AttackerAttr OwnerAttribute;
	public SkillScript(BattleUnit owner)
	{
		mOwner = owner;
		OwnerAttribute = new AttackerAttr(mOwner, uint.MaxValue, true);
	}

	/// <summary>
	/// 使用该脚本随机事件的是否是主玩家.
	/// </summary>
	protected bool IsMainPlayerSkillScript { get { return mOwner is Player; } }

	/// <summary>
	/// 该脚本被什么类型的randevent触发.
	/// </summary>
	public abstract RandEventTriggerType ScriptTriggerType { get; }

	/// <summary>
	/// 初始化脚本.
	/// </summary>
	/// <returns>是否成功, 如果失败, 脚本永不会触发.</returns>
	public abstract bool StartScript(SkillScriptStartArgument startArg);

	/// <summary>
	/// 随机事件被触发, 执行该随机事件脚本.
	/// </summary>
	/// <returns>如果返回false, 该脚本所来源的BUFF需要被移除.</returns>
	public abstract bool RunScript(RandEventArg argument);

	/// <summary>
	/// 脚本来源的BUFF被结束, 进行最终结算.
	/// </summary>
	public abstract void StopScript(SkillEffectStopReason stopReason);
}

/// <summary>
/// 吸收固定伤害数值, 类似DotA中LOA的无光之盾(AphoticShield).
/// </summary>
public class SS_AphoticShield : SkillScript
{
	public class Allocator : SkillScriptAllocator
	{
		public SkillScript Allocate(BattleUnit owner)
		{
			return new SS_AphoticShield(owner);
		}
	}
	/// 能抵挡的伤害值.
	int remaining = 0;
	/// 总共能抵挡的伤害值.
	int capacity = 0;

	/// <summary>
	/// owner可以吸收一定的伤害.
	/// </summary>
	public SS_AphoticShield(BattleUnit owner)
		: base(owner)
	{
	}

	public override RandEventTriggerType ScriptTriggerType
	{
		get { return RandEventTriggerType.OnBeAttacked; }
	}

	/// <summary>
	/// 按什么方式创建吸收盾能吸收的伤害上限.
	/// </summary>
	enum AphoticShieldType : int
	{ 
		// 按当前血量的百分比.
		PercentageOfCurrentHp,
		// 按绝对值.
		Constant,
	}

	public override bool StartScript(SkillScriptStartArgument argument)
	{
		if (argument.argument_1 <= 0)
			return ErrorHandler.Parse(ErrorCode.ConfigError, "SS_AphoticShield: invalid 2nd argument");

		switch ((AphoticShieldType)argument.argument_0)
		{
			case AphoticShieldType.PercentageOfCurrentHp:
				capacity = remaining = (int)(argument.argument_1 * mOwner.GetHP() / 100f);
				break;
			case AphoticShieldType.Constant:
				capacity = remaining = argument.argument_1;
				break;
			default:
				return ErrorHandler.Parse(ErrorCode.ConfigError, "SS_AphoticShield: invalid 1st argument");
		}

		mOwner.ShowShieldUI(true);
		mOwner.SetShieldUIProgress(1f);
		return true;
	}

	public override bool RunScript(RandEventArg argument)
	{
		BeAttackedEventArg beAtkArg = argument as BeAttackedEventArg;

		// 新的伤害值不大于当前能抵挡的伤害, 更新当前可以抵挡的伤害, 并清空新的伤害.
		if (remaining >= beAtkArg.PositiveDamage)
		{
			remaining -= beAtkArg.PositiveDamage;
			beAtkArg.PositiveDamage = 0;
		}
		else
		{	// 吸收足够的伤害值.
			beAtkArg.PositiveDamage -= remaining;
			remaining = 0;
		}

		mOwner.SetShieldUIProgress(((float)remaining) / capacity);

		// 吸收足够的伤害值之后, 该效果消失.
		return remaining > 0;
	}

	public override void StopScript(SkillEffectStopReason stopReason)
	{
		mOwner.ShowShieldUI(false);
	}
}

/// <summary>
/// 弹道效果改变.
/// </summary>
public class SS_Ballistics : SkillScript
{
	public class Allocator : SkillScriptAllocator
	{
		public SkillScript Allocate(BattleUnit owner)
		{
			return new SS_Ballistics(owner);
		}
	}

	uint buffResID = uint.MaxValue;
	uint duration = uint.MaxValue;
	public SS_Ballistics(BattleUnit owner)
		: base(owner)
	{
	}

	public override RandEventTriggerType ScriptTriggerType
	{
		get { return RandEventTriggerType.OnStartRegularSkill; }
	}

	public override bool StartScript(SkillScriptStartArgument argument)
	{
		// 显示武器强化的界面.
		if (IsMainPlayerSkillScript)
		{
			buffResID = (uint)argument.buffRes.resID;
			duration = argument.buffRes.lifeMilliseconds;
			EventSystem.Instance.addEventListener(ResetRandEventDurationEvent.RESET_RAND_EVENT_DURATION, onDurationReset);

			ReloadEvent evt = new ReloadEvent(ReloadEvent.WEAPON_UP_EVENT);
			evt.reload_time = (int)argument.buffRes.lifeMilliseconds;
			EventSystem.Instance.PushEvent(evt);
		}

		return mOwner is Role;
	}

	public override bool RunScript(RandEventArg argument)
	{
		StartRegularSkillEventArg regularSkillArg = argument as StartRegularSkillEventArg;
		Role role = mOwner as Role;
		int weaponID = role.GetMainWeaponID();
		WeaponTableItem weaponTable = (weaponID >= 0) 
			? DataManager.WeaponTable[weaponID] as WeaponTableItem : null;
		
		// 
		if (weaponTable == null)
			return ErrorHandler.Parse(ErrorCode.ConfigError, "武器ID[" + weaponID + "]不存在weapon.txt中, 加强buff将被移除");

		if (weaponTable.skill_2 < 0)
			return ErrorHandler.Parse(ErrorCode.ConfigError, "武器[" + weaponID + "]没有强化技能, 加强buff将被移除");

		regularSkillArg.SkillResID = (uint)weaponTable.skill_2;
		
		return true;
	}

	public override void StopScript(SkillEffectStopReason stopReason)
	{
		// 隐藏武器强化的界面.
		if (IsMainPlayerSkillScript)
		{
			EventSystem.Instance.removeEventListener(ResetRandEventDurationEvent.RESET_RAND_EVENT_DURATION, onDurationReset);
			ReloadEvent evt = new ReloadEvent(ReloadEvent.WEAPON_UP_REMOVE_EVENT);
			EventSystem.Instance.PushEvent(evt);
		}
	}

	void onDurationReset(EventBase e)
	{
		ResetRandEventDurationEvent evt = (ResetRandEventDurationEvent)e;
		if (mOwner.InstanceID == evt.OwnerInstanceID && evt.BuffID == buffResID)
		{
			ReloadEvent reloadEvent = new ReloadEvent(ReloadEvent.WEAPON_UP_EVENT);
			reloadEvent.reload_time = (int)duration;
			EventSystem.Instance.PushEvent(reloadEvent);
		}
	}
}

/// <summary>
/// 吸血效果.
/// </summary>
public class SS_Vampirism : SkillScript
{
	public class Allocator : SkillScriptAllocator
	{
		public SkillScript Allocate(BattleUnit owner)
		{
			return new SS_Vampirism(owner);
		}
	}
	// 将伤害值的百分比, 转换为攻击者的生命.
	float percent;

	public SS_Vampirism(BattleUnit owner)
		: base(owner)
	{
	}

	public override RandEventTriggerType ScriptTriggerType
	{
		get { return RandEventTriggerType.OnAtkOthers; }
	}

	public override bool StartScript(SkillScriptStartArgument argument)
	{
		percent = argument.argument_0 / 100f;
		return true;
	}

	public override bool RunScript(RandEventArg argument)
	{
		AtkOthersEventArg atkEventArg = argument as AtkOthersEventArg;
		int hpRegenerated = (int)(atkEventArg.FireDamage * percent);
		if (hpRegenerated != 0)
			mOwner.AddDamage(new DamageInfo() { Value = hpRegenerated }, OwnerAttribute);

		return true;
	}

	public override void StopScript(SkillEffectStopReason stopReason)
	{
	}
}

/// <summary>
/// 死亡创建.
/// </summary>
public class SS_DeathCreation : SkillScript
{
	public class Allocator : SkillScriptAllocator
	{
		public SkillScript Allocate(BattleUnit owner)
		{
			return new SS_DeathCreation(owner);
		}
	}
	AttackerAttr createrAttr;
	uint creationId = uint.MaxValue;

	public SS_DeathCreation(BattleUnit owner)
		: base(owner)
	{ 
	}

	public override RandEventTriggerType ScriptTriggerType
	{
		get { return RandEventTriggerType.OnBeKilled; }
	}

	public override bool StartScript(SkillScriptStartArgument argument)
	{
		creationId = (uint)argument.argument_0;
		createrAttr = argument.buffCreaterAttr;

		return true;
	}

	public override bool RunScript(RandEventArg argument)
	{
		ErrorHandler.Parse(
			SkillDetails.CreateCreationAround(createrAttr, creationId, mOwner.GetPosition(), mOwner.GetDirection()),
			"failed to run script SS_DeathCreation"
		);

		return true;
	}

	public override void StopScript(SkillEffectStopReason stopReason)
	{
	}
}

/// <summary>
/// 尸爆, 死亡之后, 对周围产生技能效果.
/// </summary>
public class SS_CorpseExplosion : SkillScript
{
	public class Allocator : SkillScriptAllocator
	{
		public SkillScript Allocate(BattleUnit owner)
		{
			return new SS_CorpseExplosion(owner);
		}
	}
	uint targetSelection = uint.MaxValue;
	uint skillEffect = uint.MaxValue;

	public SS_CorpseExplosion(BattleUnit owner)
		: base(owner)
	{
	}

	public override RandEventTriggerType ScriptTriggerType
	{
		get { return RandEventTriggerType.OnBeKilled; }
	}

	public override bool StartScript(SkillScriptStartArgument argument)
	{
		targetSelection = (uint)argument.argument_0;
		if (targetSelection == uint.MaxValue)
			return ErrorHandler.Parse(ErrorCode.ConfigError, "SS_CorpseExplosion: invalid 1st argument");

		skillEffect = (uint)argument.argument_1;
		if (skillEffect == uint.MaxValue)
			return ErrorHandler.Parse(ErrorCode.ConfigError, "SS_CorpseExplosion: invalid 2nd argument");

		return true;
	}

	public override bool RunScript(RandEventArg argument)
	{
		ErrorHandler.Parse(
			SkillDetails.SelectTargetAndAddSkillEffect(OwnerAttribute,
				mOwner.GetPosition(),
				mOwner.GetDirection(), targetSelection, skillEffect),
				"failed to run script SS_CorpseExplosion"
			);

		return true;
	}

	public override void StopScript(SkillEffectStopReason stopReason)
	{
	}
}

/// <summary>
/// 攻击特效(攻击到对方时, 给对方添加其他技能效果).
/// </summary>
public class SS_SkillEffectPlacer : SkillScript
{
	public class Allocator : SkillScriptAllocator
	{
		public SkillScript Allocate(BattleUnit owner)
		{
			return new SS_SkillEffectPlacer(owner);
		}
	}
	uint skillEffectID = uint.MaxValue;

	public SS_SkillEffectPlacer(BattleUnit owner)
		: base(owner)
	{
	}

	public override RandEventTriggerType ScriptTriggerType
	{
		get { return RandEventTriggerType.OnAtkOthers; }
	}

	public override bool StartScript(SkillScriptStartArgument argument)
	{
		skillEffectID = (uint)argument.argument_0;
		if (skillEffectID == uint.MaxValue)
			return ErrorHandler.Parse(ErrorCode.ConfigError, "SS_SkillEffectPlacer: invalid 1st argument");

		return true;
	}

	public override bool RunScript(RandEventArg argument)
	{
		AtkOthersEventArg atkOthersArg = argument as AtkOthersEventArg;

		ErrorHandler.Parse(
			SkillDetails.AddSkillEffectByResource(OwnerAttribute,
				atkOthersArg.Target,
				skillEffectID),
				"failed to run script SS_SkillEffectPlacer"
			);

		return true;
	}

	public override void StopScript(SkillEffectStopReason stopReason)
	{
	}
}

/// <summary>
/// 重生! 瞬间回复满血量和蓝量.
/// </summary>
public class SS_Reincarnation : SkillScript
{
	public class Allocator : SkillScriptAllocator
	{
		public SkillScript Allocate(BattleUnit owner)
		{
			return new SS_Reincarnation(owner);
		}
	}

	public SS_Reincarnation(BattleUnit owner)
		: base(owner)
	{
	}

	public override RandEventTriggerType ScriptTriggerType
	{
		get { return RandEventTriggerType.OnFatalStrike; }
	}

	public override bool StartScript(SkillScriptStartArgument argument)
	{
		return true;
	}

	public override bool RunScript(RandEventArg argument)
	{
		FatalStrikeEventArg fatalStrikeArg = argument as FatalStrikeEventArg;

		fatalStrikeArg.BlockThisDamage = true;

		int hpMax = mOwner.GetPropertyValue((int)PropertyTypeEnum.PropertyTypeMaxHP);
		int manaMax = mOwner.GetPropertyValue((int)PropertyTypeEnum.PropertyTypeMaxMana);

		// 回满血量和魔量.
		mOwner.ModifyPropertyValue((int)PropertyTypeEnum.PropertyTypeHP, hpMax);
		mOwner.ModifyPropertyValue((int)PropertyTypeEnum.PropertyTypeMana, manaMax);

		// 一次重生之后, 移除buff.
		return false;
	}

	public override void StopScript(SkillEffectStopReason stopReason)
	{
	}
}

/// <summary>
/// 为事件目标添加技能效果.
/// 该类为抽象类, 子类制定在何时触发添加技能效果.
/// </summary>
public abstract class SkillEffectGenerator : SkillScript
{ 
	uint skillEffectID = uint.MaxValue;
	public SkillEffectGenerator(BattleUnit owner)
		: base(owner)
	{
	}

	public override bool StartScript(SkillScriptStartArgument argument)
	{
		skillEffectID = (uint)argument.argument_0;
		return true;
	}

	public override bool RunScript(RandEventArg argument)
	{
		ErrorHandler.Parse(
			SkillDetails.AddSkillEffectByResource(OwnerAttribute, mOwner, skillEffectID),
			"SkillEffectGenerator: failed to add skill effect"
			);

		return true;
	}

	public override void StopScript(SkillEffectStopReason stopReason)
	{
	}
}

/// <summary>
/// 被攻击时, 为自己添加技能效果.
/// </summary>
public class SS_CounterSkillEffectGenerator : SkillEffectGenerator
{
	public class Allocator : SkillScriptAllocator
	{
		public SkillScript Allocate(BattleUnit owner)
		{
			return new SS_CounterSkillEffectGenerator(owner);
		}
	}

	public SS_CounterSkillEffectGenerator(BattleUnit owner)
		: base(owner)
	{
	}

	public override RandEventTriggerType ScriptTriggerType
	{
		get { return RandEventTriggerType.OnBeAttacked; }
	}
}

/// <summary>
/// 暴击他人时, 为自己添加技能效果..
/// </summary>
public class SS_CriticalStrikeSkillEffectGenerator : SkillEffectGenerator
{
	public class Allocator : SkillScriptAllocator
	{
		public SkillScript Allocate(BattleUnit owner)
		{
			return new SS_CriticalStrikeSkillEffectGenerator(owner);
		}
	}

	public SS_CriticalStrikeSkillEffectGenerator(BattleUnit owner)
		: base(owner)
	{
	}

	public override RandEventTriggerType ScriptTriggerType
	{
		get { return RandEventTriggerType.OnCriticalStrikeOthers; }
	}
}

/// <summary>
/// 成功使用任何非普通攻击的技能之后, 给自身添加技能效果.
/// </summary>
/// <remarks>类似DotA中风暴之灵的超负荷(Overload)</remarks>
public class SS_Overload : SkillEffectGenerator
{
	public class Allocator : SkillScriptAllocator
	{
		public SkillScript Allocate(BattleUnit owner)
		{
			return new SS_Overload(owner);
		}
	}

	public SS_Overload(BattleUnit owner)
		: base(owner)
	{ 
	}

	public override RandEventTriggerType ScriptTriggerType
	{
		get { return RandEventTriggerType.OnSkillFinished; }
	}
}

/// <summary>
/// 完全抵挡数次伤害.
/// </summary>
/// <remarks>类似DotA中圣堂刺客的折光(Refraction)</remarks>
public class SS_Refraction : SkillScript
{
	public class Allocator : SkillScriptAllocator
	{
		public SkillScript Allocate(BattleUnit owner)
		{
			return new SS_Refraction(owner);
		}
	}

	uint leftCount = 0;
	public SS_Refraction(BattleUnit owner)
		: base(owner)
	{
	}

	public override RandEventTriggerType ScriptTriggerType
	{
		get { return RandEventTriggerType.OnBeAttacked; }
	}

	public override bool StartScript(SkillScriptStartArgument startArg)
	{
		leftCount = (uint)startArg.argument_0;
		return true;
	}

	public override bool RunScript(RandEventArg argument)
	{
		if (leftCount == 0)
			return ErrorHandler.Parse(ErrorCode.LogicError, "invalid left count for skill script SS_Refraction");

		// 完全抵挡这次伤害.
		BeAttackedEventArg eventArg = argument as BeAttackedEventArg;
		eventArg.PositiveDamage = 0;
		return (--leftCount != 0);
	}

	public override void StopScript(SkillEffectStopReason stopReason)
	{
	}
}

public class SkillScriptFactory
{
	static SkillScriptFactory instance = new SkillScriptFactory();
	static public SkillScriptFactory Instance { get { return instance; } }

	SkillScriptAllocator[] allocators = new SkillScriptAllocator[(int)SkillScriptDef.Count];

	SkillScriptFactory()
	{
		allocators[(int)SkillScriptDef.AphoticShield] = new SS_AphoticShield.Allocator();
		allocators[(int)SkillScriptDef.Ballistics] = new SS_Ballistics.Allocator();
		allocators[(int)SkillScriptDef.Vampirism] = new SS_Vampirism.Allocator();
		allocators[(int)SkillScriptDef.DeathCreation] = new SS_DeathCreation.Allocator();
		allocators[(int)SkillScriptDef.CorpseExplosion] = new SS_CorpseExplosion.Allocator();
		allocators[(int)SkillScriptDef.SkillEffectPlacer] = new SS_SkillEffectPlacer.Allocator();
		allocators[(int)SkillScriptDef.Reincarnation] = new SS_Reincarnation.Allocator();
		allocators[(int)SkillScriptDef.CounterSkillEffectGenerator] = new SS_CounterSkillEffectGenerator.Allocator();
		allocators[(int)SkillScriptDef.CriticalStrikeSkillEffectGenerator] = new SS_CriticalStrikeSkillEffectGenerator.Allocator();
		allocators[(int)SkillScriptDef.Overload] = new SS_Overload.Allocator();
		allocators[(int)SkillScriptDef.Refraction] = new SS_Refraction.Allocator();
	}

	public SkillScript Allocate(SkillScriptDef type, BattleUnit owner)
	{
		if (type > SkillScriptDef.Count)
		{
			ErrorHandler.Parse(ErrorCode.LogicError, "invalid skill script type");
			return null;
		}

		return allocators[(int)type].Allocate(owner);
	}
}
