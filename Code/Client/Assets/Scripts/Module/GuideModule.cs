using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class GuideModule : ModuleBase
{
    override protected void OnEnable()
    {
        EventSystem.Instance.addEventListener(FinishQuestEvent.QUEST_FINISHED, OnQuestFinished);
        EventSystem.Instance.addEventListener(QuestEvent.QUEST_ACCEPT, OnQuestAccept);
        EventSystem.Instance.addEventListener(StagePassServerEvent.STAGE_PASS_SERVER_EVENT, OnStagePass);

    }

    override protected void OnDisable()
    {
        EventSystem.Instance.removeEventListener(FinishQuestEvent.QUEST_FINISHED, OnQuestFinished);
        EventSystem.Instance.removeEventListener(QuestEvent.QUEST_ACCEPT, OnQuestAccept);
        EventSystem.Instance.removeEventListener(StagePassServerEvent.STAGE_PASS_SERVER_EVENT, OnStagePass);
    }

    public void ForceHideGuide(bool hide)
    {
        GuideManager.Instance.ForceHide(hide);
    }

    private void OnStagePass( EventBase e )
    {
        StagePassServerEvent evt = (StagePassServerEvent)e;
        if( evt.mStageData != null )
        {
            GuideManager.Instance.OnStageComplete(evt.mStageData.stageid);
        }
    }

    private void OnQuestFinished( EventBase e )
    {
        FinishQuestEvent evt = (FinishQuestEvent)e;
        GuideManager.Instance.OnQuestComplete(evt.mQuestId);
    }

    private void OnQuestAccept( EventBase e )
    {
        QuestEvent evt = (QuestEvent)e;
        GuideManager.Instance.OnQuestAccept(evt.mQuestId);
    }

    public void OnFunctionUnlock(int functionid)
    {
        GuideManager.Instance.OnOpenFunction(functionid);
    }

    public void EnterTrigger(int guideid)
    {
        GuideManager.Instance.OnEnterTrigger(guideid);
    }
}
