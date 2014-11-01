using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Message;

// 子类型数据
public class StageSubTypeData
{
	// 战区id->战区数据
	public Dictionary<int, StageFightZone> mFightZoneDatas = null;
}

// 战区数据
public class StageFightZone
{
	// 战区星星收集数
	public int mStarNum = 0;

	// 战区星星总数
	public int mMaxStarNum = 0;

	// 关卡列表 按解锁顺序整理好的
	public ArrayList mStageListIds = null;
}

// 战区中间数据
public class ZoneTempData
{
	// 头关
	//public int mHeadStageId;
	public List<int> mHeadStageIdList = new List<int>();

	// 关卡中间数据
	public Dictionary<int, StageTempData> mStageTempDatas = null;
}

// 关卡中间数据
public class StageTempData
{
	// 关卡Id
	public int stageId;

	// 前置关卡Id
	public int prevStageId;

	// 后继关卡Id
	public int nextStageId;
}

// 关卡解锁数据
public class StageUnlockData
{
	// 条件
	public ConditionTableItem condition;

	// 关卡id
	public int stageid;
}

// 处理关卡本身的数据 玩家关卡的数据通过PlayerDataModule操作
public class StageDataManager
{
    private static StageDataManager instance;

	// 子类型id->子类型数据数据
	private Dictionary<SceneType, StageSubTypeData> mSubTypeDatas = null;

	// 解锁条件id->解锁关卡id列表
	private Dictionary<ConditionType, ArrayList> mUnlockDatas = null;

	// 缓存服务器奖励
	private role_stageaward mAwards = null;

    public StageDataManager()
	{
		instance = this;

		EventSystem.Instance.addEventListener(StageSyncServerEvent.STAGE_SYNC_SERVER_EVENT, onSyncStageData);
        EventSystem.Instance.addEventListener(StageEnterEvent.STAGE_ENTER_RESPOND_EVENT, stageEnterSuccessed);
		EventSystem.Instance.addEventListener(PropertyEvent.MAIN_PROPERTY_CHANGE, onMainPropertyChange);
		EventSystem.Instance.addEventListener(QuestEvent.QUEST_ACCEPT, OnAcceptQuestResponse);
	}

    public static StageDataManager Instance
	{
		get
		{
			return instance;
		}
	}

	// 操作结果
	public void PrintErrorCode(ERROR_CODE code)
	{
		PopTipManager.Instance.AddNewTip(StringHelper.GetErrorString(code, FontColor.Red));
		//PromptUIManager.Instance.AddNewPrompt(StringHelper.GetErrorString(code));
	}

	// 检查是否可以进入关卡
	public bool CheckEnterStage(int stageId)
	{
		ERROR_CODE result = ERROR_CODE.ERR_SCENE_ENTER_OK;

		do
		{
			PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
			if (module == null)
			{
				result = ERROR_CODE.ERR_SCENE_ENTER_FAILED;
				break;
			}

			StageData psd = module.GetStageData(stageId);
			if (psd == null)
			{
				result = ERROR_CODE.ERR_SCENE_ENTER_FAILED;
				break;
			}

			if (!DataManager.Scene_StageSceneTable.ContainsKey(stageId))
			{
				result = ERROR_CODE.ERR_SCENE_ENTER_FAILED;
				break;
			}

			Scene_StageSceneTableItem ssti = DataManager.Scene_StageSceneTable[stageId] as Scene_StageSceneTableItem;
			if (ssti == null)
			{
				result = ERROR_CODE.ERR_SCENE_ENTER_FAILED;
				break;
			}

			if (ssti.mEnterTimes > 0 && module.GetStageDailyTimes(stageId) >= ssti.mEnterTimes)
			{
				result = ERROR_CODE.ERR_SCENE_ENTER_FAILED_NOTIMES;
				break;
			}

			return true;

		} while (true);

		PrintErrorCode(result);

		return false;
	}

	// 缓存服务器生成的奖励数据
	public void CacheServerAward(role_stageaward awards)
	{
		if(awards == null)
		{
			return;
		}

		mAwards = awards;
	}

	// 获得普通翻牌奖励
	public List<role_dropaward> GetNormalRandomAwards()
	{
		return mAwards.normal_random;
	}

	// 获得普通翻牌奖励
	public role_dropaward GetNormalRandomAward(int idx)
	{
		if(idx < 0 || idx >= mAwards.normal_random.Count)
		{
			return null;
		}

		return mAwards.normal_random[idx];
	}

