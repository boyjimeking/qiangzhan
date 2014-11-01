using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using Message;
using System.Threading;

public class DefenceUIParam
{
    public int itemid = -1; //防具ID
    public PackageType packtype = PackageType.Invalid;
    public int packpos = -1;
    public int stoneid = -1;
    public int stonepos = -1;
    public int strenten = 10;
    public int risingten = 10;
    public bool isequiped = true;

    public bool isother = false;
}

public class UIDefenceForm : UIWindow
{
    //Button

    #region 初始界面
    private UIButton mUnwearButton = null;
    private UILabel mEquipButton = null;
    private UIButton mRisingstarBtn = null;
    private UIButton mPromoteBtn = null;
    private UIButton mStrenBtn = null;
    private UIButton mStoneBtn = null;
    private UIButton mSaleBtn = null;
    #endregion

    #region 升星
    private UIButton mRisingBtn = null;
    private UIButton mRisingtensBtn = null;
    #endregion

    #region 强化界面
    private UIButton mStrenButton = null;
    private UIButton mStrenTensButton = null;
    #endregion

    #region 升阶界面
    private UIButton mPromoteButton = null;
    #endregion

    #region 宝石界面
    //BtnDisplayPanel
    private UIButton mBtnElemente1 = null;
    private UIButton mBtnElemente2 = null;
    private UIButton mBtnElemente3 = null;
    private UIButton mBtnElemente4 = null;

    private UISprite mElementBack1 = null;
    private UISprite mElementBack2 = null;
    private UISprite mElementBack3 = null;
    private UISprite mElementBack4 = null;

    private UISprite mCanPromote1 = null;
    private UISprite mCanPromote2 = null;
    private UISprite mCanPromote3 = null;
    private UISprite mCanPromote4 = null;

    private UISprite mStonePic1 = null;
    private UISprite mStonePic2 = null;
    private UISprite mStonePic3 = null;
    private UISprite mStonePic4 = null;
    #endregion

    #region 宝石升级
    private UIButton mBtncomblv = null;
    #endregion

    #region 宝石镶嵌2
    private UIButton mBtnuninlay = null;
    private UIButton mBtncomb = null;
    #endregion

    #region 宝石镶嵌界面1
    private UIButton mBtnReturn = null;
    private UIButton mBtnInlay = null;
    #endregion

    //~Btn
    #region 装备属性界面
    private UILabel mWearlabel = null;
    private UILabel mDefencename = null;
    private UILabel mUselevel = null;
    private UILabel mWearpos = null;
    private UISprite mDefencepic = null;
    private UILabel mStrenlv = null;
    private UILabel mStarslv = null;
    private UISprite[] mStarsRankArr = new UISprite[(int)STARS_RANK.MAX_STARS_RANK_NUMBER];
    private UISprite[] mStoneRankArr = new UISprite[(int)STONE_RANK.MAX_STONE_RANK_NUMBER];
    private UILabel mSaleMoney = null;
    private UILabel mFightValue = null;
    private UISprite mStarsPic = null;
    private UISprite mInitStonePic = null;
    private UILabel mStarsLabel = null;
    #endregion

    #region 宝石镶嵌装备属性界面1
    private UILabel mStoneWearlabel = null;
    private UILabel mStoneDefencename = null;
    private UILabel mStoneUselevel = null;
    private UILabel mStoneWearpos = null;
    private UISprite mStoneDefencepic = null;
    private UILabel mStoneStrenlv = null;
    private UILabel mStoneStarslv = null;
    private UISprite[] mStoneStarsRankArr = new UISprite[(int)STARS_RANK.MAX_STARS_RANK_NUMBER];
    private UISprite[] mStoneStoneRankArr = new UISprite[(int)STONE_RANK.MAX_STONE_RANK_NUMBER];
    private UILabel mStoneSaleMoney = null;
    private UILabel mStoneFightValue = null;
    private UISprite mStoneStarsPic = null;
    private UILabel mStoneStarsLabel = null;
    private UISprite mStoneStonePic = null;
    #endregion

    #region 宝石镶嵌装备属性界面2
    private UISprite mStonePic = null;
    private UILabel mStoneName = null;
    private UILabel mStoneDesc = null;
    private UILabel mProidandvalue = null;
    private UILabel mStoneNum = null;
    #endregion

    #region 宝石升级界面
    private UISprite mStoneDemandPic = null;
    private UILabel mStoneDemandName = null;
    private UILabel mStoneDemandPro = null;
    private UILabel mStoneDemandMoney = null;
    private UILabel mStoneDemandNum = null;

    #endregion

    #region 强化、升星材料需求界面
    private UILabel mOddsTitle = null;
    private UILabel mOdds = null;
    private UILabel mConsumeTitle = null;
    private UILabel mConsume = null;
    private UILabel mNotes = null;
    #endregion

    #region 升阶界面
    private UILabel mPromoteWearlabel = null;
    private UILabel mPromoteDefencename = null;
    private UILabel mPromoteUselevel = null;
    private UILabel mPromoteWearpos = null;
    private UISprite mPromoteDefencepic = null;
    private UILabel mPromoteStrenlv = null;
    private UILabel mPromoteStarslv = null;
    private UISprite[] mPromoteStarsRankArr = new UISprite[(int)STARS_RANK.MAX_STARS_RANK_NUMBER];
    private UISprite[] mPromoteStoneRankArr = new UISprite[(int)STONE_RANK.MAX_STONE_RANK_NUMBER];
    private UILabel mPromoteSaleMoney = null;
    private UILabel mPromoteFightValue = null;
    private UILabel mFightvalueRised = null;
    private UISprite mPromoteStonePic = null;
    private UISprite mPromoteStarsPic = null;
    private UILabel mPromoteStarsLabel = null;
    #endregion


    #region GameObject
    private GameObject mInitButtonPanel = null;
    private GameObject mRisingButtonPanel = null;
    private GameObject mEquipback = null;
    private GameObject mDemand = null;
    private GameObject mPromotePicPanel = null;
    private GameObject mPromoteToPanel = null;
    private GameObject mStrenButtonPanel = null;
    private GameObject mPromoteButtonPanel = null;
    private GameObject mStonePanel = null;
    private GameObject mStoneDisplayPanel = null;
    private GameObject mStoneDemandPanel = null;
    private GameObject mDefenceinfoPanel = null;
    private GameObject mStoneInlayPanel1 = null;
    private GameObject mStoneInlayPanel2 = null;

    private GameObject mElement1Panel = null;
    private GameObject mElement2Panel = null;
    private GameObject mElement3Panel = null;
    private GameObject mElement4Panel = null;

    private List<GameObject> mStonePanelList1 = new List<GameObject>();
    private List<GameObject> mStonePanelList2 = new List<GameObject>();
    private List<GameObject> mStonePanelList3 = new List<GameObject>();
    #endregion

    #region iii
    private GameObject mInitScrollPanel = null;
    private GameObject mNTScrollPanel = null;
    private GameObject mPropertyUI = null;
    private UIGrid mItemsGrid = null;
    private UIGrid mNTItemsGrid = null;
    private GameObject mPropertyValueUI = null;
    private GameObject mStoneProUI = null;

    private UIScrollView mScrollpProPromote = null;
    private UIGrid mPromoteItemsGrid = null;

    private UIScrollView mScrollProStoneDefenceInfo = null;
    private UIGrid mStoneDefenceItemsGrid = null;
    private UIGrid mStoneInlayItemsGrid = null;
    private UIScrollView mStoneInlayItemScroll = null;

    private UILabel mEquipOrUnEquip = null;

    private DefenceUIParam mParam = null;

    private Dictionary<int, GameObject> mStoneItemList = new Dictionary<int, GameObject>();
    private List<StoneTableItem> mStoneList = new List<StoneTableItem>();

    private UISprite mEquipInfo = null;

    private UISprite mOldSelect = null;
    private UISprite mOldStoneSelect = null;
    #endregion

    #region 特效
    private UISpriteAnimation mCombSuccessAni = null;
    private UISpriteAnimation mDisPlayCombSuccessAni = null;
    #endregion

