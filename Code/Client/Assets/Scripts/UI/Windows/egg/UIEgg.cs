using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Message;

public class UIEgg : UIWindow
{
    public delegate void FuncCall(GameObject go , object param);

    class FuncCallBack
    {
        public FuncCall callBack; 
        public int startMillionSec;
        public int endMillionSec;
        public GameObject go;
        public object param;
        public FuncCallBack()
        {
 
        }
    }


    #region 界面参数;

    private static readonly string[] EGG_NAMES = new string[6]{
        "egg_normal0" , "egg_normal1" ,
        "egg_supper0" , "egg_supper1" ,
        "egg_gold0" , "egg_gold1"
    };

    public const int SHOW_MILLION_SEC = 2000;  //产生的物品展示多少毫秒后消失;
    public const float FLY_TO_BAG_SEC = 1f;    //物品飞到背包用时;

    public static readonly Vector3 ITEM_DROP_START_POS = new Vector3(0f, 121f, 0f); //物品创建起始位置;

    // 三种蛋对应的掉落区域范围宽度、高度;
    private Vector3 DropEndRange = new Vector3(250f , 60f , 0f);
    private Vector3 DropEndOffset = new Vector3(0f, -70f, 0f);

    #endregion
    #region 界面组件;
    //protected BetterList<UISprite> mItems = new BetterList<UISprite>();
    protected GameObject[] mEggs = new GameObject[3];

    // 要展示的物品根节点;
    protected UIGrid mItemGrid;

    // 展示物品的ScrollBar;
    protected UIScrollBar mItemScrollBar;

    // 三种蛋免费标题显示;
    protected GameObject[] freeObj = new GameObject[3];

    // 三种蛋消费钱币数/免费显示;
    protected UILabel[] costLb = new UILabel[3];

    // 三种蛋消费钱币类型显示;
    protected UISprite[] costSp = new UISprite[3];

    // 三种蛋倒计时显示;
    protected UILabel[] timeCountLb = new UILabel[3];

    // 三种蛋剩余次数显示;
    protected UILabel[] timesRemainLb = new UILabel[3];

    // 三种蛋标题图片;
    protected UISprite[] headSp = new UISprite[3];

    // 背包按钮;
    protected UIButton bagBtn;

    protected GameObject mEggShowItemPrefab;
    protected GameObject mEggGoodsPrefab;
    #endregion

    private EggModule mModule = null;
    private PlayerDataModule mPlayerModule = null;

    private Dictionary<EggType, int> mCounter = new Dictionary<EggType, int>();

    List<FuncCallBack> mCacheCallBack = new List<FuncCallBack>();
    private uint mLastMillionSec = 0;
    private uint mOneSecNum = 0;      // 每秒计时标志;

    EggModule Module
    {
        get
        {
            if (mModule == null)
            {
                mModule = ModuleManager.Instance.FindModule<EggModule>();
            }
            return mModule;
        }
    }

