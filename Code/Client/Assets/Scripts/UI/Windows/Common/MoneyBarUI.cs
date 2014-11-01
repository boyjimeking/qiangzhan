using UnityEngine;
using System.Collections;

public enum MoneyBarType:int
{
    JiFen = 0,      //积分;
    XingXing = 1,        //星星;
    ShengWang = 2,   //声望;
    TiLi = 3,        //体力;
    Jinbi = 4,        //金币;
    ZuanShi = 5,     //钻石;

    Max = 6,
}


/// <summary>
/// 显示顺序 从左到右 ：积分、星星、声望、体力、金币、钻石;
/// </summary>
public class MoneyBarUI
{
	public UIButton closeBt;
    
    public UISprite creditSp;
    public UILabel creditLb;
    public UIButton creditBt;

    public UISprite starSp;
    public UILabel starLb;
    public UIButton starBt;

    public UISprite shengwangSp;
    public UILabel shengwangLb;
    public UIButton shengwangBt;

    public UISprite tiliSp;
    public UILabel tiliLb;
    public UIButton tiliBt;

    public UISprite coinSp;
	public UILabel coinLb;
	public UIButton coinBt;

    public UISprite diamondSp;
	public UILabel diamondLb;
	public UIButton diamondBt;

    public UIGrid grid;

    // 是否初始化过了;
    private bool mInited = false;

    private GameObject mGo = null;
    private UIWindow mParant = null;

    private PlayerDataModule mPlayerDataModule = null;

    public GameObject gameObject
    {
        get
        {
            return mGo;
        }
    }

    public MoneyBarUI(UIWindow parant)
    {
        if (parant.GetName() == "common")
            return;

        GameObject moneyBar = WindowManager.Instance.CloneCommonUI("MoneyBar");
        if (moneyBar == null)
        {
            Debug.LogError("找不到MoneyBar");
            return;
        }


        moneyBar.name = moneyBar.name.Replace("(Clone)", "");
        moneyBar.gameObject.SetActive(true);
        moneyBar.transform.parent = parant.gameObject.transform;
        moneyBar.transform.localPosition = new Vector3(186f, 288f, 0f);
        moneyBar.transform.localScale = Vector3.one;
        
        closeBt = ObjectCommon.GetChildComponent<UIButton>(moneyBar, "returnBtn");

        creditSp = ObjectCommon.GetChildComponent<UISprite>(moneyBar, "Grid/6credit/iconSp");
        creditLb = ObjectCommon.GetChildComponent<UILabel>(moneyBar, "Grid/6credit/countLb");
        creditBt = ObjectCommon.GetChildComponent<UIButton>(moneyBar, "Grid/6credit/btSp");

        starSp = ObjectCommon.GetChildComponent<UISprite>(moneyBar, "Grid/5star/iconSp");
        starLb = ObjectCommon.GetChildComponent<UILabel>(moneyBar, "Grid/5star/countLb");
        starBt = ObjectCommon.GetChildComponent<UIButton>(moneyBar, "Grid/5star/btSp");

        shengwangSp = ObjectCommon.GetChildComponent<UISprite>(moneyBar, "Grid/4shengwang/iconSp");
        shengwangLb = ObjectCommon.GetChildComponent<UILabel>(moneyBar, "Grid/4shengwang/countLb");
        shengwangBt = ObjectCommon.GetChildComponent<UIButton>(moneyBar, "Grid/4shengwang/btSp");

        tiliSp = ObjectCommon.GetChildComponent<UISprite>(moneyBar, "Grid/3tili/iconSp");
        tiliLb = ObjectCommon.GetChildComponent<UILabel>(moneyBar, "Grid/3tili/countLb");
        tiliBt = ObjectCommon.GetChildComponent<UIButton>(moneyBar, "Grid/3tili/btSp");

        coinSp = ObjectCommon.GetChildComponent<UISprite>(moneyBar, "Grid/2coin/iconSp");
        coinLb = ObjectCommon.GetChildComponent<UILabel>(moneyBar, "Grid/2coin/countLb");
        coinBt = ObjectCommon.GetChildComponent<UIButton>(moneyBar, "Grid/2coin/btSp");

        diamondSp = ObjectCommon.GetChildComponent<UISprite>(moneyBar, "Grid/1diamond/iconSp");
        diamondLb = ObjectCommon.GetChildComponent<UILabel>(moneyBar, "Grid/1diamond/countLb");
        diamondBt = ObjectCommon.GetChildComponent<UIButton>(moneyBar, "Grid/1diamond/btSp");

        grid = ObjectCommon.GetChildComponent<UIGrid>(moneyBar, "Grid");

        mGo = moneyBar;
        mParant = parant;

        mPlayerDataModule = ModuleManager.Instance.FindModule<PlayerDataModule>();

        Init();

        EventDelegate.Add(closeBt.onClick, OnCloseClick);

        EventSystem.Instance.addEventListener(ProceedsEvent.PROCEEDS_CHANGE_ALL, UpdateMoneyInfo);
        EventSystem.Instance.addEventListener(PlayerDataEvent.PLAYER_DATA_CHANGED, onPlayerDataChanged);
        EventSystem.Instance.addEventListener(PropertyEvent.MAIN_PROPERTY_CHANGE, onPlayerDataChanged);

        UpdateMoney();
        UpdateSpAndStar();
    }

