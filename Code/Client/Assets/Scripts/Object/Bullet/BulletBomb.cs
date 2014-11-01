using UnityEngine;
/// <summary>
/// 轰炸. 炮弹垂直下落.
/// </summary>
public class BulletBomb : BulletGrenade
{
	public class Allocator : BulletAllocator
	{
		public Bullet Allocate()
		{
			return new BulletBomb();
		}
	}

	float G = GameConfig.GravitationalAcceleration;
	protected override BulletFlyController initFlyController()
	{
		return new BulletFlyAlongVerticalLine(this);
	}

	protected override Quaternion DefaultRotation
	{
		get { return Quaternion.LookRotation(Vector3.down); }
	}

	/// <summary>
	/// 由于这种导弹属于垂直轰炸, 预警特效位置为起点正下方的地面位置.
	/// </summary>
	protected override Vector3 AlertEffectPosition
	{
		get
		{
			Vector3 position = StartPosition;
			position.y = Scene.GetHeight(position.x, position.z) + 0.05f;
			return position;
		}
	}
}
