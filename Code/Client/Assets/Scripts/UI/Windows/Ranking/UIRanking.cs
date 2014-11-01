using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Message;
class UIRanking : UIWindow
{

    public enum RankSelect : int

    {
        Rank_Invaild = -1,  
        Rank_Grade = 0,     //战力
        Rank_Level = 1,     //等级
        Rank_Rank = 2,      //排位
        Rank_Arena = 3,     //竞技场
    }

    private UIToggle mToggleGrade = null;
    private UIToggle mToggleLevel = null;
    private UIToggle mToggleRank = null;
    private UIToggle mToggleArena = null;

    private UILabel mToggleGradeLB = null;
    private UILabel mToggleLevelLB = null;
    private UILabel mToggleRankLB = null;
    private UILabel mToggleArenaLB = null;

    private UIButton mBackButton = null;

    private GameObject mViewItem = null;
    private UIScrollView mScrollView = null;
    private UIGrid mGrid = null;


    private RankingModule mRankingModule = null;

    private RankSelect mSelect = RankSelect.Rank_Invaild;

    //缓存所有格子数
    private ArrayList mItemList = new ArrayList();



    private UILabel mSelfLabel = null;
    private UILabel mSelfZhanli = null;
    private UILabel mSelfPt = null;

    private UISprite mSelfSprite1 = null;
    private UISprite mSelfSprite2 = null;

    public UIRanking()
    {

    }
    protected override void OnLoad()
    {
        base.OnLoad();

        mBackButton = this.FindComponent<UIButton>("BackBtn");
        mViewItem = this.FindChild("ViewItem");
        mScrollView = this.FindComponent<UIScrollView>("BackGround/ScrollView");
        mGrid = this.FindComponent<UIGrid>("BackGround/ScrollView/UIGrid");

        mToggleGrade = this.FindComponent<UIToggle>("BackGround/BntGrade");
        mToggleLevel = this.FindComponent<UIToggle>("BackGround/BntLevel");
        mToggleRank = this.FindComponent<UIToggle>("BackGround/BntRank");
        mToggleArena = this.FindComponent<UIToggle>("BackGround/BntArena");

        mToggleGradeLB = this.FindComponent<UILabel>("BackGround/BntGrade/Label");
        mToggleLevelLB = this.FindComponent<UILabel>("BackGround/BntLevel/Label");
        mToggleRankLB = this.FindComponent<UILabel>("BackGround/BntRank/Label");
        mToggleArenaLB = this.FindComponent<UILabel>("BackGround/BntArena/Label");

        mSelfLabel = this.FindComponent<UILabel>("BackGround/Self/Label");
        mSelfZhanli = this.FindComponent<UILabel>("BackGround/Self/Zhanli");
        mSelfPt = this.FindComponent<UILabel>("BackGround/Self/ArenaPt");

        mSelfSprite1 = this.FindComponent<UISprite>("BackGround/Self/Sprite1");
        mSelfSprite2 = this.FindComponent<UISprite>("BackGround/Self/Sprite2");

        EventDelegate.Add(mToggleGrade.onChange, onToggleChanged);
        EventDelegate.Add(mToggleLevel.onChange, onToggleChanged);
        EventDelegate.Add(mToggleRank.onChange, onToggleChanged);
        EventDelegate.Add(mToggleArena.onChange, onToggleChanged);

        EventDelegate.Add(mBackButton.onClick, onExitRanking);
    }

    private void onToggleChanged()
    {
        RankSelect sel = RankSelect.Rank_Invaild;
        mToggleGradeLB.applyGradient = mToggleGrade.value;
        mToggleLevelLB.applyGradient = mToggleLevel.value;
        mToggleRankLB.applyGradient = mToggleRank.value;
        mToggleArenaLB.applyGradient = mToggleArena.value;

        if (mToggleGrade.value)
        {
            sel = RankSelect.Rank_Grade;
        }
        if (mToggleLevel.value)
        {
            sel = RankSelect.Rank_Level;
        }
        if (mToggleRank.value)
        {
            sel = RankSelect.Rank_Rank;
        }
        if (mToggleArena.value)
        {
            sel = RankSelect.Rank_Arena;
        }

        if (sel == RankSelect.Rank_Invaild)
            return;
        if (mSelect != sel)
        {
            mSelect = sel;
            UpdateItems();
        }
    }
    private void onExitRanking()
    {
        WindowManager.Instance.CloseUI("ranking");
    }

    //界面打开
    protected override void OnOpen(object param = null)
    {
        WindowManager.Instance.CloseUI("city");
        WindowManager.Instance.CloseUI("joystick");

        EventSystem.Instance.addEventListener(RankingEvent.RANKING_UPDATE, OnRankingUpdate);

        mRankingModule = ModuleManager.Instance.FindModule<RankingModule>();

        mRankingModule.RequestRanking();

		SetSelectedRank(param);

        UpdateItems();
    }
    //界面关闭
    protected override void OnClose()
    {
        WindowManager.Instance.OpenUI("city");
        WindowManager.Instance.OpenUI("joystick");

        EventSystem.Instance.removeEventListener(RankingEvent.RANKING_UPDATE, OnRankingUpdate);
    }

