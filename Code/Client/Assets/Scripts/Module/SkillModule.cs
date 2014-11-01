using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Message;
using System;
public class SkillItemInfo
{
	public string spriteInfo;//图集名:Sprite名
}

public class SkillModule : ModuleBase
{
	///技能槽开发等级规则;
    //public static readonly int[] openLv = new int[]{1 , 1 , 1 , 1};
    public static readonly int[] openLv = new int[] { 1, 1, 1 };

	//最新一次人物等级信息;
	private int mLastLv = -1;
    private Dictionary<int, int> mLastSkills = new Dictionary<int, int>();

	protected override void OnEnable()
	{
		EventSystem.Instance.addEventListener(PlayerDataEvent.PLAYER_DATA_CHANGED , onPlayerDataChanged);
	}
	
	protected override void OnDisable()
	{
		EventSystem.Instance.removeEventListener(PlayerDataEvent.PLAYER_DATA_CHANGED , onPlayerDataChanged);
	}

	void onPlayerDataChanged(EventBase ev)
	{
		PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
		if(pdm == null)
			return;

		int lv = pdm.GetLevel();
		if(mLastLv == lv)
			return;

        skillOpenChecker(lv);

        ////等级改变是否导致技能槽解锁了;
        //int lastOpenSlotNum = GetUnlockedSlotNum(mLastLv);
        //int newOpenSlotNum = GetUnlockedSlotNum(lv);

        //mLastLv = lv;

        ////如果人物等级改变导致技能槽解锁，
        ////通知技能槽列表刷新;
        //if(newOpenSlotNum != lastOpenSlotNum)
        //{

        //    ArrayList slotIdx = new ArrayList();//记录的slotIdx有效范围[0...3]
        //    for(int i = lastOpenSlotNum ; i < newOpenSlotNum ; i++)
        //    {
        //        slotIdx.Add(i);
        //    }

        //    SkillUIEvent sue = new SkillUIEvent(SkillUIEvent.SKILL_SLOT_CHANGE);
        //    sue.msg = slotIdx;
        //    EventSystem.Instance.PushEvent(sue);
        //}


        ////如果人物等级改变导致技能列表新技能解锁，
        ////通知技能列表刷新;
        //PlayerSkillData pd = pdm.GetSkillData();
        //if (pd == null) return;

        //ArrayList skillIds = new ArrayList();
        //foreach(int keys in pd.mLevels.Keys)
        //{
        //    int val = pd.mLevels[keys];
        //    if(mLastSkills.ContainsKey(keys))
        //    {
        //        if(mLastSkills[keys] == val)
        //            continue;

        //        mLastSkills[keys] = val;
        //        skillIds.Add(keys);
        //    }
        //    else
        //    {
        //        mLastSkills.Add(keys , val);
        //        skillIds.Add(keys);
        //    }
        //}

        //SkillUIEvent sue1 = new SkillUIEvent(SkillUIEvent.SKILL_LIST_CHANGE);
        //sue1.msg = skillIds;
        //EventSystem.Instance.PushEvent(sue1);
	}

    /// <summary>
    /// 技能可解锁在本地判断，服务器只负责验证;
    /// </summary>
    void skillOpenChecker(int lv)
    {
        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if(pdm == null) return;

        PlayerSkillData skillData = null;

        IDictionaryEnumerator itr = DataManager.SkillLearnTable.GetEnumerator();
        while (itr.MoveNext())
        {
            SkillLearnTableItem item = itr.Value as SkillLearnTableItem;
            if (item == null)
                continue;

            if (lv < item.unlock_lv)
                continue;

            skillData = pdm.GetSkillData();
            if (skillData == null || skillData.mLevels.ContainsKey(item.id))
                continue;

            pdm.UpdateSkillData(item.id, 0);
        }
    }

    public bool canOpen(int skillId)
    {
        if (!DataManager.SkillLearnTable.ContainsKey(skillId))
            return false;
        
        SkillLearnTableItem item = DataManager.SkillLearnTable[skillId] as SkillLearnTableItem;
        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();

        if (item == null || pdm == null)
            return false;

        return pdm.GetLevel() >= item.unlock_lv;
    }

