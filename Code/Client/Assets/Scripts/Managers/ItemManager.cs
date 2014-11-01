using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class ItemManager
{
    public static ItemTableItem GetItemRes(int resId)
    {
        switch(GetItemType((uint)resId))
        {
            case ItemType.Defence:
                {
                    if (!DataManager.DefenceTable.ContainsKey(resId))
                        return null;

                    return DataManager.DefenceTable[resId] as ItemTableItem;
                }
            case ItemType.Normal:
                {
                    if (!DataManager.NormalItemTable.ContainsKey(resId))
                        return null;

                    return DataManager.NormalItemTable[resId] as ItemTableItem;
                }
            case ItemType.Weapon:
                {
                    if (!DataManager.WeaponTable.ContainsKey(resId))
                        return null;

                    return DataManager.WeaponTable[resId] as ItemTableItem;
                }
            case ItemType.Stone:
                {
                    if (!DataManager.StoneTable.ContainsKey(resId))
                        return null;

                    return DataManager.StoneTable[resId] as ItemTableItem;
                }
            case ItemType.Money:
                {
                    if (!DataManager.MoneyItemTable.ContainsKey(resId))
                        return null;

                    return DataManager.MoneyItemTable[resId] as ItemTableItem;
                }
            case ItemType.Crops:
                {
                    if (!DataManager.CropsTable.ContainsKey(resId))
                        return null;

                    return DataManager.CropsTable[resId] as CropsTableItem;
                }
            case ItemType.Box:
                {
                    if (!DataManager.BoxItemTable.ContainsKey(resId))
                        return null;

                    return DataManager.BoxItemTable[resId] as BoxItemTableItem;
                }
            default:
                return null;
        }
    }

    public static ItemType GetItemType(uint resId)
    {
        if (resId <= (uint)ItemTypeIdRangle.Item_Weapon_Res_Id_Max)
            return ItemType.Weapon;

        if (resId <= (uint)ItemTypeIdRangle.Item_Normal_Id_Max)
            return ItemType.Normal;

        if (resId <= (uint)ItemTypeIdRangle.Item_Defence_Id_Max)
            return ItemType.Defence;

        if (resId <= (uint)ItemTypeIdRangle.Item_Stone_Id_Max)
            return ItemType.Stone;

        if (resId <= (uint)ItemTypeIdRangle.Item_Money_Id_Max)
            return ItemType.Money;

        if (resId <= (uint)ItemTypeIdRangle.Item_Crops_Id_Max)
            return ItemType.Crops;

        if (resId <= (uint)ItemTypeIdRangle.Item_Box_Id_Max)
            return ItemType.Box;

        return ItemType.Invalid;
    }

    static string ItemTypeToString(ItemType type)
    {
        switch (type)
        {
            case ItemType.Defence:
                return "Defence";
            case ItemType.Normal:
                return "Normal";
            case ItemType.Weapon:
                return "Weapon";
            case ItemType.Stone:
                return "Stone";
            case ItemType.Money:
                return "Money";
            case ItemType.Crops:
                return "Crops";
            case ItemType.Box:
                return "Box";
            case ItemType.Invalid:
                return "Invalid";
            default:
                return "NONE";
        }
    }

    public static String GetItemTypeStr(int resId)
    {
        //ItemType type = GetItemType(resId);

        //return ItemTypeToString(type);
        ItemTableItem item = GetItemRes(resId);

        if(item == null)
            return "";

        return item.desc1;
    }

    //private Dictionary<ulong, ItemObj> mAllItem = new Dictionary<ulong, ItemObj>();

    private static ItemManager instance;
    public ItemManager()
    {
        instance = this;
    }

    public static ItemManager Instance
    {
        get
        {
            return instance;
        }
    }

    public ItemObj CreateItem(Message.item_info createInfo)
    {
        if (createInfo == null)
            return null;

        if (createInfo.baseinfo == null)
            return null;

        switch(createInfo.itemtype)
        {
            case (int)ItemType.Defence:
                {
                    GUID itemguid = createInfo.baseinfo.itemguid;
                    DefenceObjInit paramInit = new DefenceObjInit();
                    paramInit.mCount = (ushort)createInfo.baseinfo.count;
                    paramInit.mCreateTime = createInfo.baseinfo.createtime;
                    paramInit.mItemGuid = itemguid.ToULong();
                    paramInit.mResId = createInfo.baseinfo.resid;
                    paramInit.mStarslevel = createInfo.equip_info.starslevel;
                    paramInit.mStrenlevel = createInfo.equip_info.strenlevel;
                    
                    for (int i = 0; i < createInfo.equip_info.stoneinlays.Count; ++i)
                    {
                        paramInit.stoneinfo.Add(createInfo.equip_info.stoneinlays[i]);
                    }
                    return CreateItem(paramInit);
                }
            case (int)ItemType.Normal:
                {
                    GUID itemguid = createInfo.baseinfo.itemguid;

                    NormalObjInit paramInit = new NormalObjInit();
                    paramInit.mCount = (ushort)createInfo.baseinfo.count;
                    paramInit.mCreateTime = createInfo.baseinfo.createtime;
                    paramInit.mItemGuid = itemguid.ToULong();
                    paramInit.mResId = createInfo.baseinfo.resid;

                    return CreateItem(paramInit);
                }
            case (int)ItemType.Weapon:
                {
                    GUID itemguid = createInfo.baseinfo.itemguid;

                    WeaponObjInit paramInit = new WeaponObjInit();
                    paramInit.mCount = (ushort)createInfo.baseinfo.count;
                    paramInit.mCreateTime = createInfo.baseinfo.createtime;
                    paramInit.mItemGuid = itemguid.ToULong();
                    paramInit.mResId = createInfo.baseinfo.resid;
                    paramInit.mPromoteLv = (ushort)createInfo.weapon_info.promotelv;

                    return CreateItem(paramInit);
                }
            case (int)ItemType.Stone:
                {
                    GUID itemguid = createInfo.baseinfo.itemguid;

                    StoneObjInit paramInit = new StoneObjInit();
                    paramInit.mCount = (ushort)createInfo.baseinfo.count;
                    paramInit.mCreateTime = createInfo.baseinfo.createtime;
                    paramInit.mItemGuid = itemguid.ToULong();
                    paramInit.mResId = createInfo.baseinfo.resid;
                    return CreateItem(paramInit);
                }
            case (int)ItemType.Box:
                {
                    GUID itemguid = createInfo.baseinfo.itemguid;

                    BoxItemObjInit paramInit = new BoxItemObjInit();
                    paramInit.mCount = (ushort)createInfo.baseinfo.count;
                    paramInit.mCreateTime = createInfo.baseinfo.createtime;
                    paramInit.mItemGuid = itemguid.ToULong();
                    paramInit.mResId = createInfo.baseinfo.resid;
                    return CreateItem(paramInit);
                }
            default:
                return null;
        }
    }

    public ItemObj CreateItem(ItemObjInit initData)
    {
        if(initData == null)
            return null;

        ItemObj itemobj = null;
        switch (GetItemType((uint)initData.mResId))
        {
            case ItemType.Normal:
                {
                    itemobj = new NormalItemObj();
                }
                break;
            case ItemType.Defence:
                {
                    itemobj = new DefenceObj();
                }
                break;
            case ItemType.Weapon:
                {
                    itemobj = new WeaponObj();
                }
                break;
            case ItemType.Stone:
                {
                    itemobj = new StoneObj();
                }
                break;
            case ItemType.Box:
                {
                    itemobj = new BoxItemObj();
                }
                break;
            default:
                break;
        }

        if (itemobj == null)
            return null;

        if (!itemobj.Init(initData))
            return null;

        return itemobj;
    }

    public String getItemName(int resId)
    {
        ItemTableItem itemRes = ItemManager.GetItemRes(resId);
        if (itemRes == null)
            return null;

        return itemRes.name;
    }

    public static string getItemNameWithColor(int resId)
    {
        ItemTableItem itemRes = ItemManager.GetItemRes(resId);
        if (itemRes == null)
            return null;

        if(!DataManager.ConfigTable.ContainsKey(itemRes.quality))
            return itemRes.name;

        ConfigTableItem item = DataManager.ConfigTable[itemRes.quality] as ConfigTableItem;
        if (item == null)
            return itemRes.name;
        
        return "[" + item.value + "]" + itemRes.name + "[-]";
    }

    public String getItemBmp(int resId)
    {
        ItemTableItem itemRes = ItemManager.GetItemRes(resId);
        if (itemRes == null)
            return null;

        return itemRes.picname;
    }
    public ArrayList getItemDesc(int resId)
    {
        ItemTableItem itemRes = ItemManager.GetItemRes(resId);
        if (itemRes == null)
            return null;

        ArrayList arr = new ArrayList();
        if (itemRes.desc0 != null && itemRes.desc0 != "")
            arr.Add(itemRes.desc0);
        if (itemRes.desc1 != null && itemRes.desc1 != "")
            arr.Add(itemRes.desc1);
        if (itemRes.desc2 != null && itemRes.desc2 != "")
            arr.Add(itemRes.desc2);
        if (itemRes.desc3 != null && itemRes.desc3 != "")
            arr.Add(itemRes.desc3);
        if (itemRes.desc4 != null && itemRes.desc4 != "")
            arr.Add(itemRes.desc4);

        return arr;
    }

    public DefenceTableItem getDefenceTable(int resId)
    {
        return DataManager.DefenceTable[resId] as DefenceTableItem;
    }

    public WeaponTableItem getWeaponTable(int resId)
    {
        return DataManager.WeaponTable[resId] as WeaponTableItem;
    }

    public NormalItemTableItem getNormalItemTable(int resId)
    {
        return DataManager.NormalItemTable[resId] as NormalItemTableItem;
    }

    public CropsTableItem getCropsItemTable(int resId)
    {
        return DataManager.CropsTable[resId] as CropsTableItem;
    }

    //模拟服务器创建道具
    //public ItemObjInit UnrealCreateItem(int resId)
    //{
    //    ItemTableItem restab = ItemManager.GetItemRes(resId);
    //    if (restab == null)
    //    {
    //        GameDebug.LogError("资源ID为" + resId + "的道具不存在表格中 ");
    //        return null;
    //    }

 
    //    switch (ItemManager.GetItemType((uint)resId))
    //    {
    //        case ItemType.Normal:
    //            {
    //                NormalObjInit initParam = new NormalObjInit();
    //                initParam.mResId = resId;
    //                initParam.mItemGuid = (UInt64)UnityEngine.Random.Range(0, 100000);
    //                return initParam;
    //            }
    //        case ItemType.Defence:
    //            {
    //                DefenceObjInit initParam = new DefenceObjInit();
    //                initParam.mResId = resId;
    //                initParam.mItemGuid = (UInt64)UnityEngine.Random.Range(0, 100000);
    //                return initParam;
    //            }
    //        case ItemType.Weapon:
    //            {
    //                WeaponObjInit initParam = new WeaponObjInit();
    //                initParam.mResId = resId;
    //                initParam.mPromoteLv = 0;
    //                initParam.mItemGuid = (UInt64)UnityEngine.Random.Range(0, 100000);
    //                return initParam;
    //            }
    //        default:
    //            return null;
    //    }
    //}

}