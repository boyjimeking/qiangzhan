/// <summary>
/// 子弹参数.
/// </summary>
public class BulletTableItem
{
	public int resID;
	public string desc;

    public BulletType type;

	// 特效ID.
	public uint bulletFigureID;

	// 目标位置的预警特效.
	public uint effectOnTargetPosition;

	// 飞行速度.
	public float flySpeed;

    // 加速度.
    public float accelerateSpeed;

	// 加速度延迟.
	public uint accelerateDelay;

	// 射程.
	public uint flyRange;

	// 命中多少人后消失.
	public uint flyHitCount;

	// 碰撞时的阵营识别.
	public LeagueSelection leagueSelection;

	// 飞行时的碰撞半径(仅对子弹两侧的单位有效).
	public float radiusOnCollide;

	// 对被命中者产生的效果(skilleffect.txt).
	public uint skilleffectOnFlyCollide;

	// 爆炸时在场景内播放爆炸特效.
	public uint threeDEffectOnArrive;

	// 在爆炸位置进行目标选择(targetselection.txt).
	public uint targetSelectionOnArrive;
	
	// 对上述目标产生的效果.
	public uint skilleffectOnArrive;

	// 爆炸时创建召唤物.
	public uint creationOnArrive;

	// 追踪弹的追踪延迟.
    public float bulletFlyParam_0;

	// 追踪持续时间.
	public float bulletFlyParam_1;
}
