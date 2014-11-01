using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 检查数据表的正确性.
/// </summary>
public class DataChecker
{
	Dictionary<DataType, string> mFileNameMap = new Dictionary<DataType, string>();
	static DataChecker mInstance = null;
	delegate bool Checker();
	List<Checker> mCheckerContainer = null;
	public bool Run()
	{
		foreach (Checker checker in mCheckerContainer)
		{
			if (!checker())
				return false;
		}

		return true;
	}

	/// <summary>
	/// 只有一个单例.
	/// </summary>
	private DataChecker() {
		mCheckerContainer = new List<Checker>();
		mCheckerContainer.Add(checkSkillCommon);
		mCheckerContainer.Add(checkSkillClientBehaviour);
		mCheckerContainer.Add(checkSkillEffect);
		mCheckerContainer.Add(checkTargetSelection);
		mCheckerContainer.Add(checkBullet);
		mCheckerContainer.Add(checkCreation);
		mCheckerContainer.Add(checkDisplacement);
		mCheckerContainer.Add(checkImpact);
		mCheckerContainer.Add(checkBuff);
		mCheckerContainer.Add(checkSkillRandEvent);
		mCheckerContainer.Add(checkProjectileSettings);
		mCheckerContainer.Add(checkTrap);
		mCheckerContainer.Add(checkBulletDistribution);
		mCheckerContainer.Add(checkMaterial);
        mCheckerContainer.Add(CheckItem);
        mCheckerContainer.Add(CheckGuide);
        mCheckerContainer.Add(checkWeapon);
        mCheckerContainer.Add(checkStrFilter);
        mCheckerContainer.Add(checkZhushou);
        //mCheckerContainer.Add(checkSound);
        mCheckerContainer.Add(checkUI);
	}

	public static DataChecker GetInstance()
	{
		if (mInstance == null) mInstance = new DataChecker();
		return mInstance;
	}

	public static void DestroyInstance() {
		mInstance = null;
	}

	/// <summary>
	/// 将数据类型对应的文件名存储.
	/// </summary>
	public void Append(DataType type, string filePath)
	{
		mFileNameMap.Add(type, System.IO.Path.GetFileName(filePath));
	}

    private bool CheckGuide()
    {
        return GuideManager.Instance.Init(DataManager.GuideTable);
    }

	private bool checkSkillCommon()
	{
		DataType myName = DataType.DATA_SKILL_COMMON;
        IDictionaryEnumerator itr = DataManager.SkillCommonTable.GetEnumerator();
        while (itr.MoveNext())
        {
            SkillCommonTableItem item = itr.Value as SkillCommonTableItem;
            if (!checkParam(item.myCd != uint.MaxValue, myName, item.resID, "cd"))
                return false;

            if (!checkParam(item.manaCost != uint.MaxValue && item.bulletCost != uint.MaxValue,
                myName, item.resID, "cost"))
                return false;

            if (!checkParam(item.skillUseStateDuration > item.skillEffectStartTime, myName, item.resID, "技能覆盖时间",
                "assert(技能开始时间 < 技能覆盖时间)"))
                return false;

            if (!checkParam(item.skillUseStateLoopLeft != 0, myName, item.resID, "技能使用阶段的循环次数"))
                return false;

            if (!checkParam(item.maxRange > 0 && item.minRange >= 0, myName, item.resID, "skillrange"))
                return false;

            if (item.chargeBehaviour != uint.MaxValue &&
                !checkLink(myName, item.resID, DataType.DATA_SKILL_BEHAVIOUR, item.chargeBehaviour))
                return false;

            if (item.useBehaviour != uint.MaxValue &&
                !checkLink(myName, item.resID, DataType.DATA_SKILL_BEHAVIOUR, item.useBehaviour))
                return false;

            if (item.buffToSkillUser != uint.MaxValue
                && !checkLink(myName, item.resID, DataType.DATA_SKILL_BUFF, item.buffToSkillUser))
                return false;

            if (item.buffToSkillUser != uint.MaxValue)
            {
                SkillBuffTableItem buffItem = DataManager.BuffTable[item.buffToSkillUser] as SkillBuffTableItem;
                if (!checkParam(buffItem.lifeMilliseconds == uint.MaxValue, DataType.DATA_SKILL_BUFF, (int)item.buffToSkillUser,
                    "buffToSkillUser", "控制行为的buff的持续时间必须为-1"))
                    return false;
            }

            if (item.skillEffect2UserInUseState != uint.MaxValue
                && !checkLink(myName, item.resID, DataType.DATA_SKILL_EFFECT, item.skillEffect2UserInUseState))
                return false;

            if (item.targetSelection != uint.MaxValue
                && !checkLink(myName, item.resID, DataType.DATA_SKILL_TARGET_SELECTION, item.targetSelection))
                return false;

            if (item.skillEffect2Others != uint.MaxValue
                && !checkLink(myName, item.resID, DataType.DATA_SKILL_EFFECT, item.skillEffect2Others))
                return false;

            if (item.projectileResID != uint.MaxValue
                && !checkLink(myName, item.resID, DataType.DATA_PROJECTILE_SETTINGS, item.projectileResID))
                return false;

            if (item.creationID != uint.MaxValue
                && !checkLink(myName, item.resID, DataType.DATA_SKILL_CREATION, item.creationID))
                return false;
        }
// 		foreach (int key in DataManager.SkillCommonTable.Keys)
// 		{
// 			
// 		}

		return true;
	}

