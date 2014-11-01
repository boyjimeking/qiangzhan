using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Message;
public class PlayerDataModule : ModuleBase
{
    private uint mMailNumber = 0;
    private uint mUnreadMailNumber = 0;
    private bool mFirstChargeRewardPicked = false;
    private bool mCharged = false;

    // 获取战斗力
    public uint GetGrade()
    {
        PlayerData data = PlayerDataPool.Instance.MainData;

        uint grade = 0;
        for (int i = 0; i < data.mGrades.Grades.Length; ++i )
        {
            grade += data.mGrades[i];
        }
        return grade;
    }
    //获得货币
    public uint GetProceeds(ProceedsType type)
    {
        PlayerData data = PlayerDataPool.Instance.MainData;

        if (type <= ProceedsType.Invalid || type > ProceedsType.Money_Max)
            return 0;

        return data.mProceeds[(uint)type];
    }

    //货币变化
    //public void ChangeProceeds(ProceedsType type, int value)
    //{
    //    uint max = 0;
    //    uint curr =  GetProceeds(type);
    //    if (value > 0)
    //    {
    //        if (uint.MaxValue - (uint)value <= curr)
    //            max = uint.MaxValue;
    //        else
    //            max = (uint)value + curr;
    //    }
    //    else
    //    {
    //        if ((uint)(-value) >= curr)
    //            max = 0;
    //        else
    //            max = curr - (uint)(-value);
    //    }

    //    //服务器段这里还有个金钱最大值

    //    SetProceeds(type, max);
    //}


    //设置货币
    private void SetProceeds(ProceedsType type, uint value)
    {
        if (type <= ProceedsType.Invalid || type > ProceedsType.Money_Max)
            return;

        PlayerData data = PlayerDataPool.Instance.MainData;

        data.mProceeds[(uint)type] = value;

        string sss = "";
        if (type == ProceedsType.Money_Game)
        {
            sss = ProceedsEvent.PROCEEDS_CHANGE_ONE;
        }
        else if (type == ProceedsType.Money_RMB)
        {
            sss = ProceedsEvent.PROCEEDS_CHANGE_TWO;
        }
        else if (type == ProceedsType.Money_Prestige)
        {
            sss = ProceedsEvent.PROCEEDS_CHANGE_THREE;
        }
        else if (type == ProceedsType.Money_Stren)
        {
            sss = ProceedsEvent.PROCEEDS_CHANGE_FOUR;
        }
		else if(type == ProceedsType.Money_Arena)
		{
			sss = ProceedsEvent.PROCEEDS_CHANGE_FIVE;
		}

        ProceedsEvent evt = new ProceedsEvent(sss);
        evt.value = value;
        EventSystem.Instance.PushEvent(evt);

        evt = new ProceedsEvent(ProceedsEvent.PROCEEDS_CHANGE_ALL);
        evt.value = value;
        EventSystem.Instance.PushEvent(evt);
    }

    private void SetMailNumber(uint number)
    {
        if( mMailNumber != number )
        {
            mMailNumber = number;
            MailEvent evt = new MailEvent(MailEvent.MAIL_NUMBER_UPDATE);
            evt.number = number;
            EventSystem.Instance.PushEvent(evt);
        }
        
    }
    public uint GetMailNumber()
    {
        return mMailNumber;
    }
    private void SetUnreadMailNumber(uint number)
    {
        if (mUnreadMailNumber != number)
        {
            mUnreadMailNumber = number;
            MailEvent evt = new MailEvent(MailEvent.MAIL_UNREAD_NUMBER_UPDATE);
            evt.unreadnumber = number;
            EventSystem.Instance.PushEvent(evt);
        }
    }
    public uint GetUnreadMailNumber()
    {
        return mUnreadMailNumber;
    }
    private void SetFirstChargePicked(bool picked)
    {
        if (mFirstChargeRewardPicked != picked)
        {
            mFirstChargeRewardPicked = picked;
        }

    }
    public bool GetFirstChargePicked()
    {
        return mFirstChargeRewardPicked;
    }
    private void SetCharged(bool charged)
    {
        if (mCharged != charged)
        {
            mCharged = charged;
        }

    }
    public bool GetCharged()
    {
        return mCharged;
    }
    private void SetSubWeapon(int subId)
    {
        PlayerData data = PlayerDataPool.Instance.MainData;

        if (data.sub_weaponId == subId)
            return;

        data.sub_weaponId = subId;

        ItemEvent evt = new ItemEvent(ItemEvent.WEAPON_CHANGE);
        evt.isSubWeapon = true;
        EventSystem.Instance.PushEvent(evt);
    }

    public void SetWeapon(int id)
    {
        PlayerData data = PlayerDataPool.Instance.MainData;

        if (data.main_weaponId == id)
            return;

        data.main_weaponId = id;

        Player player = PlayerController.Instance.GetControlObj() as Player;

		if (player != null)
        {
            player.SceneChangeWeapon(id);
        }

        ItemEvent evt = new ItemEvent(ItemEvent.WEAPON_CHANGE);
        evt.isSubWeapon = false;
        EventSystem.Instance.PushEvent(evt);
    }

    public bool HasSubWeapon()
    {
        return this.GetSubWeaponId() >= 0;
    }

