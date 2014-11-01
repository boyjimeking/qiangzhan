using System;
using System.Collections;
using System.Collections.Generic;
using Message;
using UnityEngine;

public enum ChallengeState
{
    Passed, //已通关
    NoPass, //今日未做
    Never, //从未做过
    Current, //即将挑战
}

public class ChallengeSweepParam
{
    public ChallengeSweepParam()
    {
        mDrops = new List<DropItemParam>();
    }

    //层数
    public int mFloor;
    public List<DropItemParam> mDrops;
}

public class DropItemParam
{
    public DropItemParam(int ResId, int Num)
    {
        mResId = ResId;
        mNum = Num;
    }

    public int mResId;
    public int mNum;
}

public class ChallengeModule : ModuleBase
{
    //public int MAX_FLOOR = 50;


    /// <summary>
    /// 是否可以扫荡
    /// </summary>
    /// <returns></returns>
    public bool IsSweep()
    {
        if (GetHistoryFloor() < 5)
            return false;

        //if (GetNeedSweepItemNum() > GetSweepItemNum())
           // return false;

        return true;
    }

    public int GetCurFloor()
    {
        return (int)PlayerDataPool.Instance.MainData.mChallengeStage.mCurrentFloor;
    }

    public int GetDoingFloor()
    {
        return (int)PlayerDataPool.Instance.MainData.mChallengeStage.mChallengingFloor;
    }

    public void SetDoingFloor(int floor)
    {
        PlayerDataPool.Instance.MainData.mChallengeStage.mChallengingFloor = (uint)floor;
    }

    protected override void OnEnable()
    {
  
    }

   
    public int GetHistoryFloor()
    {
        return (int)PlayerDataPool.Instance.MainData.mChallengeStage.mMaxFloor;
    }

   
    public bool IsPataGame
    {
        get
        {
            GameScene bs = SceneManager.Instance.GetCurScene() as GameScene;
            if (bs == null)
            {
                return false;
            }

            return SceneManager.GetSceneType(bs.GetSceneRes()) == SceneType.SceneType_Tower;
        }
    }
    //返回道具数量

    public int GetSweepItemNum()
    {
        int resId = ConfigManager.GetChallengeSweepNeedItemResID();
        return (int)ModuleManager.Instance.FindModule<PlayerDataModule>().GetItemNumByID(resId);
    }

    public String GetItemName()
    {
        return ItemManager.Instance.getItemName(ConfigManager.GetChallengeSweepNeedItemResID());
    }