	private bool checkSkillClientBehaviour()
	{
		DataType myName = DataType.DATA_SKILL_BEHAVIOUR;
        IDictionaryEnumerator itr = DataManager.SkillClientBehaviourTable.GetEnumerator();
        while (itr.MoveNext())
        {
            SkillClientBehaviourItem item = itr.Value as SkillClientBehaviourItem;

            uint[] sounds = Utility.SplitToUnsigned(item.soundResID);

            foreach (uint resID in sounds)
            {
                if (!checkLink(myName, item.resID, DataType.DATA_SOUND, resID))
                    return false;
            }

            if (item.effectID_0 != uint.MaxValue)
            {
                if (!checkParam(item.effectStartTime_0 != uint.MaxValue, myName, item.resID, "特效起始时间 I"))
                    return false;

                if (!checkLink(myName, item.resID, DataType.DATA_EFFECT, item.effectID_0))
                    return false;
            }

            if (item.effectID_1 != uint.MaxValue)
            {
                if (!checkParam(item.effectStartTime_1 != uint.MaxValue, myName, item.resID, "特效起始时间 II"))
                    return false;

                if (!checkLink(myName, item.resID, DataType.DATA_EFFECT, item.effectID_1))
                    return false;
            }
        }
// 		foreach (int key in DataManager.SkillClientBehaviourTable.Keys)
// 		{
// 			
// 		}

		return true;
	}

	private bool checkSkillEffect()
	{
		DataType myName = DataType.DATA_SKILL_EFFECT;
        IDictionaryEnumerator itr = DataManager.SkillEffectTable.GetEnumerator();
        while (itr.MoveNext())
        {
            SkillEffectTableItem item = itr.Value as SkillEffectTableItem;

            for (uint i = 0; i < SkillEffectTableItem.SkillEffectCount; ++i)
            {
                SkillEffectItem subItem = item.items[i];
                SkillEffectType type = subItem.effectType;
                uint resId = subItem.effectID;
                switch (type)
                {
                    case SkillEffectType.Buff:
                        if (!checkLink(myName, item.resID, DataType.DATA_SKILL_BUFF, resId))
                            return false;
                        break;
                    case SkillEffectType.Displacement:
                        if (!checkLink(myName, item.resID, DataType.DATA_SKILL_DISPLACEMENT, resId))
                            return false;
                        break;
                    case SkillEffectType.Impact:
                        if (!checkLink(myName, item.resID, DataType.DATA_SKILL_IMPACT, resId))
                            return false;
                        break;
                    case SkillEffectType.Spasticity:
                        if (!checkLink(myName, item.resID, DataType.DATA_SKILL_SPASTICITY, resId))
                            return false;
                        break;
                    case SkillEffectType.Invalid:
                        break;
                    default:
                        checkParam(false, myName, item.resID, "效果类型");
                        return false;
                }
            }
        }
// 		foreach (int key in DataManager.SkillEffectTable.Keys)
// 		{
// 			
// 		}

		return true;
	}

	private bool checkTargetSelection()
	{
		DataType myName = DataType.DATA_SKILL_TARGET_SELECTION;

        IDictionaryEnumerator itr = DataManager.TargetSelectionTable.GetEnumerator();
        while (itr.MoveNext())
        {
            TargetSelectionTableItem item = itr.Value as TargetSelectionTableItem;
            if (!checkParam(item.maxTargetCount != 0, myName, item.resID, "最大目标个数", "最大目标个数不可为0"))
                return false;

            if (!checkParam(item.leagueSel < LeagueSelection.Count, myName, item.resID, "敌我识别"))
                return false;
        }
// 		foreach (int key in DataManager.TargetSelectionTable.Keys)
// 		{
// 			TargetSelectionTableItem item = DataManager.TargetSelectionTable[key] as TargetSelectionTableItem;
// 			if (!checkParam(item.maxTargetCount != 0, myName, key, "最大目标个数", "最大目标个数不可为0"))
// 				return false;
// 
// 			if (!checkParam(item.leagueSel < LeagueSelection.Count, myName, key, "敌我识别"))
// 				return false;
// 
// 		}

		return true;
	}

