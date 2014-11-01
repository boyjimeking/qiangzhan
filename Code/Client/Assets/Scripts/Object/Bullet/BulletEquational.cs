/// <summary>
/// ͨ�����̿��ƹ켣���ӵ�(ֻ������X, Zƽ���Ϸ���).
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
	/// ��ʼ���켣���Ʒ���.
	/// </summary>
	/// <returns></returns>
	protected abstract BulletPathEquation initBulletPathEquation();
}
