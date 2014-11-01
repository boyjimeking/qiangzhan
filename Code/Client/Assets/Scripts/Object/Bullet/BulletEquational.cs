/// <summary>
/// 通过方程控制轨迹的子弹(只可以在X, Z平面上飞行).
/// </summary>
public abstract class BulletEquational : Bullet
{
	protected override BulletFlyController initFlyController()
	{
		return new BulletFlyAlongEquationGraph(this, initBulletPathEquation());
	}

	protected override BulletExplodeController initExplodeController()
	{
		return new BulletExplodeNormally(this);
	}

	/// <summary>
	/// 初始化轨迹控制方程.
	/// </summary>
	/// <returns></returns>
	protected abstract BulletPathEquation initBulletPathEquation();
}