	private bool checkBullet()
	{
		DataType myName = DataType.DATA_SKILL_BULLET;
        IDictionaryEnumerator itr = DataManager.BulletTable.GetEnumerator();
        while (itr.MoveNext())
        {
            BulletTableItem item = itr.Value as BulletTableItem;
            if (!checkParam(item.type < BulletType.BulletTypeCount, myName, item.resID, "bullet type"))
                return false;

            if (item.bulletFigureID != uint.MaxValue
                && !checkLink(myName, item.resID, DataType.DATA_EFFECT, item.bulletFigureID))
                return false;

            if (item.targetSelectionOnArrive != uint.MaxValue
                && !checkLink(myName, item.resID, DataType.DATA_SKILL_TARGET_SELECTION, item.targetSelectionOnArrive))
                return false;

            if (!checkParam(item.flySpeed >= 0f, myName, item.resID, "speed"))
                return false;

            if (!checkParam(item.accelerateSpeed >= 0f, myName, item.resID, "加速度"))
                return false;

            if (!checkParam(item.accelerateDelay != uint.MaxValue, myName, item.resID, "加速度延迟", "不需要加速度延迟时, 需填入0"))
                return false;

            if (!checkParam(item.flyRange != 0, myName, item.resID, "子弹射程", "射程不能为0"))
                return false;

            bool flyHorizontally = (item.type != BulletType.BulletTypeCannonball && item.type != BulletType.BulletTypeGrenade && item.type != BulletType.BulletTypeBomb);
            string dumpMessage = "类型为" + (uint)item.type + "的子弹" + (flyHorizontally ? "射程不可为-1" : "不支持射程(填入-1)");

            if (!checkParam(flyHorizontally == (item.flyRange != uint.MaxValue), myName, item.resID, "子弹射程", dumpMessage))
                return false;

            if (!checkParam(item.type != BulletType.BulletTypeMissile || item.bulletFlyParam_0 >= 0f,
                myName, item.resID, "追踪延迟", "追踪延迟必须不小于0"))
                return false;

            if (!checkParam(item.type != BulletType.BulletTypeMissile || item.bulletFlyParam_1 != 0, myName, item.resID,
                "追踪持续时间", "追踪持续时间不能为0"))
                return false;

            if (!checkParam(item.flyHitCount > 0, myName, item.resID, "穿透人数", "穿透人数必须大于0"))
                return false;

            if (!checkParam(item.leagueSelection < LeagueSelection.Count, myName, item.resID, "阵营选择"))
                return false;

            if (!checkParam(item.radiusOnCollide > 0, myName, item.resID, "碰撞半径"))
                return false;

            if (item.skilleffectOnFlyCollide != uint.MaxValue
                && !checkLink(myName, item.resID, DataType.DATA_SKILL_EFFECT, item.skilleffectOnFlyCollide))
                return false;

            if (item.threeDEffectOnArrive != uint.MaxValue
                && !checkLink(myName, item.resID, DataType.DATA_EFFECT, item.threeDEffectOnArrive))
                return false;

            if (item.targetSelectionOnArrive != uint.MaxValue
                && !checkLink(myName, item.resID, DataType.DATA_SKILL_TARGET_SELECTION, item.targetSelectionOnArrive))
                return false;

            if (item.skilleffectOnArrive != uint.MaxValue
                && !checkLink(myName, item.resID, DataType.DATA_SKILL_EFFECT, item.skilleffectOnArrive))
                return false;

            if (item.creationOnArrive != uint.MaxValue
                && !checkLink(myName, item.resID, DataType.DATA_SKILL_CREATION, item.creationOnArrive))
                return false;

            if (!checkParam(item.flyRange > 0, myName, item.resID, "子弹射程"))
                return false;

            if (!checkParam(item.type != BulletType.BulletTypeCircleArc || item.bulletFlyParam_0 > 0,
                myName, item.resID, "轨迹控制的参数 I", "该值必须大于0"))
                return false;

            if (!checkParam(item.type != BulletType.BulletTypeSineWave || (item.bulletFlyParam_0 != 0 && item.bulletFlyParam_1 != 0),
                myName, item.resID, "轨迹控制参数I或II", "二者均不可为0"))
                return false;
        }
// 		foreach (int key in DataManager.BulletTable.Keys)
// 		{
// 			
// 		}

		return true;
	}

	private bool checkCreation()
	{
		DataType myName = DataType.DATA_SKILL_CREATION;
        IDictionaryEnumerator itr = DataManager.CreationTable.GetEnumerator();
        while (itr.MoveNext())
        {
            SkillCreationTableItem item = itr.Value as SkillCreationTableItem;

            for (uint i = 0; i < SkillCreationTableItem.CreationCount; ++i)
            {
                SkillCreationItem subItem = item.items[i];

                CreationType creationType = subItem.creationType;
                uint creatureID = subItem.creatureID;
                switch (creationType)
                {
                    case CreationType.Invalid:
                        break;
                    case CreationType.Npc:
                        if (!checkLink(myName, item.resID, DataType.DATA_NPC, creatureID))
                            return false;
                        break;
                    case CreationType.Trap:
                        if (!checkLink(myName, item.resID, DataType.DATA_TRAP, creatureID))
                            return false;
                        break;
                    default:
                        checkParam(false, myName, item.resID, "召唤物类型");
                        return false;
                }
            }
        }
// 		foreach (int key in DataManager.CreationTable.Keys)
// 		{
// 			SkillCreationTableItem item = DataManager.CreationTable[key] as SkillCreationTableItem;
// 
// 			for (uint i = 0; i < SkillCreationTableItem.CreationCount; ++i)
// 			{
// 				SkillCreationItem subItem = item.items[i];
// 
// 				CreationType creationType = subItem.creationType;
// 				uint creatureID = subItem.creatureID;
// 				switch (creationType)
// 				{
// 					case CreationType.Invalid:
// 						break;
// 					case CreationType.Npc:
// 						if (!checkLink(myName, key, DataType.DATA_NPC, creatureID))
// 							return false;
// 						break;
// 					case CreationType.Trap:
// 						if (!checkLink(myName, key, DataType.DATA_TRAP, creatureID))
// 							return false;
// 						break;
// 					default:
// 						checkParam(false, myName, key, "召唤物类型");
// 						return false;
// 				}
// 			}
// 		}

		return true;
	}

