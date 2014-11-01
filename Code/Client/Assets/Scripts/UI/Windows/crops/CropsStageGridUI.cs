using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class CropsStageGridUI 
{
    private UISprite mIcon = null;
    private UISprite mCropsjob = null;
    private List<UISprite> mStarsList = new List<UISprite>();
    private UILabel mCropsname = null;
    private UILabel mHp = null;
    private UILabel mDefence = null;
    private UILabel mCrits = null;
    private UILabel mDamage = null;
    private UILabel mEnergy = null;

    private GameObject mObj = null;
    private int mCropsId = -1;

    public delegate void OnClickFuntion(CropsStageGridUI grid);
    public OnClickFuntion onClick = null;

    public CropsStageGridUI(GameObject obj)
    {
        mObj = obj;

        mIcon = ObjectCommon.GetChildComponent<UISprite>(mObj, "cropspic");
        mCropsjob = ObjectCommon.GetChildComponent<UISprite>(mObj, "cropsjob");
        for (int i = 0; i < 5; ++i)
        {
            mStarsList.Add(ObjectCommon.GetChildComponent<UISprite>(mObj, "stars" + (i + 1)));
        }
        mCropsname = ObjectCommon.GetChildComponent<UILabel>(mObj, "cropsname");
        mHp = ObjectCommon.GetChildComponent<UILabel>(mObj, "ProPanel/life/Label");
        mDefence = ObjectCommon.GetChildComponent<UILabel>(mObj, "ProPanel/defence/Label");
        mCrits = ObjectCommon.GetChildComponent<UILabel>(mObj, "ProPanel/crits/Label");
        mDamage = ObjectCommon.GetChildComponent<UILabel>(mObj, "ProPanel/damage/Label");
        mEnergy = ObjectCommon.GetChildComponent<UILabel>(mObj, "ProPanel/energy/Label");

        UIEventListener.Get(mObj).onClick = OnClick;
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

    public void SetStarslv(int starslv)
    {
        for (int i = 0; i < starslv; ++i)
        {
            UIAtlasHelper.SetSpriteImage(mStarsList[i], "common:strenth (11)");
        }
        for (int i = starslv; i < 5; ++i)
        {
            UIAtlasHelper.SetSpriteImage(mStarsList[i], "common:starslvback");
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

    public void SetProperty(float hp, float damage, float crits, float defence, float energy)
    {
        mHp.text = hp.ToString();
        mDamage.text = damage.ToString();
        mCrits.text = crits.ToString();
        mDefence.text = defence.ToString();
        mEnergy.text = energy.ToString();
    }
}
