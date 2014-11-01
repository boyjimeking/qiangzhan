using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using Message;

public class ItemInfoParam
{
    public int itemid = -1; //防具ID
    public ItemType itemtype = ItemType.Invalid;
    public PackageType packtype = PackageType.Invalid;
    public int packpos = -1;
}

public class UIItemInfoForm : UIWindow
{
    //Button
    private UIButton mBtnAbtain = null;
    private UIButton mBtnStoneComb = null;
    private UIButton mBtnSale = null;
    private UILabel mBtnSaleLB = null;
    private UIButton mBtnJunhuoku = null;
    private UIButton mBtnFitAbtain = null;
    //~Btn

    #region 普通道具
    private UILabel mItemName = null;
    private UILabel mItemUseLv = null;
    private UILabel mItemDesc = null;
    private UILabel mItemAbtain = null;
    private UILabel mSaleMoney = null;
    private UISprite mItemPic = null;
    private UISprite mItemPic1 = null;
    private UILabel mItemLb = null;
    private UILabel mItemType = null;
    #endregion

    #region 枪械
    private UILabel mWeaponName = null;
    private UILabel mWeaponUseLv = null;
    private UILabel mWeaponStrLv = null;
    private UILabel mWeaponBaseDamage = null;
    private UILabel mWeaponStrDamage = null;
    private UILabel mWeaponBounsDamage = null;
    private UILabel mWeaponFireSpeed = null;
    private UILabel mWeaponDesc = null;
    private UILabel mWeaponAbtain = null;
    private UISprite mWeaponPic = null;
    private UILabel mNumLb = null;
    private List<UISprite> mWeaponStrList = new List<UISprite>();
    #endregion

    #region 配件
    private UILabel mFitName = null;
    private UILabel mFitType = null;
    private UILabel mFitUseLv = null;
    private List<UILabel> mFitProNameList = new List<UILabel>();
    private List<UILabel> mFitproValueList = new List<UILabel>();
    private UILabel mFitDesc = null;
    private UILabel mFitAbtain = null;

    private UISprite mFitPic = null;
    #endregion

    //GameObject
    private GameObject mNormalItemPanel = null;
    private GameObject mWeaponItemPanel = null;
    private GameObject mFitItemPanel = null;
    ItemInfoParam uiparam = null;

    private const uint STREN_STEP = 10;
    private const uint PEIJIANSTART = 1000023;
    private const uint PEIJIANEND = 1000034;
    private ItemType PEIJIANFROMWEAPON = ItemType.Invalid;

