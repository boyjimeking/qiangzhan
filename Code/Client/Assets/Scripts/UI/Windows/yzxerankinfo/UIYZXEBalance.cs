using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIYZXEBalanceParam
{
	public uint score;
	public uint time;
	public int result;

	public UIYZXEBalanceParam(uint s, uint t, int r)
	{
		score = s;
		time = t;
		result = r;
	}
}

public class UIYZXEBalance : UIWindow
{
#region 界面控件
	// 离开按钮
	private UIButton mReturnBtn;

	// 胜利
	private UITweener mTitleWin;
	// 失败
	private UITweener mTitleLose;

	// 胜利信息
	private GameObject mWinUI;
	// 失败信息
	private GameObject mLoseUI;

	// 积分
	private UILabel mScoreText;
	// 时间
	private UILabel mTimeText;
#endregion

    protected override void OnLoad()
    {
		mReturnBtn = this.FindComponent<UIButton>("mBtn");
		mTitleWin = this.FindComponent<UITweener>("mWinUI/mTitleWin");
		mTitleLose = this.FindComponent<UITweener>("mLoseUI/mTitleLose");
		mScoreText = this.FindComponent<UILabel>("mScoreText");
		mTimeText = this.FindComponent<UILabel>("mTimeText");
		mWinUI = this.FindChild("mWinUI");
		mLoseUI = this.FindChild("mLoseUI");
    }

    //界面打开
    protected override void OnOpen(object param = null)
    {
		if (param == null)
			return;

		UpdateUI(param as UIYZXEBalanceParam);

		EventDelegate.Add(mReturnBtn.onClick, OnReturnBtnClicked);
    }

    //界面关闭
    protected override void OnClose()
    {
		Clear();

		EventDelegate.Remove(mReturnBtn.onClick, OnReturnBtnClicked);
    }

	private void UpdateUI(UIYZXEBalanceParam param)
	{
		if (param == null)
			return;

		if(param.result > 0)
		{
			mWinUI.SetActive(true);
			mLoseUI.SetActive(false);
			mTitleWin.Play();
		}
		else
		{
			mWinUI.SetActive(false);
			mLoseUI.SetActive(true);
			mTitleLose.Play();
		}

		mScoreText.text = param.score.ToString();

		uint min = (param.time / 60000);
		uint sec = (param.time % 60000) / 1000;
		mTimeText.text = min.ToString() + StringHelper.GetString("time_min_0") + sec.ToString() + StringHelper.GetString("time_sec");
	}

	private void Clear()
	{
		mTitleWin.gameObject.SetActive(false);
		mTitleWin.ResetToBeginning();
		mTitleLose.gameObject.SetActive(false);
		mTitleLose.ResetToBeginning();
		mWinUI.SetActive(false);
		mLoseUI.SetActive(false);
		mScoreText.text = null;
		mTimeText.text = null;
	}

	private void OnReturnBtnClicked()
    {
		SceneManager.Instance.RequestEnterLastCity();
    }
}
