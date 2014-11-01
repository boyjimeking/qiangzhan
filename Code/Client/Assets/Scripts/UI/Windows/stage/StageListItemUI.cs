
using UnityEngine;


public class StageListItemUI
{
	public GameObject mBackground;
	public GameObject mHover;
	public UISprite mStagePic;
	public GameObject mLockPic;
	public UISprite mBossIcon;
	public GameObject mStarSlot0;
	public GameObject mStarSlot1;
	public GameObject mStarSlot2;
	public GameObject mStarIcon0;
	public GameObject mStarIcon1;
	public GameObject mStarIcon2;

	public UILabel mStageNameText;
	public UILabel mLevelText;
    public UILabel mEnterTimeText;

	public UISprite mMask;

	private Scene_StageListTableItem mListItem = null;

	private int mIdx = 0;

    private int maxEnterTime;

    private GameObject mObj;

    public GameObject gameObject
    {
        get
        {
            return mObj;
        }
    }

    public StageListItemUI(GameObject obj)
    {
        mObj = obj;
        mBackground = ObjectCommon.GetChild(obj, "mBackground");
        mHover = ObjectCommon.GetChild(obj , "mHover");
        mStagePic = ObjectCommon.GetChildComponent<UISprite>(obj, "mStagePic");
        mLockPic = ObjectCommon.GetChild(obj, "mLockPic");
        mBossIcon = ObjectCommon.GetChildComponent<UISprite>(obj, "mBossIcon");
        mStarSlot0 = ObjectCommon.GetChild(obj, "mStarSlot0");
        mStarSlot1 = ObjectCommon.GetChild(obj, "mStarSlot1");
        mStarSlot2 = ObjectCommon.GetChild(obj, "mStarSlot2");
        mStarIcon0 = ObjectCommon.GetChild(obj, "mStarIcon0");
        mStarIcon1 = ObjectCommon.GetChild(obj, "mStarIcon1");
        mStarIcon2 = ObjectCommon.GetChild(obj, "mStarIcon2");
        mStageNameText = ObjectCommon.GetChildComponent<UILabel>(obj, "mStageNameText");
        mLevelText = ObjectCommon.GetChildComponent<UILabel>(obj, "mLevelText");
        mEnterTimeText = ObjectCommon.GetChildComponent<UILabel>(obj, "mEnterTimesText");
        mMask = ObjectCommon.GetChildComponent<UISprite>(obj, "mMask");
    }

    public void AddListener()
    {
		EventSystem.Instance.addEventListener(StageUnlockEvent.STAGE_UNLOCK, OnSceneUnlock);
    }

    public void RemoveListener()
    {
		EventSystem.Instance.removeEventListener(StageUnlockEvent.STAGE_UNLOCK, OnSceneUnlock);
    }

	// 关联数据
	public void SetData(Scene_StageListTableItem item)
	{
		mListItem = item;

		if(DataManager.Scene_StageSceneTable.ContainsKey(item.mNromalStageId))
		{
			SetStageRes(DataManager.Scene_StageSceneTable[item.mNromalStageId] as Scene_StageSceneTableItem);
		}
	}

	// 关联数据
	public Scene_StageListTableItem GetData()
	{
		return mListItem;
	}

	// 设置锁定状态
	public void SetLocked(bool locked)
	{
		mLockPic.SetActive(locked);

		if(locked)
		{
			mMask.alpha = 0.5f;
		}
		else
		{
			mMask.alpha = 0.0f;
		}
	}

    // 设置可进入次数;
    public void SetEnterTimes(int usetimes)
    {
        mEnterTimeText.text = "次数：" + Mathf.Max(0, maxEnterTime - usetimes) + "/" + maxEnterTime.ToString();
    }

	//是否锁定
	public bool GetLocked()
	{
		return mLockPic.active;
	}

	private void OnSceneUnlock(EventBase evt)
	{
		UpdateUI();
	}

	// 更新显示
	public void UpdateUI()
	{
		if (mListItem == null)
		{
			return;
		}

		PlayerDataModule playermodule = ModuleManager.Instance.FindModule<PlayerDataModule>();
		if(playermodule == null)
		{
			return;
		}

		StageListModule module = ModuleManager.Instance.FindModule<StageListModule>();
		if(module == null)
		{
			return;
		}

		bool unlock = false;
		if (!module.HasNormalLevel(mListItem))
		{
			mStarSlot0.SetActive(false);
			mStarIcon0.SetActive(false);
		}
		else
		{
			mStarSlot0.SetActive(true);
			mStarIcon0.SetActive(playermodule.IsStageHasPassed(mListItem.mNromalStageId));
		}

		if (!module.HasHardLevel(mListItem))
		{
			mStarSlot1.SetActive(false);
			mStarIcon1.SetActive(false);
		}
		else
		{
			mStarSlot1.SetActive(true);
			mStarIcon1.SetActive(playermodule.IsStageHasPassed(mListItem.mHardStageId));
		}

		if (!module.HasSeriousLevel(mListItem))
		{
			mStarSlot2.SetActive(false);
			mStarIcon2.SetActive(false);
		}
		else
		{
			mStarSlot2.SetActive(true);
			mStarIcon2.SetActive(playermodule.IsStageHasPassed(mListItem.mSeriousStageId));
		}
	}

	// 关卡
	public void SetStageRes(Scene_StageSceneTableItem res)
	{
        if (res == null)
            return;

		mStageNameText.text = res.name;
		mLevelText.text = StageDataManager.Instance.GetStageUnlockLevel(res.resID).ToString() + "级";
        maxEnterTime = res.mEnterTimes;
        
        //if(maxEnterTime < 0)
        mEnterTimeText.gameObject.SetActive(maxEnterTime > 0);

		UIAtlasHelper.SetSpriteImage(mStagePic, res.mStageBk);
		UIAtlasHelper.SetSpriteImage(mBossIcon, res.mBossIcon, true);
	}

	// 设置选中状态
	public void SetSelected(bool selected)
	{
		mHover.SetActive(selected);
	}

	// 在列表中位置
	public int Idx
	{
		get
		{
			return mIdx;
		}

		set
		{
			mIdx = value;
		}
	}
}
