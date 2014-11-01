using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;


public enum Pos
{
    FirstLast,
    First,
    Med,
    Last,
}

public class UIWeapon : UIWindow

{
    #region 定义
    //
    public List<GameObject> mTabIndex = new List<GameObject>();
    //军火库
    public UILabel mPresLa;
    public UIButton mBuyButton;
    public UIButton mEquipMainButton;
    public UIButton mEquipSubButton;
    public UIGrid mShopGrid;
    public UILabel mMBaseL;
    public UILabel mMStrenL;
    public UILabel mMAddL;
    public UILabel mMSpL;
    public UILabel mWName;
    public UISprite mHintSp;
    public UISprite mWeaponSkillIcon = null;
    public UIButton mBtnWeaponSkill = null;
    public GameObject mInfo = null;
    public GameObject mVal = null;
    public GameObject mSkillInfo = null;

    //枪械奥义
    public GameObject mAGrid;
    public UIScrollView mAScoll;
    private Dictionary<int, PromoteGridUI> mAWeaponM = new Dictionary<int, PromoteGridUI>();
    private int mACenId = 0;
    private PromoteGridUI MAOld = null;
    private int mOldAId = -1;

    private UILabel mSkillDesc = null;
    private UISprite mSkillIcon = null;
    private UILabel mSkillName = null;


    //强化
    public List<UISprite> mSStar = new List<UISprite>();
    public List<UILabel> mSOV = new List<UILabel>();
    public List<UILabel> mSOVName = new List<UILabel>();
    public UILabel mSLName;
    public UILabel mSLNameNT;
    public UILabel mSCuV;
    public UILabel mSNeV;
    public UILabel mSCurrP;
    public UILabel mSOLv;
    public UILabel mSCost;
    public UIButton mStrenButton;
    private UISprite mWeaponLvPic = null;
    private UISprite mWeaponIcon = null;
    private GameObject mPerItemObj = null;
    private UILabel mAddproName = null;
    private UILabel mStrNotes1 = null;
    private UILabel mStrNotes2 = null;
    private UIButton mBtnStrenItemInfo = null;

    private UISprite mStrWeaponPic = null;

    //进阶
    public UISprite mPWeaponIcon;
    public UILabel mPCurLv;
    public UILabel mPNexLv;
    public UILabel mPCurVal;
    public UILabel mPNexVal;
    public UISprite mPOne;
    public UISprite mPTwo;
    public UISprite mPOne1;
    public UISprite mPTwo1;
    public UILabel mPOne2;
    public UILabel mPTwo2;
    public UILabel mPOName;
    public UILabel mPTName;
    public UILabel mPOCount;
    public UILabel mPTCount;
    public UIButton mPromoteBtn;
    public GameObject mPGrid;
    public UIScrollView mPScoll;
    public UIButton mPUpBtn;
    public UIButton mPUpBtnAni;
    public UIButton mPDownBtn;
    public UIButton mPDownBtnAni;
    public UIButton mBtnPromoteItem1 = null;
    public UIButton mBtnPromoteItem2 = null;

    //配件
    private UIButton mBtnReturn = null;
    private UIButton mBtnFitNow = null;
    private UIButton mBtnFitNext = null;
    public UISprite mFSUI;
    public UISprite mFFUI;
    public List<GameObject> mFSlot = new List<GameObject>();
    public List<UILabel> mFSFight = new List<UILabel>();
    public List<UILabel> mFSDesc = new List<UILabel>();
    public List<GameObject> mFSDBack = new List<GameObject>();
    public List<UILabel> mFProValue = new List<UILabel>();
    public List<UIToggle> mFLockBtn = new List<UIToggle>();
    public List<UISprite> mFPName = new List<UISprite>();
    public UISprite mFIcon;
    public UILabel mFCost;
    public UILabel mFHaveNum;
    public UILabel mFFightValue;
    public UIButton mFYes;
    public UILabel mFChance;
    public UISprite mFNFitt;
    public UILabel mFNLab;
    public GameObject mFittHide1 = null;
    public GameObject mFittHide2 = null;
    public GameObject mChancePanel = null;
    public UIButton mBtnPeijianItem = null;

    private WeaponShopGridUI mOldWeaponGridUI = null;
    private Dictionary<WeaponShopGridUI, int> mGridEventMap = new Dictionary<WeaponShopGridUI, int>();
    private Dictionary<int, WeaponShopGridUI> mGridMap = new Dictionary<int, WeaponShopGridUI>();
    private Dictionary<int, WeaponObj> mWeaponMap = new Dictionary<int, WeaponObj>();
    private Dictionary<int, PromoteGridUI> mPWeaponM = new Dictionary<int, PromoteGridUI>();


    private int mMainEWeaponId = -1;
    private int mSubEWeaponId = -1;

    private const uint STREN_STEP = 10;

    private int mOldProId = -1;
    private int mOldProLv = -1;
    private int mPMaxId = int.MinValue;
    private int mPMinId = int.MaxValue;
    private int mPCenId = 0;
    private PromoteGridUI MPOld = null;

    //已经被选择的武器
    private int mOldId;

    private bool[] mFOpen = new bool[(uint)FittingsType.MAX_FITTGINS];
    private int mFId = -1;
    private uint mFPos = 0;
    private int mNXPos = 0;


    private GameObject WeaponShopGridUIPrefab = null;
    private GameObject PromoteGridUIPrefa = null;
    private GameObject mFittingCostPanel = null;

    private UICharacterPreview mPreview = new UICharacterPreview();
    private UISprite mPreveiwSprite = null;
    private UISprite mPreveiwSpriteStren = null;
    private UISprite mBackgroundSprite = null;

    private GameObject mPerItem0 = null;
    private GameObject mPerItem1 = null;

    private UIWeaponPreview mWeaponPreview = new UIWeaponPreview();

    //特效
    public UISpriteAnimation mStrenSuccessAni;

