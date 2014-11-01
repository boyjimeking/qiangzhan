/// <summary>
/// 直线水平飞行, 碰撞模拟爆炸, 且只在没有命中足够的敌人时播放爆炸特效.
/// </summary>
public class BulletBasic : Bullet
{
	public class Allocator : BulletAllocator
	{
		public Bullet Allocate()
		{
			return new BulletBasic();
		}
	}

	protected override BulletFlyController initFlyController()
	{
		return new BulletFlyAlongHorizontalLine(this);
	}

	protected override BulletExplodeController initExplodeController()
	{
		return new BulletExplodeByCollision(this);
	}
}
