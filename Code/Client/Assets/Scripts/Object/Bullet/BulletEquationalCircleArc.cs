/// <summary>
/// ÑØÔ²»¡·ÉÐÐµÄ×Óµ¯.
/// </summary>
public class BulletEquationalCircleArc : BulletEquational
{
	public class Allocator : BulletAllocator
	{
		public Bullet Allocate()
		{
			return new BulletEquationalCircleArc();
		}
	}

	protected override BulletPathEquation initBulletPathEquation()
	{
		return new BulletPathEquationCircularArc(mBulletResource.bulletFlyParam_0, !Utility.isZero(mBulletResource.bulletFlyParam_1));
	}
}