    public int mPromoteItem1 = -1;
    public int mPromoteItem2 = -1;
    public GameObject mOldSelectedTab = null;
    #endregion
    public UIWeapon()
    {

    }
    protected override void OnLoad()
    {
        base.OnLoad();

        mTabIndex.Add(this.FindChild("TagGroup/Toggle0"));
        mTabIndex.Add(this.FindChild("TagGroup/Toggle2"));
        mTabIndex.Add(this.FindChild("TagGroup/Toggle3"));
        mTabIndex.Add(this.FindChild("TagGroup/Toggle4"));

        #region 军火库
        //mPresLa = this.FindComponent<UILabel>("Junhuoku/ShengWang/shengwangLb");

        mBuyButton = this.FindComponent<UIButton>("Junhuoku/BuyBtn");
        mEquipMainButton = this.FindComponent<UIButton>("Junhuoku/EquipBtn");
        mEquipSubButton = this.FindComponent<UIButton>("Junhuoku/EquipBtn2");
        
        mShopGrid = this.FindComponent<UIGrid>("Junhuoku/ScrollPanel/Scroll View/UIGrid");

        mMBaseL = this.FindComponent<UILabel>("Junhuoku/ManInfo/Val/val0");
        mMStrenL = this.FindComponent<UILabel>("Junhuoku/ManInfo/Val/val1");
        mMAddL = this.FindComponent<UILabel>("Junhuoku/ManInfo/Val/val2");
        mMSpL = this.FindComponent<UILabel>("Junhuoku/ManInfo/Val/val3");
        mWName = this.FindComponent<UILabel>("Junhuoku/ManInfo/name");
        mHintSp = this.FindComponent<UISprite>("Junhuoku/hintSp");
        mInfo = this.FindChild("Junhuoku/ManInfo/Info");
        mVal = this.FindChild("Junhuoku/ManInfo/Val");
        mWeaponSkillIcon = this.FindComponent<UISprite>("Junhuoku/ManInfo/SkillPanel/icon");
        mBtnWeaponSkill = this.FindComponent<UIButton>("Junhuoku/ManInfo/SkillPanel/bg");
        mSkillInfo = this.FindChild("Junhuoku/ManInfo/SkillInfo");
        #endregion

        #region 技能
        mAGrid = this.FindChild("Jineng/ScrollPanel/ScrollView/UIGrid");
        mAScoll = this.FindComponent<UIScrollView>("Jineng/ScrollPanel/ScrollView");
        mSkillDesc = this.FindComponent<UILabel>("Jineng/Sprite/Sprite/leftLb");
        mSkillIcon = this.FindComponent<UISprite>("Jineng/Sprite/SkillIcon");
        mSkillName = this.FindComponent<UILabel>("Jineng/Sprite/SkillName");

        #endregion

        #region 强化
        mSStar.Add(this.FindComponent<UISprite>("Qianghua/Item/Stars/Sprite0"));
        mSStar.Add(this.FindComponent<UISprite>("Qianghua/Item/Stars/Sprite1"));
        mSStar.Add(this.FindComponent<UISprite>("Qianghua/Item/Stars/Sprite2"));
        mSStar.Add(this.FindComponent<UISprite>("Qianghua/Item/Stars/Sprite3"));
        mSStar.Add(this.FindComponent<UISprite>("Qianghua/Item/Stars/Sprite4"));
        mSStar.Add(this.FindComponent<UISprite>("Qianghua/Item/Stars/Sprite5"));
        mSStar.Add(this.FindComponent<UISprite>("Qianghua/Item/Stars/Sprite6"));
        mSStar.Add(this.FindComponent<UISprite>("Qianghua/Item/Stars/Sprite7"));
        mSStar.Add(this.FindComponent<UISprite>("Qianghua/Item/Stars/Sprite8"));
        mSStar.Add(this.FindComponent<UISprite>("Qianghua/Item/Stars/Sprite9"));
        mSOVName.Add(this.FindComponent<UILabel>("Qianghua/ItemInfo/AttriRoot/Label0"));
        mSOVName.Add(this.FindComponent<UILabel>("Qianghua/ItemInfo/AttriRoot/Label1"));
        mSOVName.Add(this.FindComponent<UILabel>("Qianghua/ItemInfo/AttriRoot/Label2"));
        mSOVName.Add(this.FindComponent<UILabel>("Qianghua/ItemInfo/AttriRoot/Label3"));
        mSOV.Add(this.FindComponent<UILabel>("Qianghua/ItemInfo/AttriRoot/Label4"));
        mSOV.Add(this.FindComponent<UILabel>("Qianghua/ItemInfo/AttriRoot/Label5"));
        mSOV.Add(this.FindComponent<UILabel>("Qianghua/ItemInfo/AttriRoot/Label6"));
        mSOV.Add(this.FindComponent<UILabel>("Qianghua/ItemInfo/AttriRoot/Label7"));
        mSLName = this.FindComponent<UILabel>("Qianghua/Item/levelLb");
        mSLNameNT = this.FindComponent<UILabel>("Qianghua/Item/levelLbNT");
        mSCuV = this.FindComponent<UILabel>("Qianghua/Item/Current/damage");
        mSNeV = this.FindComponent<UILabel>("Qianghua/Item/Next/damage");
        mSCurrP = this.FindComponent<UILabel>("Qianghua/Item/PerItem/countLb");
        mSOLv = this.FindComponent<UILabel>("Qianghua/ItemInfo/openLb");
        mSCost = this.FindComponent<UILabel>("Qianghua/Item/PerItem/nameLb");
        mPerItemObj = this.FindChild("Qianghua/Item/PerItem");
        mAddproName = this.FindComponent<UILabel>("Qianghua/ItemInfo/Label");
        mStrNotes1 = this.FindComponent<UILabel>("Qianghua/strnotes1");
        mStrNotes2 = this.FindComponent<UILabel>("Qianghua/strnotes2");
        mBtnStrenItemInfo = this.FindComponent<UIButton>("Qianghua/Item/PerItem/iconSp");

        mStrenButton = this.FindComponent<UIButton>("Qianghua/BuyBtn");
        mWeaponLvPic = this.FindComponent<UISprite>("Qianghua/ItemInfo/weaponlv");
        mWeaponIcon = this.FindComponent<UISprite>("Qianghua/ItemInfo/iconSp");

        mStrWeaponPic =  this.FindComponent<UISprite>("Qianghua/Man/pic");
        #endregion

        #region 进阶-shengjie
        mPWeaponIcon = this.FindComponent<UISprite>("Shengjie/weaponSp");
        mPCurLv = this.FindComponent<UILabel>("Shengjie/Stage0/numLb");
        mPNexLv = this.FindComponent<UILabel>("Shengjie/Stage1/numLb");
        mPCurVal = this.FindComponent<UILabel>("Shengjie/Stage0/Damage/damage");
        mPNexVal = this.FindComponent<UILabel>("Shengjie/Stage1/Damage/damage");
        mPOne = this.FindComponent<UISprite>("Shengjie/Items/PerItem0/iconSp/Sprite");
        mPTwo = this.FindComponent<UISprite>("Shengjie/Items/PerItem1/iconSp/Sprite");
        mPOne1 = this.FindComponent<UISprite>("Shengjie/Items/PerItem0/iconSp/sprite1");
        mPTwo1 = this.FindComponent<UISprite>("Shengjie/Items/PerItem1/iconSp/sprite1");
        mPOne2 = this.FindComponent<UILabel>("Shengjie/Items/PerItem0/iconSp/label1");
        mPTwo2 = this.FindComponent<UILabel>("Shengjie/Items/PerItem1/iconSp/label1");
        mPOName = this.FindComponent<UILabel>("Shengjie/Items/PerItem0/nameLb");
        mPTName = this.FindComponent<UILabel>("Shengjie/Items/PerItem1/nameLb");
        mPOCount = this.FindComponent<UILabel>("Shengjie/Items/PerItem0/countLb");
        mPTCount = this.FindComponent<UILabel>("Shengjie/Items/PerItem1/countLb");
        mPromoteBtn = this.FindComponent<UIButton>("Shengjie/BuyBtn");
        mPGrid = this.FindChild("Shengjie/ScrollPanel/Scroll View/UIGrid");
        mPScoll = this.FindComponent<UIScrollView>("Shengjie/ScrollPanel/Scroll View");
        mPUpBtn = this.FindComponent<UIButton>("Shengjie/ScrollPanel/upBtn");
        //mPUpBtnAni = this.FindComponent<UIButton>("Shengjie/ScrollPanel/upBtnAni");
        mPDownBtn = this.FindComponent<UIButton>("Shengjie/ScrollPanel/downBtn");
        mPDownBtnAni = this.FindComponent<UIButton>("Shengjie/ScrollPanel/downBtnAni");

        mBtnPromoteItem1 = this.FindComponent<UIButton>("Shengjie/Items/PerItem0/iconSp");
        mBtnPromoteItem2 = this.FindComponent<UIButton>("Shengjie/Items/PerItem1/iconSp");


        mWeaponPreview.SetTargetSprite(mPWeaponIcon);
        mWeaponPreview.RotationY = 180;
        #endregion

        #region 配件
        mFSUI = this.FindComponent<UISprite>("Peijian/slot");
        mFFUI = this.FindComponent<UISprite>("Peijian/fitt");

        for (int i = 0; i < 6; i++)
        {
            mFSlot.Add(this.FindChild("Peijian/slot/Sprite" + i));
            mFSFight.Add(this.FindComponent<UILabel>("Peijian/slot/Sprite" + i + "/Label"));
            mFSDesc.Add(this.FindComponent<UILabel>("Peijian/slot/Sprite" + i + "/Sprite1/Label"));
            mFSDBack.Add(this.FindChild("Peijian/slot/Sprite" + i + "/Sprite1"));
            mFPName.Add(this.FindComponent<UISprite>("Peijian/fitt/Sprite1/Sprite" + i));
        }

        mFProValue.Add(this.FindComponent<UILabel>("Peijian/fitt/Label0"));
        mFProValue.Add(this.FindComponent<UILabel>("Peijian/fitt/Label1"));
        mFProValue.Add(this.FindComponent<UILabel>("Peijian/fitt/Label2"));

        mFLockBtn.Add(this.FindComponent<UIToggle>("Peijian/fitt/btn0"));
        mFLockBtn.Add(this.FindComponent<UIToggle>("Peijian/fitt/btn1"));
        mFLockBtn.Add(this.FindComponent<UIToggle>("Peijian/fitt/btn2"));

        mFIcon = this.FindComponent<UISprite>("Peijian/fitt/Sprite0/Sprite");
        mFCost = this.FindComponent<UILabel>("Peijian/fitt/fittingcostpanel/LabelCost");
        mFHaveNum = this.FindComponent<UILabel>("Peijian/fitt/fittingcostpanel/LabelNum");
        mFFightValue = this.FindComponent<UILabel>("Peijian/fitt/LabelFight");
        mFYes = this.FindComponent<UIButton>("Peijian/Btn");
        mFChance = this.FindComponent<UILabel>("Peijian/fitt/chancepanel/LabelChance");
        mFNFitt = this.FindComponent<UISprite>("Peijian/fitt/Sprite2/Sprite");
        mFNLab = this.FindComponent<UILabel>("Peijian/fitt/Sprite3/Label");
        mFittHide1 = this.FindChild("Peijian/fitt/Sprite2");
        mFittHide2 = this.FindChild("Peijian/fitt/Sprite3");
        mFittingCostPanel = this.FindChild("Peijian/fitt/fittingcostpanel");
        mChancePanel = this.FindChild("Peijian/fitt/chancepanel");

        mBtnFitNow = this.FindComponent<UIButton>("Peijian/fitt/Sprite0");
        mBtnFitNext = this.FindComponent<UIButton>("Peijian/fitt/Sprite2");
        mBtnReturn = this.FindComponent<UIButton>("Peijian/fitt/btnreturn");

        mBtnPeijianItem = this.FindComponent<UIButton>("Peijian/fitt/fittingcostpanel/Sprite");
        #endregion

        WeaponShopGridUIPrefab = this.FindChild("Items/EquipItem0");
        PromoteGridUIPrefa = this.FindChild("Items/EquipItem1");

        mPreveiwSprite = FindComponent<UISprite>("Junhuoku/Man/pic");
        //mPreveiwSpriteStren = this.FindComponent<UISprite>("Qianghua/Man/pic");
        mBackgroundSprite = this.FindComponent<UISprite>("Junhuoku/Man/Sprite");

        mPerItem0 = this.FindChild("Shengjie/Items/PerItem0");
        mPerItem1 = this.FindChild("Shengjie/Items/PerItem1");

        //特效
        mStrenSuccessAni = this.FindComponent<UISpriteAnimation>("Qianghua/shengxingtexiao");
    }
    protected override void OnOpen(object param = null)
    {
        WeaponModule weamodule = ModuleManager.Instance.FindModule<WeaponModule>();
        if (null == weamodule)
            return;

        weamodule.SetTabIndex(0);
        mOldSelectedTab = mTabIndex[0];

        EventSystem.Instance.addEventListener(ProceedsEvent.PROCEEDS_CHANGE_ALL, UpdateGameHandler);
        EventSystem.Instance.addEventListener(ProceedsEvent.PROCEEDS_CHANGE_ALL, WeaponStrenHandler);
        EventSystem.Instance.addEventListener(ItemEvent.WEAPON_CHANGE, EquipWeaponHandler);
        EventSystem.Instance.addEventListener(ItemEvent.UPDATE_CHANGE, WeaponGridUIHandler);
        EventSystem.Instance.addEventListener(WeaponCultivateEvent.STRENGTH_CHANGE, StrenHandler);
        EventSystem.Instance.addEventListener(WeaponCultivateEvent.FITTING_CHANGE, WeaponFittHandler);
        EventSystem.Instance.addEventListener(WeaponCultivateEvent.TAB_INDEX, SetTabIndex);
        EventSystem.Instance.addEventListener(ItemEvent.WEAPON_PROMOTE, WeaponPromoteHandler);
        UIEventListener.Get(mBtnWeaponSkill.gameObject).onPress = OnPressWeaponSkill;

        InitUI();

        Player player = PlayerController.Instance.GetControlObj() as Player;
        if (player != null)
        {
            PlayerPropertyModule module = ModuleManager.Instance.FindModule<PlayerPropertyModule>();
            //int[] equips = new int[] { -1, -1, 1, -1, -1, -1 };
            mPreview.SetupCharacter(player.ModelID, module.GetEquipConfigs(), -1/*player.GetEquipWing()*/, uint.MaxValue);

            PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();

            mPreview.ChangeWeapon(pdm.GetMainWeaponId());
        }

    }
    protected override void OnClose()
    {

        if (mPreview != null)
            mPreview.Enable = false;
        if (mWeaponPreview != null)
            mWeaponPreview.Enable = false;

        EventSystem.Instance.removeEventListener(ProceedsEvent.PROCEEDS_CHANGE_ALL, UpdateGameHandler);
        EventSystem.Instance.removeEventListener(ProceedsEvent.PROCEEDS_CHANGE_ALL, WeaponStrenHandler);
        EventSystem.Instance.removeEventListener(ItemEvent.WEAPON_CHANGE, EquipWeaponHandler);
        EventSystem.Instance.removeEventListener(ItemEvent.UPDATE_CHANGE, WeaponGridUIHandler);
        EventSystem.Instance.removeEventListener(WeaponCultivateEvent.STRENGTH_CHANGE, StrenHandler);
        EventSystem.Instance.removeEventListener(WeaponCultivateEvent.FITTING_CHANGE, WeaponFittHandler);
        EventSystem.Instance.removeEventListener(WeaponCultivateEvent.TAB_INDEX, SetTabIndex);
        EventSystem.Instance.removeEventListener(ItemEvent.WEAPON_PROMOTE, WeaponPromoteHandler);

        EventDelegate.Remove(mBuyButton.onClick, OnBuyHandler);
        EventDelegate.Remove(mEquipMainButton.onClick, OnEquipWeaponHandler);
        EventDelegate.Remove(mEquipSubButton.onClick, OnEquipSubWeaponHandler);

        EventDelegate.Remove(mStrenButton.onClick, OnStrenHandler);
        EventDelegate.Remove(mPromoteBtn.onClick, OnPromoteHandler);
        EventDelegate.Remove(mPUpBtn.onClick, OnPSUpHandler);
        //EventDelegate.Add(mPUpBtnAni.onClick, OnPSUpHandler);
        EventDelegate.Remove(mPDownBtn.onClick, OnPSDownHandler);
        //EventDelegate.Add(mPDownBtnAni.onClick, OnPSDownHandler);
        EventDelegate.Remove(mFYes.onClick, OnFYesHandler);
        EventDelegate.Remove(mBtnFitNow.onClick, OnBtnFitNowHandler);
        EventDelegate.Remove(mBtnFitNext.onClick, OnBtnFitNextHandler);
        EventDelegate.Remove(mBtnReturn.onClick, OnBtnReturnHandler);
        EventDelegate.Remove(mBtnStrenItemInfo.onClick, OnBtnStrenItemInfoHandler);
        EventDelegate.Remove(mBtnPromoteItem1.onClick, OnBtnPromoteItemInfo1Handler);
        EventDelegate.Remove(mBtnPromoteItem2.onClick, OnBtnPromoteItemInfo2Handler);
        EventDelegate.Remove(mBtnPeijianItem.onClick, OnBtnPeijianItemInfoHandler);
    }
    public override void Update(uint elapsed)
    {
        if (!mPreveiwSprite.gameObject.activeInHierarchy /*&& !mPreveiwSpriteStren.gameObject.activeInHierarchy*/)
        {
            mPreview.Enable = false;
        }
        else if (!mPreview.Enable)
        {
            mPreview.Enable = true;
        }
        if (mWeaponPreview.Enable != mPWeaponIcon.gameObject.activeInHierarchy)
        {
            mWeaponPreview.Enable = mPWeaponIcon.gameObject.activeInHierarchy;
        }

        mPreview.Update();
        mWeaponPreview.Update();

        

    }

