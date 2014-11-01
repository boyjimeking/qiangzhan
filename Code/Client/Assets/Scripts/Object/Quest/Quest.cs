using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class Quest : IComparable
{
    
    public QuestState mState;
    public readonly QuestTableItem mVO;
    /// <summary>
    /// ID
    /// </summary>
    public int mId;
    /// <summary>
    /// 完成进度
    /// </summary>
    public int mProcess;


    private QuestTriggerEvent mTrigger;

	public int mType;
    // 0 打开界面，1是移动到指定点
    public int mTriggerType;
    public Quest(int questid)
    {
        mVO = DataManager.QuestTable[questid] as QuestTableItem;
        if (mVO == null)
        {
            GameDebug.LogError("无效的任务id");
            return;
        }
        mTrigger = QuestTriggerEvent.Parse(mVO.triggerEvent);
        mId = mVO.id;
		mType = mVO.questType;
		mState = QuestState.Accepted;
        mProcess = 0;
    }

    public void Todo()
    {
        if (mTrigger != null)
        {
            mTrigger.trigger();
        }
    }

    public int GetTriggerType()
    {
        return mTrigger.triggerType;
    }

    public int CompareTo(object obj)
    {
         int res = 0;
        Quest qobj = (Quest) obj;
        if (mType < qobj.mType)
        {
            res = -1;
        }
        else if (mType > qobj.mType)
        {
            res = 1;
        }
        else
        {
            if (mId < qobj.mId)
            {
                res = -1;
            }else if (mId > qobj.mId)
            {
                res = 1;
            }

        }

        return res;
    }
}

public class QuestTriggerEvent
{
    public  int triggerType;
    public virtual void trigger()
    {
        
    }

    public static QuestTriggerEvent Parse(string param)
    {
       var strs= param.Split('_');
        
        switch (strs[0])
        {
            case "open":
                var uitrigger = new OpenUITriggerEvent();
                uitrigger.mUIName = strs[1];
                uitrigger.mParamOne = Convert.ToInt32(strs[2]);
                uitrigger.triggerType = 0;
               
                return uitrigger;
            case "npc":
                var moveTrigger = new MovePosTriggerEvent();
                moveTrigger.x = Convert.ToSingle(strs[1]);
                moveTrigger.z = Convert.ToSingle(strs[2]);
                moveTrigger.mapId = Convert.ToInt32(strs[3]);
                moveTrigger.stageId = Convert.ToInt32(strs[4]);
                moveTrigger.triggerType = 1;
                return moveTrigger;

        }

        return new QuestTriggerEvent();
    }
}

public class OpenUITriggerEvent : QuestTriggerEvent
{
    public string mUIName;
    //界面索引
    public int mParamOne;
    public int mParamTwo;
    public override void trigger()
    {
        if (mUIName == "tower")
        {
            ChallengeModule module = ModuleManager.Instance.FindModule<ChallengeModule>();
            module.OpenUIByFloor(mParamOne);           
        }
        {
            WindowManager.Instance.OpenUI(mUIName);
        }

        
        
             
    }
}

public class MovePosTriggerEvent : QuestTriggerEvent
{
    public float x;
    public float z;

    public int mapId;
    public int stageId;
    public override void trigger()
    {
        ModuleManager.Instance.FindModule<StageListModule>().mCurStageId = stageId;
       PlayerController.Instance.MoveTargetMap(x,z,mapId);
    }
}


 

