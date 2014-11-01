using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class PropertyEvent : EventBase
{
	// 主属性变动
	public static string MAIN_PROPERTY_CHANGE = "MAIN_PROPERTY_CHANGE";

	// 战斗属性变动
	public static string FIGHT_PROPERTY_CHANGE = "FIGHT_PROPERTY_CHANGE";

	// Ghost属性变动
	public static string GHOST_FIGHT_PROPERTY_CHANGE = "GHOST_FIGHT_PROPERTY_CHANGE";

    // 佣兵属性变动
    public static string CROPS_PROPERTY_CHANGE = "CROPS_PROPERTY_CHANGE";

    //当前Player属性改变(PlayerPropertyModule)

    public static string PLAYER_DATA_PROPERTY_CHANGED = "PLAYER_DATA_PROPERTY_CHANGED";

	// 属性ID
	public int propertyId = -1;

	// 旧值
	public int oldValue = -1;

	// 新值
	public int newValue = -1;

	public PropertyEvent(string eventName)
        : base(eventName)
    {

    }
}
