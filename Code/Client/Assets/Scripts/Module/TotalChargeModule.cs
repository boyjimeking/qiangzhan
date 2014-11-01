using UnityEngine;
using System.Collections;

public class TotalChargeModule : ModuleBase
{
    public const int MAX_TOTAL_CHARGE_ITEM = 16;

    public static TotalChargeTableItem GetItem(int id)
    {
        if (!DataManager.TotalChargeTable.ContainsKey(id))
            return null;

        TotalChargeTableItem item = DataManager.TotalChargeTable[id] as TotalChargeTableItem;

        return item;
    }

    /// <summary>
    /// 获取升到下个阶段还需充值多少钻石;
    /// </summary>
    /// <returns></returns>
    public static int GetNeedMoneyToNextReward()
    {
        return -1;
    }

    /// <summary>
    /// 获取玩家当前可以领取的最大奖励resid;
    /// </summary>
    /// <returns></returns>
    public static int GetMaxRewardIdx()
    {
        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();

        if(pdm == null)
            return -1;

        uint total = pdm.GetTotalChargeNum();
        int res = 0;
        for(int i = 0, j = DataManager.TotalChargeTable.Count ; i < j; i++)
        {
            TotalChargeTableItem item = DataManager.TotalChargeTable[i] as TotalChargeTableItem;

            if (item == null)
                continue;

            if (total >= item.chargeNum)
                res++;
            else
                break;
        }

        return res;
    }

    /// <summary>
    /// 是否是最高奖励，超过最高奖励不可领取;
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static bool IsLastRewardIdx(int id)
    {
        return id >= DataManager.TotalChargeTable.Count;
    }
}