    public void OnEnable()
    {
        UpdateMoney();
        UpdateSpAndStar();
    }

    public void Destroy()
    {
        EventSystem.Instance.removeEventListener(ProceedsEvent.PROCEEDS_CHANGE_ALL, UpdateMoneyInfo);
        EventSystem.Instance.removeEventListener(PlayerDataEvent.PLAYER_DATA_CHANGED, onPlayerDataChanged);
        EventSystem.Instance.removeEventListener(PropertyEvent.MAIN_PROPERTY_CHANGE, onPlayerDataChanged);
    }
    public void SetShowType(MoneyBarType showType)
    {
        Init();

        for (int i = 0, j = (int)MoneyBarType.Max; i < j; i++)
        {
            GameObject go = getGameobjectBySortid(i);
            go.SetActive((MoneyBarType)i == showType);
        }

        grid.repositionNow = true;
    }

    public void SetShowType(BetterList<MoneyBarType> showTypes)
    {
        Init();

        for(int i = 0, j = (int)MoneyBarType.Max; i < j; i++)
        {
            GameObject go = getGameobjectBySortid(i);
            go.SetActive(showTypes.Contains((MoneyBarType)i));
        }

        grid.repositionNow = true;
    }


    void Init()
    {
        if (!mInited)
        {
            mInited = true;

            InitClickListener();
            InitSpriteAni();
            initShowType();
        }
    }

    void InitClickListener()
    {
        EventDelegate.Add(starBt.onClick, OnStarBtnClick);
        EventDelegate.Add(shengwangBt.onClick, OnShengWangClick);
        EventDelegate.Add(tiliBt.onClick, OnTiliClick);
        EventDelegate.Add(creditBt.onClick, OnCreditBtnClick);
        EventDelegate.Add(coinBt.onClick, OnCoinBtnClick);
        EventDelegate.Add(diamondBt.onClick, OnDiamondBtnClick);
    }

    void InitSpriteAni()
    {
        AnimationManager.Instance.AddSpriteAnimation("huobiliuguang1", creditSp.gameObject, 5, 6).Interval = 2f;
        AnimationManager.Instance.AddSpriteAnimation("huobiliuguang2", shengwangSp.gameObject, 5, 6).Interval = 2f;
        AnimationManager.Instance.AddSpriteAnimation("huobiliuguang3", coinSp.gameObject, 5, 6).Interval = 2f;
        AnimationManager.Instance.AddSpriteAnimation("huobiliuguang4", tiliSp.gameObject, 5, 6).Interval = 2f;
        AnimationManager.Instance.AddSpriteAnimation("huobiliuguang5", starSp.gameObject, 5, 6).Interval = 2f;
        AnimationManager.Instance.AddSpriteAnimation("huobiliuguang6", diamondSp.gameObject, 5, 6).Interval = 2f;
    }

