using UnityEngine;
using System.Collections;

public class UIQueRen : UIWindow
{

	public UILabel mTitleLb;
	public UILabel mDetailLb;
    public UILabel mYesLb;
    public UILabel mNoLb;
	public UIButton mYesBt;
	public UIButton mNoBt;
    public UIButton mReturn;

    protected override void OnLoad()
    {
        base.OnLoad();

        mTitleLb = this.FindComponent<UILabel>("titleLb");
        mDetailLb = this.FindComponent<UILabel>("detailLb");
        mYesLb = this.FindComponent<UILabel>("yesBt/Label");
        mNoLb = this.FindComponent<UILabel>("noBt/Label");
        mYesBt = this.FindComponent<UIButton>("yesBt");
        mNoBt = this.FindComponent<UIButton>("noBt");
        mReturn = this.FindComponent<UIButton>("returnBtn");
    }

    protected override void OnOpen(object param = null)
    {
        base.OnOpen();

		EventDelegate.Add(mYesBt.onClick , OnYesClick);
		EventDelegate.Add(mNoBt.onClick , OnNoClick);
        EventDelegate.Add(mReturn.onClick, OnReturnClick);

		UpdateUI(null);
		EventSystem.Instance.addEventListener(QueRenEvent.CONTENT_CHANGE , UpdateUI);
    }

    protected override void OnClose()
    {
        base.OnClose();
		
        EventSystem.Instance.removeEventListener(QueRenEvent.CONTENT_CHANGE , UpdateUI);
		EventDelegate.Remove(mYesBt.onClick , OnYesClick);
		EventDelegate.Remove(mNoBt.onClick , OnNoClick);
        EventDelegate.Remove(mReturn.onClick, OnReturnClick);
    }

	void UpdateUI(EventBase ev)
	{
		YesOrNoBoxManager yb = YesOrNoBoxManager.Instance;

		mTitleLb.text = yb.TitleString;
		mDetailLb.text = yb.ContentString;
        mYesLb.text = yb.YesLabel;
        mNoLb.text = yb.NoLabel;
	}

	void OnYesClick()
	{
		YesOrNoBoxManager.Instance.OnYesBtnClick();
	}

	void OnNoClick()
	{
		YesOrNoBoxManager.Instance.OnNoBtnClick();
	}

    void OnReturnClick()
    {
        YesOrNoBoxManager.Instance.OnReturnClick();
    }
}
