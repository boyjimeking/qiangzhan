
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class UIStageList : UIWindow
{
#region 界面控件
	public GameObject mHardBtn;
	public GameObject mNormalBtn;
	public GameObject mSeriousBtn;
	public GameObject mEnterBtn;

    public GameObject mZoneIcon;

    public UILabel mMyFPText;

	public UILabel mBestFPText;

	public UILabel mCostSPText;
 
	public UILabel mExpText;

	private GameObject mFirstPassItem;

	private GameObject mNormalPassItem;

	public GameObject mHasAwardedIcon;

	public GameObject mLevelBar;
	public UIPanel mStagePanel;

	public GameObject mNormalBtnLabel;

	public GameObject mNormalDisable;
	public GameObject mHardDisable;
	public GameObject mSeriousDisable;

	public GameObject mNormalBtnHover;
	public GameObject mHardBtnHover;
	public GameObject mSeriousBtnHover;

    public UIGrid mGrid;

    public GameObject stagelistItemPrefab = null;

#endregion

	private StageListModule mModule = null;
	private PlayerDataModule mPlayerModule = null;

	private bool mDirty = false;
    private bool Dirty
    {
        set
        {
            if (value)
            {
                OnStageChanged();
	            mDirty = false;
            }
        }
    }

	private List<GameObject> mHoverList = null;

	static private Vector4 region0 = new Vector4(0.0f, 0.0f, 900.0f, 292.0f);
	static private Vector4 region1 = new Vector4(0.0f, -49.0f, 900.0f, 390.0f);

    protected override void OnLoad()
    {
        base.OnLoad();

        mHardBtn = this.FindChild("mBottom/mLevelBar/mHardBtn");
        mNormalBtn = this.FindChild("mBottom/mLevelBar/mNormalBtn");
        mSeriousBtn = this.FindChild("mBottom/mLevelBar/mSeriousBtn");
        mEnterBtn = this.FindChild("mEnterBtn");

        mZoneIcon = this.FindChild("mZoneIcon");

        mMyFPText = this.FindComponent<UILabel>("mBottom/mMyFPText");

        mBestFPText = this.FindComponent<UILabel>("mBottom/mBestFPText");
        mCostSPText = this.FindComponent<UILabel>("mBottom/mCostSPText");

        mExpText = this.FindComponent<UILabel>("mExpText");

        mHasAwardedIcon = this.FindChild("mBottom/mHasAwardedIcon");

		mFirstPassItem = this.FindChild("mBottom/mFirstPassItem");
		mNormalPassItem = this.FindChild("mBottom/mNormalPassItem");

        mLevelBar = this.FindChild("mBottom/mLevelBar");

        mNormalBtnLabel = this.FindChild("mBottom/mLevelBar/mNormalBtn/mNormalBtnLabel");

        mNormalDisable = this.FindChild("mBottom/mLevelBar/mNormalDisable");
        mHardDisable = this.FindChild("mBottom/mLevelBar/mHardDisable");
        mSeriousDisable = this.FindChild("mBottom/mLevelBar/mSeriousDisable");

        mNormalBtnHover = this.FindChild("mBottom/mLevelBar/mNormalBtn/mNormalBtnHover");
        mHardBtnHover = this.FindChild("mBottom/mLevelBar/mHardBtn/mHardBtnHover");
        mSeriousBtnHover = this.FindChild("mBottom/mLevelBar/mSeriousBtn/mSeriousBtnHover");

        mStagePanel = this.FindComponent<UIPanel>("mStagePanel");
        mGrid = this.FindComponent<UIGrid>("mStagePanel/mStagePanelGrid");

        stagelistItemPrefab = this.FindChild("items/StageListItemUI");
    }

    protected override void OnOpen(object param = null)
    {
        base.OnOpen();

        UIEventListener.Get(mHardBtn).onClick = OnHardBtnClicked;
        UIEventListener.Get(mNormalBtn).onClick = OnNormalBtnClicked;
        UIEventListener.Get(mSeriousBtn).onClick = OnSeriousBtnClicked;
        UIEventListener.Get(mNormalBtnLabel).onClick = OnNormalBtnClicked;
        UIEventListener.Get(mEnterBtn).onClick = OnEnterBtnClicked;

        EventSystem.Instance.addEventListener(StageUnlockEvent.STAGE_UNLOCK, OnSceneUnlock);

        if (mGrid == null)
        {
            mGrid = GameObject.Find("mStagePanelGrid").GetComponent<UIGrid>();
        }

        if (mModule == null)
        {
            mModule = ModuleManager.Instance.FindModule<StageListModule>();
        }

		if (mPlayerModule == null)
		{
			mPlayerModule = ModuleManager.Instance.FindModule<PlayerDataModule>();
		}

        if (mModule.ShowLevelBar)
        {
            mLevelBar.SetActive(true);
            mStagePanel.baseClipRegion = region0;
        }
        else
        {
            mLevelBar.SetActive(false);
            mStagePanel.baseClipRegion = region1;
        }


        if (mHoverList == null)
        {
            mHoverList = new List<GameObject>();
            mHoverList.Add(mNormalBtnHover);
            mHoverList.Add(mHardBtnHover);
            mHoverList.Add(mSeriousBtnHover);
        }

        RefreshUI();
        PlayerController.Instance.BreakQuestMove();
    }

    protected override void OnClose()
    {
        base.OnClose();

        ClearItemList();
    }

// 	public void OnCloseBtnClicked(GameObject target)
//     {
//         WindowManager.Instance.CloseUI("stagelist");
//     }

	private void OnSceneUnlock(EventBase evt)
	{
		Dirty = true;
	}

	private void OnNormalBtnClicked(GameObject target)
	{
		mModule.SelectedLevel = 0;

		Dirty = true;
	}

	private void OnHardBtnClicked(GameObject target)
	{
		mModule.SelectedLevel = 1;

		Dirty = true;
	}

	private void OnSeriousBtnClicked(GameObject target)
	{
		mModule.SelectedLevel = 2;

		Dirty = true;
	}

	private void OnEnterBtnClicked(GameObject target)
	{
		if(mModule.SelectedStageRes == null)
		{
			return;
		}

        //GameDebug.Log(mModule.SelectedStageRes.mId);
		SceneManager.Instance.RequestEnterScene(mModule.SelectedStageRes.resID);
	}

	// 点击关卡项目
	private void OnStageItemClicked(GameObject target)
	{
		StageListItemUI item = null;
        List<StageListItemUI> items = mModule.GetStageUIList();
        foreach(StageListItemUI sli in items)
        {
            if (sli == null) continue;
            if (GameObject.ReferenceEquals(target , sli.gameObject))
            {
                item = sli;
                break;
            }
        }
		if (item == null)
		{
			return;
		}

		OnStageUIClicked(item);

		UpdateSelectedItem();
	}
	
	// 点击关卡项目
	private void OnStageUIClicked(StageListItemUI ui)
	{
		if(ui == null)
		{
			return;
		}

		foreach (StageListItemUI uiitem in mModule.GetStageUIList())
		{
			uiitem.SetSelected(false);
		}

		ui.SetSelected(true);

		mModule.SelectedUI = ui;
	}

	// 刷新
	private void RefreshUI()
	{
		UpdateZoneIcon();
		UpdateItemList();
		UpdateSelectedItem();
	}

    void reposition()
    {
        mGrid.repositionNow = true;
    }

	// 更新选中的关卡
	private void UpdateSelectedItem()
	{
		if(mModule.SelectedUI == null)
		{
			StageListItemUI ui = null;
			int selectedLevel = 0;
			if (mModule.SelectedStageRes != null)
			{
				List<StageListItemUI> list = mModule.GetStageUIList();
				if(list == null)
				{
					return;
				}

				foreach(StageListItemUI item in list)
				{
					Scene_StageListTableItem listres = item.GetData();
					if(!item.GetLocked())
					{
						if (listres.mNromalStageId == mModule.SelectedStageRes.resID && mPlayerModule.IsStageUnlock(listres.mNromalStageId))
						{
							ui = item;
							selectedLevel = 0;
							break;
						}
						else if (listres.mHardStageId == mModule.SelectedStageRes.resID && mPlayerModule.IsStageUnlock(listres.mHardStageId))
						{
							ui = item;
							selectedLevel = 1;
							break;
						}
						else if (listres.mSeriousStageId == mModule.SelectedStageRes.resID && mPlayerModule.IsStageUnlock(listres.mSeriousStageId))
						{
							ui = item;
							selectedLevel = 2;
							break;
						}
					}
				}
			}

			if (ui == null)
			{
				ui = mModule.GetHeadStageUI();

				mStagePanel.transform.localPosition = new Vector3(mStagePanel.transform.localPosition.x, 92.0f, mStagePanel.transform.localPosition.z);
				mStagePanel.clipOffset = new Vector2(mStagePanel.clipOffset.x, 19.0f);
			}
			else
			{
				int curline = ui.Idx / 3;

				mStagePanel.transform.localPosition = new Vector3(mStagePanel.transform.localPosition.x, 92.0f + (curline * 182.0f), mStagePanel.transform.localPosition.z);
				mStagePanel.clipOffset = new Vector2(mStagePanel.clipOffset.x, 19.0f - (curline * 182.0f));
			}

			OnStageUIClicked(ui);
			mModule.SelectedLevel = selectedLevel;
		}

		Dirty = true;
	}

	// 显示关卡信息
	private void OnStageChanged()
	{
		PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
		if (module == null)
		{
			return;
		}

		mMyFPText.text = module.GetGrade().ToString();

		mBestFPText.text = mModule.GetSuitableFC().ToString();
		mCostSPText.text = mModule.GetCostSP().ToString();
		mExpText.text = mModule.GetAwardExp().ToString();

        if (mModule.ShowLevelBar)
        {
            bool unlock = false;
            if (!mModule.HasNormalLevel(mModule.SelectedStageListRes))
            {
                mNormalBtn.SetActive(false);
                mNormalDisable.SetActive(false);
                //SetEnergyBarShowType(EnergyBarUI.EnergyBarShowType.ShowSpOnly);
                SetMoneyBarShowType(MoneyBarType.TiLi);
            }
            else
            {
                unlock = mPlayerModule.IsStageUnlock(mModule.SelectedStageListRes.mNromalStageId);
                mNormalBtn.SetActive(unlock);
                mNormalDisable.SetActive(!unlock);
                //SetEnergyBarShowType(EnergyBarUI.EnergyBarShowType.All);
                BetterList<MoneyBarType> list = new BetterList<MoneyBarType>();
                list.Add(MoneyBarType.TiLi);
                list.Add(MoneyBarType.XingXing);
                SetMoneyBarShowType(list);
            }

            if (!mModule.HasHardLevel(mModule.SelectedStageListRes))
            {
                mHardBtn.SetActive(false);
                mHardDisable.SetActive(false);
            }
            else
            {
                unlock = mPlayerModule.IsStageUnlock(mModule.SelectedStageListRes.mHardStageId);
                mHardBtn.SetActive(unlock);
                mHardDisable.SetActive(!unlock);
            }

            if (!mModule.HasSeriousLevel(mModule.SelectedStageListRes))
            {
                mSeriousBtn.SetActive(false);
                mSeriousDisable.SetActive(false);
            }
            else
            {
                unlock = mPlayerModule.IsStageUnlock(mModule.SelectedStageListRes.mSeriousStageId);
                mSeriousBtn.SetActive(unlock);
                mSeriousDisable.SetActive(!unlock);
            }

            for (int i = 0; i < mHoverList.Count; ++i)
            {
                mHoverList[i].SetActive(mModule.SelectedLevel == i);
            }
        }
        else
        {
            //SetEnergyBarShowType(EnergyBarUI.EnergyBarShowType.ShowSpOnly);
            SetMoneyBarShowType(MoneyBarType.TiLi);
        }

		if(mModule.SelectedUI != null)
		{
			mModule.SelectedUI.SetStageRes(mModule.SelectedStageRes);

			mEnterBtn.SetActive(!mModule.SelectedUI.GetLocked());
		}

		ObjectCommon.DestoryChildren(mFirstPassItem);
		ObjectCommon.DestoryChildren(mNormalPassItem);

		if (mModule.SelectedStageRes != null)
		{
			mHasAwardedIcon.SetActive(module.IsStageHasPassed(mModule.SelectedStageRes.resID));

			if (mModule.SelectedStageRes.mFirstAwardId >= 0)
			{
				AwardItemUI awardItemUI = new AwardItemUI(mModule.SelectedStageRes.mFirstAwardId, 1);
				awardItemUI.gameObject.transform.parent = mFirstPassItem.transform;
				awardItemUI.gameObject.transform.localPosition = Vector3.zero;
				awardItemUI.gameObject.transform.localScale = Vector3.one;
			}

			if (mModule.SelectedStageRes.mPassAwardId0 >= 0)
			{
				AwardItemUI awardItemUI = new AwardItemUI(mModule.SelectedStageRes.mPassAwardId0, 1);
				awardItemUI.gameObject.transform.parent = mNormalPassItem.transform;
				awardItemUI.gameObject.transform.localPosition = Vector3.zero;
				awardItemUI.gameObject.transform.localScale = Vector3.one;
			}
		}
	}

	// 清除子项
	private void ClearItemList()
	{
		List<StageListItemUI> list = mModule.GetStageUIList();
		if(list == null)
		{
			return;
		}

		foreach (StageListItemUI ui in list)
		{
            ui.RemoveListener();
            GameObject.Destroy(ui.gameObject);
		}

		mModule.ClearStageListItem();
	}

	// 更新关卡列表
	private void UpdateItemList()
	{
		SortedList<int, Scene_StageListTableItem> sortedlist = new SortedList<int, Scene_StageListTableItem>();

        IDictionaryEnumerator itr = DataManager.Scene_StageListTable.GetEnumerator();
        while (itr.MoveNext())
        {
            Scene_StageListTableItem list = itr.Value as Scene_StageListTableItem;
            sortedlist.Add(list.mId, list);
        }

// 		foreach(Scene_StageListTableItem list in DataManager.Scene_StageListTable.Values)
// 		{
// 			sortedlist.Add(list.mId, list);
// 		}

		PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
		if (module == null)
			return;

		int itemidx = 0;
		foreach(Scene_StageListTableItem list in sortedlist.Values)
		{
			if(list.mZoneId != mModule.ZoneId)
			{
				continue;
			}

			GameObject obj = WindowManager.Instance.CloneGameObject(stagelistItemPrefab);
			if (obj == null)
			{
				continue;
			}
            obj.SetActive(true);
			obj.name = "StageListItemUI" + list.mId.ToString();
			obj.transform.parent = mGrid.transform;
			obj.transform.localScale = Vector3.one;

			StageListItemUI itemui = new StageListItemUI(obj);
			if (itemui == null)
			{
				continue;
			}

			itemui.Idx = itemidx++;
            itemui.AddListener();

			mModule.AddStageListItem(itemui);

			itemui.SetData(list);

			UIEventListener.Get(obj).onClick = OnStageItemClicked;

			StageData data = module.GetStageData(list.mNromalStageId);
			itemui.SetEnterTimes((int)module.GetStageDailyTimes(list.mNromalStageId));
            //if (mModule.GetStageListCount() > 1 && data == null)
            //{
            //    itemui.SetLocked(true);
            //    itemui.UpdateUI();
            //    break;
            //}

            ///活动本儿只显示一个可进入的关卡;
            if (mModule.IsActiveStage())
            {
                if (data == null)
                {
                    itemui.SetLocked(true);
                    itemui.UpdateUI();
                    break;
                }

                if(mModule.GetStageListCount() > 1)
                    mModule.RemoveStageListFirstItem();

                itemui.SetLocked(false);
                itemui.UpdateUI();
            }
            else
            {
                if (mModule.GetStageListCount() > 1 && data == null)
                {
                    itemui.SetLocked(true);
                    itemui.UpdateUI();
                    break;
                }

                itemui.SetLocked(false);
                itemui.UpdateUI();
            }
		}

        mGrid.repositionNow = true;
	}

	// 更新战区图标
	private void UpdateZoneIcon()
	{
		UISprite sprite = mZoneIcon.GetComponent<UISprite>();
		if(sprite == null)
		{
			return;
		}

        //UIAtlasHelper.SetSpriteImage(sprite, "stage_zone_" + mModule.ZoneId.ToString());
        UIAtlasHelper.SetSpriteName(sprite, "stage_zone_" + mModule.ZoneId.ToString());
	}

    public GameObject FindEnterBtn(int id)
    {
        return mEnterBtn;
    }
    public int GetCurrentID()
    {
        if (mModule.SelectedStageRes == null)
        {
            return -1;
        }
        return mModule.SelectedStageRes.resID;
    }
}
