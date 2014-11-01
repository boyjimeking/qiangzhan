using UnityEngine;
using System.Collections;

public class UIStory : UIWindow
{
    public UIPanel mUI;

    // 上方对话
    public StoryDialogUI mTopDlg;

    // 下方对话
    public StoryDialogUI mBottomDlg;

    // 跳过情节
    public UIButton mSkipBtn;

    // 点击继续
    public UIButton mMaskBtn;

    // 计时
    private float mTimer = 0.0f;

    // 当前情节
    private StoryTableItem mRes = null;

    // 当前阶段
    private UIState mState = UIState.STATE_INVALID;

    // UI阶段
    private enum UIState : int
    {
        // 原始状态
        STATE_INVALID = 0,

        // fadein
        STATE_FADEIN,

        // 情节展现
        STATE_STORY,

        // fadeout
        STATE_FADEOUT,
    }
    public UIStory()
    {

    }
    protected override void OnLoad()
    {
        mUI = mView.GetComponent<UIPanel>();
        mSkipBtn = this.FindComponent<UIButton>("mMaskBar/mSkipBtn");
        mMaskBtn = this.FindComponent<UIButton>("mMaskBtn");

        mTopDlg = new StoryDialogUI();

        mTopDlg.mWidget = this.FindComponent<UIWidget>("mTopWidget");
        mTopDlg.mHead = this.FindComponent<UISprite>("mTopWidget/mTopHead");
        mTopDlg.mHeadAnchor = this.FindComponent<UIAnchor>("mTopWidget/mTopHead");
        mTopDlg.mAnchor = this.FindComponent<UIAnchor>("mTopWidget/mTopTextPanel");
        mTopDlg.mTitle = this.FindComponent<UILabel>("mTopWidget/mTopTextPanel/mTopTitleText");
        mTopDlg.mTitleEffect = this.FindChild("mTopWidget/mTopTextPanel/mTopTitleText");
        mTopDlg.mContent = this.FindComponent<UILabel>("mTopWidget/mTopTextPanel/mTopContentText");
        mTopDlg.mContentEffect = this.FindChild("mTopWidget/mTopTextPanel/mTopContentText");
        mTopDlg.mBtn = this.FindComponent<UISprite>("mTopWidget/mTopTextPanel/mTopPressBtn");

        mBottomDlg = new StoryDialogUI();
        mBottomDlg.mWidget = this.FindComponent<UIWidget>("mBottomWidget");
        mBottomDlg.mHead = this.FindComponent<UISprite>("mBottomWidget/mBottomHead");
        mBottomDlg.mHeadAnchor = this.FindComponent<UIAnchor>("mBottomWidget/mBottomHead");
        mBottomDlg.mAnchor = this.FindComponent<UIAnchor>("mBottomWidget/mBottomTextPanel");
        mBottomDlg.mTitle = this.FindComponent<UILabel>("mBottomWidget/mBottomTextPanel/mBottomTitleText");
        mBottomDlg.mTitleEffect = this.FindChild("mBottomWidget/mBottomTextPanel/mBottomTitleText");
        mBottomDlg.mContent = this.FindComponent<UILabel>("mBottomWidget/mBottomTextPanel/mBottomContentText");
        mBottomDlg.mContentEffect = this.FindChild("mBottomWidget/mBottomTextPanel/mBottomContentText");
        mBottomDlg.mBtn = this.FindComponent<UISprite>("mBottomWidget/mBottomTextPanel/mBottomPressBtn");
    }
    protected override void OnOpen(object param = null)
    {
        if (mTopDlg != null)
            mTopDlg.Open();
        if (mBottomDlg != null)
            mBottomDlg.Open();
        mTimer = 0.0f;
        mState = UIState.STATE_FADEIN;
		mSkipBtn.gameObject.SetActive(false);

        EventDelegate.Add(mSkipBtn.onClick, onSkipBtnClicked);
        EventDelegate.Add(mMaskBtn.onClick, onMaskBtnClicked);

        EventSystem.Instance.addEventListener(StoryEvent.STORY_STEP_BEGIN, onStoryStepBegin);
        EventSystem.Instance.addEventListener(StoryEvent.STORY_END, onStoryEnd);
    }
    protected override void OnClose()
    {
        mRes = null;

        EventDelegate.Remove(mSkipBtn.onClick, onSkipBtnClicked);
        EventDelegate.Remove(mMaskBtn.onClick, onMaskBtnClicked);

        EventSystem.Instance.removeEventListener(StoryEvent.STORY_STEP_BEGIN, onStoryStepBegin);
        EventSystem.Instance.removeEventListener(StoryEvent.STORY_END, onStoryEnd);
    }
    public override void Update(uint elapsed)
    {
        if( mTopDlg != null )
            mTopDlg.Update(elapsed);
        if (mBottomDlg != null)
            mBottomDlg.Update(elapsed);
        switch (mState)
        {
            case UIState.STATE_INVALID:
                {

                }
                break;
            case UIState.STATE_FADEIN:
                {
					mTimer += (float)(elapsed * 0.001);//Time.unscaledDeltaTime;
                    mUI.alpha = mTimer / 1.0f;
                    if (mTimer > 1.0f)
                    {
                        mTimer = 0.0f;
                        mState = UIState.STATE_STORY;

                        onStoryStepBegin(null);
                    }
                }
                break;
            case UIState.STATE_STORY:
                {

                }
                break;
            case UIState.STATE_FADEOUT:
                {
					mTimer += (float)(elapsed * 0.001);// Time.unscaledDeltaTime;
                    if (mTimer > 1.0f)
                    {
                        mUI.alpha = 0.0f;
                        mTimer = 0.0f;
                        mState = UIState.STATE_INVALID;

                        mTopDlg.Clear();
                        mBottomDlg.Clear();
                        WindowManager.Instance.CloseUI("story");
                    }
                    else
                    {
                        mUI.alpha = (1.0f - mTimer) / 1.0f;
                    }
                }
                break;
        }
    }
    // 情节结束
    private void onStoryEnd(EventBase e)
    {
        mTopDlg.Skip();
        mBottomDlg.Skip();

        if (mState == UIState.STATE_FADEOUT)
        {
            return;
        }
        else if (mState == UIState.STATE_FADEIN)
        {
            mTimer = 1.0f - mTimer;
            mState = UIState.STATE_FADEOUT;
        }
        else
        {
            mState = UIState.STATE_FADEOUT;
        }
    }