    PlayerDataModule PDM
    {
        get 
        {
            if (mPlayerModule == null)
            {
                mPlayerModule = ModuleManager.Instance.FindModule<PlayerDataModule>();
            }

            return mPlayerModule;
        }
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        //mItems.Add(this.FindComponent<UISprite>("Bottom/Items/item8/icon"));

        mEggs[0] = this.FindChild("Eggs/egg1");
        mEggs[1] = this.FindChild("Eggs/egg2");
        mEggs[2] = this.FindChild("Eggs/egg3");

        mItemGrid = this.FindComponent<UIGrid>("Bottom/ScrollView/UIGrid");

        mItemScrollBar = this.FindComponent<UIScrollBar>("Bottom/ScrollBar");

        freeObj[0] = this.FindChild("Eggs/egg1/title");
        freeObj[1] = this.FindChild("Eggs/egg2/title");
        freeObj[2] = this.FindChild("Eggs/egg3/title");

        costLb[0] = this.FindComponent<UILabel>("Eggs/egg1/cost/costLb");
        costLb[1] = this.FindComponent<UILabel>("Eggs/egg2/cost/costLb");
        costLb[2] = this.FindComponent<UILabel>("Eggs/egg3/cost/costLb");

        costSp[0] = this.FindComponent<UISprite>("Eggs/egg1/cost/costSp");
        costSp[1] = this.FindComponent<UISprite>("Eggs/egg2/cost/costSp");
        costSp[2] = this.FindComponent<UISprite>("Eggs/egg3/cost/costSp");

        timeCountLb[0] = this.FindComponent<UILabel>("Eggs/egg1/timeCountLb");
        timeCountLb[1] = this.FindComponent<UILabel>("Eggs/egg2/timeCountLb");
        timeCountLb[2] = this.FindComponent<UILabel>("Eggs/egg3/timeCountLb");

        timesRemainLb[0] = this.FindComponent<UILabel>("Eggs/egg1/remainLb");
        timesRemainLb[1] = this.FindComponent<UILabel>("Eggs/egg2/remainLb");
        timesRemainLb[2] = this.FindComponent<UILabel>("Eggs/egg3/remainLb");

        headSp[0] = this.FindComponent<UISprite>("Eggs/egg1/headSp");
        headSp[1] = this.FindComponent<UISprite>("Eggs/egg2/headSp");
        headSp[2] = this.FindComponent<UISprite>("Eggs/egg3/headSp");

        bagBtn = this.FindComponent<UIButton>("Bottom/Items/bag");

        mEggGoodsPrefab = this.FindChild("items/goodsitem");
        mEggShowItemPrefab = this.FindChild("items/showitem");

        Init();
    }

    protected override void OnOpen(object param = null)
    {
        base.OnOpen(param);

        AddEventListener();
        updateForm();
        resetFormShow();

        PopTipManager.Instance.SetOffsetY(100f);
    }

    protected override void OnClose()
    {
        base.OnClose();

        RemoveEventListener();

        PopTipManager.Instance.SetOffsetY(0f);
    }

    public override void Update(uint elapsed)
    {
        base.Update(elapsed);

        updateTimeCountDown();

        mLastMillionSec = (mLastMillionSec + elapsed) % uint.MaxValue;

        if ((mLastMillionSec - mOneSecNum) >= 1000)
        {
            mOneSecNum = mLastMillionSec;
            //updateTimeCountDown();
        }

        if (mCacheCallBack != null && mCacheCallBack.Count != 0)
        {
            for (int i = 0; i < mCacheCallBack.Count; i++ )
            {
                FuncCallBack func = mCacheCallBack[i];
                if (func == null)
                    continue;

                if (func.startMillionSec < func.endMillionSec)
                {
                    func.startMillionSec += (int)elapsed;
                }
                else
                {
                    func.callBack(func.go , func.param);
                    mCacheCallBack.Remove(func);
                    i = 0; // 重新从头开始遍历数组;
                }
            }
        }
    }

    void updateForm()
    {
        updateEggsInfo();
        updateEggCostInfo();
    }

    void updateEggsInfo()
    {
        for (int i = 0; i < EggModule.EGG_COUNT; i++)
        {
            timesRemainLb[i].text = "今日剩余次数：" + Module.GetRemainTimeByEggId((EggType)(i + 1));
        }
    }

    void updateTimeCountDown()
    {
        for (int i = 0; i < 3; i++)
        {
            int sec = Module.GetSecondsByEggId((EggType)(i+1));

            if (sec <= 0)
            {
                freeObj[i].gameObject.SetActive(true);
                timeCountLb[i].gameObject.SetActive(false);

                continue;
            }
            else
            {
                freeObj[i].SetActive(false);
                timeCountLb[i].gameObject.SetActive(true);
            }

            //PDM.SubEggTimeSeconds(et);

            timeCountLb[i].text = TimeUtilities.GetCountDownHMS(sec * 1000);
        }
        //Module.GetSecondsByEggId()
    }

