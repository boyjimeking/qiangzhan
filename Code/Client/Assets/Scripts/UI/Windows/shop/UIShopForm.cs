using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIShopForm : UIWindow
{
    #region 变量定义
    //ToggleGroup;
    UIToggle[] mToggles = new UIToggle[3];

    //神秘商店;
    UIButton sPageLeftBtn;
    UIButton sPageRightBtn;
    UIScrollBar sScrollBar;
    UIScrollView sScrollView;
    UIGrid sGrid;
    UIButton sRefreshBtn;
    UILabel sRefreshTimeLb;
    //private BetterList<ShopItemUI> mSecretItems = new BetterList<ShopItemUI>();
    private Dictionary<int, ShopItemUI> mSecretItems = new Dictionary<int, ShopItemUI>();
    //private int sCurPageNum = -1;
    private float sPageVal = 0f;

    // 积分兑换;
    UIButton jCreditBtn;
    UILabel jCreditLb;

    UIButton jPageLeftBtn;
    UIButton jPageRightBtn;
    UIButton jPageLeftBtnAni;
    UIButton jPageRightBtnAni;
    UIScrollBar jScrollBar;
    UIScrollView jScrollView;
    UIGrid jGrid;

    //private BetterList<ShopItemUI> mCreditItems = new BetterList<ShopItemUI>();
    private int mCreditCount = 0;// 积分兑换商品计数;
    //private int jCurPageNum = -1;
    private float jPageVal = 0f;

    // 装备购买;
    UIButton zPageLeftBtn;
    UIButton zPageRightBtn;
    UIButton zPageLeftBtnAni;
    UIButton zPageRightBtnAni;
    UIScrollBar zScrollBar;
    UIScrollView zScrollView;
    UIGrid zGrid;

    //private BetterList<ShopItemUI> mEquipItems = new BetterList<ShopItemUI>();
    private int mEquipCount = 0;// 装备商品计数;
    //private int zCurPageNum = -1;
    private float zPageVal = 0f;

    // 购买界面;
    GameObject gBuyObj;
    UISprite gIconSp;
    UILabel gNameLb;
    UISprite gMoneySp;
    UILabel gMoneyLb;
    UILabel gItemTypeLb;
    UILabel gItemDetailLb;
    UILabel gGetDetailLb;
    UIButton gBuyBtn;
    UIButton gCloseBtn;
    UIButton gOuterLineBtn;//外层点击;
    #endregion

    GameObject mMallItemPrefab;

    const int MAX_NUM_PER_PAGE = 8;
    const float DURATION_PAGE = 0.5f;

    private ShopModule mModule = null;

    private int mCurSelResID = -1;//商城表格ID;
    private int mCurSelSubID = -1;//每组最多6个商品，标记买第几组商品[0-5];
    private int mlastGroupResId = -1;//如果两次打开相同的GroupItem，则只需要创建一次;

    //所有界面上的MallItemUI数据;<resId , ShopItemUI>
    private Dictionary<int, ShopItemUI> mAllItemsLists = new Dictionary<int, ShopItemUI>();

    private bool mNeedInit = true;  // 只执行一次的标记;

    protected override void OnLoad()
    {
        base.OnLoad();

        mToggles[0] = this.FindComponent<UIToggle>("ToggleGroup/Toggle1");
        mToggles[1] = this.FindComponent<UIToggle>("ToggleGroup/Toggle2");
        mToggles[2] = this.FindComponent<UIToggle>("ToggleGroup/Toggle3");

        sPageLeftBtn = this.FindComponent<UIButton>("shenmi/pageLeftBtn");
        sPageRightBtn = this.FindComponent<UIButton>("shenmi/pageRightBtn");
        sScrollBar = this.FindComponent<UIScrollBar>("shenmi/ScrollBar");
        sScrollView = this.FindComponent<UIScrollView>("shenmi/ScrollView");
        sGrid = this.FindComponent<UIGrid>("shenmi/ScrollView/UIGrid");
        sRefreshBtn = this.FindComponent<UIButton>("shenmi/refreshBtn");
        sRefreshTimeLb = this.FindComponent<UILabel>("shenmi/time/Label");

        jCreditBtn = this.FindComponent<UIButton>("jifen/credit/creditSp");
        jCreditLb = this.FindComponent<UILabel>("jifen/credit/creditLb");
        jPageLeftBtn = this.FindComponent<UIButton>("jifen/pageLeftBtn");
        jPageRightBtn = this.FindComponent<UIButton>("jifen/pageRightBtn");
        jPageLeftBtnAni = this.FindComponent<UIButton>("jifen/pageLeftBtnAni");
        jPageRightBtnAni = this.FindComponent<UIButton>("jifen/pageRightBtnAni");
        jScrollBar = this.FindComponent<UIScrollBar>("jifen/ScrollBar");
        jScrollView = this.FindComponent<UIScrollView>("jifen/ScrollView");
        jGrid = this.FindComponent<UIGrid>("jifen/ScrollView/UIGrid");

        zPageLeftBtn = this.FindComponent<UIButton>("zhuangbei/pageLeftBtn");
        zPageRightBtn = this.FindComponent<UIButton>("zhuangbei/pageRightBtn");
        zPageLeftBtnAni = this.FindComponent<UIButton>("zhuangbei/pageLeftBtnAni");
        zPageRightBtnAni = this.FindComponent<UIButton>("zhuangbei/pageRightBtnAni");
        zScrollBar = this.FindComponent<UIScrollBar>("zhuangbei/ScrollBar");
        zScrollView = this.FindComponent<UIScrollView>("zhuangbei/ScrollView");
        zGrid = this.FindComponent<UIGrid>("zhuangbei/ScrollView/UIGrid");

        //mMoudle = ModuleManager.Instance.FindModule<MallFormModule>();
        gBuyObj = this.FindChild("ItemBuyForm");
        gNameLb = this.FindComponent<UILabel>("ItemBuyForm/nameLb");
        gIconSp = this.FindComponent<UISprite>("ItemBuyForm/itemIconSp");
        gItemDetailLb = this.FindComponent<UILabel>("ItemBuyForm/itemDetailLb");
        gGetDetailLb = this.FindComponent<UILabel>("ItemBuyForm/getDetailLb");
        gMoneySp = this.FindComponent<UISprite>("ItemBuyForm/moneySp");
        gMoneyLb = this.FindComponent<UILabel>("ItemBuyForm/moneyLb");
        gItemTypeLb = this.FindComponent<UILabel>("ItemBuyForm/itemTypeLb");
        gBuyBtn = this.FindComponent<UIButton>("ItemBuyForm/buyBtn");
        gCloseBtn = this.FindComponent<UIButton>("ItemBuyForm/closeBtn");
        gOuterLineBtn = this.FindComponent<UIButton>("ItemBuyForm");

        mMallItemPrefab = this.FindChild("items/MallItem");

        EventSystem.Instance.addEventListener(ProceedsEvent.PROCEEDS_CHANGE_FIVE, updateCreditBar);
    }

    float PageVal1
    {
        set
        {
            TweenScrollValue.Begin(sScrollBar.gameObject, DURATION_PAGE, value).PlayForward();
            //sScrollBar.value = value;
        }
        get
        {
            return sScrollBar.value;
        }
    }

    float PageVal2
    {
        set
        {
            TweenScrollValue.Begin(jScrollBar.gameObject, DURATION_PAGE, value).PlayForward();
            //jScrollBar.value = value;
        }
        get
        {
            return jScrollBar.value;
        }
    }

    float PageVal3
    {
        set
        {
            TweenScrollValue.Begin(zScrollBar.gameObject, DURATION_PAGE, value).PlayForward();
            //zScrollBar.value = value;
        }
        get
        {
            return zScrollBar.value;
        }
    }

    ShopModule Module
    {
        get
        {
            if (mModule == null)
                mModule = ModuleManager.Instance.FindModule<ShopModule>();

            return mModule;
        }
    }

    int CurSelectResID
    {
        get
        {
            return mCurSelResID;
        }
        set
        {
            if (value == mCurSelResID)
            {
                return;
            }


            mCurSelResID = value;
        }
    }

    //protected override void OnPreOpen(object param = null)
    //{
    //    base.OnPreOpen(param);

    //    if (param != null && System.Enum.IsDefined(typeof(ShopSubTable), param))
    //    {
    //        ShopSubTable sub = (ShopSubTable)param;

    //        SetStartToggle(sub);
    //    }
    //    else
    //    {
    //        SetStartToggle((ShopSubTable)0);
    //    }
    //}

    void SetStartToggle(ShopSubTable subTable)
    {
        int idx = (int)subTable;
        if ((idx < 0) || (idx > (mToggles.Length - 1)))
            return;

        for (int i = 0, j = mToggles.Length; i < j; i++)
        {
            mToggles[i].value = false;
        }

        mToggles[idx].value = true;
    }

    protected override void OnOpen(object param = null)
    {
        base.OnOpen(param);

        WindowManager.Instance.CloseUI("city");
        WindowManager.Instance.CloseUI("joystick");

        EventDelegate.Add(gBuyBtn.onClick, OnBuyClick);
        EventDelegate.Add(gCloseBtn.onClick, CloseBuyForm);
        EventDelegate.Add(gOuterLineBtn.onClick, CloseBuyForm);
        EventDelegate.Add(sRefreshBtn.onClick, refreshBtnClick);
        EventDelegate.Add(sPageLeftBtn.onClick, onPageLeftClick1);
        EventDelegate.Add(sPageRightBtn.onClick, onPageRightClick1);
        EventDelegate.Add(jPageLeftBtn.onClick, onPageLeftClick2);
        EventDelegate.Add(jPageRightBtn.onClick, onPageRightClick2);
        EventDelegate.Add(zPageLeftBtn.onClick, onPageLeftClick3);
        EventDelegate.Add(zPageRightBtn.onClick, onPageRightClick3);

        EventDelegate.Add(jScrollBar.onChange, upDownAniHandler1);
        EventDelegate.Add(zScrollBar.onChange, upDownAniHandler2);

        EventDelegate.Add(jCreditBtn.onClick, onCreditBtnClick);

        EventDelegate.Add(jPageLeftBtnAni.onClick, onPageLeftClick2);
        EventDelegate.Add(jPageRightBtnAni.onClick, onPageRightClick2);
        EventDelegate.Add(zPageLeftBtnAni.onClick, onPageLeftClick3);
        EventDelegate.Add(zPageRightBtnAni.onClick, onPageRightClick3);

        EventSystem.Instance.addEventListener(ShopUIEvent.SHOP_BUY_ITEM, onMallBuy);
        EventSystem.Instance.addEventListener(ShopUIEvent.SHOP_REFRESH_ITEM, onRefreshShop);

        Init();
        updateMallUI();
        Reset();
        updateNextRefreshTimeLb();

        updateCreditBar(null);

        if (param != null && System.Enum.IsDefined(typeof(ShopSubTable), param))
        {
            ShopSubTable sub = (ShopSubTable)param;

            SetStartToggle(sub);
        }
        else
        {
            SetStartToggle((ShopSubTable)0);
        }
    }

    protected override void OnClose()
    {
        base.OnClose();

        WindowManager.Instance.OpenUI("city");
        WindowManager.Instance.OpenUI("joystick");

        EventDelegate.Remove(gBuyBtn.onClick, OnBuyClick);
        EventDelegate.Remove(gCloseBtn.onClick, CloseBuyForm);
        EventDelegate.Remove(gOuterLineBtn.onClick, CloseBuyForm);
        EventDelegate.Remove(sRefreshBtn.onClick, refreshBtnClick);
        EventDelegate.Remove(sPageLeftBtn.onClick, onPageLeftClick1);
        EventDelegate.Remove(sPageRightBtn.onClick, onPageRightClick1);
        EventDelegate.Remove(jPageLeftBtn.onClick, onPageLeftClick2);
        EventDelegate.Remove(jPageRightBtn.onClick, onPageRightClick2);
        EventDelegate.Remove(zPageLeftBtn.onClick, onPageLeftClick3);
        EventDelegate.Remove(zPageRightBtn.onClick, onPageRightClick3);

        EventDelegate.Remove(jScrollBar.onChange, upDownAniHandler1);
        EventDelegate.Remove(zScrollBar.onChange, upDownAniHandler2);

        EventDelegate.Remove(jCreditBtn.onClick, onCreditBtnClick);

        EventDelegate.Remove(jPageLeftBtnAni.onClick, onPageLeftClick2);
        EventDelegate.Remove(jPageRightBtnAni.onClick, onPageRightClick2);
        EventDelegate.Remove(zPageLeftBtnAni.onClick, onPageLeftClick3);
        EventDelegate.Remove(zPageRightBtnAni.onClick, onPageRightClick3);

        EventSystem.Instance.removeEventListener(ShopUIEvent.SHOP_BUY_ITEM, onMallBuy);
        EventSystem.Instance.removeEventListener(ShopUIEvent.SHOP_REFRESH_ITEM, onRefreshShop);

        Reset();
    }

    public override void Update(uint elapsed)
    {
        base.Update(elapsed);
    }

    void Init()
    {
        if (mNeedInit)
        {
            mNeedInit = false;
            InitMallItems();

            sGrid.repositionNow = true;
            jGrid.repositionNow = true;
            zGrid.repositionNow = true;

            sScrollBar.gameObject.SetActive(true);
            sScrollBar.foregroundWidget.gameObject.SetActive(false);
            sScrollBar.backgroundWidget.gameObject.SetActive(false);

            jScrollBar.gameObject.SetActive(true);
            jScrollBar.foregroundWidget.gameObject.SetActive(false);
            jScrollBar.backgroundWidget.gameObject.SetActive(false);
        
            zScrollBar.gameObject.SetActive(true);
            zScrollBar.foregroundWidget.gameObject.SetActive(false);
            zScrollBar.backgroundWidget.gameObject.SetActive(false);
        }
    }
    // 按照时间段来提示玩家下次的刷新时间是什么时候;
    void InitRefreshTime()
    {
        sRefreshTimeLb.pivot = UIWidget.Pivot.Right;
        string text = StringHelper.GetString("shop_refresh_time");

        sRefreshTimeLb.text = string.Format(text);
    }

    void InitMallItems()
    {
        //// 神秘商店初始化;
        //BetterList<int> resIds = Module.GetPlayerSecretShopItemIds();
        
        //if (resIds != null) 
        //{
        //    foreach (int id in resIds)
        //    {
        //        ShopTableItem item = DataManager.ShopTable[id] as ShopTableItem;
        //        if (item == null)
        //            continue;

        //        CreateMallItem(item);
        //    }
        //}

        // 其他商店初始化;
        CreateMallItems(ShopModule.GetEffectShopItemsWithoutSecret());
        CreateNullItems();
    }

    void Reset()
    {
        sScrollBar.value = 0f;
        jScrollBar.value = 0f;
        zScrollBar.value = 0f;

        CloseBuyForm();
    }

    void refreshBtnClick()
    {
        string content = string.Format(StringHelper.GetString("shop_refresh_cost"), ShopModule.GetSecretShopRefreshCost());

        YesOrNoBoxManager.Instance.ShowYesOrNoUI("", content, confirmRefresh);
    }

    void confirmRefresh(object para)
    {
        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();

        if (pdm == null)
            return;

        //if (ShopModule.SECRET_REFRESH_COST > pdm.GetProceeds(ShopModule.SECRET_REFRESH_PROC_TYPE))
        ProceedsType pt = ShopModule.GetSecretShopRefreshPoceType();
        if (ShopModule.GetSecretShopRefreshCost() > pdm.GetProceeds(pt))
        {
            //PromptUIManager.Instance.AddNewPrompt(ShopModule.GetBuyMoneyNotEnougthStr(pt));
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("refresh_no_money", FontColor.Red));
            return;
        }

        ShopRefreshAction param = new ShopRefreshAction();
        param.OpType = (int)ShopOpType.Refresh;

        Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_SHOP, param);
    }

    void onRefreshShop(EventBase ev)
    {
        updateMallUI();
        updateNextRefreshTimeLb();
    }
    void onCreditBtnClick()
    {
 
    }

    void updateCreditBar(EventBase ev)
    {
        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (pdm == null)
            return;

        jCreditLb.text = pdm.GetProceeds(ProceedsType.Money_Prestige).ToString();
    }

    /// <summary>
    /// 刷新下次免费刷新时间Label;
    /// </summary>
    void updateNextRefreshTimeLb()
    {
        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();

        if (pdm == null)
            return;

        int index = pdm.GetPlayerShopData().Buncket;

        List<int> times = ShopModule.GetRefreshTimes();
        
        sRefreshTimeLb.text = string.Format(StringHelper.GetString("shop_refresh_time"), times[index]);
        //return null;
    }

    void clearSecretItemInAllItemList()
    {
        foreach(int key in mSecretItems.Keys)
        {
            if (mAllItemsLists.ContainsKey(key))
                mAllItemsLists.Remove(key);
        }
    }

    void UpdateShopItemsMoneyWithoutSecret()
    {
        BetterList<int> ids = ShopModule.GetEffectShopIdsWithoutSecret();
        
        if (ids == null)
            return;

        foreach (int id in ids)
        {
            if (!mAllItemsLists.ContainsKey(id))
                continue;

            MallItemUI itemUI = mAllItemsLists[id] as MallItemUI;
            
            if (itemUI == null)
                continue;

            itemUI.UpdateMoneyShow();
        }
    }

    /// <summary>
    /// 每次打开界面的强制刷新，只需要刷新神秘商店的，因为其他商店都是填表固定不变的;
    /// </summary>
    void updateMallUI(int resid = -1 , bool needDestroy = true)
    {
        PlayerDataModule playerModule = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (playerModule == null)
        {
            GameDebug.LogError("player data is null!");
            return;
        }

        // 其他商店刷新;
        UpdateShopItemsMoneyWithoutSecret();

        // 神秘商店初始化;
        BetterList<int> resIds = Module.GetPlayerSecretShopItemIds();

        if (needDestroy)
        {
            if (mSecretItems.Count > 0)
            {
                clearSecretItemInAllItemList();
                ObjectCommon.DestoryChildren(sGrid.gameObject);
                mSecretItems.Clear();
            }

            if (resIds != null)
            {
                foreach (int id in resIds)
                {
                    ShopTableItem tmpitem = DataManager.ShopTable[id] as ShopTableItem;
                    if (tmpitem == null)
                        continue;

                    CreateMallItem(tmpitem, (ShopSubTable)tmpitem.subTable);
                }

                sGrid.repositionNow = true;
            }
        }

        //刷新指定resid的商品;
        if (resIds.Contains(resid))
        {
            ShopItemInfo info = playerModule.GetShopSecretItemInfo(resid);

            if (info == null)
                return;

            setSecretItemInfo(resid, info);
            //isbuy = playerModule.GetShopIsBuyDone(resid);

            //if (!isbuy)
            //    return;

            //if (!setItemBuyDone(resid))
            //{
            //    GameDebug.LogError("商城数据错了");
            //    return;
            //}
        }
        //刷新全部的神秘商品;
        else
        {
            foreach (int id in resIds)
            {
                ShopItemInfo info = playerModule.GetShopSecretItemInfo(id);

                if (info == null)
                    continue;

                setSecretItemInfo(id, info);
            }
        }
    }

    //bool setItemBuyDone(int resid)
    //{
    //    if (mSecretItems.ContainsKey(resid))
    //    {
    //        ShopItemUI item = mSecretItems[resid];

    //        if (item != null)
    //        { 
    //            item.SetBuyDone();
    //            return true;
    //        }
    //    }

    //    return false;
    //}

    bool setSecretItemInfo(int resid , ShopItemInfo info)
    {
        if (info == null || !mSecretItems.ContainsKey(resid))
        {
            return false;
        }

        ShopItemUI item = mSecretItems[resid];

        if (item != null)
        {
            item.UpdateData(info);
            return true;
        }

        return false;
    }

    void onMallBuy(EventBase ev)
    {
        ShopUIEvent e = ev as ShopUIEvent;

        if (e == null)
            return;

        int resId = e.resId;
        if (!DataManager.ShopTable.ContainsKey(resId))
        {
            GameDebug.LogError("不存在的商店表id");
            return;
        }

        ShopTableItem item = DataManager.ShopTable[resId] as ShopTableItem;
        string content = StringHelper.GetString("egg_get_item") + ItemManager.getItemNameWithColor(item.itemId) + " X ";

        uint count = 0;
        if (mSecretItems.ContainsKey(resId))
        {
            PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
            ShopItemInfo info = pdm.GetShopSecretItemInfo(resId);

            if (info == null)
                return;

            count = (uint)info.count * item.multiple;
        }
        else
        {
            count = item.minCount * item.multiple;
        }

        PopTipManager.Instance.AddNewTip(content + count);

        updateMallUI(resId , false);
    }

    void onPageLeftClick1()
    {
        if (isFirstPage(PageVal1))
            return;

        PageVal1 -= pageDelta2BarVal(MAX_NUM_PER_PAGE, mSecretItems.Count);
    }

    void onPageRightClick1()
    {
        if (isLastPage(PageVal1))
            return;

        PageVal1 += pageDelta2BarVal(MAX_NUM_PER_PAGE, mSecretItems.Count);
    }

    void onPageLeftClick2()
    {
        if (isFirstPage(PageVal2))
            return;

        //PageVal2 -= pageDelta2BarVal(MAX_NUM_PER_PAGE, mCreditItems.size);
        PageVal2 -= pageDelta2BarVal(MAX_NUM_PER_PAGE, mCreditCount);
    }

    void onPageRightClick2()
    {
        if (isLastPage(PageVal2))
            return;

        //PageVal2 += pageDelta2BarVal(MAX_NUM_PER_PAGE, mCreditItems.size);
        PageVal2 += pageDelta2BarVal(MAX_NUM_PER_PAGE, mCreditCount);
    }

    void onPageLeftClick3()
    {
        if (isFirstPage(PageVal3))
            return;

        //PageVal3 -= pageDelta2BarVal(MAX_NUM_PER_PAGE, mEquipItems.size);
        PageVal3 -= pageDelta2BarVal(MAX_NUM_PER_PAGE, mEquipCount);
    }

    void onPageRightClick3()
    {
        if (isLastPage(PageVal3))
            return;

        //PageVal3 += pageDelta2BarVal(MAX_NUM_PER_PAGE, mEquipItems.size);
        PageVal3 += pageDelta2BarVal(MAX_NUM_PER_PAGE, mEquipCount);
    }

    bool isFirstPage(float val)
    {
        //return val <= Mathf.Epsilon;
        return val <= 0.01f;
    }

    bool isLastPage(float val)
    {
        //return Mathf.Approximately(val, 1f);
        return val >= 0.99f;
    }

    /// <summary>
    /// 根据每页显示的个数和总个数算出对应到0f-1f的偏移值;
    /// </summary>
    /// <param name="perNum"></param>
    /// <param name="totalNum"></param>
    /// <returns></returns>
    float pageDelta2BarVal(int perNum, int totalNum)
    {
        if (totalNum < perNum)
            return 0f;

        return Mathf.Min(1f, (float)perNum / (float)(totalNum - perNum));
    }

    void BuyMallItem(int resId)
    {
        ShopTableItem item = DataManager.ShopTable[resId] as ShopTableItem;

        if (item == null)
            return;

        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (pdm == null || pdm.GetShopIsBuyDone(resId))
            return;

        if (!mAllItemsLists.ContainsKey(resId))
            return;

        ShopItemUI itemUi = mAllItemsLists[resId];
        if (itemUi == null)
            return;

        // 已经买了吗;
        if (itemUi.IsScretItemBuyDone)
        {
            //PromptUIManager.Instance.AddNewPrompt(StringHelper.GetString("buy_done"));
            PopTipManager.Instance.AddNewTip(StringHelper.StringWithColor(FontColor.Red, StringHelper.GetString("buy_done")));
            return;
        }

        /// 钱够了吗;
        ProceedsType pt = ProceedsType.Invalid;
        int cost = 0;

        if (!itemUi.GetProcTypeAndCost(ref pt, ref cost))
            return;

        if (pdm.GetProceeds(pt) < cost)
        {
            //PromptUIManager.Instance.AddNewPrompt(ShopModule.GetBuyMoneyNotEnougthStr(pt));
            PopTipManager.Instance.AddNewTip(StringHelper.StringWithColor(FontColor.Red, ShopModule.GetBuyMoneyNotEnougthStr(pt)));
            return;
        }

        // 成功逻辑;
        ShopBuyItemAction param = new ShopBuyItemAction();
        param.OpType = (int)ShopOpType.BuyItem;
        param.ResId = resId;

        Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_SHOP, param);

        //MallBuyItemAction param = new MallBuyItemAction();
        //param.ResId = resId;
        //param.SubIdx = subId;

        ////有钱没;
        //MallItemInfo info = item.mallItems[subId];
        //if (pdm.GetProceeds((ProceedsType)item.processType) < info.processNow)
        //{
        //    PromptUIManager.Instance.AddNewPrompt("钱不够");
        //    return;
        //}

        ////次数够了没;
        //switch (mModule.GetLimitTypeByID(resId))
        //{
        //    case MallLimitType.ERROR:
        //        GameDebug.LogError("数据错误");
        //        return;
        //    case MallLimitType.NONE:
        //        break;
        //    case MallLimitType.DAY:
        //    case MallLimitType.FOREVER:
        //        if (mModule.GetPlayerBuyTimes(resId) >= item.limitTimes)
        //        {
        //            PromptUIManager.Instance.AddNewPrompt("购买次数够了");
        //            return;
        //        }
        //        break;
        //}

        //Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_MALL_BUY, param);
    }

    void OnBuyClick()
    {
        
        BuyMallItem(CurSelectResID);

        CloseBuyForm();
    }

    void CloseBuyForm()
    {
        gBuyObj.gameObject.SetActive(false);
    }

    void OpenBuyForm(int resId)
    {
        if (!DataManager.ShopTable.ContainsKey(resId))
            return;

        if (!mAllItemsLists.ContainsKey(resId))
            return;

        ShopItemUI itemUi = mAllItemsLists[resId];
        if (itemUi == null || itemUi.IsScretItemBuyDone)
            return;

        ShopTableItem item = DataManager.ShopTable[resId] as ShopTableItem;

        ItemTableItem tableItem = ItemManager.GetItemRes(item.itemId);
        if (tableItem == null)
        {
#if UNITY_EDITOR
            GameDebug.LogError("物品表中缺少ID为" + item.itemId + "的物品");
#endif
            return;
        }

        //gNameLb.text = tableItem.name;
        gNameLb.text = ItemManager.getItemNameWithColor(item.itemId);
        gItemDetailLb.text = tableItem.desc;
        gGetDetailLb.text = tableItem.desc0;
        UIAtlasHelper.SetSpriteImage(gIconSp, tableItem.picname, true);
        gItemTypeLb.text = ItemManager.GetItemTypeStr(item.itemId);
        
        ProceedsType pt = ProceedsType.Invalid;
        int cost = 0;

        if (!itemUi.GetProcTypeAndCost(ref pt, ref cost))
            return;

        UIAtlasHelper.SetSpriteByMoneyType(gMoneySp, pt);
        gMoneySp.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

        PlayerDataModule playerModule = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (playerModule == null)
        {
            GameDebug.LogError("player data is null!");
            return;
        }

        if (playerModule.GetProceeds(pt) < cost)
        {
            gMoneyLb.text = StringHelper.StringWithColor(FontColor.Red, cost.ToString());
        }
        else
        { 
            gMoneyLb.text = cost.ToString();
        }

        gBuyObj.SetActive(true);
    }

    void onMallItemClick(GameObject go)
    {
        int resId = System.Convert.ToInt32(go.name);

        if (!DataManager.ShopTable.ContainsKey(resId))
            return;

        CurSelectResID = resId;

        OpenBuyForm(resId);
    }

    void CreateMallItem(ShopTableItem item, ShopSubTable table = ShopSubTable.None)
    {
        GameObject go = WindowManager.Instance.CloneGameObject(mMallItemPrefab);

        if (go == null)
            return;

        go.SetActive(true);

        go.name = item == null ? "504" : item.resId.ToString();

        ShopItemUI shopItem = new ShopItemUI(go);

        switch (table)
        {
            case ShopSubTable.Sceret:
                go.transform.parent = sGrid.transform;
                go.transform.localScale = Vector3.one;

                if(item == null)
                    mSecretItems.Add(mSecretItems.Count, shopItem);
                else
                    mSecretItems.Add(item.resId , shopItem);
                break;
            case ShopSubTable.Credit:
                go.transform.parent = jGrid.transform;
                go.transform.localScale = Vector3.one;
                //mCreditItems.Add(shopItem);
                mCreditCount ++;
                break;
            case ShopSubTable.Equip:
                go.transform.parent = zGrid.transform;
                go.transform.localScale = Vector3.one;
                //mEquipItems.Add(shopItem);
                mEquipCount ++;
                break;
            default:
                GameDebug.LogError("商店商品所在分栏错误");
                break;
        }

        if (item != null)
        {
            if (!mAllItemsLists.ContainsKey(item.resId))
            {
                mAllItemsLists.Add(item.resId, shopItem);
            }
        }

        shopItem.SetData(item);
        UIEventListener.Get(go).onClick = onMallItemClick;
    }

    void CreateMallItems(BetterList<ShopTableItem> items)
    {
        if (items == null || items.size == 0)
            return;

        foreach (ShopTableItem item in items)
        {
            if (item == null) continue;

            CreateMallItem(item, (ShopSubTable)item.subTable);
        }
    }

    void CreateNullItems()
    {
        CreateNullItem(ShopSubTable.Credit);
        CreateNullItem(ShopSubTable.Equip);
    }

    void CreateNullItem(ShopSubTable table)
    {
        int num = getNullItemNum(table);

        while (num > 0)
        {
            CreateMallItem(null, table);
            --num;
        }
    }

    int getNullItemNum(ShopSubTable table)
    {
        int num = 0;

        switch (table)
        {
            case ShopSubTable.Sceret:
                num = mSecretItems.Count;
                break;
            case ShopSubTable.Credit:
                num = mCreditCount;
                break;
            case ShopSubTable.Equip:
                num = mEquipCount;
                break;
        }

        if (num < 8)
        {
            return 8 - num;
        }
        else
        {
            return (num % 2 == 0) ? 0 : 1;
        }
    }

    Pos getCurPagePos(float val, int count)
    {
        if (count <= MAX_NUM_PER_PAGE)
            return Pos.FirstLast;

        if (isFirstPage(val))
            return Pos.First;

        if (isLastPage(val))
            return Pos.Last;

        return Pos.Med;
    }

    void upDownAniHandler1()
    {
        switch (getCurPagePos(PageVal2, mCreditCount))
        {
            case Pos.FirstLast:
                jPageRightBtnAni.gameObject.SetActive(false);
                jPageLeftBtnAni.gameObject.SetActive(false);
                jPageRightBtn.gameObject.SetActive(true);
                jPageLeftBtn.gameObject.SetActive(true);
                break;
            case Pos.First:
                jPageLeftBtnAni.gameObject.SetActive(false);
                jPageLeftBtn.gameObject.SetActive(true);
                jPageRightBtnAni.gameObject.SetActive(true);
                jPageRightBtn.gameObject.SetActive(false);
                break;
            case Pos.Last:
                jPageRightBtnAni.gameObject.SetActive(false);
                jPageRightBtn.gameObject.SetActive(true);
                jPageLeftBtnAni.gameObject.SetActive(true);
                jPageLeftBtn.gameObject.SetActive(false);
                break;
            case Pos.Med:
                jPageRightBtnAni.gameObject.SetActive(true);
                jPageLeftBtnAni.gameObject.SetActive(true);
                jPageRightBtn.gameObject.SetActive(false);
                jPageLeftBtn.gameObject.SetActive(false);
                break;
        }

        UISpriteAnimation ani1 = jPageRightBtnAni.GetComponent<UISpriteAnimation>();
        if (ani1 != null) ani1.Reset();

        UISpriteAnimation ani2 = jPageLeftBtnAni.GetComponent<UISpriteAnimation>();
        if (ani2 != null) ani2.Reset();

    }

    void upDownAniHandler2()
    {
        switch (getCurPagePos(PageVal3, mEquipCount))
        {
            case Pos.FirstLast:
                zPageRightBtnAni.gameObject.SetActive(false);
                zPageLeftBtnAni.gameObject.SetActive(false);
                zPageRightBtn.gameObject.SetActive(true);
                zPageLeftBtn.gameObject.SetActive(true);
                break;
            case Pos.First:
                zPageLeftBtnAni.gameObject.SetActive(false);
                zPageLeftBtn.gameObject.SetActive(true);
                zPageRightBtnAni.gameObject.SetActive(true);
                zPageRightBtn.gameObject.SetActive(false);
                break;
            case Pos.Last:
                zPageRightBtnAni.gameObject.SetActive(false);
                zPageRightBtn.gameObject.SetActive(true);
                zPageLeftBtnAni.gameObject.SetActive(true);
                zPageLeftBtn.gameObject.SetActive(false);
                break;
            case Pos.Med:
                zPageRightBtnAni.gameObject.SetActive(true);
                zPageLeftBtnAni.gameObject.SetActive(true);
                zPageRightBtn.gameObject.SetActive(false);
                zPageLeftBtn.gameObject.SetActive(false);
                break;
        }

        UISpriteAnimation ani1 = zPageRightBtnAni.GetComponent<UISpriteAnimation>();
        if (ani1 != null) ani1.Reset();

        UISpriteAnimation ani2 = zPageLeftBtnAni.GetComponent<UISpriteAnimation>();
        if (ani2 != null) ani2.Reset();
    }
}
