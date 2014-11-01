using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
class UIBagForm : UIWindow
{

    public enum BagSelect : int
    {
        Bag_Invaild = -1,
        Bag_All = 0,
        Bag_Item = 1,
        Bag_Gem = 2,
    }

    private CommonItemUI[] mEquipUI = new CommonItemUI[(int)EquipSlot.EquipSlot_MAX];

    private UIToggle mToggleAll = null;
    private UIToggle mToggleItem = null;
    private UIToggle mToggleGem = null;

    private UIButton mWeaponIcon = null;

    private UIGrid mItemsGrid = null;

    private GameObject mTitleRedPoint = null;

    private UIButton mTitleBtn = null;
    private UIButton mOpenBtn = null;
    private UIButton mBtnWeapon = null;

    private PlayerDataModule mDataModule = null;

    private GameObject mItemPrefab = null;
    private GameObject mLockItem = null;

    private BagSelect mSelect = BagSelect.Bag_All;

    private UIScrollView mScrollView = null;
    private UIScrollBar mScrollBar = null;

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

    private GameObject mMainCrops = null;
    private GameObject mSubCrops = null;




    private UILabel mRoleName = null;

    private UILabel mOpenLabel = null;
    //保存 所有Grid里面的格子数
    private ArrayList mItemList = new ArrayList();

