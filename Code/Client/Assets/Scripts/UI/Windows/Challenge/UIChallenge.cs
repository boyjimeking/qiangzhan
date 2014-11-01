using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Collections;


public class UIChallenge : UIWindow
{

    private UIButton mButtonReadMe;      //玩法说明
    private UIButton mButtonLeftBtn;     //左翻页
    private UIButton mButtonRightBtn;    //右翻页

    //我的战斗力
    private UILabel mLabelMyFight;
    private UILabel mLabelMyFightTxt;

    //推荐战斗力
    private UILabel mLabelRecomFight;

    private UILabel mLabelScrib;
    private UILabel mLabelState;     // 当前挑战状态
    private UIButton mButtonOk;      // 开始挑战
    private UIButton mButtonSweep;   // 扫荡通关
    private UIButton mButtonBack;    // 返回到当前层
    private UIButton mButtonDoAgain; // 再次挑战
    private UIScrollView mScrolV;
    private UIGrid mGridDrop;        // 掉落物容器
    private UIGrid mBtnGrid;
    private UIGrid mGridFloor;
    private UIButton mButtonRank;

    private UISprite mSpriteAchieveOne;
    private UISprite mSpriteAchieveTwo;
    private UISprite mSpriteAchieveThree;

    private UILabel mLabelWeekScore;

    private GameObject mBackGround;

    //首次掉落
    private GameObject mFirstDrop = null;
    private GameObject mNormalDrop = null;

    private CommonItemUI mFirstAwardItem = null;
    private CommonItemUI mNormalAwardItem = null;

    //用于实例化的源
    private GameObject mExamDropItem;
    private GameObject mExamChallengeBtn;
    private GameObject mExamRankItem;

    private ChallengeModule mChalModule;

    private int mSelectFloor = -1;
    //关卡按钮列表
    private List<ChallengeItemUI> mChallengeBtn = new List<ChallengeItemUI>();

