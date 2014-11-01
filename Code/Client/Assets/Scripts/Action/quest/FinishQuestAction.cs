using System;
using System.Collections.Generic;
using Message;

public class FinishQuestAction : LogicAction<request_finish_quest, respond_finish_quest>
{
    public FinishQuestAction()
        :base((int)MESSAGE_ID.ID_MSG_QUEST_FINISH)
    {

    }

    protected override void OnRequest(request_finish_quest request, object userdata)
    {
        request.questid = Convert.ToInt32(userdata);
        GameDebug.Log("请求完成任务： " +request.questid);
    }

    protected override void OnRespond(respond_finish_quest respond, object userdata)
    {
        if(respond.result == (int)ERROR_CODE.ERR_QUEST_FINISH_OK)
        {
            FinishQuestEvent evt = new FinishQuestEvent(FinishQuestEvent.QUEST_FINISHED);
            if (respond.awards != null)
            {
                evt.mAwardInfo = respond.awards;
            }
            evt.mQuestId = Convert.ToInt32(userdata);
            PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
            pdm.FinishQuest(evt.mQuestId);
            //GameDebug.Log("任务完成" + evt.mQuestId);

            if (!QuestHelper.IsInFightScene() && !QuestHelper.IsLoading())
            {
              
                EventSystem.Instance.PushEvent(evt);              
            }
            else
            {
                GameDebug.Log("完成任务缓存" + evt.mQuestId);
                QuestModule qm = ModuleManager.Instance.FindModule<QuestModule>();
                qm.mEventCache.Enqueue(evt);
            }
        }
        else
        {
           // GameDebug.Log("任务完成失败");
        }
    }
}

