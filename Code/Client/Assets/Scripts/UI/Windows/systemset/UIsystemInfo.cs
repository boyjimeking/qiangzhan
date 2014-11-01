using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIsystemInfo : UIWindow
{
    // 返回按钮
    public UIButton mReturnBtn;
    // 竞技场规则
    public UILabel mRuleText;

    private ArenaModule mModule = ModuleManager.Instance.FindModule<ArenaModule>();

    protected override void OnLoad()
    {
        mReturnBtn = this.FindComponent<UIButton>("mReturnBtn");
        mRuleText = this.FindComponent<UILabel>("mRuleText");
    }

    //界面打开
    protected override void OnOpen(object param = null)
    {
        EventDelegate.Add(mReturnBtn.onClick, OnReturnBtnClicked);
        mRuleText.text = "[fdc718]" + param as string;
    }

    //界面关闭
    protected override void OnClose()
    {
        EventDelegate.Remove(mReturnBtn.onClick, OnReturnBtnClicked);
    }

    private void OnReturnBtnClicked()
    {
        WindowManager.Instance.CloseUI("systeminfo");
    }
}
