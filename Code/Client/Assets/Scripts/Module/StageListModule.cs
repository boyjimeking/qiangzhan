using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class StageListModule : ModuleBase
{
	// 子类型
	private SceneType mStageType;

	// 战区Id
	private int mZoneId;

	// 当前选中的UI
	private StageListItemUI mSelectedUI = null;

	// 当前选中的难度
	private int mSelectedLevel = 0;

	// 当前选中的关卡
	private Scene_StageSceneTableItem mSelectedStageItem = null;

	// 关卡列表
	public List<StageListItemUI> mStageItemList = new List<StageListItemUI>();

	// 显示难度
	private bool mShowLevelBar = true;

    //当前显示的关卡id；
    public int mCurStageId;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }



	// 打开关卡进入界面
	public void OpenStageListUI(SceneType type, int zoneId, int stageId = -1)
	{
		StageType = type;
	    ZoneId = zoneId;
	    if (stageId == -1&& mCurStageId!=-1)
	    {
	        stageId = mCurStageId;
	    }
        if (DataManager.Scene_StageSceneTable.ContainsKey(stageId))
		{
            SelectedStageRes = DataManager.Scene_StageSceneTable[stageId] as Scene_StageSceneTableItem;
		}

		WindowManager.Instance.OpenUI("stagelist");
	}

	// 关卡类型
	public SceneType StageType
	{
		get
		{
			return mStageType;
		}

		set
		{
			mStageType = value;
			mShowLevelBar = isShowLvBar();
		}
	}

    bool isShowLvBar()
    {
        return !IsActiveStage();
    }

    /// <summary>
    /// 是否是活动本儿;(逗比猫，僵尸本儿，魂能);
    /// </summary>
    /// <returns></returns>
    public bool IsActiveStage()
    {
		if (mStageType == SceneType.SceneType_Zombies ||
			mStageType == SceneType.SceneType_Mao ||
			mStageType == SceneType.SceneType_HunNeng ||
			mStageType == SceneType.SceneType_TD)
            return true;

        return false;
    }

	// 显示难度
	public bool ShowLevelBar
	{
		get
		{
			return mShowLevelBar;
		}
	}

	// 战区Id
	public int ZoneId
	{
		get
		{
			return mZoneId;
		}

		set
		{
			mZoneId = value;
		}
	}

	// 当前选中的UI
	public StageListItemUI SelectedUI
	{
		get
		{
			return mSelectedUI;
		}

		set
		{
			mSelectedUI = value;
			mSelectedLevel = 0;

			Scene_StageListTableItem listres = mSelectedUI.GetData();
			if (listres == null)
			{
				return;
			}

			if (!DataManager.Scene_StageSceneTable.ContainsKey(listres.mNromalStageId))
			{
				return;
			}

			mSelectedStageItem = DataManager.Scene_StageSceneTable[listres.mNromalStageId] as Scene_StageSceneTableItem;
		}
	}

	// 当前选中的难度
	public int SelectedLevel
	{
		get
		{
			return mSelectedLevel;
		}

		set
		{
			mSelectedLevel = value;

			if(mSelectedUI == null)
			{
				return;
			}

			Scene_StageListTableItem listres = mSelectedUI.GetData();
			if (listres == null)
			{
				return;
			}

			int stageId = -1;
			if(mSelectedLevel == 0)
			{
				stageId = listres.mNromalStageId;
			}
			else if(mSelectedLevel == 1)
			{
				stageId = listres.mHardStageId;
			}
			else if(mSelectedLevel == 2)
			{
				stageId = listres.mSeriousStageId;
			}

			if (!DataManager.Scene_StageSceneTable.ContainsKey(stageId))
			{
				return;
			}

			mSelectedStageItem = DataManager.Scene_StageSceneTable[stageId] as Scene_StageSceneTableItem;
		}
	}

	// 当前选中的关卡
	public Scene_StageListTableItem SelectedStageListRes
	{
		get
		{
			if(mSelectedUI == null)
			{
				return null;
			}

			return mSelectedUI.GetData();
		}
	}

	// 当前选中的关卡
	public Scene_StageSceneTableItem SelectedStageRes
	{
		get
		{
			return mSelectedStageItem;
		}

		set
		{
			mSelectedStageItem = value;
		}
	}

	// 得到战区第一个关卡
	public StageListItemUI GetHeadStageUI()
	{
		if(mStageItemList == null || mStageItemList.Count < 1)
		{
			return null;
		}

		return mStageItemList[0];
	}

	// UI列表
	public List<StageListItemUI> GetStageUIList()
	{
		return mStageItemList;
	}

	// 关卡个数
	public int GetStageListCount()
	{
		if(mStageItemList == null)
		{
			return 0;
		}

		return mStageItemList.Count;
	}

	// 添加
	public void AddStageListItem(StageListItemUI ui)
	{
		if(mStageItemList == null)
		{
			mStageItemList = new List<StageListItemUI>();
		}

		mStageItemList.Add(ui);
	}

    public void RemoveStageListFirstItem()
    {
        if ((mStageItemList == null) || (mStageItemList.Count < 1))
            return;

        GameObject.DestroyImmediate(mStageItemList[0].gameObject);
        mStageItemList.RemoveAt(0);
    }

	// 清空
	public void ClearStageListItem()
	{
		mSelectedUI = null;
		mSelectedLevel = 0;
		mSelectedStageItem = null;
		mStageItemList = new List<StageListItemUI>();
	}

	// 推荐战斗力
	public int GetSuitableFC()
	{
		if(SelectedStageRes == null)
		{
			return 0;
		}

		return SelectedStageRes.mSuitableFC;
	}

	// 消耗行动力
	public int GetCostSP()
	{
		if(SelectedStageRes == null)
		{
			return 0;
		}

		int costSp = 0;
		if(SelectedStageRes.mEnterCostSP > 0)
		{
			costSp += SelectedStageRes.mEnterCostSP;
		}

		if(SelectedStageRes.mAwardCostSP > 0)
		{
			costSp += SelectedStageRes.mAwardCostSP;
		}

		return costSp;
	}

	// 奖励经验
	public int GetAwardExp()
	{
		if(SelectedStageRes == null)
		{
			return 0;
		}

		return SelectedStageRes.mAwardExp;
	}

	// 是否有普通难度;
	public bool HasNormalLevel(Scene_StageListTableItem res)
	{
		if(res == null)
		{
			return false;
		}

        return DataManager.Scene_StageSceneTable.ContainsKey(res.mNromalStageId) && Convert.ToBoolean(res.mIsShowStar);
	}

	// 是否有精英难度
	public bool HasHardLevel(Scene_StageListTableItem res)
	{
		if (res == null)
		{
			return false;
		}

		return (DataManager.Scene_StageSceneTable.ContainsKey(res.mHardStageId));
	}

	// 是否有英雄难度
	public bool HasSeriousLevel(Scene_StageListTableItem res)
	{
		if (res == null)
		{
			return false;
		}

		return (DataManager.Scene_StageSceneTable.ContainsKey(res.mSeriousStageId));
	}

    public bool FindLastStage(SceneType sceneType, out int stageid)
    {
        stageid = -1;
        //包括min不包括max
        int minStageId = 0;
        int maxStageId = 0;
        switch (sceneType)
        {
            case SceneType.SceneType_Stage:
                minStageId = 100000;
                maxStageId = 200000;
                break;
            case SceneType.SceneType_Zombies:
                minStageId = 400000;
                maxStageId = 500000;
                break;
            case SceneType.SceneType_Mao:
                minStageId = 900000;
                maxStageId = 1000000;
                break;
            case SceneType.SceneType_TD:
                minStageId = 1300000;
                maxStageId = 1400000;
                break;

        }
        if (minStageId == maxStageId)
        {
            return false;
        }
        var iter = PlayerDataPool.Instance.MainData.mStageData.mPlayerStageData.GetEnumerator();
        while (iter.MoveNext())
        {
            if (iter.Current.Key >= minStageId && iter.Current.Key < maxStageId && iter.Current.Key > stageid)
            {
                stageid = iter.Current.Key;
               
            }
        }
        //GameDebug.Log("最大关卡id:"+ stageid);
        return true;
    }
}
