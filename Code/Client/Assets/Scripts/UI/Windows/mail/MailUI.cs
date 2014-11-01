using UnityEngine;
using System.Collections;

public class UIMail : UIWindow
{
    public enum MailState
    {
        MAIL_STATE_DEL = 0,
        MAIL_STATE_NORMAL,
        MAIL_STATE_READ,
        MAIL_STATE_EMPTY,
    }
    private MailItemNode mMailItemNode = null;
    private UIButton mReturnBtn;
    private UIButton mDeleteBtn;
    private UIButton mPickBtn;
    private UIGrid mGrid;
    private UIPanel MailContent;
    // 滑动条
    public UIScrollBar mScrollBar;
    public UIScrollBar itemScrollBar;
    private UISprite mEmpty;
    protected override void OnLoad()
    {
        base.OnLoad();
        mEmpty = this.FindComponent<UISprite>("Empty");
        mGrid = this.FindComponent<UIGrid>("ScrollView/UIGrid");
        mReturnBtn = this.FindComponent<UIButton>("background/returnBtn");
        mDeleteBtn = this.FindComponent<UIButton>("background/mGiveUpBtn");
        mPickBtn = this.FindComponent<UIButton>("background/mGetBtn");
        mScrollBar = this.FindComponent<UIScrollBar>("mScrollBar");
        itemScrollBar = this.FindComponent<UIScrollBar>("Open/ItemGridBK/itemScrollBar");
        PlayerData data = PlayerDataPool.Instance.MainData;
        mScrollBar.gameObject.SetActive(true);
        mScrollBar.foregroundWidget.gameObject.SetActive(false);
        mScrollBar.backgroundWidget.gameObject.SetActive(false);
        itemScrollBar.gameObject.SetActive(true);
        itemScrollBar.foregroundWidget.gameObject.SetActive(false);
        itemScrollBar.backgroundWidget.gameObject.SetActive(false);
        MailContent = this.FindComponent<UIPanel>("Open");

    }
    protected override void OnClose()
    {
        base.OnClose();
        MailModule module = ModuleManager.Instance.FindModule<MailModule>();
        if (module.SelectedMail != null)
        {
            module.SelectedMail = null;
        }
        UIAtlasHelper.SetButtonImage(mDeleteBtn, "common:anniuhui", true);
        UIAtlasHelper.SetButtonImage(mPickBtn, "common:anniuhui", true);
        EventDelegate.Remove(mReturnBtn.onClick, OnReturnBtnClick);
        EventDelegate.Remove(mDeleteBtn.onClick, OnDeleteBtnClick);
        EventDelegate.Remove(mPickBtn.onClick, OnPickBtnClick);
        ObjectCommon.DestoryChildren(mGrid.gameObject);
    }
    protected override void OnOpen(object param = null)
    {
        base.OnOpen();
        EventDelegate.Add(mReturnBtn.onClick, OnReturnBtnClick);
        EventDelegate.Add(mDeleteBtn.onClick, OnDeleteBtnClick);
        EventDelegate.Add(mPickBtn.onClick, OnPickBtnClick);
        OnCreateMailItem();
        mScrollBar.value = 0.0f;
        itemScrollBar.value = 0.0f;
        NGUITools.SetActive(MailContent.gameObject, false);
        NGUITools.SetActive(mEmpty.gameObject, true);
        mGrid.Reposition();
        mGrid.repositionNow = true;
        MailModule module = ModuleManager.Instance.FindModule<MailModule>();
    }
    private void OnDeleteBtnClick()
    {
        MailModule module = ModuleManager.Instance.FindModule<MailModule>();
        if(module.SelectedMail != null)
        { 
            module.SelectedMail.OnDeleteItem();
        }
        UIAtlasHelper.SetButtonImage(mDeleteBtn, "common:anniuhui", true);
    }
    private void OnPickBtnClick()
    {
        MailModule module = ModuleManager.Instance.FindModule<MailModule>();
        if(module.SelectedMail!=null &&module.SelectedMail.mMaildata.mItemsList!=null &&module.SelectedMail.mMaildata.itemcnt!=0)
        {
            module.SelectedMail.OnPickItem();
        }
    }
    public override void Update(uint elapsed)
    {
        base.Update(elapsed);
        PlayerDataModule mPlayerDataModule = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (mPlayerDataModule.GetMailNumber() <= 0)
        {
            Close();
        }
    }
    private void OnReturnBtnClick()
    {
        Close();
    }
    private bool OnCreateMailItem()
    {
        MailModule module = ModuleManager.Instance.FindModule<MailModule>();
        if (module.mMailsList.Count > 0)
        {
            for (int id = 0; id < module.mMailsList.Count; id++)
            {
                if (module.mMailsList[id].state != 0)
                {
                   mMailItemNode = MailItemManager.Instance.CreateMailItem(module.mMailsList[id]);
                }
            }
            return true;
        }
        return false;
    }
}

