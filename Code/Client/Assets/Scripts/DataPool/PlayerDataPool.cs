using System;
using FantasyEngine;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Message;

//玩家技能信息
public class PlayerSkillData
{
    //技能等级数据
    public Dictionary<int, int> mLevels = new Dictionary<int, int>();
    //装配的技能列表
    public int[] skills = new int[SkillMaxCountDefine.MAX_EQUIP_SKILL_NUM];
    public PlayerSkillData()
    {
        Clear();
    }

    public void Clear()
    {
        for (int i = 0; i < skills.Length; ++i )
        {
            skills[i] = -1;
        }
        mLevels.Clear();
    }

	public void SyncSkillData(role_skill_info skillinfo)
	{
		if (skillinfo == null)
		{
			return;
		}

		for (int i = 0; i < skillinfo.levels.Count; ++i)
		{
			mLevels.Add(i, skillinfo.levels[i]);
		}

		for (int i = 0; i < skillinfo.equips.Count && i < SkillMaxCountDefine.MAX_EQUIP_SKILL_NUM; ++i)
		{
			skills[i] = skillinfo.equips[i];
		}
	}
}


/// <summary>
/// 枪械配件属性
/// </summary>
public class FittingsData
{
    private int[] mProperty = new int[(uint)FittingsType.MAX_PROPERTY];
    private int[] mValue = new int[(uint)FittingsType.MAX_PROPERTY];
    private bool[] mLock = new bool[(uint)FittingsType.MAX_PROPERTY];//这个值是客户端临时存储的，也可以服务器存储，看策划
    private int mId = -1;
    public FittingsData()
    {
        Clear();
    }

    public void Clear()
    {
        mId = -1;
        for (int i = 0; i < (int)FittingsType.MAX_PROPERTY; ++i)
        {
            mProperty[i] = -1;
            mValue[i] = 0;
            mLock[i] = false;
        }
    }

    public bool IsOpen()
    {
        return mId != -1;
    }

    public int GetId()
    {
        return mId;
    }

    public void SetId(int id)
    {
        mId = id;
    }

    public bool GetProValue(uint pos, ref int proId, ref int value, ref bool forbid)
    {
        if (pos >= (uint)FittingsType.MAX_PROPERTY)
            return false;

        proId = mProperty[pos];
        value = mValue[pos];
        forbid = mLock[pos];

        return true;
    }

    public int GetProIdByPos(int pos)
    {
        if (pos >= (uint)FittingsType.MAX_PROPERTY)
            return -1;

        return mProperty[pos];
    }

    public int GetProValueByPos(int pos)
    {
        if (pos >= (uint)FittingsType.MAX_PROPERTY)
            return -1;

        return mValue[pos];
    }

    public bool SetProValue(uint pos, int pro, int value, bool forbid)
    {
        if (pos >= (uint)FittingsType.MAX_PROPERTY)
            return false;

        mProperty[pos] = pro;
        mValue[pos] = value;
        mLock[pos] = forbid;

        return true;
    }

    public int GetFightValue()
    {
        FittingsFightValueItem odres = null; 
        int value = 0;
        for (int i = 0; i < mProperty.Length; ++i)
        {
            odres = DataManager.FittingsFightValueTable[mProperty[i]] as FittingsFightValueItem;
            if (null == odres)
                return 0;
            value += Mathf.FloorToInt(odres.score * mValue[i]);
        }
        return value;
    }

}

public class QuestAwardData
{
    public int mQuestId;
    public List<QuestAwardItem> mAwardList;

    public QuestAwardData()
    {
        mAwardList = new List<QuestAwardItem>();
    }

}
public class QuestAwardItem
{
    public int mResId;
    public uint mNum;
}
public class QuestData
{
	public bool IsInit = false;
    public BitArray mQuestFlag;

    public Queue<QuestAwardData> mAwardCache;

    public List<Quest> mAllQuest;
    //主线任务
    public List<Quest> mMainQuest;
    //支线任务
    public List<Quest> mSideQuest;
    //日常任务
    public List<Quest> mDailyQuest;
    public QuestData()
    {
        mQuestFlag = new BitArray((int)QuestDefine.MaxCount);
        mAllQuest = new List<Quest>();
        mMainQuest = new List<Quest>();
        mSideQuest = new List<Quest>();
        mDailyQuest = new List<Quest>();
        mAwardCache = new Queue<QuestAwardData>();
    }

    public void Clear()
    {
        mAllQuest.Clear();
        mMainQuest.Clear();
        mSideQuest.Clear();
        mDailyQuest.Clear();
        mAwardCache.Clear();
        mQuestFlag.SetAll(false);
    }

   
    public bool AddQuest(Quest q)
    {
        mAllQuest.Add(q);
		if(q.mType == QuestType.Main)
		{
			mMainQuest.Add(q);

		}else if(q.mType == QuestType.Side)
		{
			//mSideQuest.Add(q);
            mMainQuest.Add(q);

		}else if(q.mType == QuestType.Daily)
		{
			mDailyQuest.Add(q);
		}
        mAllQuest.Sort();
        mMainQuest.Sort();
        mDailyQuest.Sort();
		return true;
    }

    public void SetFinish(int questId,bool flag=true)
    {
        mQuestFlag[questId - 1] = flag;
    }

    public void AddAward(QuestAwardData qa)
    {
        mAwardCache.Enqueue(qa);
    }

    public QuestAwardData GetAward()
    {
        return mAwardCache.Dequeue();
    }

