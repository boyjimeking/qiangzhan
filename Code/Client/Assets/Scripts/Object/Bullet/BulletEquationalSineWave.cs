/// <summary>
/// 沿正弦曲线飞行的子弹.
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
