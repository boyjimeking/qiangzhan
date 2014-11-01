using System;
using System.Collections.Generic;

 class MallUIEvent : EventBase
 {
     //刷新所有装备槽技能图标;
     public static string MALL_BUY_ITEM = "MALL_BUY_ITEM";

    public int resId;  //商城物品表id;
    public int subId;  //子物品id[0-5]，最多6个;

	public MallUIEvent(string eventName) :base(eventName)
	{
		
	}
 }