	private bool checkDisplacement()
	{
		DataType myName = DataType.DATA_SKILL_DISPLACEMENT;
        IDictionaryEnumerator itr = DataManager.DisplacementTable.GetEnumerator();
        while (itr.MoveNext())
        {
            SkillDisplacementTableItem item = itr.Value as SkillDisplacementTableItem;
            if (!checkParam(item.displacementType < SkillDisplacementType.Count, myName, item.resID, "type"))
                return false;

            if (!checkParam(!Utility.isZero(item.speed), myName, item.resID, "speed"))
                return false;

            if (item._3DEffectIdOnExecute != uint.MaxValue
                && !checkLink(myName, item.resID, DataType.DATA_EFFECT, item._3DEffectIdOnExecute))
                return false;

            if (!checkParam(item.leagueSelectionOnExecute < LeagueSelection.Count, myName, item.resID, "阵营识别"))
                return false;

            if (!checkParam(item.radiusOnCollide >= 0, myName, item.resID, "碰撞半径"))
                return false;

            if (!checkParam(!item.interruptSkillUsing || item.displacementType == SkillDisplacementType.Beatback,
                myName, item.resID, "打断技能", "只有类型为" + (uint)SkillDisplacementType.Beatback + "的displacement才可以打断技能"))
                return false;

            if (item.skillEffect2OthersOnExecute != uint.MaxValue
                && !checkLink(myName, item.resID, DataType.DATA_SKILL_EFFECT, item.skillEffect2OthersOnExecute))
                return false;

            if (item.skillEffect2SelfOnArrive != uint.MaxValue
                && !checkLink(myName, item.resID, DataType.DATA_SKILL_EFFECT, item.skillEffect2SelfOnArrive))
                return false;

            if (item.targetSelectionOnArrive != uint.MaxValue
                && !checkLink(myName, item.resID, DataType.DATA_SKILL_TARGET_SELECTION, item.targetSelectionOnArrive))
                return false;

            if (item.skillEffect2OthersOnArrive != uint.MaxValue
                && !checkLink(myName, item.resID, DataType.DATA_SKILL_EFFECT, item.skillEffect2OthersOnArrive))
                return false;
        }
// 		foreach (int key in DataManager.DisplacementTable.Keys)
// 		{
// 			
// 		}

		return true;
	}

	private bool checkImpact()
	{
		DataType myName = DataType.DATA_SKILL_IMPACT;
		const string onlyHarmfulAvailable = "该列只对有害的impact有效, 无害的需要填入0";
        IDictionaryEnumerator itr = DataManager.ImpactTable.GetEnumerator();
        while (itr.MoveNext())
        {
            SkillImpactTableItem item = itr.Value as SkillImpactTableItem;

            if (!item.harmful)
            {
                if (!checkParam(item.damageAddPerMile == 0, myName, item.resID, "附加距离伤害",
                    onlyHarmfulAvailable))
                    return false;

                if (!checkParam(item.damageAttrPercent == 0, myName, item.resID, "伤害属性百分比",
                    onlyHarmfulAvailable))
                    return false;
            }
            else
            {
                if (!checkParam(item.impactDamageType < ImpactDamageType.Count, myName, item.resID, "伤害类型"))
                    return false;
            }

            if (!checkParam(item.shakeCameraDuration != uint.MaxValue, myName, item.resID, "无效的震屏时间"))
                return false;

            if (!checkParam(item.damageAttrPercent >= 0f, myName, item.resID, "伤害属性百分比", "伤害属性百分比 ≥ 0"))
                return false;

            if (item._3DEffectID != uint.MaxValue
                && !checkLink(myName, item.resID, DataType.DATA_EFFECT, item._3DEffectID))
                return false;
        }
// 		foreach (int key in DataManager.ImpactTable.Keys)
// 		{
// 			
// 		}

		return true;
	}