    public bool IsFinished(int questId)
    {
        return mQuestFlag[questId-1];
    }

	public bool IsAccepted(int questid)
	{
		for( int i = 0;i< mAllQuest.Count; ++i)
		{
			if( mAllQuest[i].mId == questid)
				return true;
		}		
		return false;
	}

    public List<Quest> GetQuestList()
    {
        return mAllQuest;
    }

    public bool IsMaxAcceptCount()
    {
       return mAllQuest.Count >= (int) QuestDefine.MaxAcceptCount;
    }

    public void DeleteQuest(int questId)
    {
        mAllQuest.RemoveAll(x => (x.mId == questId));
		mMainQuest.RemoveAll(x =>(x.mId == questId));
		mSideQuest.RemoveAll(x =>(x.mId == questId));
		mDailyQuest.RemoveAll(x =>(x.mId == questId));

    }

    public Quest GetQuestById(int questId)
    {
        return mAllQuest.Find(x => (x.mId == questId));
    }

    public bool HasAwardCache()
    {
        return mAwardCache.Count > 0;
    }
}

public class ChallengeStageFloor
{	
	public uint  mMaxScore;
	public uint  mTime;		
	public bool  mAchieveOne;
	public bool  mAchieveTwo;
	public bool  mAchieveThree;
	public uint  mDayMaxScore;

    public ChallengeStageFloor()
    {
        Clear();
    }

    public void Clear()
    {
        mMaxScore = 0;
        mTime = 0;
        mAchieveOne = false;
        mAchieveTwo = false;
        mAchieveThree = false;
        mDayMaxScore = 0;
    }
}

public class RankingChallengeInfo
{
    //public UInt64 guid;		//GUID
    public int level;	//等级
    public string name;		//名称
    public uint challenge_weekscore;//挑战周积分
    public uint floor; //当前层
    public int resid;
    public GUID guid;
}
public class PlayerChallengeData
{
    public const int MAX_FLOOR_COUNT = 50;

	public uint mCurrentFloor;
	public uint	mMaxFloor;
    public uint	mWeekScore;
    public ChallengeStageFloor[] mFloors = new ChallengeStageFloor[MAX_FLOOR_COUNT + 1];
    public uint mRankNum;
    public List<RankingChallengeInfo> mRankList = new List<RankingChallengeInfo>();
    public int mRankVersion;
    public uint mChallengingFloor;

    public void Clear()
    {
        mCurrentFloor = 1;
        mMaxFloor = 0;
        mWeekScore = 0;
        mChallengingFloor = uint.MaxValue;

        for(int i = 1; i <= MAX_FLOOR_COUNT; i++)
        {
            mFloors[i] = new ChallengeStageFloor();
        }

        mRankVersion = 0;
    }
}


// 关卡数据
public class StageData
{
	// 关卡id
	public int stageid = -1;

	// 获得最高评级
	public StageGrade maxgrade = StageGrade.StageGrade_Invalid;

	// 连续杀伤率
	public uint killrate = 0;

	// 最高连击
	public uint maxcombo = 0;

	// 最快通关时间
	public uint passtimerecord = uint.MaxValue;

	// 总通关次数
	public uint passtimes = 0;
}


// 关卡日常数据
public class StageDailyData
{
	// 关卡id
	public int stageid = -1;

	// 今日挑战次数
	public uint daily_times = 0;
}
//礼包
public class PlayerstateData
{

    public Dictionary<int, BigBagModle.BUTTON_STATE> mStateDic = new Dictionary<int, BigBagModle.BUTTON_STATE>();
    public void Clear()
    {
        mStateDic.Clear();
    }
    public void updateState(int week, BigBagModle.BUTTON_STATE stage)
    {
        if (!mStateDic.ContainsKey(week))
            mStateDic.Add(week, stage);
        else
            mStateDic[week] = stage;
    }
}

public class planData
{
    public int planid  = -1;
    public PlayerPlanModule.BUTTON_STATE state = PlayerPlanModule.BUTTON_STATE.Invalid;
    public UInt32 jewel = 0;
}

public class PlayerPlanData
{

    public Dictionary<int, planData> mDataDic = new Dictionary<int, planData>();
    public void Clear()
    {
        mDataDic.Clear();
    }

    public void UpStateData(int planid,planData data)
    {
        if (!mDataDic.ContainsKey(planid))
            mDataDic.Add(planid, data);
        else
            mDataDic[planid] = data;
    }
}

// 关卡角色数据
public class PlayerStageData
{
	// 关卡id->角色关卡数据
	public Dictionary<int, StageData> mPlayerStageData = new Dictionary<int, StageData>();

	// 关卡id->角色关卡日常数据
	public Dictionary<int, StageDailyData> mPlayerStageDailyData = new Dictionary<int, StageDailyData>();

	// 清除
	public void Clear()
	{
		mPlayerStageData.Clear();
		mPlayerStageDailyData.Clear();
	}

	// 获取关卡数据
	public StageData GetStageData(int stageId)
	{
		if(!mPlayerStageData.ContainsKey(stageId))
		{
			return null;
		}

		return mPlayerStageData[stageId];
	}

	// 更新关卡数据
	public void UpdateStageData(StageData stagedata)
	{
		if(stagedata == null)
		{
			return;
		}

		if(!mPlayerStageData.ContainsKey(stagedata.stageid))
		{
			mPlayerStageData.Add(stagedata.stageid, stagedata);
            GameDebug.Log("stagedata.passtimes:" + stagedata.passtimes);
		}
		else
		{
			StageData old = mPlayerStageData[stagedata.stageid];
			old.maxgrade = stagedata.maxgrade;
			old.maxcombo = stagedata.maxcombo;
			old.killrate = stagedata.killrate;
			old.passtimerecord = stagedata.passtimerecord;
			old.passtimes = stagedata.passtimes;

		}
	}