    public bool SceneChangeWeapon()
    {
        if( this.GetSubWeaponId() < 0 )
        {
            return false;
        }
        Player player = PlayerController.Instance.GetControlObj() as Player;

        if (player != null && player.isAlive())
        {
            ChangeWeaponActionParam param = new ChangeWeaponActionParam();
            int curID = player.GetMainWeaponID();
			if (curID == this.GetMainWeaponId())
			{
				param.WeaponResId = this.GetSubWeaponId();
				param.SubWeaponResId = curID;
				Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_WD_CHANGE_WEAPON, param);
				return player.SceneChangeWeapon(this.GetSubWeaponId());
			}
			else
			{
				param.WeaponResId = curID;
				param.SubWeaponResId = this.GetSubWeaponId();
				Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_WD_CHANGE_WEAPON, param);
				return player.SceneChangeWeapon(this.GetMainWeaponId());
			}
        }
        return false;
    }

    public int GetSceneUseWeapon()
    {
        Player player = PlayerController.Instance.GetControlObj() as Player;
        if (player == null)
            return -1;
        return player.GetMainWeaponID();
    }

    private void SetStrenLv(uint lv)
    {
        PlayerData data = PlayerDataPool.Instance.MainData;
        data.mStrenLv = lv;

        WeaponCultivateEvent evt = new WeaponCultivateEvent(WeaponCultivateEvent.STRENGTH_CHANGE);
        EventSystem.Instance.PushEvent(evt);
    }

    public int GetMainWeaponId()
    {
        PlayerData data = PlayerDataPool.Instance.MainData;
        return data.main_weaponId;
    }

    public int GetSubWeaponId()
    {
        PlayerData data = PlayerDataPool.Instance.MainData;
        return data.sub_weaponId;
    }
    public uint GetStrenLv()
    {
        PlayerData data = PlayerDataPool.Instance.MainData;
        return data.mStrenLv;
    }

    public uint GetFittChance()
    {
        PlayerData data = PlayerDataPool.Instance.MainData;
        return data.mFittChance;
    }

    private void SetFittChance(uint fittChance)
    {
        PlayerData data = PlayerDataPool.Instance.MainData;
        data.mFittChance = fittChance;
    }

    public bool OpenFittings(uint pos, int id)
    {
        if (pos >= (uint)FittingsType.MAX_FITTGINS)
            return false;

        FittingsTableItem res = DataManager.FittingsTable[id] as FittingsTableItem;
        if (res == null)
            return false;

        PlayerData data = PlayerDataPool.Instance.MainData;
        data.mFittings[pos].SetId(id);

        for (uint i = 0; i < (uint)FittingsType.MAX_PROPERTY; ++i)
        {
            int proid = res.GetProId();
            data.mFittings[pos].SetProValue(i, proid, 0, false);
        }        

        return true;
    }

    public FittingsData GetFittingsData(uint pos)
    {
        if (pos >= (uint)FittingsType.MAX_FITTGINS)
            return null;

        PlayerData data = PlayerDataPool.Instance.MainData;
        return data.mFittings[pos];
    }

    public bool SetFittingsData(uint pos, uint index, int pro, int value, bool forbid)
    {
        FittingsData data = GetFittingsData(pos);
        if (data == null)
            return false;

        data.SetProValue(index, pro, value, forbid);

        WeaponCultivateEvent evt = new WeaponCultivateEvent(WeaponCultivateEvent.FITTING_CHANGE);
        EventSystem.Instance.PushEvent(evt);

        return true;
    }


    public int GetLevel()
    {
        PlayerData data = PlayerDataPool.Instance.MainData;
        return data.level;
    }

    private void SetLevel(uint level)
    {
        PlayerData data = PlayerDataPool.Instance.MainData;
        data.level = (int)level;

		OnLevelChanged(level);
    }

	private void OnLevelChanged(uint level)
	{
		LevelTableItem levelRes = DataManager.LevelTable[level] as LevelTableItem;
		if (levelRes == null)
		{
			ErrorHandler.Parse(ErrorCode.ConfigError, "level.txt不存在等级" + level);
			return;
		}

		PlayerData data = PlayerDataPool.Instance.MainData;

		data.sp_max = levelRes.sp;

        FundModule.CheckOpenFundActivity();
	}

	public uint GetVipValue()
	{
		PlayerData data = PlayerDataPool.Instance.MainData;
		return data.vip_value;
	}

	private void SetVipValue(uint value)
	{
		PlayerData data = PlayerDataPool.Instance.MainData;
		data.vip_value = value;
	}

	public uint GetVipLevel()
	{
		PlayerData data = PlayerDataPool.Instance.MainData;
		return data.vip_level;
	}

	private void SetVipLevel(uint level)
	{
		PlayerData data = PlayerDataPool.Instance.MainData;
		data.vip_level = level;
	}

	public int GetExp()
	{
		PlayerData data = PlayerDataPool.Instance.MainData;
		return data.exp;
	}

    public  int GetMaxExp()
    {
        int lv = GetLevel();
        if (!DataManager.LevelTable.ContainsKey(lv))
            return 0;

        LevelTableItem levelRes = DataManager.LevelTable[lv] as LevelTableItem;

        if (levelRes == null)
            return 0;
        return levelRes.exp;
    }

    private void SetExp(uint exp)
    {
        PlayerData data = PlayerDataPool.Instance.MainData;
        data.exp = (int)exp;
    }

    public string GetName()
    {
        PlayerData data = PlayerDataPool.Instance.MainData;
        return data.name;
    }

    private void SetName(string name)
    {
        PlayerData data = PlayerDataPool.Instance.MainData;
        data.name = name;
    }

	// 行动力
	public int GetSP()
	{
		PlayerData data = PlayerDataPool.Instance.MainData;
		return data.sp;
	}

    private void SetSP(uint sp)
    {
        PlayerData data = PlayerDataPool.Instance.MainData;
        data.sp = (int)sp;
    }

	public int GetSPMax()
	{
		PlayerData data = PlayerDataPool.Instance.MainData;
		return data.sp_max;
	}

	public ulong GetSpNextIncreaseTime()
	{
		PlayerData data = PlayerDataPool.Instance.MainData;
		return data.sp_next_inc_time;
	}

	public void SetSpNextIncreaseTime(ulong next_time)
	{
		PlayerData data = PlayerDataPool.Instance.MainData;
		data.sp_next_inc_time = next_time;
	}

	// 职业
	public int GetResId()
	{
		PlayerData data = PlayerDataPool.Instance.MainData;
		return data.resId;
	}

    public string GetFace()
    {
        PlayerTableItem items = DataManager.PlayerTable[GetResId()] as PlayerTableItem;
        if (items != null)
        {
            return items.face;
        }
        return "";
    }

    private void SetResId(uint resid)
    {
        PlayerData data = PlayerDataPool.Instance.MainData;
        data.resId = (int)resid;
    }

    public ItemObj GetItem(PackageType type , int packpos)
    {
        PlayerData data = PlayerDataPool.Instance.MainData;
        Dictionary<int, ItemObj> dic = data.mPack.getPackDic(type);
        if( dic.ContainsKey(packpos) )
        {
            return dic[packpos];
        }
        return null;
    }

    public ItemObj GetItemByID(int resId, PackageType type = PackageType.Invalid)
    {
        PlayerData data = PlayerDataPool.Instance.MainData;
        return data.mPack.GetItemByID(resId, type);
    }

    public ItemObj GetItemByIDAndPos(int resId, int pos, PackageType packageType = PackageType.Pack_Bag)
    {
        PlayerData data = PlayerDataPool.Instance.MainData;
        return data.mPack.GetItemByIDAndPos(resId,pos,packageType);
    }

    public ItemObj GetItemByIDAndPosOtherData(int resId, int pos, PackageType packageType = PackageType.Pack_Bag)
    {
        PlayerData data = OtherDataPool.Instance.GetOtherData();
        return data.mPack.GetItemByIDAndPos(resId, pos, packageType);
    }

    public uint GetItemNumByIDAndPos(int resId, int pos, PackageType packageType = PackageType.Pack_Bag)
    {
        PlayerData data = PlayerDataPool.Instance.MainData;
        return data.mPack.GetItemNumByIDAndPos(resId, pos, packageType);
    }

    public uint GetItemNumByID(int resId, PackageType type = PackageType.Invalid)
    {
        PlayerData data = PlayerDataPool.Instance.MainData;
        return data.mPack.GetNumByID(resId, type);
    }

	public uint GetArenaCurRank()
	{
		PlayerData data = PlayerDataPool.Instance.MainData;
		return data.arena_currank;
	}

	public void SetArenaCurRank(uint rank)
	{
		PlayerData data = PlayerDataPool.Instance.MainData;
		data.arena_currank = rank;
	}

	public uint GetArenaBestRank()
	{
		PlayerData data = PlayerDataPool.Instance.MainData;
		return data.arena_bestrank;
	}

	private void SetArenaBestRank(uint rank)
	{
		PlayerData data = PlayerDataPool.Instance.MainData;
		data.arena_bestrank = rank;
	}

	public uint GetArenaScore()
	{
		PlayerData data = PlayerDataPool.Instance.MainData;
		return data.arena_score;
	}

	private void SetArenaScore(uint score)
	{
		PlayerData data = PlayerDataPool.Instance.MainData;
		data.arena_score = score;
	}

	public uint GetArenaLeftTimes()
	{
		PlayerData data = PlayerDataPool.Instance.MainData;
		return data.arena_lefttimes;
	}

	private void SetArenaLeftTimes(uint times)
	{
		PlayerData data = PlayerDataPool.Instance.MainData;
		data.arena_lefttimes = times;
	}

	public uint GetArenaBuyTimes()
	{
		PlayerData data = PlayerDataPool.Instance.MainData;
		return data.arena_buytimes;
	}

	private void SetArenaBuyTimes(uint times)
	{
		PlayerData data = PlayerDataPool.Instance.MainData;
		data.arena_buytimes = times;
	}

	public ulong GetArenaLastTime()
	{
		PlayerData data = PlayerDataPool.Instance.MainData;
		return data.arena_lasttime;
	}

	private void SetArenaLastTime(ulong lasttime)
	{
		PlayerData data = PlayerDataPool.Instance.MainData;
		data.arena_lasttime = lasttime;
	}

	public uint GetQualifyingCurRank()
	{
		PlayerData data = PlayerDataPool.Instance.MainData;
		return data.qualifying_currank;
	}

	private void SetQualifyingCurRank(uint rank)
	{
		PlayerData data = PlayerDataPool.Instance.MainData;
		data.qualifying_currank = rank;
	}

	public uint GetQualifyingBestRank()
	{
		PlayerData data = PlayerDataPool.Instance.MainData;
		return data.qualifying_bestrank;
	}

	private void SetQualifyingBestRank(uint rank)
	{
		PlayerData data = PlayerDataPool.Instance.MainData;
		data.qualifying_bestrank = rank;
	}

	public uint GetQualifyingLeftTimes()
	{
		PlayerData data = PlayerDataPool.Instance.MainData;
		return data.qualifying_lefttimes;
	}

	private void SetQualifyingLeftTimes(uint times)
	{
		PlayerData data = PlayerDataPool.Instance.MainData;
		data.qualifying_lefttimes = times;
	}

	public uint GetQualifyingBuyTimes()
	{
		PlayerData data = PlayerDataPool.Instance.MainData;
		return data.qualifying_buytimes;
	}

	private void SetQualifyingBuyTimes(uint times)
	{
		PlayerData data = PlayerDataPool.Instance.MainData;
		data.qualifying_buytimes = times;
	}

	public ulong GetQualifyingLastTime()
	{
		PlayerData data = PlayerDataPool.Instance.MainData;
		return data.qualifying_lasttime;
	}

	private void SetQualifyingLastTime(ulong lasttime)
	{
		PlayerData data = PlayerDataPool.Instance.MainData;
		data.qualifying_lasttime = lasttime;
	}

	// 获得某关卡数据
	public StageData GetStageData(int stageId)
	{
		PlayerData data = PlayerDataPool.Instance.MainData;

		return data.mStageData.GetStageData(stageId);
	}

	// 更新关卡数据
	public void UpdateStageData(StageData stagedata)
	{
		PlayerData data = PlayerDataPool.Instance.MainData;

		data.mStageData.UpdateStageData(stagedata);
	}

	// 获得每日挑战次数
	public uint GetStageDailyTimes(int stageId)
	{
		PlayerData data = PlayerDataPool.Instance.MainData;

		if (!data.mStageData.mPlayerStageDailyData.ContainsKey(stageId))
		{
			return 0;
		}

		return data.mStageData.mPlayerStageDailyData[stageId].daily_times;
	}

	// 更新每日挑战次数
	public void UpdateStageDailyData(StageDailyData stagedata)
	{
		PlayerData data = PlayerDataPool.Instance.MainData;

		data.mStageData.UpdateStageDailyData(stagedata);
	}

	// 关卡是否通关过
	public bool IsStageHasPassed(int stageId)
	{
		StageData stagedata = GetStageData(stageId);
		if(stagedata == null)
		{
			return false;
		}

		if(stagedata.maxgrade == StageGrade.StageGrade_Invalid)
		{
			return false;
		}

		return true;
	}

	// 得到某关卡是否解锁
	public bool IsStageUnlock(int stageId)
	{
		StageData stagedata = GetStageData(stageId);

		return (stagedata != null);
	}

	// 得到某关卡最佳通关评价
	public StageGrade GetStageGrade(int stageId)
	{
		StageData stagedata = GetStageData(stageId);
		if(stagedata == null)
		{
			return StageGrade.StageGrade_Invalid;
		}

		return stagedata.maxgrade;
	}

	// 关卡通关次数
	public uint GetStagePassTimes(int stageId)
	{
		StageData stagedata = GetStageData(stageId);
		if(stagedata == null)
		{
			return 0;
		}

		return stagedata.passtimes;
	}

    public uint GetZonePassTimes(int zoneid)
    {
        uint rst = 0;
        PlayerStageData psd = PlayerDataPool.Instance.MainData.mStageData;
        foreach (var kvp in psd.mPlayerStageData)
        {
            Scene_StageSceneTableItem sst =
                DataManager.Scene_StageSceneTable[kvp.Value.stageid] as Scene_StageSceneTableItem;
            if (sst == null)
            {
                GameDebug.LogError("获取关卡资源失败");
            }

            if (sst.mZoneId == zoneid)
            {
                rst += kvp.Value.passtimes;
            }


        }

        return rst;

    }


    public PlayerSkillData GetSkillData()
    {
        return PlayerDataPool.Instance.MainData.skillData;
    }

    public void UpdateSkillData(int key , int val)
    {
        PlayerData data = PlayerDataPool.Instance.MainData;

        if (data.skillData.mLevels.ContainsKey(key))
        {
            data.skillData.mLevels[key] = val;
        }
        else
        {
            data.skillData.mLevels.Add(key, val);
        }
    }

    public void AddMallBuyTimes(int resId)
    {
        PlayerData data = PlayerDataPool.Instance.MainData;

        if (data == null)
            return;

        MallFormModule mallmodule = ModuleManager.Instance.FindModule<MallFormModule>();
        if (mallmodule == null)
            return;

        MallLimitType type = mallmodule.GetLimitTypeByID(resId);
        switch (type)
        {
            case MallLimitType.DAY:
                data.mMall.AddData(type, resId);
                break;
            case MallLimitType.FOREVER:
                int itemId = mallmodule.GetItemIdByMallId(resId);
                if(ItemManager.GetItemRes(itemId) == null)
                {
                    GameDebug.LogError("商城中对应的物品在物品表中不存在");
                    return;
                }
                data.mMall.AddData(MallLimitType.FOREVER, itemId);
                break;
            default:
                return;
        }
    }

    public int GetMallBuyTimesByID(int resId)
    {
        PlayerData data = PlayerDataPool.Instance.MainData;

        if (data == null)
            return -1;

        return data.mMall.GetTimesByMallId(resId);
    }


    public PackageManager GetPackManager()
    {
        return PlayerDataPool.Instance.MainData.mPack;
    }

    public int GetChallengeCurrentFloor()
    {
        return (int)PlayerDataPool.Instance.MainData.mChallengeStage.mCurrentFloor;
    }

    public int GetChallengMaxFloor()
    {
        return (int) PlayerDataPool.Instance.MainData.mChallengeStage.mMaxFloor;
    }
    public void SetShopIsBuyDone(int idx, bool isBuyDone)
    {
        PlayerDataPool.Instance.MainData.mShop.SetIsDone(idx, isBuyDone);
    }

    public bool GetShopIsBuyDone(int idx)
    {
        ShopItemInfo info = PlayerDataPool.Instance.MainData.mShop.GetData(idx);
        
        return info != null && info.isBuyDone;
    }

    public ShopItemInfo GetShopSecretItemInfo(int idx)
    {
        ShopItemInfo info = PlayerDataPool.Instance.MainData.mShop.GetData(idx);

        return info;
    }

    public PlayerShopData GetPlayerShopData() 
    {
        return PlayerDataPool.Instance.MainData.mShop;
    }

    public int GetEggOpenTimes(EggType et)
    {
        return PlayerDataPool.Instance.MainData.mEgg.GetOpenTimes(et);
    }

    public int GetEggTimeSeconds(EggType et)
    {
        return PlayerDataPool.Instance.MainData.mEgg.GetTimeSeconds(et);
    }

    public void AddEggOpenTimes(EggType et, int adder = 1)
    {
        PlayerDataPool.Instance.MainData.mEgg.AddEggOpenTimes(et , adder);
    }

    public void SubEggTimeSeconds(EggType et, int substracter = 1)
    {
        PlayerDataPool.Instance.MainData.mEgg.SubtractEggTimeCounter(et, substracter);
    }

    public void ResetEggTimeSeconds(EggType et)
    {
        PlayerDataPool.Instance.MainData.mEgg.ResetEggTimeCounter(et);
    }

    public int GetCurTitle()
    {
        return PlayerDataPool.Instance.MainData.mTitle.TitleID;
    }

    public void SetCurTitle(int titleid)
    {
        if (TitleModule.GetTitleItemById(titleid) == null)
        {
            titleid = -1;
        }

        PlayerDataPool.Instance.MainData.mTitle.TitleID = titleid;
    }

    public void SetTitleFlagsData(BitArray values)
    {
        PlayerDataPool.Instance.MainData.mTitle.TitleFlags = values;
    }

    public bool IsHasTitleByID(int titleid)
    {
        if (TitleModule.GetTitleItemById(titleid) == null)
            return false;

        return PlayerDataPool.Instance.MainData.mTitle.TitleFlags[titleid - 1] == true;
    }

    public bool IsFundItemGetDone(int id)
    {
        if (id < 1 || id > FundModule.MAX_FUND_ITEM_COUNT)
            return false;

        if (FundModule.GetItemByID(id) == null)
            return false;

        return PlayerDataPool.Instance.MainData.mFundData.ItemFlags[id - 1] == true;
    }

    public BitArray GetFundFlags()
    {
        return PlayerDataPool.Instance.MainData.mFundData.ItemFlags;
    }

    public int GetFundTimeSec()
    {
        return PlayerDataPool.Instance.MainData.mFundData.SecToEnd;
    }

    public void SubFundTimeSec()
    {
        if (PlayerDataPool.Instance.MainData.mFundData.SecToEnd < 0)
            return;

        PlayerDataPool.Instance.MainData.mFundData.SecToEnd--;
    }

    public ulong GetNextDailyResetTime()
    {
        return PlayerDataPool.Instance.MainData.next_daily_resettime;
    }

    private void SetNextDailyResetTime(ulong time)
    {
        PlayerDataPool.Instance.MainData.next_daily_resettime = time;
    }

    private void SetWeaponSkillUnlocked(bool unlock)
    {
        PlayerDataPool.Instance.MainData.weapon_skill_unlocked = unlock;

        SkillUIEvent e = new SkillUIEvent(SkillUIEvent.SKILL_WEAPON_SKILL);
        EventSystem.Instance.PushEvent(e);
    }

    public void AddFightGradeChangeListener(FightGradeManager.OnGradeChanged del)
    {
        if (del != null)
            PlayerDataPool.Instance.MainData.mGrades.onGradeChanged += del;
    }

    public void SyncPlayerProperty(role_property serverPropery)
    {
        if (serverPropery == null)
            return;

        SyncBaseProperty(serverPropery);
        SyncChallengeData(serverPropery);
        SyncSkillData(serverPropery);
        SyncStageData(serverPropery);
        SyncStateData(serverPropery);
        SyncBag(serverPropery);
        SyncFittings(serverPropery);
        SyncMallData(serverPropery);
        SyncShopData(serverPropery);
		SyncArenaData(serverPropery);
		SyncQualifyingData(serverPropery);
        SyncWingData(serverPropery);
        SyncActivityData(serverPropery);
        SyncMailProperty(serverPropery);
        SyncSomeProperty(serverPropery);
        SyncEggData(serverPropery);
        SyncZonesRewardData(serverPropery);
        SyncTitleData(serverPropery);
        SyncFashionData(serverPropery);      
        SyncLastError(serverPropery);
        SyncFundData(serverPropery);
        SyncCropsData(serverPropery);
        SyncFirstChargeProperty(serverPropery);
        SyncPlanData(serverPropery);
        SyncQuestData(serverPropery);
    }
    private void SyncSomeProperty(role_property serverPropery)
    {
        if (serverPropery.levelup_info == null || serverPropery.levelup_info.levels.Count < 0)
            return;

        PlayerData data = PlayerDataPool.Instance.MainData;
        if (data == null)
            return;

        for(int i = 0; i < serverPropery.levelup_info.levels.Count; i++)
        {
            data.mLevelUp.Add(serverPropery.levelup_info.levels[i]);
        }
    }

    public void SyncMailProperty(role_property serverPropery)
    {
        if( serverPropery.mail_count != null )
        {
            SetMailNumber(serverPropery.mail_count.value);
        }
         if( serverPropery.mail_unread_count != null )
        {
            SetUnreadMailNumber(serverPropery.mail_unread_count.value);
        }
    }
    public void SyncFirstChargeProperty(role_property serverPropery)
    {
        if (serverPropery.firstcharge_picked != null)
        {
            SetFirstChargePicked(serverPropery.firstcharge_picked.value);
            FundModule.OnFirstChargePicked(serverPropery.firstcharge_picked.value);
        }
        if (serverPropery.charged != null)
        {
            SetCharged(serverPropery.charged.value);
        }
    }
    public void SyncBaseProperty(role_property serverPropery)
    {
        bool changed = false;
        if (serverPropery.name != null)
        {
            changed = true;
            SetName(serverPropery.name.value);
        }

        if (serverPropery.level != null)
        {
            changed = true;
            SetLevel(serverPropery.level.value);
        }

        if (serverPropery.resid != null)
        {
            changed = true;
            SetResId(serverPropery.resid.value);
        }

		if (serverPropery.vip_value != null)
		{
			changed = true;
			SetVipValue(serverPropery.vip_value.value);
		}

		if (serverPropery.vip_level != null)
		{
			changed = true;
			SetVipLevel(serverPropery.vip_level.value);
		}

        if (serverPropery.weapon_id != null)
        {
            changed = true;
            SetWeapon((int)serverPropery.weapon_id.value);
        }

        if (serverPropery.sub_weapon_id != null)
        {
            changed = true;
            SetSubWeapon((int)serverPropery.sub_weapon_id.value);
        }

        if (serverPropery.exp != null)
        {
            changed = true;
            SetExp(serverPropery.exp.value);
        }

        if (serverPropery.stren_lv != null)
        {
            changed = true;
            SetStrenLv(serverPropery.stren_lv.value);
        }

        if (serverPropery.sp != null)
        {
            changed = true;
            SetSP(serverPropery.sp.value);
        }

		if (serverPropery.sp_next_inc_time != null)
		{
			changed = true;
			SetSpNextIncreaseTime(serverPropery.sp_next_inc_time.value);
		}

        if (serverPropery.fittchance != null)
        {
            changed = true;
            SetFittChance(serverPropery.fittchance.value);
        }

        if (serverPropery.money_game != null)
        {
            changed = true;
            SetProceeds(ProceedsType.Money_Game, serverPropery.money_game.value);
        }

        if (serverPropery.money_rmb != null)
        {
            changed = true;
            SetProceeds(ProceedsType.Money_RMB, serverPropery.money_rmb.value);
        }

        if (serverPropery.money_prestige != null)
        {
            changed = true;
            SetProceeds(ProceedsType.Money_Prestige, serverPropery.money_prestige.value);
        }

        if (serverPropery.money_stren != null)
        {
            changed = true;
            SetProceeds(ProceedsType.Money_Stren, serverPropery.money_stren.value);
        }

		if (serverPropery.money_arena != null)
		{
			changed = true;
			SetProceeds(ProceedsType.Money_Arena, serverPropery.money_arena.value);
		}

        if(serverPropery.next_daily_resettime != null)
        {
            changed = true;
            SetNextDailyResetTime(System.Convert.ToUInt64(serverPropery.next_daily_resettime.value));
        }

        if (serverPropery.weaponskill != null)
        {
            changed = true;
            SetWeaponSkillUnlocked(System.Convert.ToBoolean(serverPropery.weaponskill.value));
        }

        if (serverPropery.rmb_used != null)
        {
            changed = true;
            PlayerData data = PlayerDataPool.Instance.MainData;
            data.rmb_used = serverPropery.rmb_used.value;
        }

        if (serverPropery.grades != null && serverPropery.grades.Count > 0)
        {
            PlayerData data = PlayerDataPool.Instance.MainData;
            if (data != null)
            {
                int count = (int)PlayerGradeEnum.PlayerGradeEnumMax;
                uint[] vals = new uint[count];
                
                for (int i = 0; i < serverPropery.grades.Count; ++i)
                {
                    if( i < count )
                    {
                        vals[i] = serverPropery.grades[i];
                    }
                }

                data.mGrades.Grades = vals;
            }

            changed = true;
        }

        if (serverPropery.last_city_id != null)
        {
            //SceneManager.Instance.SetLastCityResId(serverPropery.last_city_id.value);
            //changed = true;
        }
        
        if( changed )
        {
            PropertyEvent evt = new PropertyEvent(PropertyEvent.MAIN_PROPERTY_CHANGE);
            EventSystem.Instance.PushEvent(evt); 
        }
    }

    private void SyncChallengeData(role_property serverPropery)
    {
        PlayerData data = PlayerDataPool.Instance.MainData;
        if (data == null)
            return;

        bool refresh = false;
        if(serverPropery.challenge_currentfloor != null)
        {
            data.mChallengeStage.mCurrentFloor = serverPropery.challenge_currentfloor.value;
            refresh = true;

            if(data.mChallengeStage.mChallengingFloor == uint.MaxValue)
            {
                data.mChallengeStage.mChallengingFloor = data.mChallengeStage.mCurrentFloor;
               
            }
        }

        if(serverPropery.challenge_maxfloor != null)
        {
            data.mChallengeStage.mMaxFloor = serverPropery.challenge_maxfloor.value;
            GameDebug.Log("mMaxFloor = " + data.mChallengeStage.mMaxFloor);
            refresh = true;
        }

        if(serverPropery.challenge_weekscore != null)
        {
            data.mChallengeStage.mWeekScore = serverPropery.challenge_weekscore.value;
            refresh = true;
        }

        if (serverPropery.challengestage_floor != null && serverPropery.challengestage_floor.Count > 0)
        {
            refresh = true;
            for(int i = 0; i < serverPropery.challengestage_floor.Count; i++)
            {
                challenge_stage_floor_info floorinfo = serverPropery.challengestage_floor[i];
                data.mChallengeStage.mFloors[floorinfo.floor].mAchieveOne = floorinfo.achieve_one;
                data.mChallengeStage.mFloors[floorinfo.floor].mAchieveTwo = floorinfo.achieve_two;
                data.mChallengeStage.mFloors[floorinfo.floor].mAchieveThree = floorinfo.achieve_three;
                data.mChallengeStage.mFloors[floorinfo.floor].mDayMaxScore = floorinfo.day_maxscore;
                data.mChallengeStage.mFloors[floorinfo.floor].mTime = 0;
                data.mChallengeStage.mFloors[floorinfo.floor].mMaxScore = floorinfo.maxscore;
            }
        }

        if(refresh)
        {
            EventSystem.Instance.PushEvent(new ChallengeEvent(ChallengeEvent.CHALLENGE_UI_UPDATE));
        }
    }

    private void SyncSkillData(role_property serverPropery)
    {
        if (serverPropery.skillinfo == null)
            return;

        PlayerData data = PlayerDataPool.Instance.MainData;
        if (data == null)
            return;

        data.skillData.Clear();
        for (int i = 0; i < serverPropery.skillinfo.levels.Count; ++i)
        {
            data.skillData.mLevels.Add(i, serverPropery.skillinfo.levels[i]);
        }

        for (int i = 0; i < serverPropery.skillinfo.equips.Count && i < SkillMaxCountDefine.MAX_EQUIP_SKILL_NUM; ++i)
        {
            data.skillData.skills[i] = serverPropery.skillinfo.equips[i];
        }

        //更新界面 
        SkillUIEvent sue = new SkillUIEvent(SkillUIEvent.SKILL_EQUIP);
        EventSystem.Instance.PushEvent(sue);
    }

    private void SyncQuestData(role_property serverProperty)
    {
        if (serverProperty.questinfo == null)
            return;

        PlayerData data = PlayerDataPool.Instance.MainData;
        if (data == null)
            return;
		GameDebug.Log("同步任务数据");
		data.mQuestData.IsInit = true;
        if (serverProperty.questinfo.quest_flag != null)
        {
            data.mQuestData.Clear();
            data.mQuestData.mQuestFlag = new BitArray(serverProperty.questinfo.quest_flag);
        }
        
        for (int i = 0; i < serverProperty.questinfo.quests.Count; i++)
        {
            role_quest rq = serverProperty.questinfo.quests[i];
            if ((DataManager.QuestTable[rq.questid] as QuestTableItem) == null)
            {
                GameDebug.LogError("无效任务id = "+rq.questid);
                continue;
            }
            Quest quest;
            if (!data.mQuestData.IsAccepted(rq.questid))
            {
                quest = new Quest(rq.questid);
                quest.mProcess = rq.process;
                data.mQuestData.AddQuest(quest);                
            }
            else
            {
                quest= data.mQuestData.GetQuestById(rq.questid);
                quest.mProcess = rq.process;
            }
            GameDebug.Log("任务数据：" + quest.mId + " 任务进度：" + quest.mProcess);            
        }

		EventSystem.Instance.PushEvent(new QuestEvent(QuestEvent.Quest_SYNC_SERVER_EVENT));
    }

    private void SyncStageData(role_property serverProperty)
    {
        if (serverProperty.stageinfo == null)
            return;

        PlayerData data = PlayerDataPool.Instance.MainData;
        if (data == null)
            return;

		if (serverProperty.stageinfo.stages != null && serverProperty.stageinfo.stages.Count > 0)
		{
			data.mStageData.mPlayerStageData.Clear();
			for (int i = 0; i < serverProperty.stageinfo.stages.Count; ++i)
			{
				role_stage stage = serverProperty.stageinfo.stages[i];

				if (data.mStageData.mPlayerStageData.ContainsKey((int)stage.stage_id))
				{
					GameDebug.LogError("SyncStageData Error: Same StageId.");
					continue;
				}

				StageData stagedata = new StageData();
				stagedata.stageid = (int)stage.stage_id;
				stagedata.maxgrade = (StageGrade)stage.max_grade;
				stagedata.maxcombo = stage.max_combo;
				stagedata.killrate = stage.kill_rate;
				stagedata.passtimerecord = stage.passtime_record;
				stagedata.passtimes = stage.pass_times;

				data.mStageData.mPlayerStageData.Add((int)stage.stage_id, stagedata);
			}

			EventSystem.Instance.PushEvent(new StageSyncServerEvent(StageSyncServerEvent.STAGE_SYNC_SERVER_EVENT));
		}

		if (serverProperty.stageinfo.stages_daily != null && serverProperty.stageinfo.stages_daily.Count > 0)
		{
			data.mStageData.mPlayerStageDailyData.Clear();
			for(int i = 0; i < serverProperty.stageinfo.stages_daily.Count; ++i)
			{
				role_stage_daily stage_daily = serverProperty.stageinfo.stages_daily[i];

				if (data.mStageData.mPlayerStageDailyData.ContainsKey((int)stage_daily.stage_id))
				{
					GameDebug.LogError("SyncStageDailyData Error: Same StageId.");
					continue;
				}

				StageDailyData stage_daily_data = new StageDailyData();
				stage_daily_data.stageid = (int)stage_daily.stage_id;
				stage_daily_data.daily_times = stage_daily.daily_times;

				data.mStageData.mPlayerStageDailyData.Add((int)stage_daily.stage_id, stage_daily_data);
			}

			EventSystem.Instance.PushEvent(new StageSyncServerEvent(StageSyncServerEvent.STAGE_DAILY_SYNC_SERVER_EVENT));
		}
    }

    private void SyncStateData(role_property serverProperty)
    {
        PlayerData data = PlayerDataPool.Instance.MainData;
        if (data == null)
            return;

        if (serverProperty.statenfo != null && serverProperty.statenfo.Count > 0)
        {
            for (int i = 0; i < BigBagModle.MAX_DATA_NUM; i++)
            {
                data.mStateData.updateState(serverProperty.statenfo[i].week, (BigBagModle.BUTTON_STATE)serverProperty.statenfo[i].result);
            }  
        }
    }

    private void SyncPlanData(role_property serverProperty)
    {
        PlayerData data = PlayerDataPool.Instance.MainData;
        if (data == null)
            return;
 
        if (serverProperty.planinfo != null && serverProperty.planinfo.Count > 0)
        {
            for (int i = 0; i < PlayerPlanModule.MAX_PLAN_NUM; i++)
            {
                planData plan = new planData();
                plan.planid = serverProperty.planinfo[i].planid;
                plan.state = (PlayerPlanModule.BUTTON_STATE)serverProperty.planinfo[i].state;
                plan.jewel = serverProperty.planinfo[i].jewel;

                data.mPlanData.UpStateData(plan.planid, plan);
            }
        }
    }
    public void SyncBag(role_property serverProperty)
    {
        PlayerData data = PlayerDataPool.Instance.MainData;
        if (data == null)
            return;

        //差异更新
        if (serverProperty.bag_diff != null && serverProperty.bag_diff.Count > 0)
        {
            for (int j = 0; j < serverProperty.bag_diff.Count; j++)
            {
                role_bag_diff_info diff_info = serverProperty.bag_diff[j];

                data.mPack.SyncDiffItem(diff_info);
            }
        }
        else
        {
            if (serverProperty.normalbag == null || serverProperty.equipbag == null ||
                serverProperty.weaponbag == null || serverProperty.gembag == null)
                return;

            data.mPack.Clear();

            data.mPack.SyncPackBag(PackageType.Pack_Bag, serverProperty.normalbag);
            data.mPack.SyncPackBag(PackageType.Pack_Equip, serverProperty.equipbag);
            data.mPack.SyncPackBag(PackageType.Pack_Weapon, serverProperty.weaponbag);
            data.mPack.SyncPackBag(PackageType.Pack_Gem, serverProperty.gembag);
        }   
    }

    private void SyncFittings(role_property serverProperty)
    {
        if (serverProperty.fittingslot == null)
            return;

        if (serverProperty.fittingslot.Count <= 0)
            return;

        PlayerData data = PlayerDataPool.Instance.MainData;
        if (data == null)
            return;

        List<bap_fittings_info> fitting_info = serverProperty.fittingslot;

        int floor = (int)data.mChallengeStage.mMaxFloor / 5;
        int index = 0;
        if (floor > 0)
        {
            if (floor % 6 != 0)
            {
                for (int i = floor - floor % 6 + 1; i <= floor; ++i)
                    data.mFittings[index++].SetId(i * 5);
                if (6 < floor)
                {
                    for (int i = floor - 6 + 1; i <= floor - floor % 6; ++i)
                        data.mFittings[index++].SetId(i * 5);
                }
            }
            else
            {
                for (int i = floor - 6 + 1; i <= floor; ++i)
                    data.mFittings[index++].SetId(i * 5);
            }
        }

        for (int i = 0; i < fitting_info.Count; ++i)
        {
            for (int j = 0; j < (uint)FittingsType.MAX_PROPERTY; ++j)
            {
                data.mFittings[i].SetProValue((uint)j, fitting_info[i].mproperty[j], fitting_info[i].mvalue[j], fitting_info[i].mlock[j]);
            }
        }
    }

    private void SyncMallData(role_property serverProperty)
    {
        PlayerData data = PlayerDataPool.Instance.MainData;

        if (data == null)
            return;

        if (serverProperty.mall_dayinfo != null)
        {
            for (int i = 0, j = serverProperty.mall_dayinfo.count.Count; i < j; ++i)
            {
                data.mMall.AddData(MallLimitType.DAY, serverProperty.mall_dayinfo.mallid[i], serverProperty.mall_dayinfo.count[i]);
            }
        }

        if (serverProperty.mall_feverinfo != null)
        {
            for (int m = 0, n = serverProperty.mall_feverinfo.count.Count; m < n; ++m)
            {
                data.mMall.AddData(MallLimitType.FOREVER, serverProperty.mall_feverinfo.itemid[m], serverProperty.mall_feverinfo.count[m]);
            }
        }
    }

    private void SyncShopData(role_property serverProperty)
    {
        PlayerData data = PlayerDataPool.Instance.MainData;

        if (data == null)
            return;

        if (serverProperty.shop_info != null)
        {
            int totalnum = serverProperty.shop_info.item.Count;
            if (totalnum != ShopModule.MAX_SHOP_SALE_COUNT)
            {
                GameDebug.LogError("随机出的商品个数不为" + ShopModule.MAX_SHOP_SALE_COUNT + "个");
                return;
            }

            data.mShop.clear();

            for (int i = 0; i < totalnum; i++)
            {
                ShopItemInfo info = new ShopItemInfo();
                role_shop_item_info roleInfo = serverProperty.shop_info.item[i];
                info.count = roleInfo.count;
                info.proceedsTypeIdx = roleInfo.pricetypeidx;
                info.isBuyDone = roleInfo.isbuy != 0;
                data.mShop.SetData(roleInfo.shopid , info);
            }

            data.mShop.Seconds = serverProperty.shop_info.sec.refresh_seconds + 1;
            data.mShop.Buncket = serverProperty.shop_info.sec.refresh_buncket;
        }

        ShopModule module = ModuleManager.Instance.FindModule<ShopModule>();
        if (module != null)
            module.RefreshShop();
    }

    private void SyncTitleData(role_property serverProperty)
    {
        PlayerData data = PlayerDataPool.Instance.MainData;

        if (data == null)
            return;

        if (serverProperty.title_info != null)
        {
            if (serverProperty.title_info.titleid != null)
                data.mTitle.TitleID = serverProperty.title_info.titleid.value;

            if (serverProperty.title_info.title_flags != null)
            {
                data.mTitle.TitleFlags = new BitArray(serverProperty.title_info.title_flags);
                data.mTitle.IsFirst = false;
            }
        }
    }

    private void SyncFundData(role_property serverProperty)
    {
        PlayerData data = PlayerDataPool.Instance.MainData;

        if (data == null)
            return;

        if (serverProperty.fund_info != null)
        {
            //data.mFundData.IsJoined = serverProperty.fund_info.is_joined;
            data.mFundData.SecToEnd = serverProperty.fund_info.end_seconds;
            data.mFundData.ItemFlags = new BitArray(serverProperty.fund_info.fund_flags);
            FundModule.CheckOpenFundActivity();
        }
    }

    private void SyncLastError(role_property serverProperty)
    {
        if( serverProperty.lasterror != 0 )
        {
            PopTipManager.Instance.AddNewTip(StringHelper.GetErrorString((ERROR_CODE)serverProperty.lasterror));
            //PromptUIManager.Instance.AddNewPrompt();
        }
    }
    private void SyncEggData(role_property serverProperty)
    {
        PlayerData data = PlayerDataPool.Instance.MainData;

        if (data == null)
            return;

        if (serverProperty.egg_info != null)
        {
            int totalnum = serverProperty.egg_info.item.Count;
            if (totalnum != EggModule.EGG_COUNT)
            {
                GameDebug.LogError("砸蛋网络数据错了");
                return;
            }

            data.mEgg.clear();

            for (int i = 0; i < totalnum; i++)
            {
                EggItemData eggData = new EggItemData();

                role_egg_item_info eggInfo = serverProperty.egg_info.item[i];

                eggData.time_second = eggInfo.seconds;
                eggData.times = eggInfo.opentimes;

                data.mEgg.SetData(((EggType)(i + 1)), eggData);
            }
        }
    }

    private void SyncActivityData(role_property serverProperty)
    {
        PlayerData data = PlayerDataPool.Instance.MainData;
        if (data == null)
            return;

        bool notify = false;
        if (serverProperty.activity_time != null)
        {
            notify = true;
            for(int i = 0; i < serverProperty.activity_time.item.Count; i++)
            {
                int type = serverProperty.activity_time.item[i].activity_type;
                int count = serverProperty.activity_time.item[i].time;

                if (!data.mActivity.mActivityTypeCompletedTime.ContainsKey(type))
                {
                    data.mActivity.mActivityTypeCompletedTime.Add(type, count);
                }
                else
                {
                    data.mActivity.mActivityTypeCompletedTime[type] = count;
                }
            }
        }

        if (serverProperty.activity_complete != null)
        {
            notify = true;
            data.mActivity.mHasCompletedActivity.Clear();
            for(int i = 0; i < serverProperty.activity_complete.act.Count; i++)
            {
                data.mActivity.mHasCompletedActivity.Add(serverProperty.activity_complete.act[i], 1);
            }
        }

        if (notify)
        {
            EventSystem.Instance.PushEvent(new ActivityDataUpdateEvent());
        }
    }


	private void SyncWingData(role_property serverProperty)
	{
        PlayerData data = PlayerDataPool.Instance.MainData;

        if (data == null)
            return;

        if (serverProperty.wing_info == null) return;

	    if (serverProperty.wing_info.unlock_count != null)
	    {
	        if (data.mWingData.wingItems.Count > (int)serverProperty.wing_info.unlock_count.value)
	        {
                //除非重置数据否则不会出现这种情况
	            data.mWingData.Clear();
	        }
	    }
	    if (serverProperty.wing_info.wearid != null)
	    {
	        data.mWingData.mWearId = serverProperty.wing_info.wearid.value;
	    }
        if (serverProperty.wing_info.items != null && serverProperty.wing_info.items.Count > 0)
        {
            for (int i = 0; i < serverProperty.wing_info.items.Count; ++i)
            {

                WingItemData wing_item_data =
                    data.mWingData.wingItems.Find(x => (x.id == serverProperty.wing_info.items[i].id));
                if (wing_item_data == null)
                {
                    wing_item_data = new WingItemData();
                    data.mWingData.wingItems.Add(wing_item_data);
                    wing_item_data.id = serverProperty.wing_info.items[i].id;
                    wing_item_data.level = serverProperty.wing_info.items[i].level;
                    data.mWingData.getPropertyTotal(ref wing_item_data);
                   // GameDebug.Log("添加WingitemData数据" + serverProperty.wing_info.items[i].id);
                }

                wing_item_data.id = serverProperty.wing_info.items[i].id;
                wing_item_data.level = serverProperty.wing_info.items[i].level;
                wing_item_data.process = serverProperty.wing_info.items[i].process;
                wing_item_data.UpdateProperty();

            }

            EventSystem.Instance.PushEvent(new WingUIEvent(WingUIEvent.Wing_UI_UPDATE));
            
        }
       
    }

	private void SyncArenaData(role_property serverProperty)
	{
		PlayerData data = PlayerDataPool.Instance.MainData;

		if (data == null)
			return;

		if (serverProperty.arenainfo == null)
			return;

		if (serverProperty.arenainfo.max_rank != null)
		{
			SetArenaBestRank(serverProperty.arenainfo.max_rank.value);
		}
		
		if(serverProperty.arenainfo.score != null)
		{
			SetArenaScore(serverProperty.arenainfo.score.value);
		}

		if (serverProperty.arenainfo.left_times != null)
		{
			SetArenaLeftTimes(serverProperty.arenainfo.left_times.value);
		}

		if (serverProperty.arenainfo.buy_times != null)
		{
			SetArenaBuyTimes(serverProperty.arenainfo.buy_times.value);
		}

		if (serverProperty.arenainfo.last_time_stamp != null)
		{
			SetArenaLastTime(serverProperty.arenainfo.last_time_stamp.value);
		}

		EventSystem.Instance.PushEvent(new ArenaEvent(ArenaEvent.RECEIVE_MAIN_DATA));
	}

	private void SyncQualifyingData(role_property serverProperty)
	{
		PlayerData data = PlayerDataPool.Instance.MainData;

		if (data == null)
			return;

		if (serverProperty.qualifying_info == null)
			return;

		if (serverProperty.qualifying_info.cur_rank != null)
		{
			SetQualifyingCurRank(serverProperty.qualifying_info.cur_rank.value);
		}

		if (serverProperty.qualifying_info.max_rank != null)
		{
			SetQualifyingBestRank(serverProperty.qualifying_info.max_rank.value);
		}

		if (serverProperty.qualifying_info.left_times != null)
		{
			SetQualifyingLeftTimes(serverProperty.qualifying_info.left_times.value);
		}

		if (serverProperty.qualifying_info.buy_times != null)
		{
			SetQualifyingBuyTimes(serverProperty.qualifying_info.buy_times.value);
		}

		if (serverProperty.qualifying_info.last_time_stamp != null)
		{
			SetQualifyingLastTime(serverProperty.qualifying_info.last_time_stamp.value);
		}

		EventSystem.Instance.PushEvent(new QualifyingEvent(QualifyingEvent.RECEIVE_MAIN_DATA));
	}

    public void SyncCropsData(role_property serverProperty)
    {
        PlayerData data = PlayerDataPool.Instance.MainData;

        if (data == null)
            return;

        if (null == serverProperty.cropsinfo)
            return;

        data.mCropsData.SyncCropsData(serverProperty.cropsinfo);
    }

    public int GetCropsStarsLv(int resid)
    {
        PlayerData data = PlayerDataPool.Instance.MainData;

        if (data == null)
            return -1;

        return data.mCropsData.GetCropsStarsLvById(resid);
    }

    public int GetMainCropsId()
    {
        PlayerData data = PlayerDataPool.Instance.MainData;

        if (data == null)
            return -1;
        return data.mCropsData.GetMainCropsId();
    }

    public int GetSubCropsId()
    {
        PlayerData data = PlayerDataPool.Instance.MainData;

        if (data == null)
            return -1;
        return data.mCropsData.GetSubCropsId();
    }

    public CropsItemInfo GetMainCropsInfo()
    {
        PlayerData data = PlayerDataPool.Instance.MainData;

        if (data == null)
            return null;
        return data.mCropsData.GetMainCropsInfo();
    }

    public CropsItemInfo GetSubCropsInfo()
    {
        PlayerData data = PlayerDataPool.Instance.MainData;

        if (data == null)
            return null;
        return data.mCropsData.GetSubCropsInfo();
    }

    public bool GetCropsState(int resid)
    {
        if (resid == GetMainCropsId())
            return true;
        if (resid == GetSubCropsId())
            return true;
        return false;
    }

    public bool HasObtainCrops(int resid)
    {
        PlayerData data = PlayerDataPool.Instance.MainData;

        if (data == null)
            return false;

        return data.mCropsData.HasCropsById(resid);
    }
    public bool IsActivityCompleted(int resid)
    {
        PlayerData data = PlayerDataPool.Instance.MainData;
        if (data == null)
            return false;

        return data.mActivity.mHasCompletedActivity.ContainsKey(resid);
    }

    public int GetActivityTypeCompleteTime(int actType)
    {
        PlayerData data = PlayerDataPool.Instance.MainData;
        if (data == null)
            return 0;

        if (!data.mActivity.mActivityTypeCompletedTime.ContainsKey(actType))
            return 0;

        return data.mActivity.mActivityTypeCompletedTime[actType];
    }

	//任务进度
	public int GetQuestProgress(int questid)
	{
		PlayerData data = PlayerDataPool.Instance.MainData;
		if(data == null)
			return 0;

		return data.mQuestData.GetQuestById(questid).mProcess;
	}

	public bool IsQuestFinish(int questid)
	{
		PlayerData data = PlayerDataPool.Instance.MainData;
		if(data == null)
			return false;
		return data.mQuestData.IsFinished(questid);		
	}

	public bool IsQuestAccepted(int questid)
	{
		return PlayerDataPool.Instance.MainData.mQuestData.IsAccepted(questid);
	}
    public List<Quest> GetQuestList()
    {
        return PlayerDataPool.Instance.MainData.mQuestData.GetQuestList();
    }
	public bool AcceptQuest(int questid)
	{
		var quest = new Quest(questid);
		quest.mState = QuestState.Accepted;
        PlayerDataPool.Instance.MainData.mQuestData.SetFinish(questid,false);
		PlayerDataPool.Instance.MainData.mQuestData.AddQuest(quest);
		return true;
	}

    public bool FinishQuest(int questid)
    {
        PlayerDataPool.Instance.MainData.mQuestData.SetFinish(questid);
        PlayerDataPool.Instance.MainData.mQuestData.DeleteQuest(questid);
        return true;
    }

    public List<int> GetLevelUpInfo()
    {
        PlayerData data = PlayerDataPool.Instance.MainData;
        if (data == null)
            return null;

        return data.mLevelUp;
    }

    public void ClearLevelUpInfo()
    {
        PlayerData data = PlayerDataPool.Instance.MainData;
        if (data == null)
            return;

        data.mLevelUp.Clear();
    }


    public bool IsLevelUp()
    {
        PlayerData data = PlayerDataPool.Instance.MainData;
        if (data == null || data.mLevelUp == null)
            return false;

        return data.mLevelUp.Count > 0;
    }

    public uint GetWingLevel(int wingid)
    {
        PlayerData data = PlayerDataPool.Instance.MainData;
        WingItemData wing_item_data = data.mWingData.GetWingItemDataById(wingid);
        if (wing_item_data == null)
        {
            return 0;
        }
        return wing_item_data.level;
    }

    public uint GetRMBUsed()
    {
        PlayerData data = PlayerDataPool.Instance.MainData;
        return data.rmb_used;
    }

    public int GetQuestTotalProgress(int questid)
    {
        QuestTableItem res = DataManager.QuestTable[questid] as QuestTableItem;
        if (res == null)
        {
            GameDebug.LogError("无效任务id = " + questid);
            return 0;
        }

        int totalProgress = 0;
        if (res.completeCondition1 != -1 && res.completeCondition1Argu3 > 0)
        {
            totalProgress += res.completeCondition1Argu3;
        }
        if (res.completeCondition2 != -1 && res.completeCondition2Argu3 > 0)
        {
            totalProgress += res.completeCondition2Argu3;
        }
       // GameDebug.Log("获取任务总进度" + totalProgress);
        return totalProgress;
    }

    public int getQuestProgress(int questid)
    {
        PlayerData data = PlayerDataPool.Instance.MainData;
        for (int i = 0; i < data.mQuestData.mAllQuest.Count;i++)
        {
            if (data.mQuestData.mAllQuest[i].mId == questid)
            {
                //GameDebug.Log("获取任务进度" + data.mQuestData.mAllQuest[i].mProcess);
                return data.mQuestData.mAllQuest[i].mProcess;
            }
        }
        return 0;
    }

	public GUID getGUID()
	{
		PlayerData data = PlayerDataPool.Instance.MainData;
		return data.guid;
	}

	public void setGUID(GUID guid)
	{
		PlayerData data = PlayerDataPool.Instance.MainData;
		data.guid = guid;
	}

    private void SyncZonesRewardData(role_property serverProperty)
    {
        PlayerData data = PlayerDataPool.Instance.MainData;
        if (data == null)
            return;
        zones_reward_info zonesList = serverProperty.zones_reward_info;

        if (zonesList != null)
            for (int i = 0; i < zonesList.zones_reward.Count; ++i)
            {
                data.mZonesData.SetZonsInfo(zonesList.zones_reward[i]);
            }
    }

    public bool GetZoneHasObtainReward(int zoneid)
    {
        PlayerData data = PlayerDataPool.Instance.MainData;
        return data.mZonesData.GetZoneHasObtainReward(zoneid);
    }

    private void SyncFashionData(role_property serverProperty)
    {
        PlayerData data = PlayerDataPool.Instance.MainData;
        if (data == null)
            return;
        role_fashion_info fashion_info = serverProperty.fashion_info;

        if (fashion_info != null)
        {
            if (fashion_info.head_id != null)
            {
                data.mFashion.head_id = fashion_info.head_id.value;
                GameDebug.Log("同步Fashion head_id = " + data.mFashion.head_id);
            }

            if (fashion_info.uppper_body_id != null)
            {
                data.mFashion.upper_body_id = fashion_info.uppper_body_id.value;
                GameDebug.Log("同步Fashion upper_body_id = " + data.mFashion.upper_body_id);
            }

            if (fashion_info.lower_body_id != null)
            {
                data.mFashion.lower_body_id = fashion_info.lower_body_id.value;
                GameDebug.Log("同步Fashion lower_body_id = " + data.mFashion.lower_body_id);
            }


            if (fashion_info.unlock_count != null)
            {
                data.mFashion.unlock_count = fashion_info.unlock_count.value;
                GameDebug.Log("同步Fashion unlock_count = " + data.mFashion.unlock_count);
            }

            if (fashion_info.items != null)
            {
                for (int i = 0; i < data.mFashion.unlock_count; ++i)
                {
                    data.mFashion.items[i].id = fashion_info.items[i].id;
                    data.mFashion.items[i].starnum = fashion_info.items[i].starnum;
                    GameDebug.Log("同步Fashion items = " + data.mFashion.items[i].id + " starnum =" + data.mFashion.items[i].starnum);
                }
                
            }

            EventSystem.Instance.PushEvent(new FashionEvent(FashionEvent.FASHION_UPDATE));
        }
    }

    public uint GetTotalChargeNum()
    {
        return PlayerDataPool.Instance.MainData.mTotalChargeData.TotalNum;
    }

    public bool GetTotalChargeRewardGot(int rewardIdx)
    {
        BitArray ba = PlayerDataPool.Instance.MainData.mTotalChargeData.Flags;
        
        if (ba == null || rewardIdx < 1 || ba.Count < rewardIdx)
            return false;

        return ba[rewardIdx - 1];
    }

    private void SyncTotalChargeData(role_property serverProperty)
    {
        PlayerDataPool.Instance.MainData.mTotalChargeData.SyncTotalChargeData(serverProperty.totalcharge_info);
    }
}
