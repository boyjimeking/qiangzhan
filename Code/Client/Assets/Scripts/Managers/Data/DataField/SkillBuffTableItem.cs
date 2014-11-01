public class SkillBuffTableItem
{
	public int resID = -1;
	public string desc = "system";

	public string icon;

	public bool harmful;

	public uint lifeMilliseconds = 0;

	public BuffRemoveCondition removeCondition;

	// buff组, 用来删除多个同组buff.
	public uint group;

	// 最大叠加次数.
	public uint stackCountMax = 1;

	// 互斥类型, 相同互斥类型的buff互斥.
	// 同为uint.MaxValue的互斥类型之间不互斥.
	public uint mutex = uint.MaxValue;

	// 每隔dotEffectTimeInterval毫秒, 对周围的单位产生效果.
	public uint dotEffectTimeInterval = uint.MaxValue;

	// 给buff拥有者的效果.
	public uint dotEffect2Owner = uint.MaxValue;

	// 以发起者的位置为中心, 应用选择参数来添加dot效果(targetselection.txt).
	public uint dotEffectTargetSelection = uint.MaxValue;

	// dot效果的资源(skilleffect.txt).
	public uint dotEffect2Others = uint.MaxValue;

	// 随机事件资源ID(skillrandevent.txt).
	public uint randEvent;

	// 属性修改. 需要遵照"技能表格说明.xlsx/buff"中的该列说明.
	public string properties;

	public bool disableMovement;
	public bool disableAttack;
	public bool disableSkillUse;
	public bool disableRotate;

	// 免疫控制.
	public bool stunImmunity;

	// 免疫伤血.
	public bool damageImmunity;

	// see ActiveFlagsDef.Inviolability.
	public bool inviolability;

	// 磁铁效果, 可以吸收Pick.
	public bool magneticEffect;

	// 模型描边效果.
	public bool strokeEffect;

	// buff消失后, 对buff拥有者的效果(skilleffect.txt).
	public uint effect2OwnerOnExpired;
	// buff消失后, 在buff拥有者周围进行目标选择.
	public uint targetSelectionOnExpired;
	// 根据上述目标选择, 在buff消失时对目标产生效果.
	public uint effect2OthersOnExpired;

	// buff消失后, 在buff拥有者附近进行创建.
	public uint creationAroundOwnerOnExpired;

	// 变模型.
	public uint newModelID;

	// 变武器.
	public uint newWeaponID;

	public uint superNewWeaponID;

	// 变技能.
	public string skillTransform;

	// 特效绑点.
	public string _3DEffectBindpoint;
	// 特效ID.
	public uint _3DEffectID;
	// 特效是否循环.
	public bool loop3DEffect;

	// buff结束特效帮点及ID.
	public string endEffectBindpoint;
	public uint endEffectID;

	// 动作.
	public string animationName;
	// 动作是否循环.
	public bool loopAnimation;

	/// <summary>
	/// 该buff是否为控制BUFF.
	/// </summary>
	public bool IsStunBuff
	{
		get {
			return (disableAttack || disableMovement || disableRotate || disableSkillUse);
		}
	}
}
