using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIFirstCharge : UIWindow
{
    public UIScrollBar mScrollBar;
    private UIButton mReturnBtn;
    private UIButton mChargeBtn;
    private UIButton mGetRewardBtn;
    private UIButton mFirstChargeBtn;
    private UIGrid Grid;
    private List<ChargeItemUI> mItemList = new List<ChargeItemUI>();
    private bool picked;
    protected override void OnLoad()
    {
        mReturnBtn = this.FindComponent<UIButton>("background/returnBtn");
        mChargeBtn = this.FindComponent<UIButton>("background/mOnCharge");
        mGetRewardBtn = this.FindComponent<UIButton>("background/mGetReward");
        Grid = this.FindComponent<UIGrid>("background/ItemGridBK/Scroll View/UIGrid");
        mScrollBar = this.FindComponent<UIScrollBar>("background/ItemGridBK/itemScrollBar");
        picked = true;
        mScrollBar.gameObject.SetActive(true);
        mScrollBar.foregroundWidget.gameObject.SetActive(false);
        mScrollBar.backgroundWidget.gameObject.SetActive(false);
    }
    protected override void OnClose()
    {
        EventDelegate.Remove(mReturnBtn.onClick, OnReturnBtnClick);
        PlayerDataModule mPlayerDataModule = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (!mPlayerDataModule.GetCharged())
        {
            EventDelegate.Remove(mChargeBtn.onClick, OnChargeBtnClick);
        }
        else if (!mPlayerDataModule.GetFirstChargePicked())
        {
            EventDelegate.Remove(mGetRewardBtn.onClick, OnRewardBtnClick);
        }
    }
    protected override void OnOpen(object param = null)
    {
        FirstChargeRewardTableItemBase item = DataManager.FirstChargeTable[0] as FirstChargeRewardTableItemBase;

        if (item == null)
        {
            Close();
        }
        else
        {
            FirstChargeRewardTableItem Item = (FirstChargeRewardTableItem)item;
            mItemList.Clear();
            ObjectCommon.DestoryChildren(Grid.gameObject);
            for (int i = 0; i < 7; i++)
            {
                if (Item.FirstChargeItems[i] != null && Item.FirstChargeItems[i].itemid > 0 && Item.FirstChargeItems[i].itemnum > 0)
                {
                    ItemTableItem mItemRes = ItemManager.GetItemRes(Item.FirstChargeItems[i].itemid);
                    ChargeItemInfo info = new ChargeItemInfo(Item.FirstChargeItems[i].itemid, Item.FirstChargeItems[i].itemnum);
                    ChargeItemUI itemIcon = new ChargeItemUI(info);
                    itemIcon.gameObject.transform.parent = Grid.gameObject.transform;
                    itemIcon.gameObject.transform.localScale = Vector3.one;

                    mItemList.Add(itemIcon);
                }
            }
            Grid.Reposition();
            Grid.repositionNow = true;
        }
        EventDelegate.Add(mReturnBtn.onClick, OnReturnBtnClick);
        PlayerDataModule mPlayerDataModule = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (!mPlayerDataModule.GetCharged())
        {
            NGUITools.SetActive(mGetRewardBtn.gameObject, false);
            NGUITools.SetActive(mChargeBtn.gameObject,true);
            EventDelegate.Add(mChargeBtn.onClick, OnChargeBtnClick);
        }
        else if (!mPlayerDataModule.GetFirstChargePicked())
        {
            NGUITools.SetActive(mGetRewardBtn.gameObject, true);
            NGUITools.SetActive(mChargeBtn.gameObject, false);
            EventDelegate.Add(mGetRewardBtn.onClick, OnRewardBtnClick);
            picked = false;
        }
        else
        {
            NGUITools.SetActive(mChargeBtn.gameObject, false);
            UIAtlasHelper.SetButtonImage(mGetRewardBtn, "common:anniuhui", true);
        }
        mScrollBar.value = 0.0f;
    }    
    public override void Update(uint elapsed)
    {
    }
    private void OnReturnBtnClick()
    {
        Close();
    }
    private void OnChargeBtnClick()
    {
        //打开充值界面
    }
    private void OnRewardBtnClick()
    {
        if (!picked)
        {
            Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_FIRST_CHARGE_REWARD, null);
            UIAtlasHelper.SetButtonImage(mGetRewardBtn, "common:anniuhui", true);
            picked = true;
        }
    }
}

