using UnityEngine;
using System.Collections;

public class ShopUIEvent : EventBase
{
    public static string SHOP_BUY_ITEM = "SHOP_BUY_ITEM";
    public static string SHOP_REFRESH_ITEM = "SHOP_REFRESH_ITEM";

    public int resId;  //商店物品表id;

	public ShopUIEvent(string eventName) :base(eventName)
	{
		
	}
	
}
