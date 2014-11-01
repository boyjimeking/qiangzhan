using UnityEngine;
using System.Collections;

public class UIStageRelive : UIWindow
{
    public UISprite mUIPanel;
    public UIButton mOkBtn;

    public UILabel mLeftTimes;

    public UIButton mNormalBtn;
    public UISprite mNormalIcon;
    public UILabel mNormalCost;

    public UIButton mExtraBtn;
    public UISprite mExtraIcon;
    public UILabel mExtraCost;

    public UISlider mProgressBar;

    private float mTimer = 0.0f;

	private StageReliveModule mModule = ModuleManager.Instance.FindModule<StageReliveModule>();


    protected override void OnLoad()
    {
        mUIPanel = this.GetComponent<UISprite>();
        mOkBtn = this.FindComponent<UIButton>("mOkBtn");
        mLeftTimes = this.FindComponent<UILabel>("mReliveTimesText");

        mNormalBtn = this.FindComponent<UIButton>("mRelivePanel0");
        mNormalIcon = this.FindComponent<UISprite>("mRelivePanel0/mReliveIcon0");
        mNormalCost = this.FindComponent<UILabel>("mRelivePanel0/mReliveCost0");

        mExtraBtn = this.FindComponent<UIButton>("mRelivePanel1");
        mExtraIcon = this.FindComponent<UISprite>("mRelivePanel1/mReliveIcon1");
        mExtraCost = this.FindComponent<UILabel>("mRelivePanel1/mReliveCost1");
        mProgressBar = this.FindComponent<UISlider>("mProgressBar");
    }

    protected override void OnOpen(object param = null)
    {
        EventDelegate.Add(mOkBtn.onClick, OnOkClicked);
        EventDelegate.Add(mNormalBtn.onClick, OnNromalClicked);
        EventDelegate.Add(mExtraBtn.onClick, OnExtraClicked);

		mModule.WaitRelive = false;
        mTimer = 0.0f;

        mUIPanel.alpha = 0.0f;

        mLeftTimes.text = mModule.GetLeftTimes().ToString();
        mNormalCost.text = mModule.GetNormalCost().ToString();
        mExtraCost.text = mModule.GetExtraCost().ToString();
        UIAtlasHelper.SetSpriteImage(mNormalIcon, mModule.GetNormalIcon());
        UIAtlasHelper.SetSpriteImage(mExtraIcon, mModule.GetExtraIcon());
        mProgressBar.value = 0.0f;
    }

    protected override void OnClose()
    {
        
    }

    public override void Update(uint elapsed)
    {
        float delta = (float)elapsed / 1000.0f;//Time.unscaledDeltaTime;
        float newTimer = mTimer + delta;

        if (mTimer < 1.0f)
        {
            if (newTimer > 1.0f)
            {
                mUIPanel.alpha = 1.0f;
            }
            else
            {
                mUIPanel.alpha = newTimer;
            }
        }

		if (mTimer < 10.0f)
		{
			if (newTimer > 10.0f)
			{
				mProgressBar.value = 1.0f;
				if (!mModule.WaitRelive)
				{
					OnOkClicked();
				}
			}
			else
			{
				mProgressBar.value = newTimer / 10.0f;
			}
		}

        mTimer = newTimer;
    }

    private void OnOkClicked()
    {
        SceneManager.Instance.RequestEnterLastCity();
    }

    private void OnNromalClicked()
    {
        EventSystem.Instance.PushEvent(new StageReliveEvent(StageReliveEvent.STAGE_RELIVE_REQUEST, ReliveType.ReliveType_Normal));
    }

    private void OnExtraClicked()
    {
        EventSystem.Instance.PushEvent(new StageReliveEvent(StageReliveEvent.STAGE_RELIVE_REQUEST, ReliveType.ReliveType_Extra));
    }
}