	// 更新关卡日常数据
	public void UpdateStageDailyData(StageDailyData stagedata)
	{
		if (stagedata == null)
		{
			return;
		}

		if (!mPlayerStageDailyData.ContainsKey(stagedata.stageid))
		{
			mPlayerStageDailyData.Add(stagedata.stageid, stagedata);
		}
		else
		{
			StageDailyData old = mPlayerStageDailyData[stagedata.stageid];
			old.daily_times = stagedata.daily_times;
		}
	}
}


public class ZoneRewardData
{
    public int zoneid;
    public bool mhasobtained;
}

public class ZonesRewardData
{
    private List<ZoneRewardData> mZonesDataList = new List<ZoneRewardData>();

    public void Clear()
    {
        for (int i = 0; i < mZonesDataList.Count; ++i)
        {
            mZonesDataList[i].zoneid = 0;
            mZonesDataList[i].mhasobtained = false;
        }
    }

    public void SetZonsInfo(zone_reward_info info)
    {
        ZoneRewardData data = new ZoneRewardData();
        data.zoneid = info.zoneid;
        data.mhasobtained = info.mhasobtained;
        mZonesDataList.Add(data);
    }

    public bool GetZoneHasObtainReward(int zoneid)
    {
        for (int i = 0; i < mZonesDataList.Count; ++i)
        {
            if (zoneid == mZonesDataList[i].zoneid)
                return mZonesDataList[i].mhasobtained;
        }
        return false;
    }
}

public class GhostData
{
	public string name;
	public int level;			// 等级
	public int resId;			// 职业
	public int main_weaponId;	// 当前持有主武器
	public int sub_weaponId;	// 当前切换武器
	public uint mStrenLv;		// 强化等级
    public WingData mWingData;
    public PlayerFashionData mFashionData;
	// 配件
	public FittingsData[] mFittings = new FittingsData[(uint)FittingsType.MAX_FITTGINS];

	// 技能养成数据
	public PlayerSkillData skillData = new PlayerSkillData();
    // 佣兵数据
    public CropsItemInfo mMainCropsInfo = new CropsItemInfo();
    public CropsItemInfo mSubCropsInfo = new CropsItemInfo();

	public GhostData()
	{
		name = "Brink";
		level = 1;
        resId = 1;
		main_weaponId = 1000;
		sub_weaponId = 1000;
		mStrenLv = 0;

		for (uint i = 0; i < (uint)FittingsType.MAX_FITTGINS; i++)
		{
			mFittings[i] = new FittingsData();
		}

		skillData.Clear();
        mWingData = new WingData();
        mFashionData = new PlayerFashionData();
        mWingData.Clear();
        mMainCropsInfo.Clear();
        mSubCropsInfo.Clear();
	}

	public void SyncProperty(PlayerData playerData)
	{
		//temp
        playerData.mCropsData.GetMainCropsId();
		name = playerData.name;
		level = playerData.level;
        resId = playerData.resId;
		main_weaponId = playerData.main_weaponId;
		sub_weaponId = playerData.sub_weaponId;
		mStrenLv = playerData.mStrenLv;
        mMainCropsInfo = playerData.mCropsData.GetMainCropsInfo();
        mSubCropsInfo = playerData.mCropsData.GetSubCropsInfo();

		skillData.Clear();

		int pro = 0;
		int value = 0;
		bool forbid = false;
		for (uint i = 0; i < (uint)FittingsType.MAX_PROPERTY; ++i)
		{
			mFittings[i].SetId(playerData.mFittings[i].GetId());

			for (uint j = 0; j < (uint)FittingsType.MAX_PROPERTY; ++j)
			{
				playerData.mFittings[i].GetProValue(j, ref pro, ref value, ref forbid);
				mFittings[i].SetProValue((uint)j, pro, value, forbid);
			}
		}

		for (int i = 0; i < playerData.skillData.mLevels.Count; ++i)
		{
			skillData.mLevels.Add(i, playerData.skillData.mLevels[i]);
		}

		for (int i = 0; i < SkillMaxCountDefine.MAX_EQUIP_SKILL_NUM; ++i)
		{
			skillData.skills[i] = playerData.skillData.skills[i];
		}
	}

