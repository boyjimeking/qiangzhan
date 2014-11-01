using Message;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;

public class qualifying_simple_s
{
	public GUID guid;
	public string name;
	public uint level;
	public uint job;
	public uint sex;
	public uint grade;
}

public class qualifying_record_s
{
	public string src_name;
	public string tar_name;
	public int src_level;
	public int tar_level;
	public int src_job;
	public int tar_job;
	public int result;
	public ulong time_stamp;
	public int rank_change;
}

public class QualifyingModule : ModuleBase
{
	public static readonly int MAX_FIGHTER_COUNT = 5;
	public static readonly int MAX_RECORD_COUNT = 10;

	// 对手数据
	private List<qualifying_simple_s> mSimpleData = new List<qualifying_simple_s>();

	// 战绩数据
	private List<qualifying_record_s> mRecordData = new List<qualifying_record_s>();

	private GhostData mGhost = new GhostData();
	private GUID mGhostGUID = null;

	private PlayerDataModule mDataModule = ModuleManager.Instance.FindModule<PlayerDataModule>();

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

	public uint AwardPrestige
	{
		get
		{
			if (mWin)
				return GameConfig.QualifyingWinPrestige;
			else
				return GameConfig.QualifyingLosePrestige;
		}
	}

	public uint AwardGold
	{
		get
		{
			if (mWin)
				return GameConfig.QualifyingWinGold;
			else
				return GameConfig.QualifyingLoseGold;
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

	// 请求对手数据
	public void RequestQualifyingData()
	{
		// 不到10秒钟 用上次请求的列表
		if(mLastRequestList != DateTime.MaxValue)
		{
			TimeSpan tp = DateTime.Now - mLastRequestList;
			if (tp.TotalMilliseconds < 10000)
			{
				EventSystem.Instance.PushEvent(new QualifyingEvent(QualifyingEvent.RECEIVE_LIST_DATA));
				return;
			}
		}

		mLastRequestList = DateTime.Now;

		Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_QUALIFYING_LIST, null);
	}

	// 购买次数
	public void RequestBuyTimes()
	{
		Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_QUALIFYING_BUYTIMES, null);
	}

