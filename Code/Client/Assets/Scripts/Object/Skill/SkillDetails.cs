using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SkillDetails
{
	#region SkillUseDetails
	/// <summary>
	/// 创建技能Action. 开始向targetPosition使用技能.
	/// </summary>
	/// <remarks>
	/// <para>普通攻击可以被其他非普通攻击, 无条件打断</para>
	/// <para>非普通攻击只有在准备阶段, 且配置中指定了可被打断才可以被打断</para>
	/// <para>非普通攻击被相同的技能打断, 表示从使用阶段切换到使用阶段</para>
	/// 非普通攻击被其他技能打断, 直接停止当前技能, 开始新的技能.
	/// </remarks>
	public static ErrorCode StartSkillAction(BattleUnitActionCenter actionCenter, 
		BattleUnitSkill skill, Vector3 targetPosition)
	{
		if(skill == null)
			return ErrorCode.InvalidParam;

		ActionSkill currentAs = actionCenter.GetActionByType(ActionTypeDef.ActionTypeSkill) as ActionSkill;

		ErrorCode err = ErrorCode.Succeeded;

		if(currentAs == null)
		{
			ActionSkillInitParam param = new ActionSkillInitParam();
			param.skill = skill;
			param.targetPosition = targetPosition;
			err = actionCenter.StartAction(param);
			if(err != ErrorCode.Succeeded)
				return err;
		}
		else
		{
			// 如果此时的currentAs不为null, 那么它必然为一个正在准备状态的技能且可以切换状态.
#if UNITY_EDITOR
			if (!(currentAs.InChargingState && currentAs.Interruptable))
				ErrorHandler.Parse(ErrorCode.LogicError, "技能处于错误的状态.");
#endif
			err = currentAs.EnterUseState(targetPosition);
		}

		return err;
	}

	/// <summary>
	/// 当skilleffect的owner发生事件时, 检查并移除skilleffect.
	/// </summary>
	/// <param name="effectManager"></param>
	/// <param name="ownerEvent"></param>
	public static void OnSkillEffectOwnerEvent(SkillEffectManager effectManager, SkillEffectOwnerEventDef ownerEvent)
	{
		effectManager.ForEverySkillEffect(new SkillUtilities.KillSkillEffectOnOwnerEvent(ownerEvent));
	}

	/// <summary>
	/// user使用的技能对targetPosition产生skillRes指定的技能效果.
	/// </summary>
	public static ErrorCode SkillTakeEffect(BattleUnit user, SkillCommonTableItem skillRes, Vector3 targetPosition)
	{
		AttackerAttr attackerAttr = new AttackerAttr(user, (uint)skillRes.resID);

		if (skillRes.projectileResID != uint.MaxValue)
			createProjectiles(user, (uint)skillRes.resID, skillRes.projectileResID, targetPosition);
		
		// 给技能使用者添加的效果.
		ErrorHandler.Parse(
			AddSkillEffectByResource(attackerAttr, user, skillRes.skillEffect2UserInUseState)
			);

		// 向前延伸一定距离, 作为目标选择的中心点.
		Vector3 centerPosition = Utility.MoveVector3Towards(attackerAttr.AttackerPosition,
			user.GetDirection(), skillRes.distForward);

		// 在中心点进行目标选择, 并添加技能效果.
		SelectTargetAndAddSkillEffect(attackerAttr, centerPosition, user.GetDirection(),
			skillRes.targetSelection, skillRes.skillEffect2Others);

		// 创建召唤物.
		ErrorHandler.Parse(
			CreateCreationAround(attackerAttr, skillRes.creationID, user.GetPosition(), user.GetDirection())
			);

		return ErrorCode.Succeeded;
	}

	/// <summary>
	/// 技能开始使用之后调用, 用来置CD.
	/// 技能进入使用阶段才会设置CD, 也就是说, 如果一个技能在准备阶段不会设置CD.
	/// </summary>
	public static void OnActionEnterUseState(BattleUnit user, BattleUnitSkill skill)
	{
		// CD.
        skill.lastUseTime = TimeUtilities.GetNow();
	}

	/// <summary>
	/// 技能产生效果后.
	/// </summary>
	public static void OnSkillEffected(BattleUnit user, BattleUnitSkill skill)
	{
		// 在技能产生效果, 即子弹确保已经产生之后, 再扣子弹.
		if (skill.skillRes.bulletCost != 0)
		{
			user.CostWeaponBullet((int)skill.skillRes.bulletCost);
		}
	}

	/// <summary>
	/// 技能开始使用.
	/// </summary>
	public static void OnSkillStarted(BattleUnit user, BattleUnitSkill skill)
	{
		// 消耗.	
		user.ModifyPropertyValue((int)PropertyTypeEnum.PropertyTypeMana, -((int)skill.skillRes.manaCost));
	}

	/// <summary>
	/// 技能正常结束之后.
	/// </summary>
	public static void OnSkillFinished(BattleUnit user, BattleUnitSkill skill)
	{
		// 只有非普通攻击, 才会触发攻击者的使用技能之后的随机事件.
		if(!skill.IsRegularAttack)
			user.ApplyRandEvent(null, RandEventTriggerType.OnSkillFinished, new OnSkillFinishedEventArg(skill.skillRes));
	}

	#endregion SkillUseDetails

	#region SkillEffectDetails
	/// <summary>
	/// 添加技能效果.
	/// </summary>
	/// <param name="attackerAttr">添加者的属性</param>
	/// <param name="effectManager">被添加者的技能效果控制器</param>
	/// <param name="type">效果类型</param>
	/// <param name="resID">效果ID</param>
	/// <returns></returns>
	public static ErrorCode StartSkillEffect(AttackerAttr attackerAttr,
		SkillEffectManager effectManager,
		SkillEffectType type,
		uint resID
		)
	{
		if (effectManager == null)
			return ErrorCode.LogicError;

		SkillEffect effect = null;
		SkillEffectInitParam param = null;

		switch (type)
		{
			case SkillEffectType.Buff:
				effect = effectManager.FindSkillEffectByPredicate(new SkillUtilities.FindBuffByResource(resID));

				// 存在相同的buff, 进行叠加.
				if (effect != null)
					return (effect as SkillBuff).AddStack();

				// 不存在相同的buff, 创建buff.
				param = new SkillBuffInitParam(effectManager.Owner, attackerAttr, resID);

				// 移除所有与新buff互斥的效果.
				uint mutex = ((SkillBuffInitParam)param).buffResource.mutex;
				if(mutex != uint.MaxValue)
					effectManager.ForEverySkillEffect(new SkillUtilities.KillMutuallyExclusiveSkillBuff(mutex));
				break;
			case SkillEffectType.Impact:
				// 创建impact.
				param = new SkillImpactInitParam(effectManager.Owner, attackerAttr, resID);
				break;
			default:
				ErrorHandler.Parse(ErrorCode.ConfigError, "invalid skilleffect type " + (uint)type);
				break;
		}

		return effectManager.CreateSkillEffect(param);
	}

	/// <summary>
	/// 开始硬直.
	/// </summary>
	public static ErrorCode StartSpasticity(BattleUnitActionCenter actionCenter, 
		AttackerAttr attackerAttr, 
		uint spasticityResID, 
		ref uint cdMilliseconds
		)
	{
		if (cdMilliseconds != 0)
			return ErrorCode.CoolingDown;

		// 无法被控制.
		if (!actionCenter.Owner.CanBeStuned())
			return ErrorCode.AddEffectFailedSkillEffectImmunity;

		// 硬直抗性.
		int resistance = actionCenter.Owner.GetPropertyValue((int)PropertyTypeEnum.PropertyTypeSpasticityResistance);

		// 硬直抗性不低于抗性最大值, 无法触发硬直.
		if (resistance >= GameConfig.MaxSpasticityResistance)
			return ErrorCode.AddEffectFailedSkillEffectImmunity;

		SkillSpasticityTableItem spasticityRes = DataManager.SkillSpasticityTable[spasticityResID] as SkillSpasticityTableItem;

		if (spasticityRes == null)
		{
			SkillUtilities.ResourceNotFound("skillspasticity", spasticityResID);
			return ErrorCode.ConfigError;
		}

		Vector3 ownerPosition = actionCenter.Owner.GetPosition();
		Vector3 targetPosition = ownerPosition;

		Vector3 spasticityDirection = Utility.RadianToVector3(attackerAttr.EffectStartDirection);
		spasticityDirection.y = 0;

		// 被硬直的单位的朝向为硬直位移方向的反方向.
		actionCenter.Owner.SetDirection(Utility.Vector3ToRadian(spasticityDirection) + Mathf.PI);

		// 计算硬直位移的距离.
		float distance = spasticityRes.distance;
		// 如果硬直抗性低于默认值, 击退距离 *= (100 / 抗性).
		if (resistance > 100)
			resistance *= (100 / resistance);

		spasticityDirection.Normalize();
		spasticityDirection *= distance;
		targetPosition += spasticityDirection;

		BaseScene scn = SceneManager.Instance.GetCurScene();
		if (scn == null)
			return ErrorCode.LogicError;

		// 检测场景阻挡, 确定终点位置.
		targetPosition = scn.FarthestWalkable(ownerPosition, targetPosition);

		ActionSpasticityInitParam param = new ActionSpasticityInitParam();
		param.spasticityRes = spasticityRes;
		param.targetPosition = targetPosition;
		param.mAttackerAttr = attackerAttr;

		// 当前的硬直动作.
		ActionSpasticity spasticityAction = actionCenter.GetActionByType(ActionTypeDef.ActionTypeSpasticity) as ActionSpasticity;

		ErrorCode err = ErrorCode.Succeeded;

		// 重新开始硬直.
		err = (spasticityAction != null) ? spasticityAction.Restart(param) : actionCenter.StartAction(param);

		if (err == ErrorCode.Succeeded)
		{
			// 硬直抗性, 可以用来增加硬直的CD时间.
			cdMilliseconds = (uint)(param.spasticityRes.cdMilliseconds * resistance / 100f);
		}

		return err;
	}

	/// <summary>
	/// 开始位移.
	/// </summary>
	/// <param name="actionCenter">动作控制器</param>
	/// <param name="attackerAttr">发起这次位移的单位的属性(如果是突进, 那么是角色本身; 如果是击退, 为发起击退技能的单位)</param>
	/// <param name="displacementRes"></param>
	/// <returns></returns>
	public static ErrorCode StartDisplace(BattleUnitActionCenter actionCenter, AttackerAttr attackerAttr, uint displacementResID)
	{
		SkillDisplacementTableItem displacementRes = DataManager.DisplacementTable[displacementResID] as SkillDisplacementTableItem;
		if (displacementRes == null)
		{
			SkillUtilities.ResourceNotFound("displacement", displacementResID);
			return ErrorCode.ConfigError;
		}

		// 免疫控制(有害的位移, 都视为控制).
		if (!actionCenter.Owner.CanBeStuned() && displacementRes.harmful)
			return ErrorCode.AddEffectFailedSkillEffectImmunity;

		if ((actionCenter.GetActionByType(ActionTypeDef.ActionTypeDisplacement) as ActionDisplacement) != null)
			return ErrorCode.MaxStackCount;

		Vector3 ownerPosition = actionCenter.Owner.GetPosition();
		Vector3 targetPosition = ownerPosition;

		float distance = displacementRes.distance;

		if (distance < 0)
			return ErrorCode.ConfigError;

		Vector3 displaceDirection = Vector3.zero;

		// 确定方向.
		switch (displacementRes.displacementType)
		{
			case SkillDisplacementType.Rush:
				displaceDirection = Utility.RadianToVector3(actionCenter.Owner.GetDirection());
				break;
			case SkillDisplacementType.Beatback:
				displaceDirection = Utility.RadianToVector3(attackerAttr.EffectStartDirection);
				break;
			default:
				return ErrorCode.ConfigError;
		}

		displaceDirection.y = 0;

		// 速度为负, 取反方向.
		if (displacementRes.speed < 0f)
			displaceDirection = Vector3.zero - displaceDirection;

		// beatback时, 特殊处理距离与朝向.
		if (displacementRes.displacementType == SkillDisplacementType.Beatback)
		{
			// 击退, 且速度为负时, 表示牵引向目标点.
			if (displacementRes.speed < 0f)
			{
				float magnitude = displaceDirection.magnitude;
				// 牵引的位置不会超过目标点的位置, 且与目标点的位置至少为0.5f.
				if (magnitude < distance)
				{
					distance = Mathf.Max(0f, magnitude - 0.5f);
				}
			}
			else	// 击退时, 面朝起始点方向(后仰).
				actionCenter.Owner.SetDirection(Utility.Vector3ToRadian(displaceDirection) + Mathf.PI);
		}

		displaceDirection.Normalize();
		displaceDirection *= distance;
		targetPosition += displaceDirection;

		BaseScene scn = SceneManager.Instance.GetCurScene();
		if (scn == null)
			return ErrorCode.LogicError;

		targetPosition = scn.FarthestWalkable(ownerPosition, targetPosition);

		ActionDisplacementInitParam param = new ActionDisplacementInitParam();
		param.targetPosition = targetPosition;
		param.displacementResource = displacementRes;
		param.mAttackerAttr = attackerAttr;

		return actionCenter.StartAction(param);
	}

	/// <summary>
	/// 添加randEventResourceID指定的skillrandevent资源.
	/// </summary>
	/// <param name="randEventResourceID"></param>
	public static ErrorCode AddRandEvent(BattleUnit owner, 
		BattleUnitRandEventContainer randEventContainer, 
		SkillBuffTableItem buffRes,
		AttackerAttr buffCreaterAttr
		)
	{
		if (buffRes.randEvent == uint.MaxValue)
			return ErrorCode.Succeeded;

		SkillRandEventTableItem randEventRes = DataManager.RandEventTable[buffRes.randEvent] as SkillRandEventTableItem;
		randEventContainer.Add(owner, buffRes, randEventRes, ref buffCreaterAttr);

		return ErrorCode.Succeeded;
	}

	public static ErrorCode RemoveRandEvent(BattleUnit owner,
		BattleUnitRandEventContainer randEventContainer,
		SkillBuffTableItem buffRes,
		SkillEffectStopReason stopReason
		)
	{
		if (buffRes.randEvent == uint.MaxValue)
			return ErrorCode.Succeeded;

		randEventContainer.Remove(buffRes, stopReason);

		return ErrorCode.Succeeded;
	}

	/// <summary>
	/// 当triggerType指定的事件发生时, 检查randEventContainer列表, 触发随机事件.
	/// </summary>
	/// <param name="theOther">触发事件的单位.</param>
	/// <param name="eventOwner">为随机事件的拥有者.</param>
	/// <remarks>randevent的技能效果不由任何技能发起(只可以得知该随机事件是如何被注册的)</remarks>
	public static void ApplyRandEvent(
		BattleUnit theOther,
		BattleUnit eventOwner,
		BattleUnitRandEventContainer randEventContainer,
		RandEventTriggerType triggerType,
		RandEventArg triggerArg
		)
	{
		List<uint> buffNeed2Remove = new List<uint>();
		List<BattleUnitRandEvent> typedContainer = randEventContainer[triggerType];
		//LinkedListNode<BattleUnitRandEvent> currentNode = typedContainer.First;

		// SkillScript可能产生新的randevent加入容器中, 导致容器改变, 因此不能通过foreach遍历.
		// 获取当前指定类型的容器中的元素个数, 只遍历当前的元素(新的randevent通过AddLast加入到容器中, 因此不会被遍历到).

        for (int i = typedContainer.Count - 1; i >= 0; --i )
        {
            BattleUnitRandEvent randEvent = typedContainer[i];

            SkillRandEventTableItem randEventRes = randEvent.randEventResource;

            // 检查CD.
            if (randEvent.cdMilliseconds != 0)
                continue;

            // 检查概率.
            if (!SkillUtilities.Random100(randEventRes.probability))
                continue;

            SkillClientBehaviour.AddEffect2Object(eventOwner, randEventRes._3DEffectID, randEventRes._3DEffectBindpoint);

            if (randEvent.mScript != null && !randEvent.mScript.RunScript(triggerArg))
                buffNeed2Remove.Add((uint)randEvent.fromBuffRes.resID);

            randEvent.cdMilliseconds = randEventRes.cdMilliseconds;
        }

//             for (int count = typedContainer.Count; count != 0; --count, currentNode = currentNode.Next)
//             {
//                 
//             }

        for (int i = 0; i < buffNeed2Remove.Count; ++i )
        {
            eventOwner.RemoveSkillBuffByResID(buffNeed2Remove[i]);
        }
/*            foreach (uint id in buffNeed2Remove)*/
	}

	/// <summary>
	/// 在targetPostion周围选取合适的位置, 进行创建.
	/// </summary>
	/// <param name="summonerAttr">创建者的属性</param>
	/// <param name="creationResID">creation资源ID</param>
	/// <param name="targetPosition">创造点(最终创造点会随creation表格的数据调整)</param>
	/// <param name="dir">初始朝向(最终朝向会随creation表格的数据调整)</param>
	/// <returns></returns>
	public static ErrorCode CreateCreationAround(AttackerAttr summonerAttr, uint creationResID, Vector3 targetPosition, float dir)
	{
		if (creationResID == uint.MaxValue)
			return ErrorCode.Succeeded;
		BaseScene scn = SceneManager.Instance.GetCurScene();
		if(scn == null)
			return ErrorCode.LogicError;

		ErrorCode err = ErrorCode.Succeeded;

		SkillCreationTableItem creationRes = DataManager.CreationTable[creationResID] as SkillCreationTableItem;

		for (uint i = 0; (i < SkillCreationTableItem.CreationCount && err == ErrorCode.Succeeded); ++i)
		{
			SkillCreationItem item = creationRes.items[i];

			if (item.creationType == CreationType.Invalid) 
				break;

			Vector3 spawnPosition = Utility.MoveVector3Towards(targetPosition, dir, item.spDistOffset);
			spawnPosition = Utility.RotateVectorByRadian(spawnPosition, item.spAngleOffset, targetPosition);

			// 如果计算出来的位置为不可行走区域, 那么, 直接创建在原始位置.
			if (!scn.IsInWalkableRegion(spawnPosition)) 
				spawnPosition = targetPosition;

			err = createCreationAt(spawnPosition, summonerAttr, item.creationType, item.creatureID, item.lifeTime);
		}

		return err;
	}

	/// <summary>
	/// 在指定位置创建召唤物.
	/// </summary>
	private static ErrorCode createCreationAt(Vector3 spawnPosition, AttackerAttr ownerAttribute, CreationType creationType, uint creationID, uint lifeTime)
	{
		BattleUnit summonedCreature = null;
		switch (creationType)
		{
			case CreationType.Npc:
				summonedCreature = SceneManager.Instance.GetCurScene().CreateSprite(new NpcInitParam()
				{
					init_pos = spawnPosition,
					init_dir = ownerAttribute.AttackerDirection,
					npc_res_id = (int)creationID,
					summonerAttr = ownerAttribute,
					lifeTime = lifeTime,
				}) as BattleUnit;
				break;
			case CreationType.Trap:
				summonedCreature = SceneManager.Instance.GetCurScene().CreateSprite(new TrapInitParam()
				{
					init_pos = spawnPosition,
					init_dir = ownerAttribute.AttackerDirection,
					summonerAttr = ownerAttribute,
					trapResID = creationID,
					lifeTime = lifeTime
				}) as BattleUnit;
				break;
			default:
				return ErrorCode.ConfigError;
		}

		// 设置召唤物属性.
		if (summonedCreature != null)
		{
			// 与召唤者相同阵营.
			summonedCreature.SetLeague(ownerAttribute.AttackerLeague);
			// 免疫外界的技能任何技能效果, 而且不能被锁定, 不能阻挡子弹.
			if (creationType == CreationType.Trap)
				summonedCreature.AddActiveFlag(ActiveFlagsDef.Inviolability, true, true);
		}

		return ErrorCode.Succeeded;
	}

	/// <summary>
	/// 为target添加技能效果.
	/// skillEffectResID为skilleffect.txt的资源ID.
	/// </summary>
	public static ErrorCode AddSkillEffectByResource(AttackerAttr attackerAttr, BattleUnit target, uint skillEffectResID)
	{
		if (skillEffectResID == uint.MaxValue)
			return ErrorCode.Succeeded;

		SkillEffectTableItem effectRes = DataManager.SkillEffectTable[skillEffectResID] as SkillEffectTableItem;
		if (effectRes == null)
		{
			SkillUtilities.ResourceNotFound("skilleffect", skillEffectResID);
			return ErrorCode.ConfigError;
		}

		System.Type T = effectRes.GetType();
		for (uint i = 0; i < SkillEffectTableItem.SkillEffectCount; ++i)
		{
			SkillEffectItem item = effectRes.items[i];

			if (item.effectType == SkillEffectType.Invalid || item.effectID == uint.MaxValue)
				break;

			ErrorHandler.Parse(
				target.AddSkillEffect(attackerAttr, item.effectType, item.effectID), 
				"in AddSkillEffectByResource"
				);
		}

		return ErrorCode.Succeeded;
	}

	/// <summary>
	/// 选择目标, 并添加效果.
	/// </summary>
	/// <param name="attackerAttr">攻击者属性</param>
	/// <param name="centerPosition">在该位置进行目标选择</param>
	/// <param name= "direction">在中心位置时的朝向(如果形状是圆, 那么该参数没有作用)</param>
	/// <param name="targetSelection">目标选择参数</param>
	/// <param name="skillCommonResID">最初技能ID</param>
	/// <param name="skillEffectResID">为选择到的目标添加效果</param>
	/// <returns>对多少人添加了效果</returns>
	/// <remarks>
	/// 通过该方法产生的技能效果, 起始点和起始位置的方向会改变为技能选择中心点以及该点处的方向,
	/// 对之后的位移发起位置, 特效方向等产生影响.
	/// </remarks>
	public static ErrorCode SelectTargetAndAddSkillEffect(
		AttackerAttr attackerAttr, 
		Vector3 centerPosition,
		float direction,
		uint targetSelection, 
		uint skillEffectResID,
		SkillUtilities.FilterTarget filter = null
		)
	{
		if (targetSelection == uint.MaxValue && skillEffectResID == uint.MaxValue)
			return ErrorCode.Succeeded;

		TargetSelectionTableItem targetSelectionRes = DataManager.TargetSelectionTable[targetSelection] as TargetSelectionTableItem;
		if (targetSelectionRes == null)
		{
			SkillUtilities.ResourceNotFound("targetselection", targetSelection);
			return ErrorCode.ConfigError;
		}

		return SelectTargetAndAddSkillEffect(attackerAttr, centerPosition, direction, targetSelectionRes, skillEffectResID, filter);
	}

	/// <summary>
	/// 以目标选择的对象(而不是资源ID)作为参数, 外界可以通过直接构造该对象, 而非从数据表读取, 从而调用该方法.
	/// </summary>
	public static ErrorCode SelectTargetAndAddSkillEffect(
		AttackerAttr attackerAttr,
		Vector3 centerPosition,
		float direction,
		TargetSelectionTableItem targetSelectionRes,
		uint skillEffectResID,
		SkillUtilities.FilterTarget filter = null
		)
	{
		ArrayList targets = SkillUtilities.SelectTargets(attackerAttr,
				centerPosition, direction, targetSelectionRes);

		if (filter != null)
		{
			SkillUtilities.FilterTargetsBy(targets, filter, null);
		}

		foreach (BattleUnit t in targets)
		{
			// 设置效果的起始点.
			attackerAttr.SetEffectStartLocation(centerPosition,
				Utility.Vector3ToRadian(t.GetPosition() - attackerAttr.EffectStartPosition, direction)
				);

			ErrorHandler.Parse(
			AddSkillEffectByResource(attackerAttr, t, skillEffectResID),
			"failed to add skill effect with SelectTargetAndAddSkillEffect"
			);
		}

		return ErrorCode.Succeeded;
	}

	public static SkillEffect AllocSkillEffect(SkillEffectInitParam param)
	{
		System.Type paramType = param.GetType();

		SkillEffect result = null;

		if (paramType == typeof(SkillBuffInitParam))
			result = new SkillBuff();
		else if (paramType == typeof(SkillImpactInitParam))
			result = new SkillImpact();
		// else if branch.
		else
			ErrorHandler.Parse(ErrorCode.LogicError, "allocate skill effect failed: invalid skilleffect type");

		return result;
	}

	/// <summary>
	/// 添加材质特效和声音.
	/// </summary>
	/// <param name="owner">被添加特效和声音的单位</param>
	/// <param name="cdContainer">cd的容器, 该函数会检查并设置CD.</param>
	/// <param name="name">添加的类型(被击, 死亡等)</param>
	/// <param name="impactDamageType">impact的伤害类型</param>
	public static ErrorCode AddMaterialBehaviour(BattleUnit owner, HitMaterialEffectCdContainer cdContainer,
		MaterialBehaviourDef name, ImpactDamageType impactDamageType, float dir
		)
	{
		// 无效的伤害类型.
		if (impactDamageType >= ImpactDamageType.Count)
			return ErrorCode.Succeeded;

		// CD中, 只有被击的材质特效有CD, 死亡材质特效没有CD.
		if (name == MaterialBehaviourDef.OnMaterialBeHit && cdContainer.IsCoolingDown(impactDamageType))
			return ErrorCode.CoolingDown;

		uint materialID = owner.GetMaterialResourceID();

		if (materialID == uint.MaxValue)
		{
			ErrorHandler.Parse(ErrorCode.ConfigError, owner.dbgGetIdentifier() + " 没有材质!");
			return ErrorCode.ConfigError;
		}

		MaterialTableItem material = DataManager.MaterialTable[materialID] as MaterialTableItem;
		if (material == null)
		{
			SkillUtilities.ResourceNotFound("material", materialID);
			return ErrorCode.ConfigError;
		}

		if (name == MaterialBehaviourDef.OnMaterialBeHit)
			cdContainer.StartCd(impactDamageType);

		MaterialItem item = material.items[(int)impactDamageType];

		uint effectID = uint.MaxValue;
		uint sound = uint.MaxValue;
		string bindpoint = "";
		switch (name)
		{ 
			case MaterialBehaviourDef.OnMaterialBeHit:
				effectID = item.hitEffect;
				bindpoint = material.hitEffectBindpoint;
				sound = item.sound;
				// 被击材质特效在被击者身上播放.
/*				SkillClientBehaviour.AddEffect2Object(owner, effectID, bindpoint, dir);*/
                if (string.IsNullOrEmpty(bindpoint))
                {
                    SkillClientBehaviour.AddSceneEffect(effectID, owner.GetPosition(), dir);
                }
                else
                {
                    SkillClientBehaviour.AddEffect2Object(owner, effectID, bindpoint, dir);
                }
				break;

			case MaterialBehaviourDef.OnMaterialDie:
				effectID = item.deathEffect;
				bindpoint = material.deathEffectBindpoint;
				if (string.IsNullOrEmpty(bindpoint))
				{
					SkillClientBehaviour.AddSceneEffect(effectID, owner.GetPosition(), dir);
				}
				else
				{
					SkillClientBehaviour.AddEffect2Object(owner, effectID, bindpoint, dir);
				}

				break;

			default:
				break;
		}

		if (sound != uint.MaxValue)
			SkillClientBehaviour.PlaySound(sound);

		return ErrorCode.Succeeded;
	}
	
	#endregion SkillEffectDetails

	#region CreateProjectileDetails
	/// <summary>
	/// 创建投掷物.
	/// </summary>
	public static ErrorCode createProjectiles(BattleUnit user, uint skillResID, uint projResID, Vector3 targetPosition)
	{
		ProjectileSettingsTableItem projSettings = DataManager.ProjectileSettingsTable[projResID] as ProjectileSettingsTableItem;

		for (uint i = 0; i < ProjectileSettingsTableItem.ProjectileCount; ++i)
		{
			ProjectileItem item = projSettings.items[i];

			if (item.bulletResID == uint.MaxValue)
				break;

			BulletTableItem bulletRes = DataManager.BulletTable[item.bulletResID] as BulletTableItem;

			// 枪口特效.
			if (item.bindpointEffect != uint.MaxValue)
				SkillClientBehaviour.AddEffect2Object(user, item.bindpointEffect, item.initBindpoint);

			ProjectileCreateParam param = new ProjectileCreateParam()
			{
				User = user,
				StartPosition = user.GetBonePositionByName(item.initBindpoint),
				TargetPosition = targetPosition,
				DistributionType = item.distributionType,
				DistributionArgument = item.distributionArgument,
				BulletRes = bulletRes,
				SkillResID = skillResID
			};

			createProjectiles(param);
		}

		return ErrorCode.Succeeded;
	}

	/// <summary>
	/// 根据投掷物的创建参数, 创建投掷物.
	/// <para>按如下步骤处理子弹的分布:</para>
	/// <para>1 根据子弹的类型, 调整子弹方向, 并将个别类型的子弹的目标点缩放到子弹射程的最远点.</para>
	/// <para>2 依次应用分布参数.</para>
	/// 3 根据上述结果, 创建子弹.
	/// </summary>
	private static ErrorCode createProjectiles(ProjectileCreateParam param)
	{
		// 获取子弹分布.
		ArrayList distributionContainer = new ArrayList();

		// 处理子弹的飞行方向.
		param.TargetPosition = BulletAdjuster.AdjustBulletDirection(param.BulletRes, param.User,
			param.StartPosition, param.TargetPosition);

		// 调整个别类型的子弹的终点, 到子弹的最大射程.
		param.TargetPosition = BulletAdjuster.AdjustBulletTargetPosition(param.BulletRes, param.StartPosition, param.TargetPosition);

		// 根据子弹的分布, 调整子弹的起始/终点位置, 以及延迟等.
		ErrorCode err = computeBulletDistribution(distributionContainer, param);

		if (err != ErrorCode.Succeeded)
			return err;

		// 产生子弹.
		foreach (BulletDistributionResultDef distribution in distributionContainer)
		{
			BulletInitParam initParam = new BulletInitParam()
			{
				resID = (uint)param.BulletRes.resID,
				startPosition = distribution.StartPosition,
				targetPosition = distribution.TargetPosition,
				firerAttr = new AttackerAttr(param.User, param.SkillResID),
				createDelay = distribution.Delay,
			};

			Bullet bullet = BulletFactory.Instance.Allocate(param.BulletRes.type);
			if (bullet != null && bullet.Init(initParam))
				SceneManager.Instance.GetCurScene().AddSprte(bullet);
		}

		return err;
	}

	delegate ErrorCode DistributeHandler(ArrayList result, DistributionComputeParam param);
	/// <summary>
	/// 计算子弹的分布.
	/// </summary>
	private static ErrorCode computeBulletDistribution(ArrayList result,
		ProjectileCreateParam param)
	{
		DistributeHandler handler = null;
		switch (param.DistributionType)
		{
			case BulletDistributionType.ByAngle:
				handler = distributeByAngle;
				break;
			case BulletDistributionType.ByRandomOffset:
				handler = distributeByRandomOffset;
				break;
			case BulletDistributionType.ByData:
				handler = distributeByData;
				break;
			default:
				break;
		}

		DistributionComputeParam computeParam = new DistributionComputeParam()
		{
			distributionArgument = param.DistributionArgument,
			startPosition = param.StartPosition,
			targetPosition = param.TargetPosition
		};

		ErrorCode err = ErrorCode.ConfigError;

		if (handler != null)
			err = handler(result, computeParam);

		ErrorHandler.Parse(err, "failed to compute bullet distribution");
		return err;
	}

	/// <summary>
	/// 按照对每颗子弹的数据指定, 计算子弹的起点和终点.
	/// </summary>
	private static ErrorCode distributeByData(ArrayList result, DistributionComputeParam param)
	{
		uint[] array = Utility.SplitToUnsigned(param.distributionArgument);

		foreach (uint id in array)
		{
			Vector3 start = param.startPosition, end = param.targetPosition;
			BulletDistributionTableItem distribution = DataManager.BulletDistributionTable[id] as BulletDistributionTableItem;
			if (distribution == null)
			{
				return ErrorCode.ConfigError;
			}

			// 按角度, 将起点到终点的射线旋转指定角度.
			end = Utility.RotateVectorByRadian(end, distribution.angle * Mathf.Deg2Rad, start);

			float flyDirection = Utility.Vector3ToRadian(end - start);

			// 起点右偏移.
			start = Utility.MoveVector3Towards(start, flyDirection + Mathf.PI / 2f, distribution.startOffsetRight);

			flyDirection = Utility.Vector3ToRadian(end - start);

			// 起点前偏移.
			start = Utility.MoveVector3Towards(start, flyDirection, distribution.startOffsetForward);

			flyDirection = Utility.Vector3ToRadian(end - start);

			// 终点右偏移.
			end = Utility.MoveVector3Towards(end, flyDirection + Mathf.PI / 2f, distribution.endOffsetRight);

			flyDirection = Utility.Vector3ToRadian(end - start);

			// 终点向前偏移.
			end = Utility.MoveVector3Towards(end, flyDirection, distribution.endOffsetForward);

			result.Add(new BulletDistributionResultDef()
			{
				Delay = distribution.delay,
				StartPosition = start,
				TargetPosition = end
			});
		}

		return ErrorCode.Succeeded;
	}

	/// <summary>
	///  按角度偏移子弹.
	/// </summary>
	private static ErrorCode distributeByAngle(ArrayList result, DistributionComputeParam param)
	{
		uint[] args = Utility.SplitToUnsigned(param.distributionArgument);

		if (args.Length != 2)
		{
			return ErrorCode.ConfigError;
		}

		uint count = args[0];
		float radian = args[1] * Mathf.Deg2Rad;

		BulletDistributionResultDef distribution = new BulletDistributionResultDef()
		{
			StartPosition = param.startPosition,
			TargetPosition = Utility.RotateVectorByRadian(param.targetPosition, (count - 1) * radian / 2f, param.startPosition),
			Delay = 0
		};

		// 从中间位置开始, 顺时针方向最后一个子弹的分布.
		result.Add(distribution);

		// 逆时针旋转radian弧度, 计算剩余的子弹的目标点.
		for (uint i = 1; i < count; ++i)
		{
			distribution.TargetPosition = Utility.RotateVectorByRadian(distribution.TargetPosition, -radian, param.startPosition);
			result.Add(distribution);
		}

		return ErrorCode.Succeeded;
	}

	/// <summary>
	///  终点按照随机的偏移分布.
	/// </summary>
	private static ErrorCode distributeByRandomOffset(ArrayList result, DistributionComputeParam param)
	{
		string[] args = param.distributionArgument.Split('|');

		if (args.Length != 2)
		{
			return ErrorCode.ConfigError;
		}

		uint count = System.Convert.ToUInt32(args[0]);
		float maxOffset = System.Convert.ToSingle(args[1]);

		// 起点到目标点射线的垂直方向.
		float radian = Utility.Vector3ToRadian(param.targetPosition - param.startPosition) + Mathf.PI / 2f;

		BulletDistributionResultDef distribution = new BulletDistributionResultDef() { Delay = 0, StartPosition = param.startPosition };

		for (uint i = 0; i < count; ++i)
		{
			float random = UnityEngine.Random.value * 2 - 1f;	// [-1, 1].
			random *= maxOffset;	// [-maxOffset, maxOffset].
			distribution.TargetPosition = Utility.MoveVector3Towards(param.targetPosition, radian, random);
			result.Add(distribution);
		}

		return ErrorCode.Succeeded;
	}

	#endregion CreateProjectileDetails
}

