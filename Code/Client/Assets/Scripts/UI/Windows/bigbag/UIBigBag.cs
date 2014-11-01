using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class BigItemUnit
{
    public GameObject mObj;
    public UISprite mDate;
    public UIButton mClickBtn;
    public UIGrid mItemsGrid = null;
    public UIScrollView mScrollView = null;
    public UILabel mlabel = null;
    public UISprite mTitle = null;
    public ArrayList mItemList = new ArrayList();
    public BigBagModle.BUTTON_STATE mStage;

    public BigItemUnit(GameObject gameObj)
    {
        mObj = gameObj;
        mDate = ObjectCommon.GetChildComponent<UISprite>(mObj, "Sprite/date");
        mItemsGrid = ObjectCommon.GetChildComponent<UIGrid>(mObj, "ItemsScrollView/UIGrid");
        mScrollView = ObjectCommon.GetChildComponent<UIScrollView>(mObj, "ItemsScrollView");
        mClickBtn = ObjectCommon.GetChildComponent<UIButton>(mObj, "Button");
        mlabel = ObjectCommon.GetChildComponent<UILabel>(mObj, "Label");
        mTitle = ObjectCommon.GetChildComponent<UISprite>(mObj, "Title");
    }

    public void updateitem(BigBagModle Pdm, BigBagModle.BUTTON_STATE stage)
    {
        mlabel.text = Pdm.getText(stage, mClickBtn);
        mStage = stage;
        if (stage == BigBagModle.BUTTON_STATE.Have_yes || stage == BigBagModle.BUTTON_STATE.Replenish_yes)
        {
            updateAllItemStage(true, false);
            mlabel.text = "";
            mTitle.gameObject.SetActive(true);
        }
        else
        {
            updateAllItemStage(false, true);
            mTitle.gameObject.SetActive(false);
        }
        UISprite gold = mObj.GetComponent<UISprite>();
        if(gold)
            gold.depth = BigBagModle.MAX_DATA_NUM - int.Parse(mClickBtn.gameObject.name) - 1;
    }

    public void updateAllItemStage(bool image = false,bool btn = false)
    {
        foreach (AwardItemUI item in mItemList)
        {
            item.setShowImage(image);
            mClickBtn.gameObject.SetActive(btn);
        }
    }
    public void UpdateAwardItem(ArrayList list)
    {
        foreach (DropBoxItem item in list)
        {
            AwardItemUI awardItemUI = new AwardItemUI(item.itemid, item.itemnum);
            awardItemUI.gameObject.transform.parent =mItemsGrid.gameObject.transform;
            awardItemUI.gameObject.transform.localScale = new Vector3(0.7f,0.7f,0.7f);
            awardItemUI.ItemUI.SetBoxSize(130.0f, 130.0f);
            if (!mItemList.Contains(awardItemUI))
            {
                mItemList.Add(awardItemUI);
            }

            awardItemUI.setShowText(false);
        }

        mItemsGrid.repositionNow = true;
    }
}
 

public class UIBigBag: UIWindow
{
    private UIGrid mGrid;
    private UIButton mClose;
    private BigBagModle Module = null;
    PlayerData data = null;
    private GameObject mBigItemUnit;
    private GameObject CurGameObj = null;
    private UIScrollView mScroll;
    BigBagModle Pdm
    {
        get
        {
            if (Module == null)
                Module = ModuleManager.Instance.FindModule<BigBagModle>();

            return Module;
        }
    }

    PlayerData Pdata
    {
        get
        {
            if (data == null)
                data = PlayerDataPool.Instance.MainData;

            return data;
        }
    }
    protected override void OnOpen(object param = null)
    {
        UpdateAward();
        EventDelegate.Add(mClose.onClick, OnCloseClick);
        EventSystem.Instance.addEventListener(BigBagEvent.BIGBAG_UPDATE_EVENT, onBigBagChange);
    }

    protected override void OnClose()
    {
        EventDelegate.Remove(mClose.onClick, OnCloseClick);
        EventSystem.Instance.removeEventListener(BigBagEvent.BIGBAG_UPDATE_EVENT, onBigBagChange);
    }
    private int getWeek()
    {
        DateTime now = DateTime.Now; 
        return (int)now.DayOfWeek; 
    }

    void onBigBagChange(EventBase ev)
    {
        foreach (var item in Pdata.mStateData.mStateDic)
        {
           foreach(var item_ in Pdm.mItemDic)
           {
               if (item.Key == item_.Key)
               {
                   if (item.Value != item_.Value.mStage && (item.Value == BigBagModle.BUTTON_STATE.Have_yes || item.Value == BigBagModle.BUTTON_STATE.Replenish_yes))
                        PopTipManager.Instance.AddNewTip(StringHelper.GetString("send_maill"));

                   item_.Value.updateitem(Pdm, item.Value);
               }
           }
        }
    }

    private void OnCloseClick()
    {
        WindowManager.Instance.CloseUI("bigbag");
    }
 
