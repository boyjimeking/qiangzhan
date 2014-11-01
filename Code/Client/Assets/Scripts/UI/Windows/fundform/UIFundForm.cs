using FantasyEngine;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class UIFundForm : UIWindow
{
    protected UIButton MReturnButton;
    protected UIButton MChargeButton;
    protected UIGrid MGrid;
    
    //活动结束时间;
    protected UILabel MRemainTimeLabel;
    //剩余钻石数;
    protected UILabel MMoneyLabel;
    protected UILabel MDiamondLabel;
    protected UILabel MDayLabel;
    protected UIScrollBar MScrollBar;

    //protected GameObject MFundItemObj;

    private Dictionary<int, ChargeItemUI> mFundItemUis = new Dictionary<int, ChargeItemUI>(); 

    protected override void OnLoad()
    {
        base.OnLoad();

        MReturnButton = this.FindComponent<UIButton>("returnBtn");
        MChargeButton = this.FindComponent<UIButton>("ChargeBtn");
        MGrid = this.FindComponent<UIGrid>("ScrollView/UIGrid");
        MRemainTimeLabel = this.FindComponent<UILabel>("Info/time");
        MMoneyLabel = this.FindComponent<UILabel>("Info/diamond");
        MDiamondLabel = this.FindComponent<UILabel>("Info/count");
        MDayLabel = this.FindComponent<UILabel>("Info/day");
        MScrollBar = this.FindComponent<UIScrollBar>("ScrollBar");

        //MFundItemObj = this.FindChild("Item/FundItem");

        Init();
    }

    protected override void OnOpen(object param = null)
    {
        base.OnOpen(param);
        
        EventDelegate.Add(MReturnButton.onClick, onReturnBtnClick);
        EventDelegate.Add(MChargeButton.onClick, onChargeBtnClick);

        EventSystem.Instance.addEventListener(ProceedsEvent.PROCEEDS_CHANGE_TWO, onRMBChanged);

        Reset();
        updateItems();
        updateRMBInfo();
    }

    protected override void OnClose()
    {
        base.OnClose();

        EventDelegate.Remove(MReturnButton.onClick, onReturnBtnClick);
        EventDelegate.Remove(MChargeButton.onClick, onChargeBtnClick);

        EventSystem.Instance.removeEventListener(ProceedsEvent.PROCEEDS_CHANGE_TWO, onRMBChanged);
    }

    public override void Update(uint elapsed)
    {
        base.Update(elapsed);

        MRemainTimeLabel.text = StringHelper.GetString("fund_remaind_time") + FundModule.GetTimeHMSStr();
    }

    private void onReturnBtnClick()
    {
        WindowManager.Instance.CloseUI("fund");
    }

    void onChargeBtnClick()
    {
        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
        
        if(pdm == null)
            return;

        int cost = (int)ConfigManager.GetVal<int>(ConfigItemKey.FUND_CHARGE_NUM);

        if(pdm.GetProceeds(ProceedsType.Money_RMB) < cost)
        {
            string str = StringHelper.GetString("fund_no_money", FontColor.Red);
            PopTipManager.Instance.AddNewTip(str);
            return;
        }

        if(pdm.GetFundTimeSec() < 0)
        {
            string str = StringHelper.GetString("fund_over", FontColor.Red);
            PopTipManager.Instance.AddNewTip(str);
            return;
        }

        FundJoinParam param = new FundJoinParam();
        param.opType = Message.FUND_OP_TYPE.FUND_BUY;
        Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_FUND, param);
    }

    private void Init()
    {
        CreateItems();

        MDayLabel.text = ConfigManager.GetValStr(ConfigItemKey.FUND_REWARD_DAYS_NUM);
        MDiamondLabel.text = ConfigManager.GetValStr(ConfigItemKey.FUND_CHARGE_NUM);
    }

    private void Reset()
    {
        MScrollBar.value = 0f;
    }

    void updateRMBInfo()
    {
        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
        
        if (pdm != null)
        {
            MMoneyLabel.text = StringHelper.GetString("fund_money_num") + pdm.GetProceeds(ProceedsType.Money_RMB).ToString();
        }
    }

    void onRMBChanged(EventBase ev)
    {
        if (ev == null)
            return;

        ProceedsEvent pe = ev as ProceedsEvent;
        if(pe == null)
            return;

        MMoneyLabel.text = pe.value.ToString();
    }

    void updateItems(int id = -1)
    {
        if (id > 0)
        {
            updateItem(id);
        }
        else
        {
            foreach (int key in mFundItemUis.Keys)
            {
                updateItem(key);
            }
        }
    }

    void updateItem(int id)
    {
        if (!mFundItemUis.ContainsKey(id))
            return;

        FundTableItem item = FundModule.GetItemByID(id);
        if (item == null)
            return;

        ChargeItemUI ui = mFundItemUis[id];
        if (ui == null)
            return;

        ui.IsGetDone(FundModule.GetItemGetDone(id));
    }

    void CreateItems()
    {
        IDictionaryEnumerator itr = DataManager.FundTable.GetEnumerator();
        while (itr.MoveNext())
        {
            FundTableItem item = itr.Value as FundTableItem;

            if (item == null)
                continue;

            CreateItem(item);
        }
//         foreach (int key in DataManager.FundTable.Keys)
//         {
//             FundTableItem item = DataManager.FundTable[key] as FundTableItem;
// 
//             if(item == null)
//                 continue;
// 
//             CreateItem(item);
//         }

        MGrid.repositionNow = true;
    }

    private void CreateItem(FundTableItem item)
    {
        if (item == null)
            return;

        //GameObject go = WindowManager.Instance.CloneGameObject(MFundItemObj);
        //if (go == null)
        //    return;

        //go.name = item.id.ToString();
        //go.transform.parent = MGrid.transform;
        //go.transform.localScale = Vector3.one;

        ChargeItemInfo info = new ChargeItemInfo(item.itemId, item.count, item.title);

        ChargeItemUI ui = new ChargeItemUI(info);
        if (ui == null)
            return;

        ui.gameObject.name = item.id.ToString();
        ui.gameObject.transform.parent = MGrid.transform;
        ui.gameObject.transform.localScale = Vector3.one;

        //UIEventListener.Get(ui.gameObject).onClick = onFundItemClick;

        if (!mFundItemUis.ContainsKey(item.id))
        {
            mFundItemUis.Add(item.id, ui);
        }
        else
        {
            Debug.LogError("怎么会出现重复的;");
        }
    }

    //void onFundItemClick(GameObject go)
    //{
    //    int id = getFundItemId(go);
    //    if (id == -1)
    //        return;

    //    FundTableItem item = FundModule.GetItemByID(id);
    //    if (item == null)
    //        return;

    //    openItemInfoUI(item.itemId);
    //}

    //void openItemInfoUI(int itemId)
    //{
    //    ItemInfoParam param = new ItemInfoParam();
    //    param.itemid = itemId;
    //    WindowManager.Instance.OpenUI("iteminfo", param);
    //}

    int getFundItemId(GameObject go)
    {
        if (go == null)
            return -1;

        return System.Convert.ToInt32(go.name);
    }
}

