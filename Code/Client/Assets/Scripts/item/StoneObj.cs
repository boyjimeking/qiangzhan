using UnityEngine;
using System.Collections;
using System;

public class StoneObjInit : ItemObjInit
{
    
};

public class StoneObj : ItemObj
{   
    public static bool IsItemType(int type)
    {
        return type == (int)ItemType.Stone;
    }


    public StoneObj()
    {
        
    }

    override public ItemType GetType()
    {
        return ItemType.Stone;
    }

    //服务器数据信息,临时数据
    override public bool Init(ItemObjInit initData)
    {
        if (!base.Init(initData))
            return false;

        StoneObjInit stoneInit = initData as StoneObjInit;
        if (stoneInit == null)
            return false;

        return true;
    }


    public StoneTableItem GetStoneRes()
    {
        return DataManager.StoneTable[mResId] as StoneTableItem;
    }

    public override ItemTableItem GetRes()
    {
        return GetStoneRes();
    }
}