    private bool cheak(int week)
    {
        //有钱没;
        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (pdm == null)
            return false;

      SevenTableItem seven = DataManager.SevenTable[week] as SevenTableItem;
      if (null == seven)
          return false;

      if (pdm.GetProceeds(ProceedsType.Money_RMB) < seven.diamond)
      {
          PopTipManager.Instance.AddNewTip(StringHelper.GetString("money_not_enough", FontColor.Red));
          return false;
      }
      else
      {
          string text = String.Format(StringHelper.GetString("money_enough"), (int)seven.diamond);
         PopTipManager.Instance.AddNewTip(text);
         return true;
      }

    }
    private void OnBtnClick(GameObject gameObj)
    {
        if (BigBagModle.MIN_DATA_NUM > int.Parse(gameObj.name) || BigBagModle.MAX_DATA_NUM < int.Parse(gameObj.name))
            return;

        CurGameObj = gameObj;
        BigItemUnit big = null;
        foreach (var item in Pdm.mItemDic)
        {
            if (item.Key == int.Parse(gameObj.name))
            {
                big = item.Value;
                break;
            }
        }

        if (null == big)
            return;

        BigBagModle.BUTTON_STATE state = 0;
        foreach (var item in Pdata.mStateData.mStateDic)
        {
            if (item.Key == int.Parse(gameObj.name))
            {
                state = item.Value;
                break;
            }
        }

        switch (state)
        {
            case BigBagModle.BUTTON_STATE.Have_no:
                {
                    big.mlabel.text = "领取";
                    UIAtlasHelper.SetButtonImage(big.mClickBtn, "common:btn_blue_4word");
                    int num = getWeek();
                    if (0 == num)
                        num = BigBagModle.MAX_DATA_NUM;

                    if (num != int.Parse(gameObj.name))//不发送消息
                    {
                        PopTipManager.Instance.AddNewTip(StringHelper.GetString("time_enough", FontColor.Red));
                        return;
                    }
                       
                    seven_stateparam param = new seven_stateparam();
                    param.week = int.Parse(gameObj.name);
                    param.type = 1;
                    Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_SEVEN_AWARD_STAGE, param);
                } break;

            case BigBagModle.BUTTON_STATE.Replenish_no:
                {
                    big.mlabel.text = "补领";
                    UIAtlasHelper.SetButtonImage(big.mClickBtn, "common:btn_yellow_4word");
                    int num = getWeek();
                    if (0 == num)
                        num = BigBagModle.MAX_DATA_NUM;

                    if (num < int.Parse(gameObj.name))//不发送消息
                        return;
                    
                    refreshBtnClick();

                } break;
            case BigBagModle.BUTTON_STATE.Invalid:
                {
                    GameDebug.Log("服务器错误");
                } break;
                
            default:
               
                break;

        }
    }

    void refreshBtnClick()
    {
        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (pdm == null)
            return ;

        SevenTableItem seven = DataManager.SevenTable[int.Parse(CurGameObj.name)] as SevenTableItem;
        if (null == seven)
            return;


        string content = string.Format(StringHelper.GetString("refresh_cost"), seven.diamond);

        YesOrNoBoxManager.Instance.ShowYesOrNoUI("", content, confirmRefresh);
    }

    void confirmRefresh(object para)
    {
        if (!cheak(int.Parse(CurGameObj.name)))
            return;

        seven_stateparam param = new seven_stateparam();
        param.week = int.Parse(CurGameObj.name);
        param.type = 3;
        Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_SEVEN_AWARD_STAGE, param);
    }
    protected override void OnLoad()
    {
        mBigItemUnit = FindChild("BigItemUnit");
        mGrid = FindComponent<UIGrid>("contianer/Scroll View/UIGrid");
        mScroll = FindComponent<UIScrollView>("contianer/Scroll View");
        mClose = this.FindComponent<UIButton>("contianer/close");
    }
    
    private string showText( int week)
    {

        BigBagModle.BUTTON_STATE state = 0;
        foreach (var item in Pdata.mStateData.mStateDic)
        {
            if (item.Key == week)
            {
                state = item.Value;
                foreach(var it in Pdm.mItemDic)
                {
                    if(it.Key == week)
                         return Pdm.getText(state,it.Value.mClickBtn);
                }          
            }
        }

        return "";
      
    }

    private void UpdateAward()
    {
        foreach (var item in Pdata.mStateData.mStateDic)
        {
            if (!Pdm.mItemDic.ContainsKey(item.Key))
            {
                GameObject gameObj = WindowManager.Instance.CloneGameObject(mBigItemUnit);
                if (gameObj == null)
                {
                    GameDebug.LogError("instance BigItemUnit error");
                    return;
                }

                gameObj.SetActive(true);
                gameObj.transform.parent = mGrid.gameObject.transform;
                gameObj.transform.localScale = Vector3.one;
                gameObj.name = "bigbag_" + item.Key.ToString();
                BigItemUnit big = new BigItemUnit(gameObj);
                Pdm.mItemDic.Add(item.Key, big);

                ArrayList list = Pdm.getBigBagList(item.Key);
                if(null != list)
                    big.UpdateAwardItem(list);

                big.mlabel.text = showText(item.Key);
                big.mClickBtn.gameObject.name = item.Key.ToString();
                UIEventListener.Get(big.mClickBtn.gameObject).onClick = OnBtnClick;

                UIAtlasHelper.SetSpriteImage(big.mDate, "qirilibao:qirilibao_0" + item.Key.ToString());
            }

            if (null != Pdm.mItemDic[item.Key])
                Pdm.mItemDic[item.Key].updateitem(Pdm,item.Value);

            mGrid.repositionNow = true;
        } 
    }
}