/// <summary>
/// 添加前端表现, 并根据表现是否循环, 而保存它们的运行时ID到指定的容器中.
/// 并可以通过以该容器为参数, 一次性移除所有的前端表现.
/// </summary>
public class SkillClientBehaviour
{
	/// <summary>
	/// 播放动画.
	/// </summary>
	public static void PlayAnimation(BattleUnit owner, string aniName, bool loop = false, ClientBehaviourIdContainer cont = null)
	{
		if (cont != null && cont.roleAnimationHashCode != 0)
		{
			owner.GetStateController().FinishCurrentState(cont.roleAnimationHashCode);
			cont.roleAnimationHashCode = 0;
		}

		if (!string.IsNullOrEmpty(aniName))
		{
			string animationName = owner.CombineAnimname(aniName);
			int hashCode = 0;
			if(owner.GetStateController().AnimSet != null)
				hashCode = owner.GetStateController().AnimSet.GetStateHash(animationName);

			if (hashCode != 0)
			{
				AnimActionUseSkill skillAction = AnimActionFactory.Create(AnimActionFactory.E_Type.UseSkill) as AnimActionUseSkill;
				skillAction.AnimName = animationName;

				skillAction.loop = loop;
				owner.GetStateController().DoAction(skillAction);

				// 动作无论循环与否, 都保存ID.
				if (cont != null)
					cont.roleAnimationHashCode = hashCode;
			}
		}
	}

