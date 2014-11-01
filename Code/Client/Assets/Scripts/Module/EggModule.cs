using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Message;

public enum EggType : int
{
    Normal = 1,   //普通蛋;
    Supper,       //高级蛋;
    ZuanShi,      //钻石蛋;
}

// 砸蛋掉落物品信息;
public class EggItemItem
{
    // 物品id;
    public int ItemId
    {
        get;
        set;
    }

    // 物品num;
    public int ItemNum
    {
        get;
        set;
    }

    public EggItemItem()
    {
 
    }
}

public class EggModule : ModuleBase
{
    #region 规则参数
    // 几个蛋;
    public const int EGG_COUNT = 3;

    public const float ITEM_DISPLAY_TIME = 2.0f;
    #endregion

    private PlayerDataModule mPlayerModule = null;

    private uint mLastMillionSec = 0;
    private uint mOneSecNum = 0;
    PlayerDataModule Pdm
    {
        get
        {
            if (mPlayerModule == null)
                mPlayerModule = ModuleManager.Instance.FindModule<PlayerDataModule>();

            return mPlayerModule;
        }
    }

    public void Update(uint elapsed)
    {
        mLastMillionSec = (mLastMillionSec + elapsed) % uint.MaxValue;

        if ((mLastMillionSec - mOneSecNum) >= 1000)
        {
            mOneSecNum = mLastMillionSec;
            //Pdm.SubEggTimeSeconds(

            EggType et = EggType.Normal;
            for (int i = 0; i < 3; i++)
            {
                et = (EggType)(i + 1);

                int sec = GetSecondsByEggId(et);

                if (sec <= 0)
                {
                    if (et == EggType.Normal)
                    {
                        CityFormManager.SetRedPointActive("egg", true);
                    }
                    continue;
                }

                Pdm.SubEggTimeSeconds(et);

                if (et == EggType.Normal)
                {
                    CityFormManager.SetRedPointActive("egg", false);
                }
            }
        }
    }

    public bool IsEnougthMoney(EggType et , ref ProceedsType pt)
    {
        pt = getCostProcType(et);

        if(Pdm.GetProceeds(pt) < getCostProcNum(et))
            return false;

        return true;
    }

    /// <summary>
    /// 获得对应蛋已经砸的次数;[1..3];
    /// </summary>
    /// <param name="idx"></param>
    public int GetOpenTimeByEggID(EggType et)
    {
        return Pdm.GetEggOpenTimes(et);
    }

    /// <summary>
    /// 大于0表示剩余开启次数;
    /// 等于0表示没有开启次数;
    /// 小于0表示无限制;
    /// </summary>
    /// <param name="idx"></param>
    /// <returns></returns>
    public int GetRemainTimeByEggId(EggType et)
    {
        return getMaxOpenTimes(et) - GetOpenTimeByEggID(et);
    }

    /// <summary>
    /// 获得每个蛋的倒计时剩余秒数;
    /// </summary>
    /// <param name="idx"></param>
    /// <returns></returns>
    public int GetSecondsByEggId(EggType et)
    {
        return Pdm.GetEggTimeSeconds(et);
    }

    /// <summary>
    /// 开蛋成功结果;
    /// </summary>
    /// <param name="idx"></param>
    public void OpenEggSucess(EggType et , List<role_egg_item_items> items)
    {
        Pdm.AddEggOpenTimes(et);

        if (et == EggType.Normal)
        {
            ResetTimeCounter(EggType.Normal);
        }
        else
        {
            if (GetSecondsByEggId(et) <= 0)
            {
                ResetTimeCounter(et);
            }
        }

        EggUIEvent ev = new EggUIEvent(EggUIEvent.EGG_OPEN_SUCESS);
        
        ev.eggType = et;
        ev.items = items;

        EventSystem.Instance.PushEvent(ev);
    }

    /// <summary>
    /// 重置下蛋的时间计时;
    /// </summary>
    /// <param name="idx"></param>
    void ResetTimeCounter(EggType et)
    {
        Pdm.ResetEggTimeSeconds(et);
    }

    public static BetterList<int> GetShowItemsItemID()
    {
        BetterList<int> result = new BetterList<int>();

        IDictionaryEnumerator itr = DataManager.EggTable.GetEnumerator();
        while (itr.MoveNext())
        {
            EggTableItem item = itr.Value as EggTableItem;

            if (item == null || (item.isShow == 0))
                continue;

            result.Add(item.itemId);
        }
//         foreach (int key in DataManager.EggTable.Keys)
//         {
//             EggTableItem item = DataManager.EggTable[key] as EggTableItem;
//             
//             if (item == null || (item.isShow == 0))
//                 continue;
// 
//             result.Add(item.itemId);
//         }

        return result;
    }

    /// <summary>
    /// 判断给定id的物品是否在砸蛋界面的展示列表中;
    /// </summary>
    /// <param name="itemid"></param>
    /// <returns></returns>
    public static bool GetItemIsInShowItems(int itemid)
    {
        BetterList<int> result = new BetterList<int>();
        
        result = GetShowItemsItemID();

        return result.Contains(itemid);
    }

    #region egg_config.txt 表格数据，砸蛋规则;
    // 开蛋时间间隔秒数;
    public static int getCountDownSeconds(EggType et)
    {
        EggConfigTableItem item = DataManager.EggConfigTable[(int)et] as EggConfigTableItem;
        if (item == null)
        {
            GameDebug.LogError("表格数据错误");
            return -1;
        }
        return item.minutes * 60;
    }

    // 最大开蛋次数,知道次数重置;
    public static int getMaxOpenTimes(EggType et)
    {
        EggConfigTableItem item = DataManager.EggConfigTable[(int)et] as EggConfigTableItem;
        if (item == null)
        {
            GameDebug.LogError("表格数据错误");
            return -1;
        }
        return item.times;
    }

    // 倒计时期间是否可以开蛋;
    public static bool canOpenInTimeCounting(EggType et)
    {
        EggConfigTableItem item = DataManager.EggConfigTable[(int)et] as EggConfigTableItem;
        if (item == null)
        {
            GameDebug.LogError("表格数据错误");
            return false;
        }

        return item.canOpen != 0;
    }

    public static ProceedsType getCostProcType(EggType et)
    {
        EggConfigTableItem item = DataManager.EggConfigTable[(int)et] as EggConfigTableItem;
        if (item == null)
        {
            GameDebug.LogError("表格数据错误");
            return ProceedsType.Invalid;
        }

        return (ProceedsType)item.procType;
    }

    public static int getCostProcNum(EggType et)
    {
        EggConfigTableItem item = DataManager.EggConfigTable[(int)et] as EggConfigTableItem;
        if (item == null)
        {
            GameDebug.LogError("表格数据错误");
            return -1;
        }

        return item.procNum;
    }
    #endregion
}
