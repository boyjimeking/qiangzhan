using UnityEngine;
using System.Collections;
using System;

public class WeaponObjInit : ItemObjInit
{
    public ushort mPromoteLv;
};

public class WeaponObj : ItemObj
{
    private const int STREN_STEP = 10;
    private const int PICINITNUM = 7;
    public static bool IsItemType(int type)
    {
        return type == (int)ItemType.Weapon;
    }

    private ushort mPromoteLv;

    public WeaponObj()
    {
        mPromoteLv = 0;
    }

    override public ItemType GetType()
    {
        return ItemType.Weapon;
    }

    //服务器数据信息,临时数据
    override public bool Init(ItemObjInit initData)
    {
        if (!base.Init(initData))
            return false;

        WeaponObjInit weaponInit = initData as WeaponObjInit;
        if (weaponInit == null)
            return false;

        mPromoteLv = weaponInit.mPromoteLv;
        return true;
    }

    public ushort GetPromoteLv()
    {
        return mPromoteLv;
    }

    public ushort SetPromoteLv(ushort lv)
    {
        mPromoteLv = lv;
        return mPromoteLv;
    }

    public WeaponTableItem GetWeaponRes()
    {
        return DataManager.WeaponTable[mResId] as WeaponTableItem;
    }

    public int getProtemResId()
    {
        return (int)(mPromoteLv + GetWeaponRes().upgrade);
    }

    public PromoteTableItem GetPromoteRes()
    {
        return DataManager.PromoteTable[getProtemResId()] as PromoteTableItem;
    }

    public override ItemTableItem GetRes()
    {
        return GetWeaponRes();
    }

    public uint GetWeaponLv()
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return 0;

        //武器强化等级不是强化在武器上。。。。。
        return module.GetStrenLv();
    }

    public string GetWeaponLvPic()
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return "";

        uint lv = module.GetStrenLv();
        if (lv < 1)
            return "";

        lv = (lv - 1) / STREN_STEP + PICINITNUM;
        //先这样写着
        
        return "common:strenth (" + lv + ")";
    }

    public string GetWeaponGradePic()
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return "";

        uint lv = module.GetStrenLv();
        int starlv = (int)(lv / STREN_STEP);
        if (starlv > 0 && (lv % STREN_STEP) == 0)
            starlv -= 1;

        if (starlv == 0)
            return null;

        return "common:weaponlv" + starlv;
    }

    public static string GetWeaponGradePicS()
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return null;

        uint lv = module.GetStrenLv();
        int starlv = (int)(lv / STREN_STEP);
        if (starlv > 0 && (lv % STREN_STEP) == 0)
            starlv -= 1;

        if (starlv == 0)
            return null;
        return "common:weaponlv" + starlv;
    }

    override public bool BuildProperty(PropertyOperation operation)
    {
        PromoteTableItem item = GetPromoteRes();
        if (null != item)
            operation.AddPro((int)PropertyTypeEnum.PropertyTypeDamage, item.value);
        return true;
    }

    public uint GetFightValue()
    {
        uint max_value = 0;
        PromoteTableItem item = DataManager.PromoteTable[mPromoteLv] as PromoteTableItem;
        if (null != item)
            max_value += item.score;

        return max_value + (uint)GetWeaponRes().grade;
    }
}