	/// <summary>
	/// 根据技能ID判断该技能是否满级;
	/// </summary>
	/// <returns><c>true</c> if this instance is full lv the specified skillID; otherwise, <c>false</c>.</returns>
	/// <param name="skillID">Skill I.</param>
	public bool IsFullLv(int skillID)
	{
		int lv = GetSkillLvBySkillID(skillID);

		//等级有效，且满级;
		return  (lv > 0) && (lv >= GetSkillMaxLvByID(skillID));
	}

	/// <summary>
	/// 判断技能槽是否为未解锁状态
	/// 参数[1..SkillMaxCountDefine.MAX_EQUIP_SKILL_NUM]
	/// </summary>
	/// <returns><c>true</c> if this instance is slot locked the specified slotIdx; otherwise, <c>false</c>.</returns>
	public bool IsSlotLocked(int slotIdx)
	{
		PlayerData pd = PlayerDataPool.Instance.MainData;
		if(pd == null)
			return true;

		if(slotIdx < 1 || slotIdx > SkillMaxCountDefine.MAX_EQUIP_SKILL_NUM)
			return true;

		if(openLv.Length != SkillMaxCountDefine.MAX_EQUIP_SKILL_NUM)
		{
#if UNITY_EDITOR
			GameDebug.LogError("技能槽开放等级长度和最大装备技能数不同");
#endif
			return true;
		}

		return pd.level < openLv[slotIdx - 1];
	}

	/// <summary>
	/// 判断该技能ID是有效的技能ID么;
	/// </summary>
	public bool IsLegalSkillID(int skillID)
	{
		return DataManager.SkillLearnTable.ContainsKey(skillID);
	}

	/// <summary>
	/// 根据技能的ID获取当前该技能的培养等级;
	/// 初始等级为1级;
	/// 返回值为 < -1时为错误的skillID;
	/// 返回值为 = -1时为未解锁的技能;
	/// 返回值为 = 0时为可解锁而未解锁的技能;
	/// 返回值为 > 1时为解锁了的技能等级;
	/// </summary>
	/// <returns>The skill lv by skill I.</returns>
	/// <param name="skillID">Skill I.</param>
	public int GetSkillLvBySkillID(int skillID)
	{
		PlayerData data = PlayerDataPool.Instance.MainData;
		
		if(data == null)
		{
			Debug.LogError("MainData is Null");
			return -3;
		}

		if(!IsLegalSkillID(skillID))
			return -2;

		int lv = 0;

		if(data != null && data.skillData.mLevels.ContainsKey(skillID))
		{
			lv = data.skillData.mLevels[skillID];
		}
		
		return lv;
	}

	/// <summary>
	/// 根据技能ID获得该技能的最高等级;
	/// </summary>
	/// <returns>The skill max lv by I.</returns>
	/// <param name="skillID">Skill I.</param>
	public int GetSkillMaxLvByID(int skillID)
	{
		int count = 0;
        IDictionaryEnumerator itr = DataManager.SkillLevelTable.GetEnumerator();
        while (itr.MoveNext())
        {

            if (SkillLvIsSkillLearn(skillID, Convert.ToInt32( itr.Key)))
            {
                count++;
            }
        }
// 		foreach(int key in DataManager.SkillLevelTable.Keys)
// 		{
// 			if(SkillLvIsSkillLearn(skillID , key))
// 			{
// 				count ++;
// 			}
// 		}

		return count;
	}

	/// <summary>
	/// 获得所有技能的个数
	/// </summary>
	public int GetAllSkillsNum()
	{
		return DataManager.SkillLearnTable.Count;
	}

	/// <summary>
	/// 获得当前解锁技能的个数;
	/// </summary>
	/// <returns>The un locked skill number.</returns>
	public int GetUnLockedSkillNum()
	{
		PlayerData data = PlayerDataPool.Instance.MainData;
		if(data == null)
		{
			Debug.LogError("MainData is Null!");
			return 0;
		}

		//MainData上存储的是已经解锁后的技能的信息;
		return data.skillData.mLevels.Count;
	}

	/// <summary>
	/// 获得当前等级lv，开放的技能槽的个数
	/// </summary>
	/// <returns>The unlocked slot number.</returns>
	int GetUnlockedSlotNum(int lv)
	{
//		PlayerData pd = PlayerDataPool.Instance.MainData;
//		if(pd == null)
//		{
//			Debug.LogError("MainData is Null");
//			return 0;
//		}

		int i = 0 , j = openLv.Length;
		for( ; i < j ; i++)
		{
			if(lv < openLv[i])
				break;
		}

		return i == 0 ? i : i + 1;
	}

