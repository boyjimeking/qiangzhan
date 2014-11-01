using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MallLimitType : int
{
    ERROR = -1,
    NONE = 0,  //不限购;
    DAY,       //每天限购;
    FOREVER,   //每个账号限购;
};

//商城表分栏;
public enum MallSubTableType : int
{
    None = -1,
    HotSale = 0,
    LimitCount,
}

public class MallFormModule : ModuleBase
{
    public const int MAX_SUB_ID = 5; // 子物品所在的idx[0-5];

    // <resId , times>
    private Dictionary<int, int> buyTimes = new Dictionary<int, int>();

    private PlayerDataModule mMoudle = ModuleManager.Instance.FindModule<PlayerDataModule>();

    private Hashtable mMallTable = null;

    public Hashtable MallTable
    {
        get 
        {
            if (mMallTable == null)
            {
                mMallTable = new Hashtable();
                IDictionaryEnumerator itr = DataManager.MallTable.GetEnumerator();
                while (itr.MoveNext())
                {
                    MallTableItemBase item = itr.Value as MallTableItemBase;

                    if (item == null) continue;

                    //MallTableItem mallItem = new MallTableItem();
                    //mallItem = item;
                    //mallTableItem.Add(mallItem.resId, mallItem);
                    mMallTable.Add(item.resId, (MallTableItem)item);
                }
//                 Hashtable data = DataManager.MallTable;
//                 foreach (int key in data.Keys)
//                 {
//                     MallTableItemBase item = data[key] as MallTableItemBase;
// 
//                     if (item == null) continue;
// 
//                     //MallTableItem mallItem = new MallTableItem();
//                     //mallItem = item;
//                     //mallTableItem.Add(mallItem.resId, mallItem);
//                     mMallTable.Add(item.resId, (MallTableItem)item);
//                 }

            }
            return mMallTable;
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

    public int GetPlayerBuyTimes(int resId)
    {
        if (mMoudle == null)
            return -1;

        return mMoudle.GetMallBuyTimesByID(resId);
    }

    public MallLimitType GetLimitTypeByID(int resId)
    {
        if (!MallTable.ContainsKey(resId))
            return MallLimitType.ERROR;

        MallTableItem item = MallTable[resId] as MallTableItem;
        if (item == null)
            return MallLimitType.ERROR;

        return (MallLimitType)item.limitType;
    }

    public BetterList<int> GetEffectMallItemIDs()
    {
        BetterList<MallTableItem> temp = GetEffectMallItems();

        if (temp == null)
            return null;

        BetterList<int> res = new BetterList<int>();

        foreach (MallTableItem item in temp)
        {
            if (res.Contains(item.resId))
                continue;

            res.Add(item.resId);
        }

        return res;
    }

    public BetterList<MallTableItem> GetEffectMallItems()
    {
        BetterList<MallTableItem> list = new BetterList<MallTableItem>();

        //foreach (MallTableItem item in MallTable)
        foreach(int key in MallTable.Keys)
        {
            MallTableItem item = MallTable[key] as MallTableItem;
            if (item == null)
                continue;

            if (item.saleType == 1)
                list.Add(item);
        }

        return list;
    }

    public BetterList<MallTableItem> GetEffectMallItemsByType(MallSubTableType type)
    {
        BetterList<MallTableItem> list = new BetterList<MallTableItem>();
        BetterList<MallTableItem> temp = GetEffectMallItems();

        foreach (MallTableItem item in temp)
        {
            if (item == null)
                continue;

            if (item.subField == (int)type)
                list.Add(item);
        }

        return list;
    }

    /// <summary>
    /// 获取每个商品打包的物品个数，如果result=1，表示没打包;
    /// </summary>
    /// <param name="resId"></param>
    /// <returns></returns>
    public int GetItemNumByMallId(int resId)
    {
        if(!MallTable.ContainsKey(resId))
            return 0;

        MallTableItem item = MallTable[resId] as MallTableItem;
        if (item == null)
            return 0;

        int result = 0;
        foreach (MallItemInfo info in item.mallItems)
        {
            if (info.count > 0)
            {
                result++;
            }
        }

        return result;
    }

    public int GetItemIdByMallId(int resId)
    {
        if (!MallTable.ContainsKey(resId))
            return -1;

        MallTableItem item = MallTable[resId] as MallTableItem;
        if (item == null)
            return -1;

        return item.itemId;
    }

    public void BuyMallItem(int resId , int subId)
    {
        if (!MallTable.ContainsKey(resId))
            return;

        if (subId > MAX_SUB_ID || subId < 0) return;

        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        module.AddMallBuyTimes(resId);

        MallUIEvent ev = new MallUIEvent(MallUIEvent.MALL_BUY_ITEM);
        ev.resId = resId;
        ev.subId = (int)subId;
        EventSystem.Instance.PushEvent(ev);
    }

    public void OpenMallFormByItemId(int itemId, string returnUI = "")
    {
        int mallId = getMallIdByItemId(itemId);

        if (mallId < 0)
        {
            Debug.LogError("商城中不出售该物品，物品id为：" + itemId);
            return;
        }

        WindowManager.Instance.OpenUI("mall", mallId, null, returnUI);
    }

    int getMallIdByItemId(int itemId)
    {
        int res = -1;

        BetterList<MallTableItem> items = GetEffectMallItems();

        foreach (MallTableItem item in items)
        {
            if (item.itemId == itemId)
                res = item.resId;
        }

        return res;
    }
}
