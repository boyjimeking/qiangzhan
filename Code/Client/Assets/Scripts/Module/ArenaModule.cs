using Message;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;
public class arena_simple_s
{
	public GUID guid;
	public string name;
	public uint level;
	public uint job;
	public uint sex;
	public uint grade;
	public uint weaponid;
	public uint rank;
}

public class arena_record_s
{
	public string src_name;
	public string tar_name;
	public int src_level;
	public int tar_level;
	public int src_job;
	public int tar_job;
	public int result;
	public ulong time_stamp;
}

public class ArenaModule : ModuleBase
{
	public static readonly int MAX_FIGHTER_COUNT = 3;
	public static readonly int MAX_RECORD_COUNT = 10;

	// 对手数据
	private List<arena_simple_s> mSimpleData = new List<arena_simple_s>();

	// 战绩数据
	private List<arena_record_s> mRecordData = new List<arena_record_s>();

	private GhostData mGhost = new GhostData();
	private GUID mGhostGUID = null;

	private PlayerDataModule mDataModule = ModuleManager.Instance.FindModule<PlayerDataModule>();

	private int mAwardScore = 0;
	private int mAwardPoint = 0;
	private bool mWin = false;

	private DateTime mLastRequestList = DateTime.MaxValue;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    public CropsItemInfo MainCropsInfo
    {
        get
        {
            return mGhost.mMainCropsInfo;
        }
    }

    public CropsItemInfo SubCropsInfo
    {
        get
        {
            return mGhost.mSubCropsInfo;
        }
    }

	public int AwardScore
	{
		get
		{
			return mAwardScore;
		}

		set
		{
			mAwardScore = value;
		}
	}

	public int AwardPoint
	{
		get
		{
			return mAwardPoint;
		}

		set
		{
			mAwardPoint = value;
		}
	}

	public bool Win
	{
		get
		{
			return mWin;
		}

		set
		{
			mWin = value;
		}
	}

