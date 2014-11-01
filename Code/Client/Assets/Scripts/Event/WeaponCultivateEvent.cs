using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class WeaponCultivateEvent : EventBase
{
	// 强化
	public static string STRENGTH_CHANGE = "STRENGTH_CHANGE";

    // 进阶
	public static string PROMOTE_CHANGE = "PROMOTE_CHANGE";

    // 配件
	public static string FITTING_CHANGE = "FITTING_CHANGE";

    //标签变化
    public static string TAB_INDEX = "TAB_INDEX";

	public uint weaponId = 0;
    public uint fittingPos = 0;

    public WeaponCultivateEvent(string eventName)
        : base(eventName)
    {

    }
}
