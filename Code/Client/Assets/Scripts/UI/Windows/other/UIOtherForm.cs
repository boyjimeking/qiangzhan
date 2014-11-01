using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
class UIOtherForm : UIWindow
{

    private CommonItemUI[] mEquipUI = new CommonItemUI[(int)EquipSlot.EquipSlot_MAX];

    private UIButton mWeaponIcon = null;

    //private UIGrid mItemsGrid = null;

    private UIButton mTitleBtn = null;
    private UIButton mOpenBtn = null;
    private UIButton mBtnWeapon = null;

    //private PlayerDataModule mDataModule = null;

    //private GameObject mItemPrefab = null;
    //private GameObject mLockItem = null;

    //private UIScrollView mScrollView = null;
    //private UIScrollBar mScrollBar = null;

    private GameObject mBagPanel = null;
    private GameObject mInfoPanel = null;

    private UILabel mName = null;
    private UILabel mZhanli1 = null;
    private UILabel mZhanli2 = null;
    private UILabel mExp = null;

    private UILabel mHp = null;
    private UILabel mMp = null;
    private UILabel mDefence = null;
    private UILabel mDamage = null;
    private UILabel mCritLv = null;


    private UILabel mWeaponSterLv = null;       //强化等级
    private UILabel mWeaponPromoteLv = null;    //阶级
    private UISprite mWeaponGradeBg = null;     //制式背景
    private UISprite mWeaponGradePic = null;    //制式
    private UISprite mWeaponLvPic = null;       //星星图标

    private static int msMaxWingNum = 5;

    private GameObject[] mWingObject = new GameObject[msMaxWingNum];
    private UISprite[] mWingIcon = new UISprite[msMaxWingNum];
    private UILabel[] mWingLevel = new UILabel[msMaxWingNum];

    private UILabel mRoleName = null;

    private UILabel mOpenLabel = null;
    //保存 所有Grid里面的格子数
    private ArrayList mItemList = new ArrayList();

    private UICharacterPreview mPreview = new UICharacterPreview();
    private UISprite mPreveiwSprite;

    private PlayerData mOtherData = null;

    private GameObject mMainCrops = null;
    private GameObject mSubCrops = null;
    public UIOtherForm()
    {

    }
    protected override void OnLoad()
    {
        base.OnLoad();

        for( int i = 0 ; i < (int)EquipSlot.EquipSlot_MAX ; ++i )
        {
            mEquipUI[i] = new CommonItemUI(this.FindChild("RolePanel/Equip" + (i + 1).ToString()));
        }


        for (int i = 0; i < msMaxWingNum; ++i)
        {
            mWingObject[i] = this.FindChild("ProperyPanel/AllDisScroll/Panel/Wings/WingScroll/Wing" + (i + 1).ToString());
            mWingIcon[i] = this.FindComponent<UISprite>("ProperyPanel/AllDisScroll/Panel/Wings/WingScroll/Wing" + (i + 1).ToString() + "/icon");
            mWingLevel[i] = this.FindComponent<UILabel>("ProperyPanel/AllDisScroll/Panel/Wings/WingScroll/Wing" + (i + 1).ToString() + "/level");
        }


        mRoleName = this.FindComponent<UILabel>("RolePanel/Name");

        mName = this.FindComponent<UILabel>("ProperyPanel/AllDisScroll/Panel/Name");
        mZhanli1 = this.FindComponent<UILabel>("ProperyPanel/AllDisScroll/Panel/Zhanli");
        mZhanli2 = this.FindComponent<UILabel>("RolePanel/Zhanli/Label");
        mExp = this.FindComponent<UILabel>("ProperyPanel/AllDisScroll/Panel/Exp");
        mHp = this.FindComponent<UILabel>("ProperyPanel/AllDisScroll/Panel/hp");
        mMp = this.FindComponent<UILabel>("ProperyPanel/AllDisScroll/Panel/mp");
        mDefence = this.FindComponent<UILabel>("ProperyPanel/AllDisScroll/Panel/fanghu");
        mDamage = this.FindComponent<UILabel>("ProperyPanel/AllDisScroll/Panel/shanghai");
        mCritLv = this.FindComponent<UILabel>("ProperyPanel/AllDisScroll/Panel/baoji");

        mMainCrops = this.FindChild("ProperyPanel/AllDisScroll/Panel/Crops/CropsItem1");
        mSubCrops = this.FindChild("ProperyPanel/AllDisScroll/Panel/Crops/CropsItem2");

         mWeaponIcon = this.FindComponent<UIButton>("RolePanel/bg/weapon");

        mPreveiwSprite = this.FindComponent<UISprite>("RolePanel/RoleModel");

        mTitleBtn = this.FindComponent<UIButton>("RolePanel/bntChenghao");

        mBtnWeapon = this.FindComponent<UIButton>("RolePanel/bg/weapon");

        mOpenBtn = this.FindComponent<UIButton>("RolePanel/bntBag");


        mInfoPanel = this.FindChild("ProperyPanel");


        mWeaponSterLv = this.FindComponent<UILabel>("RolePanel/weaponlv");
        mWeaponPromoteLv = this.FindComponent<UILabel>("RolePanel/jieji/Label");

        mWeaponGradeBg = this.FindComponent<UISprite>("RolePanel/zhishi");
        mWeaponGradePic = this.FindComponent<UISprite>("RolePanel/zhishi/lv");

        mWeaponLvPic = this.FindComponent<UISprite>("RolePanel/xingji");


// 
         mOpenLabel = this.FindComponent<UILabel>("RolePanel/bntBag/Label");


         mTitleBtn.gameObject.SetActive(false);        

         mOpenLabel.text = "打开背包";
         mInfoPanel.SetActive(true);
         mOpenBtn.gameObject.SetActive(false);


// 
//         mItemsGrid = this.FindComponent<UIGrid>("ItemPanel/ItemsScrollView/UIGrid");
// 
//         EventDelegate.Add(mToggleAll.onChange, onToggleChanged);
//         EventDelegate.Add(mToggleItem.onChange, onToggleChanged);
//         EventDelegate.Add(mToggleGem.onChange, onToggleChanged);
//         EventDelegate.Add(mOpenBtn.onClick, onOpenButtonClick);
         EventDelegate.Add(mBtnWeapon.onClick, OnBtnWeaponClick);

        mPreveiwSprite.width = mPreveiwSprite.height;
        mPreview.SetTargetSprite(mPreveiwSprite);
        mPreview.RotationY = 180;

    }

