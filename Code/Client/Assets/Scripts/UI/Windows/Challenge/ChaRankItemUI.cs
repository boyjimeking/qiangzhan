 using System;
 using System.Reflection;
 using UnityEngine;

public delegate void  ItemClickCallBack(int index);
public class ChaRankItemUI
{
    public GameObject mGameObject;
    public UIButton mButton;
    public UILabel mPlayerName;
    public UILabel mScore;
    public UILabel mFloor;
    public UILabel mRankNum;
    public UISprite mRankIcon;
    public UISprite mHeadIcon;
    public UISprite mSelected;
    public ItemClickCallBack clickCallBack;
    public ChaRankItemUI(GameObject obj)
    {
        mGameObject = obj;
        mButton = mGameObject.GetComponent<UIButton>();
        mPlayerName = ObjectCommon.GetChildComponent<UILabel>(mGameObject, "Name");
        mScore = ObjectCommon.GetChildComponent<UILabel>(mGameObject, "score");
        mFloor = ObjectCommon.GetChildComponent<UILabel>(mGameObject, "floor");
        mRankNum = ObjectCommon.GetChildComponent<UILabel>(mGameObject, "IconLable");
        mRankIcon = ObjectCommon.GetChildComponent<UISprite>(mGameObject, "Icon");
        mHeadIcon = ObjectCommon.GetChildComponent<UISprite>(mGameObject, "roleicon");
        mSelected = ObjectCommon.GetChildComponent<UISprite>(mGameObject, "Select");
        EventDelegate.Add(mButton.onClick,OnClick);
        mSelected.gameObject.SetActive(false);
    }

    public void OnClick()
    {
        SoundManager.Instance.Play(15);
        if (clickCallBack != null)
        clickCallBack(Convert.ToInt32(mButton.CustomData));
    }

    

    public void Clear()
    {
         EventDelegate.Remove(mButton.onClick,OnClick);
         clickCallBack = null;
    }

    public void SetShowInfo(int rankNum, int resId, int level, string name, uint score, uint floor)
    {
        if (rankNum > 3)
        {
            mRankNum.gameObject.SetActive(true);
            mRankIcon.gameObject.SetActive(false);
            mRankNum.text = rankNum.ToString();
        }
        else
        {
            mRankNum.gameObject.SetActive(false);
            mRankIcon.gameObject.SetActive(true);
            UIAtlasHelper.SetSpriteImage(mRankIcon, "common:top" + rankNum);
        }

        PlayerTableItem playrRes = DataManager.PlayerTable[resId] as PlayerTableItem;
        if (playrRes != null)
        {
            UIAtlasHelper.SetSpriteImage(mHeadIcon,playrRes.face);
        }

        mPlayerName.text = "LV." + level + " " + name;

        mFloor.text = floor + StringHelper.GetString("ceng");

        mScore.text = score.ToString();


    }
}

