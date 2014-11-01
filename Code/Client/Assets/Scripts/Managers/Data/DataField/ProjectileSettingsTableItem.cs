public class ProjectileItem
{
	// 初始位置, 即投掷者的绑点名.
	public string initBindpoint;

	public uint bindpointEffect;

	// 子弹资源ID.
	public uint bulletResID;

	public BulletDistributionType distributionType;

	// 子弹分布(bulletdistribution.txt).
	public string distributionArgument;
}

/// <summary>
/// 投掷物的配置表.
/// </summary>
public class ProjectileSettingsTableItem
{
	public static readonly uint ProjectileCount = 3;

	public int resID;
	public string desc;

	public ProjectileItem[] items = new ProjectileItem[ProjectileCount];
}