    void Init()
    {
        BuildItems();
        //updateEggCostInfo();
        AddEventDelegate();
    }

    void resetFormShow()
    {
        mItemScrollBar.value = 0f;
    }

    void BuildItems()
    {
        BetterList<int> itemIds = EggModule.GetShowItemsItemID();
        
        if (itemIds == null)
        {
            GameDebug.LogError("表格数据错误");
            return;
        }

        //int count = 0;
        foreach(int itemid in itemIds)
        {
            ItemTableItem item = ItemManager.GetItemRes(itemid);

            if (item == null)
            {
                GameDebug.LogError("物品表中不存在物品ID：" + itemid);
                continue;
            }

            GameObject go = WindowManager.Instance.CloneGameObject(mEggShowItemPrefab);
            if (go == null)
            {
                GameDebug.LogError("创建gameobj失败");
                continue;
            }

            go.name = item.id.ToString();
            go.transform.parent = mItemGrid.transform;
            go.transform.localScale = Vector3.one;

            EggShowItemUI showui = new EggShowItemUI(go);
            
            //showui.SetImg(item.picname, true);
            showui.SetInfo(item.id);
            

            //UIAtlasHelper.SetSpriteImage(mItems[count], item.picname , true);
            //count++;

            //if (count > mItems.size)
            //    break;
        }

        mItemGrid.repositionNow = true;
        mItemScrollBar.value = 0f;
    }

    void updateEggCostInfo()
    {
        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (pdm == null)
            return;

        for (int i = 0; i < EggModule.EGG_COUNT; i++)
        {
            EggType et = (EggType)(i+1);
            int num = EggModule.getCostProcNum(et);
            ProceedsType pt = EggModule.getCostProcType(et);

            if(pt == ProceedsType.Invalid || num <= 0)
            {
                costLb[i].transform.parent.gameObject.SetActive(false);
            }
            else
            {
                if (pdm.GetProceeds(pt) < num)
                    costLb[i].text = StringHelper.StringWithColor(FontColor.Red, num.ToString());
                else
                    costLb[i].text = num.ToString();

                UIAtlasHelper.SetSpriteByMoneyType(costSp[i], pt , false);
                costLb[i].transform.parent.gameObject.SetActive(true);
            }
        }
    }

    void AddEventDelegate()
    {
        for (int i = 0, j = mEggs.Length; i < j; i++)
        {
            UIEventListener.Get(mEggs[i]).onClick = onEggClick;
        }

        UIEventListener.Get(bagBtn.gameObject).onClick = onBagBtnClick;
    }

    void AddEventListener()
    {
        EventSystem.Instance.addEventListener(EggUIEvent.EGG_OPEN_SUCESS, onEggOpenSucess);
    }

    void RemoveEventListener()
    {
        EventSystem.Instance.removeEventListener(EggUIEvent.EGG_OPEN_SUCESS, onEggOpenSucess);
    }

    void onBagBtnClick(GameObject go)
    {
        WindowManager.Instance.OpenUI("bag");
    }

    /// <summary>
    /// 普通蛋：倒计时完成了就可以砸;
    /// 高级蛋、钻石蛋：倒计时到了就可以免费砸一次，否则花钱也可以砸;
    /// </summary>
    /// <param name="go"></param>
    void onEggClick(GameObject go)
    {
        if (go == null)
            return;

        string last = go.name.Substring(go.name.Length - 1, 1);
        
        int idx = System.Convert.ToInt32(last);

        if (idx < 1 || idx > mEggs.Length)
        {
            GameDebug.LogError("名字不配代码");
            return;
        }

        bool isFree = false;

        EggType et = (EggType)idx;

        // 时间到了吗;
        if (Module.GetSecondsByEggId(et) > 0)
        {
            switch (et)
            {
                case EggType.Normal:
                    //GameDebug.LogError("时间没到砸不开");
                    PopTipManager.Instance.AddNewTip(StringHelper.GetString("egg_is_counting", FontColor.Red));
                    return;

                case EggType.Supper:
                case EggType.ZuanShi:
                    break;
            }

            isFree = false;
        }
        else
        {
            isFree = true;
        }

        // 次数不够了;
        if (Module.GetRemainTimeByEggId(et) == 0)
        {
            //PromptUIManager.Instance.AddNewPrompt("今日次数已用完！");
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("egg_no_times" , FontColor.Red));
            return;
        }

