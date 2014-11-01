using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TitleModule : ModuleBase
{
    #region 数据相关;
    private Queue<int> mCacheList = new Queue<int>();

    private static TitleModule mInstance = null;

    private bool mNeedShowRedPoint = false;
    public static TitleModule Instance
    {
        get
        {
            if (mInstance == null)
                mInstance = ModuleManager.Instance.FindModule<TitleModule>();

            return mInstance;
        }
    }


    public bool IsShowRedPoint
    {
        get
        {
            return mNeedShowRedPoint;
        }
        set
        {
            mNeedShowRedPoint = value;
        }
    }

    public void GetNewTitle(int titleId)
    {
        string text = StringHelper.GetString("get_new_title") + TitleModule.GetTitleNameById(titleId);
        PopTipManager.Instance.AddNewTip(text);

        //AddUnlockQueue(i + 1);

        OnGetNewTitle();
    }

    void OnGetNewTitle()
    {
        CityFormManager.SetRedPointActive("beibao", true);
        IsShowRedPoint = true;
    }

    public void CheckAndPlayTitleUnlock()
    {
        if (mCacheList.Count <= 0)
            return;

        if (!canShow())
            return;

        if (!WindowManager.Instance.IsOpen("titleopen"))
        {
            if (!WindowManager.Instance.HasQueueOpenUI("titleopen"))
            {
                WindowManager.Instance.QueueOpenUI("titleopen");
            }
        }
    }

    public void AddUnlockQueue(int titleid)
    {
        if (mCacheList.Contains(titleid))
            return;

        mCacheList.Enqueue(titleid);

        if(canShow())
        {
            if (!WindowManager.Instance.HasQueueOpenUI("titleopen"))
                WindowManager.Instance.QueueOpenUI("titleopen");
        }
    }

    public int PopCache()
    {
        if (mCacheList.Count <= 0)
        {
            return -1;
        }
        return mCacheList.Dequeue();
    }

    public int Peek()
    {
        if (mCacheList.Count <= 0)
            return -1;

        return mCacheList.Peek();
    }

    public bool needClose(int funid)
    {
        if (mCacheList.Count > 0)
            return false;
        return true;
    }

    private bool canShow()
    {
        return !QuestHelper.IsInFightScene() && !WindowManager.Instance.IsOpen("loading");
    }

    #endregion

    #region 表格相关;

    // 最多的称号数目;
    public const int MAX_TITLE_COUNT = 64;

    public static BetterList<int> GetAllTitleGroupId()
    {
        BetterList<int> res = new BetterList<int>();

        IDictionaryEnumerator itr = DataManager.TitleGroupTable.GetEnumerator();
        while (itr.MoveNext())
        {
            if (res.Contains((int)itr.Key))
                continue;

            res.Add((int)itr.Key);
        }
        return res;
    }

    public static string GetTitleNameById(int id)
    {
        TitleItemTableItem item = GetTitleItemById(id);
        if (item == null)
            return null;

        return item.name;
    }

    public static string GetTitleGroupNameById(int id)
    {
        TitleGroupTableItem item = GetTitleGroupItemById(id);
        if (item == null)
            return null;

        return item.name;
    }

    public static TitleGroupTableItem GetTitleGroupItemById(int id)
    {
        if (!DataManager.TitleGroupTable.ContainsKey(id))
            return null;

        TitleGroupTableItem item = DataManager.TitleGroupTable[id] as TitleGroupTableItem;
        if (item == null)
            return null;

        return item;
    }

    public static TitleItemTableItem GetTitleItemById(int id)
    {
        if (!DataManager.TitleItemTable.ContainsKey(id))
            return null;

        TitleItemTableItem item = DataManager.TitleItemTable[id] as TitleItemTableItem;

        return item;
    }

    public static string GetTitleImgById(int id)
    {
        TitleItemTableItem item = GetTitleItemById(id);

        if (item == null)
            return null;

        return item.picName;
    }

    public static int GetTitleBuffIdById(int id)
    {
        TitleItemTableItem item = GetTitleItemById(id);

        if (item == null)
            return -1;

        return item.buffId;
    }
    #endregion
}
