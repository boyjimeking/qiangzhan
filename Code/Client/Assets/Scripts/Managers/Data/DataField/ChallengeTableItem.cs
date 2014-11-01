
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

  public  class ChallengeTableItem
  {
      //id就是层数
      public int mId;
      //关联Scene_StageSceneTableItem中的ID
      public int mStageId;
      //推荐战力
      public int mRecomBattleScore;
      //提示信息
      public string mTip;
      //掉落界面tip
      public string mDropTip;

      //通关积分
      public int mFloorScore;

      //神速通关积分
      public int mAchieveScoreOne;

      //快速通关积分
      public int mAchieveScoreTwo;

      //龟速通关积分
      public int mAchieveScoreThree;

      //获得成就条件
      public int mAchieveConditionOne;

      //获得成就参数
      public int mAchieveParamOne;

      //获得成就条件
      public int mAchieveConditionTwo;

      //获得成就参数
      public int mAchieveParamTwo;

      //获得成就条件
      public int mAchieveConditionThree;

      //获得成就参数
      public int mAchieveParamThree;

      public uint mFlittingPos;
      public int mFlitingID;

      public int mFirstAwardItemId;
      public int mEveryDayAwardItemId;

      public int mFirstDropGroupId;
      public int mEvertDayDropGroupId;

  }

