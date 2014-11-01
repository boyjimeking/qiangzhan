using UnityEngine;

public class SkillImpactInitParam : SkillEffectInitParam
{
	public SkillImpactInitParam(BattleUnit owner, AttackerAttr attr, uint impactResID)
		: base(owner, attr)
	{
		impactResource = DataManager.ImpactTable[impactResID] as SkillImpactTableItem;
		if (impactResource == null)
			SkillUtilities.ResourceNotFound("impact", impactResID);
	}
	public SkillImpactTableItem impactResource;
}

class SkillImpact : SkillEffect
{
	SkillImpactTableItem mImpactRes;

	public override bool Initialize(SkillEffectInitParam param)
	{
		SkillImpactInitParam initParam = param as SkillImpactInitParam;
		mImpactRes = initParam.impactResource;

		if (mImpactRes == null)
			return false;

		return base.Initialize(param);
	}

	protected override ErrorCode doStart(SkillEffectInitParam param)
	{
		return base.doStart(param);
	}

	protected override void onStarted()
	{
		if (mImpactRes.shakeCameraDuration != 0)
            CameraController.Instance.ShakeCamera(mImpactRes.shapeCameraAmount, mImpactRes.shakeCameraDuration / 1000f, (ShakeType)mImpactRes.shapeCameraType);

		int hpChanged = 0, manaChanged = 0;
		bool critical = false;
		if (!computeImpactModify(out hpChanged, out manaChanged, out critical))
		{
			mOwner.Die(mAttackerAttr, mImpactRes.impactDamageType);
			return;
		}

		// 有益的impact, 或者可以被减血.
		if (!mImpactRes.harmful || mOwner.CanBeDamaged())
			impactModify(hpChanged, manaChanged, critical);

		// 表示是增益的impact(产生了加血, 有益的impact的特效没有CD, 且不会触发材质特效)
		//     || (减益imapct, 且添加被击特效成功(表示被击材质特效不在CD中且目标未死亡)).
		if (!mImpactRes.harmful
			|| mOwner.AddMaterialBehaviour(MaterialBehaviourDef.OnMaterialBeHit, mImpactRes.impactDamageType, mAttackerAttr.EffectStartDirection) == ErrorCode.Succeeded)
		{
			// 播放impact自带的特效.
			// 注意: 带有伤害的impact的特效, 与被击材质特效共同CD.
			SkillClientBehaviour.AddEffect2Object(mOwner, mImpactRes._3DEffectID, mImpactRes._3DEffectBindpoint, mAttackerAttr.EffectStartDirection);
		}

		base.onStarted();
	}

	/// <summary>
	/// 计算伤害以及魔法改变.
	/// </summary>
	/// <returns>返回false, 单位直接死亡</returns>
    private bool computeImpactModify(out int hpChanged, out int manaChanged, out bool critical)
	{
		hpChanged = manaChanged = 0;
		critical = false;

		float hpPercent = 100f * ((float)mOwner.GetHP()) / (float)mOwner.GetMaxHP();
		// 无论有害有益impact, 都可以引起自杀.
		if (hpPercent <= mImpactRes.seckill)
		{
			return false;
		}

		// 如果是有益的impact, 直接增加血量加值.
		// 否则需要计算实际伤害.

		if (mImpactRes.harmful)
		{
			// 根据伤害公式计算伤害.
			if(!Utility.isZero(mImpactRes.damageAttrPercent))
				hpChanged = computeDamageByFormula(out critical);

			// 根据距离产生额外伤害.
			if (mImpactRes.damageAddPerMile != 0)
			{
				float dist = Utility.Distance2D(mAttackerAttr.AttackerPosition, mOwner.GetPosition());
				hpChanged += (int)(mImpactRes.damageAddPerMile * dist);
			}

			// 将伤害转化为血量的实际变化值.
			hpChanged = -hpChanged;
		}

		// 按照固定值+-血量.
		hpChanged += mImpactRes.hpRegenerated;

		// 按照总血量的百分比+-血量.
		hpChanged += (int)(mImpactRes.hpPercentByTotal * mOwner.GetMaxHP() / 100f);

		// 按照当前血量的百分比+-血量.
		hpChanged += (int)(mImpactRes.hpPercentByCurrent * mOwner.GetHP() / 100f);

		manaChanged = mImpactRes.manaRegenerated;

		return true;
	}