    private void OnRankingUpdate( EventBase e )
    {
        UpdateItems();
    }

	private void SetSelectedRank(object param)
	{
		if(param == null)
		{
			mToggleGrade.value = true;
			return;
		}

		RankSelect rank = (RankSelect)param;
		switch(rank)
		{
			case RankSelect.Rank_Grade:
				mToggleGrade.value = true;
				break;
			case RankSelect.Rank_Level:
				mToggleLevel.value = true;
				break;
			case RankSelect.Rank_Rank:
				mToggleRank.value = true;
				break;
			case RankSelect.Rank_Arena:
				mToggleArena.value = true;
				break;
			default:
				mToggleGrade.value = true;
				break;
		}
	}

    private void UpdateItems()
    {
        PlayerDataModule dataModule = ModuleManager.Instance.FindModule<PlayerDataModule>();

        if( dataModule == null )
        {
            return;
        }

        for (int i = 0; i < mItemList.Count; ++i)
        {
            RankingItemUI grid = mItemList[i] as RankingItemUI;
            grid.gameObject.SetActive(false);
        }
        int idx = 0;
        RankingInfo[] dic = null;
        if (mSelect == RankSelect.Rank_Grade)
            dic = mRankingModule.GetRankingGrade();
        if (mSelect == RankSelect.Rank_Level)
            dic = mRankingModule.GetRankingLevel();
        if (mSelect == RankSelect.Rank_Rank)
            dic = mRankingModule.GetRankingRank();
        if (mSelect == RankSelect.Rank_Arena)
            dic = mRankingModule.GetRankingArena();

        if( dic == null )
        {
            return;
        }

        if( dic != null )
        {
            for (int i = 0; i < dic.Length; ++i )
            {
                UpdateItemGrid(idx++, dic[i], (dic[i].name == dataModule.GetName()));
            }
        }

        
        mGrid.repositionNow = true;
        UpdateSelfInfo();
    }

    private void UpdateSelfInfo()
    {
        Dictionary<int, RankingInfo> dic = null;
        RankingInfo rankInfo = null;
        if (mSelect == RankSelect.Rank_Grade)
        {
            rankInfo = mRankingModule.GetSelfGrade();
        }
        if (mSelect == RankSelect.Rank_Level)
        {
            rankInfo = mRankingModule.GetSelfLevel();
        }
        if (mSelect == RankSelect.Rank_Rank)
        {
            rankInfo = mRankingModule.GetSelfRank();
        }
        if (mSelect == RankSelect.Rank_Arena)
        {
            rankInfo = mRankingModule.GetSelfArena();
        }

        if (rankInfo == null)
        {
            //NGUITools.SetActive(mSelfLabel.gameObject, false);
            NGUITools.SetActive(mSelfZhanli.gameObject, false);
            NGUITools.SetActive(mSelfPt.gameObject, false);
            NGUITools.SetActive(mSelfSprite1.gameObject, false);
            NGUITools.SetActive(mSelfSprite2.gameObject, false);
            mSelfLabel.text = "您未上榜";
        }else
        {//
            NGUITools.SetActive(mSelfLabel.gameObject, true);
            NGUITools.SetActive(mSelfZhanli.gameObject, true);
            NGUITools.SetActive(mSelfPt.gameObject, true);
            NGUITools.SetActive(mSelfSprite1.gameObject, true);
            NGUITools.SetActive(mSelfSprite2.gameObject, true);

            mSelfLabel.text = "您的排名:" + (rankInfo.rank + 1).ToString() + "    Lv." + rankInfo.level.ToString() + " " + rankInfo.name;
            mSelfZhanli.text = rankInfo.grade.ToString();
            mSelfPt.text = rankInfo.score.ToString();
        }
    }

    private void UpdateItemGrid(int idx , RankingInfo item , bool self = false)
    {
        if( idx >= mItemList.Count )
        {
            GameObject obj = WindowManager.Instance.CloneGameObject(mViewItem);
            RankingItemUI itemui = new RankingItemUI(obj);
            itemui.gameObject.transform.parent = mGrid.transform;
            itemui.gameObject.transform.localScale = Vector3.one;
            itemui.gameObject.SetActive(false);

            mItemList.Add(itemui);
        }

        RankingItemUI grid = mItemList[idx] as RankingItemUI;

        grid.SetRankingInfo(item, idx, self );
        grid.ArenaPointSp.gameObject.SetActive((mSelect == RankSelect.Rank_Arena));
        grid.mArenaPoint.gameObject.SetActive((mSelect == RankSelect.Rank_Arena));
        grid.functionCall = OnItemClick;

        grid.gameObject.SetActive(true);
    }

    private void OnItemClick(GUID guid)
    {
        ViewOtherActionParam param = new ViewOtherActionParam();
        param.charGuid = guid;
        Net.Instance.DoAction((int)MESSAGE_ID.ID_MSG_REQUEST_OTHER, param);
    }

    public override void Update(uint elapsed)
    {
        
    }
}

