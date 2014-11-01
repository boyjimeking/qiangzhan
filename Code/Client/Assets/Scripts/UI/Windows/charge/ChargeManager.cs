using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ChargeItem
{
    private GameObject mObj;
    private UILabel mDiamond;
    private UILabel mMoney;
    private UILabel mRewardnum;
    private UISprite mIcon;
    private UIButton mItem;
    public ChargeRewardsTableItem mChargedata;

    public ChargeItem(GameObject go, ChargeRewardsTableItem chargedata)
    {
        mObj = go;
        if (go != null)
        {
            mDiamond = ObjectCommon.GetChildComponent<UILabel>(go, "Diamond");
            mMoney = ObjectCommon.GetChildComponent<UILabel>(go, "Money");
            mRewardnum = ObjectCommon.GetChildComponent<UILabel>(go, "Rewardnum");
            mIcon = ObjectCommon.GetChildComponent<UISprite>(go, "Icon");
            mItem = go.GetComponent<UIButton>();
            EventDelegate.Add(mItem.onClick, OnClickItem);
            mChargedata = chargedata;
        }
    }
    private void OnClickItem()
    {
       //打开充值界面 充值（mChargedata.Diamondnum）;
    }
    public void SetDiamond(string Diamondnum)
    {
        if (mDiamond)
        {
            mDiamond.text = Diamondnum;
        }
    }
    public void SetIcon(string icon)
    {
        if (mIcon)
        {
            UIAtlasHelper.SetSpriteImage(mIcon, icon, true);
        }
    }
    public void SetReward(string num)
    {
        if (mRewardnum)
        {
            mRewardnum.text = "另赠"+num+"钻石（限购1次）";
        }
    }
    public void SetMoney(string money)
    {
        if (mMoney)
        {
            mMoney.text = money+"元";
        }
    }
    public GameObject gameObject
    {
        get { return mObj; }
    }
}
public class ChargeItemNode
{
    public ChargeItem mUI = null;
    public ChargeItemNode()
    {

    }

    public bool Init(ChargeRewardsTableItem chargedata)
    {

        //等待新机制修改
        UIWindow ChargeUI = WindowManager.Instance.GetUI("charge");
        UIGrid mGrid = ChargeUI.FindComponent<UIGrid>("ScrollView/UIGrid");
        GameObject mClone = ChargeUI.FindChild("ChargeItem");
        GameObject clone = WindowManager.Instance.CloneGameObject(mClone);
        mUI = new ChargeItem(clone, chargedata);
        mUI.gameObject.transform.parent = mGrid.transform;
        mUI.gameObject.transform.localScale = Vector3.one;
        mGrid.Reposition();
        mGrid.repositionNow = true;
        SetDiamond(chargedata.Diamondnum.ToString());
        SetIcon(chargedata.icon);
        SetMoney(chargedata.money.ToString());
        SetReward(chargedata.rewards.ToString());

        Show();

        return true;
    }
    public bool IsHide()
    {
        return !mUI.gameObject.activeSelf;
    }

    public void Show()
    {
        if (mUI != null && IsHide() && (!NGUITools.GetActive(mUI.gameObject)))
        {
            NGUITools.SetActive(mUI.gameObject, true);
        }
    }

    public void SetDiamond(string text)
    {
        if (mUI == null)
            return;
        if (WindowManager.current2DCamera != null)
        {
            mUI.SetDiamond(text);
        }
    }
    public void SetIcon(string text)
    {
        if (mUI == null)
            return;
        if (WindowManager.current2DCamera != null)
        {
            mUI.SetIcon(text);
        }
    }
    public void SetReward(string text)
    {
        if (mUI == null)
            return;
        if (WindowManager.current2DCamera != null)
        {
            mUI.SetReward(text);
        }
    }
    public void SetMoney(string text)
    {
        if (mUI == null)
            return;
        if (WindowManager.current2DCamera != null)
        {
            mUI.SetMoney(text);
        }
    }
}

public class ChargeItemManager
{
    private static ChargeItemManager instance = null;
    public static ChargeItemManager Instance
    {
        get
        {
            return instance;
        }
    }
    public ChargeItemManager()
    {
        instance = this;
    }
    public ChargeItemNode CreateChargeItem(ChargeRewardsTableItem chargedata)
    {
        ChargeItemNode node = null;
        node = new ChargeItemNode();
        node.Init(chargedata);
        return node;
    }
    public void Update()
    {

    }
}