    void initShowType()
    {
        UITableItem item = WindowManager.Instance.GetUIRes(mParant.GetName());

        if (item == null)
            return;

        if (string.IsNullOrEmpty(item.barFlag))
        {
            mGo.SetActive(false);

            return;
        }
        string[] flags = item.barFlag.Split(new string[] { "|" }, System.StringSplitOptions.RemoveEmptyEntries);
        if(flags.Length != 6)
        {
            Debug.LogError("uiconfig表的右上角MoneyBar列，数据错误。id=" + item.resID);
            return;
        }

        bool allFalse = true;
        for(int i = 0; i < 6 ; i++)
        {
            GameObject go = getGameobjectBySortid(i);
            int c = System.Convert.ToInt32(flags[i]);

            if (c != 0)
                allFalse = false;
            go.SetActive(c != 0);
        }

        if (allFalse)
            mGo.gameObject.SetActive(false);

        grid.repositionNow = true;
    }

    /// <summary>
    /// 积分、星星、声望、体力、金币、钻石;
    /// </summary>
    /// <param name="id"></param>
    GameObject getGameobjectBySortid(int id)
    {
        Transform trans = null;
        switch(id)
        {
            case 0:
                trans = creditSp.transform;
                break;
            case 1:
                trans = starSp.transform;
                break;
            case 2:
                trans = shengwangSp.transform;
                break;
            case 3:
                trans = tiliSp.transform;
                break;
            case 4:
                trans = coinSp.transform;
                break;
            case 5:
                trans = diamondSp.transform;
                break;
        }

        return trans.parent.gameObject;
    }

    void activeParent(GameObject go, bool active)
    {
        go.transform.parent.gameObject.SetActive(active);
    }

// 	void Open()
// 	{
//         EventDelegate.Add(closeBt.onClick, OnCloseClick);
//         EventDelegate.Add(coinBt.onClick, OnCoinBtnClick);
//         EventDelegate.Add(diamondBt.onClick, OnDiamondBtnClick);
//         Init();
//         
// 	}


// 	void Close()
// 	{
// 		EventSystem.Instance.removeEventListener(PlayerDataEvent.PLAYER_DATA_CHANGED , UpdateMoneyInfo);
// 	}

	void OnCloseClick()
	{
	    SoundManager.Instance.Play(30);
		CloseParent();
	}

    void OnCreditBtnClick()
    {

    }

	/// <summary>
	/// 金币点击;
	/// </summary>
	void OnCoinBtnClick()
	{

	}

	/// <summary>
	/// 钻石点击
	/// </summary>
	void OnDiamondBtnClick()
	{

	}

    /// <summary>
    /// 星星点击;
    /// </summary>
    void OnStarBtnClick()
    {

    }

    /// <summary>
    /// 声望点击;
    /// </summary>
    void OnShengWangClick()
    {

    }

    /// <summary>
    /// 体力点击;
    /// </summary>
    void OnTiliClick()
    {

    }

	void UpdateMoney()
	{
		if(mPlayerDataModule == null)
			return;

        creditLb.text = mPlayerDataModule.GetProceeds(ProceedsType.Money_Arena).ToString();
		coinLb.text = mPlayerDataModule.GetProceeds(ProceedsType.Money_Game).ToString();
		diamondLb.text = mPlayerDataModule.GetProceeds(ProceedsType.Money_RMB).ToString();
        shengwangLb.text = mPlayerDataModule.GetProceeds(ProceedsType.Money_Prestige).ToString();
	}

	void UpdateMoneyInfo(EventBase ev)
	{
		if(ev == null)
			return;

		UpdateMoney();
	}

    void UpdateSpAndStar()
    {
        StageListModule mStageListModule = ModuleManager.Instance.FindModule<StageListModule>();
        if (mStageListModule == null)
            return;
        starLb.text = StageDataManager.Instance.GetZoneCurrentStarNum(mStageListModule.StageType, mStageListModule.ZoneId).ToString() + "/" +
            StageDataManager.Instance.GetZoneMaxStarNum(mStageListModule.StageType, mStageListModule.ZoneId).ToString(); ;


        int level = mPlayerDataModule.GetLevel();
        if (!DataManager.LevelTable.ContainsKey(level))
        {
            return;
        }

        LevelTableItem res = DataManager.LevelTable[level] as LevelTableItem;
        tiliLb.text = mPlayerDataModule.GetSP().ToString() + "/" + res.sp.ToString();
    }

    void onPlayerDataChanged(EventBase ev)
    {
        if (ev == null)
            return;

        UpdateSpAndStar();
    }

    void CloseParent()
    {
        if( mParant != null )
        {
            WindowManager.Instance.CloseUI(mParant.GetName());
        }
    }
}