	private bool checkBuff()
	{
		DataType myName = DataType.DATA_SKILL_BUFF;
        IDictionaryEnumerator itr = DataManager.BuffTable.GetEnumerator();
        while (itr.MoveNext())
        {
            SkillBuffTableItem item = itr.Value as SkillBuffTableItem;
            if (!checkParam(item.removeCondition < BuffRemoveCondition.Count, myName, item.resID, "buff移除条件"))
                return false;

            if (!checkParam(item.stackCountMax > 0, myName, item.resID, "叠加次数"))
                return false;

            if (item.dotEffectTargetSelection != uint.MaxValue
                && !checkLink(myName, item.resID, DataType.DATA_SKILL_TARGET_SELECTION, item.dotEffectTargetSelection))
                return false;

            if (item.dotEffect2Others != uint.MaxValue
                && !checkLink(myName, item.resID, DataType.DATA_SKILL_EFFECT, item.dotEffect2Others))
                return false;

            if (item.randEvent != uint.MaxValue
                && !checkLink(myName, item.resID, DataType.DATA_SKILL_RAND_EVENT, item.randEvent))
                return false;

            if (item.newModelID != uint.MaxValue
                && !checkLink(myName, item.resID, DataType.DATA_MODEL, item.newModelID))
                return false;

            if (item.newWeaponID != uint.MaxValue
                && !checkLink(myName, item.resID, DataType.DATA_WEAPON, item.newWeaponID))
                return false;

            if (item.superNewWeaponID != uint.MaxValue
                && !checkLink(myName, item.resID, DataType.DATA_WEAPON, item.superNewWeaponID))
                return false;

            try
            {
                SkillUtilities.ParseProperties(item.properties);
            }
            catch (System.Exception exp)
            {
                GameDebug.LogError("buff.txt[" + item.resID + "]的属性格式错误.");
            }

            if (item.effect2OwnerOnExpired != uint.MaxValue
                && !checkLink(myName, item.resID, DataType.DATA_SKILL_EFFECT, item.effect2OwnerOnExpired))
                return false;

            if (item.targetSelectionOnExpired != uint.MaxValue
                && !checkLink(myName, item.resID, DataType.DATA_SKILL_TARGET_SELECTION, item.targetSelectionOnExpired))
                return false;

            if (item.effect2OthersOnExpired != uint.MaxValue
                && !checkLink(myName, item.resID, DataType.DATA_SKILL_EFFECT, item.effect2OthersOnExpired))
                return false;

            if (item._3DEffectID != uint.MaxValue
                && !checkLink(myName, item.resID, DataType.DATA_EFFECT, item._3DEffectID))
                return false;
        }
// 		foreach (int key in DataManager.BuffTable.Keys)
// 		{
// 			
// 		}

		return true;
	}

	private bool checkSkillRandEvent()
	{
		DataType myName = DataType.DATA_SKILL_RAND_EVENT;
        IDictionaryEnumerator itr = DataManager.RandEventTable.GetEnumerator();
         while (itr.MoveNext())
         {
             SkillRandEventTableItem item = itr.Value as SkillRandEventTableItem;
             if (!checkParam(item.triggerType < RandEventTriggerType.Count, myName, item.resID, "触发类型"))
                 return false;

             if (item._3DEffectID != uint.MaxValue && !checkLink(myName, item.resID, DataType.DATA_EFFECT, item._3DEffectID))
                 return false;

             if (!checkParam(item.triggerScript == SkillScriptDef.Invalid || item.triggerScript < SkillScriptDef.Count,
                 myName, item.resID, "脚本ID"))
                 return false;

             SkillScript script = SkillScriptFactory.Instance.Allocate(item.triggerScript, null);

             if (!checkParam(script.ScriptTriggerType == item.triggerType, myName, item.resID, "触发类型", "脚本的触发类型和随机事件的触发类型不一致"))
                 return false;
         }
// 		foreach (int key in DataManager.RandEventTable.Keys)
// 		{
// 			SkillRandEventTableItem item = DataManager.RandEventTable[key] as SkillRandEventTableItem;
// 			if (!checkParam(item.triggerType < RandEventTriggerType.Count, myName, key, "触发类型"))
// 				return false;
// 
// 			if (item._3DEffectID != uint.MaxValue && !checkLink(myName, key, DataType.DATA_EFFECT, item._3DEffectID))
// 				return false;
// 
// 			if (!checkParam(item.triggerScript == SkillScriptDef.Invalid || item.triggerScript < SkillScriptDef.Count,
// 				myName, key, "脚本ID"))
// 				return false;
// 
// 			SkillScript script = SkillScriptFactory.Instance.Allocate(item.triggerScript, null);
// 
// 			if (!checkParam(script.ScriptTriggerType == item.triggerType, myName, key, "触发类型", "脚本的触发类型和随机事件的触发类型不一致"))
// 				return false;
// 		}

		return true;
	}

