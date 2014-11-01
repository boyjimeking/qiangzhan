public static class LeagueManager
{
	/// <summary>
	/// 获取两个阵营间的关系.
	/// </summary>
	public static LeagueRelationship GetRelationship(LeagueDef league, LeagueDef other)
	{
		return leagueRelationship[(int)league, (int)other];
	}

	/// <summary>
	/// 返回target是否是attackerAttr标识的攻击者者所要寻找的目标.
	/// selection表示攻击者要寻找什么类型的目标.
	/// </summary>
	public static bool IsDisiredTarget(AttackerAttr attackerAttr, BattleUnit target, LeagueSelection selection)
	{
		if (target == null || target.IsDead() || selection == LeagueSelection.None)
			return false;

		if (attackerAttr.AttackerLeague == LeagueDef.InvalidLeague || target.GetLeague() == LeagueDef.InvalidLeague)
			return ErrorHandler.Parse(ErrorCode.ConfigError, "invalid league");

		if (selection >= LeagueSelection.Count)
			return ErrorHandler.Parse(ErrorCode.ConfigError, "invalid league selection " + (uint)selection);

		return IsTarget(attackerAttr, target, selection);
	}

	private static bool IsTarget(AttackerAttr attackerAttr, BattleUnit target, LeagueSelection selection)
	{
		// 自身不是Ally关系!
		if (attackerAttr.AttackerID == target.InstanceID)
			return (selection & LeagueSelection.Self) != 0;

		// 此时, 攻守双方不相同.
		LeagueRelationship r = GetRelationship(attackerAttr.AttackerLeague, target.GetLeague());
		switch (r)
		{
			case LeagueRelationship.Ally:
				return (selection & LeagueSelection.Ally) != 0;
			case LeagueRelationship.Enemy:
				return (selection & LeagueSelection.Enemy) != 0 && !target.IsInviolable();
			default:
				break;
		}

		return false;
	}

	/// <summary>
	/// LeagueDef.Count * LeagueDef.Count的二维数组.
	/// </summary>
	private static LeagueRelationship[,] leagueRelationship = new LeagueRelationship[(int)LeagueDef.Count, (int)LeagueDef.Count]
	{
		//            neutral					      red						      blue
		/*neutral*/ { LeagueRelationship.Independent, LeagueRelationship.Independent, LeagueRelationship.Independent },
		/*  red  */ { LeagueRelationship.Independent, LeagueRelationship.Ally,        LeagueRelationship.Enemy },
		/*  blue */ { LeagueRelationship.Independent, LeagueRelationship.Enemy,       LeagueRelationship.Ally }
	};
}

/// <summary>
/// 阵营选择的位定义, 已知一个阵营, 选择其他的与该阵营有如下关系的阵营.
/// </summary>
public enum LeagueSelection : uint
{	// 使用uint的低3位, 从0~2位分别表示: 选择自己, 选择友军, 选择敌人.
	None = 0,
	Self = 1,				// 自己.
	Ally = 2,				// 友军(不含自己).
	Enemy = 4,				// 敌人.

	// 自己和友军.
	SelfAndAlly = Self | Ally,

	// 自己和敌人.
	SelfAndEnemy = Self | Enemy,

	// 敌人和友军.
	AllyAndEnemy = Ally | Enemy,

	// 敌人, 友军和自己.
	SelfAllyAndEnemy = Self | Ally | Enemy,
	Count,
}

public enum LeagueDef : uint
{	
	Neutral,	// 中立阵营, 无论在任何关卡, 绝对安全.
	Red,		// 阵营1, 玩家的默认阵营, 与阵营2敌对.
	Blue,		// 阵营2, 非中立NPC的默认阵营, 与阵营1敌对.
	Count,
	InvalidLeague = uint.MaxValue,
}

/// <summary>
/// 两个阵营之间的关系.
/// </summary>
public enum LeagueRelationship
{
	Independent,	// 无关.
	Enemy,			// 敌对.
	Ally,			// 友军.
}