	public void SyncProperty(other_role_fight_property msg)
	{
		if(msg == null)
			return;

		name = msg.name;
		level = (int)msg.level;
		resId = (int)msg.job;
		main_weaponId = (int)msg.weapon_id;
		sub_weaponId = (int)msg.sub_weapon_id;
		mStrenLv = msg.stren_lv;

		skillData.Clear();

		if(msg.skillinfo != null)
		{
			for (int i = 0; i < msg.skillinfo.levels.Count; ++i)
			{
				skillData.mLevels.Add(i, msg.skillinfo.levels[i]);
			}

			for (int i = 0; i < msg.skillinfo.equips.Count && i < SkillMaxCountDefine.MAX_EQUIP_SKILL_NUM; ++i)
			{
				skillData.skills[i] = msg.skillinfo.equips[i];
			}
		}

	    if (msg.wing_info != null)
	    {
            if (msg.wing_info.unlock_count != null)
            {
                if (mWingData.wingItems.Count > (int)msg.wing_info.unlock_count.value)
                {
                    //除非重置数据否则不会出现这种情况
                    mWingData.Clear();
                }
            }
            if (msg.wing_info.wearid != null)
            {
                mWingData.mWearId = msg.wing_info.wearid.value;
            }
            if (msg.wing_info.items != null && msg.wing_info.items.Count > 0)
            {
                for (int i = 0; i < msg.wing_info.items.Count; ++i)
                {

                    WingItemData wing_item_data =
                        mWingData.wingItems.Find(x => (x.id == msg.wing_info.items[i].id));
                    if (wing_item_data == null)
                    {
                        wing_item_data = new WingItemData();
                        mWingData.wingItems.Add(wing_item_data);
                        wing_item_data.id = msg.wing_info.items[i].id;
                        wing_item_data.level = msg.wing_info.items[i].level;
                        mWingData.getPropertyTotal(ref wing_item_data);
                        // GameDebug.Log("添加WingitemData数据" + serverProperty.wing_info.items[i].id);
                    }

                    wing_item_data.id = msg.wing_info.items[i].id;
                    wing_item_data.level = msg.wing_info.items[i].level;
                    wing_item_data.process = msg.wing_info.items[i].process;
                    wing_item_data.UpdateProperty();

                }

                EventSystem.Instance.PushEvent(new WingUIEvent(WingUIEvent.WING_GHOST_UPDATE));
            }
	    }

        //时装
        role_fashion_info fashion_info = msg.fashion_info;

        if (fashion_info != null)
        {
            if (fashion_info.head_id != null)
            {
                mFashionData.head_id = fashion_info.head_id.value;
                GameDebug.Log("同步Fashion head_id = " + mFashionData.head_id);
            }

            if (fashion_info.uppper_body_id != null)
            {
                mFashionData.upper_body_id = fashion_info.uppper_body_id.value;
                GameDebug.Log("同步Fashion upper_body_id = " + mFashionData.upper_body_id);
            }

            if (fashion_info.lower_body_id != null)
            {
                mFashionData.lower_body_id = fashion_info.lower_body_id.value;
                GameDebug.Log("同步Fashion lower_body_id = " + mFashionData.lower_body_id);
            }

            if (fashion_info.unlock_count != null)
            {
                 mFashionData.unlock_count = fashion_info.unlock_count.value;
                 GameDebug.Log("同步Fashion unlock_count = " + mFashionData.unlock_count);
            }

            if (fashion_info.items != null)
            {
                for (int i = 0; i < mFashionData.unlock_count; ++i)
                {
                    mFashionData.items[i].id = fashion_info.items[i].id;
                    mFashionData.items[i].starnum = fashion_info.items[i].starnum;
                    GameDebug.Log("同步Fashion items = " + mFashionData.items[i].id + " starnum =" + mFashionData.items[i].starnum);
                }

            }
        }

        do
        {
            GameDebug.Log("同步对手佣兵信息！");
            mMainCropsInfo.mCropsId = msg.maincrops.cropsid;
            mMainCropsInfo.mCropsStarslv = msg.maincrops.starslv;
            mSubCropsInfo.mCropsId = msg.subcrops.cropsid;
            mSubCropsInfo.mCropsStarslv = msg.subcrops.starslv;
        } while (false);
	}
}

public class PlayerMallData
{
    Dictionary<int, int> mDayCount = new Dictionary<int, int>();
    Dictionary<int, int> mForeverCount = new Dictionary<int, int>();

    public PlayerMallData()
    {
        clear();
    }

    /// <summary>
    /// 购买的增加次数;key = mall表格id 或者 item表id; addition = 增加购买的次数;
    /// </summary>
    /// <param name="type"></param>
    /// <param name="key"></param>
    /// <param name="addition"></param>
    public void AddData(MallLimitType type, int key, int addition = 1)
    {
        switch (type)
        {
            case MallLimitType.ERROR:
            case MallLimitType.NONE:
                return;
            case MallLimitType.DAY:
                if (mDayCount.ContainsKey(key))
                {
                    mDayCount[key] += addition;
                }
                else
                {
                    mDayCount.Add(key, addition);
                }
                break;
            case MallLimitType.FOREVER:
                //MallFormModule mallmodule = ModuleManager.Instance.FindModule<MallFormModule>();
                //if (mallmodule == null)
                //    return;

                //int itemId = mallmodule.GetItemIdByMallId(key);
                //if(ItemManager.GetItemRes(itemId) == null)
                //{
                //    GameDebug.LogError("商城中对应的物品在物品表中不存在");
                //    return;
                //}

                if (mForeverCount.ContainsKey(key))
                {
                    mForeverCount[key] += addition;
                }
                else
                {
                    mForeverCount.Add(key, addition);
                }
                break;
        }
    }

    public int GetTimesByMallId(int mallId)
    {
        MallFormModule mallmodule = ModuleManager.Instance.FindModule<MallFormModule>();
        if (mallmodule == null)
            return 0;

        MallLimitType type = mallmodule.GetLimitTypeByID(mallId);
        switch (type)
        {
            case MallLimitType.ERROR:
                return -1;
            case MallLimitType.NONE:
                return -1;
            case MallLimitType.DAY:
                if (mDayCount.ContainsKey(mallId))
                {
                    return mDayCount[mallId];
                }
                return 0;
            case MallLimitType.FOREVER:
                int itemId = mallmodule.GetItemIdByMallId(mallId);
                if (ItemManager.GetItemRes(itemId) == null)
                {
                    GameDebug.LogError("商城中对应的物品在物品表中不存在");
                    return -1;
                }

                if (mForeverCount.ContainsKey(itemId))
                {
                    return mForeverCount[itemId];
                }
                return 0;
        }
        return -1;
    }

