using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIArenaInfo : UIWindow
{
	// 返回按钮
	public UIButton mReturnBtn;

	// 最高排名
	public UILabel mBestRankText;
	// 未开始排名
	public UILabel mNoBestRankText;

	// 竞技场规则
	public UILabel mRuleText;

	// 滑动条
	public UIScrollBar mScrollBar;
	// 滑动视图
	public UIScrollView mScrollView;

    private ArenaModule mModule =  ModuleManager.Instance.FindModule<ArenaModule>();

    protected override void OnLoad()
    {
		mReturnBtn = this.FindComponent<UIButton>("mReturnBtn");

		mBestRankText = this.FindComponent<UILabel>("mScrollView/mScrollSprite/mBestRankText");
		mNoBestRankText = this.FindComponent<UILabel>("mScrollView/mScrollSprite/mNoBestRankText");
		mRuleText = this.FindComponent<UILabel>("mScrollView/mScrollSprite/mRuleText");

		mRuleText.text = StringHelper.GetString("arena_rule");

		mScrollBar = this.FindComponent<UIScrollBar>("mScrollBar");
		mScrollView = this.FindComponent<UIScrollView>("mScrollView");

		mScrollBar.gameObject.SetActive(true);
		mScrollBar.foregroundWidget.gameObject.SetActive(false);
		mScrollBar.backgroundWidget.gameObject.SetActive(false);
    }

    //界面打开
    protected override void OnOpen(object param = null)
    {
		EventDelegate.Add(mReturnBtn.onClick, OnReturnBtnClicked);

		PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();

		if (module.GetArenaBestRank() == uint.MaxValue)
		{
			mNoBestRankText.text = StringHelper.GetString("arena_outofrank");
			mNoBestRankText.alpha = 1.0f;
			mBestRankText.alpha = 0.0f;
		}
		else
		{
			mBestRankText.text = (module.GetArenaBestRank() + 1).ToString();
			mBestRankText.alpha = 1.0f;
			mNoBestRankText.alpha = 0.0f;
		}

		mScrollBar.value = 0.0f;
    }

    //界面关闭
    protected override void OnClose()
    {
		EventDelegate.Remove(mReturnBtn.onClick, OnReturnBtnClicked);
    }

	private void OnReturnBtnClicked()
    {
		WindowManager.Instance.CloseUI("arenainfo");
    }
}
