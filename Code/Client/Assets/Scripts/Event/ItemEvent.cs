using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class ItemEvent : EventBase
{
    // 删除
    public static string DELETE_CHANGE = "DELETE_CHANGE";

    // 更新
    public static string UPDATE_CHANGE = "UPDATE_CHANGE";

    //更换武器
    public static string WEAPON_CHANGE = "WEAPON_CHANGE";

    public static string WEAPON_PROMOTE = "WEAPON_PROMOTE";

    //防具强化
    public static string DEFENCE_STREN = "DEFENCE_STREN";

    //防具升星
    public static string DEFENCE_RISING_STARS = "DEFENCE_RISING_STARS";

    //防具升阶
    public static string DEFENCE_PROMOTE = "DEFENCE_PROMOTE";

    //宝石合成
    public static string STONE_COMB = "STONE_COMB";

    //宝石升级
    public static string STONE_RISE = "STONE_RISE";

    //宝石镶嵌
    public static string STONE_INLAY = "STONE_INLAY";

    //宝石摘除
    public static string STONE_UNINLAY = "STONE_UNINLAY";
    
    //装备卖出
    public static string DEFENCE_SALE = "DEFENCE_SALE";

    //道具全部卖出
    public static string ITEM_SALE_ALL = "ITEM_SALE_ALL";

    //道具部分卖出
    public static string ITEM_SALE_PART = "ITEM_SALE_PART";

    public static string BAG_OP_UNLOCK = "BAG_OP_UNLOCK";

    public bool isSubWeapon = false;

    public ItemType type = ItemType.Normal;
    public PackageType bagType = PackageType.Invalid;
	public int itemId = -1;
    public int pos = -1;

    public ItemEvent(string eventName)
        : base(eventName)
    {

    }
}