    public UIDefenceForm()
    {

    }
    protected override void OnLoad()
    {
        base.OnLoad();

        //Button
        #region 初始界面
        mUnwearButton = this.FindComponent<UIButton>("InitButtonPanel/btnunwear");
        mEquipButton = this.FindComponent<UILabel>("InitButtonPanel/btnunwear/Label");
        mRisingstarBtn = this.FindComponent<UIButton>("InitButtonPanel/btnrisingstar");
        mPromoteBtn = this.FindComponent<UIButton>("InitButtonPanel/btnpromote");
        mStrenBtn = this.FindComponent<UIButton>("InitButtonPanel/btnstren");
        mStoneBtn = this.FindComponent<UIButton>("InitButtonPanel/btnstone");
        mSaleBtn = this.FindComponent<UIButton>("InitButtonPanel/btnsale");
        #endregion

        #region 升星界面
        mRisingBtn = this.FindComponent<UIButton>("RisingButtonPanel/btnrising");
        mRisingtensBtn = this.FindComponent<UIButton>("RisingButtonPanel/btnrisingtens");
        #endregion

        #region 强化界面
        mStrenButton = this.FindComponent<UIButton>("StrenButtonPanel/btnstren");
        mStrenTensButton = this.FindComponent<UIButton>("StrenButtonPanel/btnstrentens");
        #endregion

        #region 升阶界面
        mPromoteButton = this.FindComponent<UIButton>("PromoteToPanel/PromoteButtonPanel/btnpromote");
        #endregion

        #region 宝石界面
        //BtnDisplayPanel
        mBtnElemente1 = this.FindComponent<UIButton>("StonePanel/StoneDisplayPanel/Sprite/element1");
        mBtnElemente2 = this.FindComponent<UIButton>("StonePanel/StoneDisplayPanel/Sprite/element2");
        mBtnElemente3 = this.FindComponent<UIButton>("StonePanel/StoneDisplayPanel/Sprite/element3");
        mBtnElemente4 = this.FindComponent<UIButton>("StonePanel/StoneDisplayPanel/Sprite/element4");

        mElementBack1 = this.FindComponent<UISprite>("StonePanel/StoneDisplayPanel/Sprite/element1/elementback");
        mElementBack2 = this.FindComponent<UISprite>("StonePanel/StoneDisplayPanel/Sprite/element2/elementback");
        mElementBack3 = this.FindComponent<UISprite>("StonePanel/StoneDisplayPanel/Sprite/element3/elementback");
        mElementBack4 = this.FindComponent<UISprite>("StonePanel/StoneDisplayPanel/Sprite/element4/elementback");
        #endregion

        #region 宝石升级
        mBtncomblv = this.FindComponent<UIButton>("StonePanel/StoneDemandPanel/Sprite/btncomb");
        #endregion

        #region 宝石镶嵌2
        mBtnuninlay = this.FindComponent<UIButton>("StonePanel/StoneInlayPanel2/Sprite/btnuninlay");
        mBtncomb = this.FindComponent<UIButton>("StonePanel/StoneInlayPanel2/Sprite/btncomb");
        #endregion

        #region 宝石镶嵌界面1
        mBtnReturn = this.FindComponent<UIButton>("StonePanel/StoneInlayPanel1/Sprite/btnreturn");
        mBtnInlay = this.FindComponent<UIButton>("StonePanel/StoneInlayPanel1/Sprite/btninlay");
        #endregion 
        //~Btn
        #region 装备属性界面
        mWearlabel = this.FindComponent<UILabel>("EquipbackPanel/wearlable");
        mDefencename = this.FindComponent<UILabel>("EquipbackPanel/equipname");
        mUselevel = this.FindComponent<UILabel>("EquipbackPanel/uselevel");
        mWearpos = this.FindComponent<UILabel>("EquipbackPanel/wearpos");
        mDefencepic = this.FindComponent<UISprite>("EquipbackPanel/equippic/Sprite");
        mStrenlv = this.FindComponent<UILabel>("EquipbackPanel/equippic/Sprite/StrenpicPanel1/strenlv");
        mStarslv = this.FindComponent<UILabel>("EquipbackPanel/starslevel");
        mStarsPic = this.FindComponent<UISprite>("EquipbackPanel/equippic/Sprite/StarsPicPanel1/starspic");
        mInitStonePic = this.FindComponent<UISprite>("EquipbackPanel/equippic/Sprite/StrenpicPanel1/strenpic");
        mSaleMoney = this.FindComponent<UILabel>("EquipbackPanel/salemoney/Label");
        mFightValue = this.FindComponent<UILabel>("EquipbackPanel/fightvalue/Label");
        mStarsLabel = this.FindComponent<UILabel>("EquipbackPanel/Label");

        for (int i = 0; i < (int)STARS_RANK.MAX_STARS_RANK_NUMBER; ++i)
        {
            mStarsRankArr[i] = this.FindComponent<UISprite>("EquipbackPanel/stars" + (i + 1));
        }

        for (int i = 0; i < (int)STONE_RANK.MAX_STONE_RANK_NUMBER; ++i)
        {
            mStoneRankArr[i] = this.FindComponent<UISprite>("EquipbackPanel/stone" + (i + 1) + "/Sprite");
        }
        #endregion

        #region 强化、升星材料需求界面
        mOddsTitle = this.FindComponent<UILabel>("DemandPanel/odds1");
        mOdds = this.FindComponent<UILabel>("DemandPanel/odds2");
        mConsumeTitle = this.FindComponent<UILabel>("DemandPanel/consume1");
        mConsume = this.FindComponent<UILabel>("DemandPanel/consume2");
        mNotes = this.FindComponent<UILabel>("DemandPanel/noteslabel");
        #endregion

        #region 升阶界面
        mPromoteWearlabel = this.FindComponent<UILabel>("PromoteToPanel/wearlable");
        mPromoteDefencename = this.FindComponent<UILabel>("PromoteToPanel/equipname");
        mPromoteUselevel = this.FindComponent<UILabel>("PromoteToPanel/uselevel");
        mPromoteWearpos = this.FindComponent<UILabel>("PromoteToPanel/wearpos");
        mPromoteDefencepic = this.FindComponent<UISprite>("PromoteToPanel/equippic/Sprite");
        mPromoteStrenlv = this.FindComponent<UILabel>("PromoteToPanel/equippic/Sprite/StrenpicPanel2/strenlv");
        mPromoteStarslv = this.FindComponent<UILabel>("PromoteToPanel/starslevel");
        mPromoteSaleMoney = this.FindComponent<UILabel>("PromoteToPanel/salemoney/Label");
        mPromoteFightValue = this.FindComponent<UILabel>("PromoteToPanel/fightvalue/Label");
        mFightvalueRised = this.FindComponent<UILabel>("PromoteToPanel/fightvalueRised");
        mPromoteStarsPic = this.FindComponent<UISprite>("PromoteToPanel/equippic/Sprite/StarsPicPanel2/starspic");
        mPromoteStonePic = this.FindComponent<UISprite>("PromoteToPanel/equippic/Sprite/StrenpicPanel2/strenpic");
        mPromoteStarsLabel = this.FindComponent<UILabel>("PromoteToPanel/Label");

        for (int i = 0; i < (int)STARS_RANK.MAX_STARS_RANK_NUMBER; ++i)
        {
            mPromoteStarsRankArr[i] = this.FindComponent<UISprite>("PromoteToPanel/stars" + (i + 1));
        }

        for (int i = 0; i < (int)STONE_RANK.MAX_STONE_RANK_NUMBER; ++i)
        {
            mPromoteStoneRankArr[i] = this.FindComponent<UISprite>("PromoteToPanel/stone" + (i + 1) + "/Sprite");
        }
        #endregion

        #region 宝石界面
        mCanPromote1 = this.FindComponent<UISprite>("StonePanel/StoneDisplayPanel/Sprite/element1/Sprite");
        mCanPromote2 = this.FindComponent<UISprite>("StonePanel/StoneDisplayPanel/Sprite/element2/Sprite");
        mCanPromote3 = this.FindComponent<UISprite>("StonePanel/StoneDisplayPanel/Sprite/element3/Sprite");
        mCanPromote4 = this.FindComponent<UISprite>("StonePanel/StoneDisplayPanel/Sprite/element4/Sprite");

        mStonePic1 = this.FindComponent<UISprite>("StonePanel/StoneDisplayPanel/Sprite/element1/stonepic");
        mStonePic2 = this.FindComponent<UISprite>("StonePanel/StoneDisplayPanel/Sprite/element2/stonepic");
        mStonePic3 = this.FindComponent<UISprite>("StonePanel/StoneDisplayPanel/Sprite/element3/stonepic");
        mStonePic4 = this.FindComponent<UISprite>("StonePanel/StoneDisplayPanel/Sprite/element4/stonepic");
        #endregion

        #region 宝石镶嵌装备信息界面1
        mStoneWearlabel = this.FindComponent<UILabel>("StonePanel/DefenceinfoPanel/EquipbackPanel/wearlable");
        mStoneDefencename = this.FindComponent<UILabel>("StonePanel/DefenceinfoPanel/EquipbackPanel/equipname");
        mStoneUselevel = this.FindComponent<UILabel>("StonePanel/DefenceinfoPanel/EquipbackPanel/uselevel");
        mStoneWearpos = this.FindComponent<UILabel>("StonePanel/DefenceinfoPanel/EquipbackPanel/wearpos");
        mStoneDefencepic = this.FindComponent<UISprite>("StonePanel/DefenceinfoPanel/EquipbackPanel/equippic/Sprite");
        mStoneStrenlv = this.FindComponent<UILabel>("StonePanel/DefenceinfoPanel/EquipbackPanel/equippic/Sprite/StrenpicPanel1/strenlv");
        mStoneStarslv = this.FindComponent<UILabel>("StonePanel/DefenceinfoPanel/EquipbackPanel/starslevel");
        mStoneSaleMoney = this.FindComponent<UILabel>("StonePanel/DefenceinfoPanel/EquipbackPanel/salemoney/Label");
        mStoneFightValue = this.FindComponent<UILabel>("StonePanel/DefenceinfoPanel/EquipbackPanel/fightvalue/Label");
        mStoneStarsPic = this.FindComponent<UISprite>("StonePanel/DefenceinfoPanel/EquipbackPanel/equippic/Sprite/StarsPicPanel1/starspic");
        mStoneStonePic = this.FindComponent<UISprite>("StonePanel/DefenceinfoPanel/EquipbackPanel/equippic/Sprite/StrenpicPanel1/strenpic");
        mStoneStarsLabel = this.FindComponent<UILabel>("StonePanel/DefenceinfoPanel/EquipbackPanel/Label");

        for (int i = 0; i < (int)STARS_RANK.MAX_STARS_RANK_NUMBER; ++i)
        {
            mStoneStarsRankArr[i] = this.FindComponent<UISprite>("StonePanel/DefenceinfoPanel/EquipbackPanel/stars" + (i + 1));
        }

        for (int i = 0; i < (int)STONE_RANK.MAX_STONE_RANK_NUMBER; ++i)
        {
            mStoneStoneRankArr[i] = this.FindComponent<UISprite>("StonePanel/DefenceinfoPanel/EquipbackPanel/stone" + (i + 1) + "/Sprite");
        }
        #endregion

        #region 宝石镶嵌界面2
        mStonePic = this.FindComponent<UISprite>("StonePanel/StoneInlayPanel2/Sprite/stonepic/Sprite");
        mStoneName = this.FindComponent<UILabel>("StonePanel/StoneInlayPanel2/Sprite/stonename");
        mStoneDesc = this.FindComponent<UILabel>("StonePanel/StoneInlayPanel2/Sprite/stonedesc");
        mProidandvalue = this.FindComponent<UILabel>("StonePanel/StoneInlayPanel2/Sprite/proidandvalue");
        #endregion

        #region 宝石升级界面
        mStoneDemandPic = this.FindComponent<UISprite>("StonePanel/StoneDemandPanel/Sprite/stonepic/Sprite"); ;
        mStoneDemandName = this.FindComponent<UILabel>("StonePanel/StoneDemandPanel/Sprite/stonename");
        mStoneDemandPro = this.FindComponent<UILabel>("StonePanel/StoneDemandPanel/Sprite/pronameandvalue");
        mStoneDemandMoney = this.FindComponent<UILabel>("StonePanel/StoneDemandPanel/Sprite/moneyusednum");
        mStoneDemandNum = this.FindComponent<UILabel>("StonePanel/StoneDemandPanel/Sprite/stoneusednum");
        #endregion

        #region GameObject
        mInitButtonPanel = this.FindChild("InitButtonPanel");
        mRisingButtonPanel = this.FindChild("RisingButtonPanel");
        mEquipback = this.FindChild("EquipbackPanel");
        mDemand = this.FindChild("DemandPanel");
        mPromotePicPanel = this.FindChild("PromoteToPanel/PromotePicPanel");
        mPromoteToPanel = this.FindChild("PromoteToPanel");
        mStrenButtonPanel = this.FindChild("StrenButtonPanel");
        mPromoteButtonPanel = this.FindChild("PromoteToPanel/PromoteButtonPanel");
        mStonePanel = this.FindChild("StonePanel");
        mStoneDisplayPanel = this.FindChild("StonePanel/StoneDisplayPanel");
        mStoneDemandPanel = this.FindChild("StonePanel/StoneDemandPanel");
        mDefenceinfoPanel = this.FindChild("StonePanel/DefenceinfoPanel");
        mStoneInlayPanel1 = this.FindChild("StonePanel/StoneInlayPanel1");
        mStoneInlayPanel2 = this.FindChild("StonePanel/StoneInlayPanel2");

        mElement1Panel = this.FindChild("StonePanel/StoneDisplayPanel/Sprite/element1");
        mElement2Panel = this.FindChild("StonePanel/StoneDisplayPanel/Sprite/element2");
        mElement3Panel = this.FindChild("StonePanel/StoneDisplayPanel/Sprite/element3");
        mElement4Panel = this.FindChild("StonePanel/StoneDisplayPanel/Sprite/element4");

        for (int i = 1; i <= 4; ++i)
        {
            mStonePanelList1.Add(this.FindChild("EquipbackPanel/stone" + i));
        }

        for (int i = 1; i <= 4; ++i)
        {
            mStonePanelList2.Add(this.FindChild("PromoteToPanel/stone" + i));
        }

        for (int i = 1; i <= 4; ++i)
        {
            mStonePanelList3.Add(this.FindChild("StonePanel/DefenceinfoPanel/EquipbackPanel/stone" + i));
        }
        #endregion

        mInitScrollPanel = this.FindChild("EquipbackPanel/Scroll View");
        mNTScrollPanel = this.FindChild("EquipbackPanel/NTPanel/Scroll View");
        mItemsGrid = this.FindComponent<UIGrid>("EquipbackPanel/Scroll View/UIGrid");
        mNTItemsGrid = this.FindComponent<UIGrid>("EquipbackPanel/NTPanel/Scroll View/UIGrid");
        mPropertyUI = this.FindChild("Items/PropertyUI");
        mPropertyValueUI = this.FindChild("Items/PropertyValueUI");
        mStoneProUI = this.FindChild("Items/StoneProUI");


        mScrollpProPromote = this.FindComponent<UIScrollView>("PromoteToPanel/Scroll View");
        mPromoteItemsGrid = this.FindComponent<UIGrid>("PromoteToPanel/Scroll View/UIGrid");

        mScrollProStoneDefenceInfo = this.FindComponent<UIScrollView>("StonePanel/DefenceinfoPanel/EquipbackPanel/Scroll View");
        mStoneDefenceItemsGrid = this.FindComponent<UIGrid>("StonePanel/DefenceinfoPanel/EquipbackPanel/Scroll View/UIGrid");
        mStoneInlayItemsGrid = this.FindComponent<UIGrid>("StonePanel/StoneInlayPanel1/Sprite/Scroll View/UIGrid");
        mStoneInlayItemScroll = this.FindComponent<UIScrollView>("StonePanel/StoneInlayPanel1/Sprite/Scroll View");

        mEquipOrUnEquip = this.FindComponent<UILabel>("InitButtonPanel/btnunwear/Label");
        mEquipInfo = this.FindComponent<UISprite>("equipinfo");

        #region 特效
        mCombSuccessAni = this.FindComponent<UISpriteAnimation>("StonePanel/StoneDemandPanel/combtexiao");
        mDisPlayCombSuccessAni = this.FindComponent<UISpriteAnimation>("StonePanel/StoneDisplayPanel/stonerisetexiao");
        #endregion
    }
    protected override void OnOpen(object param = null)
    {
        EventSystem.Instance.addEventListener(ItemEvent.DEFENCE_STREN, DefenceStrenHandler);
        EventSystem.Instance.addEventListener(ItemEvent.DEFENCE_RISING_STARS, DefenceRisingHandler);
        EventSystem.Instance.addEventListener(ItemEvent.DEFENCE_PROMOTE, DefencePromoteHandler);
        EventSystem.Instance.addEventListener(ItemEvent.STONE_COMB, StoneCombHandler);
        EventSystem.Instance.addEventListener(ItemEvent.STONE_INLAY, StoneInlayHandler);
        EventSystem.Instance.addEventListener(ItemEvent.STONE_UNINLAY, StoneUnInlayHandler);
        EventSystem.Instance.addEventListener(ItemEvent.DEFENCE_SALE, DefenceSaleHandler);
        InitUI();
        mParam = (DefenceUIParam)param;

        
        OnOpenDefence();
        SyncDefenceInfo();

        if (mParam.packtype == PackageType.Pack_Bag)
        {
            //切换到装上按钮
            mEquipButton.text = "装  备";
        }
        else
        {
            mEquipButton.text = "卸  下";
        }
        if (mParam.isother)
        {
            mInitButtonPanel.gameObject.SetActive(false);
        }
    }

