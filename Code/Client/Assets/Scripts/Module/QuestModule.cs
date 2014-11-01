using System;
using System.Collections;
using System.Collections.Generic;
using Message;
using UnityEngine;



public class QuestModule : ModuleBase
{

    //当前显示的任务信息
    private int mCurShowIndex = -1;
    private bool mIsInit = false;
    private QuestData mQuestData;
    private List<int> mNetCache;
    public Queue<EventBase> mEventCache;

    public int CurShowIndex
    {
        get { return mCurShowIndex; }
        set
        {
            mCurShowIndex = value;
        }
    }

    protected override void OnEnable()
    {
        mNetCache = new List<int>();
        mEventCache = new Queue<EventBase>();
        EventSystem.Instance.addEventListener(EnterGameEvent.ENTER_GAME, OnEnterGame);
        EventSystem.Instance.addEventListener(QuestEvent.QUEST_ACCEPT, OnAcceptQuestResponse);
        EventSystem.Instance.addEventListener(FinishQuestEvent.QUEST_FINISHED, OnQuestFinishResponse);
        EventSystem.Instance.addEventListener(SceneLoadEvent.SCENE_LOAD_COMPLETE, OnSceneLoadCall);
        EventSystem.Instance.addEventListener(QuestEvent.Quest_SYNC_SERVER_EVENT, OnSyncServer);
        EventSystem.Instance.addEventListener(PropertyEvent.MAIN_PROPERTY_CHANGE, OnMainPropertyChange);
        EventSystem.Instance.addEventListener(StoryEvent.STORY_END, OnStoryEnd);
    }

   
    protected override void OnDisable()
    {
        EventSystem.Instance.removeEventListener(EnterGameEvent.ENTER_GAME, OnEnterGame);
        EventSystem.Instance.removeEventListener(QuestEvent.QUEST_ACCEPT, OnAcceptQuestResponse);
        EventSystem.Instance.removeEventListener(FinishQuestEvent.QUEST_FINISHED, OnQuestFinishResponse);
        EventSystem.Instance.removeEventListener(SceneLoadEvent.SCENE_LOAD_COMPLETE, OnSceneLoadCall);
        EventSystem.Instance.removeEventListener(QuestEvent.Quest_SYNC_SERVER_EVENT, OnSyncServer);
        EventSystem.Instance.removeEventListener(PropertyEvent.MAIN_PROPERTY_CHANGE, OnMainPropertyChange);
        EventSystem.Instance.removeEventListener(StoryEvent.STORY_END, OnStoryEnd);
    }

    private void OnStoryEnd(EventBase evt)
    {
        GameDebug.Log("OnStoryEnd");
        PlayerController.Instance.QuestMoveCheck();
    }


    private void OnSyncServer(EventBase evt)
    {
        GameDebug.Log("同步任务数据检查任务条件");
        CheckCondition();
    }

    private void OnMainPropertyChange(EventBase evt)
    {
        if (PlayerDataPool.Instance.MainData.mQuestData.IsInit)
        {
            if (mQuestData == null) return;
           // GameDebug.Log("主属性变更，检查条件");
            CheckCondition();
        }
    }

    private void OnAcceptQuestResponse(EventBase evt)
    {
        var qevt = evt as QuestEvent;
        mNetCache.Remove(qevt.mQuestId);
        GameDebug.Log("OnAcceptQuestResponse" + qevt.mQuestId);
        CheckCondition();
    }


    private void OnQuestFinishResponse(EventBase evt)
    {
        FinishQuestEvent qevt = evt as FinishQuestEvent;
        mNetCache.Remove(qevt.mQuestId);        
        if (qevt.mAwardInfo != null)
        {
            QuestAwardData questAward = new QuestAwardData();
            questAward.mQuestId = qevt.mQuestId;
            for (int i = 0; i < qevt.mAwardInfo.award_item.Count; i++)
            {
                QuestAwardItem temp = new QuestAwardItem();
                temp.mResId = qevt.mAwardInfo.award_item[i].award_id;
                temp.mNum = qevt.mAwardInfo.award_item[i].award_count;
                questAward.mAwardList.Add(temp);

            }
            mQuestData.AddAward(questAward);
            OpenQuestAwardUI();

        }

        CheckCondition();

    }

    private void OnEnterGame(EventBase evt)
    {
        mQuestData = PlayerDataPool.Instance.MainData.mQuestData;
        GameDebug.Log("进入游戏检查任务条件");
        CheckCondition();
        WorldMapModule module = ModuleManager.Instance.FindModule<WorldMapModule>();
        if (null == module)
            return;

        module.initWorldMapData();
    }

    private void CheckCondition()
    {
        IDictionaryEnumerator  itr = DataManager.QuestTable.GetEnumerator();
        while(itr.MoveNext())
        {
            QuestTableItem qti = itr.Value as QuestTableItem;
            if (qti == null || mQuestData == null)
            {
                return;
            }
            //已经完成的不用检测条件
            if (mQuestData.IsFinished(qti.id))
                continue;

            if (qti.questType == (int)QuestType.Daily)
                continue;

            if (mNetCache.Contains(qti.id))
                continue;

            //已经接取且没有完成的检测完成条件
            if (mQuestData.IsAccepted(qti.id))
            {
                if (QuestHelper.IsCompleteCondition(qti.id))
                {
                    RequestFinish(qti.id);
                }
            }
            else
            {
                //检查上限值
                if (!mQuestData.IsMaxAcceptCount())
                {
                    if (QuestHelper.IsPrediction(qti.id))
                    {
                        RequestAccept(qti.id);
                    }
                }
            }
        }
 
    }

    public void RequestAccept(int questId)
    {
        mNetCache.Add(questId);
        Net.Instance.DoAction((int) Message.MESSAGE_ID.ID_MSG_QUEST_ACCPET, questId);
    }

    public void RequestFinish(int questId)
    {
        mNetCache.Add(questId);
        Net.Instance.DoAction((int) Message.MESSAGE_ID.ID_MSG_QUEST_FINISH, questId);
    }

    public Quest GetQuestById(int questId)
    {
        return mQuestData.GetQuestById(questId);
    }

    public void DoQuest()
    {
        GetQuestById(mCurShowIndex).Todo();
    }

    public bool ShowNextAward()
    {
        if (mQuestData.HasAwardCache())
        {
            WindowManager.Instance.OpenUI("questAward");
            return true;
        }
        return false;
    }

    public void OpenQuestAwardUI()
    {
        if (QuestHelper.IsOpeningSceneAward()) return;
        if (mQuestData.HasAwardCache())
        {
           // WindowManager.Instance.OpenUI("questAward");
            WindowManager.Instance.QueueOpenUI("questAward");
        }

    }

    private void OnSceneLoadCall(EventBase evt)
    {
        if (mEventCache.Count < 0) return;
        if (!QuestHelper.IsInFightScene() && !QuestHelper.IsLoading())
        {
            //Debug.Log(" mEventCache.Count" +mEventCache.Count);
            
            
            for (; mEventCache.Count>0;)
            {
                EventBase qevt = mEventCache.Dequeue();
                //Debug.Log(qevt.mEventName);
                EventSystem.Instance.PushEvent(qevt);
            }
        }
    }

    public void OpenQuestUIById(int questid)
    {
        if (mQuestData.IsAccepted(questid))
        {
            WindowManager.Instance.OpenUI("quest", questid);
        }
        else
        {
            GameDebug.LogError("任务没有接");
        }
       
    }


}

