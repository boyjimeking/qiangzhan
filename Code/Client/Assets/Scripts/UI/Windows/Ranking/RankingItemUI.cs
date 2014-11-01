using System;
using UnityEngine;
class RankingItemUI
{
    private GameObject mObject = null;

    private RankingInfo mInfo = null;

    private UISprite mIcon = null;
    private UILabel mIconLabel = null;
    private UILabel mName = null;
    private UILabel mGrade = null;
    public UILabel mArenaPoint = null;

    public UISprite ArenaPointSp = null;

    public UISprite mSelfSprite = null;
    public UISprite mSelectBK = null;

    private UIButton mButton = null;

    public delegate void FunctionClick(GUID guid);

    public FunctionClick functionCall = null;

    public RankingItemUI(GameObject obj)
    {
        mObject = obj;

        mButton = mObject.GetComponent<UIButton>();

        mIcon = ObjectCommon.GetChildComponent<UISprite>(mObject, "Icon");
        mIconLabel = ObjectCommon.GetChildComponent<UILabel>(mObject, "IconLable");
        mName = ObjectCommon.GetChildComponent<UILabel>(mObject, "Name");
        mGrade = ObjectCommon.GetChildComponent<UILabel>(mObject, "Zhanli");
        mArenaPoint = ObjectCommon.GetChildComponent<UILabel>(mObject, "ArenaPt");

        ArenaPointSp = ObjectCommon.GetChildComponent<UISprite>(mObject, "Sprite2");

        mSelfSprite = ObjectCommon.GetChildComponent<UISprite>(mObject, "Self");
        mSelectBK = ObjectCommon.GetChildComponent<UISprite>(mObject, "Select");

        EventDelegate.Add(mButton.onClick, OnClick);
    }

    public GameObject gameObject
    {
        get
        {
            return mObject;
        }
    }

    private void OnClick()
    {
        if( functionCall != null && mInfo != null )
        {
            functionCall(mInfo.guid);
        }
    }

    public void SetRankingInfo(RankingInfo info , int top , bool self  = false)
    {
        mInfo = info;
        if (top < 3)
        {
            UIAtlasHelper.SetSpriteImage(mIcon, "common:top" + (top + 1).ToString());
            mIcon.gameObject.SetActive(true);
            mIconLabel.gameObject.SetActive(false);
            mName.color = NGUIMath.HexToColor(0xfcff00ff);
        }else
        {
            mIconLabel.text = (top + 1).ToString();
            mIcon.gameObject.SetActive(false);
            mIconLabel.gameObject.SetActive(true);
            mName.color = NGUIMath.HexToColor(0xffffffff);
        }

        mName.text = "Lv." + mInfo.level.ToString() + "  " + mInfo.name;

        mGrade.text = info.grade.ToString();
		mArenaPoint.text = info.score.ToString();

        if( self )
        {
            mSelectBK.gameObject.SetActive(true);
            mSelfSprite.gameObject.SetActive(true);
        }else
        {
            mSelectBK.gameObject.SetActive(false);
            mSelfSprite.gameObject.SetActive(false);            
        }
    }
}

