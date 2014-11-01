using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIMallForm : UIWindow
{
    protected GameObject mToggleBg;
    protected GameObject mallBg;
    //protected GameObject mMoneyBar;
    protected GameObject mToggleGroup;
    protected GameObject mReXiao;
    protected GameObject mXianLiang;

    protected UIToggle mToggle1;
    protected UIToggle mToggle2;

    #region 热卖
    UIButton rPageLeftBtn;
    UIButton rPageLeftBtnAni;
    UIButton rPageRightBtn;
    UIButton rPageRightBtnAni;
    UIScrollBar rScrollBar;
    UIScrollView rScrollView;
    UIGrid rGrid;

    private BetterList<MallItemUI> mHotItems = new BetterList<MallItemUI>();
    private int rCurPageNum = -1;
    private float rPageVal = 0f;
    #endregion

    #region 限量商品
    UIButton xPageLeftBtn;
    UIButton xPageLeftBtnAni;
    UIButton xPageRightBtn;
    UIButton xPageRightBtnAni;
    UIScrollBar xScrollBar;
    UIScrollView xScrollView;
    UIGrid xGrid;

    private BetterList<MallItemUI> mLimitItems = new BetterList<MallItemUI>();
    private int xCurPageNum = -1;
    private float xPageVal = 0f;
    #endregion

    #region 购买界面
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

    #region 打包展示界面
    GameObject dGroupObj;
    UIButton dCloseBtn;
    UISprite dGirlSp;
    UILabel dDetailLb;
    UIGrid dGrid;
    #endregion

    GameObject mMallItemPrefab;
    GameObject mGroupItemPrefab;

    const int MAX_NUM_PER_PAGE = 8;
    const float DURATION_PAGE = 0.5f;

    private bool isOpenFromOtherForm = false;

    private MallFormModule mModule = null;

    private int mCurSelResID = -1;//商城表格ID;
    private int mCurSelSubID = -1;//每组最多6个商品，标记买第几组商品[0-5];
    private int mlastGroupResId = -1;//如果两次打开相同的GroupItem，则只需要创建一次;

    //MallGroupForm界面上的mallitemui数据最多6个;<[0-5]列表下标 , MallItemUI>
    private Dictionary<int , MallGroupItemUI> mGroupItemLists = new Dictionary<int , MallGroupItemUI>();

    //所有界面上的MallItemUI数据;<resId , MallItemUI>
    private Dictionary<int, MallItemUI> mAllItemsLists = new Dictionary<int, MallItemUI>();

    private bool mNeedInit = true;  // 只执行一次的标记;

    protected override void OnLoad()
    {
        base.OnLoad();

        mToggleBg = this.FindChild("toggleBg");
        mallBg = this.FindChild("mallBg");
        //mMoneyBar = this.FindChild("MoneyBar");
        mToggleGroup = this.FindChild("ToggleGroup");
        mReXiao = this.FindChild("rexiao");
        mXianLiang = this.FindChild("xianliang");

        mToggle1 = this.FindComponent<UIToggle>("ToggleGroup/Toggle1");
        mToggle2 = this.FindComponent<UIToggle>("ToggleGroup/Toggle2");

        rPageLeftBtn = this.FindComponent<UIButton>("rexiao/pageLeftBtn");
        rPageLeftBtnAni = this.FindComponent<UIButton>("rexiao/pageLeftBtnAni");
        rPageRightBtn = this.FindComponent<UIButton>("rexiao/pageRightBtn");
        rPageRightBtnAni = this.FindComponent<UIButton>("rexiao/pageRightBtnAni");
        rScrollBar = this.FindComponent<UIScrollBar>("rexiao/ScrollBar");
        rScrollView = this.FindComponent<UIScrollView>("rexiao/ScrollView");
        rGrid = this.FindComponent<UIGrid>("rexiao/ScrollView/UIGrid");

        xPageLeftBtn = this.FindComponent<UIButton>("xianliang/pageLeftBtn");
        xPageLeftBtnAni = this.FindComponent<UIButton>("rexiao/pageLeftBtnAni");
        xPageRightBtn = this.FindComponent<UIButton>("xianliang/pageRightBtn");
        xPageRightBtnAni = this.FindComponent<UIButton>("xianliang/pageRightBtnAni");
        xScrollBar = this.FindComponent<UIScrollBar>("xianliang/ScrollBar");
        xScrollView = this.FindComponent<UIScrollView>("xianliang/ScrollView");
        xGrid = this.FindComponent<UIGrid>("xianliang/ScrollView/UIGrid");

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

        dGroupObj = this.FindChild("ItemGroupForm");
        dCloseBtn = this.FindComponent<UIButton>("ItemGroupForm/closeBtn");
        dGirlSp = this.FindComponent<UISprite>("ItemGroupForm/girlIconSp");
        dDetailLb = this.FindComponent<UILabel>("ItemGroupForm/detailLb");
        dGrid = this.FindComponent<UIGrid>("ItemGroupForm/grid");

        mMallItemPrefab = this.FindChild("items/MallItem");
        mGroupItemPrefab = this.FindChild("items/MallGroupItem");
    }


    float PageVal1
    {
        set
        {
            TweenScrollValue.Begin(rScrollBar.gameObject, DURATION_PAGE, value).PlayForward();
            //TweenAlpha.Begin(rScrollBar.gameObject, 1f, value).PlayForward();
            //rScrollBar.value = value;
        }
        get
        {
            return rScrollBar.value;
        }
    }

    float PageVal2
    {
        set
        {
            TweenScrollValue.Begin(xScrollBar.gameObject, DURATION_PAGE, value).PlayForward();
            //xScrollBar.value = value;
        }
        get
        {
            return xScrollBar.value;
        }
    }

    MallFormModule Module 
    {
        get
        {
            if (mModule == null)
                mModule = ModuleManager.Instance.FindModule<MallFormModule>();

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

    int CurSelectSubID
    {
        get
        {
            return mCurSelSubID;
        }
        set
        {
            if (value == mCurSelSubID)
            {
                return;
            }


            mCurSelSubID = value;
        }
    }

    protected override void OnOpen(object param = null)
    {
 	    base.OnOpen(param);

        WindowManager.Instance.CloseUI("city");
        WindowManager.Instance.CloseUI("joystick");

        EventDelegate.Add(gBuyBtn.onClick , OnBuyClick);
        EventDelegate.Add(gCloseBtn.onClick, CloseBuyForm);
        EventDelegate.Add(gOuterLineBtn.onClick, CloseBuyForm);
        EventDelegate.Add(dCloseBtn.onClick, CloseGroupItemsForm);
        EventDelegate.Add(rPageLeftBtn.onClick, onPageLeftClick1);
        EventDelegate.Add(rPageRightBtn.onClick, onPageRightClick1);
        EventDelegate.Add(xPageLeftBtn.onClick, onPageLeftClick2);
        EventDelegate.Add(xPageRightBtn.onClick, onPageRightClick2);

        EventDelegate.Add(rScrollBar.onChange, upDownAniHandler1);
        EventDelegate.Add(xScrollBar.onChange, upDownAniHandler2);

        EventDelegate.Add(rPageLeftBtnAni.onClick, onPageLeftClick1);
        EventDelegate.Add(rPageRightBtnAni.onClick, onPageRightClick1);
        EventDelegate.Add(xPageLeftBtnAni.onClick, onPageLeftClick2);
        EventDelegate.Add(xPageRightBtnAni.onClick, onPageRightClick2);

        EventSystem.Instance.addEventListener(MallUIEvent.MALL_BUY_ITEM, onMallBuy);

        Init();
        updateMallUI();
        Reset();

        if (param == null)
        {
            mToggleBg.SetActive(true);
            mallBg.SetActive(true);
            //mMoneyBar.SetActive(true);
            SetMoneyBarActive(true);
            mToggleGroup.SetActive(true);
            
            if(mToggle1.value)
                mReXiao.SetActive(true);
            if(mToggle2.value)
                mXianLiang.SetActive(true);

            isOpenFromOtherForm = false;
        }
        else
        {
            mToggleBg.SetActive(false);
            mallBg.SetActive(false);
            //mMoneyBar.SetActive(false);
            SetMoneyBarActive(false);
            mToggleGroup.SetActive(false);
            mReXiao.SetActive(false);
            mXianLiang.SetActive(false);

            isOpenFromOtherForm = true;

            OpenGroupItemsForm((int)param);
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
        //EventDelegate.Remove(dCloseBtn.onClick, CloseGroupItemsForm);
        EventDelegate.Remove(rPageLeftBtn.onClick, onPageLeftClick1);
        EventDelegate.Remove(rPageRightBtn.onClick, onPageRightClick1);
        EventDelegate.Remove(xPageLeftBtn.onClick, onPageLeftClick2);
        EventDelegate.Remove(xPageRightBtn.onClick, onPageRightClick2);

        EventDelegate.Remove(rScrollBar.onChange, upDownAniHandler1);
        EventDelegate.Remove(xScrollBar.onChange, upDownAniHandler2);

        EventDelegate.Remove(rPageLeftBtnAni.onClick, onPageLeftClick1);
        EventDelegate.Remove(rPageRightBtnAni.onClick, onPageRightClick1);
        EventDelegate.Remove(xPageLeftBtnAni.onClick, onPageLeftClick2);
        EventDelegate.Remove(xPageRightBtnAni.onClick, onPageRightClick2);

        EventSystem.Instance.removeEventListener(MallUIEvent.MALL_BUY_ITEM, onMallBuy);

        Reset();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        EventDelegate.Remove(dCloseBtn.onClick, CloseGroupItemsForm);
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

            rScrollBar.gameObject.SetActive(true);
            rScrollBar.foregroundWidget.gameObject.SetActive(false);
            rScrollBar.backgroundWidget.gameObject.SetActive(false);

            xScrollBar.gameObject.SetActive(true);
            xScrollBar.foregroundWidget.gameObject.SetActive(false);
            xScrollBar.backgroundWidget.gameObject.SetActive(false);
        }
    }

    void InitMallItems()
    {
        CreateMallItems(Module.GetEffectMallItems());
        CreateNullItems();

        rGrid.repositionNow = true;
        xGrid.repositionNow = true;

    }

    void Reset()
    {
        xScrollBar.value = 0f;
        rScrollBar.value = 0f;

        //CloseGroupItemsForm();
        deActiveGroupItemsForm();
        CloseBuyForm();
    }

    /// <summary>
    /// 每次打开界面的强制刷新;
    /// </summary>
    void updateMallUI()
    {
        PlayerDataModule playerModule = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (playerModule == null)
        {
            GameDebug.LogError("player data is null!");
            return;
        }

        BetterList<int> resIds = Module.GetEffectMallItemIDs();
        int time = 0;
        MallItemUI item = null;
        foreach (int id in resIds)
        {
            time = playerModule.GetMallBuyTimesByID(id);
            
            if (mAllItemsLists.ContainsKey(id))
            {
                item = mAllItemsLists[id];

                if (item == null)
                    continue;

                item.UpdateMoneyShow();

                if (time >= 0)
                    item.SetBuyTimes((uint)time);
            }
            else
            {
                GameDebug.LogError("商城数据错了");
                continue;
            }
        }
    }

    void onMallBuy(EventBase ev)
    {
        MallUIEvent mallEvent = ev as MallUIEvent;

        if (mallEvent == null || !Module.MallTable.ContainsKey(mallEvent.resId))
            return;

        int resId = mallEvent.resId;
        int subId = mallEvent.subId;

        MallTableItem item = Module.MallTable[resId] as MallTableItem;

        uint count = item.mallItems[subId].count;

        string content = StringHelper.GetString("egg_get_item") + ItemManager.getItemNameWithColor(item.itemId) + " X ";

        PopTipManager.Instance.AddNewTip(content + count);

        updateMallUI();
    }

    void onPageLeftClick1()
    {
        if (isFirstPage(PageVal1))
            return;

        PageVal1 -= pageDelta2BarVal(MAX_NUM_PER_PAGE, mHotItems.size);
    }

    void onPageRightClick1()
    {
        if (isLastPage(PageVal1))
            return;

        PageVal1 += pageDelta2BarVal(MAX_NUM_PER_PAGE, mHotItems.size);
    }

    void onPageLeftClick2()
    {
        if (isFirstPage(PageVal2))
            return;

        PageVal2 -= pageDelta2BarVal(MAX_NUM_PER_PAGE, mLimitItems.size);
    }

    void onPageRightClick2()
    {
        if (isLastPage(PageVal2))
            return;

        PageVal2 += pageDelta2BarVal(MAX_NUM_PER_PAGE, mLimitItems.size);
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

        return Mathf.Min(1f , (float)perNum / (float)(totalNum - perNum));
    }

    void BuyMallItem(int resId , int subId)
    {
        MallTableItem item = Module.MallTable[resId] as MallTableItem;

        if (item == null)
            return;

        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();

        //有钱没;
        MallItemInfo info = item.mallItems[subId];
        ProceedsType pt = (ProceedsType)item.processType;
        if (pdm.GetProceeds(pt) < info.processNow)
        {
            //PromptUIManager.Instance.AddNewPrompt(ShopModule.GetBuyMoneyNotEnougthStr(pt));
            PopTipManager.Instance.AddNewTip(StringHelper.StringWithColor(FontColor.Red, ShopModule.GetBuyMoneyNotEnougthStr(pt)));
            return;
        }

        //次数够了没;
        switch (mModule.GetLimitTypeByID(resId))
        {
            case MallLimitType.ERROR:
                GameDebug.LogError("数据错误");
                return;
            case MallLimitType.NONE:
                break;
            case MallLimitType.DAY:
            case MallLimitType.FOREVER:
                if (mModule.GetPlayerBuyTimes(resId) >= item.limitTimes)
                { 
                    //PromptUIManager.Instance.AddNewPrompt(StringHelper.GetString("buy_no_time"));
                    PopTipManager.Instance.AddNewTip(StringHelper.StringWithColor(FontColor.Red, StringHelper.GetString("buy_no_time")));
                    return;
                }
                break;
        }

        MallBuyItemAction param = new MallBuyItemAction();
        param.ResId = resId;
        param.SubIdx = subId;

        Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_MALL_BUY, param);
    }

    void OnBuyClick()
    {
        //Net.Instance.DoAction()
        BuyMallItem(CurSelectResID, CurSelectSubID);

        CloseBuyForm();
    }

    void CloseBuyForm()
    {
        gBuyObj.gameObject.SetActive(false);
    }

    void OpenBuyForm(int resId)
    {
        PlayerDataModule playerModule = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (playerModule == null)
        {
            GameDebug.LogError("player data is null!");
            return;
        }

        if(!Module.MallTable.ContainsKey(resId))
            return;

        MallTableItem item = Module.MallTable[resId] as MallTableItem;
        //EventDelegate.Add(gBuyBtn.onClick, OnBuyClick);

        ItemTableItem tableItem = ItemManager.GetItemRes(item.itemId);
        if (tableItem == null)
        {
#if UNITY_EDITOR
            GameDebug.LogError("物品表中缺少ID为"+ item.itemId +"的物品");
#endif
            return;
        }

        if (!mAllItemsLists.ContainsKey(resId))
            return;

        MallItemUI itemUI = mAllItemsLists[resId] as MallItemUI;
        if (itemUI == null || !itemUI.MallCanOpenBuyForm)
            return;

        //gNameLb.text = tableItem.name;
        gNameLb.text = ItemManager.getItemNameWithColor(item.itemId);
        gItemDetailLb.text = tableItem.desc;
        gGetDetailLb.text = tableItem.desc0;
        UIAtlasHelper.SetSpriteImage(gIconSp, tableItem.picname, true);
        UIAtlasHelper.SetSpriteByMoneyType(gMoneySp, (ProceedsType)item.processType);
        gMoneySp.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

        uint moneyNum = item.mallItems[0].processNow;
        if(playerModule.GetProceeds((ProceedsType)item.processType) < moneyNum)
        {
            gMoneyLb.text = StringHelper.StringWithColor(FontColor.Red , moneyNum.ToString());
        }
        else
        {
            gMoneyLb.text = moneyNum.ToString();
        }

        gItemTypeLb.text = ItemManager.GetItemTypeStr(item.itemId);

        gBuyObj.SetActive(true);
    }

    void onGroupItemClick(GameObject go)
    {
        if (go == null) return;

        string[] strs = go.name.Split('_');

        if (strs.Length != 2)
            return;

        int resId = System.Convert.ToInt32(strs[0]);
        int subId = System.Convert.ToInt32(strs[1]);

        BuyMallItem(resId, subId);

        CloseGroupItemsForm();
    }

    void CreateGroupItem(MallTableItem item , int idx)
    {
        GameObject go = null;
        if (!mGroupItemLists.ContainsKey(idx))
        {
            go = WindowManager.Instance.CloneGameObject(mGroupItemPrefab);
        }
        else
        {
            go = mGroupItemLists[idx].gameObject;
        }

        if (go == null)
            return;

        go.SetActive(true);
        go.name = item.resId + "_" + idx;
        go.transform.parent = dGrid.transform;
        go.transform.localScale = Vector3.one;

        MallGroupItemUI mallItem = new MallGroupItemUI(go);

        mallItem.SetData(item , (uint)idx);
        UIEventListener.Get(go).onClick = onGroupItemClick;


        if (!mGroupItemLists.ContainsKey(idx))
        {
            mGroupItemLists.Add(idx , mallItem);
        }
        else
        {
            mGroupItemLists[idx] = mallItem;
        }
    }

    void OpenGroupItemsForm(int resId)
    {
        if (mlastGroupResId == resId)
        {
            dGroupObj.SetActive(true);

            //if (isOpenFromOtherForm)
            //    this.mView.SetActive(true);

            return;
        }

        mlastGroupResId = resId;

        MallTableItem item = Module.MallTable[resId] as MallTableItem;
        if (item == null)
        {
            Debug.LogError("错误了");
            return;
        }

        int i = 0;
        int count = Module.GetItemNumByMallId(resId);
        for ( ; i < count; i++)
        {
            if (item.mallItems[i].count > 0)
            {
                CreateGroupItem(item, i);
            }
            else
            {
                if (mGroupItemLists.ContainsKey(i))
                { 
                    mGroupItemLists[i].gameObject.SetActive(false);
                }
            }
        }

        for (int m = i ; m < mGroupItemLists.Count; m++)
        {
            mGroupItemLists[m].gameObject.SetActive(false);
        }

        ItemTableItem tableItem = ItemManager.GetItemRes(item.itemId);
        if (tableItem == null)
        {
#if UNITY_EDITOR
            GameDebug.LogError("物品表中缺少ID为" + item.itemId + "的物品");
#endif
            return;
        }

        dGroupObj.SetActive(true);
        dDetailLb.text = item.detail;

        dGrid.repositionNow = true;
    }

    void deActiveGroupItemsForm()
    {
        dGroupObj.gameObject.SetActive(false);
    }

    void CloseGroupItemsForm()
    {
        deActiveGroupItemsForm();

        if (isOpenFromOtherForm)
        {
            WindowManager.Instance.CloseUI("mall");
        }
    }

    void onMallItemClick(GameObject go)
    {
        int resId = System.Convert.ToInt32(go.name);

        if (!Module.MallTable.ContainsKey(resId))
            return;

        CurSelectResID = resId;
        CurSelectSubID = 0;

        if (Module.GetItemNumByMallId(resId) > 1)
        {
            OpenGroupItemsForm(resId);
        }
        else
        {
            OpenBuyForm(resId);
        }
    }

    void CreateMallItem(MallTableItem item, MallSubTableType type = MallSubTableType.None)
    {
        GameObject go = WindowManager.Instance.CloneGameObject(mMallItemPrefab);

        if (go == null)
            return;

        go.SetActive(true);

        go.name = (item == null) ? "504" : item.resId.ToString();
        
        MallItemUI mallItem = new MallItemUI(go);

        switch(type)
        {
            case MallSubTableType.HotSale:
                go.transform.parent = rGrid.transform;
                go.transform.localScale = Vector3.one;
                mallItem.ShowHuiSprite(false);
                mHotItems.Add(mallItem);
                //rGrid.repositionNow = true;
                break;
            case MallSubTableType.LimitCount:
                go.transform.parent = xGrid.transform;
                go.transform.localScale = Vector3.one;
                mLimitItems.Add(mallItem);
                //xGrid.repositionNow = true;
                break;
            default:
                GameDebug.LogError("商城商品所在分栏错误");
                break;
        }

        if (item != null)
        { 
            if (!mAllItemsLists.ContainsKey(item.resId))
            {
                mAllItemsLists.Add(item.resId, mallItem);
            }
        }

        mallItem.SetData(item);
        UIEventListener.Get(go).onClick = onMallItemClick;
    }

    void CreateMallItems(BetterList<MallTableItem> items)
    {
        if(items == null || items.size == 0)
            return;

        foreach (MallTableItem item in items)
        {
            if (item == null) continue;

            CreateMallItem(item, (MallSubTableType)item.subField);
        }
    }

    void CreateNullItems()
    {
        CreateNullItem(MallSubTableType.HotSale);
        CreateNullItem(MallSubTableType.LimitCount);
    }

    void CreateNullItem(MallSubTableType table)
    {
        int num = getNullItemNum(table);

        while (num > 0)
        {
            CreateMallItem(null, table);
            --num;
        }
    }

    int getNullItemNum(MallSubTableType table)
    {
        int num = 0;

        switch (table)
        {
            case MallSubTableType.HotSale:
                num = mHotItems.size;
                break;
            case MallSubTableType.LimitCount:
                num = mLimitItems.size;
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

    Pos getCurPagePos(float val , BetterList<MallItemUI> list)
    {
        if (list.size <= MAX_NUM_PER_PAGE)
            return Pos.FirstLast;

        if (isFirstPage(val))
            return Pos.First;

        if (isLastPage(val))
            return Pos.Last;

        return Pos.Med;
    }

    void upDownAniHandler1()
    {
        switch (getCurPagePos(PageVal1 , mHotItems))
        {
            case Pos.FirstLast:
                rPageRightBtnAni.gameObject.SetActive(false);
                rPageLeftBtnAni.gameObject.SetActive(false);
                rPageRightBtn.gameObject.SetActive(true);
                rPageLeftBtn.gameObject.SetActive(true);
                break;
            case Pos.First:
                rPageLeftBtnAni.gameObject.SetActive(false);
                rPageLeftBtn.gameObject.SetActive(true);
                rPageRightBtnAni.gameObject.SetActive(true);
                rPageRightBtn.gameObject.SetActive(false);
                break;
            case Pos.Last:
                rPageRightBtnAni.gameObject.SetActive(false);
                rPageRightBtn.gameObject.SetActive(true);
                rPageLeftBtnAni.gameObject.SetActive(true);
                rPageLeftBtn.gameObject.SetActive(false);
                break;
            case Pos.Med:
                rPageRightBtnAni.gameObject.SetActive(true);
                rPageLeftBtnAni.gameObject.SetActive(true);
                rPageRightBtn.gameObject.SetActive(false);
                rPageLeftBtn.gameObject.SetActive(false);
                break;
        }

        UISpriteAnimation ani1 = rPageRightBtnAni.GetComponent<UISpriteAnimation>();
        if (ani1 != null) ani1.Reset();

        UISpriteAnimation ani2 = rPageLeftBtnAni.GetComponent<UISpriteAnimation>();
        if (ani2 != null) ani2.Reset();

    }

    void upDownAniHandler2()
    {
        switch (getCurPagePos(PageVal2 , mLimitItems))
        {
            case Pos.FirstLast:
                xPageRightBtnAni.gameObject.SetActive(false);
                xPageLeftBtnAni.gameObject.SetActive(false);
                xPageRightBtn.gameObject.SetActive(true);
                xPageLeftBtn.gameObject.SetActive(true);
                break;
            case Pos.First:
                xPageLeftBtnAni.gameObject.SetActive(false);
                xPageLeftBtn.gameObject.SetActive(true);
                xPageRightBtnAni.gameObject.SetActive(true);
                xPageRightBtn.gameObject.SetActive(false);
                break;
            case Pos.Last:
                xPageRightBtnAni.gameObject.SetActive(false);
                xPageRightBtn.gameObject.SetActive(true);
                xPageLeftBtnAni.gameObject.SetActive(true);
                xPageLeftBtn.gameObject.SetActive(false);
                break;
            case Pos.Med:
                xPageRightBtnAni.gameObject.SetActive(true);
                xPageLeftBtnAni.gameObject.SetActive(true);
                xPageRightBtn.gameObject.SetActive(false);
                xPageLeftBtn.gameObject.SetActive(false);
                break;
        }

        UISpriteAnimation ani1 = xPageRightBtnAni.GetComponent<UISpriteAnimation>();
        if (ani1 != null) ani1.Reset();

        UISpriteAnimation ani2 = xPageLeftBtnAni.GetComponent<UISpriteAnimation>();
        if (ani2 != null) ani2.Reset();

    }
}
