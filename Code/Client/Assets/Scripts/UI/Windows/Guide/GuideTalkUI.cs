using System;
using UnityEngine;

class GuideTalkUI
{
    private GameObject mMaskBar;

    // 上方对话
    public GuideTalkDialogUI mTopDlg;

    // 下方对话
    public GuideTalkDialogUI mBottomDlg;

    public GameObject mMaskBtn;
    public GameObject mSkipBtn;


    private GameObject mObject = null;

    private bool mOpened = false;
    private int mDepth = 1000;

    public GuideTalkUI(GameObject obj)
    {
        mObject = obj;
        mMaskBar = ObjectCommon.GetChild(mObject, "mMaskBar");

        mMaskBtn = ObjectCommon.GetChild(mObject, "mMaskBtn");
        mSkipBtn = ObjectCommon.GetChild(mObject, "mMaskBar/mSkipBtn");

        mTopDlg = new GuideTalkDialogUI();


        mTopDlg.mWidget = ObjectCommon.GetChildComponent<UIWidget>(mObject,"mTopWidget");
        mTopDlg.mHead = ObjectCommon.GetChildComponent<UISprite>(mObject, "mTopWidget/mTopHead");
        mTopDlg.mHeadAnchor = ObjectCommon.GetChildComponent<UIAnchor>(mObject, "mTopWidget/mTopHead");
        mTopDlg.mAnchor = ObjectCommon.GetChildComponent<UIAnchor>(mObject, "mTopWidget/mTopTextPanel");
        mTopDlg.mTitle = ObjectCommon.GetChildComponent<UILabel>(mObject, "mTopWidget/mTopTextPanel/mTopTitleText");
        mTopDlg.mTitleEffect = ObjectCommon.GetChild(mObject, "mTopWidget/mTopTextPanel/mTopTitleText");
        mTopDlg.mContent = ObjectCommon.GetChildComponent<UILabel>(mObject, "mTopWidget/mTopTextPanel/mTopContentText");
        mTopDlg.mContentEffect = ObjectCommon.GetChild(mObject, "mTopWidget/mTopTextPanel/mTopContentText");
        mTopDlg.mBtn = ObjectCommon.GetChildComponent<UISprite>(mObject, "mTopWidget/mTopTextPanel/mTopPressBtn");

        mBottomDlg = new GuideTalkDialogUI();
        mBottomDlg.mWidget = ObjectCommon.GetChildComponent<UIWidget>(mObject, "mBottomWidget");
        mBottomDlg.mHead = ObjectCommon.GetChildComponent<UISprite>(mObject, "mBottomWidget/mBottomHead");
        mBottomDlg.mHeadAnchor = ObjectCommon.GetChildComponent<UIAnchor>(mObject, "mBottomWidget/mBottomHead");
        mBottomDlg.mAnchor = ObjectCommon.GetChildComponent<UIAnchor>(mObject, "mBottomWidget/mBottomTextPanel");
        mBottomDlg.mTitle = ObjectCommon.GetChildComponent<UILabel>(mObject, "mBottomWidget/mBottomTextPanel/mBottomTitleText");
        mBottomDlg.mTitleEffect = ObjectCommon.GetChild(mObject, "mBottomWidget/mBottomTextPanel/mBottomTitleText");
        mBottomDlg.mContent = ObjectCommon.GetChildComponent<UILabel>(mObject, "mBottomWidget/mBottomTextPanel/mBottomContentText");
        mBottomDlg.mContentEffect = ObjectCommon.GetChild(mObject, "mBottomWidget/mBottomTextPanel/mBottomContentText");
        mBottomDlg.mBtn = ObjectCommon.GetChildComponent<UISprite>(mObject, "mBottomWidget/mBottomTextPanel/mBottomPressBtn");

        mSkipBtn.SetActive(false);
        mMaskBar.SetActive(false);
        mMaskBtn.SetActive(false);
    }

    public GameObject gameObject
    {
        get
        {
            return mObject;
        }
    }


    public void Open(string talk, int talkpos , int depth)
    {
        if (mDepth != depth)
        {
            mDepth = depth;
            WindowManager.Instance.SetDepth(gameObject, mDepth);
        }
        SetData(talk, talkpos);
        Show();
        mOpened = true;
    }

    public void Close()
    {
        mOpened = false;
        Hide();
    }

    public void SetData(string talk, int talkpos)
    {
        if (talk == null)
        {
            return;
        }

        mTopDlg.Open();
        mBottomDlg.Open();

        if (talkpos == 0)
        {
            mTopDlg.UpdateTalk(true, talk);
        }
        else
        {
            mBottomDlg.UpdateTalk(false, talk);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
    public bool IsShow()
    {
        return gameObject.activeSelf;
    }

    public bool IsOpened()
    {
        return mOpened;
    }
}
