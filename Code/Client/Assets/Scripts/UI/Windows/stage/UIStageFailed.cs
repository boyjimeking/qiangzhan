using UnityEngine;
using System.Collections;

public class UIStageFailed : UIWindow
{
	private UISprite mUIPanel;

	private UITweener mTitle;

	private UIButton mOkBtn;

    protected override void OnLoad()
    {
        mOkBtn = this.FindComponent<UIButton>("mOKBtn");
        mUIPanel = this.GetComponent<UISprite>();
		mTitle = this.FindComponent<UITweener>("mTitle");
    }
    //界面打开
    protected override void OnOpen(object param = null)
    {
        mUIPanel.alpha = 0.0f;

		mTitle.gameObject.SetActive(true);
		mTitle.Play();

        EventDelegate.Add(mOkBtn.onClick, OnOkClicked);
    }
    //界面关闭
    protected override void OnClose()
    {
		mTitle.gameObject.SetActive(false);
		mTitle.ResetToBeginning();

        EventDelegate.Remove(mOkBtn.onClick, OnOkClicked);
    }
    public override void Update(uint elapsed)
    {
        if (mUIPanel.alpha < 1.0f)
        {
            mUIPanel.alpha += ((float)elapsed / 1000.0f);
        }
    }
    private void OnOkClicked()
    {
        SceneManager.Instance.RequestEnterLastCity();
    }
}