    private UICharacterPreview mPreview = new UICharacterPreview();
    //private UIParticlePreview mPreview = new UIParticlePreview();
    private UISprite mPreveiwSprite;
    public UIBagForm()
    {

    }
    protected override void OnLoad()
    {
        base.OnLoad();

        for( int i = 0 ; i < (int)EquipSlot.EquipSlot_MAX ; ++i )
        {
            mEquipUI[i] = new CommonItemUI(this.FindChild("RolePanel/Equip" + (i + 1).ToString()));
        }

        for (int i = 0; i < msMaxWingNum; ++i )
        {
            mWingObject[i] = this.FindChild("ProperyPanel/AllDisScroll/Panel/Wings/WingScroll/Wing" + (i + 1).ToString());
            mWingIcon[i] = this.FindComponent<UISprite>("ProperyPanel/AllDisScroll/Panel/Wings/WingScroll/Wing" + (i + 1).ToString() + "/icon");
            mWingLevel[i] = this.FindComponent<UILabel>("ProperyPanel/AllDisScroll/Panel/Wings/WingScroll/Wing" + (i + 1).ToString()  +"/level");
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

        mItemPrefab = this.FindChild("Items/ItemUI");
        mLockItem = this.FindChild("ItemPanel/ItemsScrollView/LockUI");
        mScrollView = this.FindComponent<UIScrollView>("ItemPanel/ItemsScrollView");
        mScrollBar = this.FindComponent<UIScrollBar>("ItemPanel/ScrollBar");
        mScrollBar.gameObject.SetActive(true);
        mScrollBar.foregroundWidget.gameObject.SetActive(false);
        mWeaponIcon = this.FindComponent<UIButton>("RolePanel/bg/weapon");
        mToggleAll = this.FindComponent<UIToggle>("ItemPanel/BntAll");
        mToggleItem = this.FindComponent<UIToggle>("ItemPanel/BntItem");
        mToggleGem = this.FindComponent<UIToggle>("ItemPanel/BntGem");

        mPreveiwSprite = this.FindComponent<UISprite>("RolePanel/RoleModel");

        mTitleRedPoint = this.FindChild("RolePanel/bntChenghao/hongdian");

        mTitleBtn = this.FindComponent<UIButton>("RolePanel/bntChenghao");
        mOpenBtn = this.FindComponent<UIButton>("RolePanel/bntBag");
        mBtnWeapon = this.FindComponent<UIButton>("RolePanel/bg/weapon");

        mBagPanel = this.FindChild("ItemPanel");


        mInfoPanel = this.FindChild("ProperyPanel");


        mWeaponSterLv = this.FindComponent<UILabel>("RolePanel/weaponlv");
        mWeaponPromoteLv = this.FindComponent<UILabel>("RolePanel/jieji/Label");

        mWeaponGradeBg = this.FindComponent<UISprite>("RolePanel/zhishi");
        mWeaponGradePic = this.FindComponent<UISprite>("RolePanel/zhishi/lv");

        mWeaponLvPic = this.FindComponent<UISprite>("RolePanel/xingji");



        mOpenLabel = this.FindComponent<UILabel>("RolePanel/bntBag/Label");

        mOpenLabel.text = "角色信息";

        NGUITools.SetActive(mBagPanel.gameObject, true);
        NGUITools.SetActive(mInfoPanel.gameObject, false);

        mItemsGrid = this.FindComponent<UIGrid>("ItemPanel/ItemsScrollView/UIGrid");

        EventDelegate.Add(mToggleAll.onChange, onToggleChanged);
        EventDelegate.Add(mToggleItem.onChange, onToggleChanged);
        EventDelegate.Add(mToggleGem.onChange, onToggleChanged);
        EventDelegate.Add(mTitleBtn.onClick, onTitleBtnClick);
        EventDelegate.Add(mOpenBtn.onClick, onOpenButtonClick);
        EventDelegate.Add(mBtnWeapon.onClick, OnBtnWeaponClick);

        UIButton lockButton = mLockItem.GetComponent<UIButton>();

        EventDelegate.Add(lockButton.onClick, onLockButtonClick);

        mPreveiwSprite.width = mPreveiwSprite.height;
        mPreview.SetTargetSprite(mPreveiwSprite);
        mPreview.RotationY = 180;

    }

    private void onLockButtonClick()
    {
        PackageManager pack = mDataModule.GetPackManager();
        if( pack == null )
        {
            return;
        }
        PackageType type = PackageType.Invalid;
        if( mSelect == BagSelect.Bag_Item )
        {
            type = PackageType.Pack_Bag;
        }else if( mSelect == BagSelect.Bag_Gem )
        {
            type = PackageType.Pack_Gem;
        }

        if( type == PackageType.Invalid )
        {
            return;
        }

        int max_vaild_number = pack.GetPackMaxVaildSize(type);
        int max_number = pack.GetPackMaxSize(type);
        if (max_vaild_number >= max_number)
        {
            return;
        }


        int curNum = (int)PackExtendNum.MAX_PACK_EXTEND_NUM - ((max_number - max_vaild_number) / 3) + 1;

        if( !DataManager.PackageExtendTable.ContainsKey( curNum ) )
        {
            return ;
        }

        PackageTableItem item = DataManager.PackageExtendTable[curNum] as PackageTableItem;

        string str = "";
        if( item.moneyvalue <= 0 )
        {
            str = "当前第" +curNum.ToString() + "次扩充背包,本次免费,是否继续?";
        }else
        {
            str = "当前第" +curNum.ToString() + "次扩充背包,消耗"+ item.moneyvalue.ToString() + StringHelper.StringMoney(item.moneytype)  + ",是否继续?";
        }

        YesOrNoBoxManager.Instance.ShowYesOrNoUI("提示", str, OnUnlockYes);
    }

    private void OnUnlockYes(object para)
    {
        PackageManager pack = mDataModule.GetPackManager();

        PackageType type = PackageType.Invalid;
        if (mSelect == BagSelect.Bag_Item)
        {
            type = PackageType.Pack_Bag;
        }
        else if (mSelect == BagSelect.Bag_Gem)
        {
            type = PackageType.Pack_Gem;
        }
        if (type == PackageType.Invalid)
        {
            return;
        }
        int max_vaild_number = pack.GetPackMaxVaildSize(type);
        int max_number = pack.GetPackMaxSize(type);
        if (max_vaild_number >= max_number)
        {
            return;
        }

        int curNum = (int)PackExtendNum.MAX_PACK_EXTEND_NUM - ((max_number - max_vaild_number) / 3) + 1;

        PackageTableItem item = DataManager.PackageExtendTable[curNum] as PackageTableItem;

        if( mDataModule.GetProceeds((ProceedsType)item.moneytype) < item.moneyvalue )
        {
            //货币不足
            PromptUIManager.Instance.AddNewPrompt(string.Format(StringHelper.GetString("not_much_money"), StringHelper.StringMoney(item.moneytype)));
            return;
        }
        BagOpActionParam param = new BagOpActionParam();
        param.bagType = (int)type;
        param.op_type = (int)Message.BAG_OP_TYPE.BAG_OP_TYPE_UNLOCK;
        Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_BAG_OPERATION, param);
    }

