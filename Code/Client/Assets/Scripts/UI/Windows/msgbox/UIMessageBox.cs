using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void UIMessageBoxCallback();

public class UIMessageBoxParam
{
	public string mMsgText = null;
	public string mOkText = null;
	public string mCancelText = null;
	public UIMessageBoxCallback mOkBtnCallback = null;
	public UIMessageBoxCallback mCancelBtnCallback = null;
}

public class UIMessageBox : UIWindow
{
	// 确定按钮
	public UIButton mOkBtn;
	// 确定文本
	public UILabel mOkLabel;

	// 取消按钮
	public UIButton mCancelBtn;
	// 取消文本
	public UILabel mCancelLabel;

	// 文本
	public UILabel mMessageText;

	// 参数
	private UIMessageBoxParam mParam = null;

    protected override void OnLoad()
    {
		mOkBtn = this.FindComponent<UIButton>("mOkBtn");
		mOkLabel = this.FindComponent<UILabel>("mOkBtn/mOkBtnLabel");
		mCancelBtn = this.FindComponent<UIButton>("mCancelBtn");
		mCancelLabel = this.FindComponent<UILabel>("mCancelBtn/mCancelBtnLabel");
		mMessageText = this.FindComponent<UILabel>("mMessageText");
    }

    //界面打开
    protected override void OnOpen(object param = null)
    {
		if (param == null)
			return;

		mParam = param as UIMessageBoxParam;

		EventDelegate.Add(mOkBtn.onClick, OnOkBtnClicked);
		EventDelegate.Add(mCancelBtn.onClick, OnCancelBtnClicked);

		UpdateUI();
    }

    //界面关闭
    protected override void OnClose()
    {
		if (mParam == null)
			return;

		EventDelegate.Remove(mOkBtn.onClick, OnOkBtnClicked);
		EventDelegate.Remove(mCancelBtn.onClick, OnCancelBtnClicked);
    }

	private void UpdateUI()
	{
		if (mParam == null)
			return;

		mMessageText.text = mParam.mMsgText;

		if(string.IsNullOrEmpty(mParam.mOkText))
		{
			mOkLabel.text = StringHelper.GetString("msgbox_ok");
		}
		else
		{
			mOkLabel.text = StringHelper.GetString(mParam.mOkText);
		}

		if(string.IsNullOrEmpty(mParam.mCancelText))
		{
			mCancelLabel.text = StringHelper.GetString("msgbox_cancel");
		}
		else
		{
			mCancelLabel.text = StringHelper.GetString(mParam.mCancelText);
		}
	}

	private void OnOkBtnClicked()
    {
		WindowManager.Instance.CloseUI("msgbox");

		if(mParam != null && mParam.mOkBtnCallback != null)
		{
			mParam.mOkBtnCallback();
		}
    }

	private void OnCancelBtnClicked()
	{
		WindowManager.Instance.CloseUI("msgbox");

		if(mParam != null && mParam.mCancelBtnCallback != null)
		{
			mParam.mCancelBtnCallback();
		}
	}
}