	// 获得钻石翻牌奖励
	public List<role_dropaward> GetExtraRandomAwards()
	{
		return mAwards.extra_random;
	}

	// 获得钻石翻牌奖励
	public role_dropaward GetExtraRandomAward(int idx)
	{
		if(idx < 0 || idx >= mAwards.extra_random.Count)
		{
			return null;
		}

		return mAwards.extra_random[idx];
	}

	// 获得随机金币奖励
	public uint GetGoldAwards()
	{
		return mAwards.drop_gold;
	}

	// 获得首次击杀奖励
	public List<role_dropaward> GetFirstPassAwards()
	{
		return mAwards.first_award_id;
	}

	// 获得首次击杀奖励
	public role_dropaward GetFirstPassAwards(int idx)
	{
		if (idx < 0 || idx >= mAwards.first_award_id.Count)
		{
			return null;
		}

		return mAwards.first_award_id[idx];
	}

	// 获得Boss及小怪击杀奖励
	public List<role_dropaward> GetBossKillAwards()
	{
		return mAwards.dropbox_id;
	}

	// 获得Boss及小怪击杀奖励
	public role_dropaward GetBossKillAwards(int idx)
	{
		if (idx < 0 || idx >= mAwards.dropbox_id.Count)
		{
			return null;
		}

		return mAwards.dropbox_id[idx];
	}

	// 服务器同步关卡解锁
	public void SyncUnlockStage(int stageId)
	{
		PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
		if (module == null)
		{
			return;
		}

		if(module.IsStageUnlock(stageId))
		{
			return;
		}

		StageData stagedata = new StageData();
		stagedata.stageid = stageId;
		module.UpdateStageData(stagedata);

		StageUnlockEvent unlockevent = new StageUnlockEvent(StageUnlockEvent.STAGE_UNLOCK);
		unlockevent.stageId = stageId;
		EventSystem.Instance.PushEvent(unlockevent);
	}

