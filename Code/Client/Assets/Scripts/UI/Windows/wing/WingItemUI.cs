using System;
using System.Collections.Generic;
using UnityEngine;

public enum WingState
{
    Invalid = -1,
    Locked,
    UnLocked,
    Wear,
}
public class WingItemUI
{
    public GameObject mView;

    private UISprite mDaChengState;
    public UISprite mWingSprite;
    private List<UISprite> mDaChengPicList;
    public bool IsTryOn = false;
    //public UISpriteAnimation mWingPicAni;
    public ParticleAnimation mWingPicAni;
    public WingItemUI(GameObject view)
    {
        mView = view;
        mWingSprite = mView.GetComponent<UISprite>();
        mDaChengState = ObjectCommon.GetChildComponent<UISprite>(view, "dachengstate");
		mDaChengState.gameObject.SetActive(false);
        mDaChengPicList= new List<UISprite>();
        for (int i = 0; i < WingDefine.MaxConditonNum; ++i)
        {
			mDaChengPicList.Add(ObjectCommon.GetChildComponent<UISprite>(view, 
				"dachengstate/dachengpic" + (i+1)+ "/aniparent/dachengAni"));
        }

       

    }

    public void Clear()
    {
        mWingSprite = null;
        mDaChengState = null;
        if (mWingPicAni != null)
        {
            mWingPicAni.Destroy();
            mWingPicAni = null;
        }
        mDaChengPicList.Clear();
        GameObject.DestroyImmediate(mView);
    }
    private WingState mState;

    public WingState State
    {
        get { return mState; }
        set
        {
            mState = value;
            switch (mState)
            {
                case WingState.Locked:
                    mDaChengState.gameObject.SetActive(true);
                    break;
                case WingState.UnLocked:
                    mDaChengState.gameObject.SetActive(false);
                    break;
                case WingState.Wear:
                    mDaChengState.gameObject.SetActive(false);
                    break;
            }
        }
    }

   
    public void SetDaChengPicVisable(int condition)
    {
		for(int i = 0;i < WingDefine.MaxConditonNum; i++)
		{		
			if((condition & (1<<i)) == 0) //没有达成条件i+1
			{
				if(mDaChengPicList[i].atlas.name == "chibangjiesuoblue")
				{
					mDaChengPicList[i].transform.localEulerAngles = new Vector3(0,0,143);
					mDaChengPicList[i].transform.localPosition = new Vector3(2.73f,1.22f,0);
				}
				UIAtlasHelper.SetSpriteImage(mDaChengPicList[i],
					"chibangjiesuored:jm_chibangjiesuo1_00000");
				UISpriteAnimation sa = mDaChengPicList[i].GetComponent<UISpriteAnimation>();
				sa.RebuildSpriteList();
				sa.Reset();

			}else
			{
				if(mDaChengPicList[i].atlas.name == "chibangjiesuored")
				{
					mDaChengPicList[i].transform.localEulerAngles = Vector3.zero;
					mDaChengPicList[i].transform.localPosition = Vector3.zero;
				}
				UIAtlasHelper.SetSpriteImage(mDaChengPicList[i],
					"chibangjiesuoblue:jm_chibangjiesuo2_00000");

				UISpriteAnimation sa = mDaChengPicList[i].GetComponent<UISpriteAnimation>();
				sa.RebuildSpriteList();
				sa.Reset();
			}
		}

    }

}



