using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//功能解锁模块
public class FunctionModule : ModuleBase
{
    private Hashtable mUnlocks = new Hashtable();
    private BetterList<int> mIgnoreList = new BetterList<int>();
    private Dictionary<int, List<int>> mSubFunc = new Dictionary<int, List<int>>();  
    private int mLastLevel = -1;

    private Queue<int> mCacheList = new Queue<int>();
    override protected void OnEnable()
    {
        EventSystem.Instance.addEventListener(PlayerDataEvent.PLAYER_DATA_CHANGED, OnCheckFunction);
        EventSystem.Instance.addEventListener(FunctionEvent.FUNCTION_CHECK_EVENT, OnCheckFunction);
        EventSystem.Instance.addEventListener(FinishQuestEvent.QUEST_FINISHED_ALL, OnCheckFunction);
    }

    override protected void OnDisable()
    {
        EventSystem.Instance.removeEventListener(PlayerDataEvent.PLAYER_DATA_CHANGED, OnCheckFunction);
        EventSystem.Instance.removeEventListener(FunctionEvent.FUNCTION_CHECK_EVENT, OnCheckFunction);
        EventSystem.Instance.removeEventListener(FinishQuestEvent.QUEST_FINISHED_ALL, OnCheckFunction);
    }


    public int PopCache()
    {
        if( mCacheList.Count <= 0 )
        {
            return -1;
        }
        return mCacheList.Dequeue();
    }

    public bool OnEnd(int funid)
    {
        OnUnlockComplete(funid);

        if (mCacheList.Count > 0)
            return false;
        return true;
    }

    private void OnCheckFunction(EventBase evt)
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;
//         if (mLastLevel == module.GetLevel())
//             return;

        bool first = false;

        if( mLastLevel == -1 )
        {
            first = true;
        }

        mLastLevel = module.GetLevel();