    private UIPlayTween mRankTween;
    private UIPlayTween mMainTween;
    private TweenPosition mMainTweenPos;
    private TweenPosition mRankLeftTween;
    private TweenPosition mRankRightTween;
    private UILabel mRankNum;
    private UILabel mPlayerName;
    private UILabel mRankTimer;
    private UIButton mShowAwardBtn;
    private UIButton mGoToChalBtn;
    private UILabel mSelfScore;
    private UILabel mScoreTip;
    private UISprite mPreview;
    private UICharacterPreview mCharacterPreview;
    private UIGrid mRankGuid;
    private ChaRankAwardUI mRanAwardPanel;
    private UISprite mDizuo;
    //1 表示显示排行榜 2 显示挑战界面
    public int mShowState = 1;
    private int mRankVersion = 0;
    public List<ChaRankItemUI> mRankItemList;
    public int mRankSelectIndex = -1;
    public bool IsPreviewInit = false;
    public int ShowState
    {
        get { return mShowState; }
        set
        {
            mShowState = value;
            if (mShowState == 1)
            {
                mRankTween.gameObject.SetActive(true);
                mMainTweenPos.gameObject.transform.localPosition = new Vector3(-Screen.width * 1.5f, 0, 0);
                mRankLeftTween.gameObject.transform.localPosition = Vector3.zero;
                mRankRightTween.gameObject.transform.localPosition = Vector3.zero;               
                mChalModule.RequestRankList();
                RefreshRankPanel();
            }
            else if(mShowState == 2)
            {
                mMainTween.gameObject.SetActive(true);
                mRankLeftTween.gameObject.transform.localPosition = new Vector3(-Screen.width, 0, 0);
                mRankRightTween.gameObject.transform.localPosition = new Vector3(Screen.width, 0, 0);
                mMainTween.gameObject.transform.localPosition = Vector3.zero;
                RefreshMain();
            }
        }
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        mButtonReadMe = FindComponent<UIButton>("mainContainer/Center/ReadMe");
        mButtonLeftBtn = FindComponent<UIButton>("mainContainer/Center/PageLeft");
        mButtonRightBtn = FindComponent<UIButton>("mainContainer/Center/PageRight");
        mLabelMyFight = FindComponent<UILabel>("mainContainer/Bottom/MyFigh");
        mLabelMyFightTxt = FindComponent<UILabel>("mainContainer/Bottom/MyFightTxt");
        mLabelRecomFight = FindComponent<UILabel>("mainContainer/Bottom/RecomFight");
        mLabelScrib = FindComponent<UILabel>("mainContainer/Bottom/Scrib");
        mLabelState = FindComponent<UILabel>("mainContainer/Bottom/State/Label");
        mButtonOk = FindComponent<UIButton>("mainContainer/Bottom/BtnGrid/OkBtn");
        mButtonSweep = FindComponent<UIButton>("mainContainer/Bottom/BtnGrid/Sweep");
        mButtonBack = FindComponent<UIButton>("mainContainer/Bottom/BtnGrid/Back");
        mButtonDoAgain = FindComponent<UIButton>("mainContainer/Bottom/BtnGrid/DoAgain");
        mButtonRank = FindComponent<UIButton>("mainContainer/Bottom/Paihang");
        mScrolV = FindComponent<UIScrollView>("mainContainer/Center/Scroll View");
        mGridDrop = FindComponent<UIGrid>("mainContainer/Bottom/DropGrid");
        mBtnGrid = FindComponent<UIGrid>("mainContainer/Bottom/BtnGrid");
        mGridFloor = FindComponent<UIGrid>("mainContainer/Center/Scroll View/Grid");
        mSpriteAchieveOne = FindComponent<UISprite>("mainContainer/Bottom/AchieveGrid/Achieve1");
        mSpriteAchieveTwo = FindComponent<UISprite>("mainContainer/Bottom/AchieveGrid/Achieve2");
        mSpriteAchieveThree = FindComponent<UISprite>("mainContainer/Bottom/AchieveGrid/Achieve3");
        mLabelWeekScore = FindComponent<UILabel>("mainContainer/Bottom/WeekScoreNum");
        mFirstDrop = FindChild("mainContainer/Bottom/DropGrid/FirstDrop");
        mNormalDrop = FindChild("mainContainer/Bottom/DropGrid/SecondDrop");
        mBackGround = FindChild("BackGround");
        mExamRankItem = FindChild("ViewItem");
        mExamRankItem.SetActive(false);
        mRankTween = FindComponent<UIPlayTween>("rankContainer");
        mMainTween = FindComponent<UIPlayTween>("mainContainer");
        mRankLeftTween = FindComponent<TweenPosition>("rankContainer/left");
        mRankRightTween = FindComponent<TweenPosition>("rankContainer/right");
        mMainTweenPos = FindComponent<TweenPosition>("mainContainer");
        mMainTween.resetOnPlay = true;
        mRankTween.resetOnPlay = true;

        mRankNum = FindComponent<UILabel>("rankContainer/right/ranknum");
        mPlayerName = FindComponent<UILabel>("rankContainer/right/playername");
        mRankTimer = FindComponent<UILabel>("rankContainer/right/awardIcon/timer");
        mShowAwardBtn = FindComponent<UIButton>("rankContainer/right/awardIcon");
        mGoToChalBtn = FindComponent<UIButton>("rankContainer/right/GotoChallenge");
        mSelfScore = FindComponent<UILabel>("rankContainer/right/scorebg/Label");
        mScoreTip = FindComponent<UILabel>("rankContainer/right/tip");
        mPreview = FindComponent<UISprite>("rankContainer/right/Preview");
        mDizuo = FindComponent<UISprite>("rankContainer/right/dizuo");
        mRankGuid = FindComponent<UIGrid>("rankContainer/left/scrollView/itemguid");
        mRanAwardPanel = new ChaRankAwardUI(FindChild("RankAwardPanel"));
        mRanAwardPanel.mGameObject.SetActive(false);
        
        
        mExamChallengeBtn = FindChild("floorinfo");
        mExamChallengeBtn.gameObject.SetActive(false);
        mChalModule = ModuleManager.Instance.FindModule<ChallengeModule>();
       
        mCharacterPreview = new UICharacterPreview();
        mCharacterPreview.BackgroundSprite = mDizuo;
        mCharacterPreview.SetTargetSprite(mPreview);
        mCharacterPreview.SetCameraOrthographicSize(1.2f);
        mCharacterPreview.RotationY = 180;
        mCharacterPreview.Pos = new Vector3(0, 0.3f, 0);
        mCharacterPreview.Enable = true;
        //初始化
        InitUI();
       
    }

