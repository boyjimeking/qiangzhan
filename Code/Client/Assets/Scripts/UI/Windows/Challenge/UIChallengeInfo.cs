using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ChallengeInfoParam
{
    public string mTitle;
    public string mRuleText;
}

public class UIChallengeInfo : UIWindow
{
    public UILabel mTitleText;
    // 返回按钮
    public UIButton mReturnBtn;
    // 竞技场规则
    public UILabel mRuleText;
    protected override void OnLoad()
    {
        base.OnLoad();
        mReturnBtn = this.FindComponent<UIButton>("mReturnBtn");
        mRuleText = this.FindComponent<UILabel>("mRuleText");
        mTitleText = FindComponent<UILabel>("Label");
    }

    protected override void OnOpen(object param = null)
    {
        base.OnOpen(param);
        ChallengeInfoParam info = param as ChallengeInfoParam;
        if (info == null) return;
        mRuleText.text = info.mRuleText;
        mTitleText.text = info.mTitle;
        EventDelegate.Add(mReturnBtn.onClick, OnReturnBtnClicked);
    }

    protected override void OnClose()
    {
        EventDelegate.Remove(mReturnBtn.onClick, OnReturnBtnClicked);
        base.OnClose();
        
    }

    private void OnReturnBtnClicked()
    {
        WindowManager.Instance.CloseUI("challengeinfo");
    }

}

