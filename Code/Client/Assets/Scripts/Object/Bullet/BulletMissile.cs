/// <summary>
/// 追踪导弹.
/// </summary>
public class BulletMissile : Bullet
{
	public class Allocator : BulletAllocator
	{
		public Bullet Allocate()
		{
			return new BulletMissile();
		}
	}

	protected override BulletFlyController initFlyController()
	{
		return new BulletFlyTrackingTarget(this, 
			(uint)mBulletResource.bulletFlyParam_0,
			(uint)mBulletResource.bulletFlyParam_1
			);
	}

	protected override BulletExplodeController initExplodeController()
	{
		return new BulletExplodeNormally(this);
	}
}
