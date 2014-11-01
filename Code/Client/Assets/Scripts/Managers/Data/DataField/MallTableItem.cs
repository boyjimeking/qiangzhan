using System.Collections;

public class MallItemInfo
{
    public uint count;
    public uint processOrignal;
    public uint processNow;

    public MallItemInfo()
    {
        count = 0;
        processOrignal = 0;
        processNow = 0;
    }
}

/// <summary>
/// DataManager中用到的读取数据的类(mall表);
/// </summary>
public class MallTableItemBase
{
    public int resId;
    public int itemId;
    public int processType;
    public int subField;   //所属的分栏;
    public int limitType; //限购类型--决定商品显示在热销栏处还是限购栏处;
    public uint limitTimes;//限购次数;
    ///上架类型;
    public int saleType;

    public uint count0;
    public uint processOrignal0;
    public uint processNow0;

    public uint count1;
    public uint processOrignal1;
    public uint processNow1;

    public uint count2;
    public uint processOrignal2;
    public uint processNow2;

    public uint count3;
    public uint processOrignal3;
    public uint processNow3;

    public uint count4;
    public uint processOrignal4;
    public uint processNow4;

    public uint count5;
    public uint processOrignal5;
    public uint processNow5;

    public string detail;
}

public class MallTableItem
{
    public int resId;
    public int itemId;
    public int processType;
    public int subField;   //所属的分栏;
    public int limitType; //限购类型--决定商品显示在热销栏处还是限购栏处;
    public uint limitTimes;//限购次数;
    ///上架类型;
    public int saleType;
    public MallItemInfo[] mallItems = null;
    public string detail; //物品说明;


    public MallTableItem()
    {
        resId = 0;
        itemId = 0;
        processType = 0;
        subField = 0;
        limitType = 0;
        limitTimes = 0;

        mallItems = new MallItemInfo[6];
        for (int i = 0, j = mallItems.Length; i < j; i++)
        {
            MallItemInfo info = new MallItemInfo();
            mallItems[i] = info;
        }
    }

    //public static implicit operator MallTableItem(MallTableItemBase mb)
    //{
    //    if (mb == null)
    //        return null;

    //    MallTableItem item = new MallTableItem();
        
    //    item.resId = mb.itemId;
    //    item.itemId = mb.itemId;
    //    item.processType = mb.processType;
    //    item.subField = mb.subField;
    //    item.limitType = mb.limitType;
    //    item.limitTimes = mb.limitTimes;
    //    item.saleType = mb.saleType;

    //    item.mallItems[0].count = mb.count0;
    //    item.mallItems[0].processOrignal = mb.processOrignal0;
    //    item.mallItems[0].processNow = mb.processNow0;

    //    item.mallItems[1].count = mb.count1;
    //    item.mallItems[1].processOrignal = mb.processOrignal1;
    //    item.mallItems[1].processNow = mb.processNow1;

    //    item.mallItems[2].count = mb.count2;
    //    item.mallItems[2].processOrignal = mb.processOrignal2;
    //    item.mallItems[2].processNow = mb.processNow2;

    //    item.mallItems[3].count = mb.count3;
    //    item.mallItems[3].processOrignal = mb.processOrignal3;
    //    item.mallItems[3].processNow = mb.processNow3;

    //    item.mallItems[4].count = mb.count4;
    //    item.mallItems[4].processOrignal = mb.processOrignal4;
    //    item.mallItems[4].processNow = mb.processNow4;

    //    item.mallItems[5].count = mb.count5;
    //    item.mallItems[5].processOrignal = mb.processOrignal5;
    //    item.mallItems[5].processNow = mb.processNow5;

    //    return item;
    //}

    public static explicit operator MallTableItem(MallTableItemBase mb)
    {
        if (mb == null)
            return null;

        MallTableItem item = new MallTableItem();

        item.resId = mb.resId;
        item.itemId = mb.itemId;
        item.processType = mb.processType;
        item.subField = mb.subField;
        item.limitType = mb.limitType;
        item.limitTimes = mb.limitTimes;
        item.saleType = mb.saleType;

        item.mallItems[0].count = mb.count0;
        item.mallItems[0].processOrignal = mb.processOrignal0;
        item.mallItems[0].processNow = mb.processNow0;

        item.mallItems[1].count = mb.count1;
        item.mallItems[1].processOrignal = mb.processOrignal1;
        item.mallItems[1].processNow = mb.processNow1;

        item.mallItems[2].count = mb.count2;
        item.mallItems[2].processOrignal = mb.processOrignal2;
        item.mallItems[2].processNow = mb.processNow2;

        item.mallItems[3].count = mb.count3;
        item.mallItems[3].processOrignal = mb.processOrignal3;
        item.mallItems[3].processNow = mb.processNow3;

        item.mallItems[4].count = mb.count4;
        item.mallItems[4].processOrignal = mb.processOrignal4;
        item.mallItems[4].processNow = mb.processNow4;

        item.mallItems[5].count = mb.count5;
        item.mallItems[5].processOrignal = mb.processOrignal5;
        item.mallItems[5].processNow = mb.processNow5;

        item.detail = mb.detail;

        return item;
    }
}