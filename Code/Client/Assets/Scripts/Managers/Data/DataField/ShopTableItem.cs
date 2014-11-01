using UnityEngine;
using System.Collections;

public class ShopTableItem
{
    public int resId;
    public int itemId;
    public int isOnSale;        //是否上架;
    public uint subTable;        //所属的分栏;
    public uint minCount;        //数量下限;
    public uint maxCount;        //数量上限;
    public uint multiple;        //倍数;

    public int proceedType0;    //货币类型;
    public int perPrice0;       //对应货币类型的单价;

    public int proceedType1;
    public int perPrice1;
    
    public int proceedType2;
    public int perPrice2;

    public string detail;
    
    public int this[int index]
    {
        get
        {
            switch (index)
            {
                case 0:
                    return proceedType0;
                case 1:
                    return proceedType1;
                case 2:
                    return proceedType2;
                case 3:
                    return perPrice0;
                case 4:
                    return perPrice1;
                case 5:
                    return perPrice2;
                default:
                    return -1;
            }
        }
    }
}