    void clear()
    {
        mDayCount.Clear();
        mForeverCount.Clear();
    }
}

public class WingItemData
{
    public int id;
    public uint level;
    public uint process;

	public uint life;
	public uint attack;
	public uint defence;  
	public uint critical; //加暴击
	public uint power;

	public uint liftTotal;
	public uint attackTotal;
	public uint defenceTotal;
	public uint criticalTotal;
	public uint powerTotal;
    public void Clear()
    {
        
        level = 0;
        process = 0;
        id = 0;

		life = 0;
		attack = 0;
		defence = 0;
		critical = 0;
		power = 0;
        
		liftTotal=0;
		attackTotal=0;
		defenceTotal=0;
		criticalTotal=0;
		powerTotal=0;

    }

    public void UpdateProperty()
    {
        life = 0;
        attack = 0;
        defence = 0;
        critical = 0;
        power = 0;
        for (int j = 0; j <= level; ++j)
        {

            WingLevelTableItem wlt = DataManager.WingLevelTable[id * 1000 + j] as WingLevelTableItem;
            if (wlt == null)
            {
                GameDebug.LogError("winglevel表不存在id为" + (id * 1000 +j) + "的数据");
            }

            switch ((WingPropertyType)wlt.propertyType)
            {
                case WingPropertyType.Life:
                    life += wlt.propertyNum;
                    break;
                case WingPropertyType.Attack:
                    attack += wlt.propertyNum;
                    break;
                case WingPropertyType.Defence:
                    defence += wlt.propertyNum;
                    break;
                case WingPropertyType.Critical:
                    critical += wlt.propertyNum;
                    break;
                case WingPropertyType.Energy:
                     power += wlt.propertyNum;
                    break;
            }
        }
    }
}

public class WingData
{
    public bool IsInit = false;
    public int mWearId = -1;
    public List<WingItemData> wingItems;

    public WingData()
    {
        wingItems = new List<WingItemData>();
    }

    public int GetUnlockNum()
    {
        return wingItems.Count;
    }

    public int GetLockIndex()
    {
        return wingItems.Count + 1;
    }

    public void Clear()
    {
        wingItems.Clear();
    }

    public WingItemData GetWingItemDataById(int resId)
    {
        for (int i = 0; i < wingItems.Count; ++i)
        {
            if (wingItems[i].id == resId)
            {
                return wingItems[i];
            }
        }

        return null;
    }

    public WingLevelTableItem GetLevelRes(int index)
    {
        int resid = System.Convert.ToInt32(wingItems[index].id*1000 + wingItems[index].level);
        return DataManager.WingLevelTable[resid] as WingLevelTableItem;
    }

    //计算已解锁翅膀属性总和
    public void getPropertyTotal(ref WingItemData wingitemdata)
    {
        for (int j = 0; j < WingDefine.Max_Wing_Level; ++j)
        {

            WingLevelTableItem wlt = DataManager.WingLevelTable[wingitemdata.id * 1000 + (j + 1)] as WingLevelTableItem;
            if (wlt == null)
            {
                GameDebug.LogError("winglevel表不存在id为" + wingitemdata.id * 1000 + (j + 1) + "的数据");
            }

            switch ((WingPropertyType) wlt.propertyType)
            {
                case WingPropertyType.Life:
                    wingitemdata.liftTotal += wlt.propertyNum;
                    break;
                case WingPropertyType.Attack:
                    wingitemdata.attackTotal += wlt.propertyNum;
                    break;
                case WingPropertyType.Defence:
                    wingitemdata.defenceTotal += wlt.propertyNum;
                    break;
                case WingPropertyType.Critical:
                    wingitemdata.criticalTotal += wlt.propertyNum;
                    break;
                case WingPropertyType.Energy:
                    wingitemdata.powerTotal += wlt.propertyNum;
                    break;
            }
        }
    }

}

public class ShopItemInfo
{
    public int proceedsTypeIdx;
    public int count;
    public bool isBuyDone;

    public ShopItemInfo()
    {
        proceedsTypeIdx = 0;
        count = 0;
        isBuyDone = false;
    }
}

public class PlayerShopData
{
    //神秘商店物品是否已经购买<int , bool> int=随机到的shop resId，bool=是否已经购买过;
    //Dictionary<int, bool> isBuyDone = new Dictionary<int, bool>(ShopModule.MAX_SHOP_SALE_COUNT);
    Dictionary<int , ShopItemInfo> mData = new Dictionary<int, ShopItemInfo>(ShopModule.MAX_SHOP_SALE_COUNT);
    // 距离下次刷新剩余的秒数;
    private int seconds = -1;
    // 上次刷新所处的时间段;
    private int buncket = -1;

    public PlayerShopData()
    {
        clear();
    }

    public Dictionary<int, ShopItemInfo> roleShopData
    {
        get
        {
            return mData;
        }
    }

    public int Buncket
    {
        get
        {
            return buncket;
        }
        set
        {
            buncket = value;
        }
    }

    public int Seconds
    {
        get 
        {
            return seconds;
        }
        set
        {
            seconds = value;
        }
    }

    public void SetIsDone(int resId, bool isBuy)
    {
        if (mData.ContainsKey(resId))
        {
            if (mData[resId].isBuyDone != isBuy)
                mData[resId].isBuyDone = isBuy;
        }
    }