    private void OnBtnWeaponClick()
    {
        int mainWeaponid = mDataModule.GetMainWeaponId();
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
        mDataModule = ModuleManager.Instance.FindModule<PlayerDataModule>();
        UpdateEquips();
        UpdateBags();
        UpdatePlayerData();
        OnWingUpdate(null);
        EventSystem.Instance.addEventListener(ItemEvent.UPDATE_CHANGE, ItemUpdateHandler);
        EventSystem.Instance.addEventListener(PropertyEvent.PLAYER_DATA_PROPERTY_CHANGED, OnDataPropertyChanged);
        EventSystem.Instance.addEventListener(WingUIEvent.Wing_UI_UPDATE, OnWingUpdate);
        EventSystem.Instance.addEventListener(ItemEvent.BAG_OP_UNLOCK, OnBagUnlock);

        Player player = PlayerController.Instance.GetControlObj() as Player;
        if (player != null)
        {
            PlayerPropertyModule module = ModuleManager.Instance.FindModule<PlayerPropertyModule>();

            mPreview.SetupCharacter(player.ModelID, module.GetEquipConfigs(),player.GetEquipWing(),uint.MaxValue);

            mPreview.ChangeWeapon(mDataModule.GetMainWeaponId());
            //mPreview.SetupParticle("ui_heiying");
        }
        onOpenBag();

        TitleModule tm = ModuleManager.Instance.FindModule<TitleModule>();
        mTitleRedPoint.SetActive(tm.IsShowRedPoint);
    }
    //界面关闭
    protected override void OnClose()
    {
        EventSystem.Instance.removeEventListener(ItemEvent.UPDATE_CHANGE, ItemUpdateHandler);
        EventSystem.Instance.removeEventListener(PropertyEvent.PLAYER_DATA_PROPERTY_CHANGED, OnDataPropertyChanged);
        EventSystem.Instance.removeEventListener(WingUIEvent.Wing_UI_UPDATE, OnWingUpdate);
        EventSystem.Instance.removeEventListener(ItemEvent.BAG_OP_UNLOCK, OnBagUnlock);

    }

    private void OnWingUpdate(EventBase e)
    {
        WingModule module = ModuleManager.Instance.FindModule<WingModule>();
        List<int> list =  module.GetUnlockWing();

        for (int i = 0; i < msMaxWingNum; ++i)
        {
            if( i < list.Count )
            {
                mWingObject[i].SetActive(true);
                WingCommonTableItem commonRes = DataManager.WingCommonTable[list[i]] as WingCommonTableItem;
                if (commonRes != null)
                {
                    ItemTableItem  itemRes = ItemManager.GetItemRes(commonRes.costId);
                    if( itemRes != null )
                    {
                        UIAtlasHelper.SetSpriteImage(mWingIcon[i] , itemRes.picname);
                        uint wingLevel = module.GetWingLevel(list[i]);
                        if (wingLevel > 0)
                        {
                            mWingLevel[i].gameObject.SetActive(true);
                            mWingLevel[i].text = "Lv " + wingLevel;
                        }
                        else
                        {
                            mWingLevel[i].gameObject.SetActive(false);
                        }
                    }
                }
            }else
            {
                mWingObject[i].SetActive(false);
                UIAtlasHelper.SetSpriteImage(mWingIcon[i], null);
            }
        }
    }

    private void onTitleBtnClick()
    {
        WindowManager.Instance.OpenUI("title");

        TitleModule tm = ModuleManager.Instance.FindModule<TitleModule>();
        tm.IsShowRedPoint = false;
        mTitleRedPoint.SetActive(false);
        CityFormManager.SetRedPointActive("beibao", false);
    }

    private void onOpenButtonClick()
    {
        if (mBagPanel.activeSelf)
        {
            onOpenRole();
        }
        else
        {
            onOpenBag();
        }
    }

    private void onOpenBag()
    {
        mOpenLabel.text = "角色信息";
        mBagPanel.SetActive(true);
        mInfoPanel.SetActive(false);
        mToggleAll.value = true;
    }
    private void onOpenRole()
    {
        mOpenLabel.text = "打开背包";
        mBagPanel.SetActive(false);
        mInfoPanel.SetActive(true);     
    }

    private void onToggleChanged()
    {
        BagSelect sel = BagSelect.Bag_Invaild;
        if (mToggleAll.value)
            sel = BagSelect.Bag_All;
        if (mToggleItem.value)
            sel = BagSelect.Bag_Item;
        if (mToggleGem.value)
            sel = BagSelect.Bag_Gem;

        if (sel == BagSelect.Bag_Invaild)
            return;
        if( mSelect != sel )
        {
            mSelect = sel;
            UpdateBags();
        }
    }

