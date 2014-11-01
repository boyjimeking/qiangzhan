using UnityEngine;
using System.Collections;

public class FirstChargeItemInfo
{
    public int itemid;
    public int itemnum;
    public FirstChargeItemInfo()
    {
        itemid = -1;
        itemnum = -1;
    }
}

/// <summary>
/// DataManager中用到的读取数据的类(firstcharge表);
/// </summary>
public class FirstChargeRewardTableItemBase
{
    public int id;
	public string desc;//描述
	public string title;//邮件标题
	public string content;//邮件内容

    public int itemid0;
    public int itemid1;
    public int itemid2;
    public int itemid3;
    public int itemid4;
    public int itemid5;
    public int itemid6;

    public int itemnum0;
    public int itemnum1;
    public int itemnum2;
    public int itemnum3;
    public int itemnum4;
    public int itemnum5;
    public int itemnum6;
}

public class FirstChargeRewardTableItem
{
    public int id;
    public string desc;//描述
    public string title;//邮件标题
    public string content;//邮件内容
    public FirstChargeItemInfo[] FirstChargeItems = null;

    public FirstChargeRewardTableItem()
    {
        id = 0;
        desc = "";
        title = "";
        content = "";

        FirstChargeItems = new FirstChargeItemInfo[7];
        for (int i = 0, j = FirstChargeItems.Length; i < j; i++)
        {
            FirstChargeItemInfo info = new FirstChargeItemInfo();
            FirstChargeItems[i] = info;
        }
    }
    public static explicit operator FirstChargeRewardTableItem(FirstChargeRewardTableItemBase mb)
    {
        if (mb == null)
            return null;

        FirstChargeRewardTableItem item = new FirstChargeRewardTableItem();

        item.id = mb.id;
        item.desc = mb.desc;
        item.title = mb.title;
        item.content = mb.content;

        item.FirstChargeItems[0].itemid = mb.itemid0;
        item.FirstChargeItems[1].itemid = mb.itemid1;
        item.FirstChargeItems[2].itemid = mb.itemid2;
        item.FirstChargeItems[3].itemid = mb.itemid3;
        item.FirstChargeItems[4].itemid = mb.itemid4;
        item.FirstChargeItems[5].itemid = mb.itemid5;
        item.FirstChargeItems[6].itemid = mb.itemid6;

        item.FirstChargeItems[0].itemnum = mb.itemnum0;
        item.FirstChargeItems[1].itemnum = mb.itemnum1;
        item.FirstChargeItems[2].itemnum = mb.itemnum2;
        item.FirstChargeItems[3].itemnum = mb.itemnum3;
        item.FirstChargeItems[4].itemnum = mb.itemnum4;
        item.FirstChargeItems[5].itemnum = mb.itemnum5;
        item.FirstChargeItems[6].itemnum = mb.itemnum6;
        
        return item;
    }
}