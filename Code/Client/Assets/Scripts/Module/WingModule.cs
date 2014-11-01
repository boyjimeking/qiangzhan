using System;
using System.Collections;
using System.Collections.Generic;
using Message;

public class WingModule:ModuleBase
{
	public int mCurPageNum;

    //是否需要重新装备
    public bool IsReEquip;
	public WingCommonTableItem GetCommonResByIndex(int index)
	{
	    int id = GetWingIdByIndex(index);
		WingCommonTableItem item =  DataManager.WingCommonTable[id] as WingCommonTableItem;
		return item;
	}
	public bool RequestActive()
	{

	    WingCommonTableItem commonRes = DataManager.WingCommonTable[GetWingIdByIndex(mCurPageNum)] as WingCommonTableItem;
        
	    int p;
	    if (CheckCondition(ref commonRes, out p))
		{
			Net.Instance.DoAction((int)MESSAGE_ID.ID_MSG_WING_ACTIVE,commonRes.id);
			
		}else
		{
			PromptUIManager.Instance.AddNewPrompt(StringHelper.GetString("wingcondition"));
			return false;
		}

		return true;
	}

	public bool RequestForge()
	{
		WingItemData wing_item_data; 
	    GetWingItemData(mCurPageNum,out wing_item_data);
	    WingCommonTableItem wing_common_res = DataManager.WingCommonTable[wing_item_data.id] as WingCommonTableItem;
		PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
         
		uint curCostNum = pdm.GetItemNumByID(wing_common_res.costId);
		if(curCostNum == 0)
		{
            MallFormModule mallModule = ModuleManager.Instance.FindModule<MallFormModule>();
            mallModule.OpenMallFormByItemId(wing_common_res.costId, "wing");
			return false;
		}
	    if (wing_item_data.level == (wing_common_res.middleLevel-1) || wing_item_data.level == (wing_common_res.hightLevel-1))
	    {
            IsReEquip = true;
	       
	    }
		Net.Instance.DoAction((int) MESSAGE_ID.ID_MSG_WING_FORGE,wing_item_data.id);
		return true;
	}

    
	public bool RequestEquip()
	{

		WingEquipActionParam param = new WingEquipActionParam();
        WingData wd = PlayerDataPool.Instance.MainData.mWingData;
        if(wd.mWearId == wd.wingItems[mCurPageNum -1].id)
        {
            param.action = 1;
        }
        else
        {
            param.action = 0;
        }

	    param.wingid = wd.wingItems[mCurPageNum - 1].id;
		Net.Instance.DoAction((int)MESSAGE_ID.ID_MSG_WING_EQUIP,param);
		return true;
	}

    public bool IsFullLevel()
    {
        WingItemData wing_item_data;
        if (GetWingItemData(mCurPageNum, out wing_item_data))
        {
            WingLevelTableItem level_res;
            GetWingLevelRes(wing_item_data.id, (int)wing_item_data.level, out level_res);

            if (wing_item_data.level == WingDefine.Max_Wing_Level)
            {
                return true;
            }
        }      
        return false;
    }

    public List<int> GetUnlockWing()
    {
        List<int> re = new List<int>();
        WingData wd = PlayerDataPool.Instance.MainData.mWingData;
        for (int i = 0; i < wd.wingItems.Count; ++i)
        {
            re.Add(wd.wingItems[i].id);
        }
        return re;
    }

    public string GetWingPicPath(int wingid)
    {
        WingCommonTableItem res = DataManager.WingCommonTable[wingid] as WingCommonTableItem;
        if (res == null) return "";
        return res.wingPicLow;
    }

    public int GetWingPageCount()
    {
        WingData wd = PlayerDataPool.Instance.MainData.mWingData;

        if (wd.wingItems.Count == DataManager.WingCommonTable.Count)
        {
            return wd.wingItems.Count;
        }
        else
        {
            return wd.wingItems.Count + 1;
        }
    
    }

    public int GetWingIdByIndex(int index)
    {
        IDictionaryEnumerator itr = DataManager.WingCommonTable.GetEnumerator();
        while( itr.MoveNext() )
        {
            WingCommonTableItem resItem = itr.Value as WingCommonTableItem;
            if (resItem.index == index)
                return resItem.id;
        }
//         foreach (DictionaryEntry wct in DataManager.WingCommonTable)
//         {
//             WingCommonTableItem resItem = wct.Value as WingCommonTableItem;
//             if (resItem.index == index)
//                 return resItem.id;
//         }

        return -1;
    }