	private bool checkProjectileSettings()
	{
		DataType myName = DataType.DATA_PROJECTILE_SETTINGS;
		System.Type T = typeof(ProjectileSettingsTableItem);
        IDictionaryEnumerator itr = DataManager.ProjectileSettingsTable.GetEnumerator();
        while (itr.MoveNext())
        {
            ProjectileSettingsTableItem item = itr.Value as ProjectileSettingsTableItem;
            for (uint i = 0; i < ProjectileSettingsTableItem.ProjectileCount; ++i)
            {
                ProjectileItem subItem = item.items[i];

                uint bindpointEffect = subItem.bindpointEffect;
                uint bulletResID = subItem.bulletResID;
                BulletDistributionType distributionType = subItem.distributionType;
                string distributionArgument = subItem.distributionArgument;

                if (bulletResID == uint.MaxValue)
                    continue;

                if (bindpointEffect != uint.MaxValue
                    && !checkLink(myName, item.resID, DataType.DATA_EFFECT, bindpointEffect))
                    return false;

                if (!checkLink(myName, item.resID, DataType.DATA_SKILL_BULLET, bulletResID))
                    return false;

                string[] args = distributionArgument.Split('|');
                switch (distributionType)
                {
                    case BulletDistributionType.ByAngle:
                    case BulletDistributionType.ByRandomOffset:
                        if (!checkParam(args.Length == 2, myName, item.resID, "分布参数"))
                            return false;
                        break;
                    case BulletDistributionType.ByData:
                        foreach (string str in args)
                        {
                            uint distribution = System.Convert.ToUInt32(str);
                            if (!checkLink(myName, item.resID, DataType.DATA_BULLET_DISTRIBUTION, distribution))
                                return false;
                        }
                        break;
                    default:
                        checkParam(false, myName, item.resID, "分布类型");
                        return false;
                }
            }
        }
// 		foreach (int key in DataManager.ProjectileSettingsTable.Keys)
// 		{
// 			ProjectileSettingsTableItem item = DataManager.ProjectileSettingsTable[key] as ProjectileSettingsTableItem;
// 			for (uint i = 0; i < ProjectileSettingsTableItem.ProjectileCount; ++i)
// 			{
// 				ProjectileItem subItem = item.items[i];
// 
// 				uint bindpointEffect = subItem.bindpointEffect;
// 				uint bulletResID = subItem.bulletResID;
// 				BulletDistributionType distributionType = subItem.distributionType;
// 				string distributionArgument = subItem.distributionArgument;
// 
// 				if (bulletResID == uint.MaxValue)
// 					continue;
// 
// 				if (bindpointEffect != uint.MaxValue
// 					&& !checkLink(myName, key, DataType.DATA_EFFECT, bindpointEffect))
// 					return false;
// 
// 				if (!checkLink(myName, key, DataType.DATA_SKILL_BULLET, bulletResID))
// 					return false;
// 
// 				string[] args = distributionArgument.Split('|');
// 				switch (distributionType)
// 				{ 
// 					case BulletDistributionType.ByAngle:
// 					case BulletDistributionType.ByRandomOffset:
// 						if (!checkParam(args.Length == 2, myName, key, "分布参数"))
// 							return false;
// 						break;
// 					case BulletDistributionType.ByData:
// 						foreach (string str in args)
// 						{
// 							uint distribution = System.Convert.ToUInt32(str);
// 							if (!checkLink(myName, key, DataType.DATA_BULLET_DISTRIBUTION, distribution))
// 								return false;
// 						}
// 						break;
// 					default:
// 						checkParam(false, myName, key, "分布类型");
// 						return false;
// 				}
// 			}
// 		}

		return true;
	}

	private bool checkTrap()
	{
		DataType myName = DataType.DATA_TRAP;
        IDictionaryEnumerator itr = DataManager.TrapTable.GetEnumerator();
        while (itr.MoveNext())
        {
            TrapTableItem item = itr.Value as TrapTableItem;
            if (!checkParam(item.modelID != uint.MaxValue, myName, item.resID, "模型ID"))
                return false;

            if (!checkLink(myName, item.resID, DataType.DATA_MODEL, item.modelID))
                return false;

            if (item.buffID != uint.MaxValue
                && !checkLink(myName, item.resID, DataType.DATA_SKILL_BUFF, item.buffID))
                return false;

            if (!checkParam(item.explodeDelay != uint.MaxValue, myName, item.resID, "爆炸延迟"))
                return false;

            if (item.buffID != uint.MaxValue
                && !checkParam(
                    !SkillUtilities.IsHarmfulEffect(SkillEffectType.Buff, item.buffID),
                    DataType.DATA_TRAP, item.resID, "出生buff", "不能为trap添加有害的出生buff."))
                return false;

            if (item.delayEffect != uint.MaxValue && !checkLink(myName, item.resID, DataType.DATA_EFFECT, item.delayEffect))
                return false;

            if (!checkParam(item.collideRadius != uint.MaxValue, myName, item.resID, "碰撞检测半径", "无效的碰撞检测半径需要填入0"))
                return false;

            if (item.targetSelectionOnExplode != uint.MaxValue
                && !checkLink(myName, item.resID, DataType.DATA_SKILL_TARGET_SELECTION, item.targetSelectionOnExplode))
                return false;

            if (item.skillEffectOnExplode != uint.MaxValue
                && !checkLink(myName, item.resID, DataType.DATA_SKILL_EFFECT, item.skillEffectOnExplode))
                return false;

            if (item._3DEffectOnExplode != uint.MaxValue
                && !checkLink(myName, item.resID, DataType.DATA_EFFECT, item._3DEffectOnExplode))
                return false;
        }
// 		foreach (int key in DataManager.TrapTable.Keys)
// 		{ 
// 			TrapTableItem item = DataManager.TrapTable[key] as TrapTableItem;
// 			if(!checkParam(item.modelID != uint.MaxValue, myName, key, "模型ID"))
// 				return false;
// 
// 			if (!checkLink(myName, key, DataType.DATA_MODEL, item.modelID))
// 				return false;
// 
// 			if (item.buffID != uint.MaxValue
// 				&& !checkLink(myName, key, DataType.DATA_SKILL_BUFF, item.buffID))
// 				return false;
// 
// 			if (!checkParam(item.explodeDelay != uint.MaxValue, myName, key, "爆炸延迟"))
// 				return false;
// 
// 			if (item.buffID != uint.MaxValue
// 				&& !checkParam(
// 					!SkillUtilities.IsHarmfulEffect(SkillEffectType.Buff, item.buffID),
// 					DataType.DATA_TRAP, key, "出生buff", "不能为trap添加有害的出生buff."))
// 				return false;
// 
// 			if (item.delayEffect != uint.MaxValue && !checkLink(myName, key, DataType.DATA_EFFECT, item.delayEffect))
// 				return false;
// 
// 			if (!checkParam(item.collideRadius != uint.MaxValue, myName, key, "碰撞检测半径", "无效的碰撞检测半径需要填入0"))
// 				return false;
// 
// 			if (item.targetSelectionOnExplode != uint.MaxValue
// 				&& !checkLink(myName, key, DataType.DATA_SKILL_TARGET_SELECTION, item.targetSelectionOnExplode))
// 				return false;
// 
// 			if (item.skillEffectOnExplode != uint.MaxValue
// 				&& !checkLink(myName, key, DataType.DATA_SKILL_EFFECT, item.skillEffectOnExplode))
// 				return false;
// 
// 			if (item._3DEffectOnExplode != uint.MaxValue
// 				&& !checkLink(myName, key, DataType.DATA_EFFECT, item._3DEffectOnExplode))
// 				return false;
// 		}

		return true;
	}

