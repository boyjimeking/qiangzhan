using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class WeaponShopGridUI 
{
    public UISprite mIcon;
    public UILabel mDesc;
    public UISprite mPre;
    public UISprite mGame;
    public UISprite mDiamond;
    public UISprite mItem;
    //public UISprite mSele;
    public UISpriteAnimation mSele;
    public UISprite mBack;

    //private uint mWeaponId;

    private GameObject mObj = null;

    public delegate void OnClickFuntion(WeaponShopGridUI grid);

    public OnClickFuntion onClick = null;

    public WeaponShopGridUI(GameObject obj)
    {
        mObj = obj;

        mIcon = ObjectCommon.GetChildComponent<UISprite>(mObj,"weaponSp");
        mDesc = ObjectCommon.GetChildComponent<UILabel>(mObj, "priceNumLb");
        mPre = ObjectCommon.GetChildComponent<UISprite>(mObj, "prestigeTypeSp");
        mGame = ObjectCommon.GetChildComponent<UISprite>(mObj, "priceTypeSp");
        mSele = ObjectCommon.GetChildComponent<UISpriteAnimation>(mObj, "qiangxiexuanzhong");
        mBack = ObjectCommon.GetChildComponent<UISprite>(mObj, "bgSp1");
        mDiamond = ObjectCommon.GetChildComponent<UISprite>(mObj, "priceTypeZuanshiSp");
        mItem = ObjectCommon.GetChildComponent<UISprite>(mObj, "priceTypeItemSp");

        UIEventListener.Get(mObj).onClick = OnClick;
    }

    private void OnClick(GameObject obj)
    {
        if (onClick != null)
            onClick(this);
    }
	
    //设置图片
    public void SetIcon(string name)
    {
        UIAtlasHelper.SetSpriteImage(mIcon, name , true);
        mIcon.transform.localScale = new Vector3(0.8f, 0.8f, 0f);
    }
    //设置具体信息
    //type:0待买 显示声望， 1待买 显示金钱， 2拥有 ，3装备, 4待买 显示钻石， 5待买 显示道具数量
    //color:false红色
    public void SetInfo(int type, string value, bool color, string picname = "")
    {
        if (type == 0)
        {
            mGame.gameObject.SetActive(false);
            mPre.gameObject.SetActive(true);
            mBack.gameObject.SetActive(true);
            mDiamond.gameObject.SetActive(false);
            mItem.gameObject.SetActive(false);
        }
        else if (type == 1)
        {            
            mGame.gameObject.SetActive(true);
            mPre.gameObject.SetActive(false);
            mBack.gameObject.SetActive(true);
            mDiamond.gameObject.SetActive(false);
            mItem.gameObject.SetActive(false);
        }
        else if (type == 4)
        {
            mGame.gameObject.SetActive(false);
            mPre.gameObject.SetActive(false);
            mBack.gameObject.SetActive(true);
            mDiamond.gameObject.SetActive(true);
            mItem.gameObject.SetActive(false);
        }
        else if (type == 5)
        {
            mGame.gameObject.SetActive(false);
            mPre.gameObject.SetActive(false);
            mBack.gameObject.SetActive(true);
            mDiamond.gameObject.SetActive(false);
            mItem.gameObject.SetActive(true);

            UIAtlasHelper.SetSpriteImage(mItem, picname);
        }
        else
        {
            mGame.gameObject.SetActive(false);
            mPre.gameObject.SetActive(false);
            mBack.gameObject.SetActive(false);
            mDiamond.gameObject.SetActive(false);
            mItem.gameObject.SetActive(false);
        }

        mDesc.text = value;

        if (color)
        {
            mDesc.color = Color.white;
        }
        else
        {
            mDesc.color = Color.red;
        }
    }
    //是否被选中
    public void SetSelect(bool select)
    {
        mSele.gameObject.SetActive(select);
        mSele.Reset();
    }


}
