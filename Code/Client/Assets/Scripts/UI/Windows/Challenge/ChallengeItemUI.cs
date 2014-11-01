
using UnityEngine;


public class ChallengeItemUI
{
    public delegate void OnObjectClick(ChallengeItemUI script);

    private UIButton mButton;
    public UILabel  mLabel;

    private UISprite mSpriteHead;
    public UILabel  mLabelFloorIcon;

    //成就背景
    private UISprite mSpriteChengjiuBg;
    private UISprite mSpriteAchieveOne;
    private UISprite mSpriteAchieveTwo;
    private UISprite mSpriteAchieveThree;

    public GameObject mGameObject;



    public int mFloorID;    //层数id
    public int mIndex = 0;
    private bool mCompleteOne = false;
    private bool mCompleteTwo = false;
    private bool mCompleteThree = false;
    private bool mSelected = false;
    private bool mLocked = true;

    private ChallengeState mState = ChallengeState.Never;

    public OnObjectClick OnClickCallback = null;

    public ChallengeItemUI(GameObject obj)
    {
        mGameObject = obj;

        mButton = mGameObject.GetComponent<UIButton>();
        mLabel = ObjectCommon.GetChildComponent<UILabel>(mGameObject, "Label");
        mSpriteHead = ObjectCommon.GetChildComponent<UISprite>(mGameObject, "Head");
        mLabelFloorIcon = ObjectCommon.GetChildComponent<UILabel>(mGameObject, "FloorIcon");
        mSpriteChengjiuBg = ObjectCommon.GetChildComponent<UISprite>(mGameObject, "chengjiuBg");
        mSpriteAchieveOne = ObjectCommon.GetChildComponent<UISprite>(mGameObject, "Achieve1");
        mSpriteAchieveTwo = ObjectCommon.GetChildComponent<UISprite>(mGameObject, "Achieve2");
        mSpriteAchieveThree = ObjectCommon.GetChildComponent<UISprite>(mGameObject, "Achieve3");

        UIEventListener.Get(mGameObject).onClick = OnClick;
    }

    public void OnClick(GameObject go)
    {
        SoundManager.Instance.Play(15);
        if (OnClickCallback == null)
            return;

        OnClickCallback(this);
    }

    public void Refresh()
    {
        switch (mState)
        {
            case ChallengeState.Current:
                //当前层是玩家所在的层 显示玩家头像
                UIAtlasHelper.SetButtonImage(mButton, "pata:pata-004", true);
                UIAtlasHelper.SetSpriteImage(mSpriteHead, "touxiang:head1", true);
                SetFloorIconVisible(false);
                RefreshAchieveMent(false);
                break;

                //显示已通关
            case ChallengeState.Passed:
                RefreshAchieveMent(true);
                UIAtlasHelper.SetButtonImage(mButton, mSelected ? "pata:pata-gai002" : "pata:pata-gai001",
                    true);
                UIAtlasHelper.SetSpriteImage(mSpriteHead, "pata:pata-gai003", true);
                mSpriteHead.gameObject.SetActive(true);
                SetFloorIconVisible(true);
                break;

            case ChallengeState.NoPass:
                RefreshAchieveMent(false);
                 UIAtlasHelper.SetButtonImage(mButton, mSelected ? "pata:pata-110" : "pata:pata-005", true);
                 UIAtlasHelper.SetSpriteImage(mSpriteHead, mFloorID % 5 == 0 ? "pata:pata-007" : "pata:pata-006", true);
                 SetFloorIconVisible(false);
                 break;
            case ChallengeState.Never:
                RefreshAchieveMent(false);
                UIAtlasHelper.SetButtonImage(mButton, mSelected ? "pata:pata-110" : "pata:pata-005", true);
                UIAtlasHelper.SetSpriteImage(mSpriteHead, mFloorID % 5 == 0 ? "pata:pata-007" : "pata:pata-006", true);
                SetFloorIconVisible(false);
                break;
        }
    }

    public void UpdateData(ChallengeState state, bool complete_one, bool complete_two, bool complete_three)
    {
        mState = state;
        mCompleteOne = complete_one;
        mCompleteTwo = complete_two;
        mCompleteThree = complete_three;
        Refresh();
    }

    //刷新成就
    private void RefreshAchieveMent(bool show)
    {
        mSpriteChengjiuBg.gameObject.SetActive(show);
        if (!show) 
            return;

        UIAtlasHelper.SetSpriteGrey(mSpriteAchieveOne, !mCompleteOne);
        UIAtlasHelper.SetSpriteGrey(mSpriteAchieveTwo, !mCompleteTwo);
        UIAtlasHelper.SetSpriteGrey(mSpriteAchieveThree, !mCompleteThree);
    }

    public void SetFloorIconVisible(bool visible)
    {
        mLabelFloorIcon.gameObject.SetActive(visible);
    }

    public void SetSelected(bool selected)
    {
        mSelected = selected;
        Refresh();   
    }

    public bool IsSelected()
    {
        return false;
    }
}
