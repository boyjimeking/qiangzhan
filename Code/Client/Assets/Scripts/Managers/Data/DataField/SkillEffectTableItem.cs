public class SkillEffectItem
{
	// 效果类型.
	public SkillEffectType effectType;
	// 效果ID.
	public uint effectID;
}

public class SkillEffectTableItem
{
	// 效果个数.
	public static readonly uint SkillEffectCount = 3;

	public int resID;
	public string desc;

	public SkillEffectItem[] items = new SkillEffectItem[SkillEffectCount];
}