    public override void Update(uint elapsed)
    {
        
    }

    private void InitUI()
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        //Init界面
        EventDelegate.Add(mUnwearButton.onClick, OnItemEquip);
        EventDelegate.Add(mSaleBtn.onClick, OnSaleHandler);
        EventDelegate.Add(mRisingstarBtn.onClick, OnRisingHandler);
        EventDelegate.Add(mStrenBtn.onClick, OnStrenHandler);
        EventDelegate.Add(mPromoteBtn.onClick, OnPromoteHandler);
        EventDelegate.Add(mStoneBtn.onClick, OnStoneHandler);

        //强化界面
        EventDelegate.Add(mStrenButton.onClick, OnStrenEquestHandler);
        EventDelegate.Add(mStrenTensButton.onClick, OnStrenTensEquestHandler);

        //升星界面
        EventDelegate.Add(mRisingBtn.onClick, OnRiseStarsEquestHandler);
        EventDelegate.Add(mRisingtensBtn.onClick, OnRiseStarsTensEquestHandler);

        //升阶界面
        EventDelegate.Add(mPromoteButton.onClick, OnPromoteEquestHandler);

        //宝石界面
        EventDelegate.Add(mBtnElemente1.onClick, OnStoneElement1Handler);
        EventDelegate.Add(mBtnElemente2.onClick, OnStoneElement2Handler);
        EventDelegate.Add(mBtnElemente3.onClick, OnStoneElement3Handler);
        EventDelegate.Add(mBtnElemente4.onClick, OnStoneElement4Handler);

        //宝石升级
        EventDelegate.Add(mBtncomblv.onClick, OnStoneComblvHandler);

        //宝石镶嵌界面1
        EventDelegate.Add(mBtnReturn.onClick, OnStoneReturnHandler);
        EventDelegate.Add(mBtnInlay.onClick, OnStoneInlayHandler);