	/// <summary>
	/// 根据SkillID判断，该技能在技能槽中的位置[0...3]
	/// falseult < 0 表示不存在;
	/// </summary>
	/// <returns>The slot index by skill I.</returns>
	public bool GetSlotIdxBySkillID(int skillID , ref int slotIdx)
	{
		PlayerData pd = PlayerDataPool.Instance.MainData;
		if(pd == null)
		{
			Debug.LogError("MainData is Null");
			return false;
		}

        PlayerSkillData skillData = pd.skillData;

		slotIdx = -1;
        for (int i = 0, j = skillData.skills.Length; i < j; i++)
		{
            if (skillData.skills[i] == skillID)
			{
				slotIdx = i;
                return true;
			}
		}
		return false;
	}

	///技能升级;
	public bool Upgrade(int skillId)
	{
		SkillLevelTableItem item = GetDetailByCurLvSkillID(skillId);
		
		if(item == null)
			return false;

		PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
		if(pdm == null)
			return false;

        ProceedsType pt = (ProceedsType) item.money_type;
		uint moneyNum = pdm.GetProceeds(pt);
		if(item.money_num > moneyNum)
		{
            //Debug.LogError("钱不够了，升级失败");
            if(pt == ProceedsType.Money_Game)
                PopTipManager.Instance.AddNewTip(StringHelper.StringWithColor(FontColor.Red, StringHelper.GetString("skill_lvup_nomoney")));
            else if(pt == ProceedsType.Money_RMB)
                PopTipManager.Instance.AddNewTip(StringHelper.StringWithColor(FontColor.Red, StringHelper.GetString("skill_lvup_normb")));
			return false;
		}

        if (pdm.GetLevel() < item.level)
        {
            //人物等级不足，技能升级失败;
            PopTipManager.Instance.AddNewTip(StringHelper.StringWithColor(FontColor.Red, StringHelper.GetString("skill_lvup_nolevel")));
            return false;
        }

        SkillLevelActionParam param = new SkillLevelActionParam();
        param.SkillIdxs.Add(skillId);

        Net.Instance.DoAction((int)MESSAGE_ID.ID_MSG_SKILL, param);

//         pdm.ChangeProceeds((ProceedsType)item.money_type, (int)(moneyNum - item.money_num));
// 		PlayerData pd = PlayerDataPool.Instance.MainData;
// 		int tempLv = pd.skillData.mLevels[skillId];
// 		pd.skillData.mLevels[skillId] = tempLv + 1;
		return true;
	}

    public bool SetSkillLevel(int skillId , int level)
    {
//         SkillLevelTableItem item = GetDetailByCurLvSkillID(skillId);
//         if (item == null)
//             return false;
        PlayerData pd = PlayerDataPool.Instance.MainData;

 //       int tempLv = pd.skillData.mLevels[skillId];
        pd.skillData.mLevels[skillId] = level;

        //通知界面更新
        SkillUIEvent sue = new SkillUIEvent(SkillUIEvent.SKILL_LEVEL_UP);
        sue.skillId = skillId;
        sue.skillLv = level;
        EventSystem.Instance.PushEvent(sue);

        return true;
    }

//     public bool SetSkillEquip(int[] equips )
//     {
//         PlayerData pd = PlayerDataPool.Instance.MainData;
//         for (int i = 0; i < equips.Length && i < SkillMaxCountDefine.MAX_EQUIP_SKILL_NUM;++i )
//         {
//             pd.skillData.skills[i] = equips[i];
//         }
// 
//         //更新界面 
//         SkillUIEvent sue = new SkillUIEvent(SkillUIEvent.SKILL_EQUIP);
//         EventSystem.Instance.PushEvent(sue);
// 
//         return true;
//    
    public void GiveSkills(List<string> skills)
    {
        SkillLevelActionParam param = new SkillLevelActionParam();

        for (int i = 0; i < skills.Count; ++i)
        {
            int skillidx = int.Parse(skills[i]);
            if (GetSkillLvBySkillID(skillidx) <= 0)
            {
                param.SkillIdxs.Add(skillidx);
                param.DefaultEquips.Add(skillidx - 1);
            }
        }
        if( param.SkillIdxs.Count > 0 )
        {
            Net.Instance.DoAction((int)MESSAGE_ID.ID_MSG_SKILL, param);
        }
    }

