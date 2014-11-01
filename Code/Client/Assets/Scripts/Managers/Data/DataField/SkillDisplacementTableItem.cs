public class SkillDisplacementTableItem 
{
	public int resID;
	public string desc;

	// 是否有害.
	public bool harmful;

	// 类型.
	public SkillDisplacementType displacementType;

	// 速度/距离.
	public float speed;
	public float distance;

	// 是否打断当前技能.
	public bool interruptSkillUsing;

	// 冲撞时的动作名.
	public string animationName;
	// 动作是否循环.
	public bool loopAnimation;

	// 冲撞时的特效绑点/特效ID/是否循环.
	public string _3DEffectBindpointOnExecute;
	public uint _3DEffectIdOnExecute;
	public bool loop3DEffectOnExecute;

	// 冲撞时的阵营识别.
	public LeagueSelection leagueSelectionOnExecute;

	// 冲撞时的碰撞半径(仅对单位两侧的单位有效).
	public float radiusOnCollide;

	// 对选中的目标产生技能效果.
	public uint skillEffect2OthersOnExecute;

	// 停止时给自己添加技能效果.
	public uint skillEffect2SelfOnArrive;

	// 冲撞停止时, 进行目标选择, 并对之中目标添加效果.
	public uint targetSelectionOnArrive;
	public uint skillEffect2OthersOnArrive;

	// 冲撞停止时, 创建召唤物.
	public uint creationOnArrive;
}
