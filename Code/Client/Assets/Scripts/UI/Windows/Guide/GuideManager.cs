using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GUIDE_TYPE:int
{
    GUIDE_FIRST_GAME = 0,       //第一次进入游戏
    GUIDE_QUEST_ACCPET = 1,     //接受任务
    GUIDE_QUEST_FINISHED = 2,   //完成任务
    GUIDE_STAGE_COMPLETE = 3,   //完成关卡
    GUIDE_ENTER_TRIGGER = 4,    //进入触发点
    GUIDE_FUNCTION_OPEN = 5,    //功能开启
    GUIDE_SUB_FUNCTION_OPEN = 6,//子功能开启    
    GUIDE_OPEN_UI = 7,          //打开指定界面    

    GUIDE_HELPER_ENTER = 8,     //右上角助手

    GUIDE_TYPE_MAX,         
}

public class GuideManager
{
    public Dictionary<GUIDE_TYPE, List<GuideTableItem>> mGuides = new Dictionary<GUIDE_TYPE, List<GuideTableItem>>();

    private static GuideManager msInstance = null;

    //整个游戏只会有一个 引导指向
    private GuideUI mGuideUI = null;

    //mm头像
    private GuideTalkUI mGuideTalkUI = null;

    //当前正在运行的
    private GuideRuntime mGuideRuntime = null;

    //缓存等待引导
    private Queue<int> mCacheQueue = new Queue<int>();

    private List<bool> mForceHideList = new List<bool>();

    private bool mShowLog = true;

    public static GuideManager Instance
    {   
        get{
            return msInstance;
        }
    }
    public GuideManager()
    {
        msInstance = this;
    }
    

    public bool Init(DataTable table)
    {
        IDictionaryEnumerator itr = table.GetEnumerator();
        while (itr.MoveNext())
        {
            GuideTableItem item = itr.Value as GuideTableItem;

            if (item == null)
            {
                continue;
            }
            if (item.type < (int)GUIDE_TYPE.GUIDE_FIRST_GAME || item.type > (int)GUIDE_TYPE.GUIDE_TYPE_MAX)
            {
                LogOut("引导类型填写错误 type=" + item.type.ToString());
                return false;
            }
            GUIDE_TYPE type = (GUIDE_TYPE)item.type;

            if (type == GUIDE_TYPE.GUIDE_ENTER_TRIGGER)
            {
                continue;
            }

            if (!GuideManager.Instance.mGuides.ContainsKey(type))
            {
                mGuides.Add(type, new List<GuideTableItem>());
            }
            mGuides[type].Add(item);
        }
        return true;
    }

    private void HideUI()
    {
        if (mGuideUI != null && mGuideUI.IsOpned())
        {
            mGuideUI.End();
            if (!mGuideRuntime.IsWeak())
            {
                InputSystem.Instance.SetLockMove(false);
            }
        }
        if( mGuideTalkUI != null )
        {
            mGuideTalkUI.Hide();
        }
    }

    private void ShowUI()
    {
        if (mGuideUI == null)
        {
            GameObject obj = WindowManager.Instance.CloneCommonUI("GuideArrowUI");
            obj.SetActive(false);
            GameObject.DontDestroyOnLoad(obj);
            mGuideUI = new GuideUI(obj);
        }

        if (mGuideUI != null && !mGuideUI.IsOpned())
        {
            mGuideUI.Open();
            if (!mGuideRuntime.IsWeak())
            {
                InputSystem.Instance.SetLockMove(true);
            }
        }

        if( mGuideTalkUI != null && mGuideTalkUI.IsOpened() )
        {
            mGuideTalkUI.Show();
        }
    }

    //是否在进行新手引导
    public bool IsGuideShow()
    {
        if (mGuideRuntime != null && mGuideRuntime.IsRuning())
        {
            return true;
        }

        return false;
    }

    private int CheckComplete( int guideID )
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if( module == null )
        {
            return -1;
        }

        string name = module.GetName();

