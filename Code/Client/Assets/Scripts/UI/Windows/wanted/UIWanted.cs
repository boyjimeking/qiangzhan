
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class UIWanted : UIWindow
{
#region 界面控件
	// 进入按钮
	public UIButton mEnterBtn;
	// 返回按钮
	public UIButton mReturnBtn;

	// 我的战斗力
	public UILabel mMyFPText;
	// 推荐战斗力
	public UILabel mBestFPText;
	// 消耗行动力
	public UILabel mCostSPText;
	// 通关经验
	public UILabel mExpText;

	// 首次通关奖励
	private GameObject mFirstPassItem;
	// 固定通关奖励
	private GameObject mNormalPassItem;
	// 已获得奖励
	public GameObject mHasAwardedIcon;

	// 关卡面板
	public UIGrid mGrid;
	// 向左
	public UIButton mLeftBtn;
	// 向右
	public UIButton mRightBtn;
	// 滑动条
	public UIScrollBar mScrollBar;
	// 滑动视图
	public UIScrollView mScrollView;
#endregion

	public GameObject cloneItemPrefab = null;

	const int MAX_NUM_PER_PAGE = 4;
	const float DURATION_PAGE = 0.5f;

	public UIWantedItem mSelectedItemUI = null;
	public Scene_StageSceneTableItem mSelectedItemRes = null;

	private List<UIWantedItem> mItemList = new List<UIWantedItem>();

	private WantedModule mModule = ModuleManager.Instance.FindModule<WantedModule>();
	private PlayerDataModule mPlayerModule = ModuleManager.Instance.FindModule<PlayerDataModule>();

    protected override void OnLoad()
    {
		base.OnLoad();

		mEnterBtn = this.FindComponent<UIButton>("mEnterBtn");
		mReturnBtn = this.FindComponent<UIButton>("energybar_todo/mCloseBtn");

		mMyFPText = this.FindComponent<UILabel>("mBottom/mMyFPText");
		mBestFPText = this.FindComponent<UILabel>("mBottom/mBestFPText");
		mCostSPText = this.FindComponent<UILabel>("mBottom/mCostSPText");
		mExpText = this.FindComponent<UILabel>("mBottom/mExpText");

		mHasAwardedIcon = this.FindChild("mBottom/mHasAwardedIcon");
		mFirstPassItem = this.FindChild("mBottom/mFirstPassItem");
		mNormalPassItem = this.FindChild("mBottom/mNormalPassItem");

		mGrid = this.FindComponent<UIGrid>("mGradPanel/mScrollView/mGrid");
		mLeftBtn = this.FindComponent<UIButton>("mGradPanel/mLeftBtn");
		mRightBtn = this.FindComponent<UIButton>("mGradPanel/mRightBtn");
		mScrollBar = this.FindComponent<UIScrollBar>("mGradPanel/mScrollBar");
		mScrollView = this.FindComponent<UIScrollView>("mGradPanel/mScrollView");

		cloneItemPrefab = this.FindChild("items/WantedItemUI");

		mScrollBar.gameObject.SetActive(true);
		mScrollBar.foregroundWidget.gameObject.SetActive(false);
		mScrollBar.backgroundWidget.gameObject.SetActive(false);

		InitItemUIList();
    }

	protected override void OnPreOpen(object param = null)
	{
		
	}

    protected override void OnOpen(object param = null)
    {
		base.OnOpen();

		EventDelegate.Add(mEnterBtn.onClick, OnEnterBtnClicked);
		EventDelegate.Add(mReturnBtn.onClick, OnReturnBtnClicked);
		EventDelegate.Add(mLeftBtn.onClick, OnLeftBtnClicked);
		EventDelegate.Add(mRightBtn.onClick, OnRightBtnClicked);

		EventSystem.Instance.addEventListener(StageUnlockEvent.STAGE_UNLOCK, OnSceneUnlock);

		UpdateItemUI();

		SelectLastItem();
    }

    protected override void OnClose()
    {
		EventDelegate.Remove(mEnterBtn.onClick, OnEnterBtnClicked);
		EventDelegate.Remove(mReturnBtn.onClick, OnReturnBtnClicked);
		EventDelegate.Remove(mLeftBtn.onClick, OnLeftBtnClicked);
		EventDelegate.Remove(mRightBtn.onClick, OnRightBtnClicked);

		EventSystem.Instance.removeEventListener(StageUnlockEvent.STAGE_UNLOCK, OnSceneUnlock);

		base.OnClose();
    }

	private void UpdateItemUI()
	{
		foreach (UIWantedItem ui in mItemList)
		{
			ui.UpdateUI();
		}

		mScrollView.ResetPosition();
	}

	private void InitItemUIList()
	{
		if (mItemList == null)
			return;

		mItemList.Clear();

		SortedList<int, Scene_StageSceneTableItem> sortedlist = new SortedList<int, Scene_StageSceneTableItem>();

        IDictionaryEnumerator itr = DataManager.Scene_WantedSceneTable.GetEnumerator();
        while (itr.MoveNext())
        {
            Scene_StageSceneTableItem res = itr.Value as Scene_StageSceneTableItem;
            sortedlist.Add(res.resID, res);

        }

// 		foreach (Scene_StageSceneTableItem res in DataManager.Scene_WantedSceneTable.Values)
// 		{
// 			sortedlist.Add(res.resID, res);
// 		}

		foreach (Scene_StageSceneTableItem res in sortedlist.Values)
		{
			GameObject obj = WindowManager.Instance.CloneGameObject(cloneItemPrefab);
			if (obj == null)
			{
				continue;
			}

			obj.SetActive(true);
			obj.name = "UIWantedItem" + res.resID.ToString();
			obj.transform.parent = mGrid.transform;
			obj.transform.localScale = Vector3.one;

			UIWantedItem itemui = new UIWantedItem(obj);
			//itemui.Idx = i;
			itemui.Res = res;
			UIEventListener.Get(obj).onClick = OnWantedItemClicked;

			itemui.InitUI();

			mItemList.Add(itemui);
		}

		mGrid.repositionNow = true;
	}

	private void OnEnterBtnClicked()
	{
		if (mSelectedItemRes == null)
			return;

		SceneManager.Instance.RequestEnterScene(mSelectedItemRes.resID);
	}

	private void OnReturnBtnClicked()
	{
		WindowManager.Instance.CloseUI("wanted");
	}

	float PageVal
	{
		set
		{
			TweenScrollValue.Begin(mScrollBar.gameObject, DURATION_PAGE, value).PlayForward();
		}
		get
		{
			return mScrollBar.value;
		}
	}

	float pageDelta2BarVal(int perNum, int totalNum)
	{
		if (totalNum < perNum)
			return 0f;

		return Mathf.Min(1f, (float)perNum / (float)(totalNum - perNum));
	}

	private void OnLeftBtnClicked()
	{
		if (isFirstPage(PageVal))
			return;

		PageVal -= pageDelta2BarVal(MAX_NUM_PER_PAGE, VisibleItemCount());
	}

	private void OnRightBtnClicked()
	{
		if (isLastPage(PageVal))
			return;

		PageVal += pageDelta2BarVal(MAX_NUM_PER_PAGE, VisibleItemCount());
	}

	bool isFirstPage(float val)
	{
		return val <= Mathf.Epsilon;
	}

	bool isLastPage(float val)
	{
		return Mathf.Approximately(val, 1f);
	}

	private int VisibleItemCount()
	{
		if (mItemList == null)
			return 0;

		int count = 0;
		for(int i = 0; i < mItemList.Count; ++i)
		{
			if (!mItemList[i].IsLocked())
				count++;
		}

		return count;
	}

	private void OnSceneUnlock(EventBase evt)
	{
		
	}

	private void SelectLastItem()
	{
		if (mItemList == null)
			return;

		int idx = -1;
		for (int i = mItemList.Count - 1; i >= 0; --i)
		{
			if(!mItemList[i].IsLocked())
			{
				idx = i;
				break;
			}
		}

		if (idx < 0 && mItemList.Count > 0)
			idx = 0;

 		if (idx < MAX_NUM_PER_PAGE)
		{
			mScrollBar.value = 0.0f;
		}
		else
		{
			mScrollBar.value = 1.0f;
		}

		OnWantedItemClicked(mItemList[idx].gameObject);
	}

	private void OnWantedItemClicked(GameObject target)
	{
		UIWantedItem item = null;
		foreach (UIWantedItem ui in mItemList)
		{
			if (ObjectCommon.ReferenceEquals(ui.gameObject, target))
			{
				item = ui;
				ui.SetSelected(true);
			}
			else
			{
				ui.SetSelected(false);
			}
		}

		mSelectedItemUI = item;
		mSelectedItemRes = item.Res;

		UpdateSelectedItem();
	}

	private void UpdateSelectedItem()
	{
		ObjectCommon.DestoryChildren(mFirstPassItem);
		ObjectCommon.DestoryChildren(mNormalPassItem);

		if (mSelectedItemUI == null || mSelectedItemRes == null)
			return;

		mMyFPText.text = mPlayerModule.GetGrade().ToString();
		mBestFPText.text = mSelectedItemRes.mSuitableFC.ToString();
		mCostSPText.text = GetCostSp().ToString();
		mExpText.text = mSelectedItemRes.mAwardExp.ToString();

		mEnterBtn.gameObject.SetActive(!mSelectedItemUI.IsLocked());

		mHasAwardedIcon.gameObject.SetActive(mPlayerModule.IsStageHasPassed(mSelectedItemRes.resID));

		if (mSelectedItemRes.mFirstAwardId >= 0)
		{
			AwardItemUI awardItemUI = new AwardItemUI(mSelectedItemRes.mFirstAwardId, 1);
			awardItemUI.gameObject.transform.parent = mFirstPassItem.transform;
			awardItemUI.gameObject.transform.localPosition = Vector3.zero;
			awardItemUI.gameObject.transform.localScale = Vector3.one;
		}

		if (mSelectedItemRes.mPassAwardId0 >= 0)
		{
			AwardItemUI awardItemUI = new AwardItemUI(mSelectedItemRes.mPassAwardId0, 1);
			awardItemUI.gameObject.transform.parent = mNormalPassItem.transform;
			awardItemUI.gameObject.transform.localPosition = Vector3.zero;
			awardItemUI.gameObject.transform.localScale = Vector3.one;
		}
	}

	private int GetCostSp()
	{
		if (mSelectedItemRes == null)
			return 0;

		int costSp = 0;
		if (mSelectedItemRes.mEnterCostSP > 0)
		{
			costSp += mSelectedItemRes.mEnterCostSP;
		}

		if (mSelectedItemRes.mAwardCostSP > 0)
		{
			costSp += mSelectedItemRes.mAwardCostSP;
		}

		return costSp;
	}
}