    public void SetData(int resId, ShopItemInfo val)
    {
        if (mData.ContainsKey(resId))
        {
            if (mData[resId] != val)
                mData[resId] = val;
        }
        else
        {
            //if (mData.Count >= ShopModule.MAX_SHOP_SALE_COUNT)
            //{
            //    GameDebug.LogError("购买了超过最大种类的商品个数");
            //    return;
            //}
            mData.Add(resId, val);
        }
    }

    public ShopItemInfo GetData(int resId)
    {
        if(mData.ContainsKey(resId))
            return mData[resId];

        return null;
    }

    //重置初始值;
    public void clear()
    {
        //for (int i = 0, j = mData.Count ; i < j; i++)
        //{
        //    mData[i] = null;
        //}
        if (mData.Count > 0)
            mData.Clear();

        seconds = 0;
        buncket = 0;
    }
}

public class PlayerActivityData
{
    public Dictionary<int, int> mActivityTypeCompletedTime = new Dictionary<int, int>();
    public Dictionary<int, int> mHasCompletedActivity = new Dictionary<int, int>();
}

public class EggItemData
{
    public int time_second; // 计时剩余秒数;
    public int times;       // 开蛋次数;

    public EggItemData()
    {
        clear();
    }

    public void clear()
    {
        time_second = 0;
        times = 0;
    }
}


public class PlayerEggData
{
    private EggItemData[] mEggData = new EggItemData[EggModule.EGG_COUNT];

    public PlayerEggData()
    {
        clear();
    }

    public void clear()
    {
        for (int i = 0 , j = mEggData.Length ; i < j ; i++)
        {
            if (mEggData[i] == null)
                mEggData[i] = new EggItemData();

            mEggData[i].clear();
        }
    }

    public int GetOpenTimes(EggType et)
    {
        return mEggData[(int)et - 1].times;
    }

    public int GetTimeSeconds(EggType et)
    {
        return mEggData[(int)et - 1].time_second;
    }

    public void SetData(EggType et, EggItemData data)
    {
        mEggData[(int)et - 1] = data;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="eggIdx">范围[1..EggModule.EGG_COUNT]</param>
    /// <param name="times"></param>
    public bool AddEggOpenTimes(EggType et , int adder)
    {
        int idx = (int)et - 1;

        mEggData[idx].times += adder;

        return true;
    }

    public bool SetTimeCounter(EggType et , int seconds)
    {
        mEggData[(int)et - 1].time_second = seconds;

        return true;
    }

    public bool ResetEggTimeCounter(EggType et)
    {
        int idx = (int)et - 1;

        mEggData[idx].time_second = EggModule.getCountDownSeconds(et);

        return true;
    }

    public bool SubtractEggTimeCounter(EggType et, int subtractor)
    {
        int idx = (int)et - 1;

        mEggData[idx].time_second -= subtractor;

        return true;
    }
}


public class PlayerTitleData
{
    private int mTitleId = -1; //当前穿戴称号;
    private BitArray mTitleFlags = null;

    private bool mFirst = true;

    public PlayerTitleData()
    {
        mTitleId = -1;
        mTitleFlags = new BitArray(TitleModule.MAX_TITLE_COUNT, false);
    }

    public int TitleID
    {
        get
        {
            return mTitleId;
        }
        set
        {
            if (mTitleId != value)
            {
                mTitleId = value;
                
                Player p = PlayerController.Instance.GetControlObj() as Player;
                if (p != null)
                {
                    //去除之前的buff;
                    int oldBuffId = TitleModule.GetTitleBuffIdById(mTitleId);
                    if(oldBuffId != -1)
                        p.RemoveSkillBuffByResID((uint)oldBuffId);


                    //添加新的buff;
                    int newBuffId = TitleModule.GetTitleBuffIdById(mTitleId);
                    if(newBuffId != -1)
                        p.AddSkillEffect(new AttackerAttr(p), SkillEffectType.Buff, (uint)newBuffId);

                    p.RefreshTitle();
                }

                TitleUIEvent eve = new TitleUIEvent(TitleUIEvent.TITLE_CHANGED);
                eve.titleId = value;
                EventSystem.Instance.PushEvent(eve);
            }
        }
    }

    public bool IsFirst
    {
        get
        {
            return mFirst;
        }
        set
        {
            mFirst = value;
        }
    }

    public BitArray TitleFlags
    {
        get
        {
            return mTitleFlags;
        }
        set
        {
            if (value == null)
                return;

            if (value.Count != TitleModule.MAX_TITLE_COUNT)
                return;

            if (!IsFirst)
            {
                for(int i = 0, j = TitleModule.MAX_TITLE_COUNT; i < j; i++)
                {
                    if(mTitleFlags[i] != value[i])
                    {
                        if(value[i])
                        {
                            TitleModule tm = ModuleManager.Instance.FindModule<TitleModule>();
                            tm.GetNewTitle(i + 1);
                        }
                    }
                }
            }
            mTitleFlags = value;
        }
    }
}

public class PlayerTotalChargeData
{
    private uint totalNum;
    private BitArray mFlags;

    public uint TotalNum
    {
        get
        {
            return totalNum;
        }
    }

    public BitArray Flags
    {
        get
        {
            return mFlags;
        }
    }