    protected override void OnOpen(object param = null)
    {
        EventDelegate.Add(mButtonLeftBtn.onClick, OnLeft);
        EventDelegate.Add(mButtonRightBtn.onClick, OnRight);
        EventDelegate.Add(mButtonBack.onClick, OnBack);
        EventDelegate.Add(mButtonReadMe.onClick, OnReadMe);
        EventDelegate.Add(mButtonSweep.onClick, OnSweep);
        EventDelegate.Add(mButtonOk.onClick, OnChallenge);
        EventDelegate.Add(mButtonDoAgain.onClick, OnChallenge);
        EventDelegate.Add(mButtonRank.onClick,OnRequestRank);
        EventDelegate.Add(mMainTween.onFinished,OnMainTweenFinish);
        EventDelegate.Add(mRankTween.onFinished,OnRankTweenFinish);
        UIEventListener.Get(mBackGround).onClick = OnBackgroundClick;
        EventSystem.Instance.addEventListener(ChallengeEvent.CHALLENGE_UI_UPDATE, RefreshAll);
        EventSystem.Instance.addEventListener(ChallengeEvent.CHALLENGE_RANK_UPDATE, RefreshRankPanel);
        EventDelegate.Add(mShowAwardBtn.onClick,OnShowRankAward);
        EventDelegate.Add(mGoToChalBtn.onClick,OnShowMainUI);

        //ShowState = 1;
        ShowState = param != null ? Convert.ToInt32(param) : 1;
       

    }

    //界面关闭
    protected override void OnClose()
    {
        EventDelegate.Remove(mButtonLeftBtn.onClick, OnLeft);
        EventDelegate.Remove(mButtonRightBtn.onClick, OnRight);
        EventDelegate.Remove(mButtonBack.onClick, OnBack);
        EventDelegate.Remove(mButtonReadMe.onClick, OnReadMe);
        EventDelegate.Remove(mButtonSweep.onClick, OnSweep);
        EventDelegate.Remove(mButtonOk.onClick, OnChallenge);
        EventDelegate.Remove(mButtonDoAgain.onClick, OnChallenge);
        EventDelegate.Remove(mButtonRank.onClick, OnRequestRank);
        UIEventListener.Get(mBackGround).onClick = null;
        EventSystem.Instance.removeEventListener(ChallengeEvent.CHALLENGE_UI_UPDATE, RefreshAll);
        EventSystem.Instance.removeEventListener(ChallengeEvent.CHALLENGE_RANK_UPDATE, RefreshRankPanel);
        EventDelegate.Remove(mShowAwardBtn.onClick, OnShowRankAward);
        EventDelegate.Remove(mMainTween.onFinished, OnMainTweenFinish);
        EventDelegate.Remove(mRankTween.onFinished, OnRankTweenFinish);
        EventDelegate.Remove(mGoToChalBtn.onClick, OnShowMainUI);
      
        WindowManager.Instance.CloseUI("sweepDrop");
        WindowManager.Instance.CloseUI("quickChallenge");
        mCharacterPreview.Enable = false;
    }

    private void InitUI()
    {
        mRankItemList = new List<ChaRankItemUI>();
        //关卡按钮
        for (int i = 1; i <= PlayerChallengeData.MAX_FLOOR_COUNT; i++)
        {
            var tempItem = GameObject.Instantiate(mExamChallengeBtn) as GameObject;
            tempItem.SetActive(true);
            tempItem.transform.parent = mGridFloor.gameObject.transform;
            tempItem.transform.localScale = new Vector3(1, 1, 1);

            var ci = new ChallengeItemUI(tempItem);
            ci.mFloorID = i;
            ci.mIndex = i - 1;
            ci.OnClickCallback = OnClickChallengeBtn;
            
            if (i < 10)
            {
                ci.mLabelFloorIcon.text = "0" + i;
                tempItem.name = "0" + i;
            }
            else
            {
                ci.mLabelFloorIcon.text = i.ToString();
                tempItem.name = i.ToString();
            }

            ci.mLabel.text = String.Format(StringHelper.GetString("floornum"), i); 
            mChallengeBtn.Add(ci);

        }

        mGridFloor.Reposition();
    }