	private bool checkBulletDistribution()
	{
		//DataType myName = DataType.DATA_BULLET_DISTRIBUTION;
// 		foreach (int key in DataManager.BulletDistributionTable.Keys)
// 		{ 
// 			
// 		}

		return true;
	}

	private bool checkWeapon()
	{
		DataType myName = DataType.DATA_WEAPON;
        IDictionaryEnumerator itr = DataManager.WeaponTable.GetEnumerator();
        while (itr.MoveNext())
        {
            WeaponTableItem item = itr.Value as WeaponTableItem;
            if (item.upgrade < 0)
                continue;

            if (!DataManager.PromoteTable.ContainsKey((int)item.upgrade))
            {
                GameDebug.LogError("进阶ID为" + item.upgrade.ToString() + "不存在表格promote.txt中 ");
                return false;
            }

            if (item.skill_1 >= 0)
            {
                SkillCommonTableItem fireSkill = DataManager.SkillCommonTable[item.skill_1] as SkillCommonTableItem;
                if (fireSkill == null)
                {
                    checkParam(false, myName, item.id, "射击技能 I", "未找到资源");
                    return false;
                }

                if (!checkParam(fireSkill.isRegularAttack, DataType.DATA_SKILL_COMMON, fireSkill.resID, "是否为普通攻击", "武器技能必须标识为普通攻击"))
                    return false;
            }

            if (item.skill_2 >= 0)
            {
                SkillCommonTableItem fireSkill2 = DataManager.SkillCommonTable[item.skill_2] as SkillCommonTableItem;
                if (fireSkill2 == null)
                {
                    checkParam(false, myName, item.id, "射击技能 II", "未找到资源");
                    return false;
                }

                if (!checkParam(fireSkill2.isRegularAttack, DataType.DATA_SKILL_COMMON, fireSkill2.resID, "是否为普通攻击", "武器技能必须标识为普通攻击"))
                    return false;
            }
        }
// 		foreach (int key in DataManager.WeaponTable.Keys)
// 		{
// 			
// 		}

		return true;
	}

	private bool checkMaterial()
	{
		DataType myName = DataType.DATA_MATERIAL;
		System.Type T = typeof(MaterialTableItem);
        IDictionaryEnumerator itr = DataManager.MaterialTable.GetEnumerator();
        while (itr.MoveNext())
        {
            MaterialTableItem item = itr.Value as MaterialTableItem;
            for (uint i = 0; i < (uint)ImpactDamageType.Count; ++i)
            {
                MaterialItem subItem = item.items[i];
                uint hitEffect = subItem.hitEffect;
                uint sound = subItem.sound;
                uint deathEffect = subItem.deathEffect;

                if (hitEffect != uint.MaxValue
                    && !checkLink(myName, item.resID, DataType.DATA_EFFECT, hitEffect))
                    return false;

                if (sound != uint.MaxValue
                    && !checkLink(myName, item.resID, DataType.DATA_SOUND, sound))
                    return false;

                if (deathEffect != uint.MaxValue
                    && !checkLink(myName, item.resID, DataType.DATA_EFFECT, deathEffect))
                    return false;
            }
        }
// 		foreach (int key in DataManager.MaterialTable.Keys)
// 		{
// 			MaterialTableItem item = DataManager.MaterialTable[key] as MaterialTableItem;
// 			for (uint i = 0; i < (uint)ImpactDamageType.Count; ++i)
// 			{
// 				MaterialItem subItem = item.items[i];
// 				uint hitEffect = subItem.hitEffect;
// 				uint sound = subItem.sound;
// 				uint deathEffect = subItem.deathEffect;
// 
// 				if (hitEffect != uint.MaxValue
// 					&& !checkLink(myName, key, DataType.DATA_EFFECT, hitEffect))
// 					return false;
// 
// 				if (sound != uint.MaxValue
// 					&& !checkLink(myName, key, DataType.DATA_SOUND, sound))
// 					return false;
// 
// 				if (deathEffect != uint.MaxValue
// 					&& !checkLink(myName, key, DataType.DATA_EFFECT, deathEffect))
// 					return false;
// 			}
// 		}

		return true;
	}

