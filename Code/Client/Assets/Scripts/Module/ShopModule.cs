using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum ShopOpType : int
{
    Refresh = 1,
    BuyItem = 2,
    FreeRefresh = 3,
}

public enum ShopSubTable : int
{
    None = -1,    //错误;
    Sceret = 0,       //神秘商店;
    Credit,       //积分兑换;
    Equip,        //装备购买;
}

public class ShopModule : ModuleBase
{
    public const int MAX_SHOP_SALE_COUNT = 8; //神秘商店一次随机买出物品的种数;

    // 神秘商店刷新消耗的货币类型以及货币值;
    public const ProceedsType SECRET_REFRESH_PROC_TYPE = ProceedsType.Money_RMB;
    public const int SECRET_REFRESH_COST = 25;

    public static ProceedsType GetSecretShopRefreshPoceType()
    {
        //string val = ConfigManager.GetValStr(ConfigItemKey.SECRET_SHOP_REFRESH_PROC_TYPE);

        //return (ProceedsType)System.Enum.Parse(typeof(ProceedsType), val);

        return (ProceedsType)ConfigManager.GetVal<int>(ConfigItemKey.SECRET_SHOP_REFRESH_PROC_TYPE);
    }

    public static int GetSecretShopRefreshCost()
    {
        //string val = ConfigManager.GetValStr(ConfigItemKey.SECRET_SHOP_REFRESH_PROC_TYPE);

        //return System.Convert.ToInt32(val);

        return (int)ConfigManager.GetVal<int>(ConfigItemKey.SECRET_SHOP_REFRESH_PROC_NUM);
    }

    /// <summary>
    /// 获取商店总共需要刷新几次;
    /// </summary>
    /// <returns></returns>
    public static int GetRefreshTimesCount()
    {
        List<int> res = GetRefreshTimes();

        if (res == null)
            return 0;

        else
            return res.Count;
    }

    /// <summary>
    /// 获取商店每天刷新的时间点;(存储格式为9|12|18)
    /// </summary>
    /// <returns></returns>
    public static List<int> GetRefreshTimes()
    {
        List<int> res = new List<int>();

        string content = ConfigManager.GetValStr(ConfigItemKey.SECRET_SHOP_REFRESH_TIME_BUCKET);
        string[] splits = content.Split('|');

        foreach (string str in splits)
        {
            if(string.IsNullOrEmpty(str))
                continue;

            int val = int.Parse(str);
            if (res.Contains(val))
                continue;

            res.Add(val);
        }

        //按照时间从小到大进行排序;
        res.Sort(delegate(int a, int b) { return a.CompareTo(b); });
        
        return res;
    }

