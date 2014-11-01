using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UICharge : UIWindow
{
    private ChargeItemNode mChargeItemNode = null;
    public UIScrollBar mScrollBar;
    private UIGrid mGrid;
    private UIButton mStartBtn;
    private UILabel mStartBtnLab;
    private UISprite mChargebk;
    private UISprite mIcon;
    private bool chooseMode;//true 是充值 false是vip
    protected override void OnLoad()
    {
        base.OnLoad();
        mScrollBar = this.FindComponent<UIScrollBar>("mScrollBar");
        mGrid = this.FindComponent<UIGrid>("ScrollView/UIGrid");
        mStartBtn = this.FindComponent<UIButton>("Background/mStartBtn");
        mStartBtnLab = this.FindComponent<UILabel>("Background/mStartBtn/StartLabel");
        mChargebk = this.FindComponent<UISprite>("chargebk");
        mIcon = this.FindComponent<UISprite>("Background/TitleIcon");
        mScrollBar.gameObject.SetActive(true);
        mScrollBar.foregroundWidget.gameObject.SetActive(false);
        mScrollBar.backgroundWidget.gameObject.SetActive(false);
    }

    protected override void OnOpen(object param = null)
    {
        base.OnOpen();
        chooseMode = true;
        EventDelegate.Add(mStartBtn.onClick, OnStartBtnClick);
        if(chooseMode == true)
        {
            OnCreateChargeItem();
            mStartBtnLab.text = "VIP";
            UIAtlasHelper.SetSpriteImage(mIcon,"chongzhi:chongzhi_10" , true);
            NGUITools.SetActive(mChargebk.gameObject, true);
        }
    }
    private void OnStartBtnClick()
    {
        if (chooseMode == true)
        {
            chooseMode = false;
            mStartBtnLab.text = "充  值";
            UIAtlasHelper.SetSpriteImage(mIcon, "chongzhi:chongzhi_11", true);
            ObjectCommon.DestoryChildren(mGrid.gameObject);
            NGUITools.SetActive(mChargebk.gameObject, false);
        }
        else
        {
            chooseMode = true;
            mStartBtnLab.text = "VIP";
            UIAtlasHelper.SetSpriteImage(mIcon, "chongzhi:chongzhi_10", true);
            NGUITools.SetActive(mChargebk.gameObject, true);
            OnCreateChargeItem();
        }
    }
    protected override void OnClose()
    {
        base.OnClose();
        EventDelegate.Remove(mStartBtn.onClick, OnStartBtnClick);
        ObjectCommon.DestoryChildren(mGrid.gameObject);
    }

    void updateForm()
    {

    }
    private bool OnCreateChargeItem()
    {
        int maxnum = DataManager.ChargeRewardsTable.Count;
        for (int id = 0; id < maxnum; id++)
        {
            ChargeRewardsTableItem item = DataManager.ChargeRewardsTable[id] as ChargeRewardsTableItem;
            if (item != null)
            {
                mChargeItemNode = ChargeItemManager.Instance.CreateChargeItem(item);
            }
            else
            {
                return false;
            }
        }
        mScrollBar.value = 0.0f;
        mGrid.Reposition();
        mGrid.repositionNow = true;
        return true;
    }

}