    private Dictionary<int, int> dic = new Dictionary<int, int>();
    public UIItemInfoForm()
    {

    }
    protected override void OnLoad()
    {
        base.OnLoad();

        //btn
        mBtnStoneComb = this.FindComponent<UIButton>("btnstonecomb");
        mBtnAbtain = this.FindComponent<UIButton>("btnabtain");
        mBtnSale = this.FindComponent<UIButton>("btnsale");
        mBtnSaleLB = this.FindComponent<UILabel>("btnsale/Label");
        mBtnJunhuoku = this.FindComponent<UIButton>("btnJunHuoKu");
        mBtnFitAbtain = this.FindComponent<UIButton>("btnfitabtain");
        //~btn
        #region 普通道具
        mItemName = this.FindComponent<UILabel>("normalitem/itemname");
        mItemUseLv = this.FindComponent<UILabel>("normalitem/uselv");
        mItemDesc = this.FindComponent<UILabel>("normalitem/itemdesc");
        mItemAbtain = this.FindComponent<UILabel>("normalitem/itemabtain");
        mSaleMoney = this.FindComponent<UILabel>("normalitem/salemoney/Label");
        mItemPic = this.FindComponent<UISprite>("normalitem/itembg/icon");
        mItemPic1 = this.FindComponent<UISprite>("normalitem/itembg/sprite1");
        mItemLb = this.FindComponent<UILabel>("normalitem/itembg/label1");
        mItemType = this.FindComponent<UILabel>("normalitem/itemlv");
        #endregion

        #region 枪械
        mWeaponName = this.FindComponent<UILabel>("weaponitem/itemname");
        mWeaponUseLv = this.FindComponent<UILabel>("weaponitem/uselv");
        mWeaponStrLv = this.FindComponent<UILabel>("weaponitem/strenlv");
        mWeaponBaseDamage = this.FindComponent<UILabel>("weaponitem/basedamage");
        mWeaponStrDamage = this.FindComponent<UILabel>("weaponitem/strendamage");
        mWeaponBounsDamage = this.FindComponent<UILabel>("weaponitem/bounsdamage");
        mWeaponFireSpeed = this.FindComponent<UILabel>("weaponitem/firespeed");
        mWeaponDesc = this.FindComponent<UILabel>("weaponitem/weapondesc");
        mWeaponAbtain = this.FindComponent<UILabel>("weaponitem/weaponabtain");
        mNumLb = this.FindComponent<UILabel>("weaponitem/weaponbg/weaonpic/numLb");

        mWeaponPic = this.FindComponent<UISprite>("weaponitem/weaponbg/weaonpic");

        for (int i = 0; i < (int)STARS_RANK.MAX_STARS_RANK_NUMBER; ++i)
        {
            mWeaponStrList.Add(this.FindComponent<UISprite>("weaponitem/stars" + (i + 1)));
        }
        #endregion

        #region 配件
        mFitName = this.FindComponent<UILabel>("fittingsitem/itemname");
        mFitType = this.FindComponent<UILabel>("fittingsitem/Label");
        mFitUseLv = this.FindComponent<UILabel>("fittingsitem/uselv");
        mFitProNameList.Add(this.FindComponent<UILabel>("fittingsitem/proname1"));
        mFitProNameList.Add(this.FindComponent<UILabel>("fittingsitem/proname2"));
        mFitProNameList.Add(this.FindComponent<UILabel>("fittingsitem/proname3"));
        mFitproValueList.Add(this.FindComponent<UILabel>("fittingsitem/basedamage"));
        mFitproValueList.Add(this.FindComponent<UILabel>("fittingsitem/strendamage"));
        mFitproValueList.Add(this.FindComponent<UILabel>("fittingsitem/bounsdamage"));
        mFitDesc = this.FindComponent<UILabel>("fittingsitem/weapondesc");
        mFitAbtain = this.FindComponent<UILabel>("fittingsitem/weaponabtain");

        mFitPic = this.FindComponent<UISprite>("fittingsitem/weaponbg/weaonpic");
        #endregion

        //GameObject
        mNormalItemPanel = this.FindChild("normalitem");
        mWeaponItemPanel = this.FindChild("weaponitem");
        mFitItemPanel = this.FindChild("fittingsitem");

        #region NormalItem-FittingsItem
        dic.Add(1000023, 20);
        dic.Add(1000024, 10);
        dic.Add(1000025, 15);
        dic.Add(1000026, 30);
        dic.Add(1000027, 25);
        dic.Add(1000028, 5);
        dic.Add(1000029, 50);
        dic.Add(1000030, 40);
        dic.Add(1000031, 45);
        dic.Add(1000032, 60);
        dic.Add(1000033, 55);
        dic.Add(1000034, 35);
        #endregion
    }
    protected override void OnOpen(object param = null)
    {
        PEIJIANFROMWEAPON = ItemType.Invalid;
        uiparam = (ItemInfoParam)param;
        if (uiparam.itemtype != ItemType.Invalid)
            PEIJIANFROMWEAPON = ItemType.Fittings;
        InitPanelActive();
        InitUI();
    }
    protected override void OnClose()
    {
        if (ItemType.Fittings == uiparam.itemtype && ItemType.Fittings == PEIJIANFROMWEAPON)
            WindowManager.Instance.OpenUI("weapon");

        EventSystem.Instance.removeEventListener(ItemEvent.ITEM_SALE_ALL, OnSaleItemAllHandler);
        EventSystem.Instance.removeEventListener(ItemEvent.ITEM_SALE_PART, OnSaleItemPartHandler);
        EventDelegate.Remove(mBtnAbtain.onClick, OnBtnAbtainHandler);
        EventDelegate.Remove(mBtnStoneComb.onClick, OnBtnStoneCombHandler);
        EventDelegate.Remove(mBtnSale.onClick, OnBtnSaleHandler);
        EventDelegate.Remove(mBtnJunhuoku.onClick, OnBtnJunhuokuHandler);
        EventDelegate.Remove(mBtnFitAbtain.onClick, OnBtnFitAbtainHandler);
    }
    public override void Update(uint elapsed)
    {

    }

