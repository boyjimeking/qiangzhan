using System.Collections.Generic;
using UnityEngine;


public class StageBalanceItemUI
{
#region 界面控件
	// 牌面
	private GameObject mAwardPanel;
	private UISprite mAwardIcon;
	private UILabel mAwardText;
	private TweenRotation mAwardTweener;
	private GameObject mAwardObject;

	// 牌背
	private GameObject mAwardMask;
	private GameObject mCostPanel;
	private UISprite mCostIcon;
	private UILabel mCostText;
	private TweenRotation mMaskTweener;
	private GameObject mMaskObject;

	public UIButton mBtn;

	private UISpriteAnimation mAni;
#endregion

	// 是否已翻牌
	private bool mAwarded = false;

    private GameObject mObj = null;

	private int mIdx = -1;

	private bool mIsNormal = true;

	private Vector3[] mMaskParamList = new Vector3[4];
	private Vector3[] mAwardParamList = new Vector3[4];

    public StageBalanceItemUI(GameObject obj)
    {
        mObj = obj;

		mBtn = mObj.GetComponent<UIButton>();
		mAwardObject = ObjectCommon.GetChild(mObj, "mAwardObject");
		mAwardTweener = ObjectCommon.GetChildComponent<TweenRotation>(mObj, "mAwardObject");
		mAwardPanel = ObjectCommon.GetChild(mObj, "mAwardObject/mAwardPanel");
		mAwardIcon = ObjectCommon.GetChildComponent<UISprite>(mObj, "mAwardObject/mAwardIcon");
		mAwardText = ObjectCommon.GetChildComponent<UILabel>(mObj, "mAwardObject/mAwardText");

		mMaskObject = ObjectCommon.GetChild(mObj, "mMaskObject");
		mMaskTweener = ObjectCommon.GetChildComponent<TweenRotation>(mObj, "mMaskObject");
		mAwardMask = ObjectCommon.GetChild(mObj, "mMaskObject/mAwardMask");
		mCostPanel = ObjectCommon.GetChild(mObj, "mMaskObject/mAwardCostPanel");
		mCostIcon = ObjectCommon.GetChildComponent<UISprite>(mObj, "mMaskObject/mAwardCostPanel/mAwardCostIcon");
		mCostText = ObjectCommon.GetChildComponent<UILabel>(mObj, "mMaskObject/mAwardCostPanel/mAwardCostText");

		mAni = ObjectCommon.GetChildComponent<UISpriteAnimation>(mObj, "mAni");

		mMaskParamList[0] = new Vector3(0.0f, 63.3f, 0.0f);
		mMaskParamList[1] = new Vector3(0.0f, 80.5f, 0.0f);
		mMaskParamList[2] = new Vector3(0.0f, -80.5f, 0.0f);
		mMaskParamList[3] = new Vector3(0.0f, -63.3f, 0.0f);

		mAwardParamList[0] = new Vector3(0.0f, -116.7f, 0.0f);
		mAwardParamList[1] = new Vector3(0.0f, -99.5f, 0.0f);
		mAwardParamList[2] = new Vector3(0.0f, 99.5f, 0.0f);
		mAwardParamList[3] = new Vector3(0.0f, 116.7f, 0.0f);

        Reset(); 
    }

	public void AddListener()
	{
		EventDelegate.Add(mBtn.onClick, OnClicked);
		mAwardTweener.AddOnFinished(OnTweenerFinished);
	}

	public void RemoveListener()
	{
		EventDelegate.Remove(mBtn.onClick, OnClicked);
		mAwardTweener.RemoveOnFinished(OnTweenerFinished);
	}

	void OnClicked()
    {
		EventSystem.Instance.PushEvent(new StageBalanceUIEvent(StageBalanceUIEvent.STAGE_BALANCE_SELECT_CARD, IsNormal, Idx));
    }

	// 是否已翻
	public bool IsAwarded()
	{
		return mAwarded;
	}

	// 结算界面关闭 重置初始状态
	public void Reset()
	{
		mAwarded = false;
		mAwardText.text = null;
		mAwardMask.SetActive(true);
		mCostPanel.SetActive(true);

		ApplyCardRotation();

		mAwardTweener.ResetToBeginning();
		mMaskTweener.ResetToBeginning();

		UIAtlasHelper.SetSpriteImage(mAwardIcon, null);

		mAni.Reset();
		mAni.gameObject.SetActive(false);
	}

	private void OnTweenerFinished()
	{
		mAwardMask.SetActive(false);
		mCostPanel.SetActive(false);
	}

	// 翻牌
	public void Award(string icon, string txt)
	{
		UIAtlasHelper.SetSpriteImage(mAwardIcon, icon);

		mAwardText.text = txt;

		mAwardTweener.Play();
		mMaskTweener.Play();

		mAni.gameObject.SetActive(true);
		mAni.onFinished += OnAniFinished;
	    SoundManager.Instance.Play(107);
		mAwarded = true;
	}

	private void OnAniFinished(GameObject go)
	{
		go.SetActive(false);
	}

	public void SetCostId(int costId)
	{
		if(costId < 0 || !DataManager.ConditionTable.ContainsKey(costId))
		{
			mCostPanel.SetActive(false);
			return;
		}

		string icon = ConditionManager.Instance.GetConditionIcon(costId);
		if(!string.IsNullOrEmpty(icon))
		{
			UIAtlasHelper.SetSpriteImage(mCostIcon, icon);
		}

		ConditionTableItem item = DataManager.ConditionTable[costId] as ConditionTableItem;
		mCostText.text = "X"+ item.mValue.ToString();

		mCostPanel.SetActive(true);
	}

	private void ApplyCardRotation()
	{
		if (mIdx >= 0 && mIdx <= 4)
		{
			Vector3 vMask = mMaskParamList[mIdx];
			mMaskTweener.from = Vector3.zero;
			mMaskTweener.to = new Vector3(vMask.x, vMask.y, vMask.z);
			mMaskObject.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

			Vector3 vAward = mAwardParamList[mIdx];
			mAwardTweener.from = new Vector3(vAward.x, vAward.y, vAward.z);
			mAwardTweener.to = Vector3.zero;
			mAwardObject.transform.localRotation = Quaternion.Euler(vAward.x, vAward.y, vAward.z);
		}
	}

	public int Idx
	{
		get
		{
			return mIdx;
		}

		set
		{
			mIdx = value;

			ApplyCardRotation();
		}
	}

	public bool IsNormal
	{
		get
		{
			return mIsNormal;
		}

		set
		{
			mIsNormal = value;
		}
	}
}
