using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Message;

public class UIStageBalance : UIWindow
{
#region 界面控件
	private GameObject mOKBtn;
	private GameObject mTitleIcon;
	private UILabel mNormalTimesText;
	private UILabel mExtraTimesText;

	private UILabel mLeftTimeText;

	private GameObject mAwardPanel;
	private GameObject mExtraAwardPanel;

	private GameObject mNormalPanel;
	private GameObject mExtraPanel;

	private UIGrid mAwardPanelGrid;
	private UIGrid mExtraAwardPanelGrid;

	private StageBalanceEffectUI mNormalEffectUI;
	private StageBalanceEffectUI mExtraEffectUI;

	private GameObject mStageBalanceItemUI = null;
	private GameObject mStageExtraBalanceItemUI = null;

	private UILabel mNormalTimesLabel1;
	private UILabel mExtraTimesLabel1;
#endregion

	private int mLeftAwardTimes = 0;
	private int mLeftExtraAwardTimes = 0;

	// 4张普通卡牌
	private const int ITEM_NORMAL_COUNT = 4;

	// 4张钻石卡牌
	private const int ITEM_EXTRA_COUNT = 4;

	// 普通卡牌列表
	public List<StageBalanceItemUI> mItemNormalList = new List<StageBalanceItemUI>();

	// 钻石卡牌列表
	public List<StageBalanceItemUI> mItemExtraList = new List<StageBalanceItemUI>();

	// 表
	private Scene_StageSceneTableItem mRes = null;

	// 翻牌时间
	private float mTimer = 15.0f;

	private StageBalanceModule mModule = null;

	// 状态
	private enum UIState : int
	{
		// 原始状态
		STATE_ORIGINAL = 0,

		// 普通牌展开
		STATE_0,

		// 等待普通展开
		STATE_1,

		// 等待普通翻牌
		STATE_2,

		// 黄金牌展开
		STATE_3,

		// 等待黄金展开
		STATE_4,

		// 等待结束
		STATE_DESTROY,
	}

	// 当前状态
	private UIState mState = UIState.STATE_ORIGINAL;

	protected override void OnLoad()
	{
		mOKBtn = this.FindChild("mOkBtn");
		mTitleIcon = this.FindChild("mTitleIcon");
		mNormalTimesText = this.FindComponent<UILabel>("mNormalPanel/mNormalTimesText");
		mExtraTimesText = this.FindComponent<UILabel>("mExtraPanel/mExtraTimesText");
		mLeftTimeText = this.FindComponent<UILabel>("mLeftTimeText");

		mAwardPanel = this.FindChild("mAwardPanel");
		mExtraAwardPanel = this.FindChild("mExtraAwardPanel");

		mNormalPanel = this.FindChild("mNormalPanel");
		mExtraPanel = this.FindChild("mExtraPanel");

		mAwardPanelGrid = this.FindComponent<UIGrid>("mAwardPanel/mAwardPanelGrid");
		mExtraAwardPanelGrid = this.FindComponent<UIGrid>("mExtraAwardPanel/mExtraAwardPanelGrid");

		mNormalEffectUI = new StageBalanceEffectUI(this.FindChild("mNormalEffectPanel"));
		mNormalEffectUI.onFinish = onNormalFinish;

		mExtraEffectUI = new StageBalanceEffectUI(this.FindChild("mExtraEffectPanel"));
		mExtraEffectUI.onFinish = onExtraEffecFinish;

		mStageBalanceItemUI = this.FindChild("Items/StageBalanceItemUI");
		mStageExtraBalanceItemUI = this.FindChild("Items/StageExtraBalanceItemUI");

		mNormalTimesLabel1 = this.FindComponent<UILabel>("mNormalPanel/mNormalTimesLabel1");
		mExtraTimesLabel1 = this.FindComponent<UILabel>("mExtraPanel/mExtraTimesLabel1");

		UIEventListener.Get(mOKBtn).onClick = OnOKBtnClicked;

		for (int i = 0; i < ITEM_NORMAL_COUNT; ++i)
		{
			GameObject obj = WindowManager.Instance.CloneGameObject(mStageBalanceItemUI); //ResourceManager.Instance.LoadUI("UI/stage/StageBalanceItemUI");

			if (obj == null)
			{
				return;
			}

			obj.name = "NormalItem" + i.ToString();
			obj.transform.parent = mAwardPanelGrid.transform;
			obj.transform.localScale = Vector3.one;

			StageBalanceItemUI item = new StageBalanceItemUI(obj);
			if (item == null)
			{
				return;
			}
			item.IsNormal = true;
			item.Idx = i;
			mItemNormalList.Add(item);
		}

		mAwardPanelGrid.repositionNow = true;

		for (int i = 0; i < ITEM_EXTRA_COUNT; ++i)
		{
			GameObject obj = WindowManager.Instance.CloneGameObject(mStageExtraBalanceItemUI); //ResourceManager.Instance.LoadUI("UI/stage/StageExtraBalanceItemUI");

			if (obj == null)
			{
				return;
			}

			obj.name = "ExtraItem" + i.ToString();
			obj.transform.parent = mExtraAwardPanelGrid.transform;
			obj.transform.localScale = Vector3.one;

			StageBalanceItemUI item = new StageBalanceItemUI(obj);
			if (item == null)
			{
				return;
			}
			item.IsNormal = false;
			item.Idx = i;
			mItemExtraList.Add(item);
		}

		mExtraAwardPanelGrid.repositionNow = true;

	}
	//界面打开
	protected override void OnOpen(object param = null)
	{
		Init();

		EventSystem.Instance.addEventListener(StageBalanceUIEvent.STAGE_BALANCE_SELECT_CARD, OnSelectCard);
	}
	//界面关闭
	protected override void OnClose()
	{
		EventSystem.Instance.removeEventListener(StageBalanceUIEvent.STAGE_BALANCE_SELECT_CARD, OnSelectCard);

		Reset();
	}

