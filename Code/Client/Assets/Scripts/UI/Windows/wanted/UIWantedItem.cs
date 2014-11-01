using System.Collections.Generic;
using UnityEngine;


public class UIWantedItem
{
#region 界面控件
	// 边框
	public GameObject mHover;
	// 遮罩
	public UISprite mMask;
	// 名称
	public UILabel mNameText;
	// 次数
	public UILabel mTimesText;
	// 头像
	public UISprite mBossIcon;
	// 锁定
	public GameObject mLockPic;
	// 评价
	public GameObject[] mGradeList = new GameObject[4];
#endregion

	private Scene_StageSceneTableItem mRes = null;

	private PlayerDataModule mPlayerModule = ModuleManager.Instance.FindModule<PlayerDataModule>();

    private GameObject mObj;

	private bool mIsLocked = false;

    public GameObject gameObject
    {
        get
        {
            return mObj;
        }
    }

	public UIWantedItem(GameObject obj)
    {
        mObj = obj;

        mHover = ObjectCommon.GetChild(obj , "mHover");
		mMask = ObjectCommon.GetChildComponent<UISprite>(obj, "mMask");
		mNameText = ObjectCommon.GetChildComponent<UILabel>(obj, "mNameText");
		mTimesText = ObjectCommon.GetChildComponent<UILabel>(obj, "mTimesText");
		mBossIcon = ObjectCommon.GetChildComponent<UISprite>(obj, "mBossIcon");
		mLockPic = ObjectCommon.GetChild(obj, "mLockPic");

		mGradeList[0] = ObjectCommon.GetChild(obj, "mGradeC");
		mGradeList[1] = ObjectCommon.GetChild(obj, "mGradeB");
		mGradeList[2] = ObjectCommon.GetChild(obj, "mGradeA");
		mGradeList[3] = ObjectCommon.GetChild(obj, "mGradeS");
    }

    public void AddListener()
    {
		EventSystem.Instance.addEventListener(StageUnlockEvent.STAGE_UNLOCK, OnSceneUnlock);
    }

    public void RemoveListener()
    {
		EventSystem.Instance.removeEventListener(StageUnlockEvent.STAGE_UNLOCK, OnSceneUnlock);
    }

	public void InitUI()
	{
		mNameText.text = Res.name;
		UIAtlasHelper.SetSpriteImage(mBossIcon, Res.mBossIcon, true);
		mMask.alpha = 0.0f;
	}

	// 更新可进入次数;
	private void UpdateEnterTimes()
	{
		if(Res.mEnterTimes > 0)
		{
			mTimesText.text = "次数:" + (Res.mEnterTimes - mPlayerModule.GetStageDailyTimes(Res.resID)).ToString() + "/" + Res.mEnterTimes.ToString();
		}
		else
		{
			mTimesText.text = null;
		}
	}

	//是否锁定
	public bool IsLocked()
	{
		return mIsLocked;
	}

	public void SetLocked(bool locked)
	{
		mIsLocked = locked;
		mLockPic.SetActive(locked);
	}

 	private void OnSceneUnlock(EventBase evt)
 	{
 		UpdateUI();
 	}
 
 	// 更新显示
 	public void UpdateUI()
 	{
		UpdateLocked();

		UpdateEnterTimes();

		UpdateGrade();
 	}

	private void UpdateLocked()
	{
		if (mPlayerModule.IsStageUnlock(Res.resID))
		{
			gameObject.SetActive(true);
			SetLocked(false);
			mMask.alpha = 0.0f;
		}
		else
		{
			int preId = StageDataManager.Instance.GetPervStageId(Res.resID);
			if (mPlayerModule.IsStageUnlock(preId))
			{
				gameObject.SetActive(true);
			}
			else
			{
				gameObject.SetActive(false);
			}

			SetLocked(true);
			mMask.alpha = 0.5f;
		}
	}

	private void UpdateGrade()
	{
		int grade = (int)mPlayerModule.GetStageGrade(Res.resID);
		
		for(int i = 0; i < 4; ++i)
		{
			mGradeList[i].SetActive(grade == i);
		}
	}

	// 设置选中状态
	public void SetSelected(bool selected)
	{
		mHover.SetActive(selected);
	}

	public Scene_StageSceneTableItem Res
	{
		get
		{
			return mRes;
		}

		set
		{
			mRes = value;
		}
	}
}
