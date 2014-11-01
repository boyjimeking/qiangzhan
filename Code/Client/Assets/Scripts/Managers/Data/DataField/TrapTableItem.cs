public class TrapTableItem
{
	public int resID;
	public string desc;

	// 模型ID.
	public uint modelID;

	// 绑定的buff ID.
	public uint buffID;

	// 爆炸延迟.
	public uint explodeDelay;

	// 延迟特效ID(倒计时等特效).
	public uint delayEffect;
	// 延迟特效绑点.
	public string delayEffectBindpoint;
	// 延迟特效是否循环.
	public bool loopDelayEffect;

	// 检测敌人时使用碰撞半径.
	public uint collideRadius;

	// 地雷的触发范围或者爆炸范围.
	public uint targetSelectionOnExplode;

	// 爆炸时对目标产生效果.
	public uint skillEffectOnExplode;

	// 爆炸时在场景内播放的特效.
	public uint _3DEffectOnExplode;
}