    private void InitUI()
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        InitItemInfo();

        EventSystem.Instance.addEventListener(ItemEvent.ITEM_SALE_ALL, OnSaleItemAllHandler);
        EventSystem.Instance.addEventListener(ItemEvent.ITEM_SALE_PART, OnSaleItemPartHandler);

        EventDelegate.Add(mBtnAbtain.onClick,OnBtnAbtainHandler);
        EventDelegate.Add(mBtnStoneComb.onClick, OnBtnStoneCombHandler);
        EventDelegate.Add(mBtnSale.onClick, OnBtnSaleHandler);
        EventDelegate.Add(mBtnJunhuoku.onClick, OnBtnJunhuokuHandler);
        EventDelegate.Add(mBtnFitAbtain.onClick, OnBtnFitAbtainHandler);
    }

    private void InitPanelActive()
    {
        mBtnSaleLB.text = "出售";

        switch ((int)uiparam.itemtype)
        {
            case (int)ItemType.Box:
                {
                    mNormalItemPanel.SetActive(true);
                    mWeaponItemPanel.SetActive(false);
                    mFitItemPanel.SetActive(false);
                    mBtnStoneComb.gameObject.SetActive(false);
                    mBtnJunhuoku.gameObject.SetActive(false);
                    mBtnAbtain.gameObject.SetActive(false);
                    mBtnSale.gameObject.SetActive(true);
                    mBtnSaleLB.text = "打开箱子";
                    mBtnFitAbtain.gameObject.SetActive(false);
                }
                break;
            case (int)ItemType.Normal:
                mNormalItemPanel.SetActive(true);
                mWeaponItemPanel.SetActive(false);
                mFitItemPanel.SetActive(false);
                mBtnStoneComb.gameObject.SetActive(false);
                mBtnJunhuoku.gameObject.SetActive(false);
                mBtnAbtain.gameObject.SetActive(true);
                mBtnSale.gameObject.SetActive(true);
                mBtnFitAbtain.gameObject.SetActive(false);
                break;
            case (int)ItemType.Stone:
                mNormalItemPanel.SetActive(true);
                mWeaponItemPanel.SetActive(false);
                mFitItemPanel.SetActive(false);
                mBtnStoneComb.gameObject.SetActive(true);
                mBtnJunhuoku.gameObject.SetActive(false);
                mBtnAbtain.gameObject.SetActive(true);
                mBtnSale.gameObject.SetActive(true);
                mBtnFitAbtain.gameObject.SetActive(false);
                break;
            case (int)ItemType.Weapon:
                mNormalItemPanel.SetActive(false);
                mWeaponItemPanel.SetActive(true);
                mFitItemPanel.SetActive(false);
                mBtnStoneComb.gameObject.SetActive(false);
                mBtnJunhuoku.gameObject.SetActive(true);
                mBtnAbtain.gameObject.SetActive(false);
                mBtnSale.gameObject.SetActive(false);
                mBtnFitAbtain.gameObject.SetActive(false);
                break;
            case (int)ItemType.Fittings:
                mNormalItemPanel.SetActive(false);
                mWeaponItemPanel.SetActive(false);
                mFitItemPanel.SetActive(true);
                mBtnStoneComb.gameObject.SetActive(false);
                mBtnJunhuoku.gameObject.SetActive(false);
                mBtnAbtain.gameObject.SetActive(false);
                mBtnSale.gameObject.SetActive(false);
                mBtnFitAbtain.gameObject.SetActive(true);
                break;
            case (int)ItemType.Invalid:
                mNormalItemPanel.SetActive(false);
                mWeaponItemPanel.SetActive(false);
                mFitItemPanel.SetActive(false);
                mBtnStoneComb.gameObject.SetActive(false);
                mBtnJunhuoku.gameObject.SetActive(false);
                mBtnAbtain.gameObject.SetActive(false);
                mBtnSale.gameObject.SetActive(false);
                mBtnFitAbtain.gameObject.SetActive(false);
                break;
        }
    }

    private void InitItemInfo()
    {
        switch ((int)uiparam.itemtype)
        {
            case (int)ItemType.Normal:
            case (int)ItemType.Box:
                InitNormalItemInfo();
                break;
            case (int)ItemType.Stone:
                InitStoneItemInfo();
                break;
            case (int)ItemType.Weapon:
                InitWeaponItemInfo();
                break;
            case (int)ItemType.Fittings:
                InitFittingsItemInfo();
                break;
            case (int)ItemType.Invalid:
                InitInvalidItemInfo();
                break;
        }
    }
    private void InitNormalItemInfo()
    {
        ItemTableItem item = ItemManager.GetItemRes(uiparam.itemid); //DataManager.NormalItemTable[uiparam.itemid] as NormalItemTableItem;
        if (null == item)
            return;
        ConfigTableItem configitem = DataManager.ConfigTable[item.quality] as ConfigTableItem;
        if(null == configitem)
            return;
        mItemName.text = "[" + configitem.value + "]" + item.name;
        mItemType.text = "物品类型";
        mItemUseLv.text = item.desc1;
        mItemDesc.text = item.desc;
        mItemAbtain.text = item.desc0;
        mSaleMoney.text = item.gameprice.ToString();
        SetIcon(mItemPic, item.picname);
        SetIcon(mItemPic1, item.picname2);
        mItemLb.text = item.picname3;
    }

    private void InitInvalidItemInfo()
    {
        if (uiparam.itemtype == ItemType.Invalid)
            uiparam.itemtype = ItemManager.GetItemType((uint)uiparam.itemid);

        if (uiparam.itemid >= PEIJIANSTART && uiparam.itemid <= PEIJIANEND)
        {
            uiparam.itemtype = ItemType.Fittings;//配件类型
            uiparam.itemid = dic[uiparam.itemid];
        }

        switch ((int)uiparam.itemtype)
        {
            case (int)ItemType.Box:
            case (int)ItemType.Normal:
                InitNormalItemInfo();
                mNormalItemPanel.SetActive(true);
                break;
            case (int)ItemType.Stone:
                InitStoneItemInfo();
                mNormalItemPanel.SetActive(true);
                break;
            case (int)ItemType.Weapon:
                InitWeaponItemInfo();
                mWeaponItemPanel.SetActive(true);
                break;
            case (int)ItemType.Fittings:
                InitFittingsItemInfo();
                mFitItemPanel.SetActive(true);
                break;
            case (int)ItemType.Invalid:
                break;
        }
    }

    private void InitWeaponItemInfo()
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if(null == module)
            return;
        WeaponObj mData = module.GetItemByID(uiparam.itemid,PackageType.Pack_Weapon) as WeaponObj;
        if(null == mData)
            return;
        WeaponTableItem item = DataManager.WeaponTable[uiparam.itemid] as WeaponTableItem;
        if (null == item)
            return;
        ConfigTableItem configitem = DataManager.ConfigTable[item.quality] as ConfigTableItem;
        if (null == configitem)
            return;
        PromoteTableItem pres = DataManager.PromoteTable[mData.GetPromoteLv() + item.upgrade] as PromoteTableItem;
        if (pres == null)
        {
            GameDebug.LogError("进阶promote.txt表格无此ID=" + mData.GetPromoteLv());
            return;
        }
        StrenTableItem stritem = DataManager.StrenTable[module.GetStrenLv()] as StrenTableItem;
        if(null == stritem)
            return;

        uint lv = module.GetStrenLv();
        int starlv = (int)(lv / STREN_STEP);
        if (starlv > 0 && (lv % STREN_STEP) == 0)
            starlv -= 1;
        uint showlv = lv == 0 ? 0 : (lv % STREN_STEP == 0 ? 10 : lv % STREN_STEP);
        for (uint i = 0; i < showlv; ++i)
        {
            SetIcon(mWeaponStrList[(int)i], "common:strenth (" + (starlv + 7) + ")");
        }

        for (uint i = showlv; i < STREN_STEP; ++i)
        {
            SetIcon(mWeaponStrList[(int)i], "common:starslvback");
        }

        mWeaponName.text = "[" + configitem.value + "]" + item.name;
        mWeaponUseLv.text = mData.GetPromoteLv() + "阶";
        mWeaponStrLv.text = module.GetStrenLv() + "级";
        mWeaponBaseDamage.text = pres.value.ToString();
        mWeaponStrDamage.text = stritem.value.ToString();
        mWeaponDesc.text = item.desc;
        mWeaponAbtain.text = item.desc0;
        mNumLb.text = mData.GetPromoteLv().ToString();

        //SetIcon(mWeaponPic, item.picname);
    }

    private void InitStoneItemInfo()
    {
        StoneTableItem item = DataManager.StoneTable[uiparam.itemid] as StoneTableItem;
        if (null == item)
            return;
        ConfigTableItem configitem = DataManager.ConfigTable[item.quality] as ConfigTableItem;
        if (null == configitem)
            return;
        mItemName.text = "[" + configitem.value + "]" + item.name;
        mItemType.text = "物品类型";
        mItemUseLv.text = item.desc1;
        mItemDesc.text = item.desc;
        mItemAbtain.text = item.desc0;
        mSaleMoney.text = item.gameprice.ToString();
        SetIcon(mItemPic, item.picname);
        SetIcon(mItemPic1, item.picname2);
        mItemLb.text = item.picname3;

    }

    private void InitFittingsItemInfo()
    {
        WeaponModule wmodule = ModuleManager.Instance.FindModule<WeaponModule>();
        if (wmodule == null)
            return;

        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;
        FittingsTableItem fres = DataManager.FittingsTable[uiparam.itemid] as FittingsTableItem;
        if (fres == null)
        {
            GameDebug.LogError("资源ID为" + uiparam.itemid + "不存在表格fittings.txt中 ");
            return;
        }
        ConfigTableItem configitem = DataManager.ConfigTable[fres.grade] as ConfigTableItem;
        if (null == configitem)
            return;
        mFitName.text = "[" + configitem.value + "]" + fres.name;
        mFitType.text = "物品类型";
        mFitUseLv.text = fres.itemtype;
        mFitDesc.text = fres.itemdesc;
        mFitAbtain.text = fres.itemabtain;

        SetIcon(mFitPic, fres.bmp);
        SetIcon(mItemPic1, null);
        mItemLb.text = "";

        FittingsData fdata = module.GetFittingsData((uint)uiparam.packpos);
        if (fdata == null)
        {
            for (int i = 0; i < (int)FittingsType.MAX_PROPERTY; ++i)
            {
                mFitProNameList[i].text = "";
                mFitproValueList[i].text = "";
            }
            mFitProNameList[0].text = "配件未洗练！";
            return;
        }

        for (int i = 0; i < (int)FittingsType.MAX_PROPERTY; ++i)
        {
            int proid = -1;
            int value = -1;
            bool forbid = false;
            if (!fdata.GetProValue((uint)i, ref proid, ref value, ref forbid))
                continue;

            string pname = RoleProperty.GetPropertyName(proid);
            if ("error".Equals(pname))
            {
                pname = "属性名称";
            }
            mFitProNameList[i].text = pname;

            int min = 0;
            int max = 1;
            if (!wmodule.GetFittMinMax(uiparam.itemid, proid, ref min, ref max))
            {
                mFitproValueList[i].text = "该条属性尚未洗炼！";
                continue;
            }

            mFitproValueList[i].text = value.ToString();

            /*int proindex = FittingsProperty.GetResId(proid);
            string colorss = "000000";
            Hashtable map = DataManager.FittcolorTable;
            foreach (FittcolorTableItem fcres in map.Values)
            {
                if (fcres.qualityid == fres.grade && value > System.Convert.ToInt32(fcres["max_" + proindex]))
                    continue;

                colorss = fcres.color;
                break;
            }*/
        }
    }

    private void SetIcon(UISprite sprite,string pic)
    {
        UIAtlasHelper.SetSpriteImage(sprite, pic);
    }

    private void OnBtnAbtainHandler()
    {
        PopTipManager.Instance.AddNewTip(StringHelper.GetString("about_to_open"));
    }

    private void OnBtnStoneCombHandler()
    {
        WindowManager.Instance.OpenUI("stonecomb", uiparam);
    }

    private void OnBtnSaleHandler()
    {
        ItemType  itemtype = ItemManager.GetItemType((uint)uiparam.itemid);
        if( itemtype == ItemType.Box )
        {
            OnOpenItemBox();
            //todo
            return;
        }

        ItemUIParam param = new ItemUIParam();
        param.itemid = uiparam.itemid;
        param.isSaleAll = true;
        param.packtype = uiparam.packtype;
        param.packpos = uiparam.packpos;

        WindowManager.Instance.OpenUI("itemsale", param);
    }


    private void OnOpenItemBox()
    {
        int resid = uiparam.itemid;
        BoxItemTableItem res = ItemManager.GetItemRes(resid) as BoxItemTableItem;
        if( res == null )
        {
            return;
        }
        string conditiontext = "";
        if( res.condition1 >= 0 )
        {
            conditiontext = ConditionManager.Instance.GetConditionText(res.condition1); 
        }
        if (res.condition2 >= 0)
        {
            conditiontext += ConditionManager.Instance.GetConditionText(res.condition2);
        }

        YesOrNoBoxManager.Instance.ShowYesOrNoUI("提示", "开启宝箱需要: " + conditiontext, OnOpenBox);
    }

    private void OnOpenBox(object para)
    {
        int resid = uiparam.itemid;
        BoxItemTableItem res = ItemManager.GetItemRes(resid) as BoxItemTableItem;
        if (res == null)
        {
            return;
        }
        if (res.condition1 >= 0)
        {
            if (!ConditionManager.Instance.CheckCondition(res.condition1))
            {
                PopTipManager.Instance.AddNewTip(StringHelper.GetString("not_condition"));
                return ;
            }
        }
        if (res.condition2 >= 0)
        {
            if (!ConditionManager.Instance.CheckCondition(res.condition2))
            {
                PopTipManager.Instance.AddNewTip(StringHelper.GetString("not_condition"));
                return;
            }
        }

        BoxItemActionParam param = new BoxItemActionParam();
        param.op_type = (int)BOX_ITEM_OP_TYPE.OP_TYPE_OPEN;
        param.bagType = (int)uiparam.packtype;
        param.bagPos = uiparam.packpos;

        Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_BOX_ITEM, param);
    }


    private void OnBtnJunhuokuHandler()
    {
        WindowManager.Instance.OpenUI("weapon");
    }

    private void OnSaleItemAllHandler(EventBase evt)
    {
        WindowManager.Instance.CloseUI("itemsale");
        WindowManager.Instance.CloseUI("iteminfo");
        WindowManager.Instance.CloseUI("saleagain");
    }

    private void OnSaleItemPartHandler(EventBase evt)
    {
        WindowManager.Instance.CloseUI("itemsale");
        WindowManager.Instance.CloseUI("saleagain");
    }

    private void OnBtnFitAbtainHandler()
    {
        PopTipManager.Instance.AddNewTip(StringHelper.GetString("about_to_open"));
    }
}