    // 情节步骤开始
    private void onStoryStepBegin(EventBase e)
    {
        StoryTableItem nextRes = StoryManager.Instance.GetCurStoryRes();

        if (mRes != null)
        {
            // 需要关闭对侧的对话框
            if (nextRes.closeOther > 0 && !isSamePos(mRes.headPos, nextRes.headPos))
            {
                if (mRes.headPos == 0 || mRes.headPos == 1)
                {
                    mTopDlg.Skip();
                }
                else
                {
                    mBottomDlg.Skip();
                }
            }
        }

        mRes = nextRes;

		mSkipBtn.gameObject.SetActive(mRes.canSkipAll);

        if (mRes.headPos == 0 || mRes.headPos == 1)
        {
            mTopDlg.UpdateStory(mRes);
        }
        else
        {
            mBottomDlg.UpdateStory(mRes);
        }
    }

    // 跳过情节
    private void onSkipBtnClicked()
    {
        EventSystem.Instance.PushEvent(new StoryEvent(StoryEvent.STORY_SKIP));
    }

    // 跳过步骤
    private void onMaskBtnClicked()
    {
        mTopDlg.Next();
        mBottomDlg.Next();
    }

    // 是否是相同位置对话框
    private bool isSamePos(int pos1, int pos2)
    {
        if (pos1 == 0 || pos1 == 1)
        {
            return (pos2 == 0 || pos2 == 1);
        }
        else if (pos1 == 2 || pos1 == 3 || pos1 < 0)
        {
            return (pos2 == 2 || pos2 == 3 || pos2 < 0);
        }

        return false;
    }
}
