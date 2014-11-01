using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UITotalCharge : UIWindow {

    protected UIButton mReturnBtn;
    protected UIButton mChargeBtn;
    protected UIButton mBoxBtn;
    protected UILabel mBoxMoney;
    protected UILabel mChargeLb;
    protected UIGrid mGrid;
    protected UIScrollBar mScrollBar;

    //protected GameObject mItemObj;
    protected GameObject mGridObj;

    private int mCurSelectObjId = -1;

    private BetterList<UIGrid> mGridItems = new BetterList<UIGrid>();

    int CurSelectObjId 
    {
        get
        {
            return mCurSelectObjId;
        }

        set
        {
            if(mCurSelectObjId != value)
            {
                mCurSelectObjId = value;

                updateBoxInfo();
            }
        }
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        mReturnBtn = this.FindComponent<UIButton>("returnBtn");
        mChargeBtn = this.FindComponent<UIButton>("ChargeBtn");
        mBoxBtn = this.FindComponent<UIButton>("box");
        mBoxMoney = this.FindComponent<UILabel>("MoneyInfo/moneyLb");
        mChargeLb = this.FindComponent<UILabel>("ChargeInfo/infoLb");
        mGrid = this.FindComponent<UIGrid>("ScrollView/UIGrid");
        mScrollBar = this.FindComponent<UIScrollBar>("ScrollBar");

        //mItemObj = this.FindChild("Item/Item");
        mGridObj = this.FindChild("Item/UIGrid");

        Init();
    }

    protected override void OnOpen(object param = null)
    {
        base.OnOpen(param);

        EventDelegate.Add(mReturnBtn.onClick, onReturnBtnClick);
        EventDelegate.Add(mChargeBtn.onClick, onChargeBtnClick);
        EventDelegate.Add(mBoxBtn.onClick, onBoxBtnClick);
        EventDelegate.Add(mScrollBar.onChange, onScrollValChanged);

        EventSystem.Instance.addEventListener(ChargeEvent.CHARGE_RMB_SUCESS, onChargeRmbSucess);

        mGrid.GetComponent<UIWidget>().enabled = false;

        updateCharge();
    }

    protected override void OnClose()
    {
        base.OnClose();

        EventDelegate.Remove(mReturnBtn.onClick, onReturnBtnClick);
        EventDelegate.Remove(mChargeBtn.onClick, onChargeBtnClick);
        EventDelegate.Remove(mBoxBtn.onClick, onBoxBtnClick);
        EventDelegate.Remove(mScrollBar.onChange, onScrollValChanged);

        EventSystem.Instance.removeEventListener(ChargeEvent.CHARGE_RMB_SUCESS, onChargeRmbSucess);
    }

    //public override void Update(uint elapsed)
    //{
    //    base.Update(elapsed);

    //    //if(mGrid != null)
    //    //Debug.LogError(mGrid.transform.localPosition);
    //}

    void updateForm()
    {

    }

    /// <summary>
    /// 礼包是否领取过，获得当前礼包需要充值钱数;
    /// </summary>
    void updateBoxInfo()
    {
        TotalChargeTableItem item = TotalChargeModule.GetItem(CurSelectObjId);

        if(item == null)
        {
            Debug.LogError("错了");
            return;
        }

        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if(pdm == null)
            return;

        if(!pdm.GetTotalChargeRewardGot(CurSelectObjId))
        {
            mBoxBtn.normalSprite = "rewardpic1";
        }
        else
        {
            mBoxBtn.normalSprite = "rewardpic2";
        }
        mBoxBtn.GetComponent<UISprite>().MakePixelPerfect();

        mBoxMoney.text = item.giftPrice.ToString();
    }

    void onChargeRmbSucess(EventBase ev)
    {
        updateCharge();
    }

    /// <summary>
    /// 还要充多少钱领什么东西;
    /// </summary>
    void updateCharge()
    {

        int max = TotalChargeModule.GetMaxRewardIdx();

        // 达到最大奖励提示;
        if (TotalChargeModule.IsLastRewardIdx(max))
        { 
            mChargeLb.text = StringHelper.GetString("totalcharge_all_got");
            return;
        }

        TotalChargeTableItem item = TotalChargeModule.GetItem(max + 1);
        if(item == null)
        {
            Debug.LogError("totalCharge 表错了！");
            return;
        }

        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if(pdm == null)
        {
            Debug.LogError("playerDataModule null 了");
            return;
        }

        int needNum = (int)(item.chargeNum - pdm.GetTotalChargeNum());
        mChargeLb.text = string.Format(StringHelper.GetString("totalcharge_detail"), needNum, item.giftPrice);
    }

    void onScrollValChanged()
    {
        int boxIdx = getCurSelectId();

        if (boxIdx < 0)
            return;

        CurSelectObjId = boxIdx;
    }

    void onReturnBtnClick()
    {
        WindowManager.Instance.CloseUI("totalcharge");
    }

    void onChargeBtnClick()
    {
        
    }

    void onBoxBtnClick()
    {
        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if(pdm == null)
            return;

        int curSelectId = getCurSelectId();
        // 领取失败，奖励已经领取过;
        if (pdm.GetTotalChargeRewardGot(curSelectId))
        {
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("totalcharge_reward_got", FontColor.Red));
            return;
        }
        
        int maxCanGetResId = TotalChargeModule.GetMaxRewardIdx();
        // 领取失败，充值额度不够;
        if(curSelectId > maxCanGetResId)
        {
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("totalcharge_reward_no", FontColor.Red));
            return;
        }

        // 请求领取奖励;
        TotalChargeRewardParam param = new TotalChargeRewardParam();

        param.opType = Message.TOTALCHARGE_OP_TYPE.TOTALCHARGE_GET_REWARD;
        param.ResId = curSelectId;

        Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_TOTALCHARGE, param);
    }

    /// <summary>
    /// 获得当前对应的累计充值表索引;
    /// </summary>
    /// <returns></returns>
    int getCurSelectId()
    {
        UICenterOnChild center = mGrid.GetComponent<UICenterOnChild>();

        if (center == null)
            return -1;

        if (center.centeredObject == null)
        {
            Debug.LogError("fuck");
            return -1;
        }

        return System.Convert.ToInt32(center.centeredObject.name);
    }

    void Init()
    {
        CreateItems();
        
        mGrid.repositionNow = true;
        //mGrid.transform.localPosition = Vector3.zero;
        mScrollBar.value = 0f;
    }

    void CreateItems()
    {
        IDictionaryEnumerator itr = DataManager.TotalChargeTable.GetEnumerator();
        while (itr.MoveNext())
        {
            TotalChargeTableItem item = TotalChargeModule.GetItem((int)itr.Key);

            if (item == null)
                continue;

            CreateItem(item);
        }
//         foreach(int key in DataManager.TotalChargeTable.Keys)
//         {
//             TotalChargeTableItem item = TotalChargeModule.GetItem(key);
// 
//             if (item == null)
//                 continue;
// 
//             CreateItem(item);
//         }
    }

    void CreateItem(TotalChargeTableItem item)
    {
        GameObject gridObj = (GameObject)GameObject.Instantiate(mGridObj);

        if (gridObj == null)
            return;

        gridObj.transform.parent = mGrid.transform;
        gridObj.transform.localScale = Vector3.one;
        gridObj.name = item.id.ToString();

        UIGrid perGrid = gridObj.GetComponent<UIGrid>();
        mGridItems.Add(perGrid);

        for(int i = 0, j = item.ItemMaxNum; i < j; i++)
        {
            TotalChargeItemItem itemItem = item[i];
            if(itemItem.itemid < 0)
            {
                continue;
            }

            ItemTableItem tableItem = ItemManager.GetItemRes(itemItem.itemid);
            if(tableItem == null)
                continue;

            //GameObject itemObj = (GameObject)GameObject.Instantiate(mItemObj);

            //if (itemObj == null)
            //    return;

            //itemObj.transform.parent = gridObj.transform;
            //itemObj.transform.localScale = Vector3.one;
            //itemObj.name = itemItem.itemid.ToString();

            //TotalChargeItemUI ui = new TotalChargeItemUI(itemObj);
            //ui.SetData(itemItem);

            ChargeItemInfo info = new ChargeItemInfo(itemItem.itemid, itemItem.itemNum);
            ChargeItemUI ui = new ChargeItemUI(info);

            ui.gameObject.transform.parent = gridObj.transform;
            ui.gameObject.transform.localScale = Vector3.one;
            ui.gameObject.name = itemItem.itemid.ToString();
        }

        perGrid.repositionNow = true;
    }
}


