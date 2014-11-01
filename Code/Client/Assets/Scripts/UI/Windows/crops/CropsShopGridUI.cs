using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class CropsShopGridUI 
{
    private UISprite mIcon = null;
    private UISprite mSelected = null;
    private UISprite mCropsjob = null;
    private List<UISprite> mStarsList = new List<UISprite>();
    public UISprite mOnclick = null;
    private UILabel mCropsname = null;
    private UILabel mCurHasNum = null;
    private UISlider mCurNumSp = null;
    private int mCropsId = -1;
    private int mStarslv = -1;

    private GameObject mObj = null;
    private GameObject mState = null;
    private GameObject mLock = null;

    public delegate void OnClickFuntion(CropsShopGridUI grid);

    public OnClickFuntion onClick = null;

    public CropsShopGridUI(GameObject obj)
    {
        mObj = obj;

        mIcon = ObjectCommon.GetChildComponent<UISprite>(mObj, "cropspic");
        mSelected = ObjectCommon.GetChildComponent<UISprite>(mObj, "selected");
        mCropsjob = ObjectCommon.GetChildComponent<UISprite>(mObj, "cropsjob");
        for (int i = 0; i < 5; ++i)
        {
            mStarsList.Add(ObjectCommon.GetChildComponent<UISprite>(mObj, "stars" + (i + 1)));
        }
        mOnclick = ObjectCommon.GetChildComponent<UISprite>(mObj, "onclick");
        mCropsname = ObjectCommon.GetChildComponent<UILabel>(mObj, "cropsname");
        mState = ObjectCommon.GetChild(mObj, "state");
        mLock = ObjectCommon.GetChild(mObj, "lockinfo");
        mCurHasNum = ObjectCommon.GetChildComponent<UILabel>(mObj, "lockinfo/Label");
        mCurNumSp = ObjectCommon.GetChildComponent<UISlider>(mObj, "lockinfo/itembg1");

        UIEventListener.Get(mObj).onClick = OnClick;
    }

    public void SetSortId(int id)
    {
        mObj.name = id.ToString();
    }

    private void OnClick(GameObject obj)
    {
        if (onClick != null)
            onClick(this);
    }
	
    //设置图片
    public void SetIcon(string param1, string param2)
    {
        UIAtlasHelper.SetSpriteImage(mIcon, param1);
        mIcon.MakePixelPerfect();
        UIAtlasHelper.SetSpriteImage(mCropsjob, param2);
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

        UILabel lb = ObjectCommon.GetChildComponent<UILabel>(mState, "Label");
        if (state)
        {
            PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
            if (null == module)
                return;

            if (mCropsId == module.GetMainCropsId())
            {
                lb.text = "[fed514]" + "主将出战";
            }
            else
            {
                lb.text = "副将出战";
            }
        }
    }

    //是否被选中
    public void SetSelect(bool select)
    {
        mSelected.gameObject.SetActive(select);
        mOnclick.gameObject.SetActive(select);
    }

    //兑换物品进度条显示
    public void SetProgress(int mHasNum, int mNeedNum)
    {
        mCurHasNum.text = mHasNum + "/" + mNeedNum;
        mCurNumSp.value = (float)mHasNum / (mNeedNum == 0 ? mHasNum : mNeedNum);
    }

    //解锁佣兵
    public void SetUnlockCrops()
    {
        mLock.SetActive(false);
    }

    public void SetStarslv(int starslv)
    {
        mStarslv = starslv;
        for (int i = 0; i < starslv; ++i)
        {
            UIAtlasHelper.SetSpriteImage(mStarsList[i], "common:strenth (11)");
        }
    }

    public int GetStarslv()
    {
        return mStarslv;
    }

    public void SetCropsId(int resid)
    {
        mCropsId = resid;
    }

    public int GetCropsId()
    {
        return mCropsId;
    }
}
