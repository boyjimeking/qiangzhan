
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class UIChallengeDrop : UIWindow
{
    public enum UIState
    {
        ShowDrop,
        Waiting
    }

    // Use this for initialization
    private UILabel  FloorNum;
    private UILabel Scribe;
    private UIButton mBack;
    private UIButton Sweep;
    private UIButton Continue;
    private UIGrid BtnGrid;
    private GameObject ExamDrop;
    private UILabel CurScore;
    private UILabel MaxScore;
    private UISpriteAnimation Jilu;
    private GameObject Main;
    private AchieveItemUI AchieveOne;
    private AchieveItemUI AchieveTwo;
    private AchieveItemUI AchieveThree;
    private GameObject MContainer;
    private GameObject WaitUI; //等待界面
    private GameObject Background;

    private UIPlayTween mTween;
    private UIPlayTween mBackTween;
    private UIPlayTween mContinueTween;
    private UIPlayTween mSweepTween;

    private ChallengeModule mModule;
    private float mMaxWaitTime = 2; //等待时间
    private float mWaitTime;
    private bool IsInit;
    private UIStep mCurStep = UIStep.STEP_2;
    private float mTimer = 0.0f;
    private AwardWidgetUI mAwardW;
    private UIState mState;
    private int curShowScore = -1;
   
    private ChallengeCompleteParam mParam = null;

    public enum UIStep
    {
        // 原始状态
        STEP_ORIGINAL = 0,
        STEP_0,
        STEP_1,
        STEP_2,
        STEP_3,
        STEP_4,
        STEP_5,
        STEP_AchieveAnim,
        STEP_Wait,
    }

    public static float STEPTIME_0 = 0.5f;
    public static float STEPTIME_3 = 1;
    public static float STEPTIME_Wait = 0.001f;

    protected override void OnLoad()
    {
        base.OnLoad();

        FloorNum = FindComponent<UILabel>("container/Main/Content/FloorNum");
        Scribe = FindComponent<UILabel>("container/Main/Content/Scribe");
        mBack = FindComponent<UIButton>("container/Main/Content/BtnGrid/Back");
        Sweep = FindComponent<UIButton>("container/Main/Content/BtnGrid/Sweep");
        Continue = FindComponent<UIButton>("container/Main/Content/BtnGrid/Continue");
        BtnGrid = FindComponent<UIGrid>("container/Main/Content/BtnGrid");
        CurScore = FindComponent<UILabel>("container/Main/Content/CurScore");
        MaxScore = FindComponent<UILabel>("container/Main/Content/MaxScore");
        Jilu = FindComponent<UISpriteAnimation>("container/Main/Content/jilu");
        Jilu.gameObject.SetActive(false);
        Main = FindChild("container/Main");
        AchieveOne = new AchieveItemUI(FindChild("container/Main/Content/AchieveGrid/Achieve1"));
        AchieveTwo = new AchieveItemUI(FindChild("container/Main/Content/AchieveGrid/Achieve2"));
        AchieveThree = new AchieveItemUI(FindChild("container/Main/Content/AchieveGrid/Achieve3"));
        MContainer = FindChild("container");
        WaitUI = FindChild("container/Waiting");
        Background = FindChild("container/Main/Background");
        mTween = FindComponent<UIPlayTween>("container");
        mBackTween = FindComponent<UIPlayTween>("container/Main/Content/BtnGrid/Back");
        mBackTween.resetOnPlay = true;
        mContinueTween = FindComponent<UIPlayTween>("container/Main/Content/BtnGrid/Continue");
        mContinueTween.resetOnPlay = true;
        mSweepTween = FindComponent<UIPlayTween>("container/Main/Content/BtnGrid/Sweep");
        mSweepTween.resetOnPlay = true;
        mAwardW = new AwardWidgetUI(FindChild("container/AwardWidget"));
        mAwardW.setShow(false);
        mCurStep = UIStep.STEP_5;
    }

    
    protected override void OnOpen(object param = null)
    {
        WindowManager.Instance.CloseUI("challengecountdown");
        mModule = ModuleManager.Instance.FindModule<ChallengeModule>();
        EventDelegate.Add(mBack.onClick, OnBack);
        EventDelegate.Add(Continue.onClick, OnContinue);
        EventDelegate.Add(Sweep.onClick, OnSweep);
        UIEventListener.Get(Background).onClick = OnBackgroundClick;
        EventSystem.Instance.addEventListener(ChallengeEvent.SWEEP_DROP, RefreshBtn);
        mParam = param as ChallengeCompleteParam;
        if (mParam == null)
            return;
        MState = UIState.ShowDrop;
        FloorNum.text = String.Format(StringHelper.GetString("floornum2"), mParam.mFloor);
        var tableItem = DataManager.ChallengeTable[mParam.mFloor] as ChallengeTableItem;
        if (tableItem != null) Scribe.text = tableItem.mDropTip;
        CurScore.text = tableItem.mFloorScore.ToString();
        MaxScore.text = String.Format(StringHelper.GetString("histortyScore"), mParam.mHistortyScore);
        mAwardW.SetShowInfo(mParam);
        AchieveOne.mMaxNum = tableItem.mAchieveScoreOne;
        AchieveTwo.mMaxNum = tableItem.mAchieveScoreTwo;
        AchieveThree.mMaxNum = tableItem.mAchieveScoreThree;
        AchieveOne.mNum.enabled = mParam.mAchieveOne;
        AchieveTwo.mNum.enabled = mParam.mAchieveTwo;
        AchieveThree.mNum.enabled = mParam.mAchieveThree;
        mCurStep = UIStep.STEP_ORIGINAL;
             
    }

    protected override void OnClose()
    {
        EventDelegate.Remove(mBack.onClick, OnBack);
        EventDelegate.Remove(Continue.onClick, OnContinue);
        EventDelegate.Remove(Sweep.onClick, OnSweep);
        UIEventListener.Get(Background).onClick = null;
        EventSystem.Instance.removeEventListener(ChallengeEvent.SWEEP_DROP, RefreshBtn);
        UIEventListener.Get(Background).onClick = null;      
    }

    private void OnBackgroundClick(GameObject target)
    {
        SoundManager.Instance.Play(15);
        WindowManager.Instance.CloseUI("sweepDrop");
        WindowManager.Instance.CloseUI("quickChallenge");
    }

    private void OnSweep()
    {
        SoundManager.Instance.Play(15);
        WindowManager.Instance.OpenUI("quickChallenge");
    }

    private void OnBack()
    {
        SoundManager.Instance.Play(15);
        WindowManager.Instance.CloseUI("challengeDrop");
        SceneManager.Instance.RequestEnterLastCity();
        WindowManager.Instance.OpenUI("challenge",2);
    }

    private void OnContinue()
    {
        OnYesClick();
        //int recom_grade;
        //int my_grade;
        //if (mModule.IsBattleGradeEnough(mModule.GetDoingFloor(), out recom_grade, out my_grade))
        //{
        //    OnYesClick();
        //}
        //else
        //{
        //    YesOrNoBoxManager.Instance.ShowYesOrNoUI(
        //      "",
        //      StringHelper.GetString("gradetext"),
        //      OnYesClick,
        //      OnNoClick,
        //      StringHelper.GetString("stillchallenge"),
        //      StringHelper.GetString("tostrong"));
        //}

    }

    private void OnYesClick()
    {
        MState = UIState.Waiting;
    }

    private void OnNoClick()
    {
        WindowManager.Instance.OpenUI("assister", 1);
    }


    public UIState MState
    {
        get { return mState; }
        set
        {
            if (value == UIState.ShowDrop)
            {
                Main.SetActive(true);
                WaitUI.gameObject.SetActive(false);
                mAwardW.setShow(false);
            }
            else
            {
                Main.SetActive(false);
                WaitUI.gameObject.SetActive(true);
                mAwardW.setShow(false);
            }
            if (mState == UIState.ShowDrop && value == UIState.Waiting)
            {
                mWaitTime = 0;
            }
            mState = value;
        }
    }
     
    private void RefreshBtn(EventBase ev = null)
    {      
        mBack.gameObject.SetActive(true);
        Continue.gameObject.SetActive(mModule.IsCanContinue());
        Sweep.gameObject.SetActive(mModule.IsSweepByFloorID(mModule.GetCurFloor()));
        BtnGrid.repositionNow = true;
        
    }

    private bool ActionScrollNum()
    {
        if (mParam == null)
            return false;

        var cti = DataManager.ChallengeTable[mParam.mFloor] as ChallengeTableItem;

        if (cti != null && mParam.mScore <= cti.mFloorScore)
        {
            return false;
        }

        if (curShowScore == -1)
        {
            if (cti != null) curShowScore = cti.mFloorScore;
        }
        int increment = (int)(mParam.mScore - cti.mFloorScore) / 20;
        curShowScore += increment;
        if (curShowScore > mParam.mScore)
        {
            curShowScore = mParam.mScore;
            CurScore.text = curShowScore.ToString(CultureInfo.InvariantCulture);
            curShowScore = -1;
            return false;
        }
        CurScore.text = curShowScore.ToString(CultureInfo.InvariantCulture);
        return true;
    }

    /// <summary>
    /// 滚动数字并且设置图片颜色
    /// </summary>
    /// <returns></returns>
    private bool ActionAchieve()
    {
        if (mParam == null)
            return false;

        bool re = false;
        if (mParam.mAchieveOne)
        {
            if (!AchieveOne.ScrollNum())
            {
                UIAtlasHelper.SetSpriteGrey(AchieveOne.mIcon, false);
                AchieveOne.PlayTween();
            }
            else
            {
                re = true;
            }
        }

        if (mParam.mAchieveTwo)
        {
            if (!AchieveTwo.ScrollNum())
            {
                UIAtlasHelper.SetSpriteGrey(AchieveTwo.mIcon, false);
                AchieveTwo.PlayTween();
            }
            else
            {
                re = true;
            }
        }

        if (!mParam.mAchieveThree) return re;
        if (!AchieveThree.ScrollNum())
        {
            UIAtlasHelper.SetSpriteGrey(AchieveThree.mIcon, false);
            AchieveThree.PlayTween();
        }
        else
        {
            re = true;
        }
        return re;
    }
        
    // Update is called once per frame
    public override void Update(uint elapsed)
    {
        if (mParam == null)
            return;

        if (MState == UIState.Waiting)
        {
			mWaitTime += Time.deltaTime;
            if (!(mWaitTime >= mMaxWaitTime)) return;
            WindowManager.Instance.CloseUI("challengeDrop");
            mModule.ContinueChallenge();
        }
        else
        {
			mTimer += Time.deltaTime;
            switch (mCurStep)
            {
                case UIStep.STEP_ORIGINAL:
                {
                   
                    mBack.gameObject.SetActive(false);
                    Continue.gameObject.SetActive(false);
                    Sweep.gameObject.SetActive(false);
                    UIAtlasHelper.SetSpriteGrey(AchieveOne.mIcon, true);
                    UIAtlasHelper.SetSpriteGrey(AchieveTwo.mIcon, true);
                    UIAtlasHelper.SetSpriteGrey(AchieveThree.mIcon, true);
                    AchieveOne.Reset();
                    AchieveTwo.Reset();
                    AchieveThree.Reset();                  
                    mTimer = 0.0f;
                    mCurStep = UIStep.STEP_Wait;
                    mTween.resetOnPlay = true;
                    MContainer.SetActive(false);
                    Jilu.gameObject.SetActive(false);
                }
                    break;

                case UIStep.STEP_Wait:                            
                {
                    if (mTimer < STEPTIME_Wait) return;
                    MContainer.SetActive(true);
                    mTween.Play(true);
                    mTimer = 0;
                    mCurStep = UIStep.STEP_0;
                }
                    break;

                case UIStep.STEP_0:
                {
                    if (!(mTimer > STEPTIME_0)) return;
                    mTimer = 0.0f;
                    mCurStep = UIStep.STEP_AchieveAnim;
                }
                    break;

                case UIStep.STEP_AchieveAnim:
                {
                    if (ActionAchieve()) return;
                    mTimer = 0.0f;
                    mCurStep = UIStep.STEP_1;
                }
                    break;

                case UIStep.STEP_1:
                {
                    if (ActionScrollNum()) return;
                    mTimer = 0.0f;
                    mCurStep = UIStep.STEP_2;
                }
                    break;

                case UIStep.STEP_2:
                {
                    if (mParam.mScore > mParam.mHistortyScore)
                    {
                        Jilu.gameObject.SetActive(true);
                        Jilu.Reset();                    
                    }

                    RefreshBtn();
                    if (mBackTween.gameObject.activeSelf)
                    {
                        mBackTween.Play(true);
                    }

                    if (mContinueTween.gameObject.activeSelf)
                    {
                       mContinueTween.Play(true);
                    }

                    if (mSweepTween.gameObject.activeSelf)
                    {
                       mSweepTween.Play(true);
                    }
                                   
                    mTimer = 0.0f;
                    mCurStep = UIStep.STEP_3;
                }
                    break;

                case UIStep.STEP_3:
                {
                    if (mParam.mDrops.Count > 0)
                    {
                        if (!(mTimer > STEPTIME_3)) return;
                        mTimer = 0.0f;
                        mCurStep = UIStep.STEP_4;
                    }
                    else
                    {
                        mTimer = 0.0f;
                        mCurStep = UIStep.STEP_5;
                    }                   
                }
                    break;

                case UIStep.STEP_4:
                {
                    mAwardW.setShow(true);
                    mAwardW.PlayTween();
                    mTimer = 0.0f;
                    mCurStep = UIStep.STEP_5;
                }
                    break;

                case UIStep.STEP_5:
                break;
            }
        }

    }

    
}
