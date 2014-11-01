using Message;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIQuestForm : UIWindow
{
    public GameObject Qbutton;
    public UILabel QuestName;
    public UILabel QuestScrib;
    public UILabel QuestGoal;
    public UIButton TodoBtn;
    public UIButton GoToBtn;
    public List<AwardItemUI> AwardItemUIList;
    public UILabel Process;
    public UIToggle toggleOne;
    public UIToggle toggleTwo;
    public UIToggle toggleThree;
    private GameObject questDetailObj;
    public UIGrid DropGrid;
    public UIGrid BtnGrid;
    public List<QuestPageUI> mTabs = new List<QuestPageUI>();
    public List<UIScrollView> scrollList = new List<UIScrollView>(); 
    //当前显示任务
    private Quest curQuest;

    private QuestModule mModule;
    private QuestData mQuestData;
    private QuestPageUI mCurPage;
    private bool IsDirty = false;

    //id和btn的映射
    private Dictionary<int, QuestBtnUI> mQuestBtnMap;
   

    protected override void OnLoad()
    {
        base.OnLoad();
        mTabs.Add(new QuestPageUI(FindComponent<UIGrid>("Content/TabPage1/ScrollView/QuestBtnGrid"), QuestType.Main));
        mTabs.Add(new QuestPageUI(FindComponent<UIGrid>("Content/TabPage2/ScrollView/QuestBtnGrid"), QuestType.Side));
        mTabs.Add(new QuestPageUI(FindComponent<UIGrid>("Content/TabPage3/ScrollView/QuestBtnGrid"),QuestType.Daily));
        for (int i = 0; i < 3; ++i)
        {
            scrollList.Add(FindComponent<UIScrollView>("Content/TabPage"+ (i+1)+ "/ScrollView"));
        }
        Qbutton = FindChild("Items/QButton");
        Qbutton.SetActive(false);
        questDetailObj = FindChild("Content/QuestDetails");
        QuestName = FindComponent<UILabel>("Content/QuestDetails/QuestName");
        QuestScrib = FindComponent<UILabel>("Content/QuestDetails/Scrib/QuestScrib");
        QuestGoal = FindComponent<UILabel>("Content/QuestDetails/Goal/QuesGoal");

        TodoBtn = FindComponent<UIButton>("Content/QuestDetails/BtnGrid/TodoBtn");
        GoToBtn = FindComponent<UIButton>("Content/QuestDetails/BtnGrid/GotoBtn");

        Process = FindComponent<UILabel>("Content/QuestDetails/Process");
        toggleOne = FindComponent<UIToggle>("Content/Tabgroup/Tab1");
        toggleTwo = FindComponent<UIToggle>("Content/Tabgroup/Tab2");
        toggleThree = FindComponent<UIToggle>("Content/Tabgroup/Tab3");
        DropGrid = FindComponent<UIGrid>("Content/QuestDetails/DropGrid");
        BtnGrid = FindComponent<UIGrid>("Content/QuestDetails/BtnGrid");
        mModule = ModuleManager.Instance.FindModule<QuestModule>();
        mQuestData = PlayerDataPool.Instance.MainData.mQuestData;
        mQuestBtnMap = new Dictionary<int, QuestBtnUI>();
        for (int i = 0; i < mQuestData.mAllQuest.Count; i++)
        {
            CreateQuestBtn(mQuestData.mAllQuest[i].mId);
        }

    }

    private void CreateQuestBtn(int questId)
    {

        if (mQuestBtnMap.ContainsKey(questId))
        {
            GameDebug.LogError("任务id重复:" + questId);
            return;
        }
        var questVo = DataManager.QuestTable[questId] as QuestTableItem;

        if (questVo == null)
        {
            GameDebug.LogError("无效的questid");
            return;
        }

        QuestBtnUI btn = new QuestBtnUI(Qbutton);
        switch (questVo.questType)
        {
            case QuestType.Daily:
                btn.gameObject.transform.parent = mTabs[2].QuestBtnGrid.gameObject.transform;
                break;
            //--case QuestType.Side:
                //--btn.gameObject.transform.parent = mTabs[1].QuestBtnGrid.gameObject.transform;
                //--break;
            default:
                btn.gameObject.transform.parent = mTabs[0].QuestBtnGrid.gameObject.transform;
                break;
        }

        btn.gameObject.transform.localScale = Vector3.one;
        btn.gameObject.transform.localPosition = Vector3.zero;
        btn.gameObject.name = QuestHelper.GetQuestBtnName(questVo.id, questVo.questType);
        btn.mQuestId = questVo.id;
        mQuestBtnMap.Add(questId, btn);
        btn.QName.text = questVo.questName;
        UIAtlasHelper.SetSpriteImage(btn.QIcon, questVo.questIcon);
        btn.mClickCallback = OnQuestBtnClick;
    }

    protected override void OnOpen(object param = null)
    {       
        mCurPage = mTabs[0];
        scrollList[0].ResetPosition();
        if (mQuestData.mMainQuest.Count > 0)
        {
            mCurPage.mCurShowId = mModule.CurShowIndex = mQuestData.mMainQuest[0].mId;
        }
       
        toggleOne.value = true;
        if (param != null)
        {
            int id = (int)param;
            Quest quest = mModule.GetQuestById(id);
            if (quest!=null && quest.mType == 2)
            {
                mCurPage = mTabs[2];
                toggleThree.value = true;
                toggleOne.value = false;
            }
            mCurPage.mCurShowId = mModule.CurShowIndex = id;
        }
       
        IsDirty = true;
        EventDelegate.Add(toggleOne.onChange, OnTabOne);
        EventDelegate.Add(toggleTwo.onChange, OnTabTwo);
        EventDelegate.Add(toggleThree.onChange, OnTabThree);
        EventDelegate.Add(TodoBtn.onClick, OnTodo);
        EventDelegate.Add(GoToBtn.onClick, OnTodo);
        EventSystem.Instance.addEventListener(QuestEvent.QUEST_ACCEPT, OnAccetped);
        EventSystem.Instance.addEventListener(FinishQuestEvent.QUEST_FINISHED, OnFinshed);
        EventSystem.Instance.addEventListener(QuestEvent.QUEST_UPDATE, OnUpdateQuest);
    }

    protected override void OnClose()
    {
        EventDelegate.Remove(toggleOne.onChange, OnTabOne);
        EventDelegate.Remove(toggleTwo.onChange, OnTabTwo);
        EventDelegate.Remove(toggleThree.onChange, OnTabThree);
        EventDelegate.Remove(TodoBtn.onClick, OnTodo);
        EventDelegate.Remove(GoToBtn.onClick, OnTodo);
        EventSystem.Instance.removeEventListener(QuestEvent.QUEST_ACCEPT, OnAccetped);
        EventSystem.Instance.removeEventListener(FinishQuestEvent.QUEST_FINISHED, OnFinshed);
        EventSystem.Instance.removeEventListener(QuestEvent.QUEST_UPDATE, OnUpdateQuest);

    }

    public void OnTabOne()
    {
        if (!UIToggle.current.value) return;
        mCurPage = mTabs[0];
        IsDirty = true;
        SoundManager.Instance.Play(5);
    }

    public void OnTabTwo()
    {
        if (!UIToggle.current.value) return;
        mCurPage = mTabs[1];
        SoundManager.Instance.Play(5);
        IsDirty = true;
    }

    public void OnTabThree()
    {
        if (!UIToggle.current.value) return;
        mCurPage = mTabs[2];
        IsDirty = true;
        SoundManager.Instance.Play(5);
    }

    //返回一个显示的任务id；

    private void OnQuestBtnClick()
    {
        mCurPage.mCurShowId = mModule.CurShowIndex;
        SoundManager.Instance.Play(15);
        IsDirty = true;
    }

    private void OnTodo()
    {
        WindowManager.Instance.CloseUI("quest");
        SoundManager.Instance.Play(15);
        mModule.DoQuest();
    }

    public void OnFinshed(EventBase evt)
    {
        var qe = evt as QuestEvent;
        if (!mQuestBtnMap.ContainsKey(qe.mQuestId)) return;
        var tempBtn = mQuestBtnMap[qe.mQuestId];
        mQuestBtnMap.Remove(qe.mQuestId);
        tempBtn.ResetBtn();
        IsDirty = true;
    }

    public void OnUpdateQuest(EventBase evt)
    {
        IsDirty = true;
    }

    public void OnAccetped(EventBase evt)
    {
        var qe = evt as QuestEvent;
        CreateQuestBtn(qe.mQuestId);
        mCurPage.QuestBtnGrid.repositionNow = true;
    }

    // Update is called once per frame
    public override void Update(uint elapsed)
    {
        if (IsDirty)
        {
            RefreshQuestList();            
            RefreshDetails();
            
            IsDirty = false;
        }
    }

    private void RefreshQuestList()
    {
        switch (mCurPage.mPageType)
        {
            case QuestType.Main:
                mCurPage.mQuestList = mQuestData.mMainQuest;
                break;
            //case QuestType.Side:
                //mCurPage.mQuestList = mQuestData.mSideQuest;
                //break;
            case QuestType.Daily:
                mCurPage.mQuestList = mQuestData.mDailyQuest;
                break;
        }

        for (int i = 0; i < mCurPage.mQuestList.Count; i++)
        {
            if (!mQuestBtnMap.ContainsKey(mCurPage.mQuestList[i].mId))
            {
                CreateQuestBtn(mCurPage.mQuestList[i].mId);
            }

            mQuestBtnMap[mCurPage.mQuestList[i].mId].IsSelected = false;
        }

        List<int> NeedDelete = new List<int>();
        foreach (var btn in mQuestBtnMap)
        {
            if (!mQuestData.IsAccepted(btn.Key))
            {
                NeedDelete.Add(btn.Key);
            }
        }

        for (int i = 0; i < NeedDelete.Count; i++)
        {
            mQuestBtnMap[NeedDelete[i]].ResetBtn();
            mQuestBtnMap.Remove(NeedDelete[i]);
        }

        if (mModule.CurShowIndex == -1 || (!mCurPage.mQuestList.Exists(x => (x.mId == mModule.CurShowIndex))))
        {
            if (mCurPage.mQuestList.Count > 0)
            {
                mModule.CurShowIndex = mCurPage.mQuestList[0].mId;
            }
            else
            {
                mModule.CurShowIndex = -1;
            }
        }

        mCurPage.QuestBtnGrid.repositionNow = true;
    }

    private void RefreshDetails()
    {
        if (mQuestBtnMap.ContainsKey(mModule.CurShowIndex) &&
            mQuestBtnMap[mModule.CurShowIndex].IsSelected == false)
        {
            mQuestBtnMap[mModule.CurShowIndex].IsSelected = true;
        }
        GameDebug.Log(mModule.CurShowIndex);
        if (mModule.CurShowIndex == -1)
        {
            questDetailObj.SetActive(false);
            return;
        }
        questDetailObj.SetActive(true);
        var quest = mModule.GetQuestById(mModule.CurShowIndex);
        QuestName.text = quest.mVO.questName;
        QuestScrib.text = quest.mVO.questScrib;
        QuestGoal.text = quest.mVO.goal;

        //0为寻路去关卡，1 为直接打开界面
        if (quest.GetTriggerType() == 0)
        {
            GoToBtn.gameObject.SetActive(true);
            TodoBtn.gameObject.SetActive(false);
        }
        else
        {
            GoToBtn.gameObject.SetActive(false);
            TodoBtn.gameObject.SetActive(true);
        }

        BtnGrid.repositionNow = true;

        //奖励
        QuestTableItem qti = DataManager.QuestTable[mModule.CurShowIndex] as QuestTableItem;
        if (qti == null)
        {
            GameDebug.LogError("无效任务id" + mModule.CurShowIndex);
            return;
        }
        if (AwardItemUIList != null)
        {
            foreach (var item in AwardItemUIList)
            {
                GameObject.DestroyImmediate(item.gameObject);
            }
            AwardItemUIList.Clear();
        }
        else
        {
            AwardItemUIList = new List<AwardItemUI>();
        }

        if (qti.itemType1 != -1)
        {
            AwardItemUI temp = new AwardItemUI(qti.itemType1, qti.itemNum1);
            AwardItemUIList.Add(temp);
            temp.gameObject.transform.parent = DropGrid.transform;
            temp.gameObject.transform.localScale = Vector3.one;
        }

        if (qti.itemType2 != -1)
        {
            AwardItemUI temp = new AwardItemUI(qti.itemType2, qti.itemNum2);
            AwardItemUIList.Add(temp);
            temp.gameObject.transform.parent = DropGrid.transform;
            temp.gameObject.transform.localScale = Vector3.one;
        }

        if (qti.itemType3 != -1)
        {
            AwardItemUI temp = new AwardItemUI(qti.itemType3, qti.itemNum3);
            AwardItemUIList.Add(temp);
            temp.gameObject.transform.parent = DropGrid.transform;
            temp.gameObject.transform.localScale = Vector3.one;
        }
        DropGrid.repositionNow = true;

        //进度
        Process.text = quest.mProcess + "/" +
                       ModuleManager.Instance.FindModule<PlayerDataModule>().GetQuestTotalProgress(quest.mId);

    }

    public void Clear()
    {
        foreach (KeyValuePair<int, QuestBtnUI> kvp in mQuestBtnMap)
        {
            kvp.Value.ResetBtn();
        }

        mQuestBtnMap.Clear();
    }

    protected override void OnDestroy()
    {
        Clear();
    }


    public GameObject FindEnterBtn(int param)
    {
        return TodoBtn.gameObject;
    }

    public int GetCurrentID()
    {
        if (mCurPage == null)
        {
            return -1;
        }
        return mCurPage.mCurShowId;
    }

}