    public void UnlockWeaponSkill()
    {
        if( PlayerDataPool.Instance.MainData.weapon_skill_unlocked != true )
        {
            SkillWeaponSkillParam param = new SkillWeaponSkillParam();
            Net.Instance.DoAction((int)MESSAGE_ID.ID_MSG_SKILL, param);
        }
    }

    public bool IsWeaponSkillUnlocked()
    {
        return PlayerDataPool.Instance.MainData.weapon_skill_unlocked;
    }

	///技能解锁;
	public void UnLock(int skillId)
	{
        if( GetSkillLvBySkillID( skillId ) <= 0 )
        {
            //直接向服务器发送 升级请求 服务器会判定 0级技能 需要解锁
            SkillLevelActionParam param = new SkillLevelActionParam();
            param.SkillIdxs.Add(skillId);
            param.DefaultEquips.Add(-1);
            Net.Instance.DoAction((int)MESSAGE_ID.ID_MSG_SKILL, param);
        }


// 		PlayerData pd = PlayerDataPool.Instance.MainData;
// 
// 		//可以解锁状态下的技能才可以解锁成功;
// 		if(!pd.skillData.mLevels.ContainsKey(skillId) || pd.skillData.mLevels[skillId] != 0)
// 			return;
// 
// 		pd.skillData.mLevels[skillId] = 1;
	}

	///解锁技能槽;
	public void UnLockSlot(int skillSlot)
	{

	}

	/// <summary>
	/// 装备/切换技能;
	/// idx : 技能槽索引[1,4];
	/// skillId : 技能Id;
	/// </summary>
	public void EquipSkill(int idx , int skillId)
	{
		PlayerData pd = PlayerDataPool.Instance.MainData;
		if(pd == null)
	    {
			Debug.LogError("MainData is Null");
			return;
		}

        if (idx < 1 || idx > SkillMaxCountDefine.MAX_EQUIP_SKILL_NUM)
		{
			Debug.LogError("技能槽索引值无效");
			return;
		}

		if(IsSlotLocked(idx))
		{
			Debug.LogError("技能槽还没有解锁，装备技能失败");
			return;
		}

        PlayerSkillData skillData = pd.skillData;

		idx -= 1;
		//要装备的技能在技能槽中存在且在技能槽中的位置不变，不做操作;
        if (skillData.skills[idx] == skillId)
			return;

        int[] skillIds = new int[4];
        skillIds = skillData.skills;

        int slotIdx = -1;
        if (!GetSlotIdxBySkillID(skillId, ref slotIdx))
        {
            skillIds[idx] = skillId;
        }
        //如果要装备的技能在技能槽中存在，在技能槽的位置改变，那么互换该技能原来位置和现在的技能槽的位置;
        else
        {
            int temp = skillData.skills[idx];

            skillIds[idx] = skillId;
            skillIds[slotIdx] = temp;
        }

        SkillEquipActionParam param = new SkillEquipActionParam();
        param.EquipIdx = skillIds;
        Net.Instance.DoAction((int)MESSAGE_ID.ID_MSG_SKILL, param);

        #region
        //int slotIdx = -1;
        ////记录哪个位置的技能槽发生改变;
        //ArrayList changeIdx = new ArrayList();
        ////记录哪个位置的技能发生改变;
        //ArrayList changeSkillIdx = new ArrayList();
        ////要装备的技能在技能槽中不存在，直接替换为该技能;
        //if(!GetSlotIdxBySkillID(skillId , ref slotIdx))
        //{
        //    changeSkillIdx.Clear();
        //    changeSkillIdx.Add(skillId);
        //    changeSkillIdx.Add(skillData.skills[idx]);

        //    skillData.skills[idx] = skillId;

        //    changeIdx.Clear();
        //    changeIdx.Add(idx);

        //}
        ////如果要装备的技能在技能槽中存在，在技能槽的位置改变，那么互换该技能原来位置和现在的技能槽的位置;
        //else
        //{
        //    int temp = skillData.skills[idx];

        //    skillData.skills[idx] = skillId;
        //    skillData.skills[slotIdx] = temp;

        //    changeIdx.Clear();
        //    changeIdx.Add(idx);
        //    changeIdx.Add(slotIdx);

        //    changeSkillIdx.Clear();
        //    changeSkillIdx.Add(temp);
        //    changeSkillIdx.Add(skillId);
        //}

        //SkillUIEvent se = new SkillUIEvent(SkillUIEvent.SKILL_SLOT_CHANGE);
        //se.msg = changeIdx;//记录的slotIdx有效范围[0...3]
        //EventSystem.Instance.PushEvent(se);

        //SkillUIEvent se1 = new SkillUIEvent(SkillUIEvent.SKILL_LIST_CHANGE);
        //se1.msg = changeSkillIdx;
        //EventSystem.Instance.PushEvent(se1);
        #endregion
    }