        string key = name + "_guide_" + guideID.ToString();
        if (!PlayerPrefs.HasKey(key))
            return -1;
        return PlayerPrefs.GetInt(key);
    }

    private bool CheckAndBeginGuide(GuideTableItem item , int condition)
    {
        if( item == null )
        {
            return false;
        }
        if (item.condition != condition)
        {
            return false;
        }
        if (CheckComplete(item.id) >= 0)
        {
            return false;
        }
        onBeginGuide(item);
        return true;
    }

    private void LogOut(string log)
    {
        if (mShowLog)
            GameDebug.LogError(log);
    }

    public void OnQuestComplete( int questId )
    {
        if (!mGuides.ContainsKey(GUIDE_TYPE.GUIDE_QUEST_FINISHED))
        {
            return;
        }
        List<GuideTableItem> childs = mGuides[GUIDE_TYPE.GUIDE_QUEST_FINISHED];

        for (int i = 0; i < childs.Count; ++i)
        {
            if (CheckAndBeginGuide(childs[i], questId))
            {
                LogOut("完成任务ID=" + questId.ToString() + " 触发引导ID" + childs[i].id.ToString());
                return;
            }
        }
    }

    public void OnQuestAccept( int questId )
    {
        if (!mGuides.ContainsKey(GUIDE_TYPE.GUIDE_QUEST_ACCPET))
        {
            return;
        }
        List<GuideTableItem> childs = mGuides[GUIDE_TYPE.GUIDE_QUEST_ACCPET];

        for (int i = 0; i < childs.Count; ++i )
        {
            if (CheckAndBeginGuide(childs[i],questId))
            {
                LogOut("接受任务ID=" + questId.ToString() + " 触发引导ID" + childs[i].id.ToString());
                return;
            }
        }
    }

    public void OnOpenFunction( int funcId )
    {
        if (!mGuides.ContainsKey(GUIDE_TYPE.GUIDE_FUNCTION_OPEN))
        {
            return;
        }
        List<GuideTableItem> childs = mGuides[GUIDE_TYPE.GUIDE_FUNCTION_OPEN];

        for (int i = 0; i < childs.Count; ++i)
        {
            if (CheckAndBeginGuide(childs[i], funcId))
            {
                LogOut("开启功能ID=" + funcId.ToString() + " 触发引导ID" + childs[i].id.ToString());
                return;
            }
        }
    }

    public void OnOpenSubFunction( int funcId )
    {
        if (!mGuides.ContainsKey(GUIDE_TYPE.GUIDE_SUB_FUNCTION_OPEN))
        {
            return;
        }
        List<GuideTableItem> childs = mGuides[GUIDE_TYPE.GUIDE_SUB_FUNCTION_OPEN];

        for (int i = 0; i < childs.Count; ++i)
        {
            if (CheckAndBeginGuide(childs[i], funcId))
            {
                LogOut("开启子功能ID=" + funcId.ToString() + " 触发引导ID" + childs[i].id.ToString());
                return;
            }
        }
    }

    public void OnStageComplete(int stageid)
    {
        if (!mGuides.ContainsKey(GUIDE_TYPE.GUIDE_STAGE_COMPLETE))
        {
            return;
        }
        List<GuideTableItem> childs = mGuides[GUIDE_TYPE.GUIDE_STAGE_COMPLETE];

        for (int i = 0; i < childs.Count; ++i)
        {
            if (CheckAndBeginGuide(childs[i], stageid))
            {
                LogOut("完成关卡ID=" + stageid.ToString() + " 触发引导ID" + childs[i].id.ToString());
                return;
            }
        }
    }

    public void OnEnterTrigger(int guideId)
    {
        if(!DataManager.GuideTable.ContainsKey(guideId))
        {
            return;
        }
        GuideTableItem item = DataManager.GuideTable[guideId] as GuideTableItem;
        if( item == null )
        {
            return;
        }
        LogOut("进入热区 触发引导ID= " + guideId.ToString());

        if (CheckComplete(item.id) >= 0)
        {
            return;
        }
        onBeginGuide(item);
    }
    //打开翻牌界面
    public void OnOpenUI(string uiName)
    {
        if (!DataManager.UITable.ContainsKey(uiName))
        {
            GameDebug.Log("OnOpenUI 没有找到UI :" + uiName);
            return ;
        }
        int condtion = -1;
        if( uiName == "quest" )
        {
            UIQuestForm quest = WindowManager.Instance.GetUI(uiName) as UIQuestForm;
            condtion = quest.GetCurrentID();
        }else if( uiName == "stagelist" )
        {
            UIStageList stagelist = WindowManager.Instance.GetUI(uiName) as UIStageList;
            condtion = stagelist.GetCurrentID();
        }

        //else if (uiName == "mainmap")
        //{
        //     WorldMapModule mWorldMapModule = ModuleManager.Instance.FindModule<WorldMapModule>();
        //     if (mWorldMapModule!= null)
        //         condtion = mWorldMapModule.GuideResId;
        //}
        UITableItem item = DataManager.UITable[uiName] as UITableItem;

        if (!mGuides.ContainsKey(GUIDE_TYPE.GUIDE_OPEN_UI))
        {
            return;
        }
        List<GuideTableItem> childs = mGuides[GUIDE_TYPE.GUIDE_OPEN_UI];
        for (int i = 0; i < childs.Count; ++i)
        {
            if( childs[i].param == item.resID )
            {
                if (CheckAndBeginGuide(childs[i], condtion))
                {
                    LogOut("打开界面 name = " + uiName + " 触发引导ID= " + childs[i].id.ToString());
                    return;
                }
            }
        }
    }

    //第一次进入游戏
    public void OnFistGame()
    {
        if( !mGuides.ContainsKey( GUIDE_TYPE.GUIDE_FIRST_GAME ) )
        {
            return;
        }
        List<GuideTableItem> childs = mGuides[GUIDE_TYPE.GUIDE_FIRST_GAME];

        if( childs.Count <= 0 )
        {
            return;
        }

        LogOut("第一次进入游戏 触发引导ID= " + childs[0].id.ToString());
        onBeginGuide(childs[0]);
    }

    public void OnBeginHelper(int questid)
    {
        if( mGuideRuntime != null && mGuideRuntime.IsRuning() && !mGuideRuntime.IsHelperReplace() )
        {
            return;
        }

        if (!mGuides.ContainsKey(GUIDE_TYPE.GUIDE_HELPER_ENTER))
        {
            return;
        }
        List<GuideTableItem> childs = mGuides[GUIDE_TYPE.GUIDE_HELPER_ENTER];

        for (int i = 0; i < childs.Count; ++i)
        {
            if (CheckAndBeginGuide(childs[i], questid))
            {
                LogOut("助手任务ID=" + questid.ToString() + " 触发引导ID" + childs[i].id.ToString());
                return;
            }
        }
    }

    private void onBeginGuide(GuideTableItem item)
    {
        if( item == null )
        {
            return;
        }

        if (mGuideRuntime != null && mGuideRuntime.GetResID() == item.id)
        {
            return;
        }

        if (mGuideRuntime != null && mGuideRuntime.IsRuning())
        {
            //mCacheQueue.Enqueue(id);
            GameDebug.Log("引导 id=" + mGuideRuntime.GetResID().ToString() + "未完成   强制下一个引导 id=" + item.id.ToString());
            mGuideRuntime.Destroy();
        }


//         if( mGuideRuntime != null && mGuideRuntime.IsRuning() )
//         {
//             mCacheQueue.Enqueue(id);
//             GameDebug.Log("引导问题 : 有一个引导正在进行中. 放入缓存 id = " + id.ToString());
//             return;
//         }

        mGuideRuntime = new GuideRuntime(item.id, item.script);

        mGuideRuntime.Begin();
    }

    public void ForceHide(bool hide)
    {
        if( hide )
        {
            mForceHideList.Add(hide);
        }else
        {
            if( mForceHideList.Count > 0 )
            {
                mForceHideList.RemoveAt(0);
            }
        }
    }

    public void Update(uint elapsed)
    {
        if (mForceHideList.Count > 0)
        {
            HideUI();
            return;
        }

        if (mGuideRuntime == null || !mGuideRuntime.IsRuning())
        {
            HideUI();
            return;
        }
        //更新控件状态
        mGuideRuntime.Update(elapsed);
        //
        Vector3 pos = mGuideRuntime.GetControlPos();
        if( pos == Vector3.zero || mGuideRuntime.IsHide() )
        {
            HideUI();
            return;
        }
        UpdateUIPos(pos);
        ShowUI();

        mGuideUI.UpdateDepth(mGuideRuntime.GetDepth());
    }

    public void RegisterTalk(string talk , int pos)
    {
        if( mGuideTalkUI == null )
        {
            GameObject obj = WindowManager.Instance.CloneCommonUI("GuideTalkUI");
            obj.SetActive(false);
            GameObject.DontDestroyOnLoad(obj);

            mGuideTalkUI = new GuideTalkUI(obj);
        }

        if( mGuideTalkUI != null )
        {
            mGuideTalkUI.Open(talk, pos, mGuideRuntime.GetDepth() + 1);
        }
    }

    public void UnRegisterTalk()
    {
        if( mGuideTalkUI != null && mGuideTalkUI.IsOpened() )
        {
            mGuideTalkUI.Close();
        }
    }

    private void UpdateUIPos(Vector3 pos)
    {
        if (mGuideUI == null || mGuideRuntime == null)
            return;

        //计算二维坐标的时候 必须加入SpectRatio 做屏幕适应
        float spectRatio = WindowManager.GetSpectRatio();

        int ctrlWidth = mGuideRuntime.GetControlWidth();
        int ctrlHeight = mGuideRuntime.GetControlHeight();

        Vector2 offset = mGuideRuntime.GetControlPovitOffset();

//         ArrowRot rot = mGuideRuntime.GetArrowRot();
// 
//         if( rot == ArrowRot.ArrowRot_Invaild )
//         {
//             //控件在屏幕左边
//             if (pos.x < Screen.width / 2)
//             {
//                 rot = ArrowRot.ArrowRot_RIGHT;
//             }
//             else
//             {
//                 rot = ArrowRot.ArrowRot_LEFT;
//             }
//         }
// 
//         Vector2 arrowPos = new Vector2();
//         if( rot == ArrowRot.ArrowRot_LEFT )
//         {
//             arrowPos.y = pos.y;
//             arrowPos.x = pos.x - (ctrlWidth / 2 + mGuideUI.GetArrowWidth() / 2) / spectRatio;            
//         }else if( rot == ArrowRot.ArrowRot_RIGHT )
//         {
//             arrowPos.y = pos.y;
//             arrowPos.x = pos.x + (ctrlWidth / 2 + mGuideUI.GetArrowWidth() / 2) / spectRatio;
//         }else if( rot == ArrowRot.ArrowRot_UP )
//         {
//             arrowPos.x = pos.x;
//             arrowPos.y = pos.y + (ctrlHeight / 2 + mGuideUI.GetArrowHeight() / 2) / spectRatio;
//         }
//         else if (rot == ArrowRot.ArrowRot_DOWN)
//         {
//             arrowPos.x = pos.x;
//             arrowPos.y = pos.y - (ctrlHeight / 2 + mGuideUI.GetArrowHeight() / 2) / spectRatio;
//         }
// 
//         mGuideUI.SetArrowRot(rot);

        Vector2 arrowPos = new Vector2();
        arrowPos.x = pos.x + 15.0f + offset.x;
        arrowPos.y = pos.y - (ctrlHeight / 2 ) / spectRatio + offset.y + 25.0f;

        mGuideUI.SetEffectPos(WindowManager.current2DCamera.ScreenToWorldPoint(pos));
        mGuideUI.SetEffectSize(ctrlWidth, ctrlHeight);
        mGuideUI.SetArrowPos(WindowManager.current2DCamera.ScreenToWorldPoint(arrowPos), mGuideRuntime.IsShowMask(), mGuideRuntime.IsWeak());
    }


    public void OnGuideEnd()
    {
        HideUI();

        if( mGuideRuntime != null )
        {
            GuideTableItem res = mGuideRuntime.GetRes();
            if( res != null && res.beginHelper > 0 )
            {
                ZhushouManager.Instance.Begin();
            }
        }
    }
}
