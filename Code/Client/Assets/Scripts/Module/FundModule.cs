using UnityEngine;
using System.Collections;

public class FundModule : ModuleBase
{
    public const int MAX_FUND_ITEM_COUNT = 32;
    //public const int FUND_MENU_ID = 19;

    private uint mLastMillionSec = 0;
    private uint mOneSecNum = 0;

    private static PlayerDataModule mPdm = null;

    static PlayerDataModule PDM
    {
        get
        {
            if (mPdm == null)
                mPdm = ModuleManager.Instance.FindModule<PlayerDataModule>();

            return mPdm;
        }
    }

    public static FundTableItem GetItemByID(int id)
    {
        if(!DataManager.FundTable.ContainsKey(id))
            return null;

        return DataManager.FundTable[id] as FundTableItem;
    }


    public static bool GetItemGetDone(int id)
    {
        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (pdm == null)
        {
            Debug.LogError("fuck");
            return false;
        }

        return pdm.IsFundItemGetDone(id);
    }

    public static void CheckOpenFundActivity()
    {
        //基金返利开启等级限制;
        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();

        int openLv = (int)ConfigManager.GetVal<int>(ConfigItemKey.FUND_OPEN_LV);
        if (openLv > pdm.GetLevel())
            return;

        int val = pdm.GetFundTimeSec();

        // 活动没结束展示界面;
        if (val > 0)
        {
            CityFormManager.OpenChildFunc("fund");
        }
        else
        {
            BitArray arr = pdm.GetFundFlags();

            // 活动结束但是参加活动了，仍然展示;
            if(arr != null)
            {
                foreach(bool a in arr)
                {
                    if (a)
                    {
                        CityFormManager.OpenChildFunc("fund");
                    }
                }
            }
            else
            {
                CityFormManager.CloseChildFunc("fund");
            }
        }
    }
    public static void OnFirstChargePicked(bool picked)
    {
        if(picked)
        {
            CityFormManager.CloseChildFunc("firstcharge");
        }
    }
    public override void Update(uint elapsed)
    {
        base.Update(elapsed);

        mLastMillionSec = (mLastMillionSec + elapsed) % uint.MaxValue;

        if ((mLastMillionSec - mOneSecNum) >= 1000)
        {
            mOneSecNum = mLastMillionSec;
            //Pdm.SubEggTimeSeconds(

            PDM.SubFundTimeSec();
        }
    }

    public static string GetTimeHMSStr()
    {
        int sec = PDM.GetFundTimeSec();
        
        if (sec < 0)
            sec = 0;

        //return TimeUtilities.GetCountDownHMS(sec * 1000);
        return TimeUtilities.GetCountDownDHMS(sec * 1000);
    }
}
