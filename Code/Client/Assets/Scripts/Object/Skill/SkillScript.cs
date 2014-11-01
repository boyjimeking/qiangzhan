using System;
using UnityEngine;

public interface SkillScriptAllocator
{ 
	SkillScript Allocate(BattleUnit owner);
}

public struct SkillScriptStartArgument
{
	/// <summary>
	/// ��Ӹ�buff�ĵ�λ������.
	/// </summary>
	public AttackerAttr buffCreaterAttr;
	/// <summary>
	/// ����buff����Դ.
	/// </summary>
	public SkillBuffTableItem buffRes;
	public int argument_0;
	public int argument_1;
	public int argument_2;
}

/// <summary>
/// ��skillrandevent���õĽű�Ч��, ���Ӧ��skillrandeventһ�𴥷�.
/// </summary>
public abstract class SkillScript
{
	protected BattleUnit mOwner;
	/// <summary>
	/// Owner������.
	/// </summary>
	protected readonly AttackerAttr OwnerAttribute;
	public SkillScript(BattleUnit owner)
	{
		mOwner = owner;
		OwnerAttribute = new AttackerAttr(mOwner, uint.MaxValue, true);
	}

	/// <summary>
	/// ʹ�øýű�����¼����Ƿ��������.
	/// </summary>
	protected bool IsMainPlayerSkillScript { get { return mOwner is Player; } }

	/// <summary>
	/// �ýű���ʲô���͵�randevent����.
	/// </summary>
	public abstract RandEventTriggerType ScriptTriggerType { get; }

	/// <summary>
	/// ��ʼ���ű�.
	/// </summary>
	/// <returns>�Ƿ�ɹ�, ���ʧ��, �ű������ᴥ��.</returns>
	public abstract bool StartScript(SkillScriptStartArgument startArg);

	/// <summary>
	/// ����¼�������, ִ�и�����¼��ű�.
	/// </summary>
	/// <returns>�������false, �ýű�����Դ��BUFF��Ҫ���Ƴ�.</returns>
	public abstract bool RunScript(RandEventArg argument);

	/// <summary>
	/// �ű���Դ��BUFF������, �������ս���.
	/// </summary>
	public abstract void StopScript(SkillEffectStopReason stopReason);
}

/// <summary>
/// ���չ̶��˺���ֵ, ����DotA��LOA���޹�֮��(AphoticShield).
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
	/// �ֵܵ����˺�ֵ.
	int remaining = 0;
	/// �ܹ��ֵܵ����˺�ֵ.
	int capacity = 0;

	/// <summary>
	/// owner��������һ�����˺�.
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
	/// ��ʲô��ʽ�������ն������յ��˺�����.
	/// </summary>
	enum AphoticShieldType : int
	{ 
		// ����ǰѪ���İٷֱ�.
		PercentageOfCurrentHp,
		// ������ֵ.
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

		// �µ��˺�ֵ�����ڵ�ǰ�ֵܵ����˺�, ���µ�ǰ���Եֵ����˺�, ������µ��˺�.
		if (remaining >= beAtkArg.PositiveDamage)
		{
			remaining -= beAtkArg.PositiveDamage;
			beAtkArg.PositiveDamage = 0;
		}
		else
		{	// �����㹻���˺�ֵ.
			beAtkArg.PositiveDamage -= remaining;
			remaining = 0;
		}

		mOwner.SetShieldUIProgress(((float)remaining) / capacity);

		// �����㹻���˺�ֵ֮��, ��Ч����ʧ.
		return remaining > 0;
	}

	public override void StopScript(SkillEffectStopReason stopReason)
	{
		mOwner.ShowShieldUI(false);
	}
}

/// <summary>
/// ����Ч���ı�.
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
		// ��ʾ����ǿ���Ľ���.
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
			return ErrorHandler.Parse(ErrorCode.ConfigError, "����ID[" + weaponID + "]������weapon.txt��, ��ǿbuff�����Ƴ�");

		if (weaponTable.skill_2 < 0)
			return ErrorHandler.Parse(ErrorCode.ConfigError, "����[" + weaponID + "]û��ǿ������, ��ǿbuff�����Ƴ�");

		regularSkillArg.SkillResID = (uint)weaponTable.skill_2;
		
		return true;
	}

	public override void StopScript(SkillEffectStopReason stopReason)
	{
		// ��������ǿ���Ľ���.
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
/// ��ѪЧ��.
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
	// ���˺�ֵ�İٷֱ�, ת��Ϊ�����ߵ�����.
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
/// ��������.
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
/// ʬ��, ����֮��, ����Χ��������Ч��.
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
/// ������Ч(�������Է�ʱ, ���Է������������Ч��).
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
/// ����! ˲��ظ���Ѫ��������.
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

		// ����Ѫ����ħ��.
		mOwner.ModifyPropertyValue((int)PropertyTypeEnum.PropertyTypeHP, hpMax);
		mOwner.ModifyPropertyValue((int)PropertyTypeEnum.PropertyTypeMana, manaMax);

		// һ������֮��, �Ƴ�buff.
		return false;
	}

	public override void StopScript(SkillEffectStopReason stopReason)
	{
	}
}

/// <summary>
/// Ϊ�¼�Ŀ����Ӽ���Ч��.
/// ����Ϊ������, �����ƶ��ں�ʱ������Ӽ���Ч��.
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
/// ������ʱ, Ϊ�Լ���Ӽ���Ч��.
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
/// ��������ʱ, Ϊ�Լ���Ӽ���Ч��..
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
/// �ɹ�ʹ���κη���ͨ�����ļ���֮��, ��������Ӽ���Ч��.
/// </summary>
/// <remarks>����DotA�з籩֮��ĳ�����(Overload)</remarks>
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
/// ��ȫ�ֵ������˺�.
/// </summary>
/// <remarks>����DotA��ʥ�ô̿͵��۹�(Refraction)</remarks>
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

		// ��ȫ�ֵ�����˺�.
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