	private bool CheckItem()
    {
        //Hashtable map = null;

        IDictionaryEnumerator itr = DataManager.NormalItemTable.GetEnumerator();
        while (itr.MoveNext())
        {
            NormalItemTableItem norres = itr.Value as NormalItemTableItem;
            if (ItemManager.GetItemType((uint)norres.id) != ItemType.Normal)
            {
                GameDebug.LogError("道具ID非法。id = " + norres.id.ToString());
                return false;
            }
        }
//         map = DataManager.NormalItemTable;
//         foreach (NormalItemTableItem norres in map.Values)
//         {
//             if(ItemManager.GetItemType((uint)norres.id) != ItemType.Normal)
//             {
//                 GameDebug.LogError("道具ID非法。id = " + norres.id.ToString());
//                 return false;
//             }
//         }
        itr = DataManager.DefenceTable.GetEnumerator();
        while (itr.MoveNext())
        {
            DefenceTableItem deres = itr.Value as DefenceTableItem;
            if (ItemManager.GetItemType((uint)deres.id) != ItemType.Defence)
            {
                GameDebug.LogError("装备ID非法。id = " + deres.id.ToString());
                return false;
            }
        }
//         map = DataManager.DefenceTable;
//         foreach (DefenceTableItem deres in map.Values)
//         {
// 
//         }
        itr = DataManager.WeaponTable.GetEnumerator();
        while (itr.MoveNext())
        {
            WeaponTableItem wres = itr.Value as WeaponTableItem;
            if (ItemManager.GetItemType((uint)wres.id) != ItemType.Weapon)
            {
                GameDebug.LogError("武器ID非法。id = " + wres.id.ToString());
                return false;
            }
        }

//         map = DataManager.WeaponTable;
//         foreach (WeaponTableItem wres in map.Values)
//         {
//             if (ItemManager.GetItemType((uint)wres.id) != ItemType.Weapon)
//             {
//                 GameDebug.LogError("武器ID非法。id = " + wres.id.ToString());
//                 return false;
//             }
//         }
        itr = DataManager.BoxItemTable.GetEnumerator();
        while (itr.MoveNext())
        {
            BoxItemTableItem wres = itr.Value as BoxItemTableItem;
            if (ItemManager.GetItemType((uint)wres.id) != ItemType.Box)
            {
                GameDebug.LogError("箱子道具ID非法。id = " + wres.id.ToString());
                return false;
            }
        }
//         map = DataManager.BoxItemTable;
//         foreach (BoxItemTableItem wres in map.Values)
//         {
//             if (ItemManager.GetItemType((uint)wres.id) != ItemType.Box)
//             {
//                 GameDebug.LogError("箱子道具ID非法。id = " + wres.id.ToString());
//                 return false;
//             }
//         }
        return true;
    }

    private bool checkStrFilter()
    {
        DataTable table = DataManager.Instance.GetTable(DataType.DATA_STR_FILTER);
        return StrFilterManager.Instance.Init(table);
    }

    private bool checkZhushou()
    {
        DataTable table = DataManager.Instance.GetTable(DataType.DATA_ZHUSHOU);
        return ZhushouManager.Instance.Init(table);
    }


//     private bool checkSound()
//     {
//         DataTable table = DataManager.Instance.GetTable(DataType.DATA_SOUND);
//         return SoundManager.Instance.Init(table);
//     }

    private bool checkUI()
    {
        DataTable table = DataManager.Instance.GetTable(DataType.DATA_UICONFIG);
        return WindowManager.Instance.InitCaches(table);
    }

	#region Helpers
	/// <summary>
	/// 检查从数据表fromTable的fromTableId到toTable的toTableId的链接是否存在.
	/// </summary>
	private bool checkLink(DataType fromTable, int fromTableId, DataType toTable, uint toTableId)
	{
		DataTable table = DataManager.Instance.GetTable(toTable);
		if (table[toTableId] == null)
		{
			GameDebug.LogError(
				string.Format("没有找到从{0}[{1}]到{2}[{3}]的链接", mFileNameMap[fromTable],
					fromTableId, mFileNameMap[toTable], toTableId)
				);
			return false;
		}
		return true;
	}

	private bool checkParam(bool expr, DataType tableName, int id, string paramName, string dump = "")
	{
		if (!expr)
		{
			string message = string.Format("{0}[{1}]上的字段\"{2}\"无效", mFileNameMap[tableName], id, paramName);
			if (!string.IsNullOrEmpty(dump))
				message += ": " + dump;

			GameDebug.LogError(message);
		}

		return expr;
	}
	#endregion
}
