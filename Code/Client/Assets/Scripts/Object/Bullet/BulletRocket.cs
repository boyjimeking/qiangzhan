/// <summary>
/// 直线水平飞行, 正常爆炸.
/// </summary>
public class BulletRocket : BulletBasic
{
	public class Allocator : BulletAllocator
	{
		public Bullet Allocate()
		{
			return new BulletRocket();
		}
	}

	protected override BulletExplodeController initExplodeController()
	{
		return new BulletExplodeNormally(this);
	}
}
