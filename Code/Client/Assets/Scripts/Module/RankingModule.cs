using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Message;
public class RankingInfo
{
    public int rank;
    public string name;
    public GUID guid;
    public int level;
    public uint grade;
	public uint score = 0;
}
class RankingModule : ModuleBase
{
    private RankingInfo[] mGrades;
    private RankingInfo[] mLevels;
    private RankingInfo[] mRanks;
    private RankingInfo[] mArenas;


    private int mSelfGrade = -1;
    private int mSelfLevel = -1;
    private int mSelfRank = -1;
    private int mSelfArena = -1;


    private int mVersion = 0;
    public RankingModule()
    {
        
    }
    public RankingInfo GetSelfGrade()
    {
        if (mGrades == null)
            return null;
        if (mSelfGrade < 0 || mSelfGrade >= mGrades.Length)
            return null;
        return mGrades[mSelfGrade];
    }
    public RankingInfo GetSelfLevel()
    {
        if (mLevels == null)
            return null;
        if (mSelfLevel < 0 || mSelfLevel >= mLevels.Length)
            return null;
        return mLevels[mSelfLevel];
    }

    public RankingInfo GetSelfRank()
    {
        if (mRanks == null)
            return null;
        if (mSelfRank < 0 || mSelfRank >= mRanks.Length)
            return null;
        return mRanks[mSelfRank];
    }

    public RankingInfo GetSelfArena()
    {
        if (mArenas == null)
            return null;
        if (mSelfArena < 0 || mSelfArena >= mArenas.Length)
            return null;
        return mArenas[mSelfArena];
    }

    public RankingInfo[] GetRankingGrade()
    {
        return mGrades;
    }
    public RankingInfo[] GetRankingLevel()
    {
        return mLevels;
    }
    public RankingInfo[] GetRankingRank()
    {
        return mRanks;
    }
    public RankingInfo[] GetRankingArena()
    {
        return mArenas;
    }

    //向服务器请求排行信息
    public void RequestRanking()
    {
        RankingActionParam param = new RankingActionParam();
        param.version = mVersion;
        Net.Instance.DoAction((int)MESSAGE_ID.ID_MSG_RANKING, null);
    }

    public void SyncRankingList(respond_ranking_list msg)
    {
        if( mVersion == msg.version )
        {
            return;
        }
        mVersion = msg.version;

        PlayerDataModule dataModule = ModuleManager.Instance.FindModule<PlayerDataModule>();
        mSelfGrade = mSelfArena = mSelfLevel = mSelfRank = -1;

        mGrades = null;
        mLevels = null;
        mRanks = null;
        mArenas = null;

        if( msg.grade_ranking != null && msg.grade_ranking.Count > 0 )
        {
            mGrades = new RankingInfo[msg.grade_ranking.Count];
            //mGrades. = msg.grade_ranking.Count;
            for( int i = 0 ; i < msg.grade_ranking.Count ; ++i )
            {
                RankingInfo info = new RankingInfo();
                info.rank = msg.grade_ranking[i].rank;
                info.name = msg.grade_ranking[i].name;
                info.guid = (GUID)(msg.grade_ranking[i].guid);
                info.level = msg.grade_ranking[i].level;
                info.grade = (uint)msg.grade_ranking[i].grade;

                mGrades[info.rank] = info;

                //mGrades.Add(rank, info);

                if( info.name == dataModule.GetName() )
                {
                    mSelfGrade = info.rank;
                }

            }
        }
        if (msg.level_ranking != null && msg.level_ranking.Count > 0)
        {
            mLevels = new RankingInfo[msg.level_ranking.Count];

            for (int i = 0; i < msg.level_ranking.Count; ++i)
            {
                RankingInfo info = new RankingInfo();
                info.rank = msg.level_ranking[i].rank;
                info.guid = (GUID)(msg.level_ranking[i].guid);
                info.name = msg.level_ranking[i].name;
                info.level = msg.level_ranking[i].level;
                info.grade = (uint)msg.level_ranking[i].grade;

                mLevels[info.rank] = info;

                if (info.name == dataModule.GetName())
                {
                    mSelfLevel = info.rank;
                }
            }
        }

		if(msg.arena_ranking != null && msg.arena_ranking.Count > 0)
		{
            mArenas = new RankingInfo[msg.arena_ranking.Count];

			for (int i = 0; i < msg.arena_ranking.Count; ++i)
			{
				RankingInfo info = new RankingInfo();
                info.rank = msg.arena_ranking[i].rank;
				info.name = msg.arena_ranking[i].name;
                info.guid = (GUID)(msg.arena_ranking[i].guid);

				info.level = msg.arena_ranking[i].level;
                info.grade = (uint)msg.arena_ranking[i].grade;
				info.score = msg.arena_ranking[i].arena_score;

                mArenas[info.rank] = info;
                if (info.name == dataModule.GetName())
                {
                    mSelfArena = info.rank;
                }
			}
		}

		if (msg.qualifying_ranking != null && msg.qualifying_ranking.Count > 0)
		{
            mRanks = new RankingInfo[msg.qualifying_ranking.Count];

			for (int i = 0; i < msg.qualifying_ranking.Count; ++i)
			{
				RankingInfo info = new RankingInfo();
                info.rank = msg.qualifying_ranking[i].rank;
				info.name = msg.qualifying_ranking[i].name;
                info.guid = (GUID)(msg.qualifying_ranking[i].guid);

				info.level = msg.qualifying_ranking[i].level;
                info.grade = (uint)msg.qualifying_ranking[i].grade;

                mRanks[info.rank] = info;
                if (info.name == dataModule.GetName())
                {
                    mSelfRank = info.rank;
                }
			}
		}

        RankingEvent evt = new RankingEvent(RankingEvent.RANKING_UPDATE);
        EventSystem.Instance.PushEvent(evt);
    }

    override protected void OnEnable()
    {

    }

    override protected void OnDisable()
    {

    }
}