    public static string GetBuyMoneyNotEnougthStr(ProceedsType pt)
    {
        switch (pt)
        {
            case ProceedsType.Money_Game:
                return StringHelper.GetString("buy_no_game");
            case ProceedsType.Money_RMB:
                return StringHelper.GetString("buy_no_rmb");
            case ProceedsType.Money_Prestige:
                return StringHelper.GetString("buy_no_prestige");
            case ProceedsType.Money_Stren:
                return StringHelper.GetString("buy_no_stren");
            case ProceedsType.Money_Arena:
                return StringHelper.GetString("buy_no_arena");
            default:
                GameDebug.LogError("缺少该类型的购买钱币不足提示");
                return "";
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    public BetterList<int> GetPlayerSecretShopItemIds()
    {
        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if(pdm == null)
            return null;
        
        BetterList<int> res = new BetterList<int>();
        
        foreach (int key in pdm.GetPlayerShopData().roleShopData.Keys)
        {
            res.Add(key);
        }

        return res;
    }

    /// <summary>
    /// 神秘商店数据是从服务器获取的;获得积分兑换、装备栏上架的商品;
    /// </summary>
    /// <returns></returns>
    public static BetterList<int> GetEffectShopIdsWithoutSecret()
    {
        BetterList<int> ids = new BetterList<int>();

        foreach(DictionaryEntry de in DataManager.ShopTable)
        {
            int id = (int)de.Key;
            
            if (id <= 0)
                continue;

            ShopTableItem item = DataManager.ShopTable[id] as ShopTableItem;
            if (item == null || item.subTable == (int)ShopSubTable.Sceret)
                continue;

            ids.Add(id);
        }

        return ids;
    }

    /// <summary>
    /// 获得所有上架的商品;
    /// </summary>
    /// <returns></returns>
    public static BetterList<ShopTableItem> GetEffectShopItemsWithoutSecret()
    {
        BetterList<ShopTableItem> items = new BetterList<ShopTableItem>();

        IDictionaryEnumerator itr = DataManager.ShopTable.GetEnumerator();
        while (itr.MoveNext())
        {
            ShopTableItem item = itr.Value as ShopTableItem;

            if (item == null || item.subTable == (int)ShopSubTable.Sceret)
                continue;

            items.Add(item);
        }
//         foreach (DictionaryEntry de in DataManager.ShopTable)
//         {
//             ShopTableItem item = de.Value as ShopTableItem;
// 
//             if (item == null || item.subTable == (int)ShopSubTable.Sceret)
//                 continue;
// 
//             items.Add(item);
//         }

        return items;
    }

    public static ShopSubTable GetShopSubTableById(int resId)
    {
        ShopSubTable result = ShopSubTable.None;

        if (!DataManager.ShopTable.ContainsKey(resId))
            return result;

        ShopTableItem item = DataManager.ShopTable[resId] as ShopTableItem;
        if (item == null)
            return result;

        return (ShopSubTable)item.subTable;
    }

    /// <summary>
    /// resId 商店表id;
    /// idx   [0-2] 货币类型1，单价1；货币类型2，单价2，货币类型3，单价3;
    /// </summary>
    /// <param name="resId"></param>
    /// <param name="idx"></param>
    /// <returns></returns>
    public static int GetShopItemProceedsType(int resId, int idx)
    {
        if (!DataManager.ShopTable.ContainsKey(resId))
        {
            return -1;
        }

        ShopTableItem item = DataManager.ShopTable[resId] as ShopTableItem;
        
        return GetShopItemProceedsType(item, idx);
    }

    public static int GetShopItemProceedsType(ShopTableItem item, int idx)
    {
        if (idx < 0 || idx > 2)
        {
            return -1;
        }

        if (item == null)
            return -1;

        return item[idx];
    }

    /// <summary>
    /// 获取商店表中货币类型索引idx对应的单价值;
    /// </summary>
    /// <param name="resId"></param>
    /// <param name="idx"></param>
    /// <returns></returns>
    public static int GetShopItemPerPrice(int resId, int idx)
    {
        if (!DataManager.ShopTable.ContainsKey(resId))
        {
            return -1;
        }

        ShopTableItem item = DataManager.ShopTable[resId] as ShopTableItem;

        return GetShopItemPerPrice(item , idx);
    }

    public static int GetShopItemPerPrice(ShopTableItem item, int idx)
    {
        if (idx < 0 || idx > 2)
        {
            return -1;
        }

        if (item == null)
            return -1;

        return item[idx + 3];
    }

    public void BuyShopItem(int resId)
    {
        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (pdm == null)
            return;

        pdm.SetShopIsBuyDone(resId, true);

        ShopUIEvent ev = new ShopUIEvent(ShopUIEvent.SHOP_BUY_ITEM);

        ev.resId = resId;

        EventSystem.Instance.PushEvent(ev);
    }

    public void RefreshShop()
    {
        ShopUIEvent ev = new ShopUIEvent(ShopUIEvent.SHOP_REFRESH_ITEM);

        EventSystem.Instance.PushEvent(ev);
    }

    public void OpenShopUI(ShopSubTable subTable)
    {
        WindowManager.Instance.OpenUI("shop", subTable, subTable);
    }
}