	private void OnSelectCard(EventBase e)
	{
		StageBalanceUIEvent evt = e as StageBalanceUIEvent;
		if (evt == null)
			return;

		if(evt.mIsNormalCard)
		{
			OnAwardClicked(evt.mIdx);
		}
		else
		{
			OnExtraAwardClicked(evt.mIdx);
		}
	}

	public override void Update(uint elapsed)
	{
		if (mNormalEffectUI != null)
			mNormalEffectUI.Update(elapsed);
		if (mExtraEffectUI != null)
			mExtraEffectUI.Update(elapsed);


		switch (mState)
		{
			case UIState.STATE_ORIGINAL:
				{

				}
				break;
			case UIState.STATE_0:
				{
					mNormalEffectUI.Open();
					mState = UIState.STATE_1;
				}
				break;
			case UIState.STATE_1:
				{

				}
				break;
			case UIState.STATE_2:
				{
					if (mLeftAwardTimes > 0)
					{
						//float delta = Time.unscaledDeltaTime;
						mTimer -= (float)(elapsed * 0.001);//delta;

						if (mTimer < float.Epsilon)
						{
							AutoAward();
							mLeftTimeText.text = null;
						}
						else
						{
							mLeftTimeText.text = ((int)mTimer).ToString();
						}
					}
				}
				break;
			case UIState.STATE_3:
				{
					mExtraPanel.SetActive(true);
					mExtraEffectUI.Open();
					mState = UIState.STATE_4;
				}
				break;
			case UIState.STATE_4:
				{

				}
				break;
			case UIState.STATE_DESTROY:
				{

				}
				break;
		}
	}

	// 初始化数据
	private void Init()
	{
		BaseScene scene = SceneManager.Instance.GetCurScene();
		if (scene == null)
		{
			return;
		}

		if (!typeof(StageScene).IsAssignableFrom(scene.GetType()))
		{
			return;
		}

		StageScene stage = scene as StageScene;

		mRes = stage.GetStageRes();
		if (mRes == null)
		{
			return;
		}

		if (mModule == null)
		{
			mModule = ModuleManager.Instance.FindModule<StageBalanceModule>();
		}

		mOKBtn.SetActive(false);

		mNormalPanel.SetActive(false);
		mExtraPanel.SetActive(false);
		mAwardPanel.SetActive(false);
		mExtraAwardPanel.SetActive(false);
		mNormalEffectUI.Stop();
		mExtraEffectUI.Stop();

		// 不显示普通翻牌
		if (mRes.mRandomAwardTimes < 1 || mRes.mRandomAwardBoxId < 0)
		{
			mLeftAwardTimes = 0;
			mState = UIState.STATE_3;
		}
		else
		{
			mNormalPanel.SetActive(true);
			mLeftAwardTimes = mRes.mRandomAwardTimes;
			mState = UIState.STATE_0;
		}

		mNormalTimesLabel1.text = GetCardTypeText(mRes.mRandomAwardCostId);

		foreach (StageBalanceItemUI item in mItemNormalList)
		{
			item.SetCostId(mRes.mRandomAwardCostId);
			item.AddListener();
		}

		UpdateAwardTimes();
		//RandomAward();

		// 不显示钻石翻牌
		if (mRes.mExtraAwardTimes < 1 || mRes.mExtraAwardBoxId < 0)
		{
			mLeftExtraAwardTimes = 0;
			if (mState == UIState.STATE_3)
			{
				mState = UIState.STATE_DESTROY;
				mOKBtn.SetActive(true);
			}
		}
		else
		{
			mLeftExtraAwardTimes = mRes.mExtraAwardTimes;
		}

		mExtraTimesLabel1.text = GetCardTypeText(mRes.mExtraAwardCostId);

		foreach (StageBalanceItemUI item in mItemExtraList)
		{
			item.SetCostId(mRes.mExtraAwardCostId);
			item.AddListener();
		}

		UpdateExtraAwardTimes();
		//RandomExtraAward();
	}

