using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Message;

public class UIQualifying : UIWindow
{
	// 返回按钮
	public UIButton mReturnBtn;

	// 规则按钮
	public UIButton mInfoBtn;
	// 记录按钮
	public UIButton mRecordBtn;
	// 竞技榜按钮
	public UIButton mRanklistBtn;

	// CD底板
	public GameObject mCoolDownBk;
	// 购买次数按钮
	public UIButton mBuyTimesBtn;
	// 剩余次数
	public UILabel mLeftTimesText;
	// 冷却时间
	public UILabel mCoolDownText;

	// 玩家信息
	public UILabel mPlayerInfoText;

	public UIGrid mGrid;

	public GameObject mCloneSrcPrefab = null;

	private QualifyingModule mModule = ModuleManager.Instance.FindModule<QualifyingModule>();

	private PlayerDataModule mPlayerDataModule = ModuleManager.Instance.FindModule<PlayerDataModule>();

	private UIMessageBoxParam mMsgBoxUIParam = null;

    private float mTimer = 0.0f;

	private List<UIQualifyingRole> mRoleList = new List<UIQualifyingRole>();

    protected override void OnLoad()
    {
		mReturnBtn = this.FindComponent<UIButton>("mTopWidget/mReturnBtn");

		mInfoBtn = this.FindComponent<UIButton>("mBottomWidget/mInfoBtn");
		mRecordBtn = this.FindComponent<UIButton>("mBottomWidget/mRecordBtn");
		mRanklistBtn = this.FindComponent<UIButton>("mBottomWidget/mRankListBtn");
		mCoolDownBk = this.FindChild("mBottomWidget/mCoolDownBk");
		mBuyTimesBtn = this.FindComponent<UIButton>("mBottomWidget/mLeftTimesBtn");
		mLeftTimesText = this.FindComponent<UILabel>("mBottomWidget/mLeftTimesBtn/mLeftTimesText");
		mCoolDownText = this.FindComponent<UILabel>("mBottomWidget/mCoolDownText");

		mPlayerInfoText = this.FindComponent<UILabel>("mPlayerInfoPanel/mPlayerInfoText");

		mGrid = this.FindComponent<UIGrid>("mRolePanel/mRoleGrid");
		mCloneSrcPrefab = this.FindChild("mCloneSrc/QualifyingRoleUI");

		InitMsgBoxUIParam();

		InitRoleList();
    }

    //界面打开
    protected override void OnOpen(object param = null)
    {
		EventDelegate.Add(mReturnBtn.onClick, OnReturnBtnClicked);
		EventDelegate.Add(mInfoBtn.onClick, OnInfoBtnClicked);
		EventDelegate.Add(mRecordBtn.onClick, OnRecordBtnClicked);
		EventDelegate.Add(mRanklistBtn.onClick, OnRanklistBtnClicked);
		EventDelegate.Add(mBuyTimesBtn.onClick, OnBuyTimeBtnClicked);

		EventSystem.Instance.addEventListener(QualifyingEvent.RECEIVE_MAIN_DATA, OnMainQualifyingDataUpdate);
		EventSystem.Instance.addEventListener(PropertyEvent.MAIN_PROPERTY_CHANGE, OnMainPropertyUpdate);
		EventSystem.Instance.addEventListener(QualifyingEvent.UI_QUALIFYING_BEGIN_FAILED_CD, OnBeginFailedCD);
		EventSystem.Instance.addEventListener(QualifyingEvent.UI_QUALIFYING_BEGIN_FAILED_NOTIMES, OnBeginFailedNoTimes);

		foreach (UIQualifyingRole role in mRoleList)
		{
			role.AddListener();
			role.ClearUI();
		}

		InitUI();

        mModule.RequestQualifyingData();
    }

    //界面关闭
    protected override void OnClose()
    {
		EventDelegate.Remove(mReturnBtn.onClick, OnReturnBtnClicked);
		EventDelegate.Remove(mInfoBtn.onClick, OnInfoBtnClicked);
		EventDelegate.Remove(mRecordBtn.onClick, OnRecordBtnClicked);
		EventDelegate.Remove(mRanklistBtn.onClick, OnRanklistBtnClicked);
		EventDelegate.Remove(mBuyTimesBtn.onClick, OnBuyTimeBtnClicked);

		EventSystem.Instance.removeEventListener(QualifyingEvent.RECEIVE_MAIN_DATA, OnMainQualifyingDataUpdate);
		EventSystem.Instance.removeEventListener(PropertyEvent.MAIN_PROPERTY_CHANGE, OnMainPropertyUpdate);
		EventSystem.Instance.removeEventListener(QualifyingEvent.UI_QUALIFYING_BEGIN_FAILED_CD, OnBeginFailedCD);
		EventSystem.Instance.removeEventListener(QualifyingEvent.UI_QUALIFYING_BEGIN_FAILED_NOTIMES, OnBeginFailedNoTimes);

		foreach (UIQualifyingRole role in mRoleList)
		{
			role.RemoveListener();
		}
    }

	private void OnReturnBtnClicked()
    {
		WindowManager.Instance.CloseUI("qualifying");
    }