	/// <summary>
	/// 对血量和魔法值进行修改, 并触发随机事件.
	/// </summary>
	private void impactModify(int hpChanged, int manaChanged, bool critical)
	{
		// 自己给自己添加的impact.
		bool impact2Self = (mAttackerAttr.AttackerID == mOwner.InstanceID);

		// 攻击者和受伤者的随机事件.
		// 攻击者必须是有效的而且是其他单位.
		if (!mAttackerAttr.StructMadeByRandEvent && hpChanged < 0	// 如果攻击者通过随机事件触发命中, 那么不触发被攻击者的随机事件.
			&& mAttackerAttr.AttackerID != uint.MaxValue && !impact2Self)
		{
			// 被击的随机事件.
			BeAttackedEventArg beAttackedEventArg = new BeAttackedEventArg(-hpChanged);
			mOwner.ApplyRandEvent(mAttackerAttr.CheckedAttackerObject(),
				RandEventTriggerType.OnBeAttacked,
				beAttackedEventArg
				);

			hpChanged = -beAttackedEventArg.PositiveDamage;
		}

		DamageInfo damageInfo = new DamageInfo()
		{
			Value = hpChanged,
			Critical = critical,
			DamageType = mImpactRes.impactDamageType
		};

		bool ownerDead = false;
		if (hpChanged != 0 && !mOwner.AddDamage(damageInfo, mAttackerAttr))
		{
			// 经过AddDamage之后, owner已经死亡, 不再播放被击特效, 以及修改魔法.
			ownerDead = true;
		}

		// 只有普通攻击才能触发攻击者的随机事件.
		SkillCommonTableItem skillRes = DataManager.SkillCommonTable[mAttackerAttr.SkillCommonID] as SkillCommonTableItem;
		if (mImpactRes.harmful && !impact2Self && skillRes != null && skillRes.isRegularAttack)
			applyAttackerRandEvent(-hpChanged, critical);

		if (!ownerDead && manaChanged != 0)
			mOwner.AddMana(manaChanged);
	}

	/// <summary>
	/// 检查并应用攻击者的随机事件.
	/// </summary>
	/// <param name="finalDamage">攻击者对目标造成的伤害值(正数)</param>
	private void applyAttackerRandEvent(int finalDamage, bool criticalStrike)
	{
		BattleUnit attacker = mAttackerAttr.CheckedAttackerObject();

		// 如果攻击者, 通过随机事件对对方造成伤害, 那么不触发攻击者的随机事件.
		if (attacker != null && !mAttackerAttr.StructMadeByRandEvent)
		{
			AtkOthersEventArg argument = new AtkOthersEventArg(mOwner, finalDamage);
			attacker.ApplyRandEvent(mOwner, RandEventTriggerType.OnAtkOthers, argument);

			if (criticalStrike)
			{
				OnCriticalStrikeOthersEventArg criticalStrikeOthersEventArg = new OnCriticalStrikeOthersEventArg();
				attacker.ApplyRandEvent(mOwner, RandEventTriggerType.OnCriticalStrikeOthers, criticalStrikeOthersEventArg);
			}
		}
	}

	/// <summary>
	/// 根据公式, 计算并返回实际伤害(正值).
	/// </summary>
	/// <param name="critical">是否为暴击伤害</param>
	/// <returns></returns>
	private int computeDamageByFormula(out bool critical)
	{
		critical = false;

		LevelCommonPropertiesItem levelProperties = DataManager.LevelCommonPropertiesTable[mAttackerAttr.AttackerLevel] as LevelCommonPropertiesItem;
		if(levelProperties == null)
		{
			SkillUtilities.ResourceNotFound("levelcommonproperties", mAttackerAttr.AttackerLevel);
			return 0;
		}

		// 等级暴击系数.
		float levelCriticalRate = (float)levelProperties.levelCriticalRate;
		float levelDefanceRate = (float)levelProperties.levelDefanceRate;

		if (Utility.isZero(levelCriticalRate))
		{
			ErrorHandler.Parse(ErrorCode.ConfigError, "等级暴击系数不可为0!");
			levelCriticalRate = 1;
		}

		// 暴击几率 = 暴击等级 / (等级暴击系数 + 暴击等级).
		float criticalProp = 100f * mAttackerAttr.CriticalLevel / (mAttackerAttr.CriticalLevel + levelCriticalRate);

		// 是否暴击.
		critical = SkillUtilities.Random100((uint)criticalProp);

		// 免伤百分比 = 防守方真实防护 / (防守方真实防护 + 等级防护系数).
		float defance = (float)mOwner.GetPropertyValue((int)PropertyTypeEnum.PropertyTypeDefance);
		float defancePercent = defance / (defance + levelDefanceRate);

		// 攻击者的实际伤害, 如果暴击, 为基本伤害的CriticalDamageScale倍.
		float criticalDamageScale = (critical ? GameConfig.CriticalDamageScale : 1f);
		float realDamage = criticalDamageScale * mAttackerAttr.AttackerDamage * mImpactRes.damageAttrPercent / 100f;

		// 最少造成1点血量伤害.
		return (int)Mathf.Max(realDamage * (1f - defancePercent), 1f);
	}

	public override bool NeedUpdate
	{
		get { return false; }
	}

	public override SkillEffectType Type
	{
		get { return SkillEffectType.Impact; }
	}

	public override uint ResID
	{
		get { return (uint)mImpactRes.resID; }
	}
}