	// 服务器同步角色关卡数据
	public void onSyncStageData(EventBase e)
	{
		PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();

		ClearZoneStarNum();

		mUnlockDatas = new Dictionary<ConditionType, ArrayList>();
		Dictionary<SceneType, Dictionary<int, ZoneTempData>> mTempDatas = new Dictionary<SceneType, Dictionary<int, ZoneTempData>>();
		System.Type type = typeof(Scene_StageSceneTableItem);
        IDictionaryEnumerator itr = DataManager.Scene_StageSceneTable.GetEnumerator();
        while (itr.MoveNext())
        {
            Scene_StageSceneTableItem res = itr.Value as Scene_StageSceneTableItem;
            ConditionTableItem condres = null;
            for (uint i = 0; i < Scene_StageSceneTableItem.MAX_UNLOCK_CONDITION_COUNT; ++i)
            {
                // 建立解锁条件->解锁关卡id的映射
                System.Reflection.FieldInfo fieldid = type.GetField("mUnlockCondId" + i.ToString());
                int unlockcondid = (int)fieldid.GetValue(res);
                if (unlockcondid >= 0 && DataManager.ConditionTable.ContainsKey(unlockcondid))
                {
                    ConditionTableItem condtableres = DataManager.ConditionTable[unlockcondid] as ConditionTableItem;
                    ArrayList unlocklist = null;
                    if (mUnlockDatas.ContainsKey(condtableres.mType))
                    {
                        unlocklist = mUnlockDatas[condtableres.mType];
                    }

                    if (unlocklist == null)
                    {
                        unlocklist = new ArrayList();
                        mUnlockDatas.Add(condtableres.mType, unlocklist);
                    }

                    StageUnlockData unlockdata = new StageUnlockData();
                    unlockdata.condition = condtableres;
                    unlockdata.stageid = res.resID;
                    unlocklist.Add(unlockdata);

                    if (condtableres.mType == ConditionType.STAGE_GRADE)
                    {
                        condres = condtableres;
                    }
                }
            }

            // 战区星星数
            if (module.IsStageHasPassed(res.resID) && mSubTypeDatas.ContainsKey(SceneManager.GetSceneType(res)))
            {
                Dictionary<int, StageFightZone> zonedata = mSubTypeDatas[SceneManager.GetSceneType(res)].mFightZoneDatas;
                if (zonedata != null && zonedata.ContainsKey(res.mZoneId))
                {
                    zonedata[res.mZoneId].mStarNum++;
                }
            }

            // 按解锁关系排序
            Dictionary<int, ZoneTempData> tempSubTypeData = null;
            if (mTempDatas.ContainsKey(SceneManager.GetSceneType(res)))
            {
                tempSubTypeData = mTempDatas[SceneManager.GetSceneType(res)];
            }

            if (tempSubTypeData == null)
            {
                tempSubTypeData = new Dictionary<int, ZoneTempData>();
                mTempDatas.Add(SceneManager.GetSceneType(res), tempSubTypeData);
            }

            ZoneTempData tempZoneData = null;
            if (tempSubTypeData.ContainsKey(res.mZoneId))
            {
                tempZoneData = tempSubTypeData[res.mZoneId];
            }

            if (tempZoneData == null)
            {
                tempZoneData = new ZoneTempData();
                tempSubTypeData.Add(res.mZoneId, tempZoneData);
            }

            if (tempZoneData.mStageTempDatas == null)
            {
                tempZoneData.mStageTempDatas = new Dictionary<int, StageTempData>();
            }

            StageTempData temp = new StageTempData();
            temp.stageId = res.resID;

            if (condres != null)
            {
                temp.prevStageId = condres.mParam1;
            }
            else
            {
                temp.prevStageId = -1;
                if (!tempZoneData.mHeadStageIdList.Contains(res.resID))
                {
                    tempZoneData.mHeadStageIdList.Add(res.resID);
                }
                //tempZoneData.mHeadStageId = res.resID;
            }
            tempZoneData.mStageTempDatas.Add(res.resID, temp);
        }
// 		foreach (Scene_StageSceneTableItem res in DataManager.Scene_StageSceneTable.Values)
// 		{
// 			
// 		}

		foreach (SceneType subtype in mTempDatas.Keys)
		{
			if (!mSubTypeDatas.ContainsKey(subtype))
			{
				continue;
			}

			StageSubTypeData subtypedata = mSubTypeDatas[subtype];
			Dictionary<int, ZoneTempData> tempsubdata = mTempDatas[subtype];
			foreach (int zoneid in tempsubdata.Keys)
			{
				if (!subtypedata.mFightZoneDatas.ContainsKey(zoneid))
				{
					continue;
				}

				StageFightZone fightzonedata = subtypedata.mFightZoneDatas[zoneid];
				fightzonedata.mStageListIds = new ArrayList();

				ZoneTempData tempzonedata = tempsubdata[zoneid];
				foreach (StageTempData tempstagedata in tempzonedata.mStageTempDatas.Values)
				{
					if (tempstagedata.prevStageId < 0 || !tempzonedata.mStageTempDatas.ContainsKey(tempstagedata.prevStageId))
					{
						continue;
					}

					tempzonedata.mStageTempDatas[tempstagedata.prevStageId].nextStageId = tempstagedata.stageId;
				}

				for (int i = 0; i < tempzonedata.mHeadStageIdList.Count; ++i)
				{
					int headId = tempzonedata.mHeadStageIdList[i];
					if (!tempzonedata.mStageTempDatas.ContainsKey(headId))
					{
						continue;
					}

					fightzonedata.mStageListIds.Add(headId);
				}

				for (int i = 0; i < tempzonedata.mHeadStageIdList.Count; ++i)
				{
					int headId = tempzonedata.mHeadStageIdList[i];
					if (!tempzonedata.mStageTempDatas.ContainsKey(headId))
					{
						continue;
					}

					StageTempData curdata = tempzonedata.mStageTempDatas[headId];
					if(curdata == null)
					{
						continue;
					}

					if (!tempzonedata.mStageTempDatas.ContainsKey(curdata.nextStageId))
					{
						continue;
					}

					StageTempData nextdata = tempzonedata.mStageTempDatas[curdata.nextStageId];
					do
					{
						fightzonedata.mStageListIds.Add(nextdata.stageId);

						nextdata = tempzonedata.mStageTempDatas.ContainsKey(nextdata.nextStageId) ? tempzonedata.mStageTempDatas[nextdata.nextStageId] : null;

					} while (nextdata != null);
				}
			}
		}
	}