	private string GetCardTypeText(int costId)
	{
		if (costId >= 0)
		{
			ConditionTableItem condRes = DataManager.ConditionTable[costId] as ConditionTableItem;
			if (condRes != null)
			{
				if (condRes.mType == ConditionType.MONEY)
				{
					if (condRes.mParam1 == (int)ProceedsType.Money_Game)
					{
						return StringHelper.GetString("money_game") + StringHelper.GetString("card");
					}
					else if (condRes.mParam1 == (int)ProceedsType.Money_RMB)
					{
						return StringHelper.GetString("money_rmb") + StringHelper.GetString("card");
					}
				}
			}
		}

		return StringHelper.GetString("free") + StringHelper.GetString("card");
	}

	// 界面还原
	private void Reset()
	{
		for (int i = 0; i < mItemNormalList.Count; ++i)
		{
			StageBalanceItemUI item = mItemNormalList[i];
			if (item != null)
			{
				item.RemoveListener();
				item.Reset();
			}
		}

		for (int i = 0; i < mItemExtraList.Count; ++i)
		{
			StageBalanceItemUI item = mItemExtraList[i];
			if (item != null)
			{
				item.RemoveListener();
				item.Reset();
			}
		}

		mRes = null;

		mTimer = 15.0f;
		mLeftTimeText.text = ((int)mTimer).ToString();

		mState = UIState.STATE_ORIGINAL;
	}

	// 更新普通翻牌剩余次数
	private void UpdateAwardTimes()
	{
		mNormalTimesText.text = mLeftAwardTimes.ToString();
		if (mLeftAwardTimes < 1)
		{
			mLeftTimeText.text = null;
		}
	}

	// 更新钻石翻牌剩余次数
	private void UpdateExtraAwardTimes()
	{
		mExtraTimesText.text = mLeftExtraAwardTimes.ToString();
	}

	public void OnOKBtnClicked(GameObject target)
	{
		PassStageActionParam param = new PassStageActionParam();
		param.stageid = mRes.resID;
		param.maxgrade = mModule.GetGrade();
		param.maxcombo = 0;
		param.killrate = 0;
		param.passtimerecord = mModule.GetPassTime();
		param.normalcount = (mRes.mRandomAwardTimes < 1 || mRes.mRandomAwardBoxId < 0) ? 0 : (uint)(mRes.mRandomAwardTimes - mLeftAwardTimes);
		param.extracount = (mRes.mExtraAwardTimes < 1 || mRes.mExtraAwardBoxId < 0) ? 0 : (uint)(mRes.mExtraAwardTimes - mLeftExtraAwardTimes);
		Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_SCENE_PASS, param);

