using UnityEngine;
using System.Collections;

public class TotalChargeTableItem
{
    public int id;
    public string detail;
    //充值钻石数;
    public int chargeNum;
    //礼包价值;
    public int giftPrice;
    
    public int itemid1;
    public int itemNum1;
    public int effId1;

    public int itemid2;
    public int itemNum2;
    public int effId2;

    public int itemid3;
    public int itemNum3;
    public int effId3;

    public int itemid4;
    public int itemNum4;
    public int effId4;

    public int itemid5;
    public int itemNum5;
    public int effId5;

    public int itemid6;
    public int itemNum6;
    public int effId6;

    public int itemid7;
    public int itemNum7;
    public int effId7;

    public int itemid8;
    public int itemNum8;
    public int effId8;

    public TotalChargeItemItem this[int idx]
    {
        get
        {
            switch (idx)
            {
                case 0:
                    return new TotalChargeItemItem(itemid1, itemNum1, effId1);
                case 1:
                    return new TotalChargeItemItem(itemid2, itemNum2, effId2);
                case 2:
                    return new TotalChargeItemItem(itemid3, itemNum3, effId3);
                case 3:
                    return new TotalChargeItemItem(itemid4, itemNum4, effId4);
                case 4:
                    return new TotalChargeItemItem(itemid5, itemNum5, effId5);
                case 5:
                    return new TotalChargeItemItem(itemid6, itemNum6, effId6);
                case 6:
                    return new TotalChargeItemItem(itemid7, itemNum7, effId7);
                case 7:
                    return new TotalChargeItemItem(itemid8, itemNum8, effId8);
                default:
                    return null;
            }
        }
    }

    public int ItemMaxNum
    {
        get
        {
            return 8;
        }
    }
}

public class TotalChargeItemItem
{
    public int itemid;
    public int itemNum;
    public int effId;

    public TotalChargeItemItem(int id, int num, int eff)
    {
        itemid = id;
        itemNum = num;
        effId = eff;
    }
}