	// 整理读表得到的数据
	public void InitDataStruct()
	{
		mSubTypeDatas = new Dictionary<SceneType, StageSubTypeData>();
        IDictionaryEnumerator itr = DataManager.Scene_StageSceneTable.GetEnumerator();
        while (itr.MoveNext())
        {
            Scene_StageSceneTableItem sceneres = itr.Value as Scene_StageSceneTableItem;
            StageSubTypeData subdata = null;
            if (mSubTypeDatas.ContainsKey(SceneManager.GetSceneType(sceneres)))
            {
                subdata = mSubTypeDatas[SceneManager.GetSceneType(sceneres)];
            }

            if (subdata == null)
            {
                subdata = new StageSubTypeData();
                mSubTypeDatas.Add(SceneManager.GetSceneType(sceneres), subdata);
            }

            if (subdata.mFightZoneDatas == null)
            {
                subdata.mFightZoneDatas = new Dictionary<int, StageFightZone>();
            }

            StageFightZone zonedata = null;
            if (subdata.mFightZoneDatas.ContainsKey(sceneres.mZoneId))
            {
                zonedata = subdata.mFightZoneDatas[sceneres.mZoneId];
            }

            if (zonedata == null)
            {
                zonedata = new StageFightZone();
                subdata.mFightZoneDatas.Add(sceneres.mZoneId, zonedata);
            }

            zonedata.mMaxStarNum++;
        }
// 		foreach(Scene_StageSceneTableItem sceneres in DataManager.Scene_StageSceneTable.Values)
// 		{
// 			
// 		}
	}

    public void stageEnterSuccessed(EventBase evt)
    {
		StageEnterEvent stageevent = evt as StageEnterEvent;

//         PlayerStageData playerdata = null;
// 		if (mPlayerStageData.ContainsKey(stageevent.mStageId))
//         {
// 			playerdata = mPlayerStageData[stageevent.mStageId];
//         }
//         else
//         {
//             playerdata = new PlayerStageData();
// 			playerdata.unlocked = true;
// 			mPlayerStageData.Add(stageevent.mStageId, playerdata);
//         }
//         
//         playerdata.entertimes ++;
    }

	// 服务器同步关卡通关数据
	public void SyncStagePass(role_stage stagedata)
	{
		if(stagedata == null)
		{
			return;
		}

		if (!DataManager.Scene_StageSceneTable.ContainsKey(stagedata.stage_id))
		{
			return;
		}

		Scene_StageSceneTableItem passstageres = DataManager.Scene_StageSceneTable[stagedata.stage_id] as Scene_StageSceneTableItem;
		if(passstageres == null)
		{
			return;
		}

		PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
		if(module == null)
		{
			return;
		}

		bool firstPass = !module.IsStageHasPassed((int)stagedata.stage_id);

		// 更新角色的关卡数据
		StageData newdata = new StageData();
		newdata.stageid = (int)stagedata.stage_id;
		newdata.maxgrade = (StageGrade)stagedata.max_grade;
		newdata.maxcombo = stagedata.max_combo;
		newdata.killrate = stagedata.kill_rate;
		newdata.passtimerecord = stagedata.passtime_record;
		newdata.passtimes = stagedata.pass_times;

		module.UpdateStageData(newdata);

		if(passstageres.mEnterTimes > 0)
		{
			StageDailyData newdailydata = new StageDailyData();
			newdailydata.stageid = (int)stagedata.stage_id;
			newdailydata.daily_times = module.GetStageDailyTimes((int)stagedata.stage_id) + 1;
			module.UpdateStageDailyData(newdailydata);
		}

		// 更新战区星星数
		if(firstPass && mSubTypeDatas.ContainsKey(SceneManager.GetSceneType(passstageres)))
		{
			Dictionary<int, StageFightZone> zonedata = mSubTypeDatas[SceneManager.GetSceneType(passstageres)].mFightZoneDatas;
			if (zonedata != null && zonedata.ContainsKey(passstageres.mZoneId))
			{
				zonedata[passstageres.mZoneId].mStarNum++;
			}
		}

		if(!mUnlockDatas.ContainsKey(ConditionType.STAGE_GRADE))
		{
			return;
		}

		ArrayList list = mUnlockDatas[ConditionType.STAGE_GRADE];
		if(list == null)
		{
			return;
		}

		// 检查是否有因此解锁的关卡
		System.Type type = typeof(Scene_StageSceneTableItem);
		foreach(StageUnlockData unlockdata in list)
		{
			if(unlockdata.condition == null)
			{
				continue;
			}

			if (unlockdata.condition.mParam1 != stagedata.stage_id)
			{
				continue;
			}

			if(!DataManager.Scene_StageSceneTable.ContainsKey(unlockdata.stageid))
			{
				continue;
			}

			bool success = true;
			Scene_StageSceneTableItem stageres = DataManager.Scene_StageSceneTable[unlockdata.stageid] as Scene_StageSceneTableItem;
			for(uint i = 0; i < Scene_StageSceneTableItem.MAX_UNLOCK_CONDITION_COUNT; ++i)
			{
				System.Reflection.FieldInfo fieldid = type.GetField("mUnlockCondId" + i.ToString());
				int unlockcondid = (int)fieldid.GetValue(stageres);
				if (unlockcondid < 0)
				{
					continue;
				}

				if (!ConditionManager.Instance.CheckCondition(unlockcondid))
				{
					success = false;
					break;
				}
			}
			
			// 有因此解锁的关卡
			if(success)
			{
				UnlockStageActionParam param = new UnlockStageActionParam();
				param.stageid = unlockdata.stageid;
				Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_SCENE_UNLOCK, param);
			}
		}

