using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class CropsItemGridUI 
{
    private UISprite mIcon = null;
    private UISprite mSelected = null;
    private List<UISprite> mStarsList = new List<UISprite>();
    private UILabel mCropsname = null;
    private UISprite mOnclick = null;

    private GameObject mObj = null;
    private GameObject mState = null;
    private int mCropsId = -1;

    public delegate void OnClickFuntion(CropsItemGridUI grid);
    public OnClickFuntion onClick = null;

    public CropsItemGridUI(GameObject obj)
    {
        mObj = obj;

        mIcon = ObjectCommon.GetChildComponent<UISprite>(mObj, "pic");
        mSelected = ObjectCommon.GetChildComponent<UISprite>(mObj, "selected");
        for (int i = 0; i < 5; ++i)
        {
            mStarsList.Add(ObjectCommon.GetChildComponent<UISprite>(mObj, "stars" + (i + 1)));
        }
        mCropsname = ObjectCommon.GetChildComponent<UILabel>(mObj, "cropsname");
        mState = ObjectCommon.GetChild(mObj, "state");
        mOnclick = ObjectCommon.GetChildComponent<UISprite>(mObj, "cropslock");

        UIEventListener.Get(mObj).onClick = OnClick;
    }

    private void OnClick(GameObject obj)
    {
        if (onClick != null)
            onClick(this);
    }
	
    //设置图片
    public void SetIcon(string param)
    {
        UIAtlasHelper.SetSpriteImage(mIcon, param);
    }

    //设置佣兵名称
    public void SetName(string name)
    {
        mCropsname.text = name;
    }

    //设置佣兵出战状态
    public void SetState(bool state)
    {
        mState.SetActive(state);
    }

    //是否被选中
    public void SetSelect(bool select)
    {
        mSelected.gameObject.SetActive(select);
    }

    public void SetStarslv(int starslv)
    {
        for (int i = 0; i < starslv; ++i)
        {
            UIAtlasHelper.SetSpriteImage(mStarsList[i], "common:strenth (11)");
        }
    }

    public void SetCropsId(int resid)
    {
        mCropsId = resid;
    }

    public int GetCropsId()
    {
        return mCropsId;
    }

    //解锁佣兵
    public void SetUnlockCrops()
    {
        mOnclick.gameObject.SetActive(false);
    }
}
