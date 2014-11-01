using System;
using System.Collections.Generic;
using Message;

public class AcceptQuestAction : LogicAction<request_accept_quest, respond_accept_quest>
{
    public AcceptQuestAction()
        :base((int)MESSAGE_ID.ID_MSG_QUEST_ACCPET)
    {

    }

    protected override void OnRequest(request_accept_quest request, object userdata)
    {
        request.questid = Convert.ToInt32(userdata);
       // GameDebug.Log("请求接受任务:" + request.questid);
    }

    protected override void OnRespond(respond_accept_quest respond, object userdata)
    {
        if(respond.result==(int) ERROR_CODE.ERR_QUEST_ACCEPT_OK)
        {

            QuestEvent evt = new QuestEvent(QuestEvent.QUEST_ACCEPT);
            evt.mQuestId = Convert.ToInt32(userdata);
            PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
            pdm.AcceptQuest(evt.mQuestId);

            if (QuestHelper.IsInFightScene() || QuestHelper.IsLoading())
            {
                GameDebug.Log("接取任务缓存" + evt.mQuestId);
                QuestModule qm = ModuleManager.Instance.FindModule<QuestModule>();
                qm.mEventCache.Enqueue(evt);
            }
            else
            {
              
                EventSystem.Instance.PushEvent(evt);
            }
           
           //GameDebug.Log("回复接受任务:" + Convert.ToInt32(userdata));
        }
    }
}