	// 请求挑战
	public void RequestBegin(int index)
	{
		if (index < 0 || index >= mSimpleData.Count)
			return;

		if (GetRankingByIdx(index) == mDataModule.GetQualifyingCurRank())
		{
			PopTipManager.Instance.AddNewTip(StringHelper.GetErrorString(ERROR_CODE.ERR_QUALIFYING_BEGIN_FAILED_SELF, FontColor.Red));
			//PromptUIManager.Instance.AddNewPrompt(StringHelper.GetErrorString(ERROR_CODE.ERR_QUALIFYING_BEGIN_FAILED_SELF));
			return;
		}

		if (GetTimer(mDataModule.GetQualifyingLastTime()) > float.Epsilon)
		{
			PopTipManager.Instance.AddNewTip(StringHelper.GetErrorString(ERROR_CODE.ERR_QUALIFYING_BEGIN_FAILED_CD, FontColor.Red));
			//PromptUIManager.Instance.AddNewPrompt(StringHelper.GetErrorString(ERROR_CODE.ERR_QUALIFYING_BEGIN_FAILED_CD));
			EventSystem.Instance.PushEvent(new QualifyingEvent(QualifyingEvent.UI_QUALIFYING_BEGIN_FAILED_CD));
			return;
		}

		if (mDataModule.GetQualifyingLeftTimes() == 0)
		{
			PopTipManager.Instance.AddNewTip(StringHelper.GetErrorString(ERROR_CODE.ERR_QUALIFYING_BEGIN_FAILED_NOTIMES, FontColor.Red));
			//PromptUIManager.Instance.AddNewPrompt(StringHelper.GetErrorString(ERROR_CODE.ERR_QUALIFYING_BEGIN_FAILED_NOTIMES));
			EventSystem.Instance.PushEvent(new QualifyingEvent(QualifyingEvent.UI_QUALIFYING_BEGIN_FAILED_NOTIMES));
			return;
		}

		mGhostGUID = null;
		Win = false;

		QualifyingBeginActionParam param = new QualifyingBeginActionParam();
		param.guid = mSimpleData[index].guid;
		Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_QUALIFYING_BEGIN, param);
	}

	// 查看战绩
	public void RequestRecord()
	{
		Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_QUALIFYING_RECORD, null);
	}

	// 同步对手数据
	public void SyncListData(respond_msg_qualifying_list msg)
	{
		if (msg == null)
			return;

		PopTipManager.Instance.AddNewTip(StringHelper.GetErrorString((ERROR_CODE)msg.result, FontColor.Red));
		//PromptUIManager.Instance.AddNewPrompt(StringHelper.GetErrorString((ERROR_CODE)msg.result));

		if (msg.result != (int)ERROR_CODE.ERR_QUALIFYING_LIST_OK)
			return;

		mSimpleData.Clear();

		if (msg.simpleroles != null)
		{
			for (int i = 0; i < msg.simpleroles.Count && i < MAX_FIGHTER_COUNT; ++i)
			{
				qualifying_simple_s simple_s = new qualifying_simple_s();
				msg_qualifying_simple_role msg_s = msg.simpleroles[i];
				simple_s.guid = msg_s.guid;
				simple_s.name = msg_s.name;
				simple_s.level = msg_s.level;
				simple_s.job = msg_s.job;
				simple_s.sex = msg_s.sex;
				simple_s.grade = msg_s.grade;
				mSimpleData.Add(simple_s);
			}

			EventSystem.Instance.PushEvent(new QualifyingEvent(QualifyingEvent.RECEIVE_LIST_DATA));
		}
	}

	// 同步开战消息
	public void SyncBeginData(respond_msg_qualifying_begin msg)
	{
		PopTipManager.Instance.AddNewTip(StringHelper.GetErrorString((ERROR_CODE)msg.result, FontColor.Red));
		//PromptUIManager.Instance.AddNewPrompt(StringHelper.GetErrorString((ERROR_CODE)msg.result));

		if (msg.result != (int)ERROR_CODE.ERR_QUALIFYING_BEGIN_OK)
			return;

		if (msg.property == null || msg.property.guid == null)
			return;

		mGhost.SyncProperty(msg.property);
		mGhostGUID = msg.property.guid;

        SceneManager.Instance.RequestEnterScene(GameConfig.QualifyingSceneID);
	}

	// 同步战绩数据
	public void SyncRecordData(respond_msg_qualifying_record msg)
	{
		if (msg == null || msg.result != (int)ERROR_CODE.ERR_QUALIFYING_RECORD_OK)
			return;

		mRecordData.Clear();

		foreach (msg_qualifying_record rmsg in msg.record)
		{
			qualifying_record_s record = new qualifying_record_s();
			record.src_name = rmsg.src_name;
			record.tar_name = rmsg.tar_name;
			record.src_level = rmsg.src_level;
			record.tar_level = rmsg.tar_level;
			record.src_job = rmsg.src_job;
			record.tar_job = rmsg.tar_job;
			record.result = rmsg.result;
			record.time_stamp = rmsg.time_stamp;
			record.rank_change = rmsg.rank_change;

			mRecordData.Add(record);
		}

		EventSystem.Instance.PushEvent(new QualifyingEvent(QualifyingEvent.RECEIVE_RECORD_DATA));
	}

	// 同步结束数据
	public void SyncEndData(respond_msg_qualifying_end msg)
	{
		if (msg == null)
			return;

		if ((ERROR_CODE)msg.result == ERROR_CODE.ERR_QUALIFYING_END_OK)
		{
			EventSystem.Instance.PushEvent(new QualifyingEvent(QualifyingEvent.RECEIVE_END_DATA));
		}
		else
		{
			PopTipManager.Instance.AddNewTip(StringHelper.GetErrorString((ERROR_CODE)msg.result, FontColor.Red));
			//PromptUIManager.Instance.AddNewPrompt(StringHelper.GetErrorString((ERROR_CODE)msg.result));
		}
	}

	// 购买次数
	public void SyncBuyTimesData(respond_msg_qualifying_buy_times msg)
	{
		if (msg == null)
			return;

		if (msg.result == (int)ERROR_CODE.ERR_QUALIFYING_BUYTIMES_OK)
		{
			PopTipManager.Instance.AddNewTip(StringHelper.GetErrorString((ERROR_CODE)msg.result, FontColor.Green));
		}
		else
		{
			PopTipManager.Instance.AddNewTip(StringHelper.GetErrorString((ERROR_CODE)msg.result, FontColor.Red));
		}
		
		//PromptUIManager.Instance.AddNewPrompt(StringHelper.GetErrorString((ERROR_CODE)msg.result));
	}

	public QualifyingAwardTableItem GetCurAwardRes()
	{
		uint curRank = mDataModule.GetQualifyingCurRank();

		QualifyingAwardTableItem awardRes = null;
        IDictionaryEnumerator itr = DataManager.QualifyingAwardTable.GetEnumerator();
        while (itr.MoveNext())
        {
            QualifyingAwardTableItem res = itr.Value as QualifyingAwardTableItem;
            if (curRank >= res.mHighestRank)
            {
                awardRes = res;
                break;
            }
        }
// 		foreach (QualifyingAwardTableItem res in DataManager.QualifyingAwardTable.Values)
// 		{
// 			if (curRank >= res.mHighestRank)
// 			{
// 				awardRes = res;
// 				break;
// 			}
// 		}

		return awardRes;
	}


	// 获得对手数据
	public qualifying_simple_s GetFighterData(int idx)
	{
		if (mSimpleData == null || mSimpleData.Count <= idx)
			return null;

		return mSimpleData[idx];
	}

	// 获得战绩数据
	public qualifying_record_s GetRecordData(int idx)
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
		if (utcNow - lastTime > GameConfig.QualifyingFightCD)
			return 0.0f;

		return (float)(GameConfig.QualifyingFightCD + lastTime - utcNow);
	}

	public GUID GetGhostGuid()
	{
		return mGhostGUID;
	}

	public GhostData GetGhostData()
	{
		return mGhost;
	}

	public uint GetRankingByIdx(int idx)
	{
		uint curRank = mDataModule.GetQualifyingCurRank();
		if (curRank <= MAX_FIGHTER_COUNT)
		{
			return (uint)idx;
		}

		QualifyingRandomTableItem randomRes = null;

        IDictionaryEnumerator itr = DataManager.QualifyingRandomTable.GetEnumerator();

        while (itr.MoveNext())
        {
            QualifyingRandomTableItem randomItem = itr.Value as QualifyingRandomTableItem;
            if (curRank >= randomItem.mHighestRank)
            {
                randomRes = randomItem;
                break;
            }
        }

// 		foreach (QualifyingRandomTableItem randomItem in DataManager.QualifyingRandomTable.Values)
// 		{
// 			if (curRank >= randomItem.mHighestRank)
// 			{
// 				randomRes = randomItem;
// 				break;
// 			}
// 		}

		if (randomRes == null)
			return uint.MaxValue;

		return (uint)(curRank - (randomRes.mRankSpace * (MAX_FIGHTER_COUNT - idx)));
	}
}