    private void OnBtnWeaponClick()
    {
        int mainWeaponid = mOtherData.main_weaponId;
        if (!DataManager.WeaponTable.ContainsKey(mainWeaponid))
            return;

        ItemInfoParam param = new ItemInfoParam();
        param.itemid = mainWeaponid;
        param.itemtype = (ItemType)PackageType.Pack_Weapon;

        WindowManager.Instance.OpenUI("iteminfo",param);
    }

    //界面打开
    protected override void OnOpen(object param = null)
    {
        if( param == null )
        {
            mOtherData = OtherDataPool.Instance.GetOtherData();
        }else
        {
            mOtherData = (PlayerData)param;
        }

        UpdateEquips();
        UpdatePlayerData();
        OnWingUpdate();


        if( !DataManager.PlayerTable.ContainsKey(mOtherData.resId) )
        {
            return;
        }

        PlayerTableItem item = DataManager.PlayerTable[mOtherData.resId] as PlayerTableItem;

        if (item != null)
        {

            int[] equipConfigs = new int[(int)EquipSlot.EquipSlot_MAX];

            PropertyBuild.BuildEquipView(mOtherData, equipConfigs);


            mPreview.SetupCharacter(item.model, equipConfigs, mOtherData.mWingData.mWearId, uint.MaxValue);

            mPreview.ChangeWeapon(mOtherData.main_weaponId);
        }
    }
    //界面关闭
    protected override void OnClose()
    {

    }

    private void OnWingUpdate()
    {
        for (int i = 0; i < msMaxWingNum; ++i)
        {
            if (i < mOtherData.mWingData.wingItems.Count)
            {
                mWingObject[i].SetActive(true);
                WingCommonTableItem commonRes = DataManager.WingCommonTable[mOtherData.mWingData.wingItems[i].id] as WingCommonTableItem;
                if (commonRes != null)
                {
                    ItemTableItem  itemRes = ItemManager.GetItemRes(commonRes.costId);
                    if( itemRes != null )
                    {
                        UIAtlasHelper.SetSpriteImage(mWingIcon[i] , itemRes.picname);
                    }
                }
            }else
            {
                mWingObject[i].SetActive(false);
                UIAtlasHelper.SetSpriteImage(mWingIcon[i], null);
            }
        }
    }

