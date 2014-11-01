public class SkillCreationItem
{
	public CreationType creationType;
	public uint creatureID;

	public uint lifeTime;

	public float spDistOffset;
	public float spAngleOffset;
}

public class SkillCreationTableItem
{
	public static readonly uint CreationCount = 3;

	public int resID;
	public string desc;

	public SkillCreationItem[] items = new SkillCreationItem[CreationCount];
}
