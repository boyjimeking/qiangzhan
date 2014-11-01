/// <summary>
/// 技能前端表现.
/// </summary>
public class SkillClientBehaviourItem
{
	public int resID;

	public string desc;

	// 使用者的动作.
	public string userAnimationName;

	// 动作是否循环.
	public bool loopUserAnimation;

	// 武器动作.
	public string weaponAnimationName;
	// 武器动作是否循环.
	public bool loopWeaponAnimation;
	
	// 声音.
	public string soundResID;
	// 声音是否循环.
	public bool loopSound;

	public float cameraDist2Player;
	public uint cameraMoveTime;

	// 特效绑点.
	public string effectBp_0;
	// 特效ID.
	public uint effectID_0;
	// 特效是否循环.
	public bool loopEffect_0;
	// 特效起始时间.
	public uint effectStartTime_0;

	// 特效绑点.
	public string effectBp_1;
	// 特效ID.
	public uint effectID_1;
	// 特效是否循环.
	public bool loopEffect_1;
	// 特效起始时间.
	public uint effectStartTime_1;
}