	/// <summary>
	/// 根据技能ID获得技能的所有信息;
	/// </summary>
	/// <returns>The detail by skill identifier.</returns>
	/// <param name="skillId">Skill identifier.</param>
	public SkillLearnTableItem GetDetailBySkillId(int skillId)
	{
// 		Hashtable table = DataManager.SkillLearnTable;

        if (DataManager.SkillLearnTable.ContainsKey(skillId))
		{
            return DataManager.SkillLearnTable[skillId] as SkillLearnTableItem;
		}

		return null;
	}

	/// <summary>
	/// 获取当前技能的等级变化信息
	/// </summary>
	/// <returns>The skill detail by I.</returns>
	public SkillLevelTableItem GetDetailByLevelID(int levelID)
	{
// 		Hashtable table = DataManager.SkillLevelTable;

        if (DataManager.SkillLevelTable.ContainsKey(levelID))
		{
            return DataManager.SkillLevelTable[levelID] as SkillLevelTableItem;
		}

		return null;
	}

	/// <summary>
    /// skillLevel表中填写的条件为（从上个等级）升到当前等级需要消耗的东西;
	/// 根据skillID指定的技能在玩家身上的等级来获取对应的SkillLevel表中的数据;
	/// lv 默认为该技能的当前培养等级;
	/// </summary>
	/// <returns>The detail by current lv skill I.</returns>
	public SkillLevelTableItem GetDetailByCurLvSkillID(int skillID , int lv = -1)
	{
        int skillLv = 0;
		if(lv < 0)
			skillLv = GetSkillLvBySkillID(skillID);
		if(skillLv < 0)
			return null;

        //if (IsFullLv(skillID)) return null;

		int levelID = SkillLearnToLevel(skillID , skillLv + 1);

		SkillLevelTableItem slti = GetDetailByLevelID(levelID);

		if(slti == null)
		{
#if UNITY_EDITOR
			GameDebug.LogError("Skill Level Table中不存在的ID" + levelID);
#endif
		}

		return slti;
	}

    /// <summary>
    /// 获取技能升级所需的人物等级;
    /// </summary>
    /// <param name="skillLvId"></param>
    /// <returns></returns>
    public static int GetPlayerLvBySkillLevelId(int skillLvId)
    {
        if (!DataManager.SkillLevelTable.ContainsKey(skillLvId))
        {
            return -1;
        }
        
        SkillLevelTableItem  item = DataManager.SkillLevelTable[skillLvId] as SkillLevelTableItem;
        if(item == null)
            return -1;

        return item.level;
    }

	///learn表ID 转换 level表ID
	public static int SkillLearnToLevel(int learnID , int level)
	{
		return (1000 * learnID) + level;
	}

	/// <summary>
	/// learn表ID(skillID) 是否对应 level表ID	
	/// </summary>
	public static bool SkillLvIsSkillLearn(int learnID , int LevelID)
	{
		return (LevelID / 1000) == learnID;
	}

	/// <summary>
	/// 获得装备的技能索引，参数技能槽索引，有效范围[0..3]
	/// </summary>
    public int GetEquipSkillID(int idx)
    {
        PlayerData pd = PlayerDataPool.Instance.MainData;
        if (pd == null)
            return -1;
        if (idx < 0 || idx >= SkillMaxCountDefine.MAX_EQUIP_SKILL_NUM)
            return -1;
        return pd.skillData.skills[idx];
    }

    public string GetSkillIconByID(int falseID)
    {
		if( !IsLegalSkillID(falseID) )
            return "";
        SkillLearnTableItem item = DataManager.SkillLearnTable[falseID] as SkillLearnTableItem;
        return item.skill_icon;
    }

