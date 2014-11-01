/// <summary>
/// 手雷等投掷物, 抛物线飞行.
/// </summary>
public class BulletGrenade : Bullet
{
	public class Allocator : BulletAllocator
	{
		public Bullet Allocate()
		{
			return new BulletGrenade();
		}
	}

    /// <summary>
	/// 重力加速度.
    /// </summary>
	private static readonly float G = GameConfig.GravitationalAcceleration;

	protected override BulletFlyController initFlyController()
	{
		return new BulletFlyAlongParabola(this, G);
	}

	protected override BulletExplodeController initExplodeController()
	{
		return new BulletExplodeNormally(this);
	}

	protected override bool CheckFlyHit
	{
		get { return false; }
	}

	protected override bool CheckLowBlockOnFlying
	{
		get { return false; }
	}
}