    private void OnDataPropertyChanged(EventBase ev)
    {
        UpdatePlayerData();
    }

    private void UpdatePlayerData()
    {
        if (!DataManager.LevelTable.ContainsKey(mOtherData.level))
            return ;

        LevelTableItem levelRes = DataManager.LevelTable[mOtherData.level] as LevelTableItem;

        if (levelRes == null)
            return ;

        PropertyOperation op = new PropertyOperation();
        PropertyOperation pro = new PropertyOperation();
        PropertyBuild.BuildBaseProperty(mOtherData, pro);
        op.Add(pro);
        PropertyBuild.BuildEquipProperty(mOtherData, pro);
        op.Add(pro);
        PropertyBuild.BuildWeaponProperty(mOtherData, pro);
        op.Add(pro);
        PropertyBuild.BuildWingProperty(mOtherData, pro);
        op.Add(pro);

        int hp = (int)op.GetPro((int)PropertyTypeEnum.PropertyTypeMaxHP);
        mHp.text = hp.ToString();
        int mp = (int)op.GetPro((int)PropertyTypeEnum.PropertyTypeMaxMana);
        mMp.text = mp.ToString();
        int damage = (int)op.GetPro((int)PropertyTypeEnum.PropertyTypeDamage);
        mDamage.text = damage.ToString();
        int defence = (int)op.GetPro((int)PropertyTypeEnum.PropertyTypeDefance);
        mDefence.text = defence.ToString();
        int crit = (int)op.GetPro((int)PropertyTypeEnum.PropertyTypeCrticalLV);
        mCritLv.text = crit.ToString();


        uint grade = 0;
        for (int i = 0; i < mOtherData.mGrades.Grades.Length; ++i)
        {
            grade += mOtherData.mGrades[i];
        }

        mZhanli1.text = grade.ToString();
        mZhanli2.text = "战斗力:" + grade.ToString();

        mExp.text = mOtherData.exp.ToString() + "/" + levelRes.exp.ToString();

        mName.text = mOtherData.name;

        mRoleName.text = "Lv " + mOtherData.level.ToString() + "." + mOtherData.name;
    }

    private void UpdateEquips()
    {
        Dictionary<int, ItemObj> dic = mOtherData.mPack.getPackDic(PackageType.Pack_Equip);

        for( int i = 0 ; i < (int)EquipSlot.EquipSlot_MAX ; ++i )
        {
            if( dic.ContainsKey( i ) )
            {
                UpdateSingleEquip( i , dic[i] );
            }else{
                UpdateSingleEquip(i , null);
            }
        }

        int mainWeaponid = mOtherData.main_weaponId;
        if (!DataManager.WeaponTable.ContainsKey(mainWeaponid))
            return;


        WeaponObj wobj = mOtherData.mPack.GetItemByID(mainWeaponid, PackageType.Pack_Weapon) as WeaponObj;
        if (wobj == null)
            return;
        WeaponTableItem res = DataManager.WeaponTable[mainWeaponid] as WeaponTableItem;
        UIAtlasHelper.SetButtonImage(mWeaponIcon, res.picname);
        mWeaponSterLv.text = "+" + wobj.GetWeaponLv().ToString();
        mWeaponPromoteLv.text = wobj.GetPromoteLv().ToString();

        UIAtlasHelper.SetSpriteImage(mWeaponLvPic, wobj.GetWeaponLvPic());

        string zhishi = wobj.GetWeaponGradePic();
        if( string.IsNullOrEmpty(zhishi) )
        {
            UIAtlasHelper.SetSpriteImage(mWeaponGradeBg, "common:weaponlvbg1");
        }else
        {
            UIAtlasHelper.SetSpriteImage(mWeaponGradeBg, "common:weaponlvbg2");
        }
        UIAtlasHelper.SetSpriteImage(mWeaponGradePic, zhishi);
    }

    private void UpdateSingleEquip(int idx , ItemObj item)
    {
        if (idx < 0 || idx >= mEquipUI.Length)
        {
            return;
        }
        CommonItemUI ui = mEquipUI[idx];
        if( item != null )
        {
            ui.SetItemObj(item);
        }
        else
        {
            ui.Clear();
        }
        ui.SetOther(true);
    }


    public override void Update(uint elapsed)
    {
        mPreview.Update();
    }
}