    private void OnShowRankAward()
    {
        //显示奖励
        SoundManager.Instance.Play(15);
        uint rankNum = mChalModule.GetChaRankNum();
        if (rankNum > ChallengeDefine.Rank_Num)
        {
            PromptUIManager.Instance.AddNewPrompt(StringHelper.GetString("out_rank"));
            return;
        }
        mRanAwardPanel.OpenUI(rankNum, mChalModule.GetRankAwardId(mChalModule.GetWeekScore(), rankNum));

    }
    private void OnClickChallengeBtn(ChallengeItemUI go)
    {
        if (go == null)
            return;

        ChangeSelectFloor(go.mFloorID);
        mChalModule.SetDoingFloor(mSelectFloor);
    }

    private void RefreshAll(EventBase eb = null)
    {
       RefreshRankPanel();
       RefreshMain();
    }

    private void RefreshMain(EventBase eb = null)
    {
        OnRefreshBtns();
        ChangeSelectFloor(mChalModule.GetDoingFloor());
        RefreshDetailsBySelectedFloor();
    }
 
    private void OnBackgroundClick(GameObject target)
    {
        WindowManager.Instance.CloseUI("sweepDrop");
        WindowManager.Instance.CloseUI("quickChallenge");
    }

    /// <summary>
    /// 开始挑战
    /// </summary>
    private void OnChallenge()
    {
        SoundManager.Instance.Play(15);
        //int recom_grade;
        //int my_grade;
        //if (mChalModule.IsBattleGradeEnough(mSelectFloor, out recom_grade, out my_grade))
        //{
        //    OnYesClick();
        //}
        //else
        //{
        //    YesOrNoBoxManager.Instance.ShowYesOrNoUI(
        //        "",
        //        StringHelper.GetString("gradetext"),
        //        OnYesClick,
        //        OnNoClick,
        //        StringHelper.GetString("stillchallenge"),
        //        StringHelper.GetString("tostrong"));
        //}

        OnYesClick();
    }

    private void OnYesClick()
    {
        mChalModule.ChallengeFloor(mSelectFloor);
        WindowManager.Instance.CloseUI("challenge");
        if (WindowManager.Instance.IsOpen("sweepDrop"))
        {
            WindowManager.Instance.CloseUI("sweepDrop");
        }
    }

    private void OnNoClick()
    {
        WindowManager.Instance.OpenUI("assister", 1);
    }

    //打开玩法说明界面
    private void OnReadMe()
    {
        SoundManager.Instance.Play(15);
         ChallengeInfoParam info = new ChallengeInfoParam();
        info.mTitle = StringHelper.GetString("ruletitle");
        info.mRuleText = StringHelper.GetString("ruleinfo");
        WindowManager.Instance.OpenUI("challengeinfo", info);
    }

    /// <summary>
    /// 扫荡
    /// </summary>
    private void OnSweep()
    {
        SoundManager.Instance.Play(15);
        WindowManager.Instance.OpenUI("quickChallenge");
    }

    /// <summary>
    /// 返回当前层
    /// </summary>
    private void OnBack()
    {
        SoundManager.Instance.Play(15);
        int floor = mChalModule.GetCurFloor();
        if (floor < 0 || floor > PlayerChallengeData.MAX_FLOOR_COUNT)
            return;

        ChangeSelectFloor(floor);
    }

    private void OnRequestRank()
    {
        SoundManager.Instance.Play(15);
        mChalModule.RequestRankList();
        mMainTweenPos.to.Set(-Screen.width*1.5f,0,0);
        mMainTween.Play(true);
     
    }

    private void OnShowMainUI()
    {
        SoundManager.Instance.Play(15);
        mRankLeftTween.to.Set(-Screen.width,0,0);
        mRankRightTween.to.Set(Screen.width,0,0);
        mRankTween.Play(true);
    }

    private void OnMainTweenFinish()
    {       
        ShowState = 1;
    }

