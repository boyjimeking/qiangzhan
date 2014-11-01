using UnityEngine;
using System.Collections;

public class SkillUIEvent : EventBase
{
	//刷新所有装备槽技能图标;
	public static string SKILL_SLOT_CHANGE = "SKILL_SLOT_CHANGE";

	//刷新所有技能列表的技能图标;
	public static string SKILL_LIST_CHANGE = "SKILL_LIST_CHANGE";

    //技能解锁和升级;
    public static string SKILL_LEVEL_UP = "SKILL_LEVEL_UP";

    //装备技能;
    public static string SKILL_EQUIP = "SKILL_EQUIP";

    //武器技能更新
    public static string SKILL_WEAPON_SKILL = "SKILL_WEAPON_SKILL";

	//单个技能详细信息展示刷新;
//	public static string SELECT_SKILL = "SELECT_SKILL";

	//刷新装备的技能列表信息展示;
//	public static string EQUIP_SKILL = "EQUIP_SKILL";

	public ArrayList msg = null;

    public int skillId;
    public int skillLv;

	public SkillUIEvent(string eventName) :base(eventName)
	{
		
	}
}