	private void OnInfoBtnClicked()
	{
		WindowManager.Instance.OpenUI("qualifyinginfo");
	}
	private void OnRecordBtnClicked()
	{
		WindowManager.Instance.OpenUI("record", UIRecord.Style_Qualifying);
		mModule.RequestRecord();
	}
	private void OnRanklistBtnClicked()
	{
		WindowManager.Instance.CloseUI("qualifying");
		WindowManager.Instance.OpenUI("ranking", UIRanking.RankSelect.Rank_Rank, null, "qualifying");
	}
	private void OnBuyTimeBtnClicked()
	{
		uint lv = mPlayerDataModule.GetVipLevel();
		if (!DataManager.VipTable.ContainsKey(lv))
			return;

		VipTableItem res = DataManager.VipTable[lv] as VipTableItem;
		if (res == null)
			return;

		uint cost = GameConfig.QualifyingBuyTimesCost;

		mMsgBoxUIParam.mMsgText = string.Format(StringHelper.GetString("pvp_buytimes"), cost);

		WindowManager.Instance.OpenUI("msgbox", mMsgBoxUIParam);
	}

	private void OnMainQualifyingDataUpdate(EventBase e)
	{
		UpdateMainQualifyingDataUI();
	}

	private void OnMainPropertyUpdate(EventBase e)
	{
		UpdateMainPlayerInfoText();
	}

	private void OnBeginFailedCD(EventBase e)
	{
		if (mCoolDownBk.GetComponentInChildren<UISpriteAnimation>() != null)
			return;

		AnimationManager.Instance.AddSpriteAnimation("tiaozhanlengque", mCoolDownBk, 30, 9, false, true);
	}

	private void OnBeginFailedNoTimes(EventBase e)
	{
		OnBuyTimeBtnClicked();
	}

	private void InitUI()
	{
		UpdateMainQualifyingDataUI();
	}

	private void InitMsgBoxUIParam()
	{
		mMsgBoxUIParam = new UIMessageBoxParam();
		mMsgBoxUIParam.mOkBtnCallback = OnMsgBoxOkCallback;
		mMsgBoxUIParam.mCancelBtnCallback = OnMsgBoxCancelCallback;
	}

	private void OnMsgBoxOkCallback()
	{
		uint lv = mPlayerDataModule.GetVipLevel();
		if (!DataManager.VipTable.ContainsKey(lv))
			return;

		VipTableItem res = DataManager.VipTable[lv] as VipTableItem;
		if (res == null)
			return;

		uint cost = GameConfig.QualifyingBuyTimesCost;

		if (mPlayerDataModule.GetProceeds(ProceedsType.Money_RMB) < cost)
		{
			PopTipManager.Instance.AddNewTip(StringHelper.GetErrorString(ERROR_CODE.ERR_QUALIFYING_BUYTIMES_FAILED_NOCOST, FontColor.Red));
			//PromptUIManager.Instance.AddNewPrompt(StringHelper.GetErrorString(ERROR_CODE.ERR_QUALIFYING_BUYTIMES_FAILED_NOCOST));
			return;
		}

		if (mPlayerDataModule.GetQualifyingBuyTimes() >= res.mQualifyingBuyCount)
		{
			PopTipManager.Instance.AddNewTip(StringHelper.GetErrorString(ERROR_CODE.ERR_QUALIFYING_BUYTIMES_FAILED_NOTIMES, FontColor.Red));
			//PromptUIManager.Instance.AddNewPrompt(StringHelper.GetErrorString(ERROR_CODE.ERR_QUALIFYING_BUYTIMES_FAILED_NOTIMES));
			return;
		}

		mModule.RequestBuyTimes();
	}

	private void OnMsgBoxCancelCallback()
	{

	}

	private void UpdateMainPlayerInfoText()
	{
		string rank = (mPlayerDataModule.GetQualifyingCurRank() == uint.MaxValue) ? StringHelper.GetString("arena_outofrank") : (mPlayerDataModule.GetQualifyingCurRank() + 1).ToString();

		mPlayerInfoText.text = string.Format(StringHelper.GetString("qualifying_player_info"), mPlayerDataModule.GetLevel(),
			mPlayerDataModule.GetName(), mPlayerDataModule.GetGrade(), rank);
	}

	private void UpdateMainQualifyingDataUI()
	{
		if (mModule == null)
			return;

		mTimer = mModule.GetTimer(mPlayerDataModule.GetQualifyingLastTime());

		UpdateTimer();

		UpdateMainPlayerInfoText();

		mLeftTimesText.text = mPlayerDataModule.GetQualifyingLeftTimes().ToString() + StringHelper.GetString("times");
	}

	private void UpdateTimer()
	{
		if(mTimer < float.Epsilon)
		{
			mCoolDownText.text = StringHelper.GetString("pvp_cooldown");
		}
		else
		{
			uint min = (uint)(mTimer / 60);
			uint sec = (((uint)(mTimer * 1000)) % 60000) / 1000;

			mCoolDownText.text = string.Format(StringHelper.GetString("pvp_timer"), min, sec);
		}
	}

    public override void Update(uint elapsed)
    {
		if (mModule == null)
			return;

		if (mTimer < float.Epsilon)
			return;

		mTimer -= (float)(elapsed * 0.001);

		UpdateTimer();

    }

	private void InitRoleList()
	{
		if (mRoleList == null)
			return;

		mRoleList.Clear();

		for (int i = 0; i < QualifyingModule.MAX_FIGHTER_COUNT; ++i)
		{
			GameObject obj = WindowManager.Instance.CloneGameObject(mCloneSrcPrefab);
			if (obj == null)
			{
				continue;
			}

			obj.SetActive(true);
			obj.name = "QualifyingRoleUI" + i.ToString();
			obj.transform.parent = mGrid.transform;
			obj.transform.localScale = Vector3.one;

			UIQualifyingRole itemui = new UIQualifyingRole(obj);
			itemui.Idx = i;

			mRoleList.Add(itemui);
		}

		mGrid.repositionNow = true;
	}
}