    private void OnRankTweenFinish()
    {
        ShowState = 2;
    }
    private void OnLeft()
    {
        mScrolV.Scroll(-1);
        SoundManager.Instance.Play(15);
    }

    private void OnRight()
    {
        mScrolV.Scroll(1);
        SoundManager.Instance.Play(15);
    }

  

    /// <summary>
    /// 刷新关卡详细信息
    /// </summary>
    /// <param name="index"></param>
    private void RefreshDetailsBySelectedFloor()
    {
        if (mSelectFloor < 1 || mSelectFloor > PlayerChallengeData.MAX_FLOOR_COUNT)
            return;

        int index = mSelectFloor - 1;
        if(index >= 0 && index < mChallengeBtn.Count)
        {
            mChallengeBtn[index].UpdateData(mChalModule.GetChallengeState(mChallengeBtn[index].mFloorID),
                mChalModule.IsGainAchievement(mChallengeBtn[index].mFloorID, 0),
                mChalModule.IsGainAchievement(mChallengeBtn[index].mFloorID, 1),
                mChalModule.IsGainAchievement(mChallengeBtn[index].mFloorID, 2));
        }

        int recom_battlescore;
        int fight;
        if (mChalModule.IsBattleGradeEnough(mSelectFloor, out recom_battlescore, out fight))
        {
            mLabelMyFight.text = string.Format(StringHelper.GetString("yellow"), fight.ToString());
            mLabelMyFightTxt.text = string.Format(StringHelper.GetString("yellow"), StringHelper.GetString("mybattlescore"));
        }
        else
        {
            mLabelMyFight.text = string.Format(StringHelper.GetString("red"), fight.ToString());
            mLabelMyFightTxt.text = string.Format(StringHelper.GetString("red"), StringHelper.GetString("mybattlescore"));
        }

        mLabelRecomFight.text = recom_battlescore.ToString();
       
        switch (mChalModule.GetChallengeState(mSelectFloor))
        {
            case ChallengeState.Passed:
                mLabelState.text = string.Format(StringHelper.GetString("getscore"), mChalModule.GetFloorScore(mSelectFloor));
                break;
            case ChallengeState.NoPass:
                mLabelState.text = StringHelper.GetString("notchallenge");
                break;
            case ChallengeState.Never:
                mLabelState.text = StringHelper.GetString("neverchallenge");
                break;
            case ChallengeState.Current:
                mLabelState.text = StringHelper.GetString("challenging");
                break;
        }

        //判断获得的成就
        //RefreshAchieveMent(mSelectFloor);
        //刷新掉落

        //首次掉落
        if(mFirstAwardItem != null)
        {
            GameObject.Destroy(mFirstAwardItem.gameObject);
            mFirstAwardItem = null;
        }

        if(mNormalAwardItem != null)
        {
            GameObject.Destroy(mNormalAwardItem.gameObject);
            mNormalAwardItem = null;
        }

        var ctt = DataManager.ChallengeTable[mSelectFloor] as ChallengeTableItem;
        if (ctt.mFirstAwardItemId >= 0)
        {
            mFirstDrop.SetActive(true);

            mFirstAwardItem = new CommonItemUI(ctt.mFirstAwardItemId);
            mFirstAwardItem.gameObject.transform.parent = mFirstDrop.transform;
            mFirstAwardItem.gameObject.transform.localScale = Vector3.one;
            mFirstAwardItem.gameObject.transform.localPosition = Vector3.zero;
        }
        else
        {
            mFirstDrop.SetActive(false);
        }
  
        //普通通关奖励
        if (ctt.mEveryDayAwardItemId >= 0)
        {
            mNormalDrop.SetActive(true);
            mNormalAwardItem = new CommonItemUI(ctt.mEveryDayAwardItemId);
            mNormalAwardItem.gameObject.transform.parent = mNormalDrop.transform;
            mNormalAwardItem.gameObject.transform.localScale = Vector3.one;
            mNormalAwardItem.gameObject.transform.localPosition = Vector3.zero;
        }
        else
        {
            mNormalDrop.SetActive(false);
        }

        mGridDrop.Reposition();

        if (mChalModule.IsCanChallenge(mSelectFloor))
        {
            //当前层
            if (mChalModule.GetCurFloor() == mSelectFloor)
            {
                mButtonOk.gameObject.SetActive(true);
                mButtonBack.gameObject.SetActive(false);
                mButtonDoAgain.gameObject.SetActive(false);
            }

            //已经通关的层
            if (mChalModule.GetCurFloor() > mSelectFloor)
            {
                mButtonOk.gameObject.SetActive(false);
                mButtonBack.gameObject.SetActive(true);
                mButtonDoAgain.gameObject.SetActive(true);
            }
          
        }
        else
        {
            //能继续挑战
            if (mChalModule.IsCanContinue())
            {

                mButtonBack.gameObject.SetActive(true);
            }
            else
            {
                mButtonBack.gameObject.SetActive(false);
            }

            mButtonOk.gameObject.SetActive(false);
            mButtonDoAgain.gameObject.SetActive(false);

        }
        //当前关卡，并且扫荡功能开启
        if (mChalModule.IsSweepByFloorID(mSelectFloor))
        {
            mButtonSweep.gameObject.SetActive(true);
        }
        else
        {
            mButtonSweep.gameObject.SetActive(false);
        }

        mBtnGrid.Reposition();

        var ct = DataManager.ChallengeTable[mSelectFloor] as ChallengeTableItem;
        mLabelScrib.text = ct.mTip;

        //本周积分
        mLabelWeekScore.text = mChalModule.GetWeekScore().ToString();

        int indexLeft = index - 2 >= 0 ? index - 2 : 0;
        int indexRight = index + 2 <= mChallengeBtn.Count - 1 ? index + 2 : mChallengeBtn.Count - 1;
        var leftSide = mChallengeBtn[indexLeft].mGameObject.transform;
        var rightSide = mChallengeBtn[indexRight].mGameObject.transform;

        mScrolV.GetComponent<UIFCenterOnChild>().CenterOn(
            mChallengeBtn[mSelectFloor - 1].mGameObject.transform,
            leftSide,
            rightSide);
    }