///用共用的ChargeItemUI来创建;

//public class FundItemUI
//{
//    protected UILabel TitleLabel;
//    protected UILabel NameLabel;
//    protected UILabel CountLabel;
//    protected UISprite IconSprite;
//    protected UISprite GetDoneSprite;

//    private GameObject mGo;

//    public GameObject gameObject
//    {
//        get
//        {
//            return mGo;
//        }
//    }

//    public FundItemUI(GameObject go)
//    {
//        if (go == null)
//            return;

//        TitleLabel = ObjectCommon.GetChildComponent<UILabel>(go, "title");
//        NameLabel = ObjectCommon.GetChildComponent<UILabel>(go, "name");
//        CountLabel = ObjectCommon.GetChildComponent<UILabel>(go, "number");
//        IconSprite = ObjectCommon.GetChildComponent<UISprite>(go, "icon");
//        GetDoneSprite = ObjectCommon.GetChildComponent<UISprite>(go, "getdone");

//        mGo = go;
//    }

//    public void SetData(FundTableItem item)
//    {
//        if (item == null)
//            return;

//        ItemTableItem itemItem = ItemManager.GetItemRes(item.itemId);
//        if (itemItem == null)
//        {
//            Debug.LogError("基金返利中，不存在的物品");
//            return;
//        }

//        TitleLabel.text = item.title;
//        NameLabel.text = item.name;
//        CountLabel.text = item.count.ToString();
//        UIAtlasHelper.SetSpriteImage(IconSprite, itemItem.picname, true);
//    }

//    public void IsGetDone(bool getDone)
//    {
//        GetDoneSprite.gameObject.SetActive(getDone);
//    }
//}