	/// <summary>
	/// 播放武器动画.
	/// </summary>
	public static void PlayWeaponAnimation(BattleUnit owner, string aniName, bool loop = false, ClientBehaviourIdContainer cont = null)
	{
        if (!string.IsNullOrEmpty(aniName))
        {
            AnimActionUseSkill skillAction = AnimActionFactory.Create(AnimActionFactory.E_Type.UseSkill) as AnimActionUseSkill;
            skillAction.loop = loop;
            owner.PlayWeaponAnim(AnimationNameDef.WeaponPrefix + aniName);

            if (cont != null)
                cont.weaponAnimation = 1;
        }
	}

	/// <summary>
	/// 播放声音.
	/// </summary>
	public static void PlaySound(uint soundId, bool loop = false, ClientBehaviourIdContainer cont = null)
	{
		if (soundId != uint.MaxValue)
		{
			uint runtimeID = (uint)SoundManager.Instance.Play((int)soundId, null, loop);
			if (loop && cont != null)
				cont.soundId = runtimeID;
		}
	}

	/// <summary>
	/// 播放场景特效.
	/// </summary>
	public static void AddSceneEffect(uint effectID, Vector3 position, float dir = float.NaN, bool loop = false, ClientBehaviourIdContainer cont = null)
	{
		uint runtimeID = uint.MaxValue;
		BaseScene scn = SceneManager.Instance.GetCurScene();
		if (scn != null && effectID != uint.MaxValue)
		{
			runtimeID = scn.CreateEffect((int)effectID, Vector3.one, position, dir,null,!loop);
			if (runtimeID == uint.MaxValue)
				ErrorHandler.Parse(ErrorCode.ConfigError, "添加特效" + effectID + "失败");
        }

        EffectTableItem eti = DataManager.EffectTable[effectID] as EffectTableItem;
        if (eti != null && eti.loop && cont != null)
        {
            cont.threeDEffectInScene = runtimeID;
        }
    }