    private void OnDataPropertyChanged(EventBase ev)
    {
        UpdatePlayerData();
    }

    private void UpdatePlayerData()
    {
        PlayerPropertyModule propertyModule = ModuleManager.Instance.FindModule<PlayerPropertyModule>();

        PlayerDataModule dataModule = ModuleManager.Instance.FindModule<PlayerDataModule>();

        if( propertyModule == null || dataModule == null )
        {
            return;
        }
        PropertyOperation op = propertyModule.GetPlayerProperty();
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

        mZhanli1.text = dataModule.GetGrade().ToString();
        mZhanli2.text = "战斗力:" + dataModule.GetGrade().ToString();

        mExp.text = dataModule.GetExp().ToString() + "/" + dataModule.GetMaxExp().ToString();

        mName.text = dataModule.GetName();

        mRoleName.text = "Lv " + dataModule.GetLevel().ToString() + "." + dataModule.GetName();

        SetCropsInfo(mMainCrops, mSubCrops);
    }

    private void UpdateBags()
    {
        PackageManager pack = mDataModule.GetPackManager();

        for (int i = 0; i < mItemList.Count; ++i)
        {
            CommonItemUI grid = mItemList[i] as CommonItemUI;
            grid.gameObject.SetActive(false);
        }
        int idx = 0;
        //选中所有
        if( mSelect == BagSelect.Bag_All )
        {
            
            Dictionary<int, ItemObj> dic = pack.getPackDic(PackageType.Pack_Bag);
            foreach (KeyValuePair<int, ItemObj> value in dic)
            {
                UpdateBagGrid(idx++, value.Value);
            }

            dic = pack.getPackDic(PackageType.Pack_Gem);
            foreach (KeyValuePair<int, ItemObj> value in dic)
            {
                UpdateBagGrid(idx++, value.Value);
            }

            mLockItem.SetActive(false);
        }
        else if (mSelect == BagSelect.Bag_Item || mSelect == BagSelect.Bag_Gem)
        {
            PackageType type = PackageType.Pack_Bag;
            if (mSelect == BagSelect.Bag_Gem)
                type = PackageType.Pack_Gem;

            Dictionary<int, ItemObj> dic = pack.getPackDic(type);
            foreach (KeyValuePair<int, ItemObj> value in dic)
            {
                UpdateBagGrid(idx++, value.Value);
            }
            int max_vaild_number = pack.GetPackMaxVaildSize(type);
            int max_number= pack.GetPackMaxSize(type);
            if (idx < max_vaild_number)
            {
                for (int i = idx; i < max_vaild_number; ++i)
                {
                    UpdateBagGrid(idx++, null);
                }
            }

            //还有扩充空间
            if (max_vaild_number < max_number)
            {
                if (mLockItem.transform.parent != null)
                {
                    mLockItem.transform.parent = null;
                }
                mLockItem.transform.parent = mItemsGrid.transform;
                mLockItem.transform.localScale = Vector3.one;
                //增加解锁节点
                mLockItem.SetActive(true);
     
            }
            else
            {
                mLockItem.SetActive(false);
            }
        }
        mItemsGrid.Reposition();// = true;

        if( mLockItem.activeSelf )
        {
            Vector3 pos = mLockItem.transform.localPosition;
            pos.x = 130;
            mLockItem.transform.localPosition = pos;
        }
        mScrollView.ResetPosition();
    }

    private void UpdateBagGrid(int idx , ItemObj item)
    {
        if( idx >= mItemList.Count )
        {
            CommonItemUI itemui = new CommonItemUI(item);
            itemui.gameObject.transform.parent = mItemsGrid.transform;
            itemui.gameObject.transform.localScale = Vector3.one;
            itemui.gameObject.transform.localPosition = Vector3.zero;
            itemui.gameObject.SetActive(false);
            itemui.SetBoxSize(130.0f, 130.0f);
            mItemList.Add(itemui);
        }

        CommonItemUI grid = mItemList[idx] as CommonItemUI;

        if (item != null)
        {
            grid.SetItemObj(item);
        }else
        {
            grid.SetItemObj(null);
        }
        grid.gameObject.name = "ItemUI" + idx.ToString();
        grid.gameObject.SetActive(true);

    }