        //宝石镶嵌界面2
        EventDelegate.Add(mBtnuninlay.onClick, OnStoneUninlayHandler);
        EventDelegate.Add(mBtncomb.onClick, OnStoneCombHandler);
    }

    private void OnItemEquip()
    {
        SoundManager.Instance.Play(15);
        if (mParam.packtype == PackageType.Pack_Bag)
        {
            //切换到装上按钮
            mEquipButton.text = StringHelper.GetString("equip");
        }
        else
        {
            mEquipButton.text = StringHelper.GetString("unequip");
        }
        ItemEquipActionParam param = new ItemEquipActionParam();
        param.bagType = (int)mParam.packtype;
        param.bagPos = mParam.packpos;
        Net.Instance.DoAction((int)MESSAGE_ID.ID_MSG_ITEM_EQUIP, param);
    }

    private void OnSaleHandler()
    {
        SoundManager.Instance.Play(15);
        ItemUIParam param = new ItemUIParam();
        param.itemid = mParam.itemid;
        param.packpos = mParam.packpos;
        param.packtype = mParam.packtype;

        WindowManager.Instance.OpenUI("saleagain",param);
    }

    private void OnRisingHandler()
    {
        SoundManager.Instance.Play(15);
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();

        if (null == module)
            return;

        DefenceObj mData = module.GetItemByIDAndPos(mParam.itemid, mParam.packpos, mParam.packtype) as DefenceObj;

        if (null == mData)
            return;

        DefenceTableItem mDefenceItem = DataManager.DefenceTable[mParam.itemid] as DefenceTableItem;

        if (null == mDefenceItem)
            return;

        if (mData.GetStarsLv() >= mDefenceItem.starsLevelMax)
        {
            //弹窗，装备已升星到满级
            PromptUIManager.Instance.AddNewPrompt(StringHelper.GetString("rising_stars_max"));
            if (mDemand.activeSelf)
                mDemand.SetActive(false);
            if (mRisingButtonPanel.activeSelf)
                mRisingButtonPanel.SetActive(false);
            if (!mInitButtonPanel.activeSelf)
                mInitButtonPanel.SetActive(true);
            if (mNTScrollPanel.activeSelf)
                mNTScrollPanel.SetActive(false);
            if (!mInitScrollPanel.activeSelf)
                mInitScrollPanel.SetActive(true);
            return;
        }

        DefenceStarsItem mStarsItem = DataManager.DefenceStarsTable[mData.GetStarsLv() + 1] as DefenceStarsItem;
        DefenceStarsProItem mStarsProItem = DataManager.DefenceStarsProTable[mDefenceItem.starsSerialNumber + mData.GetStarsLv()] as DefenceStarsProItem;

        if (null == mStarsItem || null == mStarsProItem)
            return;

        //材料需求界面
        mOddsTitle.text = "[fed514]" + StringHelper.GetString("rising_odds_desc");
        mOdds.text = "[fed514]" + mStarsItem.odds + "%";
        mConsumeTitle.text = StringHelper.GetString("rising_consume");
        if (module.GetItemNumByID(mStarsItem.starsstoneId, PackageType.Pack_Bag) < mStarsItem.cstarsstone * mStarsProItem.scale)
            mConsume.text = "[E92224]";
        else
            mConsume.text = "[3EFF00]";
        mConsume.text += "升星石X" + mStarsItem.cstarsstone * mStarsProItem.scale;
        int lv = mDefenceItem.starsLevelMax / 10;
        mNotes.text = "[79ffdb]" + StringHelper.GetString("rising_notes").Replace("?", lv.ToString());

        mDemand.SetActive(true);
        mRisingButtonPanel.SetActive(true);
        mInitButtonPanel.SetActive(false);

        UIAtlasHelper.SetSpriteImage(mEquipInfo, "atlas_defence:defencerising");

        if (!mNTScrollPanel.activeSelf)
            mNTScrollPanel.SetActive(true);
        if (mInitScrollPanel.activeSelf)
            mInitScrollPanel.SetActive(false);

        foreach (Transform trans in mNTItemsGrid.transform)
        {
            trans.gameObject.SetActive(false);
            GameObject.Destroy(trans.gameObject);
        }
        mNTItemsGrid.transform.localPosition = new Vector3(0, 84, 0);

        mStarsProItem = DataManager.DefenceStarsProTable[mDefenceItem.starsSerialNumber + mData.GetStarsLv() + 1] as DefenceStarsProItem;
        if (null == mStarsProItem)
        {
            GameDebug.LogError("没有此升星序列 id = " + (mDefenceItem.starsSerialNumber + mData.GetStarsLv() + 1));
            return;
        }

        //属性设置
        //1.当前属性
        SetProName("nowpro", mNTItemsGrid, "[FED514]");
        SetProValue(mNTItemsGrid, "damageproname", mDefenceItem.basePropertyDamage == - 1 ? -1 : mData.GetProdamagestars());
        SetProValue(mNTItemsGrid, "defenceproname", mDefenceItem.basePropertyDefence == -1 ? -1 : mData.GetProdefencestars());
        SetProValue(mNTItemsGrid, "lifeproname", mDefenceItem.basePropertyLife == -1 ? -1 : mData.GetProlifestars());
        SetProValue(mNTItemsGrid, "critproname", mDefenceItem.basePropertyCrit == -1 ? -1 : mData.GetProcritstars());

        //2.下一级
        SetProName("nextpro", mNTItemsGrid, "[FED514]");
        SetProValue(mNTItemsGrid, "damageproname", mDefenceItem.basePropertyDamage == -1 ? -1 : mStarsProItem.property);
        SetProValue(mNTItemsGrid, "defenceproname", mDefenceItem.basePropertyDefence == -1 ? -1 : mStarsProItem.property);
        SetProValue(mNTItemsGrid, "lifeproname", mDefenceItem.basePropertyLife == -1 ? -1 : mStarsProItem.property);
        SetProValue(mNTItemsGrid, "critproname", mDefenceItem.basePropertyCrit == -1 ? -1 : mStarsProItem.property);

        mNTItemsGrid.Reposition();
    }

    private void OnStrenHandler()
    {
        SoundManager.Instance.Play(15);
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();

        if (null == module)
            return;

        DefenceObj mData = module.GetItemByIDAndPos(mParam.itemid, mParam.packpos, mParam.packtype) as DefenceObj;

        if (null == mData)
        {
            GameDebug.LogError("背包中没有此道具！");
            return;
        }

        DefenceTableItem mDefenceItem = DataManager.DefenceTable[mParam.itemid] as DefenceTableItem;

        if (null == mDefenceItem)
        {
            GameDebug.LogError("defence.txt填写错误！");
            return;
        }

        if (mData.GetStrenLv() >= mDefenceItem.strenLevelMax)
        {
            //弹窗，装备已强化到满级
            PromptUIManager.Instance.AddNewPrompt(StringHelper.GetString("strenmax"));
            if (mDemand.activeSelf)
                mDemand.SetActive(false);
            if (!mInitButtonPanel.activeSelf)
                mInitButtonPanel.SetActive(true);
            if (mStrenButtonPanel.activeSelf)
                mStrenButtonPanel.SetActive(false);
            if (mNTScrollPanel.activeSelf)
                mNTScrollPanel.SetActive(false);
            if (!mInitScrollPanel.activeSelf)
                mInitScrollPanel.SetActive(true);
            return;
        }

        DefenceStrenItem mStrenItem = DataManager.DefenceStrenTable[mData.GetStrenLv() + 1] as DefenceStrenItem;
        DefenceStrenProItem mStrenProItem = DataManager.DefenceStrenProTable[mDefenceItem.strenSerialNumber + mData.GetStrenLv()] as DefenceStrenProItem;

        if (null == mStrenItem || null == mStrenProItem)
        {
            GameDebug.LogError("defencestren.txt or defencestrenpro.txt数据填写错误！");
            return;
        }

        //材料需求界面
        mOddsTitle.text = "[fed514]" + StringHelper.GetString("stren_odds_desc");
        mOdds.text = "[fed514]" + mStrenItem.odds + "%";
        mConsumeTitle.text = StringHelper.GetString("stren_consume");
        if (module.GetProceeds(ProceedsType.Money_Game) < mStrenItem.money * mStrenProItem.ratio)
        {
            mConsume.text = "[E92224]";
        }
        else
            mConsume.text = "[3EFF00]";
        mConsume.text += "金币X" + mStrenItem.money * mStrenProItem.ratio;
        mNotes.text = "[79ffdb]" + StringHelper.GetString("stren_notes").Replace("?", mDefenceItem.strenLevelMax.ToString());

        //装备信息界面
        mStrenlv.text = "+" + mData.GetStrenLv();
        UIAtlasHelper.SetSpriteImage(mEquipInfo, "atlas_defence:defencestren");

        mDemand.SetActive(true);
        mInitButtonPanel.SetActive(false);
        mStrenButtonPanel.SetActive(true);
        if (!mNTScrollPanel.activeSelf)
            mNTScrollPanel.SetActive(true);
        if (mInitScrollPanel.activeSelf)
            mInitScrollPanel.SetActive(false);

        foreach (Transform trans in mNTItemsGrid.transform)
        {
            trans.gameObject.SetActive(false);
            GameObject.Destroy(trans.gameObject);
        }
        mNTItemsGrid.transform.localPosition = new Vector3(0, 84, 0);

        mStrenProItem = DataManager.DefenceStrenProTable[mDefenceItem.strenSerialNumber + mData.GetStrenLv() + 1] as DefenceStrenProItem;
        if (null == mStrenProItem)
        {
            GameDebug.LogError("没有此强化序列 id = " + (mDefenceItem.strenSerialNumber + mData.GetStrenLv() + 1));
            return;
        }
        //属性设置
        //1.当前属性
        SetProName("nowpro", mNTItemsGrid, "[FED514]");
        SetProValue(mNTItemsGrid, "damageproname", mDefenceItem.basePropertyDamage, mData.GetProdamagestren());
        SetProValue(mNTItemsGrid, "defenceproname", mDefenceItem.basePropertyDefence, mData.GetProdefencestren());
        SetProValue(mNTItemsGrid, "lifeproname", mDefenceItem.basePropertyLife, mData.GetProlifestren());
        SetProValue(mNTItemsGrid, "critproname", mDefenceItem.basePropertyCrit, mData.GetProcritstren());

        //2.下一级
        SetProName("nextpro", mNTItemsGrid, "[FED514]");
        SetProValue(mNTItemsGrid, "damageproname", mDefenceItem.basePropertyDamage, mStrenProItem.property);
        SetProValue(mNTItemsGrid, "defenceproname", mDefenceItem.basePropertyDefence, mStrenProItem.property);
        SetProValue(mNTItemsGrid, "lifeproname", mDefenceItem.basePropertyLife, mStrenProItem.property);
        SetProValue(mNTItemsGrid, "critproname", mDefenceItem.basePropertyCrit, mStrenProItem.property);

        mNTItemsGrid.Reposition();
    }

    private void OnPromoteHandler()
    {
        SoundManager.Instance.Play(15);
        DefenceTableItem item = DataManager.DefenceTable[mParam.itemid] as DefenceTableItem;
        if (null == item)
        {
            GameDebug.LogError("defence.txt中不存在此防具 id = " + mParam.itemid);
        }
        else if (-1 != item.combId)
        {
            mPromoteToPanel.SetActive(true);
            mInitButtonPanel.SetActive(false);
            SyncDefencePromoteInfo();
        }
        else
        {
            PromptUIManager.Instance.AddNewPrompt(StringHelper.GetString("promote_max"));
        }
    }

    private void OnStoneHandler()
    {
        SoundManager.Instance.Play(15);
        DefenceTableItem defenceitem = DataManager.DefenceTable[mParam.itemid] as DefenceTableItem;
        if (null == defenceitem)
            return;

        if (0 == defenceitem.stoneCount)
        {
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("defence_cannot_inlay_stone"));
            return;
        }
        mStoneDisplayPanel.SetActive(true);
        mDefenceinfoPanel.SetActive(true);
        mInitButtonPanel.SetActive(false);
        mDemand.SetActive(false);
        mRisingButtonPanel.SetActive(false);
        mPromoteToPanel.SetActive(false);
        mStrenButtonPanel.SetActive(false);
        mStonePanel.SetActive(true);
        mEquipback.SetActive(false);
        mStoneDemandPanel.SetActive(false);
        mStoneInlayPanel1.SetActive(false);
        mStoneInlayPanel2.SetActive(false);
        SyncStoneDefenceInfo();
        SyncStoneDisplayUIInfo();

        UIAtlasHelper.SetSpriteImage(mEquipInfo, "atlas_defence:stoneinlay");
    }

    private void OnStoneElement1Handler()
    {
        SoundManager.Instance.Play(15);
        SetIcon(mElementBack1, "atlas_defence:upelement2");
        if (mOldSelect && !ObjectCommon.ReferenceEquals(mOldSelect, mElementBack1))
        {
            SetIcon(mOldSelect, "atlas_defence:upelement1");
        }
        mOldSelect = mElementBack1;

        if (mDefenceinfoPanel.activeSelf)
        {
            mDefenceinfoPanel.SetActive(false);
        }
        //此位置有宝石 slot[1] ！= -1，则跳到宝石镶嵌2
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();

        if (null == module)
            return;

        DefenceObj mData = module.GetItemByIDAndPos(mParam.itemid, mParam.packpos, mParam.packtype) as DefenceObj;

        if (null == mData)
            return;

        stone_info stoneinfo = mData.GetStoneInfoByPos(0);
        mParam.stonepos = 0;
        if (mStoneDemandPanel.activeSelf)
        {
            mStoneDemandPanel.SetActive(false);
        }
        if (stoneinfo != null)
        {
            mParam.stoneid = stoneinfo.stoneid;
            if (mStoneInlayPanel1.activeSelf)
            {
                mStoneInlayPanel1.SetActive(false);
            }
            mStoneInlayPanel2.SetActive(true);
            SynStoneDemandUIInfo(stoneinfo);
        }
        else
        {
            if (mStoneInlayPanel2.activeSelf)
            {
                mStoneInlayPanel2.SetActive(false);
            }
            mStoneInlayPanel1.SetActive(true);
            mParam.stoneid = -1;
            SyncStoneInlayUIInfo();
        }
    }

    private void OnStoneElement2Handler()
    {
        SoundManager.Instance.Play(15);
        SetIcon(mElementBack2, "atlas_defence:upelement2");
        if (mOldSelect && !ObjectCommon.ReferenceEquals(mOldSelect, mElementBack2))
        {
            SetIcon(mOldSelect, "atlas_defence:upelement1");
        }
        mOldSelect = mElementBack2;
        if (mDefenceinfoPanel.activeSelf)
        {
            mDefenceinfoPanel.SetActive(false);
        }
        //此位置有宝石 slot[1] ！= -1，则跳到宝石镶嵌2
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();

        if (null == module)
            return;

        DefenceObj mData = module.GetItemByIDAndPos(mParam.itemid, mParam.packpos, mParam.packtype) as DefenceObj;

        if (null == mData)
            return;

        stone_info stoneinfo = mData.GetStoneInfoByPos(1);
        mParam.stonepos = 1;
        if (mStoneDemandPanel.activeSelf)
        {
            mStoneDemandPanel.SetActive(false);
        }
        if (stoneinfo != null)
        {
            mParam.stoneid = stoneinfo.stoneid;
            if (mStoneInlayPanel1.activeSelf)
            {
                mStoneInlayPanel1.SetActive(false);
            }
            mStoneInlayPanel2.SetActive(true);
            SynStoneDemandUIInfo(stoneinfo);
        }
        else
        {
            if (mStoneInlayPanel2.activeSelf)
            {
                mStoneInlayPanel2.SetActive(false);
            }
            mStoneInlayPanel1.SetActive(true);
            mParam.stoneid = -1;
            SyncStoneInlayUIInfo();
        }
    }

    private void OnStoneElement3Handler()
    {
        SoundManager.Instance.Play(15);
        SetIcon(mElementBack3, "atlas_defence:upelement2");
        if (mOldSelect && !ObjectCommon.ReferenceEquals(mOldSelect, mElementBack3))
        {
            SetIcon(mOldSelect, "atlas_defence:upelement1");
        }
        mOldSelect = mElementBack3;
        if (mDefenceinfoPanel.activeSelf)
        {
            mDefenceinfoPanel.SetActive(false);
        }
        //此位置有宝石 slot[1] ！= -1，则跳到宝石镶嵌2
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();

        if (null == module)
            return;

        DefenceObj mData = module.GetItemByIDAndPos(mParam.itemid, mParam.packpos ,mParam.packtype) as DefenceObj;

        if (null == mData)
            return;

        stone_info stoneinfo = mData.GetStoneInfoByPos(2);
        mParam.stonepos = 2;
        if (mStoneDemandPanel.activeSelf)
        {
            mStoneDemandPanel.SetActive(false);
        }
        if (stoneinfo != null)
        {
            mParam.stoneid = stoneinfo.stoneid;
            if (mStoneInlayPanel1.activeSelf)
            {
                mStoneInlayPanel1.SetActive(false);
            }
            mStoneInlayPanel2.SetActive(true);
            SynStoneDemandUIInfo(stoneinfo);
        }
        else
        {
            if (mStoneInlayPanel2.activeSelf)
            {
                mStoneInlayPanel2.SetActive(false);
            }
            mStoneInlayPanel1.SetActive(true);
            mParam.stoneid = -1;
            SyncStoneInlayUIInfo();
        }
    }

    private void OnStoneElement4Handler()
    {
        SoundManager.Instance.Play(15);
        SetIcon(mElementBack4, "atlas_defence:upelement2");
        if (mOldSelect && !ObjectCommon.ReferenceEquals(mOldSelect, mElementBack4))
        {
            SetIcon(mOldSelect, "atlas_defence:upelement1");
        }
        mOldSelect = mElementBack4;
        if (mDefenceinfoPanel.activeSelf)
        {
            mDefenceinfoPanel.SetActive(false);
        }
        //此位置有宝石 slot[1] ！= -1，则跳到宝石镶嵌2
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();

        if (null == module)
            return;

        DefenceObj mData = module.GetItemByIDAndPos(mParam.itemid, mParam.packpos, mParam.packtype) as DefenceObj;

        if (null == mData)
            return;

        stone_info stoneinfo = mData.GetStoneInfoByPos(3);
        mParam.stonepos = 3;
        if (mStoneDemandPanel.activeSelf)
        {
            mStoneDemandPanel.SetActive(false);
        }
        if (stoneinfo != null)
        {
            mParam.stoneid = stoneinfo.stoneid;
            if (mStoneInlayPanel1.activeSelf)
            {
                mStoneInlayPanel1.SetActive(false);
            }
            mStoneInlayPanel2.SetActive(true);
            SynStoneDemandUIInfo(stoneinfo);
        }
        else
        {
            if (mStoneInlayPanel2.activeSelf)
            {
                mStoneInlayPanel2.SetActive(false);
            }
            mStoneInlayPanel1.SetActive(true);
            mParam.stoneid = -1;
            SyncStoneInlayUIInfo();
        }
    }

    private void OnStoneComblvHandler()
    {
        SoundManager.Instance.Play(15);
        //判断条件，宝石合成
        DefenceModule module = ModuleManager.Instance.FindModule<DefenceModule>();

        if (null == module)
            return;

        module.StoneComb(mParam);
    }

    private void OnStoneReturnHandler()
    {
        SoundManager.Instance.Play(15);
        if (mOldStoneSelect)
            mOldStoneSelect.gameObject.SetActive(false);
        OnStoneHandler();
    }

    private void OnStoneInlayHandler()
    {
        SoundManager.Instance.Play(15);
        DefenceModule module = ModuleManager.Instance.FindModule<DefenceModule>();

        if (null == module)
            return;

        module.StoneInlay(mParam);
    }

    private void OnBtnStoneProItemClicked(GameObject target)
    {
        UISprite nowselect = ObjectCommon.GetChildComponent<UISprite>(target, "Sprite/Sprite");
        nowselect.gameObject.SetActive(true);
        SetIcon(nowselect, "atlas_defence:inlayelement1");
        if (mOldStoneSelect && !ObjectCommon.ReferenceEquals(mOldStoneSelect, nowselect))
        {
            mOldStoneSelect.gameObject.SetActive(false);
        }
        mOldStoneSelect = nowselect;
        foreach (KeyValuePair<int,GameObject> value in mStoneItemList)
        {
            if (GameObject.ReferenceEquals(value.Value, target))
            {
                mParam.stoneid = System.Convert.ToInt32(target.name);
                break;
            }
        }
    }

    private void OnStoneUninlayHandler()
    {
        SoundManager.Instance.Play(15);
        //宝石摘除
        DefenceModule module = ModuleManager.Instance.FindModule<DefenceModule>();

        if (null == module)
            return;

        PlayerDataModule playermodule = ModuleManager.Instance.FindModule<PlayerDataModule>();

        if (null == playermodule)
            return;

        DefenceObj mData = playermodule.GetItemByIDAndPos(mParam.itemid, mParam.packpos, mParam.packtype) as DefenceObj;

        if (null == mData)
            return;

        stone_info stoneinfo = mData.GetStoneInfoByPos(mParam.stonepos);
        if (null == stoneinfo)
            return;
        DeleteInlayedStone(stoneinfo);

        module.StoneUnInlay(mParam);
    }

    private void OnStoneCombHandler()
    {
        SoundManager.Instance.Play(15);
        //跳转到宝石升级界面
        if (SyncStoneCombDemandInfo())
        {
            mStoneDemandPanel.SetActive(true);
            mStoneInlayPanel2.SetActive(false);
        }
    }
    private void OnStrenEquestHandler()
    {
        SoundManager.Instance.Play(15);
        DefenceModule module = ModuleManager.Instance.FindModule<DefenceModule>();

        if (null == module)
            return;
        mParam.strenten = 0;
        module.StrenDefence(mParam);
    }

    private void OnStrenTensEquestHandler()
    {
        SoundManager.Instance.Play(15);
        DefenceModule module = ModuleManager.Instance.FindModule<DefenceModule>();

        if (null == module)
            return;
        mParam.strenten = 9;
        module.StrenDefence(mParam);
    }

    private void OnRiseStarsEquestHandler()
    {
        SoundManager.Instance.Play(15);
        DefenceModule module = ModuleManager.Instance.FindModule<DefenceModule>();

        if (null == module)
            return;

        mParam.risingten = 0;
        module.RisingStar(mParam);
    }

    private void OnRiseStarsTensEquestHandler()
    {
        SoundManager.Instance.Play(15);
        DefenceModule module = ModuleManager.Instance.FindModule<DefenceModule>();

        if (null == module)
            return;

        mParam.risingten = 9 ;
        module.RisingStar(mParam);
                
    }

    private void OnPromoteEquestHandler()
    {
        SoundManager.Instance.Play(15);
        WindowManager.Instance.OpenUI("defencepromotehint", mParam);
    }

    private void DefenceStrenHandler(EventBase evt)
    {
        OnStrenHandler();
        SyncDefenceInfo();

        if (0 != mParam.strenten)
        {
            mParam.strenten -= 1;
            DefenceModule module = ModuleManager.Instance.FindModule<DefenceModule>();

            if (null == module)
                return;
            if (module.StrenDefence(mParam))
                return;
        }
        
        Thread.Sleep(200);
    }

    private void DefenceRisingHandler(EventBase evt)
    {
        SyncDefenceInfo();
        OnRisingHandler();

        if (0 != mParam.risingten)
        {
            mParam.risingten -= 1;
            DefenceModule module = ModuleManager.Instance.FindModule<DefenceModule>();

            if (null == module)
                return;
            if (module.RisingStar(mParam))
                return;
        }
        Thread.Sleep(200);
    }

    private void DefencePromoteHandler(EventBase evt)
    {
        WindowManager.Instance.CloseUI("defencepromotehint");
        SyncDefenceInfo(true);
        OnOpenDefence();
    }

    public void StoneCombHandler(EventBase evt)
    {
        SyncStoneDisplayUIInfo();
        if (!SyncStoneCombDemandInfo())
        {
            PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();

            if (null == module)
                return;

            DefenceObj mData = module.GetItemByIDAndPos(mParam.itemid, mParam.packpos, mParam.packtype) as DefenceObj;

            if (null == mData)
                return;

            stone_info stoneinfo = mData.GetStoneInfoByPos(mParam.stonepos);

            if (null == stoneinfo)
                return;

            SynStoneDemandUIInfo(stoneinfo);
            mStoneDemandPanel.SetActive(false);
            mStoneInlayPanel2.SetActive(true);
            return;
        }

        onStoneCombSucess(mParam.stonepos);
        onCombSucess();
    }

    public void StoneInlayHandler(EventBase evt)
    {
        SyncStoneDisplayUIInfo();
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();

        if (null == module)
            return;

        DefenceObj mData = module.GetItemByIDAndPos(mParam.itemid, mParam.packpos, mParam.packtype) as DefenceObj;

        if (null == mData)
            return;

        stone_info stoneinfo = mData.GetStoneInfoByPos(mParam.stonepos);
        if (null == stoneinfo)
            return;
        DeleteInlayedStone(stoneinfo);

        onStoneCombSucess(mParam.stonepos);
        switch (mParam.stonepos)
        { 
            case 0:
                OnStoneElement1Handler();
                break;
            case 1:
                OnStoneElement2Handler();
                break;
            case 2:
                OnStoneElement3Handler();
                break;
            case 3:
                OnStoneElement4Handler();
                break;
        }
    }

    public void StoneUnInlayHandler(EventBase evt)
    {
        if (mOldStoneSelect)
            mOldStoneSelect.gameObject.SetActive(false);
        if (mOldSelect)
            SetIcon(mOldSelect, "atlas_defence:upelement1");
        OnStoneHandler();
    }

    private void OnOpenDefence()
    {
        mInitButtonPanel.SetActive(true);
        mDemand.SetActive(false);
        if (!mEquipback.activeSelf)
        {
            mEquipback.SetActive(true);
        }
        mRisingButtonPanel.SetActive(false);
        mPromoteToPanel.SetActive(false);
        mStrenButtonPanel.SetActive(false);
        mStonePanel.SetActive(false);

        if (mNTScrollPanel.activeSelf)
            mNTScrollPanel.SetActive(false);
        if (!mInitScrollPanel.activeSelf)
            mInitScrollPanel.SetActive(true);

        UIAtlasHelper.SetSpriteImage(mEquipInfo, "atlas_defence:defenceinfo");
    }

    private void SetCloseActive()
    {
        OnOpenDefence();
    }

    private void SyncDefenceInfo(bool isPromoted = false)
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();

        if (null == module)
            return;

        DefenceTableItem mDefenceItem = DataManager.DefenceTable[mParam.itemid] as DefenceTableItem;
        if (null == mDefenceItem)
        {
            GameDebug.LogError("defence.txt中没有此准备 id = " + mParam.itemid);
            return;
        }

        if (isPromoted)
        {
            DefenceCombItem mCombItem = DataManager.DefenceCombTable[mDefenceItem.combId] as DefenceCombItem;
            if (null == mCombItem)
            {
                GameDebug.LogError("defencecomb.txt中没有此序列 id = " + mDefenceItem.combId);
                return;
            }
            mDefenceItem = DataManager.DefenceTable[mCombItem.defenceproducedId] as DefenceTableItem;
            if (null == mDefenceItem)
            {
                GameDebug.LogError("defence.txt中没有此装备 id = " + mCombItem.defenceproducedId);
                return;
            }
            mParam.itemid = mCombItem.defenceproducedId;
        }

        DefenceObj mData = module.GetItemByIDAndPos(mParam.itemid, mParam.packpos ,mParam.packtype) as DefenceObj;

        bool flag = false;
        //判断是否是玩家背包中的物品，不是则隐藏Button
        if (null == mData || mParam.isother)
        {
            if (!mParam.isother)
            {
                mData = new DefenceObj();
                mData.SetSomeInfo(mDefenceItem);
            }
            else
            {
                mData = module.GetItemByIDAndPosOtherData(mParam.itemid, mParam.packpos, mParam.packtype) as DefenceObj;
            }
            flag = true;
        }

        ConfigTableItem cofig = DataManager.ConfigTable[mDefenceItem.quality] as ConfigTableItem;
        if (null == cofig)
        {
            GameDebug.LogError("没有此品质！id = " + mDefenceItem.quality);
            return;
        }
        mDefencename.text = "[" + cofig.value + "]";
        SetIcon(mDefencepic, mDefenceItem.picname);

        if (mData.GetStrenLv() < 1)
        {
            mDefencename.text += mDefenceItem.name;
            mStrenlv.gameObject.SetActive(false);
        }
        else
        {
            mDefencename.text += mDefenceItem.name + "  +" + mData.GetStrenLv();
            mStrenlv.gameObject.SetActive(true);
            mStrenlv.text = "+" + mData.GetStrenLv();
        }

        int mStarsP = mData.GetStarsLv();
        DefenceStarsItem starsitem = DataManager.DefenceStarsTable[mStarsP] as DefenceStarsItem;
        if (null == starsitem)
        {
            GameDebug.LogError("defencestars.txt没有此星等id = " + mStarsP);
            
            return;
        }
        if (mDefenceItem.starsLevelMax < 1)
        {
            for (int i = 0; i < 10; ++i)
            {
                mStarsRankArr[i].gameObject.SetActive(false);
            }
            mStarsPic.gameObject.SetActive(false);
            mStarslv.gameObject.SetActive(false);
            mStarsLabel.gameObject.SetActive(false);
        }
        else if (mStarsP < 1)
        {
            mStarsPic.gameObject.SetActive(false);
            if (!mStarslv.gameObject.activeSelf)
                mStarslv.gameObject.SetActive(true);
            if (!mStarsLabel.gameObject.activeSelf)
                mStarsLabel.gameObject.SetActive(true);
            for (int i = 0; i < 10; ++i)
            {
                if (!mStarsRankArr[i].gameObject.activeSelf)
                    mStarsRankArr[i].gameObject.SetActive(true); 
                SetIcon(mStarsRankArr[i], "common:starslvback");
            }
        }
        else
        {
            if (!mStarsPic.gameObject.activeSelf)
                mStarsPic.gameObject.SetActive(true);
            if (!mStarslv.gameObject.activeSelf)
                mStarslv.gameObject.SetActive(true);
            if (!mStarsLabel.gameObject.activeSelf)
                mStarsLabel.gameObject.SetActive(true);

            int mCountStars = mStarsP % 10 == 0 ? 10 : mStarsP % 10;
            for (int i = 0; i < mCountStars; ++i)
            {
                if (!mStarsRankArr[i].gameObject.activeSelf)
                    mStarsRankArr[i].gameObject.SetActive(true); 
                SetIcon(mStarsRankArr[i], starsitem.starspicname);
            }
            SetIcon(mStarsPic, starsitem.starspicname);
            if (mCountStars < 10)
            {
                for (int i = mCountStars; i < 10; ++i)
                {
                    if (!mStarsRankArr[i].gameObject.activeSelf)
                        mStarsRankArr[i].gameObject.SetActive(true); 
                    SetIcon(mStarsRankArr[i], "common:starslvback");
                }
            }
        }
        //宝石镶嵌情况
        List<stone_info> stoneinfo = mData.GetStoneInfo();
        for (int i = 0; i < mStoneRankArr.Count(); ++i)
        {
           mStonePanelList1[i].SetActive(false);
           mStoneRankArr[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < mDefenceItem.stoneCount; ++i)
            mStonePanelList1[i].SetActive(true);
        StoneTableItem stoneitem = null;
        int stoneitempos = 0;
        for (int i = 0; i < stoneinfo.Count; ++i)
        {
            StoneTableItem item = DataManager.StoneTable[stoneinfo[i].stoneid] as StoneTableItem;
            if (stoneitem == null || item.level > stoneitem.level)
            {
                stoneitem = item;
                stoneitempos = stoneinfo[i].stonepos;
            }
            else if (item.level == stoneitem.level && stoneinfo[i].stonepos < stoneitempos)
            {
                stoneitempos = stoneinfo[i].stonepos;
                stoneitem = item;
            }
            mStoneRankArr[i].gameObject.SetActive(true);
            SetIcon(mStoneRankArr[i], item.picname);
        }
        if (null != stoneitem)
        {
            mInitStonePic.gameObject.SetActive(true);
            SetIcon(mInitStonePic, stoneitem.picname);
        }
        else
            mInitStonePic.gameObject.SetActive(false);

        mStarslv.text = "[" + starsitem.colorfont + "]" + (mStarsP % 10 == 0 && mStarsP != 0 ? mStarsP / 10 : (mStarsP / 10 + 1)) + StringHelper.GetString("promote");//10星升一级，星阶最低为一阶，数据存储待定！
        mUselevel.text = mDefenceItem.uselevel + StringHelper.GetString("lv") ;
        mWearpos.text = mDefenceItem.slotname;
        mFightValue.text = mData.GetFightvalue().ToString();
        mSaleMoney.text = mData.GetSaleMoney().ToString();

        foreach (Transform trans in mItemsGrid.transform)
        {
            trans.gameObject.SetActive(false);
            GameObject.Destroy(trans.gameObject);
        }
        mItemsGrid.transform.localPosition = new Vector3(0, 84, 0);
       
        //属性设置
        //1.基础属性
        SetProName("baseproname", mItemsGrid, "[FED514]");
        SetProValue(mItemsGrid, "damageproname", mDefenceItem.basePropertyDamage, mData.GetProdamagestren());
        SetProValue(mItemsGrid, "defenceproname", mDefenceItem.basePropertyDefence, mData.GetProdefencestren());
        SetProValue(mItemsGrid, "lifeproname", mDefenceItem.basePropertyLife, mData.GetProlifestren());
        SetProValue(mItemsGrid, "critproname", mDefenceItem.basePropertyCrit, mData.GetProcritstren());

        //2.星级属性
        if (mStarsP > 0)
        {
            SetProName("starsproname", mItemsGrid, "[79FFDB]");
            SetProValue(mItemsGrid, "damageproname", mData.GetProdamagestars());
            SetProValue(mItemsGrid, "defenceproname", mData.GetProdefencestars());
            SetProValue(mItemsGrid, "lifeproname", mData.GetProlifestars());
            SetProValue(mItemsGrid, "critproname", mData.GetProcritstars());
        }

        //3.宝石属性
        if (stoneinfo.Count > 0)
        {
            SetProName("stoneproname", mItemsGrid, "[FFFFFF]");
            for (int i = 0; i < stoneinfo.Count(); ++i)
            {
                StoneTableItem itemsss = DataManager.StoneTable[stoneinfo[i].stoneid] as StoneTableItem;
                if (null == itemsss)
                    continue;
                SetStoneProValue(mItemsGrid, itemsss.proid, itemsss.provalue);
            }
        }
        mItemsGrid.Reposition();
        if (mParam.packtype == PackageType.Pack_Equip)
        {
            mWearlabel.text = "[3EFF00]" + StringHelper.GetString("hasweared");
        }
        else
        {
            mWearlabel.text = "[FFFFFF]" + StringHelper.GetString("unweared");
        }

        if (flag)
        {
            mInitButtonPanel.SetActive(false);
        }
        else
        {
            mInitButtonPanel.SetActive(true);
        }
    }

    public void SyncDefencePromoteInfo()
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();

        if (null == module)
            return;

        DefenceObj mData = module.GetItemByIDAndPos(mParam.itemid, mParam.packpos, mParam.packtype) as DefenceObj;

        if (null == mData)
        {
            GameDebug.LogError("背包中没有此装备id = " + mParam.itemid);
            return;
        }

        DefenceTableItem mDefenceItem = DataManager.DefenceTable[mParam.itemid] as DefenceTableItem;

        if (null == mDefenceItem)
        {
            GameDebug.LogError("defence.txt中没有此装备 id = " + mParam.itemid);
            return;
        }

        int fightvalue = mDefenceItem.fightValue;

        DefenceCombItem mCombItem = DataManager.DefenceCombTable[mDefenceItem.combId] as DefenceCombItem;

        if (null == mCombItem)
        {
            GameDebug.LogError("defencecomb.txt中没有此合成序列 id = " + mDefenceItem.combId);
            return;
        }

        mDefenceItem = DataManager.DefenceTable[mCombItem.defenceproducedId] as DefenceTableItem;

        if (null == mDefenceItem)
        {
            GameDebug.LogError("defence.txt中没有此装备 id = " + mCombItem.defenceproducedId);
            return;
        }

        SetIcon(mPromoteDefencepic, mDefenceItem.picname);
        ConfigTableItem cofig = DataManager.ConfigTable[mDefenceItem.quality] as ConfigTableItem;
        if (null == cofig)
        {
            GameDebug.LogError("没有此品质！id = " + mDefenceItem.quality);
            return;
        }
        mPromoteDefencename.text = "[" + cofig.value + "]";
        if (mData.GetStrenLv() < 1)
        {
            mPromoteDefencename.text += mDefenceItem.name;
            mPromoteStrenlv.gameObject.SetActive(false);
        }
        else
        {
            mPromoteDefencename.text += mDefenceItem.name + "  +" + mData.GetStrenLv();
            mPromoteStrenlv.gameObject.SetActive(true);
            mPromoteStrenlv.text = "+" + mData.GetStrenLv();
        }

        int mStarsP = mData.GetStarsLv();
        DefenceStarsItem starsitem = DataManager.DefenceStarsTable[mStarsP] as DefenceStarsItem;
        if (null == starsitem)
        {
            GameDebug.LogError("defencestars.txt没有此星等id = " + mStarsP);
            return;
        }
        if (mDefenceItem.starsLevelMax < 1)
        {
            mPromoteStarsPic.gameObject.SetActive(false);
            mPromoteStarsLabel.gameObject.SetActive(false);
            mPromoteStarslv.gameObject.SetActive(false);
            for (int i = 0; i < 10; ++i)
            {
                mPromoteStarsRankArr[i].gameObject.SetActive(false);
            }
        }
        else if (mStarsP < 1)
        {
            mPromoteStarsPic.gameObject.SetActive(false);
            if (!mPromoteStarsLabel.gameObject.activeSelf)
                mPromoteStarsLabel.gameObject.SetActive(true);
            if (!mPromoteStarslv.gameObject.activeSelf)
                mPromoteStarslv.gameObject.SetActive(true);
            for (int i = 0; i < 10; ++i)
            {
                if (!mPromoteStarsRankArr[i].gameObject.activeSelf)
                    mPromoteStarsRankArr[i].gameObject.SetActive(true);
                SetIcon(mPromoteStarsRankArr[i], "common:starslvback");
            }
        }
        else
        {
            if (!mPromoteStarsLabel.gameObject.activeSelf)
            mPromoteStarsLabel.gameObject.SetActive(true);
            if (!mPromoteStarslv.gameObject.activeSelf)
                mPromoteStarslv.gameObject.SetActive(true);
            if (!mPromoteStarsPic.gameObject.activeSelf)
                mPromoteStarsPic.gameObject.SetActive(true);
            int mCountStars = mStarsP % 10 == 0 ? 10 : mStarsP % 10;
            for (int i = 0; i < mCountStars; ++i)
            {
                if (!mPromoteStarsRankArr[i].gameObject.activeSelf)
                    mPromoteStarsRankArr[i].gameObject.SetActive(true);
                SetIcon(mPromoteStarsRankArr[i], starsitem.starspicname);
            }
            SetIcon(mPromoteStarsPic, starsitem.starspicname);
            if (mCountStars < 10)
            {
                for (int i = mCountStars; i < 10; ++i)
                {
                    if (!mPromoteStarsRankArr[i].gameObject.activeSelf)
                        mPromoteStarsRankArr[i].gameObject.SetActive(true);
                    SetIcon(mPromoteStarsRankArr[i], "common:starslvback");
                }
            }
        }

        //宝石镶嵌情况
        List<stone_info> stoneinfo = mData.GetStoneInfo();
        for (int i = 0; i < mPromoteStoneRankArr.Count(); ++i)
        {
            mStonePanelList2[i].SetActive(false);
            mPromoteStoneRankArr[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < mDefenceItem.stoneCount; ++i)
            mStonePanelList2[i].SetActive(true);
        StoneTableItem stoneitem = null;
        int stoneitempos = 0;
        for (int i = 0; i < stoneinfo.Count; ++i)
        {
            StoneTableItem item = DataManager.StoneTable[stoneinfo[i].stoneid] as StoneTableItem;
            if (stoneitem == null || item.level > stoneitem.level)
            {
                stoneitem = item;
                stoneitempos = stoneinfo[i].stonepos;
            }
            else if (item.level == stoneitem.level && stoneinfo[i].stonepos < stoneitempos)
            {
                stoneitempos = stoneinfo[i].stonepos;
                stoneitem = item;
            }
            mPromoteStoneRankArr[i].gameObject.SetActive(true);
            UIAtlasHelper.SetSpriteImage(mPromoteStoneRankArr[i], item.picname);
        }
        if (null != stoneitem)
        {
            mPromoteStonePic.gameObject.SetActive(true);
            SetIcon(mPromoteStonePic, stoneitem.picname);
        }
        else
            mPromoteStonePic.gameObject.SetActive(false);

        mPromoteStarslv.text = "[" + starsitem.colorfont + "]" + (mStarsP % 10 == 0 && mStarsP != 0 ? mStarsP / 10 : (mStarsP / 10 + 1)) + StringHelper.GetString("promote");//10星升一级，星阶最低为一阶，数据存储待定！
        mPromoteUselevel.text = mDefenceItem.uselevel + StringHelper.GetString("lv");
        mPromoteWearpos.text = mDefenceItem.slotname;
        mPromoteWearlabel.text = "[E92224]" + StringHelper.GetString("newdefence");
        mPromoteFightValue.text = mData.GetFightvalue().ToString();
        mPromoteSaleMoney.text = mData.GetSaleMoney().ToString();
        mFightvalueRised.text = (mDefenceItem.fightValue - fightvalue).ToString();

        foreach (Transform trans in mPromoteItemsGrid.transform)
        {
            trans.gameObject.SetActive(false);
            GameObject.Destroy(trans.gameObject);
        }
        mPromoteItemsGrid.transform.localPosition = new Vector3(0, 84, 0);
        //属性设置
        //1.基础属性
        SetProName("baseproname", mPromoteItemsGrid, "[FED514]");
        SetProValue(mPromoteItemsGrid, "damageproname", mDefenceItem.basePropertyDamage, mData.GetProdamagestren());
        SetProValue(mPromoteItemsGrid, "defenceproname", mDefenceItem.basePropertyDefence, mData.GetProdefencestren());
        SetProValue(mPromoteItemsGrid, "lifeproname", mDefenceItem.basePropertyLife, mData.GetProlifestren());
        SetProValue(mPromoteItemsGrid, "critproname", mDefenceItem.basePropertyCrit, mData.GetProcritstren());

        //2.星级属性
        if (mStarsP > 0)
        {
            SetProName("starsproname", mPromoteItemsGrid, "[79FFDB]");
            SetProValue(mPromoteItemsGrid, "damageproname", mData.GetProdamagestars());
            SetProValue(mPromoteItemsGrid, "defenceproname", mData.GetProdefencestars());
            SetProValue(mPromoteItemsGrid, "lifeproname", mData.GetProlifestars());
            SetProValue(mPromoteItemsGrid, "critproname", mData.GetProcritstars());
        }

        //3.宝石属性
        if (stoneinfo.Count > 0)
        {
            SetProName("stoneproname", mPromoteItemsGrid, "[FFFFFF]");
            for (int i = 0; i < stoneinfo.Count(); ++i)
            {
                StoneTableItem itemsss = DataManager.StoneTable[stoneinfo[i].stoneid] as StoneTableItem;
                if (null == itemsss)
                    continue;
                SetStoneProValue(mPromoteItemsGrid, itemsss.proid, itemsss.provalue);
            }
        }
        mPromoteItemsGrid.Reposition();

        UIAtlasHelper.SetSpriteImage(mEquipInfo, "atlas_defence:defencepromote");

    }

    private void SetIcon(UISprite mIcon, string icon)
    {
        UIAtlasHelper.SetSpriteImage(mIcon, icon);
    }

    private void SetProName(string name, UIGrid mItemGrid, string color)
    {
        GameObject obj = WindowManager.Instance.CloneGameObject(mPropertyUI);
        ObjectCommon.GetChildComponent<UILabel>(obj, "Label").text = color + StringHelper.GetString(name);
        Vector3 pos = mItemGrid.transform.localPosition;
        obj.name = (mItemGrid.transform.childCount + 1).ToString();
        obj.transform.parent = mItemGrid.transform;
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = pos;
    }
    private void SetProValue(UIGrid mItemGrid, string proname, int basevalue, int addvalue = 0)
    {
        GameObject obj = WindowManager.Instance.CloneGameObject(mPropertyValueUI);
        if (-1 != basevalue)
        {
            ObjectCommon.GetChildComponent<UILabel>(obj, "baseproname").text = "[2A96C9]" + StringHelper.GetString(proname);
            ObjectCommon.GetChildComponent<UILabel>(obj, "baseprovalue").text = "[2A96C9]" + basevalue.ToString();
        }
        else
        {
            return;
        }
        Vector3 pos = mItemGrid.transform.localPosition;
        obj.name = (mItemGrid.transform.childCount + 1).ToString();
        obj.transform.parent = mItemGrid.transform;
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = pos;

        if (0 != addvalue)
            ObjectCommon.GetChildComponent<UILabel>(obj, "addprovalue").text = "[3EFF00]" +  "+" + addvalue;
    }

    private void SetStoneProValue(UIGrid mItemGrid, int proid, int basevalue)
    {
        GameObject obj = WindowManager.Instance.CloneGameObject(mPropertyValueUI);
        if (-1 != basevalue)
        {
            PropertyTableItem item = DataManager.PropertyTable[proid] as PropertyTableItem;
            if (null == item)
            {
                GameDebug.LogError("property.xml中没有此属性id = " + proid);
                return;
            }
            ObjectCommon.GetChildComponent<UILabel>(obj, "baseproname").text = "[2A96C9]" + item.name;
            ObjectCommon.GetChildComponent<UILabel>(obj, "baseprovalue").text = "[2A96C9]" + basevalue.ToString();
        }
        else
        {
            return;
        }
        Vector3 pos = mItemGrid.transform.localPosition;
        obj.transform.parent = mItemGrid.transform;
        obj.name = (mItemGrid.transform.childCount + 1).ToString();
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = pos;
    }

    public void SyncStoneDefenceInfo()
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();

        if (null == module)
            return;

        DefenceTableItem mDefenceItem = DataManager.DefenceTable[mParam.itemid] as DefenceTableItem;
        if (null == mDefenceItem)
        {
            GameDebug.LogError("defence.txt中没有此装备 id = " + mParam.itemid);
            return;
        }

        DefenceObj mData = module.GetItemByIDAndPos(mParam.itemid, mParam.packpos, mParam.packtype) as DefenceObj;

        if (null == mData)
        {
            GameDebug.LogError("背包中没有此装备 id = " + mParam.itemid);
            return;
        }

        SetIcon(mStoneDefencepic, mDefenceItem.picname);
        ConfigTableItem cofig = DataManager.ConfigTable[mDefenceItem.quality] as ConfigTableItem;
        if (null == cofig)
        {
            GameDebug.LogError("没有此品质！id = " + mDefenceItem.quality);
            return;
        }
        mStoneDefencename.text = "[" + cofig.value + "]";
        if (mData.GetStrenLv() < 1)
        {
            mStoneDefencename.text += mDefenceItem.name;
            mStoneStrenlv.gameObject.SetActive(false);
        }
        else
        {
            mStoneStrenlv.gameObject.SetActive(true);
            mStoneDefencename.text += mDefenceItem.name + "  +" + mData.GetStrenLv();
            mStoneStrenlv.text = "+" + mData.GetStrenLv();
        }

        int mStarsP = mData.GetStarsLv();
        DefenceStarsItem starsitem = DataManager.DefenceStarsTable[mStarsP] as DefenceStarsItem;
        if (null == starsitem)
        {
            GameDebug.LogError("defencestars.txt没有此星等id = " + mStarsP);
            return;
        }
        if (mDefenceItem.starsLevelMax < 1)
        {
            mStoneStarsLabel.gameObject.SetActive(false);
            mStoneStarsPic.gameObject.SetActive(false);
            mStoneStarslv.gameObject.SetActive(false);
            for (int i = 0; i < 10; ++i)
            {
                if (mStoneStarsRankArr[i].gameObject.activeSelf)
                    mStoneStarsRankArr[i].gameObject.SetActive(false);
            }
        }
        else if (mStarsP < 1)
        {
            mStoneStarsPic.gameObject.SetActive(false);
            if (!mStoneStarslv.gameObject.activeSelf)
                mStoneStarslv.gameObject.SetActive(true);
            if (!mStoneStarsLabel.gameObject.activeSelf)
                mStoneStarsLabel.gameObject.SetActive(true);
            for (int i = 0; i < 10; ++i)
            {
                if (!mStoneStarsRankArr[i].gameObject.activeSelf)
                    mStoneStarsRankArr[i].gameObject.SetActive(true);
                SetIcon(mStoneStarsRankArr[i], "common:starslvback");
            }
        }
        else
        {   
            if (!mStoneStarsPic.gameObject.activeSelf)
                mStoneStarsPic.gameObject.SetActive(true);
            if (!mStoneStarslv.gameObject.activeSelf)
                mStoneStarslv.gameObject.SetActive(true);
            if (!mStoneStarsLabel.gameObject.activeSelf)
                mStoneStarsLabel.gameObject.SetActive(true);
            int mCountStars = mStarsP % 10 == 0 ? 10 : mStarsP % 10;
            for (int i = 0; i < mCountStars; ++i)
            {
                if (!mStoneStarsRankArr[i].gameObject.activeSelf)
                    mStoneStarsRankArr[i].gameObject.SetActive(true);
                SetIcon(mStoneStarsRankArr[i], starsitem.starspicname);
            }
            SetIcon(mStoneStarsPic, starsitem.starspicname);
            if (mCountStars < 10)
            {
                for (int i = mCountStars; i < 10; ++i)
                {
                    if (!mStoneStarsRankArr[i].gameObject.activeSelf)
                        mStoneStarsRankArr[i].gameObject.SetActive(true);
                    SetIcon(mStoneStarsRankArr[i], "common:starslvback");
                }
            }
        }

        //宝石镶嵌情况
        List<stone_info> stoneinfo = mData.GetStoneInfo();
        for (int i = 0; i < mStoneStoneRankArr.Count(); ++i)
        {
            mStonePanelList3[i].SetActive(false);
            mStoneStoneRankArr[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < mDefenceItem.stoneCount; ++i)
            mStonePanelList3[i].SetActive(true);
        StoneTableItem stoneitem = null;
        int stoneitempos = 0;
        for (int i = 0; i < stoneinfo.Count; ++i)
        { 
            StoneTableItem item = DataManager.StoneTable[stoneinfo[i].stoneid] as StoneTableItem;
            if (stoneitem == null || item.level > stoneitem.level)
            {
                stoneitem = item;
                stoneitempos = stoneinfo[i].stonepos;
            }
            else if (item.level == stoneitem.level && stoneinfo[i].stonepos < stoneitempos)
            {
                stoneitempos = stoneinfo[i].stonepos;
                stoneitem = item;
            }
            mStoneStoneRankArr[i].gameObject.SetActive(true);
            UIAtlasHelper.SetSpriteImage(mStoneStoneRankArr[i], item.picname);
        }
        if (null != stoneitem)
        {
            mStoneStonePic.gameObject.SetActive(true);
            SetIcon(mStoneStonePic, stoneitem.picname);
        }
        else
            mStoneStonePic.gameObject.SetActive(false);
        foreach (Transform trans in mStoneDefenceItemsGrid.transform)
        {
            trans.gameObject.SetActive(false);
            GameObject.Destroy(trans.gameObject);
        }
        mStoneDefenceItemsGrid.transform.localPosition = new Vector3(0, 84, 0);
        
        //属性设置
        //1.基础属性
        SetProName("baseproname", mStoneDefenceItemsGrid, "[FED514]");
        SetProValue(mStoneDefenceItemsGrid, "damageproname", mDefenceItem.basePropertyDamage, mData.GetProdamagestren());
        SetProValue(mStoneDefenceItemsGrid, "defenceproname", mDefenceItem.basePropertyDefence, mData.GetProdefencestren());
        SetProValue(mStoneDefenceItemsGrid, "lifeproname", mDefenceItem.basePropertyLife, mData.GetProlifestren());
        SetProValue(mStoneDefenceItemsGrid, "critproname", mDefenceItem.basePropertyCrit, mData.GetProcritstren());

        //2.星级属性
        if (mStarsP > 0)
        {
            SetProName("starsproname", mStoneDefenceItemsGrid, "[79FFDB]");
            SetProValue(mStoneDefenceItemsGrid, "damageproname", mData.GetProdamagestars());
            SetProValue(mStoneDefenceItemsGrid, "defenceproname", mData.GetProdefencestars());
            SetProValue(mStoneDefenceItemsGrid, "lifeproname", mData.GetProlifestars());
            SetProValue(mStoneDefenceItemsGrid, "critproname", mData.GetProcritstars());
        }

        //3.宝石属性
        if (stoneinfo.Count > 0)
        {
            SetProName("stoneproname", mStoneDefenceItemsGrid,"[FFFFFF]");
            for (int i = 0; i < stoneinfo.Count(); ++i)
            {
                StoneTableItem itemsss = DataManager.StoneTable[stoneinfo[i].stoneid] as StoneTableItem;
                if (null == itemsss)
                    continue;
                SetStoneProValue(mStoneDefenceItemsGrid, itemsss.proid, itemsss.provalue);
            }
        }

        mStoneDefenceItemsGrid.Reposition();

        mStoneStarslv.text = "[" + starsitem.colorfont + "]" + (mStarsP % 10 == 0 && mStarsP != 0 ? mStarsP / 10 : (mStarsP / 10 + 1)) + StringHelper.GetString("promote");//10星升一级，星阶最低为一阶，数据存储待定！
        mStoneUselevel.text = mDefenceItem.uselevel + StringHelper.GetString("lv");
        mStoneWearpos.text = mDefenceItem.slotname;
        if (mParam.packtype == PackageType.Pack_Equip)
        {
            mStoneWearlabel.text = "[3EFF00]" + StringHelper.GetString("hasweared");
        }
        else
        {
            mStoneWearlabel.text = "[FFFFFF]" + StringHelper.GetString("unweared");
        }
        mStoneFightValue.text = mData.GetFightvalue().ToString();
        mStoneSaleMoney.text = mData.GetSaleMoney().ToString();
    }

    public void SyncStoneInlayUIInfo()
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();

        if (null == module)
            return;

        DefenceTableItem mDefenceItem = DataManager.DefenceTable[mParam.itemid] as DefenceTableItem;
        if (null == mDefenceItem)
        {
            GameDebug.LogError("defence.txt中没有此装备 id = " + mParam.itemid);
            return;
        }

        DefenceObj mData = module.GetItemByIDAndPos(mParam.itemid, mParam.packpos, mParam.packtype) as DefenceObj;

        if (null == mData)
        {
            GameDebug.LogError("背包中没有此装备 id = " + mParam.itemid);
            return;
        }

        PackageManager pack = module.GetPackManager();
        if (null == pack)
            return;
        Dictionary<int, ItemObj> dic = pack.getPackDic(PackageType.Pack_Gem);
        
        if(null == dic)
            return;

        mStoneInlayItemsGrid.transform.localPosition = new Vector3(0, 190, 0);
        foreach(KeyValuePair<int, ItemObj> value in dic)
        {
            if (null == value.Value)
                continue;
            StoneTableItem item = value.Value.GetRes() as StoneTableItem;
            if (mStoneList.Contains(item))
                continue;
            GameObject obj = WindowManager.Instance.CloneGameObject(mStoneProUI);
            obj.name = item.id.ToString();
            PropertyTableItem proitem = DataManager.PropertyTable[item.proid] as PropertyTableItem;
            if (proitem == null)
            {
                GameDebug.LogError("property.xml中没有此属性 id = " + item.proid);
                return;
            }

            ObjectCommon.GetChildComponent<UILabel>(obj, "Sprite/stonename").text = item.name;
            ObjectCommon.GetChildComponent<UILabel>(obj, "Sprite/stonepro").text = proitem.name + " +" + item.provalue;
            ObjectCommon.GetChildComponent<UILabel>(obj, "Sprite/Label").text = pack.GetNumByID(item.id,PackageType.Pack_Gem).ToString();
            UIAtlasHelper.SetSpriteImage(ObjectCommon.GetChildComponent<UISprite>(obj, "Sprite/stonepic"), item.picname);
            UIEventListener.Get(obj).onClick = OnBtnStoneProItemClicked;
            Vector3 pos = mStoneInlayItemsGrid.transform.localPosition;
            obj.transform.parent = mStoneInlayItemsGrid.transform;
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = pos;
            mStoneInlayItemsGrid.Reposition();
            mStoneItemList.Add(item.id,obj);
            mStoneList.Add(item);
        }
    }

    public void SynStoneDemandUIInfo(stone_info stoneinfo)
    {
        StoneTableItem item = DataManager.StoneTable[stoneinfo.stoneid] as StoneTableItem;

        if (null == item)
        {
            GameDebug.LogError("stones.txt中没有此宝石 id = " + stoneinfo.stoneid);
            return;
        }

        PropertyTableItem itempro = DataManager.PropertyTable[item.proid] as PropertyTableItem;

        if (null == itempro)
        {
            GameDebug.LogError("property.xml中没有此属性 id = " + item.proid);
            return;
        }

        UIAtlasHelper.SetSpriteImage(mStonePic, item.picname);
        ConfigTableItem cofig = DataManager.ConfigTable[item.quality] as ConfigTableItem;
        if (null == cofig)
        {
            GameDebug.LogError("没有此品质！id = " + item.quality);
            return;
        }
        mStoneName.text = "[" + cofig.value + "]";
        mStoneName.text += item.name;// + "Lv " + item.level;
        mStoneDesc.text = item.desc;
        mProidandvalue.text = itempro.name + "+" + item.provalue;

    }

    public void SyncStoneDisplayUIInfo()
    {
        mStonePic1.gameObject.SetActive(false);
        mStonePic2.gameObject.SetActive(false);
        mStonePic3.gameObject.SetActive(false);
        mStonePic4.gameObject.SetActive(false);

        mCanPromote1.gameObject.SetActive(false);
        mCanPromote2.gameObject.SetActive(false);
        mCanPromote3.gameObject.SetActive(false);
        mCanPromote4.gameObject.SetActive(false);

        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();

        if (null == module)
            return;

        DefenceObj mData = module.GetItemByIDAndPos(mParam.itemid, mParam.packpos, mParam.packtype) as DefenceObj;

        if (null == mData)
            return;

        List<stone_info> stoneinfolist = mData.GetStoneInfo();
        if (null == stoneinfolist)
            return;

        StoneTableItem item = null;
        DefenceCombItem combItem = null;

        DefenceTableItem defenceitem = DataManager.DefenceTable[mParam.itemid] as DefenceTableItem;
        if (null == defenceitem)
            return;

        switch (defenceitem.stoneCount)
        { 
            case 0:
                mElement1Panel.SetActive(false);
                mElement2Panel.SetActive(false);
                mElement3Panel.SetActive(false);
                mElement4Panel.SetActive(false);
                break;
            case 1:
                mElement1Panel.SetActive(true);
                mElement2Panel.SetActive(false);
                mElement3Panel.SetActive(false);
                mElement4Panel.SetActive(false);
                break;
            case 2:
                mElement1Panel.SetActive(true);
                mElement2Panel.SetActive(false);
                mElement3Panel.SetActive(true);
                mElement4Panel.SetActive(false);
                break;
            case 3:
                mElement1Panel.SetActive(true);
                mElement2Panel.SetActive(true);
                mElement3Panel.SetActive(true);
                mElement4Panel.SetActive(false);
                break;
            case 4:
                mElement1Panel.SetActive(true);
                mElement2Panel.SetActive(true);
                mElement3Panel.SetActive(true);
                mElement4Panel.SetActive(true);
                break;
            default:
                mElement1Panel.SetActive(true);
                mElement2Panel.SetActive(true);
                mElement3Panel.SetActive(true);
                mElement4Panel.SetActive(true);
                break;
        }

        foreach(stone_info stone in stoneinfolist)
        {
            item = DataManager.StoneTable[stone.stoneid] as StoneTableItem;
            if (null == item)
                continue;
            combItem = DataManager.DefenceCombTable[item.combid] as DefenceCombItem;

            switch (stone.stonepos)
            { 
                case 0:
                    SetStonePosInfo(mStonePic1, mCanPromote1, module, item, combItem);
                    break;
                case 1:
                    SetStonePosInfo(mStonePic2, mCanPromote2, module, item, combItem);
                    break;
                case 2:
                    SetStonePosInfo(mStonePic3, mCanPromote3, module, item, combItem);
                    break;
                case 3:
                    SetStonePosInfo(mStonePic4, mCanPromote4, module, item, combItem);
                    break;
            }
        }
    }

    private void SetStonePosInfo(UISprite mStonePic, UISprite mCanPromote, PlayerDataModule module, StoneTableItem item, DefenceCombItem combItem)
    {
        UIAtlasHelper.SetSpriteImage(mStonePic, item.picname);
        mStonePic.gameObject.SetActive(true);
        if (null == combItem)
            return;
        if (module.GetProceeds(ProceedsType.Money_Game) < combItem.moenyused)
            return ;
        uint playerhascitem = module.GetItemNumByID(combItem.item1, PackageType.Pack_Gem);
        if (playerhascitem + 1 < combItem.num1)
            return ;
        mCanPromote.gameObject.SetActive(true);
    }

    public bool SyncStoneCombDemandInfo()
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();

        if (null == module)
            return false;

        DefenceObj mData = module.GetItemByIDAndPos(mParam.itemid, mParam.packpos, mParam.packtype) as DefenceObj;

        if (null == mData)
        {
            GameDebug.LogError("背包中没有此宝石 id = " + mParam.itemid);
            return false;
        }

        stone_info stoneinfo = mData.GetStoneInfoByPos(mParam.stonepos);
        if (null == stoneinfo)
        {
            GameDebug.LogError("此位置没有镶嵌宝石");
            return false;
        }

        StoneTableItem item = DataManager.StoneTable[stoneinfo.stoneid] as StoneTableItem;

        if (null == item)
        {
            GameDebug.LogError("stones.txt中没有此宝石 id = " + stoneinfo.stoneid);
            return false;
        }

        DefenceCombItem combitem = DataManager.DefenceCombTable[item.combid] as DefenceCombItem;

        if (null == combitem)
        {
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("stonesmax"));
            return false;
        }

        item = DataManager.StoneTable[combitem.defenceproducedId] as StoneTableItem;

        if (null == item)
        {
            GameDebug.LogError("stones.txt中没有此宝石 id = " + combitem.defenceproducedId);
            return false;
        }

        PropertyTableItem itempro = DataManager.PropertyTable[item.proid] as PropertyTableItem;

        if (null == itempro)
        {
            GameDebug.LogError("property.xml中没有此属性 id = " + item.proid);
            return false;
        }
        UIAtlasHelper.SetSpriteImage(mStoneDemandPic,item.picname);
        ConfigTableItem cofig = DataManager.ConfigTable[item.quality] as ConfigTableItem;
        if (null == cofig)
        {
            GameDebug.LogError("没有此品质！id = " + item.quality);
            return false;
        }
        mStoneDemandName.text = "[" + cofig.value + "]";
        mStoneDemandName.text += item.name;// + "Lv " + item.level;
        mStoneDemandPro.text = itempro.name + "+" + item.provalue;
        if (module.GetProceeds(ProceedsType.Money_Game) < combitem.moenyused)
            mStoneDemandMoney.text = "[E92224]";
        else
            mStoneDemandMoney.text = "[3EFF00]";
        mStoneDemandMoney.text += "金币 X" + combitem.moenyused;

        if (module.GetItemNumByID(combitem.item1, PackageType.Pack_Gem) + 1 < combitem.num1)
            mStoneDemandNum.text = "[E92224]";
        else
            mStoneDemandNum.text = "[3EFF00]";
        mStoneDemandNum.text += item.name/* + "Lv" + (item.level - 1)*/ + " X " + combitem.num1;

        UIAtlasHelper.SetSpriteImage(mEquipInfo, "atlas_defence:stonerising");
        return true;
    }

    public void DeleteInlayedStone(stone_info stoneinfo)
    {
        StoneTableItem item = DataManager.StoneTable[stoneinfo.stoneid] as StoneTableItem;
        if (null == item)
            return;
        if (!mStoneItemList.ContainsKey(stoneinfo.stoneid))
            return;
        mStoneItemList[stoneinfo.stoneid].gameObject.SetActive(false);
        GameObject.Destroy(mStoneItemList[stoneinfo.stoneid]);
        mStoneItemList.Remove(stoneinfo.stoneid);
        mStoneList.Remove(item);

        mStoneInlayItemsGrid.Reposition();
    }

    private void DefenceSaleHandler(EventBase evt)
    {
        WindowManager.Instance.CloseUI("defence");
        WindowManager.Instance.CloseUI("saleagain");
    }

    protected override void OnClose()
    {
        SetCloseActive();
        if (mOldStoneSelect)
            mOldStoneSelect.gameObject.SetActive(false);
        if (mOldSelect)
            SetIcon(mOldSelect, "atlas_defence:upelement1");

        EventSystem.Instance.removeEventListener(ItemEvent.DEFENCE_STREN, DefenceStrenHandler);
        EventSystem.Instance.removeEventListener(ItemEvent.DEFENCE_RISING_STARS, DefenceRisingHandler);
        EventSystem.Instance.removeEventListener(ItemEvent.DEFENCE_PROMOTE, DefencePromoteHandler);
        EventSystem.Instance.removeEventListener(ItemEvent.STONE_COMB, StoneCombHandler);
        EventSystem.Instance.removeEventListener(ItemEvent.STONE_INLAY, StoneInlayHandler);
        EventSystem.Instance.removeEventListener(ItemEvent.STONE_UNINLAY, StoneUnInlayHandler);
        EventSystem.Instance.removeEventListener(ItemEvent.DEFENCE_SALE, DefenceSaleHandler);
    }

    #region 特效
    //宝石合成成功
    void onCombSucess()
    {
        mCombSuccessAni.Reset();
        mCombSuccessAni.gameObject.SetActive(true);
        mCombSuccessAni.onFinished += onStrenSpriteAniFinish;
    }

    void onStrenSpriteAniFinish(GameObject go)
    {
        mCombSuccessAni.onFinished -= onStrenSpriteAniFinish;
        mCombSuccessAni.Reset();
        mCombSuccessAni.gameObject.SetActive(false);
    }
    
    //宝石合成宝石显示界面特效
    void onStoneCombSucess(int pos)
    {
        UIButton mBtn = null;
        switch (pos)
        { 
            case 0:
                mBtn = mBtnElemente1;
                break;
            case 1:
                mBtn = mBtnElemente2;
                break;
            case 2:
                mBtn = mBtnElemente3;
                break;
            case 3:
                mBtn = mBtnElemente4;
                break;

        }
        mDisPlayCombSuccessAni.gameObject.transform.parent = mBtn.gameObject.transform;
        mDisPlayCombSuccessAni.gameObject.transform.localPosition = new Vector3(0, 0, 0);
        mDisPlayCombSuccessAni.Reset();
        mDisPlayCombSuccessAni.gameObject.SetActive(true);
        mDisPlayCombSuccessAni.onFinished += onStoneCombSpriteAniFinish;
    }

    void onStoneCombSpriteAniFinish(GameObject go)
    {
        mDisPlayCombSuccessAni.onFinished -= onStoneCombSpriteAniFinish;
        mDisPlayCombSuccessAni.Reset();
        mDisPlayCombSuccessAni.gameObject.SetActive(false);
    }
    #endregion
}