		StagePassServerEvent stageevent = new StagePassServerEvent(StagePassServerEvent.STAGE_PASS_SERVER_EVENT);
		stageevent.mStageData = newdata;
		EventSystem.Instance.PushEvent(stageevent);

		SceneManager.Instance.RequestEnterLastCity();
	}

	// 主属性变更
	public void onMainPropertyChange(EventBase evt)
	{
		PropertyEvent propertyevent = evt as PropertyEvent;

		if (!System.Enum.IsDefined(typeof(MainPropertyType), propertyevent.propertyId))
		{
			return;
		}

		if (propertyevent.propertyId != (int)MainPropertyType.PropertyType_Level)
		{
			return;
		}

		if (!mUnlockDatas.ContainsKey(ConditionType.LEVEL))
		{
			return;
		}

		ArrayList list = mUnlockDatas[ConditionType.LEVEL];
		if (list == null)
		{
			return;
		}

		// 检查是否有因此解锁的关卡
		System.Type type = typeof(Scene_StageSceneTableItem);
		foreach (StageUnlockData unlockdata in list)
		{
			if (unlockdata.condition == null)
			{
				continue;
			}

			if (unlockdata.condition.mValue != propertyevent.newValue)
			{
				continue;
			}

			if (!DataManager.Scene_StageSceneTable.ContainsKey(unlockdata.stageid))
			{
				continue;
			}

			bool success = true;
			Scene_StageSceneTableItem stageres = DataManager.Scene_StageSceneTable[unlockdata.stageid] as Scene_StageSceneTableItem;
			for (uint i = 0; i < Scene_StageSceneTableItem.MAX_UNLOCK_CONDITION_COUNT; ++i)
			{
				System.Reflection.FieldInfo fieldid = type.GetField("mUnlockCondId" + i.ToString());
				int unlockcondid = (int)fieldid.GetValue(stageres);
				if (unlockcondid < 0)
				{
					continue;
				}

				if (!ConditionManager.Instance.CheckCondition(unlockcondid))
				{
					success = false;
					break;
				}
			}

			// 有因此解锁的关卡
			if (success)
			{
				UnlockStageActionParam param = new UnlockStageActionParam();
				param.stageid = unlockdata.stageid;
				Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_SCENE_UNLOCK, param);
			}
		}
	}

	// 前置关卡Id
	public int GetPervStageId(int stageId)
	{
		if(!DataManager.Scene_StageSceneTable.ContainsKey(stageId))
		{
			return -1;
		}

		int ret = -1;
		Scene_StageSceneTableItem stageres = DataManager.Scene_StageSceneTable[stageId] as Scene_StageSceneTableItem;
		System.Type type = typeof(Scene_StageSceneTableItem);
		for (uint i = 0; i < Scene_StageSceneTableItem.MAX_UNLOCK_CONDITION_COUNT; ++i)
		{
			System.Reflection.FieldInfo fieldid = type.GetField("mUnlockCondId" + i.ToString());
			int unlockcondid = (int)fieldid.GetValue(stageres);
			if (unlockcondid < 0)
			{
				continue;
			}

			if(!DataManager.ConditionTable.ContainsKey(unlockcondid))
			{
				continue;
			}

			ConditionTableItem condres = DataManager.ConditionTable[unlockcondid] as ConditionTableItem;
			if(condres.mType != ConditionType.STAGE_GRADE)
			{
				continue;
			}

			ret = condres.mParam1;
		}

		return ret;
	}

	// 战区当前星星数
	public int GetZoneCurrentStarNum(SceneType type, int zoneId)
	{
		if(mSubTypeDatas == null || !mSubTypeDatas.ContainsKey(type))
		{
			return 0;
		}

		Dictionary<int, StageFightZone> fightZoneDatas = mSubTypeDatas[type].mFightZoneDatas;
		if (fightZoneDatas == null || !fightZoneDatas.ContainsKey(zoneId))
		{
			return 0;
		}

		return fightZoneDatas[zoneId].mStarNum;
	}

	// 战区星星总数
	public int GetZoneMaxStarNum(SceneType type, int zoneId)
	{
		if (mSubTypeDatas == null || !mSubTypeDatas.ContainsKey(type))
		{
			return 0;
		}

		Dictionary<int, StageFightZone> fightZoneDatas = mSubTypeDatas[type].mFightZoneDatas;
		if (fightZoneDatas == null || !fightZoneDatas.ContainsKey(zoneId))
		{
			return 0;
		}

		return fightZoneDatas[zoneId].mMaxStarNum;
	}

	// 得到关卡有序列表
	public ArrayList GetSortedStageList(SceneType type, int zoneId)
	{
		if (mSubTypeDatas == null || !mSubTypeDatas.ContainsKey(type))
		{
			return null;
		}

		Dictionary<int, StageFightZone> fightZoneDatas = mSubTypeDatas[type].mFightZoneDatas;
		if (fightZoneDatas == null || !fightZoneDatas.ContainsKey(zoneId))
		{
			return null;
		}

		return fightZoneDatas[zoneId].mStageListIds;
	}

	// 得到关卡解锁等级
	public int GetStageUnlockLevel(int stageId)
	{
		if(!DataManager.Scene_StageSceneTable.ContainsKey(stageId))
		{
			return 0;
		}

		System.Type type = typeof(Scene_StageSceneTableItem);
		Scene_StageSceneTableItem listRes = DataManager.Scene_StageSceneTable[stageId] as Scene_StageSceneTableItem;
		for (uint i = 0; i < Scene_StageSceneTableItem.MAX_UNLOCK_CONDITION_COUNT; ++i)
		{
			System.Reflection.FieldInfo fieldid = type.GetField("mUnlockCondId" + i.ToString());
			int unlockcondid = (int)fieldid.GetValue(listRes);
			if (unlockcondid < 0)
			{
				continue;
			}

			if(!DataManager.ConditionTable.ContainsKey(unlockcondid))
			{
				continue;
			}

			ConditionTableItem condRes = DataManager.ConditionTable[unlockcondid] as ConditionTableItem;
			if(condRes.mType == ConditionType.LEVEL)
			{
				return condRes.mValue;
			}
		}

		return 0;
	}

	private void ClearZoneStarNum()
	{
		if (mSubTypeDatas != null)
		{
			foreach (StageSubTypeData sstd in mSubTypeDatas.Values)
			{
				if (sstd.mFightZoneDatas != null)
				{
					foreach (StageFightZone sfz in sstd.mFightZoneDatas.Values)
					{
						sfz.mStarNum = 0;
					}
				}
			}
		}
	}

	private void OnAcceptQuestResponse(EventBase evt)
	{
		QuestEvent e = evt as QuestEvent;

		if (!mUnlockDatas.ContainsKey(ConditionType.QUEST_ACCEPT))
		{
			return;
		}

		ArrayList list = mUnlockDatas[ConditionType.QUEST_ACCEPT];
		if (list == null)
		{
			return;
		}

		// 检查是否有因此解锁的关卡
		System.Type type = typeof(Scene_StageSceneTableItem);
		foreach (StageUnlockData unlockdata in list)
		{
			if (unlockdata.condition == null)
			{
				continue;
			}

			if (unlockdata.condition.mParam1 != e.mQuestId)
			{
				continue;
			}

			if (!DataManager.Scene_StageSceneTable.ContainsKey(unlockdata.stageid))
			{
				continue;
			}

			bool success = true;
			Scene_StageSceneTableItem stageres = DataManager.Scene_StageSceneTable[unlockdata.stageid] as Scene_StageSceneTableItem;
			for (uint i = 0; i < Scene_StageSceneTableItem.MAX_UNLOCK_CONDITION_COUNT; ++i)
			{
				System.Reflection.FieldInfo fieldid = type.GetField("mUnlockCondId" + i.ToString());
				int unlockcondid = (int)fieldid.GetValue(stageres);
				if (unlockcondid < 0)
				{
					continue;
				}

				if (!ConditionManager.Instance.CheckCondition(unlockcondid))
				{
					success = false;
					break;
				}
			}

			// 有因此解锁的关卡
			if (success)
			{
				UnlockStageActionParam param = new UnlockStageActionParam();
				param.stageid = unlockdata.stageid;
				Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_SCENE_UNLOCK, param);
			}
		}
	}
}