	/// <summary>
	/// 给单位添加特效.
	/// </summary>
	public static void AddEffect2Object(BattleUnit owner, uint effectID, string bindpoint, float dir = float.NaN, bool loop = false, ClientBehaviourIdContainer cont = null)
	{
		uint runtimeID = uint.MaxValue;
		if (effectID != uint.MaxValue)
		{
			runtimeID = owner.AddEffect(effectID, bindpoint, dir);
			if (runtimeID == uint.MaxValue)
				ErrorHandler.Parse(ErrorCode.ConfigError, "添加特效" + effectID + "失败");
		}

        EffectTableItem eti = DataManager.EffectTable[effectID] as EffectTableItem;


        if (eti != null && eti.loop && cont != null)
        {
            if (cont.threeDEffect2Role_0 == uint.MaxValue)
				cont.threeDEffect2Role_0 = runtimeID;
			else
				cont.threeDEffect2Role_1 = runtimeID;
		}
	}

	/// <summary>
	/// 根据存储的ID, 移除所有前端表现.
	/// </summary>
	public static void RemoveAll(BattleUnit owner, ClientBehaviourIdContainer identifiers)
	{
		if (identifiers.roleAnimationHashCode != 0)
		{
			owner.GetStateController().FinishCurrentState(identifiers.roleAnimationHashCode);
		}

		if (identifiers.soundId != uint.MaxValue)
		{
			SoundManager.Instance.RemoveSoundByID((int)identifiers.soundId);
		}

		if (identifiers.threeDEffect2Role_0 != uint.MaxValue)
		{
			owner.RemoveEffect(identifiers.threeDEffect2Role_0);
		}

		if (identifiers.threeDEffect2Role_1 != uint.MaxValue)
		{
			owner.RemoveEffect(identifiers.threeDEffect2Role_1);
		}

		if (identifiers.threeDEffectInScene != uint.MaxValue)
		{
			BaseScene scn = SceneManager.Instance.GetCurScene();
			if (scn != null)
				scn.RemoveEffect(identifiers.threeDEffectInScene);
		}

		if (identifiers.weaponAnimation != uint.MaxValue)
		{
            owner.PlayWeaponAnim(AnimationNameDef.WeaponDefault);
		}

		identifiers.Clear();
	}
}
