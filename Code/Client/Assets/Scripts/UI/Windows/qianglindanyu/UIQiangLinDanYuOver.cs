using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIQiangLinDanYuOverParam
{
    public int score;
    public int maxScore;

    public List<int> resid = new List<int>();
    public List<int> num   = new List<int>();
};

public class UIQiangLinDanYuOver : UIWindow
{
    private UILabel mScore;
    private UILabel mMaxScore;
    private UIButton mOkBtn;
    private UIGrid mGrid;
    private UIScrollView mScrollView;
    private UISprite mJilu;
    public UIScrollBar mScrollBar;

    private UIQiangLinDanYuOverParam mParam = null;

    private List<AwardItemUI> mItemList = new List<AwardItemUI>();
    private ulong OpenTime = 0;


    public UIQiangLinDanYuOver()
    {
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        mScore = FindComponent<UILabel>("Main/Content/CurScore");
        mMaxScore = FindComponent<UILabel>("Main/Content/ScoreBG/MaxScore");
        mOkBtn = FindComponent<UIButton>("Main/Content/Ok");
        mGrid = FindComponent<UIGrid>("Main/ScrollBG/Scroll View/UIGrid");
        mScrollView = FindComponent<UIScrollView>("Main/ScrollPanel/Scroll View");
        mJilu = FindComponent<UISprite>("Main/Content/jilu");
        mScrollBar = this.FindComponent<UIScrollBar>("Main/mScrollBar");
        mScrollBar.gameObject.SetActive(true);
        mScrollBar.foregroundWidget.gameObject.SetActive(false);
        mScrollBar.backgroundWidget.gameObject.SetActive(false);
    }
 
    protected override void OnOpen(object param = null)
    {
        EventDelegate.Add(mOkBtn.onClick, OnOkClick);

        mParam = param as UIQiangLinDanYuOverParam;

        RefreshUI();
        OpenTime = TimeUtilities.GetNow();
        mScrollBar.value = 0.0f;

    }

    protected override void OnClose()
    {
        EventDelegate.Remove(mOkBtn.onClick, OnOkClick);
        OpenTime = 0;
    }

    public override void Update(uint elapsed)
    {
        if (OpenTime > 0 && TimeUtilities.GetNow() - OpenTime > (1000 * 5))
        {
            OnOkClick();
        }
    }

    private void OnOkClick()
    {
        WindowManager.Instance.CloseUI("qianglindanyuover");

        SceneManager.Instance.RequestEnterLastCity();
    }
   
    private void RefreshUI()
    {
        if (mParam == null)
            return;

        mScore.text = mParam.score.ToString();
        string maxscore = mParam.maxScore.ToString();
        mMaxScore.text = "历史最高:"+ maxscore;
        mJilu.gameObject.SetActive(mParam.score > mParam.maxScore);

        mItemList.Clear();
        ObjectCommon.DestoryChildren(mGrid.gameObject);

        for(int i = 0; i < mParam.resid.Count; i++)
        {
            AwardItemUI item = new AwardItemUI(mParam.resid[i], mParam.num[i]);
            item.gameObject.transform.parent = mGrid.gameObject.transform;
            item.gameObject.transform.localScale = Vector3.one;

            mItemList.Add(item);
        }
        mScrollBar.value = 0.0f;
        mGrid.Reposition();
    }
}