    private void InitUI()
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        EventDelegate.Add(mBuyButton.onClick, OnBuyHandler);
        EventDelegate.Add(mEquipMainButton.onClick, OnEquipWeaponHandler);
        EventDelegate.Add(mEquipSubButton.onClick, OnEquipSubWeaponHandler);

        EventDelegate.Add(mStrenButton.onClick, OnStrenHandler);
        EventDelegate.Add(mPromoteBtn.onClick, OnPromoteHandler);
        EventDelegate.Add(mPUpBtn.onClick, OnPSUpHandler);
        //EventDelegate.Add(mPUpBtnAni.onClick, OnPSUpHandler);
        EventDelegate.Add(mPDownBtn.onClick, OnPSDownHandler);
        //EventDelegate.Add(mPDownBtnAni.onClick, OnPSDownHandler);
        EventDelegate.Add(mFYes.onClick, OnFYesHandler);
        EventDelegate.Add(mBtnFitNow.onClick, OnBtnFitNowHandler);
        EventDelegate.Add(mBtnFitNext.onClick, OnBtnFitNextHandler);
        EventDelegate.Add(mBtnReturn.onClick, OnBtnReturnHandler);

        EventDelegate.Add(mBtnStrenItemInfo.onClick, OnBtnStrenItemInfoHandler);
        EventDelegate.Add(mBtnPromoteItem1.onClick, OnBtnPromoteItemInfo1Handler);
        EventDelegate.Add(mBtnPromoteItem2.onClick, OnBtnPromoteItemInfo2Handler);
        EventDelegate.Add(mBtnPeijianItem.onClick, OnBtnPeijianItemInfoHandler);

        for (int i = 0; i < mTabIndex.Count; ++i)
        { 
            UIEventListener.Get(mTabIndex[i]).onClick = OnTabIndexSelected;
        }

        UICenterOnChild cg = mPGrid.GetComponent<UICenterOnChild>();
        if (cg != null)
        {
            cg.onFinished += GetScollGameObject;
        }

        cg = mAGrid.GetComponent<UICenterOnChild>();
        if (cg != null)
        {
            cg.onFinished += GetAScorllGameObject;
        }

        mMainEWeaponId = module.GetMainWeaponId();
        mSubEWeaponId = module.GetSubWeaponId();


        //mPresLa.text = (module.GetProceeds(ProceedsType.Money_Prestige)).ToString();

        uint lv = module.GetStrenLv();

        StrenTableItem sres = DataManager.StrenTable[(int)lv] as StrenTableItem;
        if (sres == null)
        {
            return;
        }

        uint stren_money = module.GetProceeds(ProceedsType.Money_Stren);
        if (stren_money < sres.cost)
        {
            mSCurrP.text = "[E92224]";
        }
        else
        {
            mSCurrP.text = "[FAFDF4]";
        }
        mSCurrP.text += "已有：" + (module.GetProceeds(ProceedsType.Money_Stren)).ToString();

        for (int i = 0; i < (int)FittingsType.MAX_FITTGINS; i++)
        {
            GameObject obj = mFSlot[i];
            obj.name = i.ToString();
            UIEventListener.Get(obj).onClick = SetCurrFittings;
        }

        for (int i = 0; i < (int)FittingsType.MAX_PROPERTY; ++i)
        {
            mFLockBtn[i].name = i.ToString();
            UIEventListener.Get(mFLockBtn[i].gameObject).onClick = OnFittingsLockHandler;
        }

        InitShopUI();
        InitStrenUI();
        InitPromoteUI();
        InitSkillUI();
        InitFittingUI();

        OnPSDownHandler();
        OnSkillDownHandler();