public class TotalChargeItemUI
{
    protected UISprite mIcon;
    protected UILabel mCount;
    protected UILabel mName;

    private GameObject mGo;

    public TotalChargeItemUI(GameObject go)
    {
        mIcon = ObjectCommon.GetChildComponent<UISprite>(go, "icon");
        mName = ObjectCommon.GetChildComponent<UILabel>(go, "name");
        mCount = ObjectCommon.GetChildComponent<UILabel>(go, "number");

        mGo = go;
    }

    public GameObject gameObject
    {
        get
        {
            return mGo;
        }
    }

    public void SetData(TotalChargeItemItem item)
    {
        if (item == null)
        {
            setNull();
            
            return;
        }

        ItemTableItem itemitem = ItemManager.GetItemRes(item.itemid);
        if (itemitem == null)
        { 
            setNull();
            return;
        }

        UIAtlasHelper.SetSpriteImage(mIcon, itemitem.picname, true);
        mCount.text = item.itemNum.ToString();
        mName.text = ItemManager.getItemNameWithColor(item.itemid);
    }

    void setNull()
    {
        mIcon.spriteName = "";
        mCount.text = "0";
        mName.text = "";
    }
}


public class TotalChargeGroupUI
{
    protected UIGrid mGrid;
    protected UIScrollBar mScrollBar;
    protected UIScrollView mScrollView;

    private GameObject mGo;

    public TotalChargeGroupUI(GameObject go)
    {
        mGrid = ObjectCommon.GetChildComponent<UIGrid>(go, "");
        mScrollBar = ObjectCommon.GetChildComponent<UIScrollBar>(go, "");
        mScrollView = ObjectCommon.GetChildComponent<UIScrollView>(go, "");

        mGo = go;
    }

    public GameObject gameObject
    {
        get
        {
            return mGo;
        }
    }
}