        IDictionaryEnumerator itr = DataManager.MenuTable.GetEnumerator();
        while (itr.MoveNext())
        {
            MenuTableItem item = itr.Value as MenuTableItem;

            if (IsFunctionUnlock(item.mId))
            {
                continue;
            }

            if (item.unlock <= 0 && item.questid <= 0)
            {
                if (IsParentOrChildType(item))
                    continue;

                else
                {
                    UnlockFunction(item.mId, first);
                }
            }
            else if (mLastLevel >= item.unlock && (item.questid < 0 || module.IsQuestFinish(item.questid)))
            {
                //二级按钮;
                if (item.menuOpType != (int)MenuOpType.MenuOpType_ParentUI && item.parentid > 0)
                    InnerOpenChildFunc(item.mId);
                else
                {
                    UnlockFunction(item.mId, first);
                }
            }
        }
    }

    public bool IsFunctionUnlock(int id)
    {
        if (mUnlocks.ContainsKey(id))
            return true;
        return false;
    }

    public void UnlockFunction(int id,bool first)
    {
        if( !first )
        {
//             UIFuncOpen ui = WindowManager.Instance.GetUI("funcopen") as UIFuncOpen;
//             if (ui == null)
//             {
//                 WindowManager.Instance.QueueOpenUI("funcopen");
//             }

            if (!WindowManager.Instance.IsOpen("funcopen"))
            {
                if (!WindowManager.Instance.HasQueueOpenUI("funcopen"))
                {
                    WindowManager.Instance.QueueOpenUI("funcopen");
                }
            }
            mCacheList.Enqueue(id);
//             ui = WindowManager.Instance.GetUI("funcopen") as UIFuncOpen;
//             ui.AddFunction(id, OnUnlockComplete);
        }else
        {
            OnUnlockComplete(id , false);
        }
    }

    private void OnUnlockComplete(int id , bool guide = true)
    {
        mUnlocks.Add(id, true);

        FunctionEvent evt = new FunctionEvent(FunctionEvent.FUNCTION_UNLOCKED);
        evt.functionid = id;
        EventSystem.Instance.PushEvent(evt);

        //触发引导
        if (guide)
        {
            GuideModule module = ModuleManager.Instance.FindModule<GuideModule>();
            module.OnFunctionUnlock(id);
        }
    }

    public Hashtable GetUnlocks()
    {
        return mUnlocks;
    }

    /// <summary>
    /// 功能开启是否显示该功能图标;
    /// </summary>
    /// <param name="resId"></param>
    /// <returns></returns>
    public static bool IsShowOnFuncOpen(int resId)
    { 
        MenuTableItem item = GetItem(resId);
        if (item == null)
        {
            Debug.LogError("不存在的menu id:" + resId);
            return false;
        }

        // 是父图标或者子图标默认不显示;
        if (item.menuOpType == 2 || item.parentid > 0)
            return false;

        return true;
    }


    public static MenuTableItem GetItem(int resId)
    {
        if (!DataManager.MenuTable.ContainsKey(resId))
        {
            Debug.LogError("不存在的menu id:" + resId);
            return null;
        }


        return DataManager.MenuTable[resId] as MenuTableItem;
    }

    public static int GetParentIdByChildId(int resid)
    {
        MenuTableItem item = GetItem(resid);
        if (item == null)
            return -1;

        if (item.parentid <= 0)
        {
            Debug.LogError("不是子类型;");
            return -1;
        }

        return item.parentid;
    }

    public List<int> GetChildIdsByParentId(int resid)
    {
        List<int> res = new List<int>();
        IDictionaryEnumerator itr = DataManager.MenuTable.GetEnumerator();
        while (itr.MoveNext())
        {
            MenuTableItem item = itr.Value as MenuTableItem;

            if (item == null)
                continue;

            if (res.Contains(item.mId))
                continue;

            if (item.parentid != resid)
                continue;

            if (isChildUnlock(item.mId))
                res.Add(item.mId);
        }
//         foreach (int id in DataManager.MenuTable.Keys)
//         {
//             MenuTableItem item = DataManager.MenuTable[id] as MenuTableItem;
//             
//             if (item == null)
//                 continue;
// 
//             if(res.Contains(item.mId))
//                 continue;
// 
//             if (item.parentid != resid)
//                 continue;
// 
//             if (isChildUnlock(item.mId))
//                 res.Add(item.mId);
//         }

        return res;
    }

    public static int GetMenuIdByUIName(string uiname)
    {
        IDictionaryEnumerator itr = DataManager.MenuTable.GetEnumerator();
        while (itr.MoveNext())
        {
            MenuTableItem item = itr.Value as MenuTableItem;

            if (item == null)
                continue;

            if (string.Equals(item.uiName, uiname))
            {
                return item.mId;
            }
        }

        return -1;
    }

    /// <summary>
    /// 通过逻辑控制关闭(CloseChildFunc)的字图标不会再次打开;
    /// </summary>
    /// <param name="id"></param>
    private void InnerOpenChildFunc(int id)
    {
        MenuTableItem item = GetItem(id);
        if (item == null)
            return;

        // 不是子图标;
        if (item.parentid <= 0)
        {
            return;
        }

        if (mIgnoreList.Contains(id))
            return;

        openChild(id);
    }

    void openChild(int id)
    {
        int parentid = GetParentIdByChildId(id);

        if (!IsFunctionUnlock(parentid))
            OnUnlockComplete(parentid, false);

        if (mSubFunc.ContainsKey(parentid))
        {
            if (mSubFunc[parentid] == null)
            {
                mSubFunc[parentid] = new System.Collections.Generic.List<int>();
            }

            if (!mSubFunc[parentid].Contains(id))
            {
                mSubFunc[parentid].Add(id);
            }
        }
        else
        {
            mSubFunc.Add(parentid, new System.Collections.Generic.List<int>());

            mSubFunc[parentid].Add(id);
        }
    }

    /// <summary>
    /// 打开某个子功能(并显示父功能图标);
    /// 不是父子关系的图标默认功能开启就显示图标，不走这个逻辑;
    /// </summary>
    /// <param name="id"></param>
    public void OpenChildFunc(int id)
    {
        MenuTableItem item = GetItem(id);
        if (item == null)
            return;

        // 不是子图标;
        if (item.parentid <= 0)
        {
            return;
        }

        if(item.questid > 0 || item.unlock > 0)
        {
            Debug.LogError("这个是通过等级或者任务来开启的二级功能图标，无法通过代码调用打开，是不是策划表填错了id = " + id.ToString());
            return;
        }

        openChild(id);
    }


    /// <summary>
    /// 关闭二级子功能(图标);
    /// (默认通过该接口关闭的二级图标，无法通过InnerOpenChildFunction再次打开);
    /// </summary>
    /// <param name="id"></param>
    public void CloseChildFunc(int id)
    {
        MenuTableItem item = GetItem(id);
        if (item == null)
            return;

        // 不是子图标;
        if (item.parentid <= 0)
        {
            return;
        }

        addToIgnorList(id);

        int parentid = GetParentIdByChildId(id);
        
        if (mSubFunc.ContainsKey(parentid))
        {
            if (mSubFunc[parentid].Contains(id))
            {
                mSubFunc[parentid].Remove(id);
                removeMenuBtn(id);

                if (mSubFunc[parentid].Count == 0)
                {
                    //removeParentMenuBtn(parentid);
                    removeMenuBtn(parentid);
                }
            }
        }
    }

    /// <summary>
    /// 目前设定只要通过CloseChildFunc关闭过的图标，无法
    /// 再通过InnerOpenChildFunc再次打开（也就是说CityForm界面关闭再打开不会走条件检测并打开界面逻辑），
    /// 只能通过调用OpenChildFunction再次开启二级图标;
    /// </summary>
    void addToIgnorList(int childFuncId)
    {
        if (!mIgnoreList.Contains(childFuncId))
            mIgnoreList.Add(childFuncId);
    }

    void removeMenuBtn(int id)
    {
        FunctionEvent fe = new FunctionEvent(FunctionEvent.FUNCTION_LOCKED);
        fe.functionid = id;
        EventSystem.Instance.PushEvent(fe);
    }

    //void removeParentMenuBtn(int id)
    //{
    //    MenuTableItem item = GetItem(id);

    //    if (item == null)
    //        return;

    //    if (item.menuOpType != 2)
    //    {
    //        return;
    //    }

    //    removeMenuBtn(id);
    //}

    /// <summary>
    /// 判断该按钮时否是集合按钮或者是子按钮，这两类按钮必须通过代码调用才可以展示;
    /// </summary>
    /// <param name="resid"></param>
    /// <returns></returns>
    public static bool IsParentOrChildType(MenuTableItem item)
    {
        if(item == null)
            return false;

        return item.menuOpType == (uint) MenuOpType.MenuOpType_ParentUI || item.parentid > 0;
    }

    /// <summary>
    /// 判断图标是否是根据等级解锁和任务解锁;
    /// 
    /// </summary>
    /// <param name="resid"></param>
    /// <returns></returns>
    public static bool IsUnlockByLvOrQuest(int resid)
    {
        MenuTableItem item = GetItem(resid);
        if (item == null)
            return false;

        if (item.unlock > 0 || item.questid > 0)
            return true;

        return false;
    }

    bool isChildUnlock(int id)
    {
        foreach(int key in mSubFunc.Keys)
        {
            List<int> subIds = mSubFunc[key];
            if(subIds == null || subIds.Count <= 0)
                continue;

            if (subIds.Contains(id))
                return true;
        }

        return false;
    }

    //bool isUnlocked(int id)
    //{
    //    if (mUnlocks.ContainsKey(id))
    //        return true;

    //    return false;
    //}
}
