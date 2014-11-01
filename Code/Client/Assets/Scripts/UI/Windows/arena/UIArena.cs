using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Message;

public class UIArena : UIWindow
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

	// 玩家等级
	public UILabel mLevelText;
	// 玩家名称
	public UILabel mNameText;
	// 竞技排名
	public UILabel mRankingText;
	// 当前段位
	public UILabel mRankLevelText;
	// 累计积分
	public UILabel mScoreText;
	// 当前战力
	public UILabel mBattleGradeText;
	// 竞技币
	public UILabel mMoneyText;

	// 调整技能按钮
	public UIButton mSkillBtn;
	// 竞技兑换按钮
	public UIButton mShopBtn;

	public UIGrid mGrid;

	public GameObject mCloneSrcPrefab = null;

	private ArenaModule mModule = ModuleManager.Instance.FindModule<ArenaModule>();

	private PlayerDataModule mPlayerDataModule = ModuleManager.Instance.FindModule<PlayerDataModule>();

	private UIMessageBoxParam mMsgBoxUIParam = null;

    private float mTimer = 0.0f;

    private List<UIArenaRole> mRoleList = new List<UIArenaRole>();

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

		mLevelText = this.FindComponent<UILabel>("mPlayerInfoPanel/mLevelText");
		mNameText = this.FindComponent<UILabel>("mPlayerInfoPanel/mNameText");
		mRankingText = this.FindComponent<UILabel>("mPlayerInfoPanel/mRankingText");
		mRankLevelText = this.FindComponent<UILabel>("mPlayerInfoPanel/mRankLevelText");
		mScoreText = this.FindComponent<UILabel>("mPlayerInfoPanel/mRankScoreText");
		mBattleGradeText = this.FindComponent<UILabel>("mPlayerInfoPanel/mBattleGradeText");
		mMoneyText = this.FindComponent<UILabel>("mPlayerInfoPanel/mMoneyText");
		mSkillBtn = this.FindComponent<UIButton>("mPlayerInfoPanel/mSkillBtn");
		mShopBtn = this.FindComponent<UIButton>("mPlayerInfoPanel/mShopBtn");

		mGrid = this.FindComponent<UIGrid>("mRolePanel/mRoleGrid");
		mCloneSrcPrefab = this.FindChild("mCloneSrc/ArenaRoleUI");

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
		EventDelegate.Add(mSkillBtn.onClick, OnSkillBtnClicked);
		EventDelegate.Add(mShopBtn.onClick, OnShopBtnClicked);

		EventSystem.Instance.addEventListener(ArenaEvent.RECEIVE_MAIN_DATA, OnMainArenaDataUpdate);
		EventSystem.Instance.addEventListener(ProceedsEvent.PROCEEDS_CHANGE_FIVE, OnArenaMoneyDataUpdate);
		EventSystem.Instance.addEventListener(PropertyEvent.MAIN_PROPERTY_CHANGE, OnMainPropertyUpdate);
		EventSystem.Instance.addEventListener(ArenaEvent.UI_ARENA_BEGIN_FAILED_CD, OnBeginFailedCD);
		EventSystem.Instance.addEventListener(ArenaEvent.UI_ARENA_BEGIN_FAILED_NOTIMES, OnBeginFailedNoTimes);

		foreach(UIArenaRole role in mRoleList)
		{
			role.AddListener();
			role.ClearUI();
		}

		InitUI();

        mModule.RequestArenaData();
    }

    //界面关闭
    protected override void OnClose()
    {
		EventDelegate.Remove(mReturnBtn.onClick, OnReturnBtnClicked);
		EventDelegate.Remove(mInfoBtn.onClick, OnInfoBtnClicked);
		EventDelegate.Remove(mRecordBtn.onClick, OnRecordBtnClicked);
		EventDelegate.Remove(mRanklistBtn.onClick, OnRanklistBtnClicked);
		EventDelegate.Remove(mBuyTimesBtn.onClick, OnBuyTimeBtnClicked);
		EventDelegate.Remove(mSkillBtn.onClick, OnSkillBtnClicked);
		EventDelegate.Remove(mShopBtn.onClick, OnShopBtnClicked);

		EventSystem.Instance.removeEventListener(ArenaEvent.RECEIVE_MAIN_DATA, OnMainArenaDataUpdate);
		EventSystem.Instance.removeEventListener(ProceedsEvent.PROCEEDS_CHANGE_FIVE, OnArenaMoneyDataUpdate);
		EventSystem.Instance.removeEventListener(PropertyEvent.MAIN_PROPERTY_CHANGE, OnMainPropertyUpdate);
		EventSystem.Instance.removeEventListener(ArenaEvent.UI_ARENA_BEGIN_FAILED_CD, OnBeginFailedCD);
		EventSystem.Instance.removeEventListener(ArenaEvent.UI_ARENA_BEGIN_FAILED_NOTIMES, OnBeginFailedNoTimes);

		foreach (UIArenaRole role in mRoleList)
		{
			role.RemoveListener();
		}
    }

	private void OnReturnBtnClicked()
    {
		WindowManager.Instance.CloseUI("arena");
    }

	private void OnInfoBtnClicked()
	{
		WindowManager.Instance.OpenUI("arenainfo");
	}
	private void OnRecordBtnClicked()
	{
		WindowManager.Instance.OpenUI("record", UIRecord.Style_Arena);
		mModule.RequestRecord();
	}
	private void OnRanklistBtnClicked()
	{
		WindowManager.Instance.CloseUI("arena");
		WindowManager.Instance.OpenUI("ranking", UIRanking.RankSelect.Rank_Arena, null, "arena");
	}
	private void OnBuyTimeBtnClicked()
	{
		uint lv = mPlayerDataModule.GetVipLevel();
		if (!DataManager.VipTable.ContainsKey(lv))
			return;

		VipTableItem res = DataManager.VipTable[lv] as VipTableItem;
		if (res == null)
			return;

		uint cost = GameConfig.ArenaBuyTimesCost;

		mMsgBoxUIParam.mMsgText = string.Format(StringHelper.GetString("pvp_buytimes"), cost);

		WindowManager.Instance.OpenUI("msgbox", mMsgBoxUIParam);
	}
	private void OnSkillBtnClicked()
	{
		WindowManager.Instance.CloseUI("arena");
		WindowManager.Instance.OpenUI("skill", null, null, "arena");
	}
	private void OnShopBtnClicked()
	{
		WindowManager.Instance.CloseUI("arena");
        WindowManager.Instance.OpenUI("shop", ShopSubTable.Credit, null, "arena");
	}

	private void OnMainArenaDataUpdate(EventBase e)
	{
		UpdateMainArenaDataUI();
	}

	private void OnArenaMoneyDataUpdate(EventBase e)
	{
		UpdateArenaMoneyUI();
	}

	private void OnMainPropertyUpdate(EventBase e)
	{
		UpdateMainPropertyUI();
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
		UpdateMainPropertyUI();

		UpdateMainArenaDataUI();
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

		uint cost = GameConfig.ArenaBuyTimesCost;

		if (mPlayerDataModule.GetProceeds(ProceedsType.Money_RMB) < cost)
		{
			PopTipManager.Instance.AddNewTip(StringHelper.GetErrorString(ERROR_CODE.ERR_ARENA_BUYTIMES_FAILED_NOCOST, FontColor.Red));
			//PromptUIManager.Instance.AddNewPrompt(StringHelper.GetErrorString(ERROR_CODE.ERR_ARENA_BUYTIMES_FAILED_NOCOST));
			return;
		}

		if (mPlayerDataModule.GetArenaBuyTimes() >= res.mArenaBuyCount)
		{
			PopTipManager.Instance.AddNewTip(StringHelper.GetErrorString(ERROR_CODE.ERR_ARENA_BUYTIMES_FAILED_NOTIMES, FontColor.Red));
			//PromptUIManager.Instance.AddNewPrompt(StringHelper.GetErrorString(ERROR_CODE.ERR_ARENA_BUYTIMES_FAILED_NOTIMES));
			return;
		}

		mModule.RequestBuyTimes();
	}

	private void OnMsgBoxCancelCallback()
	{

	}

	private void UpdateMainPropertyUI()
	{
		mLevelText.text = mPlayerDataModule.GetLevel().ToString();
		mBattleGradeText.text = mPlayerDataModule.GetGrade().ToString();
		mNameText.text = mPlayerDataModule.GetName();

		UpdateArenaMoneyUI();
	}

	private void UpdateArenaMoneyUI()
	{
		mMoneyText.text = mPlayerDataModule.GetProceeds(ProceedsType.Money_Arena).ToString();
	}

	private void UpdateMainArenaDataUI()
	{
		if (mModule == null)
			return;

		mTimer = mModule.GetTimer(mPlayerDataModule.GetArenaLastTime());

		UpdateTimer();

		if (mPlayerDataModule.GetArenaCurRank() == uint.MaxValue)
		{
			mRankingText.text = StringHelper.GetString("arena_outofrank");
		}
		else
		{
			mRankingText.text = (mPlayerDataModule.GetArenaCurRank() + 1).ToString();
		}

		mScoreText.text = mPlayerDataModule.GetArenaScore().ToString();

		mLeftTimesText.text = mPlayerDataModule.GetArenaLeftTimes().ToString() + StringHelper.GetString("times");

		mRankLevelText.text = mModule.GetRankLevelStringByRanking(mPlayerDataModule.GetArenaCurRank());
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

		UpdateRoleUI();

		if (mTimer < float.Epsilon)
			return;

		mTimer -= (float)(elapsed * 0.001);

		UpdateTimer();

    }

	private void UpdateRoleUI()
	{
		if (mRoleList == null)
			return;

		foreach(UIArenaRole ui in mRoleList)
		{
			if (ui != null)
				ui.Update();
		}
	}

	private void InitRoleList()
	{
		if (mRoleList == null)
			return;

		mRoleList.Clear();

		for (int i = 0; i < ArenaModule.MAX_FIGHTER_COUNT; ++i)
		{
			GameObject obj = WindowManager.Instance.CloneGameObject(mCloneSrcPrefab);
			if (obj == null)
			{
				continue;
			}

			obj.SetActive(true);
			obj.name = "ArenaRoleUI" + i.ToString();
			obj.transform.parent = mGrid.transform;
			obj.transform.localScale = Vector3.one;

			UIArenaRole itemui = new UIArenaRole(obj);
			itemui.Idx = i;

			mRoleList.Add(itemui);
		}

		mGrid.repositionNow = true;
	}
}
