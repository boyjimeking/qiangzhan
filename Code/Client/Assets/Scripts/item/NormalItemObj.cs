using System;
using UnityEngine;
using System.Collections;

public class NormalObjInit : ItemObjInit
{
};

public class NormalItemObj : ItemObj
{   
    public static bool IsItemType(int type)
    {
        return type == (int)ItemType.Normal;
    }

    public NormalItemObj()
    {
    }

    override public ItemType GetType()
    {
        return ItemType.Normal;
    }

    //服务器数据信息,临时数据
    override public bool Init(ItemObjInit initData)
    {
        if (!base.Init(initData))
            return false;

        NormalObjInit normalData = initData as NormalObjInit;
        if (normalData == null)
            return false;

        return true;
    }

    public NormalItemTableItem GetNorRes()
    {
        return DataManager.NormalItemTable[mResId] as NormalItemTableItem;
    }

    public override ItemTableItem GetRes()
    {
        return GetNorRes();
    }

}
