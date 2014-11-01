public class MaterialItem
{
	// 被击特效.
	public uint hitEffect;
	// 被击声音.
	public uint sound;
	// 死亡特效.
	public uint deathEffect;
	// 启用材质.
	public bool mtl;
	// 覆盖特有的尸体存在时间.
	public float deatdelay;
}
public class MaterialTableItem
{
	public int resID;
	public string desc;

	// 个数与ImpactDamageType.Count一致.
	public MaterialItem[] items = new MaterialItem[(uint)ImpactDamageType.Count];

	public string hitEffectBindpoint;
	public string deathEffectBindpoint;
    public uint deathEffectWaitTime;
}