	public int GetSkillUnlockLvByID(int skillID)
	{
		if( !IsLegalSkillID(skillID) )
			return -1;

		SkillLearnTableItem item = DataManager.SkillLearnTable[skillID] as SkillLearnTableItem;
		return item.unlock_lv;
	}

    public void UseWeaponSkill()
    {
		Player player = PlayerController.Instance.GetControlObj() as Player;

		if (player == null)
            return ;
		ErrorCode ec = player.UseWeaponSkill();

		// 武器技能不显示CD中...
        if (ec != ErrorCode.Succeeded && ec != ErrorCode.CoolingDown)
        {
            PromptUIManager.Instance.AddNewPrompt(ErrorHandler.Error2Prompt(ec));
        }
    }

    public int GetSkillCommonID(int falseID)
    {
        int level = GetSkillLvBySkillID(falseID);

        //未解锁
        if (level < 1)
        {
            return -1;
        }

        int lvID = SkillLearnToLevel(falseID, level);

        if (!DataManager.SkillLevelTable.ContainsKey(lvID))
            return -1;

        SkillLevelTableItem item = DataManager.SkillLevelTable[lvID] as SkillLevelTableItem;

        return item.skillid;
    }

    //需要取技能等级 ..
    public bool UseSkillByIdx(int index)
    {
        int falseId = GetEquipSkillID(index);

        if (falseId < 0)
            return false;


        int skillid = GetSkillCommonID(falseId);

        if (skillid < 0)
            return false;

		Player player = PlayerController.Instance.GetControlObj() as Player;

		if (player == null)
            return false;

		ErrorCode ec = player.UseSkill(skillid, Vector3.zero);
        if( ec != ErrorCode.Succeeded )
        {
            PromptUIManager.Instance.AddNewPrompt(ErrorHandler.Error2Prompt(ec));
            return false;
        }
        return true;
    }

	public bool UseSkillById(int resId)
	{
		Player player = PlayerController.Instance.GetControlObj() as Player;

		if (player == null)
			return false;

		ErrorCode ec = player.UseSkill(resId, Vector3.zero);
        if (ec != ErrorCode.Succeeded)
        {
            PromptUIManager.Instance.AddNewPrompt(ErrorHandler.Error2Prompt(ec));
        }

		return ec == ErrorCode.Succeeded;
	}

	public void ResetSkillCD(int resID)
	{
		Player player = PlayerController.Instance.GetControlObj() as Player;
		if (player != null)
			player.ResetSkillCD(resID);
	}

    public float GetSkillMaxCoolDownByID(int skillid)
    {
        if (skillid < 0)
            return 0.0f;
        SkillCommonTableItem skillfalse = DataManager.SkillCommonTable[skillid] as SkillCommonTableItem;
        if (skillfalse == null)
            return 0.0f;
        return skillfalse.myCd;
    }

    public float GetSkillMaxCoolDown(int index)
    {
        int falseId = GetEquipSkillID(index);
        if (falseId < 0)
            return 0.0f;
        int skillid = GetSkillCommonID(falseId);
        return GetSkillMaxCoolDownByID(skillid);
    }
    public float GetSkillCoolDownByID(int skillid)
    {
        if (skillid < 0)
            return 0.0f;

		Player player = PlayerController.Instance.GetControlObj() as Player;
		if (player == null)
            return 0.0f;
		return player.GetSkillCD(skillid);
    }
    public float GetSkillCoolDown(int index)
    {
        int falseId = GetEquipSkillID(index);
        if (falseId < 0)
            return 0.0f;
        int skillid = GetSkillCommonID(falseId);
        return GetSkillCoolDownByID(skillid);
    }

    public bool CheckSkillCostByID(int skillid)
    {
        if (skillid < 0)
            return false;

		Player player = PlayerController.Instance.GetControlObj() as Player;

		if (player == null)
            return false;

		ErrorCode ec = player.CheckSkillCost(skillid);
        return (ec == ErrorCode.Succeeded);
    }

    public bool CheckSkillCost(int index)
    {
        int falseId = GetEquipSkillID(index);

        if (falseId < 0)
            return false;


        int skillid = GetSkillCommonID(falseId);
        return CheckSkillCostByID(skillid);
    }
}
