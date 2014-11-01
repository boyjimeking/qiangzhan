public class SkillImpactTableItem
{
	public int resID;
	public string desc;

	public bool harmful;

	// 伤害类型.
	public ImpactDamageType impactDamageType;

	// 血量不高于多少百分比时, 直接死亡(100表示直接杀死).
	public uint seckill;

	// 附加打击点距离发起点的距离 * damageAddPerMile的伤害.
	public uint damageAddPerMile;

	// 取当前伤害属性的百分比.
	public float damageAttrPercent;

	// 加血固定值, 负数表示减少.
	public int hpRegenerated;

	// 加血百分比, +-总血量的该百分比数值.
	public float hpPercentByTotal;

	// 加血百分比, +-当前血量的该百分比数值.
	public float hpPercentByCurrent;

	// 加蓝固定值, 负数表示减少.
	public int manaRegenerated;

	// 特效绑点.
	public string _3DEffectBindpoint;
	public uint _3DEffectID;

	// 震屏时间.
	public uint shakeCameraDuration;

	// 震屏幅度.
	public float shapeCameraAmount;

    // 震屏方式.
    public int shapeCameraType;
}