        mPreview.SetTargetSprite(mPreveiwSprite);
        //mPreview.BackgroundSprite = mBackgroundSprite;
        mPreview.SetCameraOrthographicSize(1.5f);
        mPreview.RotationY = 180;
        mPreview.Pos = new Vector3(-0.167346f, 0, 0);
    }

    #region 武器库
    //
    //
    private void InitShopUI()
    {
        SetGridShop();
        mShopGrid.repositionNow = true;

        //mGridMap = (from d in mGridMap orderby d.Key ascending select d).ToDictionary(k => k.Key, v => v.Value);
        //mGridMap = mGridMap.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);
        SetCurrShopWeapon(mGridMap[mGridMap.Keys.Min()]);
    }


    private void ResetMainWeapon()
    {
        //主手一定存在
        if (!mGridMap.ContainsKey(mMainEWeaponId))
            return;
        //quxiao
        WeaponShopGridUI grid = mGridMap[mMainEWeaponId] as WeaponShopGridUI;
        if (grid != null)
        {
            grid.SetInfo(2, "已获得", true);
        }

        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;
        mMainEWeaponId = module.GetMainWeaponId();

        if (mGridMap.ContainsKey(mOldId))
        {
            int id = mOldId;
            mOldId = -1;
            SetCurrShopWeapon(mGridMap[id]);
        }
        //zhuangbei
        grid = mGridMap[mMainEWeaponId] as WeaponShopGridUI;
        if (grid != null)
        {
            grid.SetInfo(3, "[fed514]主手", true);
        }
    }

    private void ResetSubWeapon()
    {
        WeaponShopGridUI grid = null;
        //副手不一定存在
        if (mGridMap.ContainsKey(mSubEWeaponId))
        {
            //quxiao
            grid = mGridMap[mSubEWeaponId] as WeaponShopGridUI;
            if (grid != null)
            {
                grid.SetInfo(2, "已获得", true);
            }
        }
        
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;
        mSubEWeaponId = module.GetSubWeaponId();

        if (mGridMap.ContainsKey(mOldId))
        {
            int id = mOldId;
            mOldId = -1;
            SetCurrShopWeapon(mGridMap[id]);
        }
        if (mGridMap.ContainsKey(mSubEWeaponId))
        {
            //zhuangbei
            grid = mGridMap[mSubEWeaponId] as WeaponShopGridUI;
            if (grid != null)
            {
                grid.SetInfo(3, "[fed514]副手", true);
            }
        }
        
    }

    //重置下装备标签
    private void EquipWeaponHandler(EventBase evt)
    {
        //主手一定有
        ItemEvent e = (ItemEvent)evt;
        //if( e.isSubWeapon )
        //{
            ResetSubWeapon();
        //}else
        //{
            ResetMainWeapon();
        //}
// 
//         int weaponid = mMainEWeaponId;
//         if( e.isSubWeapon )
//         {
//             weaponid = mSubEWeaponId;
//         }
// 
//         if (!mGridMap.ContainsKey(weaponid))
//             return;
//         //quxiao
//         WeaponShopGridUI grid = mGridMap[weaponid] as WeaponShopGridUI;
//         if (grid != null)
//         {
//             grid.SetInfo(2, "已获得", true);
//         }
// 
//         PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
//         if (module == null)
//             return;
// 
//         int weaponid = mMainEWeaponId;
//         if (e.isSubWeapon)
//         {
//             mMainEWeaponId = module.GetMainWeaponId();
//         }
//         mEWeaponId = module.GetMainWeaponId();
// 
//         if (mGridMap.ContainsKey(mOldId))
//         {
//             int id = mOldId;
//             mOldId = -1;
//             SetCurrShopWeapon(mGridMap[id]);
//         }
//         //zhuangbei
//         grid = mGridMap[mEWeaponId] as WeaponShopGridUI;
//         if (grid != null)
//         {
//             grid.SetInfo(3, "已装备", true);
//         }
            InitPromoteUI();

    }

    private void WeaponPromoteHandler(EventBase evt)
    {
        GetScollGameObject();
    }

    private void WeaponGridUIHandler(EventBase evt)
    {
        SetGridShop();

        if (mGridMap.ContainsKey(mOldId))
        {
            int id = mOldId;
            mOldId = -1;
            SetCurrShopWeapon(mGridMap[id]);
        }

        InitPromoteUI();
    }

    //选中当前的武器
    private void SetCurrShopWeapon(WeaponShopGridUI target)
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        if (!mGridEventMap.ContainsKey(target))
            return;

        WeaponShopGridUI grid = target;
        if (grid == null)
            return;
        if (mOldWeaponGridUI != null)
        {
            mOldWeaponGridUI.SetSelect(false);
        }

        mOldWeaponGridUI = grid;
        mOldWeaponGridUI.SetSelect(true);

        int id = mGridEventMap[target];
        if (id == mOldId)
            return;

        mOldId = id;

        mPreview.ChangeWeapon(mOldId);


        if (!DataManager.WeaponTable.ContainsKey(id))
            return;

        WeaponTableItem wres = DataManager.WeaponTable[id] as WeaponTableItem;
        if (wres == null)
            return;

        int prolv = wres.upgrade;

        WeaponObj wobj = null;
        if (mWeaponMap.ContainsKey(id))
            wobj = mWeaponMap[id] as WeaponObj;

        PrestigeTableItem pti = null;
        if (DataManager.PrestigeTable.ContainsKey(id))
        {
            pti = DataManager.PrestigeTable[id] as PrestigeTableItem;
        }

        if (wobj != null)
        {
            prolv += wobj.GetPromoteLv();


            if (mMainEWeaponId == id)
            {
                mHintSp.spriteName = "yizhuangbei";
                mHintSp.MakePixelPerfect();
                mHintSp.gameObject.SetActive(true);

                mEquipMainButton.gameObject.SetActive(false);

                if( mSubEWeaponId >= 0 )
                {
                    mEquipSubButton.gameObject.SetActive(true);
                }
                else
                {
                    mEquipSubButton.gameObject.SetActive(false);
                }

            }else if( mSubEWeaponId == id )
            {
                mHintSp.gameObject.SetActive(false);

                mEquipMainButton.gameObject.SetActive(true);
                mEquipSubButton.gameObject.SetActive(false);
            }
            else
            {
                mHintSp.gameObject.SetActive(false);
                mEquipMainButton.gameObject.SetActive(true);
                mEquipSubButton.gameObject.SetActive(true);
            }

            mBuyButton.gameObject.SetActive(false);
        }
        else
        {
            if (pti != null)
            {
                if (module.GetProceeds(ProceedsType.Money_Prestige) < pti.value)
                {
                    mHintSp.spriteName = "shengwangbuzu";
                    mHintSp.MakePixelPerfect();
                    mHintSp.gameObject.SetActive(true);
                }
                else if (module.GetProceeds(ProceedsType.Money_Game) < wres.gameprice)
                {
                    mHintSp.spriteName = "jinbibuzu";
                    mHintSp.MakePixelPerfect();
                    mHintSp.gameObject.SetActive(true);
                }
                else if (module.GetProceeds(ProceedsType.Money_RMB) < wres.diamondprice)
                {
                    mHintSp.spriteName = "diamondshortage";
                    mHintSp.MakePixelPerfect();
                    mHintSp.gameObject.SetActive(true);
                }
                else if (null != wres.itemidnum)
                {
                    string[] ss = wres.itemidnum.Split('|');
                    if (2 > ss.Length)
                    {
                        GameDebug.LogError("weapon.txt表填写错误 道具购买列 枪械ID = " + wres.id);
                        return;
                    }
                    if (module.GetItemNumByID(System.Convert.ToInt32(ss[0])) < System.Convert.ToInt32(ss[1]))
                    {
                        mHintSp.spriteName = "itemshortage";
                        mHintSp.MakePixelPerfect();
                        mHintSp.gameObject.SetActive(true);
                    }
                }
                else
                {
                    mHintSp.gameObject.SetActive(false);
                }
            }
            mBuyButton.gameObject.SetActive(!mHintSp.gameObject.activeSelf);
            mEquipMainButton.gameObject.SetActive(false);
            mEquipSubButton.gameObject.SetActive(false);
        }

        PromoteTableItem pres = DataManager.PromoteTable[prolv] as PromoteTableItem;
        if (pres == null)
        {
            GameDebug.LogError("进阶promote.txt表格无此ID=" + prolv);
            return;
        }

        mMBaseL.text = (pres.value).ToString();
        mMSpL.text = wres.firingrate.ToString() + "/S";

        mMAddL.text = wres.damageratio.ToString();

        uint strelv = module.GetStrenLv();
        StrenTableItem stres = DataManager.StrenTable[strelv] as StrenTableItem;
        if (stres == null)
        {
            GameDebug.LogError("资源ID为" + strelv + "不存在武器强化表格strength.txt中");
            return;
        }
        mMStrenL.text = (stres.value).ToString();
        mWName.text = wres.name;

        WeaponSkillTableItem skill = DataManager.WeaponSkillTable[wres.take_skill] as WeaponSkillTableItem;
        if (null == skill)
            return;
        UIAtlasHelper.SetSpriteImage(mWeaponSkillIcon, skill.icon);
        SetStren3D(id);
    }

    private void SetStren3D(int weaponId)
    {
        //搬个板凳等胖仔子豪
    }

    private void SetGridShop()
    {
        IDictionaryEnumerator itr = DataManager.PrestigeTable.GetEnumerator();
        while (itr.MoveNext())
        {
            PrestigeTableItem resp = itr.Value as PrestigeTableItem;
            InitShopWeaponGrid(resp);

        }
//         Hashtable map = DataManager.PrestigeTable;
//         foreach (PrestigeTableItem resp in map.Values)
//         {
//             InitShopWeaponGrid(resp);
//         }

    }

    //武器库各个武器
    private void InitShopWeaponGrid(PrestigeTableItem pItem)
    {
        if (pItem == null)
            return;

        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        if (!DataManager.WeaponTable.ContainsKey(pItem.weaponid))
            return;

        //if (!ItemManager.Instance.isItem(pItem.weaponid))
        //    return;

        WeaponShopGridUI grid = null;
        GameObject sp = null;

        if (mGridMap.ContainsKey(pItem.weaponid))
            grid = mGridMap[pItem.weaponid] as WeaponShopGridUI;
        if (grid == null)
        {
            sp = WindowManager.Instance.CloneGameObject(WeaponShopGridUIPrefab);
            if (sp == null)
            {
                GameDebug.LogError("equipitem0 prefab not found");
                return;
            } 

            sp.SetActive(true);
            grid = new WeaponShopGridUI(sp);
            if (grid == null)
                return;

            string bmpn = ItemManager.Instance.getItemBmp(pItem.weaponid);
            if (bmpn != null && bmpn != "")
                grid.SetIcon(bmpn);
            grid.SetSelect(false);

            grid.onClick = SetCurrShopWeapon;

            //UIEventListener.Get(sp).onClick = SetCurrShopWeapon;

            sp.name = pItem.sortid;
            sp.transform.parent = mShopGrid.transform;
            sp.transform.localScale = Vector3.one;

            mShopGrid.Reposition();
            mGridMap.Add(pItem.weaponid, grid);
            mGridEventMap.Add(grid, pItem.weaponid);
        }

        int initype = 0;
        string inss = "";
        bool color = true;
        ItemObj item = module.GetPackManager().GetItemByID(pItem.weaponid, PackageType.Pack_Weapon);
        if (item != null)
        {
            if (mMainEWeaponId == (uint)pItem.weaponid)
            {
                initype = 3;
                inss = "[fed514]主手";
            }else if( mSubEWeaponId == (uint)pItem.weaponid )
            {
                initype = 3;
                inss = "[fed514]副手";
            }
            else
            {
                initype = 2;
                inss = "已获得";
            }

            color = true;
            grid.SetInfo(initype, inss, color);

            if (mWeaponMap.ContainsKey(pItem.weaponid))
                mWeaponMap.Remove(pItem.weaponid);
            mWeaponMap.Add(pItem.weaponid, item as WeaponObj);
        }
        else
        {
            string picname = "";

            WeaponTableItem res = DataManager.WeaponTable[pItem.weaponid] as WeaponTableItem;
            if (res == null)
                return;

            //金币购买类型
            if (uint.MaxValue != (uint)res.gameprice)
            {
                uint gamei = module.GetProceeds(ProceedsType.Money_Game);
                if (gamei < (uint)res.gameprice)
                {
                    color = false;
                }
                initype = 1;
                inss = res.gameprice.ToString();
            }
            //钻石购买类型
            else if (uint.MaxValue != (uint)res.diamondprice)
            {
                uint diamond = module.GetProceeds(ProceedsType.Money_RMB);
                if (diamond < (uint)res.diamondprice)
                {
                    color = false;
                }
                initype = 4;
                inss = res.diamondprice.ToString();
            }
            //道具购买类型
            else
            { 
                string []itemidnum = res.itemidnum.Split('|');
                if (itemidnum.Length < 2)
                {
                    GameDebug.LogError("枪械道具购买类型列错误 枪械ID = " + res.id);
                    return;
                }

                uint itemnum = module.GetItemNumByID(System.Convert.ToInt32(itemidnum[0]));
                if (itemnum < System.Convert.ToUInt32(itemidnum[1]))
                {
                    color = false;
                }
                initype = 5;
                inss = itemidnum[1];
                picname = ItemManager.Instance.getItemBmp(System.Convert.ToInt32(itemidnum[0]));
            }
            
            uint pre = module.GetProceeds(ProceedsType.Money_Prestige);
            if (pre < pItem.value)
            {
                initype = 0;
                color = false;
                inss = pItem.value.ToString();
            }

            grid.SetInfo(initype, inss, color, picname);
        }
    }

    private void OnBuyHandler()
    {
        SoundManager.Instance.Play(15);
        if (!mGridMap.ContainsKey(mOldId))
            return;

        WeaponModule module = ModuleManager.Instance.FindModule<WeaponModule>();
        if (module != null)
            module.BuyWeapon(mOldId);
    }

    //装备主手
    private void OnEquipWeaponHandler()
    {
        SoundManager.Instance.Play(15);
        if (!mGridMap.ContainsKey(mOldId))
            return;

        WeaponModule module = ModuleManager.Instance.FindModule<WeaponModule>();
        if (module != null)
            module.EquipMainWeapon(mOldId);

        InitPromoteUI();

    }
    //装备副手
    private void OnEquipSubWeaponHandler()
    {
        SoundManager.Instance.Play(15);
        if (!mGridMap.ContainsKey(mOldId))
            return;

        WeaponModule module = ModuleManager.Instance.FindModule<WeaponModule>();
        if (module != null)
            module.EquipSubWeapon(mOldId);

        InitPromoteUI();
    }

    #endregion

    #region 强化
    //
    //

    private void InitStrenUI()
    {
        SetStrenUI();
    }

    private void StrenHandler(EventBase evt)
    {
        onStrenSucess();
        SetStrenUI();
    }

    private void SetStrenUI()
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        uint lv = module.GetStrenLv();
        int starlv = (int)(lv / STREN_STEP);
        if (starlv > 0 && (lv % STREN_STEP) == 0)
            starlv -= 1;
        SetStarInfo(starlv);

        StrenTableItem sres = DataManager.StrenTable[lv] as StrenTableItem;
        if (sres == null)
        {
            GameDebug.LogError("资源ID为" + lv + "不存在表格 strength.txt中 ");
            return;
        }

        mSLName.text = sres.desc;

        uint showlv = lv == 0 ? 0 : (lv % STREN_STEP == 0 ? 10 : lv % STREN_STEP);
        for (uint i = 0; i < showlv; ++i)
        {
            SetIcon(mSStar[(int)i], "common:strenth (" + (starlv + 7) + ")");
        }

        for (uint i = showlv; i < STREN_STEP; ++i)
        {
            SetIcon(mSStar[(int)i], "common:starslvback");
        }

        mSCuV.text = (sres.value).ToString();

        StrenTableItem nres = DataManager.StrenTable[lv + 1] as StrenTableItem;
        if (nres == null)
        {//满级
            mSNeV.text = StringHelper.GetString("lvfull");
            mSLNameNT.text = StringHelper.GetString("lvfull");
            mPerItemObj.SetActive(false);
            return;
        }

        mSNeV.text = (nres.value).ToString();
        mSLNameNT.text = nres.desc;

        mSCost.text = StringHelper.GetString("money_stren") + " x" + (sres.cost).ToString();
    }

    private void SetStarInfo(int starLv)
    {
        starLv++;
        StrProTableItem item = DataManager.StrProTable[starLv] as StrProTableItem;
        if (null == item)
        {
            mSOLv.text = StringHelper.GetString("opened_the_most_senior");
            //mAddproName.text = StringHelper.GetString("opened_the_most_senior");
            SetIcon(mWeaponIcon, "common:weaponlvbg2");
            for (int i = 0; i < mSOVName.Count; ++i)
            {
                mSOVName[i].text = "";
                mSOV[i].text = "";
            }
            SetIcon(mWeaponLvPic, null);
            SetIcon(mStrWeaponPic, null);
            mStrNotes1.text = "";
            mStrNotes2.text = "";
            return;
        }
        mSOLv.text = "即将开启" + StringHelper.GetString("weaponlv" + starLv) + StringHelper.GetString("add_role_pro");
        SetIcon(mWeaponLvPic, "common:weaponlv" + starLv);
        SetIcon(mWeaponIcon, "common:weaponlvbg2");
        SetIcon(mStrWeaponPic, "Equip_Atlas2:" + starLv);

        string[] descData = item.desc1.Split('|');
        mStrNotes1.text = descData[0];
        mStrNotes2.text = descData[1];
        
        int count = 0;
        if (uint.MaxValue != item.life)
        {
            mSOVName[count].text = StringHelper.GetString("lifeproname");
            mSOV[count].text = " +" + item.life.ToString();
            count++;
        }
        if (uint.MaxValue != item.damage)
        {
            mSOVName[count].text = StringHelper.GetString("damageproname");
            mSOV[count].text = " +" + item.damage.ToString();
            count++;
        }

        if (uint.MaxValue != item.defence)
        {
            mSOVName[count].text = StringHelper.GetString("defenceproname");
            mSOV[count].text = " +" + item.defence.ToString();
            count++;
        }

        if (uint.MaxValue != item.crits)
        {
            mSOVName[count].text = StringHelper.GetString("critproname");
            mSOV[count].text = " +" + item.crits.ToString();
            count++;
        }

        if (uint.MaxValue != item.energy)
        {
            mSOVName[count].text = StringHelper.GetString("energyproname");
            mSOV[count].text = " +" + item.energy.ToString();
            count++;
        }

        for (int i = count; i < mSOV.Count; ++i)
        {
            mSOVName[count].text = "";
            mSOV[count].text = "";
        }
        
    }

    private void OnStrenHandler()
    {
        SoundManager.Instance.Play(15);
        WeaponModule module = ModuleManager.Instance.FindModule<WeaponModule>();
        if (module != null)
        {
            module.StrenWeapon();
        }
    }
    #endregion

    #region 进阶
    //
    //
    private void InitPromoteUI()
    {
        foreach (WeaponObj wobj in mWeaponMap.Values)
        {
            InitPromoteGridUI(wobj);
        }
        UIGrid cg = mPGrid.GetComponent<UIGrid>();
        if (cg != null)
        {
            cg.repositionNow = true;
        }

        if (mOldProId == -1)
            SetPromoteCurrUI(mMainEWeaponId);
        else
            SetPromoteCurrUI(mOldProId);
    }

    /// <summary>
    /// 第一个return 1，最后一个return -1，其他的返回0;
    /// </summary>
    /// <returns></returns>
