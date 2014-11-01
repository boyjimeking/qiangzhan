using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIPvpBalance : UIWindow
{
#region 界面控件
	// 离开按钮
	private UIButton mReturnBtn;

	// 胜利
	private UITweener mTitleWin;
	// 失败
	private UITweener mTitleLose;

	// 竞技场数据
	private UISprite mArenaContent;
	// 分数
	private UISprite mScoreIcon;
	// 竞技币
	private UISprite mPointIcon;

	// 排位赛数据
	private UISprite mQualifyingContent;
	// 金币
	private UISprite mGoldIcon;
	// 声望
	private UISprite mReputationIcon;

	// 数值
	private UILabel mText0;
	// 数值
	private UILabel mText1;
#endregion

	private uint mTimer = 0;

	private QualifyingModule mQualifyingModule = ModuleManager.Instance.FindModule<QualifyingModule>();
	private ArenaModule mArenaModule = ModuleManager.Instance.FindModule<ArenaModule>();

    protected override void OnLoad()
    {
		mReturnBtn = this.FindComponent<UIButton>("mBtn");
		mTitleWin = this.FindComponent<UITweener>("mTitleWin");
		mTitleLose = this.FindComponent<UITweener>("mTitleLose");
		mText0 = this.FindComponent<UILabel>("mText0");
		mText1 = this.FindComponent<UILabel>("mText1");

		mArenaContent = this.FindComponent<UISprite>("mArenaContent");
		mScoreIcon = this.FindComponent<UISprite>("mArenaContent/mScoreIcon");
		mPointIcon = this.FindComponent<UISprite>("mArenaContent/mPointIcon");

		mQualifyingContent = this.FindComponent<UISprite>("mQualifyingContent");
		mGoldIcon = this.FindComponent<UISprite>("mQualifyingContent/mGoldIcon");
		mReputationIcon = this.FindComponent<UISprite>("mQualifyingContent/mReputationIcon");
    }

	public override void Update(uint elapsed)
	{
		base.Update(elapsed);

		if(mTimer < 3000)
		{
			mTimer += elapsed;

			if(mTimer >= 3000)
			{
				mView.SetActive(true);
			}
		}
	}

    //界面打开
    protected override void OnOpen(object param = null)
    {
		mView.SetActive(false);
		mTimer = 0;

		Clear();

		if (param == null)
			return;

		if((int)param == 0)
		{
			ShowTitle(mArenaModule.Win);
			ShowArenaAward();
		}
		else if((int)param == 1)
		{
			ShowTitle(mQualifyingModule.Win);
			ShowQualifyingAward();
		}

		EventDelegate.Add(mReturnBtn.onClick, OnReturnBtnClicked);

		EventSystem.Instance.addEventListener(QualifyingEvent.RECEIVE_END_DATA, OnQualifyingEndEvent);
		EventSystem.Instance.addEventListener(ArenaEvent.RECEIVE_END_DATA, OnArenaEndEvent);
    }

    //界面关闭
    protected override void OnClose()
    {
		EventDelegate.Remove(mReturnBtn.onClick, OnReturnBtnClicked);

		EventSystem.Instance.removeEventListener(QualifyingEvent.RECEIVE_END_DATA, OnQualifyingEndEvent);
		EventSystem.Instance.removeEventListener(ArenaEvent.RECEIVE_END_DATA, OnArenaEndEvent);
    }

	private void ShowTitle(bool win)
	{
		mTitleWin.gameObject.SetActive(win);
		mTitleWin.Play();
		mTitleLose.gameObject.SetActive(!win);
		mTitleLose.Play();
	}

	private void Clear()
	{
		mTitleWin.gameObject.SetActive(false);
		mTitleWin.ResetToBeginning();
		mTitleLose.gameObject.SetActive(false);
		mTitleLose.ResetToBeginning();
		mArenaContent.gameObject.SetActive(false);
		mQualifyingContent.gameObject.SetActive(false);
		mText0.text = null;
		mText1.text = null;
	}

	private void OnReturnBtnClicked()
    {
		SceneManager.Instance.RequestEnterLastCity();
    }

	private void ShowQualifyingAward()
	{
		mArenaContent.gameObject.SetActive(false);
		mQualifyingContent.gameObject.SetActive(true);
		mText0.text = StringHelper.GetString("money_prestige") + "X" + mQualifyingModule.AwardPrestige.ToString();
		mText1.text = StringHelper.GetString("money_game") + "X" + mQualifyingModule.AwardGold.ToString();
	}

	private void OnQualifyingEndEvent(EventBase ev)
	{
		ShowQualifyingAward();
	}

	private void ShowArenaAward()
	{
		mArenaContent.gameObject.SetActive(true);
		mQualifyingContent.gameObject.SetActive(false);
		mText0.text = StringHelper.GetString("money_score") + "X" + mArenaModule.AwardScore.ToString();
		mText1.text = StringHelper.GetString("money_arena") + "X" + mArenaModule.AwardPoint.ToString();
	}

	private void OnArenaEndEvent(EventBase ev)
	{
		ShowArenaAward();
	}
}