    //判断是否获得成就
    private void RefreshAchieveMent(int floor)
    {
       // UIAtlasHelper.SetSpriteGrey(mSpriteAchieveOne, !mChalModule.IsGainAchievement(floor, 0));
       // UIAtlasHelper.SetSpriteGrey(mSpriteAchieveTwo, !mChalModule.IsGainAchievement(floor, 1));
       // UIAtlasHelper.SetSpriteGrey(mSpriteAchieveThree, !mChalModule.IsGainAchievement(floor, 2));
         UIAtlasHelper.SetSpriteGrey(mSpriteAchieveOne, false);
         UIAtlasHelper.SetSpriteGrey(mSpriteAchieveTwo, false);
         UIAtlasHelper.SetSpriteGrey(mSpriteAchieveThree, false);
    }

    //刷新排行界面
    private void RefreshRankPanel(EventBase evt = null)
    {      
        if (mChalModule.GetChaRankNum() == UInt32.MaxValue)
        {
            mRankNum.gameObject.SetActive(false);
        }
        else
        {
            mRankNum.gameObject.SetActive(true);
            mRankNum.text = String.Format(StringHelper.GetString("diming"), (int)mChalModule.GetChaRankNum());
        }
        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();

        mPlayerName.text = "LV." + pdm.GetLevel() + " " + pdm.GetName();
        mSelfScore.text = StringHelper.GetString("currentscore") + mChalModule.GetWeekScore();
        if (mChalModule.IsInRankList(pdm.getGUID()))
        {
            if (mChalModule.GetWeekScore() == 0)
            {
                mScoreTip.gameObject.SetActive(true);
                mScoreTip.text = StringHelper.GetString("notchallenge2");
            }
            else
            {
                mScoreTip.gameObject.SetActive(false);
            }
        }
        else
        {        
            mScoreTip.gameObject.SetActive(true);
            mScoreTip.text = StringHelper.GetString("out_rank2");
        }
        if (mRankVersion != mChalModule.GetRankVersion())
        {
            for (int i = 0; i < mRankItemList.Count; ++i)
            {
                mRankItemList[i].Clear();
            }
            mRankItemList.Clear();

            ObjectCommon.DestoryChildren(mRankGuid.gameObject);

            List<RankingChallengeInfo> rankListInfo = mChalModule.GetRankList();

            for (int i = 0; i < rankListInfo.Count; ++i)
            {
                GameObject rankItemObj = GameObject.Instantiate(mExamRankItem) as GameObject;
                ChaRankItemUI rankItemUI = new ChaRankItemUI(rankItemObj);
                rankItemUI.SetShowInfo((i + 1), rankListInfo[i].resid, rankListInfo[i].level, rankListInfo[i].name,
                    rankListInfo[i].challenge_weekscore, rankListInfo[i].floor);
                rankItemUI.mButton.CustomData = i;
                rankItemUI.clickCallBack = OnClickRankItem;
                rankItemUI.mGameObject.SetActive(true);
                rankItemUI.mGameObject.transform.parent = mRankGuid.gameObject.transform;
                rankItemUI.mGameObject.transform.localScale = Vector3.one;
                mRankItemList.Add(rankItemUI);
            }

            mRankGuid.repositionNow = true;
            mRankVersion = mChalModule.GetRankVersion();
        }


    }