    public void SyncTotalChargeData(role_totalcharge_info info)
    {
        if (info == null)
            return;

        if(info.totalcharge_num != null)
        {
            uint num = (uint)info.totalcharge_num.value;
            if (totalNum != num)
            {
                totalNum = num;
                EventSystem.Instance.PushEvent(new EventBase(ChargeEvent.CHARGE_RMB_SUCESS));
            }
        }

        if(info.reward_flags != null)
            mFlags = new BitArray(info.reward_flags);
    }

    public PlayerTotalChargeData()
    {
        clear();
    }

    void clear()
    {
        totalNum = 0;
        mFlags = new BitArray(TotalChargeModule.MAX_TOTAL_CHARGE_ITEM);
    }
}

// 基金返利;
public class PlayerFundData
{
    //private bool mIsJoined;
    private int mSecToEnd;
    private BitArray mItemFlags = null;

    public PlayerFundData()
    {
        //mIsJoined = false;
        mSecToEnd = -1;
        mItemFlags = new BitArray(FundModule.MAX_FUND_ITEM_COUNT);
    }

    //public bool IsJoined
    //{
    //    get
    //    {
    //        return mIsJoined;
    //    }
    //    set
    //    {
    //        if (value != mIsJoined)
    //        {
    //            mIsJoined = value;
    //        }
    //    }
    //}

    public int SecToEnd
    {
        get
        {
            return mSecToEnd;
        }
        set
        {
            if (value != mSecToEnd)
            {
                mSecToEnd = value;
            }
        }
    }

    public BitArray ItemFlags
    {
        get
        {
            return mItemFlags;
        }
        set
        {
            if(null == value)
                return;

            if(value.Count != FundModule.MAX_FUND_ITEM_COUNT)
                return;

            mItemFlags = value;
        }
    }
}

// 佣兵数据
public class CropsItemInfo
{
    public int mCropsId = -1;
    public int mCropsStarslv = -1;
    public float mHp = 0.0f;
    public float mDamage = 0.0f;
    public float mCrits = 0.0f;
    public float mDefence = 0.0f;
    public float mEnergy = 0.0f;

    public void Clear()
    { 
        mCropsId = -1;
        mCropsStarslv = -1;
    }
}

public class CropsData
{
    private Dictionary<int, CropsItemInfo> mCropsList = new Dictionary<int, CropsItemInfo>();
    private int main_crops_id = -1;
    private int sub_crops_id = -1;

    public CropsData()
    {
        Clear();
    }

    public Dictionary<int, CropsItemInfo> GetCropsList()
    {
        return mCropsList;
    }

    public int GetCropsStarsLvById(int resid)
    {
        if (mCropsList.ContainsKey(resid))
            return mCropsList[resid].mCropsStarslv;
        return -1;
    }

    public void SyncCropsData(crops_info info)
    {
        if (null == info)
            return;
        Clear();
        main_crops_id = info.main_crops_id;
        sub_crops_id = info.sub_crops_id;

        List<crop_info> croplist = info.cropsinfo;
        if (null == croplist)
            return;
        
        for (int i = 0; i < croplist.Count; ++i)
        {
            if (mCropsList.ContainsKey(croplist[i].cropsid))
                continue;
            CropsItemInfo item = new CropsItemInfo();
            item.mCropsId = croplist[i].cropsid;
            item.mCropsStarslv = croplist[i].starslv;

            mCropsList.Add(item.mCropsId,item);
        }


    }

    public void Clear()
    {
        mCropsList.Clear();
        main_crops_id = -1;
        sub_crops_id = -1;
    }

    public int GetMainCropsId()
    {
        return main_crops_id;
    }

    public int GetSubCropsId()
    {
        return sub_crops_id;
    }

    public CropsItemInfo GetMainCropsInfo()
    {
        if (mCropsList.ContainsKey(main_crops_id))
            return mCropsList[main_crops_id];
        return null;
    }

    public CropsItemInfo GetSubCropsInfo()
    {
        if (mCropsList.ContainsKey(sub_crops_id))
            return mCropsList[sub_crops_id];
        return null;
    }

    public bool HasCropsById(int resid)
    {
        if (mCropsList.ContainsKey(resid))
            return true;
        return false;
    }
}




public class PlayerGrade
{
    uint[] mGrades = new uint[(int)PlayerGradeEnum.PlayerGradeEnumMax];


    private bool isFirst = true;

    public event FightGradeManager.OnGradeChanged onGradeChanged;

    public PlayerGrade()
    {
        for (int i = 0, j = mGrades.Length; i < j; i++)
        {
            mGrades[i] = 0;
        }

    }

    public uint this[int index]
    {
        get
        {
            return mGrades[index];
        }
    }

    public uint[] Grades
    {
        get
        {
            return mGrades;
        }
        set 
        {
            uint old = 0;
            uint last = 0;
            
            for (int i = 0, j = mGrades.Length; i < j; i++)
            {
                old += mGrades[i];
                mGrades[i] = value[i];
                last += mGrades[i];
            }

            if (old != last && !isFirst)
            {
                if (onGradeChanged != null)
                {
                    bool isMainCity = SceneManager.Instance.GetCurScene().getType() == SceneType.SceneType_City;
                    onGradeChanged(old, last, isMainCity);
                }
            }

            isFirst = false;
        }
    }
}

public class FashionItemData
{
    public int id;
    public uint starnum;

}
public class PlayerFashionData
{
    public int head_id;
    public int upper_body_id;
    public int lower_body_id;
    public uint unlock_count;
    public FashionItemData[] items = new FashionItemData[FashionDefine.Max_Fashion_Count];

