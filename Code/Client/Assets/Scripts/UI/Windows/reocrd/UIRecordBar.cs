using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIRecordBar
{
	// 获胜图标
	public UISprite mWinIcon;

	// 失败图标
	public UISprite mLoseIcon;

	// 上升箭头
	public UISprite mUpArrow;

	// 下降箭头
	public UISprite mDownArrow;

	// 平局图标
	public UISprite mDrawArrow;

	// 变化名次
	public UILabel mChangeText;

	// 角色头像
	public UISprite mRoleIcon;

	// 角色名称
	public UILabel mNameText;

	// 角色等级
	public UILabel mLevelText;

	// 记录时间
	public UILabel mTimeText;

	// 查看按钮
	public UIButton mShowBtn;

	private GameObject mObj;

	private uint mStyle = uint.MaxValue;

	private int mIdx = 0;

	public GameObject gameObject
	{
		get
		{
			return mObj;
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
		}
	}

	public UIRecordBar(GameObject obj)
    {
		mObj = obj;
		mWinIcon = ObjectCommon.GetChildComponent<UISprite>(obj, "mWinIcon");
		mLoseIcon = ObjectCommon.GetChildComponent<UISprite>(obj, "mLoseIcon");
		mUpArrow = ObjectCommon.GetChildComponent<UISprite>(obj, "mWinArrow");
		mDownArrow = ObjectCommon.GetChildComponent<UISprite>(obj, "mLoseArrow");
		mDrawArrow = ObjectCommon.GetChildComponent<UISprite>(obj, "mDrawArrow");
		mChangeText = ObjectCommon.GetChildComponent<UILabel>(obj, "mRankChangeText");
		mRoleIcon = ObjectCommon.GetChildComponent<UISprite>(obj, "mSlot/mRoleIcon");
		mNameText = ObjectCommon.GetChildComponent<UILabel>(obj, "mNameText");
		mLevelText = ObjectCommon.GetChildComponent<UILabel>(obj, "mLevelText");
		mTimeText = ObjectCommon.GetChildComponent<UILabel>(obj, "mTimeText");
		mShowBtn = ObjectCommon.GetChildComponent<UIButton>(obj, "mShowBtn");

		mShowBtn.gameObject.SetActive(false);
	}

	public void AddListener()
	{
		EventDelegate.Add(mShowBtn.onClick, OnShowBtnClicked);
	}

	public void RemoveListener()
	{
		EventDelegate.Remove(mShowBtn.onClick, OnShowBtnClicked);
	}

	public void UpdateUI()
	{
		if(mStyle == UIRecord.Style_Arena)
		{
			mUpArrow.alpha = 0.0f;
			mDownArrow.alpha = 0.0f;
			mDrawArrow.alpha = 0.0f;
			mChangeText.text = null;

			ArenaModule module = ModuleManager.Instance.FindModule<ArenaModule>();
			if (module == null)
				return;

			arena_record_s record_s = module.GetRecordData(mIdx);
			if (record_s == null)
			{
				ClearUI();
				return;
			}

			this.gameObject.SetActive(true);

			DisplayRecordTime(record_s.time_stamp);

			bool bWin = false;
			if (string.Equals(record_s.src_name, PlayerDataPool.Instance.MainData.name))
			{
				mRoleIcon.alpha = 1.0f;
				UIAtlasHelper.SetSpriteImage(mRoleIcon, "touxiang:head" + record_s.tar_job);

				mLevelText.text = record_s.tar_level.ToString();
				mNameText.text = record_s.tar_name;

				if (record_s.result > 0)
					bWin = true;
			}
			else
			{
				mRoleIcon.alpha = 1.0f;
				UIAtlasHelper.SetSpriteImage(mRoleIcon, "touxiang:head" + record_s.src_job);

				mLevelText.text = record_s.src_level.ToString();
				mNameText.text = record_s.src_name;

				if (record_s.result < 1)
					bWin = true;
			}

			if (bWin)
			{
				mWinIcon.alpha = 1.0f;
				mLoseIcon.alpha = 0.0f;
			}
			else
			{
				mWinIcon.alpha = 0.0f;
				mLoseIcon.alpha = 1.0f;
			}
		}
		else if(mStyle == UIRecord.Style_Qualifying)
		{
			QualifyingModule module = ModuleManager.Instance.FindModule<QualifyingModule>();
			if (module == null)
				return;

			qualifying_record_s record_s = module.GetRecordData(mIdx);
			if (record_s == null)
			{
				ClearUI();
				return;
			}

			this.gameObject.SetActive(true);

			DisplayRecordTime(record_s.time_stamp);

			bool bWin = false;
			if (string.Equals(record_s.src_name, PlayerDataPool.Instance.MainData.name))
			{
				mRoleIcon.alpha = 1.0f;
				UIAtlasHelper.SetSpriteImage(mRoleIcon, "touxiang:head" + record_s.tar_job);

				mLevelText.text = record_s.tar_level.ToString();
				mNameText.text = record_s.tar_name;

				if (record_s.result > 0)
					bWin = true;

				if (record_s.rank_change > 0)
				{
					mUpArrow.alpha = 0.0f;
					mDownArrow.alpha = 1.0f;
					mDrawArrow.alpha = 0.0f;
					mChangeText.text = record_s.rank_change.ToString();
				}
				else if (record_s.rank_change < 0)
				{
					mUpArrow.alpha = 1.0f;
					mDownArrow.alpha = 0.0f;
					mDrawArrow.alpha = 0.0f;
					mChangeText.text = (-record_s.rank_change).ToString();
				}
				else
				{
					mUpArrow.alpha = 0.0f;
					mDownArrow.alpha = 0.0f;
					mDrawArrow.alpha = 1.0f;
					mChangeText.text = "-";
				}
			}
			else
			{
				mRoleIcon.alpha = 1.0f;
				UIAtlasHelper.SetSpriteImage(mRoleIcon, "touxiang:head" + record_s.src_job);

				mLevelText.text = record_s.src_level.ToString();
				mNameText.text = record_s.src_name;

				if (record_s.result < 1)
					bWin = true;

				if (record_s.rank_change > 0)
				{
					mUpArrow.alpha = 1.0f;
					mDownArrow.alpha = 0.0f;
					mDrawArrow.alpha = 0.0f;
					mChangeText.text = record_s.rank_change.ToString();
				}
				else if (record_s.rank_change < 0)
				{
					mUpArrow.alpha = 0.0f;
					mDownArrow.alpha = 1.0f;
					mDrawArrow.alpha = 0.0f;
					mChangeText.text = (-record_s.rank_change).ToString();
				}
				else
				{
					mUpArrow.alpha = 0.0f;
					mDownArrow.alpha = 0.0f;
					mDrawArrow.alpha = 1.0f;
					mChangeText.text = "-";
				}
			}

			if (bWin)
			{
				mWinIcon.alpha = 1.0f;
				mLoseIcon.alpha = 0.0f;
			}
			else
			{
				mWinIcon.alpha = 0.0f;
				mLoseIcon.alpha = 1.0f;
			}
		}
	}

	public void ClearUI()
	{
		mWinIcon.alpha = 0.0f;
		mLoseIcon.alpha = 0.0f;
		mRoleIcon.alpha = 0.0f;
		mUpArrow.alpha = 0.0f;
		mDownArrow.alpha = 0.0f;
		mDrawArrow.alpha = 0.0f;
		mChangeText.text = null;
		mLevelText.text = null;
		mNameText.text = null;
		mTimeText.text = null;

		this.gameObject.SetActive(false);
	}

	public void SetStyle(uint style)
	{
		mStyle = style;
	}

	private void OnShowBtnClicked()
    {
		
    }

	private void DisplayRecordTime(ulong time)
	{
		ulong passedSec = TimeUtilities.GetUtcNowSeconds() - time;
		
		ulong passedDay = passedSec / 86400;
		if(passedDay > 0)
		{
			mTimeText.text = passedDay.ToString() + StringHelper.GetString("time_day_1") + StringHelper.GetString("time_ago");
			return;
		}

		ulong passedHour = passedSec / 3600;
		if(passedHour > 0)
		{
			mTimeText.text = passedHour.ToString() + StringHelper.GetString("time_hour_1") + StringHelper.GetString("time_ago");
			return;
		}

		ulong passedMin = passedSec / 60;
// 		if(passedMin > 0)
// 		{
			mTimeText.text = passedMin.ToString() + StringHelper.GetString("time_min_1") + StringHelper.GetString("time_ago");
//			return;
//		}
	}
}
