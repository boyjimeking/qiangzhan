using UnityEngine;
using System.Collections;

public class EnergyBarUI 
{
    public enum EnergyBarShowType
    {
        ShowStarOnly, // 紧显示星值;
        ShowSpOnly,   // 紧显示体力值;
        All,          // 全部显示;
    }

    public UIButton closeBt;
    public UILabel starNumLb;
    public UIButton starBt;
    public UILabel spValueLb;
    public UIButton spBt;

    private StageListModule mStageListModule = null;
    private PlayerDataModule mPlayerDataModule = null;

    // 是否初始化过了;
    private bool mInited = false;

    private UIWindow mWindow = null;

    public EnergyBarUI( GameObject obj , UIWindow win )
    {
        mWindow = win;

        closeBt = ObjectCommon.GetChildComponent<UIButton>(obj, "mCloseBtn");
        starNumLb = ObjectCommon.GetChildComponent<UILabel>(obj, "mStarNumText");
        starBt = ObjectCommon.GetChildComponent<UIButton>(obj, "mStarIcon");
        spValueLb = ObjectCommon.GetChildComponent<UILabel>(obj, "mSPValueText");
        spBt = ObjectCommon.GetChildComponent<UIButton>(obj, "mSpBtn");


        EventDelegate.Add(closeBt.onClick, OnCloseClick);
        EventDelegate.Add(spBt.onClick, OnSpBtnClick);
        EventDelegate.Add(starBt.onClick, OnStarBtnClick);

        Init();
        if (mStageListModule == null)
            mStageListModule = ModuleManager.Instance.FindModule<StageListModule>();

        if (mPlayerDataModule == null)
            mPlayerDataModule = ModuleManager.Instance.FindModule<PlayerDataModule>();

        EventSystem.Instance.addEventListener(PlayerDataEvent.PLAYER_DATA_CHANGED, onPlayerDataChanged);
		EventSystem.Instance.addEventListener(PropertyEvent.MAIN_PROPERTY_CHANGE, onPlayerDataChanged);

        UpdateEnergy();
    }

    public void Destroy()
    {
        EventDelegate.Remove(closeBt.onClick, OnCloseClick);
        EventDelegate.Remove(spBt.onClick, OnSpBtnClick);
        EventDelegate.Remove(starBt.onClick, OnStarBtnClick);

        EventSystem.Instance.removeEventListener(PlayerDataEvent.PLAYER_DATA_CHANGED, onPlayerDataChanged);
        EventSystem.Instance.removeEventListener(PropertyEvent.MAIN_PROPERTY_CHANGE, onPlayerDataChanged);
    }

    public void UpdateInfo()
    {
        UpdateEnergy();
    }

    public void SetShowType(EnergyBarShowType type)
    {
        switch (type)
        {
            case EnergyBarShowType.All:
                starNumLb.gameObject.SetActive(true);
                starBt.gameObject.SetActive(true);
                spValueLb.gameObject.SetActive(true);
                spBt.gameObject.SetActive(true);
                break;

            case EnergyBarShowType.ShowStarOnly:
                starNumLb.gameObject.SetActive(true);
                starBt.gameObject.SetActive(true);
                spValueLb.gameObject.SetActive(false);
                spBt.gameObject.SetActive(false);
                break;

            case EnergyBarShowType.ShowSpOnly:
                starNumLb.gameObject.SetActive(false);
                starBt.gameObject.SetActive(false);
                spValueLb.gameObject.SetActive(true);
                spBt.gameObject.SetActive(true);
                break;
        }
    }

    void Init()
    {
        if (!mInited)
        {
            mInited = true;

            InitSpriteAni();
        }
    }

    void InitSpriteAni()
    {
        AnimationManager.Instance.AddSpriteAnimation("huobiliuguang1", starBt.gameObject, 3, 6).Interval = 2f;
        AnimationManager.Instance.AddSpriteAnimation("huobiliuguang3", spBt.gameObject, 3, 6).Interval = 2f;
    }

 

    void OnCloseClick()
    {
        CloseParent();
    }

    /// <summary>
    /// star点击;
    /// </summary>
    void OnStarBtnClick()
    {
        StageRewardParam param = new StageRewardParam();
        param.zoneid = mStageListModule.ZoneId;
        param.mCurStars = StageDataManager.Instance.GetZoneCurrentStarNum(mStageListModule.StageType, mStageListModule.ZoneId);
        param.mMaxStars = StageDataManager.Instance.GetZoneMaxStarNum(mStageListModule.StageType, mStageListModule.ZoneId);
        WindowManager.Instance.OpenUI("stagereward",param);
    }

    /// <summary>
    /// sp点击
    /// </summary>
    void OnSpBtnClick()
    {
        
    }

    void UpdateEnergy()
    {
        starNumLb.text = StageDataManager.Instance.GetZoneCurrentStarNum(mStageListModule.StageType, mStageListModule.ZoneId).ToString() + "/" +
            StageDataManager.Instance.GetZoneMaxStarNum(mStageListModule.StageType, mStageListModule.ZoneId).ToString(); ;

        int level = mPlayerDataModule.GetLevel();
        if (!DataManager.LevelTable.ContainsKey(level))
        {
            return;
        }

        LevelTableItem res = DataManager.LevelTable[level] as LevelTableItem;
        spValueLb.text = mPlayerDataModule.GetSP().ToString() + "/" + res.sp.ToString();
    }

    void onPlayerDataChanged(EventBase ev)
    {
        if (ev == null)
            return;

        UpdateEnergy();
    }

    void CloseParent()
    {
        if( mWindow != null )
        {
            WindowManager.Instance.CloseUI(mWindow.GetName());
        }
    }
}
