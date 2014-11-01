/// <summary>
/// 从起始点直接飞向终点(含y轴).
/// </summary>
public class BulletCannonball : Bullet
{
	public class Allocator : BulletAllocator
	{
		public Bullet Allocate()
		{
			return new BulletCannonball();
		}
	}
	protected override BulletFlyController initFlyController()
	{
		return new BulletFlyAlongDiagonalLine(this);
	}

	protected override BulletExplodeController initExplodeController()
	{
		return new BulletExplodeNormally(this);
	}
}