    public void OnClickRankItem(int index)
    {
        if (mRankSelectIndex != index)
        {
            if (mRankSelectIndex != -1)
            {
                  mRankItemList[mRankSelectIndex].mSelected.gameObject.SetActive(false);
            }

            mRankItemList[index].mSelected.gameObject.SetActive(true);
            mRankSelectIndex = index;
        }
    }
    /// <summary>
    /// 刷新关卡按钮
    /// </summary>
    private void OnRefreshBtns()
    {
        if (mChallengeBtn == null)
            return;

        for (int i = 0; i < mChallengeBtn.Count; i++)
        {
            if (i > (mChalModule.GetHistoryFloor() + 2) && i > 5)
            {
                mChallengeBtn[i].mGameObject.SetActive(false);
            }
            else
            {
                mChallengeBtn[i].mGameObject.SetActive(true);
                mChallengeBtn[i].UpdateData(mChalModule.GetChallengeState(mChallengeBtn[i].mFloorID),
                    mChalModule.IsGainAchievement(mChallengeBtn[i].mFloorID, 0),
                    mChalModule.IsGainAchievement(mChallengeBtn[i].mFloorID, 1),
                    mChalModule.IsGainAchievement(mChallengeBtn[i].mFloorID, 2)); 
            }
        }

        mGridFloor.Reposition();
    }

    private void ChangeSelectFloor(int floor)
    {
        if (floor == mSelectFloor)
            return;

        if(mSelectFloor >= 1 && mSelectFloor <= PlayerChallengeData.MAX_FLOOR_COUNT)
        {
            mChallengeBtn[mSelectFloor - 1].SetSelected(false);
        }

        mSelectFloor = floor;
        if(mSelectFloor >= 1 && mSelectFloor <= PlayerChallengeData.MAX_FLOOR_COUNT)
        {
            mChallengeBtn[mSelectFloor - 1].SetSelected(true);
        }

        RefreshDetailsBySelectedFloor();
    }

    public override void Update(uint elapsed)
    {
        
        int day_seconds = ChallengeDefine.Rank_Rest_Time - (int)(TimeUtilities.GetNow() % (24 * 60 * 60 * 1000));
        if (day_seconds < 0)
            day_seconds = 0;
        mRankTimer.text =  TimeUtilities.GetCountDownHMS(day_seconds);
        Player player = PlayerController.Instance.GetControlObj() as Player;
        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();

        if (player != null && !IsPreviewInit)
        {
            mCharacterPreview.SetupCharacter(player.ModelID, null, -1, 0);

            mCharacterPreview.ChangeWeapon(pdm.GetMainWeaponId());
            mCharacterPreview.RotationY = 180;
            IsPreviewInit = true;
        }

        if (IsPreviewInit)
        {
            mCharacterPreview.Update();
        }

    }

    protected override void OnDestroy()
    {
       
        OnClose();
        base.OnDestroy();
        mCharacterPreview.Destroy();
        mCharacterPreview = null;
    }
}