    private void UpdateEquips()
    {
        PackageManager pack = mDataModule.GetPackManager();

        Dictionary<int, ItemObj> dic = pack.getPackDic(PackageType.Pack_Equip);

        for( int i = 0 ; i < (int)EquipSlot.EquipSlot_MAX ; ++i )
        {
            if( dic.ContainsKey( i ) )
            {
                UpdateSingleEquip( i , dic[i] );
            }else{
                UpdateSingleEquip(i , null);
            }
        }

        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        int mainWeaponid = mDataModule.GetMainWeaponId();
        if (!DataManager.WeaponTable.ContainsKey(mainWeaponid))
            return;


        WeaponObj wobj = module.GetItemByID(mainWeaponid, PackageType.Pack_Weapon) as WeaponObj;
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
    }

    private void SetScrollBar(float val)
    {
        mScrollBar.value =val; 
    }

    private void OnBagUnlock(EventBase e)
    {
        if( mSelect == BagSelect.Bag_Gem || mSelect == BagSelect.Bag_Item )
        {
            SetScrollBar(1.0f);
        }
    }

    private void ItemUpdateHandler(EventBase e)
    {
        ItemEvent evt = (ItemEvent)e;
        if( evt.bagType == PackageType.Pack_Bag ||
            evt.bagType == PackageType.Pack_Gem)
        {
            UpdateBags();
        }

        if( evt.bagType == PackageType.Pack_Equip )
        {
            UpdateEquips();
        }
    }

    private void ItemBagAdd(EventBase e)
    {
        ItemEvent evt = (ItemEvent)e;
        if (evt.bagType == PackageType.Pack_Bag ||
            evt.bagType == PackageType.Pack_Gem)
        {
            UpdateBags();
        }

        if (evt.bagType == PackageType.Pack_Equip)
        {
            UpdateEquips();
        }
    }

    public GameObject FindBagItem(int itemid)
    {
        for(int i = 0 ; i < mItemList.Count ; ++i )
        {
            CommonItemUI grid = mItemList[i] as CommonItemUI;
            if (!grid.gameObject.activeSelf)
                break;
            if( grid.GetItemID() == itemid )
            {
                return grid.gameObject;
            }
        }
        return null;
    }
    public GameObject FindEquipItem(int itemid)
    {
        for (int i = 0; i < mEquipUI.Length; ++i)
        {
            CommonItemUI grid = mEquipUI[i] as CommonItemUI;
            if (!grid.gameObject.activeSelf)
                break;
            if (grid.GetItemID() == itemid)
            {
                return grid.gameObject;
            }
        }
        return null;
    }
    public override void Update(uint elapsed)
    {
        mPreview.Update();
    }

    public void SetCropsInfo(GameObject main, GameObject sub)
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (null == module)
            return;
       
        if (-1 != module.GetMainCropsId())
        {
            CropsTableItem item = DataManager.CropsTable[module.GetMainCropsId()] as CropsTableItem;
            if (null == item)
            {
                main.SetActive(false);
                return;
            }
            UISprite sp = ObjectCommon.GetChildComponent<UISprite>(main, "pic");
            UIAtlasHelper.SetSpriteImage(sp, item.cropsheadpic);

            int starslv = module.GetCropsStarsLv(module.GetMainCropsId());
            for (int i = 0; i < 5; ++i)
            {
                sp = ObjectCommon.GetChildComponent<UISprite>(main, "stars" + (i + 1));
                if (i <= starslv - 1)
                {
                    UIAtlasHelper.SetSpriteImage(sp, "common:strenth (11)");
                }
                else
                {
                    UIAtlasHelper.SetSpriteImage(sp, null);
                }
            }
            main.SetActive(true);
        }

        if (-1 != module.GetSubCropsId())
        {
            CropsTableItem item = DataManager.CropsTable[module.GetSubCropsId()] as CropsTableItem;
            if (null == item)
            {
                sub.SetActive(false);
                return;
            }
            UISprite sp = ObjectCommon.GetChildComponent<UISprite>(sub, "pic");
            UIAtlasHelper.SetSpriteImage(sp, item.cropsheadpic);

            int starslv = module.GetCropsStarsLv(module.GetSubCropsId());
            for (int i = 0; i < 5; ++i)
            {
                sp = ObjectCommon.GetChildComponent<UISprite>(sub, "stars" + (i + 1));
                if (i <= starslv - 1)
                {
                    UIAtlasHelper.SetSpriteImage(sp, "common:strenth (11)");
                }
                else
                {
                    UIAtlasHelper.SetSpriteImage(sp, null);
                }
            }
            sub.SetActive(true);
        }
    }
}