    public void RequestSweep()
    {
        WindowManager.Instance.CloseUI("quickChallenge");
        Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_CHALLENGE_STAGE_SWEEP, null);
    }

    /// <summary>
    /// 挑战失败消息
    /// </summary>
    public void ChallengeFail()
    {
        WindowManager.Instance.OpenUI("challengeFail");
    }

    /// <summary>
    ///  根据层数判断是否能够扫荡
    /// </summary>
    /// <param name="FloorID"></param>
    /// <returns></returns>
    public bool IsSweepByFloorID(int floor)
    {
        return (floor >= GetCurFloor()) && (floor <= GetHistoryFloor()) && IsSweep();
    }

    /// <summary>
    /// 通过层数ID获得关卡数据表
    /// </summary>
    public Scene_StageSceneTableItem GetCheckPointDataByFloorId(int floorId)
    {
        var ct = DataManager.ChallengeTable[floorId] as ChallengeTableItem;
        if (ct == null) return null;
        return DataManager.Scene_StageSceneTable[ct.mStageId] as Scene_StageSceneTableItem;
    }

    /// <summary>
    /// 获取关卡状态
    /// </summary>
    /// <returns></returns>
    public ChallengeState GetChallengeState(int floor)
    {
        if (floor < GetCurFloor())
        {
            return ChallengeState.Passed;
        }

        if (floor == GetCurFloor())
        {
            return ChallengeState.Current;
        }

        if (floor >= GetCurFloor() && floor <= GetHistoryFloor())
        {
            return ChallengeState.NoPass;
        }

        return ChallengeState.Never;
    }

    /// <summary>
    /// 判段关卡是否可以挑战
    /// </summary>
    /// <param name="floor"></param>
    /// <returns></returns>
    public bool IsCanChallenge(int floor)
    {
        return GetCurFloor() >= floor;
    }

    /// <summary>
    /// 判读是否可以继续挑战
    /// </summary>
    /// <returns></returns>
    public bool IsCanContinue()
    {
        return GetCurFloor() <= PlayerChallengeData.MAX_FLOOR_COUNT;
    }

    public void ContinueChallenge()
    {
        int floor = GetDoingFloor();
        if (floor > GetCurFloor())
            return;

        ChallengeStageContinueActionParam param = new ChallengeStageContinueActionParam();
        param.Floor = (uint)floor;
        Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_CHALLENGE_STAGE_CONTINUE, param);
    }

    //继续挑战
    public void OnRespondContinue(int floor)
    {
        if (floor != GetDoingFloor())
            return;
        EventSystem.Instance.PushEvent(new BattleUIEvent(BattleUIEvent.BATTLE_UI_BOSSBLOOD_RESET));
        SceneManager.Instance.RestartTrigger("t" + floor);
        SceneManager.Instance.RestartTrigger("tf" + floor);
        var gameScene = SceneManager.Instance.GetCurScene() as GameScene;

        //重置运行时间
        gameScene.ResetLogicState();
        PlayerController.Instance.SetInitPos();
        SceneManager.Instance.GetCurScene().PlayBgSound();
        PlayerController.Instance.SetWudi(false);
        PlayerController.Instance.SetFreeze(false);
        WindowManager.Instance.OpenUI("challengecountdown");
    }

    public int GetFloorScore(int floor)
    {
        return (int)PlayerDataPool.Instance.MainData.mChallengeStage.mFloors[floor].mDayMaxScore;
    }

    public uint GetWeekScore()
    {
        return PlayerDataPool.Instance.MainData.mChallengeStage.mWeekScore;
    }

    public int GetRankVersion()
    {
        return PlayerDataPool.Instance.MainData.mChallengeStage.mRankVersion;
    }

    public List<RankingChallengeInfo> GetRankList()
    {
        return PlayerDataPool.Instance.MainData.mChallengeStage.mRankList;
    }
   
    /// <summary>
    /// 是否获得某个成就
    /// </summary>
    /// <param name="floor"></param>
    /// <param name="achieveIndex">0,1,2</param>
    /// <returns></returns>
    public bool IsGainAchievement(int floor, int achieveIndex)
    {
        switch(achieveIndex)
        {
            case 0:
                return PlayerDataPool.Instance.MainData.mChallengeStage.mFloors[floor].mAchieveOne;
            case 1:
                return PlayerDataPool.Instance.MainData.mChallengeStage.mFloors[floor].mAchieveTwo;
            case 2:
                return PlayerDataPool.Instance.MainData.mChallengeStage.mFloors[floor].mAchieveThree;
        }

        return false;
    }

    public ChallengeTableItem GetChallengeTableItem(int floor)
    {
        return DataManager.ChallengeTable[floor] as ChallengeTableItem;
    }

    public void OpenUIByFloor(int floor)
    {
        WindowManager.Instance.OpenUI("challenge");
    }

    public void ChallengeFloor(int floor)
    {
        SetDoingFloor(floor);

        ChallengeStageEnterStageActionParam param = new ChallengeStageEnterStageActionParam();
        param.Floor = (uint)GetDoingFloor();
        Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_CHALLENGE_STAGE_BEGIN, param);
    }

    public void RequestRankList()
    {
        Net.Instance.DoAction((int)MESSAGE_ID.ID_MSG_CHALLENGE_RANK, PlayerDataPool.Instance.MainData.mChallengeStage.mRankVersion);
    }

    public uint GetChaRankNum()
    {
        return PlayerDataPool.Instance.MainData.mChallengeStage.mRankNum;
    }

    public int GetRankAwardId(uint score, uint rankNum)
    {
        if (rankNum >= ChallengeDefine.Rank_Num) return -1;

        IDictionaryEnumerator itr = DataManager.ChaRankAwardTable.GetEnumerator();
        while( itr.MoveNext() )
        {
            ChallengeRankAwardTableItem res = itr.Value as ChallengeRankAwardTableItem;
            if (res == null)
            {
                GameDebug.LogError("无效challenge_rank_award 表id");
                return -1;
            }
            if (score <= res.max_score && score >= res.min_score)
            {
                return res.rank_list[rankNum - 1].itemid;
            }
        }

//         foreach (DictionaryEntry de in DataManager.ChaRankAwardTable)
//         {
//             var res = de.Value as ChallengeRankAwardTableItem;
//             if (res == null)
//             {
//                 GameDebug.LogError("无效challenge_rank_award 表id");
//                 return -1;
//             }
//             if (score <= res.max_score && score >= res.min_score)
//             {
//                 return res.rank_list[rankNum - 1].itemid;
//             }
//         }

        return -1;
    }

    public bool IsInRankList(GUID guid)
    {
       var mranklist = PlayerDataPool.Instance.MainData.mChallengeStage.mRankList;
        for (int i = 0; i < mranklist.Count; i++)
        {
            if (mranklist[i].guid.ToULong() == guid.ToULong())
            {
                return true;
            }            
        }
        return false;
    }

    //战斗力是否充足
    public bool IsBattleGradeEnough(int floor,out int recom_grade, out int  my_grade)
    {
        ChallengeTableItem res = DataManager.ChallengeTable[floor] as ChallengeTableItem;
         recom_grade = res.mRecomBattleScore;
        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
         my_grade = (int)pdm.GetGrade();
         return my_grade >= recom_grade;
            
    }
}