    public PlayerFashionData()
    {
        clear();
        for (uint i = 0; i < FashionDefine.Max_Fashion_Count; ++i)
        {
            items[i] = new FashionItemData();
            items[i].id = -1;
            items[i].starnum = 0;
        }
    }

    public void clear()
    {
        head_id = -1;
        upper_body_id = -1;
        lower_body_id = -1;
        unlock_count = 0;
    }
}

//玩家数据
public class PlayerData
{
	public GUID		guid;
    public string   name;
	public int      level;				//等级
	public int      resId;				//职业
	public uint		vip_value;
	public uint		vip_level;
    public int      main_weaponId;		//当前持有主武器
    public int      sub_weaponId;		//当前切换武器
    public int      exp;				//当前经验值
    public uint     mStrenLv;			//强化等级
	public int		sp;					//当前行动力
	public int		sp_max;				//最大行动力
	public ulong	sp_next_inc_time;	//行动力下次增长时刻
    public uint     mFittChance;		//免费的洗练次数
	public uint		arena_currank;
	public uint		arena_bestrank;
	public uint		arena_score;
	public uint		arena_lefttimes;
	public uint		arena_buytimes;
	public ulong	arena_lasttime;
	public uint		qualifying_currank;
	public uint		qualifying_bestrank;
	public uint		qualifying_lefttimes;
	public uint		qualifying_buytimes;
	public ulong	qualifying_lasttime;
    public ulong    next_daily_resettime;
    public uint     rmb_used;

    public bool     weapon_skill_unlocked;
    public bool     firstCharge;
	public bool     firstCharge_picked;

    public List<int> mLevelUp = new List<int>();

    //public uint[] mGrades = new uint[(int)PlayerGradeEnum.PlayerGradeEnumMax];
    public PlayerGrade mGrades = new PlayerGrade();

    //配件
    public FittingsData[] mFittings = new FittingsData[(uint)FittingsType.MAX_FITTGINS];

    //货币
    public uint[] mProceeds = new uint[(uint)ProceedsType.Money_Max];

    //技能养成数据
    public PlayerSkillData skillData = new PlayerSkillData();

    public QuestData mQuestData = new QuestData();

	// 关卡数据
	public PlayerStageData mStageData = new PlayerStageData();

    //礼包数据
    public PlayerstateData mStateData = new PlayerstateData();

    //成长数据
    public PlayerPlanData mPlanData = new PlayerPlanData();

    //商城
    //public Dictionary<int, int> mMall = new Dictionary<int, int>();
    public PlayerMallData mMall = new PlayerMallData();

    public PlayerFashionData mFashion = new PlayerFashionData();

    // 佣兵
    public CropsData mCropsData = new CropsData();


    public PackageManager mPack = null;

    public PlayerChallengeData mChallengeStage = new PlayerChallengeData();
    public WingData mWingData;
    public PlayerShopData mShop = new PlayerShopData();

    public PlayerActivityData mActivity = new PlayerActivityData();
    public PlayerEggData mEgg = new PlayerEggData();
    public PlayerTitleData mTitle = new PlayerTitleData();

    public ZonesRewardData mZonesData = new ZonesRewardData();
    public PlayerFundData mFundData = new PlayerFundData();
    public PlayerTotalChargeData mTotalChargeData = new PlayerTotalChargeData();

	public PlayerData()
	{
        name = "Brink";
		level = 1;		//temp 1
        resId = 1;
		vip_value = 0;
		vip_level = 0;
        main_weaponId = 1000;
        exp = 0;
        mStrenLv = 0;
        mPack = new PackageManager();
		sp = 0;
		sp_max = 0;
		sp_next_inc_time = ulong.MaxValue;
        mFittChance = 3;
		arena_currank = uint.MaxValue;
		arena_bestrank = uint.MaxValue;
		arena_score = 0;
		arena_lefttimes = 0;
		arena_buytimes = 0;
		arena_lasttime = 0;
		qualifying_currank = uint.MaxValue;
		qualifying_bestrank = uint.MaxValue;
		qualifying_lefttimes = 0;
		qualifying_buytimes = 0;
		qualifying_lasttime = 0;
        next_daily_resettime = ulong.MaxValue;
	    rmb_used = 0;
        for(uint i = 0 ; i < mProceeds.Length; ++i )
        {
            mProceeds[i] = 0;
        }
        mWingData = new WingData();

        skillData.Clear();
        mChallengeStage.Clear();
        mWingData.Clear();
// 		for(int i = 1 ; i < 13 ; i++)
// 		{
// 			if(skillData.mLevels.ContainsKey(i))
// 				skillData.mLevels[i] = 1;
// 			else
// 				skillData.mLevels.Add(i , 1);
// 		}

        for (uint i = 0; i < (uint)FittingsType.MAX_FITTGINS; i++)
        {
            mFittings[i] = new FittingsData();
        }

		mStageData.Clear();
        mActivity.mActivityTypeCompletedTime.Clear();
        mActivity.mHasCompletedActivity.Clear();
        mZonesData.Clear();
        mCropsData.Clear();
        weapon_skill_unlocked = false;
        firstCharge = false;
        firstCharge_picked = false;
	}
}

//玩家数据池
public class PlayerDataPool
{
	private PlayerData mPlayerData =  new PlayerData();

	private static PlayerDataPool instance = null;
	public static PlayerDataPool Instance
	{
		get
		{
			return instance;
		}
	}

	public PlayerDataPool()
	{
		instance = this;
	}

    public void Init()
    {
		
    }

	public PlayerData MainData
	{
		get
        {
			return mPlayerData;
		}
	}
}

