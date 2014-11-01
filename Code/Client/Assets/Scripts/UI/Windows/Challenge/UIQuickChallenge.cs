
using System;
using UnityEngine;

public class UIQuickChallenge : UIWindow
{
    private UILabel mItemNumLabel;

    private UILabel Tip;
    private UIButton yesBtn;
    private UIButton noBtn;

    private ChallengeModule mModule;
    private CommonItemUI itemUI;
    private UILabel yestLabel;
    private int needMoney;
    // 0 为道具不足，1 为道具充足
    private int state;
    private GameObject bg;
    protected override void OnLoad()
    {
        base.OnLoad();

        mItemNumLabel = FindComponent<UILabel>("itemNum");
        Tip = FindComponent<UILabel>("Tip");
        yesBtn = FindComponent<UIButton>("yesBt");
        yestLabel = FindComponent<UILabel>("yesBt/Label");
        noBtn = FindComponent<UIButton>("noBt");
        bg = FindChild("bg");

    }

    //界面打开
    protected override void OnOpen(object param = null)
    {
        base.OnOpen();
        mModule = ModuleManager.Instance.FindModule<ChallengeModule>();
        EventDelegate.Add(yesBtn.onClick, onYesClick);
        EventDelegate.Add(noBtn.onClick,onNoClick);
        EventSystem.Instance.addEventListener(ChallengeEvent.SWEEP_ITEM_NUM, OnSweepNumChange);
        int resId = ConfigManager.GetChallengeSweepNeedItemResID();
        GameDebug.Log("消耗道具:"+resId);
        itemUI = new CommonItemUI(resId);
        itemUI.gameObject.transform.parent = bg.transform;
        itemUI.gameObject.transform.localScale = Vector3.one;
        itemUI.gameObject.transform.localPosition = new Vector3(0,65,0);
        
        Refresh();
    }

    protected override void OnClose()
    {
        base.OnClose();
        mModule = null;
        EventDelegate.Remove(yesBtn.onClick, onYesClick);
        EventDelegate.Remove(noBtn.onClick, onNoClick);
        EventSystem.Instance.removeEventListener(ChallengeEvent.SWEEP_ITEM_NUM, OnSweepNumChange);
        GameObject.Destroy(itemUI.gameObject);
        itemUI = null;
    }


    private void onYesClick()
    {
        if (state == 0)
        {
            PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
            if (pdm.GetProceeds(ProceedsType.Money_Game) < needMoney)
            {
                PromptUIManager.Instance.AddNewPrompt(StringHelper.GetString("money_game_shortage"));
                return;
            }                   
        }      
        mModule.RequestSweep();
    }

    private void onNoClick()
    {
        WindowManager.Instance.CloseUI("quickChallenge");
    }
    //界面关闭
  
    private void OnSweepNumChange(EventBase eb)
    {
        Refresh();
    }

    private void Refresh()
    {
        int itemNum = mModule.GetSweepItemNum();
        int needNum = ConfigManager.GetChallengeSweepNeedItemNum();
        if (itemNum < needNum)
        {
            ItemTableItem itemRes = ItemManager.GetItemRes(ConfigManager.GetChallengeSweepNeedItemResID());
            needMoney = itemRes.gameprice*(needNum - itemNum);
            mItemNumLabel.text = StringHelper.GetString("money_game") + "x" + needMoney;
            yestLabel.text = "购买并使用";
            state = 0;
        }
        else
        {
            mItemNumLabel.text = string.Format(StringHelper.GetString("item_num"), itemNum, mModule.GetItemName());
            yestLabel.text = "使  用";
            state = 1;
        }
        Tip.text = String.Format(StringHelper.GetString("fastFinish"), (mModule.GetHistoryFloor() + 1));
    }
    

}
