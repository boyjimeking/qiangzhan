using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Message;
using UnityEngine;

public class QuestConditionParam
{
    public int mConditionType;
    public int mArg1;
    public int mArg2;
    public int mValue;
  
}

public class QuestHelper
{
    /// <summary>
    ///判断是否符合接取的前提条件
    /// </summary>
    /// <returns></returns>
    public static bool IsPrediction(int questId)
    {
        var vo = DataManager.QuestTable[questId] as QuestTableItem;
        return (CheckPreCondition(1, vo) && CheckPreCondition(2, vo) && CheckPreCondition(3, vo));

    }

    /// <summary>
    /// 完成条件判断
    /// </summary>
    /// <param name="questId"></param>
    /// <returns></returns>
    public static bool IsCompleteCondition(int questId)
    {
        var vo = DataManager.QuestTable[questId] as QuestTableItem;
        return CheckCompleteCondition(1, vo) && CheckCompleteCondition(2, vo);
    }

    /// <summary>
    /// 判断单个前提条件
    /// </summary>
    /// <param name="num"></param>
    /// <param name="mVO"></param>
    /// <returns></returns>
    private static bool CheckPreCondition(int num, QuestTableItem mVO)
    {
        if (mVO.questType == QuestType.Daily) return false;
        var qcp = new QuestConditionParam();
        switch (num)
        {
            case 1:
                qcp.mConditionType = mVO.precondition1;
                qcp.mArg1 = mVO.precondition1Argu1;
                qcp.mArg2 = mVO.precondition1Argu2;
                qcp.mValue = mVO.precondition1Argu3;
                break;
            case 2:
                qcp.mConditionType = mVO.precondition2;
                qcp.mArg1 = mVO.precondition2Argu1;
                qcp.mArg2 = mVO.precondition2Argu2;
                qcp.mValue = mVO.precondition2Argu3;
                break;
            case 3:
                qcp.mConditionType = mVO.precondition3;
                qcp.mArg1 = mVO.precondition3Argu1;
                qcp.mArg2 = mVO.precondition3Argu2;
                qcp.mValue = mVO.precondition3Argu3;
                break;

        }
        
        if (qcp.mConditionType != -1)
        {
            return CheckCondition(mVO.id, qcp);
        }

        return true;

    }

    public static string GetQuestBtnName(int questId, int questType)
    {
        string re = "";
        switch (questType)
        {
            case QuestType.Main:
                re = "a" + questId;
                break;
            case QuestType.Side:
                re = "b" + questId;
                break;
            case QuestType.Daily:
                re = "c" + questId;
                break;
        }
        return re;
    }


    private static bool CheckCondition(int questid, QuestConditionParam qcp)
    {
        uint rst = UInt32.MaxValue;
        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
        switch (qcp.mConditionType)
        {
            case QuestConditionType.Level:
                rst = (uint) pdm.GetLevel();
                break;

            case QuestConditionType.Money_:
                rst = pdm.GetProceeds((ProceedsType) qcp.mArg1);
                break;

            case QuestConditionType.Exp:
                rst = (uint) pdm.GetExp();
                break;

            case QuestConditionType.Item:
                rst = 0;
                break;

            case QuestConditionType.Stage_Unlock:
                rst = Convert.ToUInt32(pdm.IsStageUnlock(qcp.mArg1) ? 1 : 0);
                break;

            case QuestConditionType.Stage_Grade:
                rst = (uint) pdm.GetStageGrade(qcp.mArg1);
                break;

            case QuestConditionType.Battle_Score:
                rst = pdm.GetGrade();
                break;

            case QuestConditionType.Wing_Level:
                rst = pdm.GetWingLevel(qcp.mArg1);
                break;

            case QuestConditionType.Cost_Diamond:
                rst = pdm.GetRMBUsed();
                break;

            case QuestConditionType.Passted_Stage:
                rst = pdm.GetStagePassTimes(qcp.mArg1);
                if (rst > 0)
                {
                    GameDebug.Log("Passted_Stage:" + rst);
                }
                break;

            case QuestConditionType.Passted_Zone:
                rst = pdm.GetZonePassTimes(qcp.mArg1);
                break;
                
            case QuestConditionType.Quest:

                rst = Convert.ToUInt32(pdm.IsQuestFinish(qcp.mArg1) ? 1 : 0);
                break;
            case QuestConditionType.Top_Level:
            {
                return pdm.GetLevel() <= qcp.mValue;
            }
            
            case QuestConditionType.Tower_Floor:
            {
               GameDebug.Log("Tower_Floor"+pdm.GetChallengMaxFloor());
                return pdm.GetChallengMaxFloor() >= qcp.mArg1;
            }
            default:
            {
                int totalProgress = pdm.GetQuestTotalProgress(questid);
                int progress = pdm.getQuestProgress(questid);
                if (totalProgress != 0 && progress != 0)
                {
                    if (totalProgress == progress)
                    {
                        return true;
                    }
                    if (totalProgress < progress)
                    {
                        GameDebug.LogError("任务当前进度大于总进度 任务ID = " + questid);
                        return false;
                    }
                }
            }
                break;
        }

        if (UInt32.MaxValue == rst || qcp.mValue < 0)
        {
            return false;
        }
        return rst >= (uint) qcp.mValue;
    }


    private static bool CheckCompleteCondition(int num, QuestTableItem vo)
    {
        var qcp = new QuestConditionParam();

        switch (num)
        {
            case 1:
                qcp.mConditionType = vo.completeCondition1;
                qcp.mArg1 = vo.completeCondition1Argu1;
                qcp.mArg2 = vo.completeCondition1Argu2;
                qcp.mValue = vo.completeCondition1Argu3;
                break;
            case 2:
                qcp.mConditionType = vo.completeCondition2;
                qcp.mArg1 = vo.completeCondition2Argu1;
                qcp.mArg2 = vo.completeCondition2Argu2;
                qcp.mValue = vo.completeCondition2Argu3;
                break;
        }

        if (qcp.mConditionType != -1)
        {
            return CheckCondition(vo.id, qcp);
        }

        return true;
    }

    /// <summary>
    /// 判断玩家是否在战斗场景
    /// </summary>
    /// <returns></returns>
    public static bool IsInFightScene()
    {
        return !(SceneManager.Instance.GetCurScene() is CityScene);
    }

    /// <summary>
    /// 判断是否在加载读条
    /// </summary>
    /// <returns></returns>
    public static bool IsLoading()
    {
        return WindowManager.Instance.IsOpen("loading");
    }

    /// <summary>
    /// 关卡A的结算界面是否在打开状态
    /// </summary>
    /// <returns></returns>

    public static bool IsOpeningSceneAward()
    {
        return WindowManager.Instance.IsOpen("questAward");
        return false;
    }

    public static bool CheckCondition(int conditionid)
    {
        ConditionTableItem cti = DataManager.ConditionTable[conditionid] as ConditionTableItem;
        QuestConditionParam qcp = new QuestConditionParam();
        qcp.mConditionType = (int)cti.mType;
        qcp.mArg1 = cti.mParam1;
        qcp.mArg2 = cti.mParam2;
        qcp.mValue = cti.mValue;
        return CheckCondition(-1, qcp);
    }
}