        // 钱不够;
        ProceedsType pt = ProceedsType.Invalid;
        if (!isFree && !Module.IsEnougthMoney(et, ref pt))
        {
            //PromptUIManager.Instance.AddNewPrompt("钱不够，无法砸蛋！");
            if (pt == ProceedsType.Money_Game)
            {
                PopTipManager.Instance.AddNewTip(StringHelper.GetString("egg_no_money", FontColor.Red));
            }
            else if(pt == ProceedsType.Money_RMB)
            {
                PopTipManager.Instance.AddNewTip(StringHelper.GetString("egg_no_rmb", FontColor.Red));                
            }
            return;
        }

        RequestOpenEgg(idx);
    }

    /// <summary>
    /// 请求砸蛋;idx = [1..3];
    /// </summary>
    /// <param name="idx"></param>
    void RequestOpenEgg(int idx)
    {
        EggClickParam param = new EggClickParam();
        
        param.opType = EGG_OP_TYPE.EGG_OP_OPEN;
        param.eggType = (EggType)idx;

        Net.Instance.DoAction((int)MESSAGE_ID.ID_MSG_EGG, param);
    }

    void onEggOpenSucess(EventBase ev)
    {
        EggUIEvent evt = ev as EggUIEvent;
        
        if (evt == null)
            return;

        playSucessEffect(evt.eggType);
        createItems(evt.eggType, evt.items);
        updateForm();
    }

    void playSucessEffect(EggType et)
    {
        int idx = (int)et;

        UISprite eggSpri = mEggs[idx - 1].GetComponent<UISprite>();
        eggSpri.spriteName = EGG_NAMES[(idx - 1) * 2 + 1];
        eggSpri.MakePixelPerfect();

        ParticleAnimation ani = AnimationManager.Instance.PlayParticleAnimation(4, mEggs[idx - 1], 15);
        ani.gameObject.transform.localPosition = new Vector3(ani.gameObject.transform.localPosition.x, ani.gameObject.transform.localPosition.y + 100f, ani.gameObject.transform.localPosition.z);
        
    }

    Vector3 getRandomEndPos(EggType et)
    {
        //Vector3 eggPos = mEggs[(int)et - 1].transform.localPosition;

        Vector3 res = new Vector3();
        
        res.x = UnityEngine.Random.Range(-DropEndRange.x / 2f, DropEndRange.x / 2f);
        res.y = UnityEngine.Random.Range(-DropEndRange.y / 2f, DropEndRange.y / 2f);
        res.z = 0f;

        return res + DropEndOffset;
    }

    /// <summary>
    /// 获得背包在每个蛋中的相对位置;
    /// </summary>
    /// <returns></returns>
    Vector3 getBagLocalPositionByEgg(EggType et)
    {
        Vector3 res = new Vector3();

        res.x = bagBtn.transform.localPosition.x + bagBtn.transform.parent.localPosition.x - mEggs[(int)et - 1].transform.localPosition.x;
        res.y = bagBtn.transform.parent.localPosition.y - mEggs[(int)et - 1].transform.localPosition.y;
        res.z = 0f;

        return res;
    }

    void createItems(EggType et, List<role_egg_item_items> items)
    {
        if (items == null || items.Count == 0)
            return;

        //GameObject go = new GameObject();
        
        //go.transform.parent = mEggs[(int)et - 1].transform;
        //go.transform.localPosition = Vector3.zero;
        //go.transform.localScale = Vector3.one;
        
        int i = 0, j = items.Count;

        for (; i < j; i++)
        {
            GameObject go = createItem(et, items[i], mEggs[(int)et - 1]);

            if (go == null)
            {
                GameDebug.LogError("创建物品失败！");
                continue;
            }


            Vector3 dropPos = getRandomEndPos(et);
            // 立刻执行物品掉落特效;
            addInvoke(dropToFloor, go, dropPos,1);
            
            if (mCounter.ContainsKey(et))
            {
                mCounter[et]++;
            }
            else
            {
                mCounter.Add(et, 1);
            }
            checkCostObjIsShow(et);

            // SHOW_MILLION_SEC毫秒后执行moveToBag操作;
            addInvoke(moveToBag, go, getBagLocalPositionByEgg(et), SHOW_MILLION_SEC);

            // SHOW_MILLION_SEC毫秒后执行计数检测操作;
            addInvoke(removeCounter, go, et, SHOW_MILLION_SEC + (int)(FLY_TO_BAG_SEC * 1000));

            //string content = StringHelper.GetString("egg_get_item") + ItemManager.getItemNameWithColor(items[i].itemid) + " X " + items[i].itemnum;
            //PopTipManager.Instance.AddNewTip(content);
            PopTipManager.Instance.AddGetItemTip(items[i].itemid, items[i].itemnum);
        }
    }

    GameObject createItem(EggType et, role_egg_item_items item, GameObject parent)
    {
        GameObject go = WindowManager.Instance.CloneGameObject(mEggGoodsPrefab);

        if (go == null)
            return null;
        go.SetActive(false);
        go.name = item.itemid.ToString();
        go.transform.parent = parent.transform;
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = ITEM_DROP_START_POS;
        go.SetActive(true);

        EggGoodsUI goodsItem = new EggGoodsUI(go);

        bool isShowAni = EggModule.GetItemIsInShowItems(item.itemid);

        goodsItem.SetData(item.itemid, item.itemnum , isShowAni);

        return go;
    }

    // 物品掉落特效，掉落播放完成后，不消耗go;
    void dropToFloor(GameObject go, object param)
    {
        DropEffect drop = go.AddMissingComponent<DropEffect>();

        if (param == null)
        {
            GameDebug.LogError("error");
            return;
        }

        drop.Play(go.transform.localPosition, (Vector3)param, 1f, false);
    }

    // 物品飞特效，飞效果播放完成后，自动销毁go;
    void moveToBag(GameObject go , object param)
    {
        FlyEffect fly = go.AddMissingComponent<FlyEffect>();
        fly.Play(go.transform.position, (Vector3)param, 1f);
    }

    void removeCounter(GameObject go, object param)
    {
        EggType et = (EggType)param;

        if (!mCounter.ContainsKey(et))
        {
            return;
        }
        else 
        {
            --mCounter[et];
        }

        checkCostObjIsShow(et);
    }

    /// <summary>
    /// 是否显示每个蛋的“价格”信息;
    /// </summary>
    /// <param name="et"></param>
    void checkCostObjIsShow(EggType et)
    {
        if (et == EggType.Normal)
        {
            return;
        }
        
        if (!mCounter.ContainsKey(et))
        {
            costLb[(int)et - 1].transform.parent.gameObject.SetActive(true);
        }
        else
        {
            costLb[(int)et - 1].transform.parent.gameObject.SetActive(mCounter[et] <= 0);
        }
    }

    void addInvoke(FuncCall callback, GameObject go, object param, int endMillionSec, int startMillionSec = 0)
    {
        if (callback == null || endMillionSec <= 0 || startMillionSec < 0)
            return;

        FuncCallBack func = new FuncCallBack();
        
        func.callBack = callback;
        func.startMillionSec = startMillionSec;
        func.endMillionSec = endMillionSec;
        func.go = go;
        func.param = param;

        addInvoke(func);
    }

    void addInvoke(FuncCallBack callback)
    {
        if (callback == null)
            return;

        mCacheCallBack.Add(callback);
    }
}
