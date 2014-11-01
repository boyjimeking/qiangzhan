/// <summary>
/// ���������߷��е��ӵ�.
/// </summary>
public class BulletEquationalSineWave : BulletEquational
{
	public class Allocator : BulletAllocator
	{
		public Bullet Allocate()
		{
			return new BulletEquationalSineWave();
		}
	}

	protected override BulletPathEquation initBulletPathEquation()
	{
		return new BulletPathEquationSineWave(mBulletResource.bulletFlyParam_0, mBulletResource.bulletFlyParam_1);
	}
}