		WindowManager.Instance.CloseUI("stagebalance");
	}

	public void OnAwardClicked(int index)
	{
		// 翻牌次数用尽
		if (mLeftAwardTimes < 1)
			return;

		if (mRes == null)
			return;

		if (index < 0 || index >= mItemNormalList.Count)
			return;

		StageBalanceItemUI item = mItemNormalList[index];
		if (item == null)
			return;

		// 翻过了
		if (item.IsAwarded())
			return;

		int idx = mRes.mRandomAwardTimes - mLeftAwardTimes;
		if (idx < 0 || mLeftAwardTimes < 0)
			return;

		if (mRes.mRandomAwardCostId >= 0)
		{
			if (DataManager.ConditionTable.ContainsKey(mRes.mRandomAwardCostId))
			{
				ConditionTableItem condItem = DataManager.ConditionTable[mRes.mRandomAwardCostId] as ConditionTableItem;
				if(condItem != null)
				{
					// 消耗不足
					if (!ConditionManager.Instance.CheckCondition(condItem.mType, condItem.mParam1, condItem.mParam2, condItem.mValue * (idx + 1)))
					{
						PopTipManager.Instance.AddNewTip(StringHelper.GetErrorString(ERROR_CODE.ERR_SCENE_PASS_FAILED_AWARD_NOCOST, FontColor.Red));
						//PromptUIManager.Instance.AddNewPrompt(StringHelper.GetErrorString(ERROR_CODE.ERR_SCENE_PASS_FAILED_AWARD_NOCOST));
						return;
					}
				}
			}
		}

		role_dropaward drop = StageDataManager.Instance.GetNormalRandomAward(idx);
		if (drop == null)
			return;

		if (drop.drop_id_type == 1)
		{
			item.Award(ConditionManager.Instance.GetConditionIcon(ConditionType.ITEM, drop.drop_id, -1), ConditionManager.Instance.GetConditionText(ConditionType.ITEM, drop.drop_id, -1, drop.drop_num));
		}
		else
		{
			item.Award(ConditionManager.Instance.GetConditionIcon(drop.drop_id), ConditionManager.Instance.GetConditionText(drop.drop_id));
		}

		mLeftAwardTimes--;

		UpdateAwardTimes();

		if (mLeftAwardTimes < 1)
		{
			mOKBtn.SetActive(true);

			if (mRes.mExtraAwardTimes < 1 || mRes.mExtraAwardBoxId < 0)
			{
				mState = UIState.STATE_DESTROY;
			}
			else
			{
				mState = UIState.STATE_3;
			}
		}
	}

	public void OnExtraAwardClicked(int index)
	{
		// 翻牌次数用尽
		if (mLeftExtraAwardTimes < 1)
			return;

		if (mRes == null)
			return;

		if (index < 0 || index >= mItemExtraList.Count)
			return;

		StageBalanceItemUI item = mItemExtraList[index];
		if (item == null)
			return;

		// 翻过了
		if (item.IsAwarded())
		{
			return;
		}

		int idx = mRes.mExtraAwardTimes - mLeftExtraAwardTimes;
		if (idx < 0 || mLeftExtraAwardTimes < 0)
		{
			return;
		}

		if (mRes.mExtraAwardCostId >= 0)
		{
			if (DataManager.ConditionTable.ContainsKey(mRes.mExtraAwardCostId))
			{
				ConditionTableItem condItem = DataManager.ConditionTable[mRes.mExtraAwardCostId] as ConditionTableItem;
				if (condItem != null)
				{
					// 消耗不足
					if (!ConditionManager.Instance.CheckCondition(condItem.mType, condItem.mParam1, condItem.mParam2, condItem.mValue * (idx + 1)))
					{
						PopTipManager.Instance.AddNewTip(StringHelper.GetErrorString(ERROR_CODE.ERR_SCENE_PASS_FAILED_AWARD_NOCOST, FontColor.Red));
						//PromptUIManager.Instance.AddNewPrompt(StringHelper.GetErrorString(ERROR_CODE.ERR_SCENE_PASS_FAILED_AWARD_NOCOST));
						return;
					}
				}
			}
		}

		role_dropaward drop = StageDataManager.Instance.GetExtraRandomAward(idx);
		if (drop == null)
		{
			return;
		}

		if (drop.drop_id_type == 1)
		{
			item.Award(ConditionManager.Instance.GetConditionIcon(ConditionType.ITEM, drop.drop_id, -1), ConditionManager.Instance.GetConditionText(ConditionType.ITEM, drop.drop_id, -1, drop.drop_num));
		}
		else
		{
			item.Award(ConditionManager.Instance.GetConditionIcon(drop.drop_id), ConditionManager.Instance.GetConditionText(drop.drop_id));
		}

		//mExtraAwardsList.RemoveAt(0);

		mLeftExtraAwardTimes--;

		UpdateExtraAwardTimes();
	}

	private void AutoAward()
	{
		foreach (StageBalanceItemUI item in mItemNormalList)
		{
			if (mLeftAwardTimes < 1)
			{
				break;
			}

			if (item.IsAwarded())
			{
				continue;
			}

			OnAwardClicked(item.Idx);
		}
	}


	public void onNormalFinish(StageBalanceEffectUI  effect)
	{
		if (mState == UIState.STATE_1)
		{
			mNormalEffectUI.Stop();
			mAwardPanel.SetActive(true);
			mState = UIState.STATE_2;
		}
	}
	public void onExtraEffecFinish(StageBalanceEffectUI effect)
	{
		if (mState == UIState.STATE_4)
		{
			mExtraEffectUI.Stop();
			mExtraAwardPanel.SetActive(true);
			mOKBtn.SetActive(true);
			mState = UIState.STATE_DESTROY;
		}
	}
}