//     Pos IsFirstOrLast()
//     {
//         if (mPMaxId == mPCenId && mPMinId == mPCenId)
//             return Pos.FirstLast;
// 
//         if (mPMinId == mPCenId)
//             return Pos.First;
// 
//         if (mPMaxId == mPCenId)
//             return Pos.Last;
// 
//         return Pos.Med;
//     }
    
    private void OnPSUpHandler()
    {
        //upDownBtnAniHandler();

        if (mPMaxId == mPCenId)
            return;
        
        float y = mPGrid.transform.localPosition.y + 160;
        Vector3 vec = new Vector3(mPGrid.transform.localPosition.x, y, mPGrid.transform.localPosition.z);
        ButtonMoveScroll(vec);
    }

    private void OnPSDownHandler()
    {
        //upDownBtnAniHandler();

        if (mPMinId == mPCenId)
            return;

        float y = mPGrid.transform.localPosition.y - 160;
        Vector3 vec = new Vector3(mPGrid.transform.localPosition.x, y, mPGrid.transform.localPosition.z);
        ButtonMoveScroll(vec);
    }

    private void ButtonMoveScroll(Vector3 vec)
    {
        TweenPosition tp = TweenPosition.Begin(mPGrid, 0.4f, vec);
        tp.PlayForward();
        tp.AddOnFinished(MoveScrollFinish);
        //mPScoll.MoveRelative(vec);        
    }

    private void MoveScrollFinish()
    {
        UICenterOnChild cg = mPGrid.GetComponent<UICenterOnChild>();
        if (cg == null)
            return;

        cg.Recenter();
        GetScollGameObject();
    }



    private void GetScollGameObject()
    {
        UICenterOnChild cg = mPGrid.GetComponent<UICenterOnChild>();
        if (cg == null)
            return;

        GameObject wgo = cg.centeredObject;
        if (wgo == null)
        {
            UIGrid ug = mPGrid.GetComponent<UIGrid>();
            if (ug != null)
            {
                ug.repositionNow = true;
            }
            return;
        }

        int id = System.Convert.ToInt32( wgo.name );
        PromoteGridUI grid = null;
        if( mPWeaponM.ContainsKey(id) )
        {
            grid = mPWeaponM[id];
        }
        if (grid == null)
            return;

        mPCenId = grid.getWeaponId();

        if (MPOld != null)
            MPOld.ChangeUI(false);

        MPOld = grid;
        grid.ChangeUI(true);

        SetPromoteCurrUI(grid.getWeaponId());
        //upDownBtnAniHandler();
    }

    private void InitPromoteGridUI(WeaponObj wobj)
    {
        if (wobj == null)
            return;

        UIGrid cg = mPGrid.GetComponent<UIGrid>();
        if (cg == null)
            return;

        int id = wobj.GetResId();
        
        PromoteGridUI grid = null;
        if (mPWeaponM.ContainsKey(id))
            grid = mPWeaponM[id] as PromoteGridUI;
        if (grid == null)
        {
            GameObject sp = WindowManager.Instance.CloneGameObject(PromoteGridUIPrefa); //ResourceManager.Instance.LoadUI(WeaponModule.PromoteGridUIPrefabName); //预设在这里
            if (sp == null)
            {
                GameDebug.LogError("equipitem1 prefab not found");
                return;
            }

            sp.name = id.ToString();
            sp.SetActive(true);
            sp.transform.parent = cg.transform;
            sp.transform.localScale = Vector3.one;

            grid = new PromoteGridUI(sp);// sp.GetComponent<PromoteGridUI>();
            if (grid == null)
                return;

            string bmpn = ItemManager.Instance.getItemBmp(id);
            if (bmpn != null && bmpn != "")
                grid.SetIcon(bmpn);

            grid.SetWeaponId(id);
            grid.ChangeUI(false);

            if (id < mPMinId)
                mPMinId = id;

            if (id > mPMaxId)
                mPMaxId = id;
            mPWeaponM.Add(id, grid);
        }
        else
            grid.ChangeEquipedWeapon();
    }

    private void SetPromoteCurrUI(int resId)
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        WeaponObj wobj = module.GetItemByID(resId, PackageType.Pack_Weapon) as WeaponObj;
        if (wobj == null)
            return;

        //ItemDescrideTableItem wdes = wobj.GetDesc();
        //if (wdes == null)
        //    return;

        ItemTableItem res = wobj.GetRes();
        if (res == null)
            return;

        int plv = (int)wobj.GetPromoteLv();
        PromoteTableItem curpres = wobj.GetPromoteRes();
        if (curpres == null)
        {
            GameDebug.LogError("资源ID为" + plv + "不存在表格promote.txt中 ");
            return;
        }

        if (resId == mOldProId && plv == mOldProLv)
            return;

        //先清理
        if (resId != mOldProId)
        {
            //UIAtlasHelper.SetSpriteImage(mPWeaponIcon, null);
            //UIAtlasHelper.SetSpriteImage(mPWeaponIcon, res.picname);
            mOldProId = resId;
        }

        mWeaponPreview.SetupWeapon(resId);

        mOldProLv = plv;

        mPCurLv.text = plv.ToString();
        mPNexLv.text = (plv + 1).ToString();
        mPCurVal.text = curpres.value.ToString();

        UIAtlasHelper.SetSpriteImage(mPOne, null);
        UIAtlasHelper.SetSpriteImage(mPTwo, null);

        PromoteTableItem nexpres = DataManager.PromoteTable[wobj.getProtemResId() + 1] as PromoteTableItem;
        if (nexpres == null)
        {//满级            
            mPerItem0.SetActive(false);
            mPerItem1.SetActive(false);
            mPNexVal.text = StringHelper.GetString("promfull");
            return;
        }
        else
        {
            mPerItem0.SetActive(true);
            mPerItem1.SetActive(true);
        }

        mPNexVal.text = nexpres.value.ToString();

        NormalItemTableItem noritem = DataManager.NormalItemTable[curpres.item0] as NormalItemTableItem;
        if (null == noritem)
            return;

        UIAtlasHelper.SetSpriteImage(mPOne, noritem.picname);
        UIAtlasHelper.SetSpriteImage(mPOne1, noritem.picname2);
        mPromoteItem1 = noritem.id;
        mPOne2.text = noritem.picname3;

        noritem = DataManager.NormalItemTable[curpres.item1] as NormalItemTableItem;
        UIAtlasHelper.SetSpriteImage(mPTwo, noritem.picname);
        UIAtlasHelper.SetSpriteImage(mPTwo1, noritem.picname2);
        mPromoteItem2 = noritem.id;
        mPTwo2.text = noritem.picname3;

        mPOName.text = ItemManager.Instance.getItemName(curpres.item0) + " x" + curpres.num0;
        mPTName.text = ItemManager.Instance.getItemName(curpres.item1) + " x" + curpres.num1;

        uint count = module.GetPackManager().GetNumByID(curpres.item0);
        if (curpres.num0 > count)
        {
            mPOCount.text = "[E92224]";
        }
        else
        {
            mPOCount.text = "[FAFDF4]";
        }
        mPOCount.text += "已有：" + System.Convert.ToString(count);
        count = module.GetPackManager().GetNumByID(curpres.item1);
        if (curpres.num1 > count)
        {
            mPTCount.text = "[E92224]";
        }
        else
        {
            mPTCount.text = "[FAFDF4]";
        }
        mPTCount.text += "已有：" + System.Convert.ToString(count);
    }

    private void OnPromoteHandler()
    {
        SoundManager.Instance.Play(15);
        if (!mWeaponMap.ContainsKey(mOldProId))
            return;

        WeaponModule module = ModuleManager.Instance.FindModule<WeaponModule>();
        if (module != null)
            module.SetPromote(mOldProId);
    }
    #endregion

    #region 配件
    //
    //

    private void InitFittingUI()
    {
        SetFittingsUI(true);

        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        for (uint i = 0; i < (uint)FittingsType.MAX_FITTGINS; i++)
        {
            FittingsData data = module.GetFittingsData(i);
            if (data == null)
                return;

            FittposTableItem fpres = DataManager.FittposTable[i] as FittposTableItem;
            if (fpres == null)
            {
                GameDebug.LogError("资源ID为" + i + "不存在表格fittpos.txt中 ");
                return;
            }
            mFSDesc[(int)i].text = fpres.lockdesc;

            mFOpen[i] = data.IsOpen();
            if (mFOpen[i])
            {
                if (data.GetFightValue() != 0)
                {
                    //赋值战斗力
                    mFSFight[(int)i].text = "战斗力：" + data.GetFightValue().ToString();
                    mFSDBack[(int)i].gameObject.SetActive(false);
                }
                else
                {
                    mFSFight[(int)i].text = fpres.fittdesc;
                    mFSDBack[(int)i].gameObject.SetActive(false);
                }    
            }
            else
            {
                mFSFight[(int)i].text = "";
                mFSDBack[(int)i].gameObject.SetActive(true);
            }
        }
    }

    private void SetFittingsUI(bool show)
    {
        mFSUI.gameObject.SetActive(show);
        mFFUI.gameObject.SetActive(!show);
        mFYes.gameObject.SetActive(!show);
    }

    private void ClearFBtnState()
    {
        for (int i = 0; i < (int)FittingsType.MAX_PROPERTY; ++i)
        {
            mFLockBtn[i].value = false;
        }
    }

    private void SetCurrFittings(GameObject obj)
    {
        mFPos = System.Convert.ToUInt32(obj.name);

        if (!mFOpen[mFPos])
            return;

        SetFittingsUI(false);
        ClearFBtnState();
        SetFittingsPos();
    }

    private void SetFittingsPos()
    {
        WeaponModule wmodule = ModuleManager.Instance.FindModule<WeaponModule>();
        if (wmodule == null)
            return;

        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        FittingsData fdata = module.GetFittingsData(mFPos);
        if (fdata == null)
            return;

        for (int i = 0; i < (int)FittingsType.MAX_FITTGINS; i++)
        {
            if (i == (int)mFPos)
                mFPName[i].gameObject.SetActive(true);
            else
                mFPName[i].gameObject.SetActive(false);
        }

        int id = fdata.GetId();

        FittingsTableItem fres = DataManager.FittingsTable[id] as FittingsTableItem;
        if (fres == null)
        {
            GameDebug.LogError("资源ID为" + id + "不存在表格fittings.txt中 ");
            return;
        }

        if (mFId != id)
        {
            mFId = id;
            UIAtlasHelper.SetSpriteImage(mFIcon, fres.bmp, true);
            SetNextFitt();
        }

        for (int i = 0; i < 3; ++i)
        {
            int proid = -1;
            int value = -1;
            bool forbid = false;
            if (!fdata.GetProValue((uint)i, ref proid, ref value, ref forbid))
                continue;

            mFLockBtn[i].value = forbid;

            if (proid == -1)
            {
                mFProValue[i].text = "该条属性尚未洗炼！";
                continue;
            }

            string pname = RoleProperty.GetPropertyName(proid);

            int min = 0;
            int max = 1;
            if (!wmodule.GetFittMinMax(id, proid, ref min, ref max))
            {
                mFProValue[i].text = "该条属性尚未洗炼！";
                continue;
            }

            string ssss = min.ToString() + "-" + max.ToString();
            if (max == value)
                ssss = "满";

            int proindex = FittingsProperty.GetResId(proid);
            string colorss = "000000";

            IDictionaryEnumerator ide = DataManager.FittcolorTable.GetEnumerator();
            while (ide.MoveNext())
            {
                FittcolorTableItem fcres = ide.Value as FittcolorTableItem;
                if (fcres.qualityid == fres.grade && value > System.Convert.ToInt32(fcres["max_" + proindex]))
                    continue;

                colorss = fcres.color;
                break;
            }
//             Hashtable map = DataManager.FittcolorTable;
//             foreach (FittcolorTableItem fcres in map.Values)
//             {
//                 if (fcres.qualityid == fres.grade && value > System.Convert.ToInt32(fcres["max_" + proindex]))
//                     continue;
// 
//                 colorss = fcres.color;
//                 break;
//             }

            mFProValue[i].text = "[" + colorss + "]" + pname + "+" + value.ToString() + "(" + ssss + ")" + "[-]";

        }
        //洗练石是固定的图标，在画板预设中配置好就可以，不要在程序里面设置

        mFFightValue.text = fres.name + "战斗力：" + fdata.GetFightValue().ToString();

        mChancePanel.SetActive(true);
        mFittingCostPanel.SetActive(false);
        string chancess = "";
        switch (module.GetFittChance())
        {
            case 0:
                mChancePanel.SetActive(false);
                mFittingCostPanel.SetActive(true);
                break;
            case 1:
                chancess = "剩余1次免费洗炼次数";
                break;
            case 2:
                chancess = "剩余2次免费洗炼次数";
                break;
            case 3:
                chancess = "剩余3次免费洗炼次数";
                break;
            default:
                chancess = "";
                break;
        }

        mFChance.text = chancess;

        if (mFittingCostPanel.activeSelf)
        {
            SetFittingCost();
        }
    }

    //左下角提示
    private void SetNextFitt()
    {
        ChallengeModule module = ModuleManager.Instance.FindModule<ChallengeModule>();
        if (module == null)
            return;

        int maxcol = module.GetHistoryFloor();
        bool isHas = false;

        FittposTableItem fpres = DataManager.FittposTable[mFPos] as FittposTableItem;
        if (fpres == null)
            return;

        string[] lineData = fpres.col.Split('|');
        string[] descdata = fpres.coldesc.Split('|');
        string[] bmpdata = fpres.bmp.Split('|');
        for (int i = 0; i < lineData.Length; ++i)
        {
            int scol = System.Convert.ToInt32(lineData[i]);
            if (scol <= maxcol)
                continue;
            mNXPos = scol;
            //应该通过col得到爬塔表格在通过配件表获取到 todo
            UIAtlasHelper.SetSpriteImage(mFNFitt, bmpdata[i], true);

            mFNLab.text = descdata[i];
            isHas = true;
            break;
        }

        if (!isHas)
        {
            UIAtlasHelper.SetSpriteImage(mFNFitt, "");
            mFNLab.text = "";
            mFittHide1.SetActive(false);
            mFittHide2.SetActive(false);
        }
        else
        {
            mFittHide1.SetActive(true);
            mFittHide2.SetActive(true);
        }
    }

    private void OnTabIndexSelected(GameObject obj)
    {
        UIToggle utl = obj.GetComponent<UIToggle>();
        if (utl.value && !ObjectCommon.ReferenceEquals(obj, mOldSelectedTab))
        {
            mOldSelectedTab = obj;
            SoundManager.Instance.Play(5);
        }
    }

    private void OnFittingsLockHandler(GameObject obj)
    {
        UIToggle utl = obj.GetComponent<UIToggle>();
        if (utl == null)
            return;

        int lnum = 0;
        for (int i = 0; i < (int)FittingsType.MAX_PROPERTY; ++i)
        {
            if (mFLockBtn[i].value)
                lnum++;
        }

        if (lnum >= 3)
        {
            utl.value = false;
            return;
        }

        SetFittingCost();
    }

    private void SetFittingCost()
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (null == module)
            return;
        FittingsTableItem fres = DataManager.FittingsTable[mFId] as FittingsTableItem;
        if (fres == null)
        {
            return;
        }

        int lnum = 0;
        for (int i = 0; i < (int)FittingsType.MAX_PROPERTY; ++i)
        {
            if (mFLockBtn[i].value)
                lnum++;
        }
        uint cnum = 0;
        if (lnum == 0)
            cnum = fres.num_0;
        else if (lnum == 1)
            cnum = fres.num_1;
        else if (lnum == 2)
            cnum = fres.num_2;

        mFCost.text = ItemManager.Instance.getItemName(fres.costid) + "x" + cnum.ToString();

        uint mHasNum = module.GetPackManager().GetNumByID(fres.costid);
        if(cnum > mHasNum)
        {
            mFHaveNum.text = "[E92224]";
        }
        else
        {
            mFHaveNum.text = "[FAFDF4]";
        }
        mFHaveNum.text += "已有：" + mHasNum.ToString();
    }

    private void OnFYesHandler()
    {
        SoundManager.Instance.Play(15);
        WeaponModule module = ModuleManager.Instance.FindModule<WeaponModule>();
        if (module == null)
            return;

        bool[] ll = new bool[(int)FittingsType.MAX_PROPERTY];
        for (int i = 0; i < (int)FittingsType.MAX_PROPERTY; ++i)
        {
            ll[i] = mFLockBtn[i].value;
        }

        module.BapFittings(mFPos, mFId, ll);
    }

    private void WeaponFittHandler(EventBase evt)
    {
        if (!mFFUI.gameObject.activeSelf)
        {
            return;
        }

        SetFittingsPos();
    }

    #endregion

    public void UpdateGameHandler(EventBase evt)
    {
        SetGridShop();
        UpdateWeaponHandler(evt);
    }

    public void UpdateWeaponHandler(EventBase evt)
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        //mPresLa.text = module.GetProceeds(ProceedsType.Money_Prestige).ToString();
    }

    public void WeaponStrenHandler(EventBase evt)
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        uint lv = module.GetStrenLv();

        StrenTableItem sres = DataManager.StrenTable[(int)lv] as StrenTableItem;
        if (sres == null)
        {
            return;
        }

        uint stren_money = module.GetProceeds(ProceedsType.Money_Stren);
        if (stren_money < sres.cost)
        {
            mSCurrP.text = "[E92224]";
        }
        else
        {
            mSCurrP.text = "[FAFDF4]";
        }

        mSCurrP.text += "已有：" + module.GetProceeds(ProceedsType.Money_Stren).ToString();
    }

    public void SetTabIndex(EventBase evt)
    {
        WeaponModule module = ModuleManager.Instance.FindModule<WeaponModule>();
        if (module == null)
            return;

        int index = module.GetTabIndex();

        if (index < 0 || index >= mTabIndex.Count)
            return;

        UIToggle tg = mTabIndex[index].GetComponent<UIToggle>();
        
        if (tg == null)
            return;

        tg.value = true;

        module.SetTabIndex(0, false);
    }

    /*void upDownBtnAniHandler()
    {
        switch (IsFirstOrLast())
        {
            case Pos.FirstLast:
                mPDownBtnAni.gameObject.SetActive(false);
                mPUpBtnAni.gameObject.SetActive(false);
                break;
            case Pos.Last:
                mPDownBtnAni.gameObject.SetActive(true);
                mPUpBtnAni.gameObject.SetActive(false);
                break;
            case Pos.First:
                mPDownBtnAni.gameObject.SetActive(false);
                mPUpBtnAni.gameObject.SetActive(true);
                break;
            case Pos.Med:
                mPDownBtnAni.gameObject.SetActive(true);
                mPUpBtnAni.gameObject.SetActive(true);
                break;
        }

        UISpriteAnimation ani1 = mPDownBtnAni.GetComponent<UISpriteAnimation>();
        if (ani1 != null) ani1.Reset();

        UISpriteAnimation ani2 = mPUpBtnAni.GetComponent<UISpriteAnimation>();
        if (ani2 != null) ani2.Reset();
    }*/

    public void SetIcon(UISprite ui, string name)
    {
        UIAtlasHelper.SetSpriteImage(ui, name);
    }

    private void OnBtnFitNowHandler()
    {
        SoundManager.Instance.Play(15);
        ItemInfoParam param = new ItemInfoParam();

        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        FittingsData fdata = module.GetFittingsData(mFPos);
        if (fdata == null)
            return;
        param.packpos = (int)mFPos;
        param.itemid = fdata.GetId();
        param.itemtype = ItemType.Fittings;

        WindowManager.Instance.OpenUI("iteminfo", param);
        WindowManager.Instance.CloseUI("weapon");
    }

    private void OnBtnFitNextHandler()
    {
        SoundManager.Instance.Play(15);
        ItemInfoParam param = new ItemInfoParam();

        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;
        if (!DataManager.FittingsTable.ContainsKey(mNXPos))
        {
            GameDebug.LogError("fittings.txt中没有此id = " + mNXPos);
            return;
        }

        param.itemid = mNXPos;
        param.itemtype = ItemType.Fittings;
        WindowManager.Instance.OpenUI("iteminfo", param);
        WindowManager.Instance.CloseUI("weapon");
    }

    private void OnBtnReturnHandler()
    {
        SoundManager.Instance.Play(15);
        InitFittingUI();
    }

    #region 特效
    //强化成功
    void onStrenSucess()
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        uint lv = module.GetStrenLv();

        uint strenlv = lv % 10;
        if (lv > 0 && strenlv == 0)
            strenlv = 10;
        strenlv -= 1;

        mStrenSuccessAni.gameObject.transform.parent = mSStar[(int)strenlv].gameObject.transform;
        mStrenSuccessAni.transform.localPosition = new Vector3(0, 0, 0);
        mStrenSuccessAni.Reset();
        mStrenSuccessAni.gameObject.SetActive(true);
        mStrenSuccessAni.onFinished += onStrenSpriteAniFinish;
    }

    void onStrenSpriteAniFinish(GameObject go)
    {
        mStrenSuccessAni.onFinished -= onStrenSpriteAniFinish;
        mStrenSuccessAni.Reset();
        mStrenSuccessAni.gameObject.SetActive(false);
    }
    #endregion


    private void InitSkillUI()
    {
        foreach (WeaponObj wobj in mWeaponMap.Values)
        {
            InitSkillGridUI(wobj);
        }
        UIGrid cg = mAGrid.GetComponent<UIGrid>();
        if (cg != null)
        {
            cg.repositionNow = true;
        }

        if( mOldAId == -1 )
        {
            UpdateSkillInfo(mMainEWeaponId,false);
        }else
        {
            UpdateSkillInfo(mOldAId, false);
        }
// 
//         if (mOldProId == -1)
//             SetPromoteCurrUI(mMainEWeaponId);
//         else
//             SetPromoteCurrUI(mOldProId);
    }

    private void InitSkillGridUI(WeaponObj wobj)
    {
        if (wobj == null)
            return;

        UIGrid cg = mAGrid.GetComponent<UIGrid>();
        if (cg == null)
            return;

        int id = wobj.GetResId();

        PromoteGridUI grid = null;
        if (mAWeaponM.ContainsKey(id))
            grid = mAWeaponM[id] as PromoteGridUI;
        if (grid == null)
        {
            GameObject sp = WindowManager.Instance.CloneGameObject(PromoteGridUIPrefa); //ResourceManager.Instance.LoadUI(WeaponModule.PromoteGridUIPrefabName); //预设在这里
            if (sp == null)
            {
                GameDebug.LogError("equipitem1 prefab not found");
                return;
            }

            sp.name = id.ToString();
            sp.SetActive(true);
            sp.transform.parent = cg.transform;
            sp.transform.localScale = Vector3.one;

            grid = new PromoteGridUI(sp);// sp.GetComponent<PromoteGridUI>();
            if (grid == null)
                return;

            string bmpn = ItemManager.Instance.getItemBmp(id);
            if (bmpn != null && bmpn != "")
                grid.SetIcon(bmpn);

            grid.SetWeaponId(id);
            grid.ChangeUI(false);

//             if (id < mPMinId)
//                 mPMinId = id;
// 
//             if (id > mPMaxId)
//                 mPMaxId = id;
            mAWeaponM.Add(id, grid);
        }
        else
            grid.ChangeEquipedWeapon();
    }


    private void GetAScorllGameObject()
    {
        UICenterOnChild cg = mAGrid.GetComponent<UICenterOnChild>();
        if (cg == null)
            return;

        GameObject wgo = cg.centeredObject;
        if (wgo == null)
        {
            UIGrid ug = mAGrid.GetComponent<UIGrid>();
            if (ug != null)
            {
                ug.repositionNow = true;
            }
            return;
        }

        int id = System.Convert.ToInt32(wgo.name);
        PromoteGridUI grid = null;
        if (mAWeaponM.ContainsKey(id))
        {
            grid = mAWeaponM[id];
        }
        if (grid == null)
            return;

        mACenId = grid.getWeaponId();

        if (MAOld != null)
            MAOld.ChangeUI(false);

        MAOld = grid;
        grid.ChangeUI(true);

        UpdateSkillInfo(grid.getWeaponId());

        //SetPromoteCurrUI(grid.getWeaponId());
    }

    private void OnSkillUpHandler()
    {
        float y = mAGrid.transform.localPosition.y + 160;
        Vector3 vec = new Vector3(mAGrid.transform.localPosition.x, y, mAGrid.transform.localPosition.z);
        SkillButtonMoveScroll(vec);
    }

    private void OnSkillDownHandler()
    {

        float y = mAGrid.transform.localPosition.y - 160;
        Vector3 vec = new Vector3(mAGrid.transform.localPosition.x, y, mAGrid.transform.localPosition.z);
        SkillButtonMoveScroll(vec);
    }

    private void SkillButtonMoveScroll(Vector3 vec)
    {
        TweenPosition tp = TweenPosition.Begin(mAGrid, 0.4f, vec);
        tp.PlayForward();
        tp.AddOnFinished(SkillMoveScrollFinish);
        //mPScoll.MoveRelative(vec);        
    }

    private void SkillMoveScrollFinish()
    {
        UICenterOnChild cg = mAGrid.GetComponent<UICenterOnChild>();
        if (cg == null)
            return;

        cg.Recenter();
        GetAScorllGameObject();
    }

    private void UpdateSkillInfo(int weaponid, bool needTypeWriterEffect = true)
    {
        if (!DataManager.WeaponTable.ContainsKey(weaponid))
            return;

        WeaponTableItem wres = DataManager.WeaponTable[weaponid] as WeaponTableItem;

        if (!DataManager.WeaponSkillTable.ContainsKey(wres.take_skill))
        {
            return;
        }

        WeaponSkillTableItem item = DataManager.WeaponSkillTable[wres.take_skill] as WeaponSkillTableItem;
        if (item == null)
        {
            return ;
        }

        UIAtlasHelper.SetSpriteImage(mSkillIcon, item.icon);

        mSkillName.text = item.name;

        mSkillDesc.text = item.desc.Replace("\\n", "\n") + "\n" + item.desc2.Replace("\\n", "\n");

        TypewriterEffect te = null;

        if (UIEffectManager.IsUIEffectActive<TypewriterEffect>(mSkillDesc.gameObject, ref te))
        {
            if (needTypeWriterEffect)
            {
                te.ReStart();
            }
            else
            {
                te.SetProcessText();
            }
        }
        else
        {
            if (needTypeWriterEffect)
            {
                mSkillDesc.gameObject.AddMissingComponent<TypewriterEffect>();
            }
        }
    }

    private void OnBtnStrenItemInfoHandler()
    {
        SoundManager.Instance.Play(15);
        ItemInfoParam param = new ItemInfoParam();
        //能源核心-写死的，看策划
        param.itemid = 1000003;

        WindowManager.Instance.OpenUI("iteminfo", param);
    }

    private void OnBtnPromoteItemInfo1Handler()
    {
        SoundManager.Instance.Play(15);
        ItemInfoParam param = new ItemInfoParam();
        param.itemid = mPromoteItem1;
        WindowManager.Instance.OpenUI("iteminfo", param);
    }

    private void OnBtnPromoteItemInfo2Handler()
    {
        SoundManager.Instance.Play(15);
        ItemInfoParam param = new ItemInfoParam();
        param.itemid = mPromoteItem2;
        WindowManager.Instance.OpenUI("iteminfo", param);
    }

    private void OnBtnPeijianItemInfoHandler()
    {
        SoundManager.Instance.Play(15);
        ItemInfoParam param = new ItemInfoParam();
        //洗练石-写死的，看策划
        param.itemid = 1000005;
        WindowManager.Instance.OpenUI("iteminfo", param);
    }

    private void OnPressWeaponSkill(GameObject target, bool isPressed)
    {
        if (isPressed)
        {
            mInfo.SetActive(false);
            mVal.SetActive(false);
            mSkillInfo.SetActive(true);

            UILabel name = ObjectCommon.GetChildComponent<UILabel>(mSkillInfo, "name");
            UILabel desc = ObjectCommon.GetChildComponent<UILabel>(mSkillInfo, "Label");
            name.text = "";
            desc.text = "";

            WeaponTableItem item = DataManager.WeaponTable[mOldId] as WeaponTableItem;
            if (null == item)
                return;

            WeaponSkillTableItem skill = DataManager.WeaponSkillTable[item.take_skill] as WeaponSkillTableItem;
            if (null == skill)
                return;

            name.text = "[fed514]" + skill.name;
            desc.text = skill.desc;
        }
        else
        {
            mInfo.SetActive(true);
            mVal.SetActive(true);
            mSkillInfo.SetActive(false);
        }
        //string name = "                  ";
    }
}