	// 请求竞技场数据
	public void RequestArenaData()
	{
		// 不到10秒钟 用上次请求的列表
		if (mLastRequestList != DateTime.MaxValue)
		{
			TimeSpan tp = DateTime.Now - mLastRequestList;
			if (tp.TotalMilliseconds < 10000)
			{
				EventSystem.Instance.PushEvent(new ArenaEvent(ArenaEvent.RECEIVE_REFRESH_DATA));
				return;
			}
		}

		mLastRequestList = DateTime.Now;

		Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_ARENA_REFRESH, null);
	}

	// 购买次数
	public void RequestBuyTimes()
	{
		Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_ARENA_BUYTIMES, null);
	}

	// 请求挑战
	public void RequestBegin(int index)
	{
		if (index < 0 || index >= mSimpleData.Count)
			return;

		if(GetTimer(mDataModule.GetArenaLastTime()) > float.Epsilon)
		{
			PopTipManager.Instance.AddNewTip(StringHelper.GetErrorString(ERROR_CODE.ERR_ARENA_BEGIN_FAILED_CD, FontColor.Red));
			//PromptUIManager.Instance.AddNewPrompt(StringHelper.GetErrorString(ERROR_CODE.ERR_ARENA_BEGIN_FAILED_CD));
			EventSystem.Instance.PushEvent(new ArenaEvent(ArenaEvent.UI_ARENA_BEGIN_FAILED_CD));
			return;
		}

		if(mDataModule.GetArenaLeftTimes() == 0)
		{
			PopTipManager.Instance.AddNewTip(StringHelper.GetErrorString(ERROR_CODE.ERR_ARENA_BEGIN_FAILED_NOTIMES, FontColor.Red));
			//PromptUIManager.Instance.AddNewPrompt(StringHelper.GetErrorString(ERROR_CODE.ERR_ARENA_BEGIN_FAILED_NOTIMES));
			EventSystem.Instance.PushEvent(new ArenaEvent(ArenaEvent.UI_ARENA_BEGIN_FAILED_NOTIMES));
			return;
		}

		mGhostGUID = null;
		AwardScore = 0;
		AwardPoint = 0;
		Win = false;

		ArenaBeginActionParam param = new ArenaBeginActionParam();
		param.guid = mSimpleData[index].guid;
		Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_ARENA_BEGIN, param);
	}

	// 查看战绩
	public void RequestRecord()
	{
		Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_ARENA_RECORD, null);
	}

	// 同步对手数据
	public void SyncFighterData(respond_msg_arena_refresh msg)
	{
		if (msg == null)
			return;

		if(msg.cur_rank != null)
		{
			mDataModule.SetArenaCurRank(msg.cur_rank.value);
			EventSystem.Instance.PushEvent(new ArenaEvent(ArenaEvent.RECEIVE_MAIN_DATA));
		}

		PopTipManager.Instance.AddNewTip(StringHelper.GetErrorString((ERROR_CODE)msg.result, FontColor.Red));
		//PromptUIManager.Instance.AddNewPrompt(StringHelper.GetErrorString((ERROR_CODE)msg.result));

		if (msg.result != (int)ERROR_CODE.ERR_ARENA_REFRESH_OK)
			return;

		mSimpleData.Clear();

		if(msg.simpleroles != null)
		{
			for (int i = 0; i < msg.simpleroles.Count && i < MAX_FIGHTER_COUNT; ++i)
			{
				arena_simple_s simple_s = new arena_simple_s();
				msg_arena_simple_role msg_s = msg.simpleroles[i];
				simple_s.guid = msg_s.guid;
				simple_s.name = msg_s.name;
				simple_s.level = msg_s.level;
				simple_s.job = msg_s.job;
				simple_s.sex = msg_s.sex;
				simple_s.grade = msg_s.grade;
				simple_s.weaponid = msg_s.weapon_id;
				simple_s.rank = msg_s.rank;
				mSimpleData.Add(simple_s);
			}

			EventSystem.Instance.PushEvent(new ArenaEvent(ArenaEvent.RECEIVE_REFRESH_DATA));
		}
	}

	// 同步结束数据
	public void SyncEndData(respond_msg_arena_end msg)
	{
		if (msg == null)
			return;

		if (msg.result == (int)ERROR_CODE.ERR_ARENA_END_OK)
		{
			AwardScore = msg.score;
			AwardPoint = msg.money;
			EventSystem.Instance.PushEvent(new ArenaEvent(ArenaEvent.RECEIVE_END_DATA));
		}
		else
		{
			PopTipManager.Instance.AddNewTip(StringHelper.GetErrorString((ERROR_CODE)msg.result, FontColor.Red));
			//PromptUIManager.Instance.AddNewPrompt(StringHelper.GetErrorString((ERROR_CODE)msg.result));
		}

	}

	// 同步战绩数据
	public void SyncRecordData(respond_msg_arena_record msg)
	{
		if (msg == null || msg.result != (int)ERROR_CODE.ERR_ARENA_RECORD_OK)
			return;

		mRecordData.Clear();

		foreach(msg_arena_record rmsg in msg.record)
		{
			arena_record_s record = new arena_record_s();
			record.src_name = rmsg.src_name;
			record.tar_name = rmsg.tar_name;
			record.src_level = rmsg.src_level;
			record.tar_level = rmsg.tar_level;
			record.src_job = rmsg.src_job;
			record.tar_job = rmsg.tar_job;
			record.result = rmsg.result;
			record.time_stamp = rmsg.time_stamp;

			mRecordData.Add(record);
		}

		EventSystem.Instance.PushEvent(new ArenaEvent(ArenaEvent.RECEIVE_RECORD_DATA));
	}

	// 同步开战消息
	public void SyncBeginData(respond_msg_arena_begin msg)
	{
		PopTipManager.Instance.AddNewTip(StringHelper.GetErrorString((ERROR_CODE)msg.result, FontColor.Red));
		//PromptUIManager.Instance.AddNewPrompt(StringHelper.GetErrorString((ERROR_CODE)msg.result));

		if (msg.result != (int)ERROR_CODE.ERR_ARENA_BEGIN_OK)
			return;

		if (msg.property == null || msg.property.guid == null)
			return;

		mGhost.SyncProperty(msg.property);
		mGhostGUID = msg.property.guid;

        SceneManager.Instance.RequestEnterScene(GameConfig.ArenaSceneID);
	}

	// 购买次数
	public void SyncBuyTimesData(respond_msg_arena_buy_times msg)
	{
		if (msg == null)
			return;

		if (msg.result == (int)ERROR_CODE.ERR_ARENA_BUYTIMES_OK)
		{
			PopTipManager.Instance.AddNewTip(StringHelper.GetErrorString((ERROR_CODE)msg.result, FontColor.Green));
		}
		else
		{
			PopTipManager.Instance.AddNewTip(StringHelper.GetErrorString((ERROR_CODE)msg.result, FontColor.Red));
		}
		//PromptUIManager.Instance.AddNewPrompt(StringHelper.GetErrorString((ERROR_CODE)msg.result));
	}

	// 获得对手数据
	public arena_simple_s GetFighterData(int idx)
	{
		if (mSimpleData == null || mSimpleData.Count <= idx)
			return null;

		return mSimpleData[idx];
	}

	// 获得战绩数据
	public arena_record_s GetRecordData(int idx)
	{
		if (mRecordData == null || mRecordData.Count <= idx)
			return null;

		return mRecordData[idx];
	}

	public float GetTimer(ulong lastTime)
	{
		if (lastTime == 0)
			return 0.0f;

		ulong utcNow = TimeUtilities.GetUtcNowSeconds();
		if (utcNow - lastTime > GameConfig.ArenaFightCD)
			return 0.0f;

		return (float)(GameConfig.ArenaFightCD + lastTime - utcNow);
	}

	public ArenaRankLevel GetRankLevel(uint rank)
	{
		ArenaRankLevel ranklevel = ArenaRankLevel.ArenaRankLevel_End;

        IDictionaryEnumerator itr = DataManager.ArenaTable.GetEnumerator();
        while (itr.MoveNext())
        {
            ArenaTableItem item = itr.Value as ArenaTableItem;
            if (rank >= item.mHighestRank && rank <= item.mLowestRank)
            {
                ranklevel = (ArenaRankLevel)item.mRankLevel;
                break;
            }
        }
// 		foreach(ArenaTableItem item in DataManager.ArenaTable.Values)
// 		{
// 			if(rank >= item.mHighestRank && rank <= item.mLowestRank)
// 			{
// 				ranklevel = (ArenaRankLevel)item.mRankLevel;
// 				break;
// 			}
// 		}

		return ranklevel;
	}

	public int GetAwardByIdx(int idx)
	{
		if(mSimpleData == null || mSimpleData.Count <= idx)
			return 0;

		uint curRank = mDataModule.GetArenaCurRank();

		ArenaRandomTableItem randomRes = null;
        IDictionaryEnumerator itr = DataManager.ArenaRandomTable.GetEnumerator();
        while (itr.MoveNext())
        {
            ArenaRandomTableItem randomItem = itr.Value as ArenaRandomTableItem;
            if (curRank >= randomItem.mHighestRank)
            {
                randomRes = randomItem;
                break;
            }
        }
// 		foreach(ArenaRandomTableItem randomItem in DataManager.ArenaRandomTable.Values)
// 		{
// 			if(curRank >= randomItem.mHighestRank)
// 			{
// 				randomRes = randomItem;
// 				break;
// 			}
// 		}

		if(randomRes == null)
			return 0;

		ArenaTableItem arenaRes = null;
        itr = DataManager.ArenaTable.GetEnumerator();
        while (itr.MoveNext())
        {
            ArenaTableItem arenaItem = itr.Value as ArenaTableItem;
            if (curRank >= arenaItem.mHighestRank)
            {
                arenaRes = arenaItem;
                break;
            }
        }
// 		foreach(ArenaTableItem arenaItem in DataManager.ArenaTable.Values)
// 		{
// 			if (curRank >= arenaItem.mHighestRank)
// 			{
// 				arenaRes = arenaItem;
// 				break;
// 			}
// 		}

		if(arenaRes == null)
			return 0;

		uint againsterRank = mSimpleData[idx].rank;

		if (againsterRank <= curRank + randomRes.mHigherRight)
		{
			return arenaRes.mMoneyWinHigher;
		}

		if (againsterRank >= curRank + randomRes.mLowerLeft)
		{
			return arenaRes.mMoneyWinLower;
		}

		return arenaRes.mMoneyWinSame;
	}

	public string GetRankLevelStringByRankLevel(ArenaRankLevel ranklevel)
	{
		if (ArenaRankLevel.ArenaRankLevel_0 <= ranklevel && ranklevel <= ArenaRankLevel.ArenaRankLevel_3)
		{
			return StringHelper.GetString("arena_rank_" + ((int)ranklevel).ToString());
		}

		return StringHelper.GetString("arena_rank_out");
	}

	public string GetRankLevelStringByRanking(uint rank)
	{
		return GetRankLevelStringByRankLevel(GetRankLevel(rank));
	}

	public GUID GetGhostGuid()
	{
		return mGhostGUID;
	}

	public GhostData GetGhostData()
	{
		return mGhost;
	}
}
