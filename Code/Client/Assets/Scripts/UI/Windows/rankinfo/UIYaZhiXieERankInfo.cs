using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIYaZhiXieERankInfo : UIWindow
{
	public static readonly int Display_Count = 10;

	// 主角
	private UILabel mRankingText;

	// 主角姓名
	private UILabel mNameText;

	// 主角分数
	private UILabel mScoreText;
	
	// 记录面板
	public UIGrid mGrid;

	// 进度条
	public UIProgressBar mProgress;

	// 百分比
	public UILabel mProgressText;

	// 待复制的预设
	public GameObject mCloneSrcPrefab = null;

	private List<UIYaZhiXieERankInfoBar> mBarList = new List<UIYaZhiXieERankInfoBar>();

    protected override void OnLoad()
    {
		mRankingText = this.FindComponent<UILabel>("mWidget/PlayerRankInfo/mRankingText");
		mNameText = this.FindComponent<UILabel>("mWidget/PlayerRankInfo/mNameText");
		mScoreText = this.FindComponent<UILabel>("mWidget/PlayerRankInfo/mScoreText");
		mGrid = this.FindComponent<UIGrid>("mWidget/mItemPanel/mGrid");
		mProgressText = this.FindComponent<UILabel>("mTopWidget/mProgress/mProgressText");
		mProgress = this.FindComponent<UIProgressBar>("mTopWidget/mProgress");

		mCloneSrcPrefab = this.FindChild("mCloneSrc/YZXERankInfoBarUI");

		InitBarList();
    }

    //界面打开
    protected override void OnOpen(object param = null)
    {
		EventSystem.Instance.addEventListener(YaZhiXieEUpdateScoreEvent.YAZHIXIEE_UPDATE_SCORE_EVENT, OnReceivePlayerScore);
		EventSystem.Instance.addEventListener(YaZhiXieEUpdateRankListEvent.YAZHIXIEE_UPDATE_RANKLIST_EVENT, OnReceiveRankList);

		foreach (UIYaZhiXieERankInfoBar ui in mBarList)
		{
			ui.ClearUI();
		}

		mGrid.repositionNow = true;

		ClearUI();

		mNameText.text = PlayerDataPool.Instance.MainData.name;
    }

    //界面关闭
    protected override void OnClose()
    {
		EventSystem.Instance.removeEventListener(YaZhiXieEUpdateScoreEvent.YAZHIXIEE_UPDATE_SCORE_EVENT, OnReceivePlayerScore);
		EventSystem.Instance.removeEventListener(YaZhiXieEUpdateRankListEvent.YAZHIXIEE_UPDATE_RANKLIST_EVENT, OnReceiveRankList);
    }

	private void OnReceivePlayerScore(EventBase ev)
	{
		YaZhiXieEUpdateScoreEvent e = ev as YaZhiXieEUpdateScoreEvent;
		if (e == null)
			return;

		mScoreText.text = e.mScore.ToString();

		int value = (int)((e.mCount * 100) / GameConfig.YZXEMonsterMaxCount);
		mProgressText.text = value.ToString() + "%";
		mProgress.value = (float)(value / 100.0f);
	}

	private void OnReceiveRankList(EventBase ev)
	{
		YaZhiXieEUpdateRankListEvent e = ev as YaZhiXieEUpdateRankListEvent;
		if (e == null || e.sortInfo == null)
			return;

		for (int i = 0; i < mBarList.Count; ++i)
		{
			UIYaZhiXieERankInfoBar ui = mBarList[i];
			if(i >= e.sortInfo.Count)
			{
				ui.ClearUI();
			}
			else
			{
				ui.UpdateUI((i+1).ToString(), e.sortInfo[i].name, e.sortInfo[i].score.ToString());
			}
		}

		mGrid.repositionNow = true;
	}

	private void ClearUI()
	{
		mRankingText.text = null;
		mScoreText.text = "0";
		mProgressText.text = "0%";
		mProgress.value = 0.0f;
	}

	private void InitBarList()
	{
		if (mBarList == null)
			return;

		mBarList.Clear();

		for (int i = 0; i < Display_Count; ++i)
		{
			GameObject obj = WindowManager.Instance.CloneGameObject(mCloneSrcPrefab);
			if (obj == null)
			{
				continue;
			}

			obj.SetActive(true);
			obj.name = "BarUI" + i.ToString();
			obj.transform.parent = mGrid.transform;
			obj.transform.localScale = Vector3.one;

			UIYaZhiXieERankInfoBar itemui = new UIYaZhiXieERankInfoBar(obj);
			itemui.Idx = i;

			mBarList.Add(itemui);
		}

		mGrid.repositionNow = true;
	}
}
