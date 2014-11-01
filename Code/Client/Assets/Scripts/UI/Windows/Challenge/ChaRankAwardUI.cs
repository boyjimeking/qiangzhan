
using UnityEngine;


public class ChaRankAwardUI
{
    public GameObject mGameObject;
    public UILabel mRankLabel;
    public UISprite mRankIcon;
    public UILabel mTip1;
    public UILabel mTip2;
    public UIGrid mAwardGuid;
    public GameObject Background;
    public ChaRankAwardUI(GameObject obj)
    {
        mGameObject = obj;
        mRankLabel = ObjectCommon.GetChildComponent<UILabel>(mGameObject, "RankLabel");
        mRankIcon = ObjectCommon.GetChildComponent<UISprite>(mGameObject, "RankIcon");
        mTip1 = ObjectCommon.GetChildComponent<UILabel>(mGameObject, "Tip1");
        mTip2 = ObjectCommon.GetChildComponent<UILabel>(mGameObject, "TipTxt2");
        mAwardGuid = ObjectCommon.GetChildComponent<UIGrid>(mGameObject, "AwardGrid");
        Background = ObjectCommon.GetChild(mGameObject, "BackGround");
       
    }

    private void OnCloseUI(GameObject obj)
    {
        UIEventListener.Get(Background).onClick = null;
        mGameObject.SetActive(false);
    }

    public void OpenUI(uint ranknum, int resId)
    {
        mGameObject.SetActive(true);
        UIEventListener.Get(Background).onClick = OnCloseUI;
        if (ranknum > 3)
        {
            mRankLabel.gameObject.SetActive(true);
            mRankIcon.gameObject.SetActive(false);
            mRankLabel.text = ranknum.ToString();
        }
        else
        {
            mRankLabel.gameObject.SetActive(false);
            mRankIcon.gameObject.SetActive(true);
            UIAtlasHelper.SetSpriteImage(mRankIcon,"common:top"+ ranknum);
        }

        mTip1.text = StringHelper.GetString("cha_rank_tip1");
        mTip2.text = StringHelper.GetString("cha_rank_tip2");

        ObjectCommon.DestoryChildren(mAwardGuid.gameObject);
        var res = DataManager.ChaRankAwardItemTable[resId] as ChaRankAwardItemTableItem;
        if (res == null) return;

        for (int i = 0; i < res.awardItems.Length; i++)
        {
            if (res.awardItems[i].itemid != -1)
            {
                AwardItemUI awardItemUI = new AwardItemUI(res.awardItems[i].itemid, res.awardItems[i].itemnum);
                awardItemUI.gameObject.transform.parent = mAwardGuid.gameObject.transform;
                awardItemUI.gameObject.transform.localScale = Vector3.one;
            }
          
        }

        mAwardGuid.repositionNow = true;
    }    
}