    public bool GetWingLevelRes(int wingid, int level,out WingLevelTableItem levelRes)
    {
         levelRes = DataManager.WingLevelTable[wingid * 1000 + level] as WingLevelTableItem;
        return levelRes != null;
    }
    public bool GetWingItemData(int curPageNum,out WingItemData itemdata)
    {
        WingData wd = PlayerDataPool.Instance.MainData.mWingData;
        if (mCurPageNum > wd.wingItems.Count)
        {
            itemdata = null;
            GameDebug.Log("获取翅膀数ItemData据失败");
            return false;
        }
        itemdata = PlayerDataPool.Instance.MainData.mWingData.wingItems[curPageNum - 1];
        return true;
    }

    public bool CheckCondition( ref WingCommonTableItem commonRes ,out int finishProgress)
    {
        bool re = true;
        finishProgress = 0;
        for (int i = 0; i < WingDefine.MaxConditonNum; ++i)
        {
           Type t = commonRes.GetType();
           int condition = Convert.ToInt32(t.GetField("condition" + (i + 1)).GetValue(commonRes));
            if(condition == -1) continue;
            if (!QuestHelper.CheckCondition(condition))
            {
                re = false;
            }
            else
            {
                finishProgress |= 1 << i;
                //GameDebug.Log(commonRes.id + " 激活条件" + finishProgress);
            }
        }

        return re;
    }

    public WingState GetWingState(int pageIndex)
    {
        var wd = PlayerDataPool.Instance.MainData.mWingData;
        if (pageIndex <= PlayerDataPool.Instance.MainData.mWingData.wingItems.Count)
        {
            if (wd.wingItems[pageIndex - 1].id == wd.mWearId)
            {
                return WingState.Wear;
            }
            else
            {
                return WingState.UnLocked;
            }
        }
        return WingState.Locked;
    }

    public static string GetModelName(int wingid, uint winglevel)
    {
        WingCommonTableItem commonRes = DataManager.WingCommonTable[wingid] as WingCommonTableItem;
        if (commonRes == null)
        {
            GameDebug.Log("无效的翅膀id"+wingid);
            return "";
        }

        if (winglevel < commonRes.middleLevel)
        {
            return commonRes.modelLow;
        } 
        
        if (winglevel >= commonRes.middleLevel && winglevel < commonRes.hightLevel)
        {
            return commonRes.modelMid;
        }

        return commonRes.modelHigh;
    }

    public static int GetEffectId(int wingid, int winglevel)
    {
        WingCommonTableItem commonRes = DataManager.WingCommonTable[wingid] as WingCommonTableItem;
        if (commonRes == null)
        {
            GameDebug.Log("无效的翅膀id" + wingid);
            return -1;
        }

        if (winglevel < commonRes.middleLevel)
        {
            return -1;
           
        }

        if (winglevel >= commonRes.middleLevel)
        {
            if (SceneManager.Instance.GetCurScene() is CityScene)
                return commonRes.effectNomal;
            return commonRes.effectFight;
        }

        return -1;
    }

    public static string GetWingPic(int wingid, int winglevel)
    {
        WingCommonTableItem commonRes = DataManager.WingCommonTable[wingid] as WingCommonTableItem;
        if (commonRes == null)
        {
            GameDebug.Log("无效的翅膀id" + wingid);
            return "";
        }

        if (winglevel < commonRes.middleLevel)
        {
            return commonRes.wingPicLow;

        }

        if (winglevel >= commonRes.middleLevel && winglevel < commonRes.hightLevel)
        {
            
            return commonRes.wingPicMid;
        }

        return commonRes.wingPicHigh;
    }

    public uint GetWingLevel(int wingid)
    {
       WingItemData itemData = PlayerDataPool.Instance.MainData.mWingData.GetWingItemDataById(wingid);
        if (itemData == null)
        {
            return UInt32.MaxValue;
        }
        return itemData.level;      
    }

    public string GetWingPicAni(int wingid)
    {
        WingCommonTableItem commonRes = DataManager.WingCommonTable[wingid] as WingCommonTableItem;
        if (commonRes == null)
        {
            GameDebug.Log("无效的翅膀id" + wingid);
            return "";
        }

        return commonRes.wingPicAni;
    }
}
