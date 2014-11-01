using UnityEngine;
using System.Collections;
using System;

public class BoxItemObjInit : ItemObjInit
{
    
};

public class BoxItemObj : ItemObj
{
    public static bool IsItemType(int type)
    {
        return type == (int)ItemType.Box;
    }

    private ushort mPromoteLv;

    public BoxItemObj()
    {
        
    }

    override public ItemType GetType()
    {
        return ItemType.Box;
    }

    //服务器数据信息,临时数据
    override public bool Init(ItemObjInit initData)
    {
        if (!base.Init(initData))
            return false;

        BoxItemObjInit boxInit = initData as BoxItemObjInit;
        if (boxInit == null)
            return false;

        return true;
    }


    public BoxItemTableItem GetBoxRes()
    {
        return DataManager.BoxItemTable[mResId] as BoxItemTableItem;
    }

    public override ItemTableItem GetRes()
    {
        return GetBoxRes();
    }

}
