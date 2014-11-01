using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Message;
using System.IO;
using System;

public class PlayerPlanModule : ModuleBase
{
    public  const int MAX_PLAN_NUM = 21;
    public  const int MIN_PLAN_NUM = 1;
    PlayerData data = null;
    public enum PlanEnum : int
    {
        CHENGZHANG = 1,
        JINGYING = 2,
        ZHIZHUN = 3
    };

     public enum BUTTON_STATE:int
    {
        Invalid = 0,
        Have_noliqu = 1,//未领取
        Have_liqu = 2,//已领取
        Have_guqi = 3,//过期
    };

     PlayerData Pdata
     {
         get
         {
             if (data == null)
                 data = PlayerDataPool.Instance.MainData;

             return data;
         }
     }

     public Dictionary<int, PlanUnitUI> mItemDic = new Dictionary<int, PlanUnitUI>();

    protected override void OnEnable()
    {
        base.OnEnable();
        EventSystem.Instance.addEventListener(PlanyerPlan.PLAN_UPDATE_EVENT, onPlanChange);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        EventSystem.Instance.removeEventListener(PlanyerPlan.PLAN_UPDATE_EVENT, onPlanChange);
    }

    private void onPlanChange(EventBase evt)
    {
        foreach (var item in Pdata.mPlanData.mDataDic)
        {
            foreach (var item_ in mItemDic)
            {
                if (item.Key == item_.Key)
                {
                    if (item.Value.state != item_.Value.mBtnState && item.Value.state == PlayerPlanModule.BUTTON_STATE.Have_liqu)
                        PopTipManager.Instance.AddNewTip(StringHelper.GetString("send_maill"));

                    item_.Value.updateitem(this, item.Value.state, item_.Value.mPlanState);
                }
            }
        }
    }

    public string getjiangliText(int planid)
    {
        IDictionaryEnumerator itr = DataManager.PlanTable.GetEnumerator();
        while (itr.MoveNext())
        {
            PlayerPlanTableItem item = itr.Value as PlayerPlanTableItem;

            if (item == null)
                continue;

            if (item.mId == planid)
                return item.mAwardText;
        }
      return "";
    }

    public string gettiaojianText(int planid)
    {
        IDictionaryEnumerator itr = DataManager.PlanTable.GetEnumerator();
        while (itr.MoveNext())
        {
            PlayerPlanTableItem item = itr.Value as PlayerPlanTableItem;

            if (item == null)
                continue;

            if (item.mId == planid)
                return item.mConText;
        }
//         foreach (int key in DataManager.PlanTable.Keys)
//         {
//             PlayerPlanTableItem item = DataManager.PlanTable[key] as PlayerPlanTableItem;
// 
//             if (item == null)
//                 continue;
// 
//             if (item.mId == planid)
//                 return item.mConText;
//         }

        return "";
    }

    public int getType(int planid)
    {
        IDictionaryEnumerator itr = DataManager.PlanTable.GetEnumerator();
        while (itr.MoveNext())
        {
            PlayerPlanTableItem item = itr.Value as PlayerPlanTableItem;

            if (item == null)
                continue;

            if (item.mId == planid)
                return item.mType;
        }
//         foreach (int key in DataManager.PlanTable.Keys)
//         {
//             PlayerPlanTableItem item = DataManager.PlanTable[key] as PlayerPlanTableItem;
// 
//             if (item == null)
//                 continue;
// 
//             if (item.mId == planid)
//                 return item.mType;
//         }

        return 0;
    }

    public uint getCondition(int planid)
    {
        IDictionaryEnumerator itr = DataManager.PlanTable.GetEnumerator();
        while (itr.MoveNext())
        {
            PlayerPlanTableItem item = itr.Value as PlayerPlanTableItem;
            if (item == null)
                continue;

            if (item.mId == planid)
                return item.mCondition;
        }

        return 0;
    }

    public uint getJewelNum(int planid)
    {
        foreach (var item in Pdata.mPlanData.mDataDic)
        {
            if (item.Value.planid == planid)
                return item.Value.jewel;
        }

        return 0;
    }

    public int getWeek()
    {
        DateTime now = DateTime.Now;
        return (int)now.DayOfWeek;
    }
}
