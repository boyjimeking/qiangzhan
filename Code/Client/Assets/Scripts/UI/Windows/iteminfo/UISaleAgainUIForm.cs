using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using Message;

public class UISaleAgainUIForm : UIWindow
{
    //Button
    private UIButton mBtnCancle = null;
    private UIButton mBtnYes = null;
    //~Btn
    private UILabel mLabel1 = null;
    private UILabel mLabel2 = null;

    ItemUIParam mParam = null;
    public UISaleAgainUIForm()
    {

    }
    protected override void OnLoad()
    {
        base.OnLoad();

        //btn
        mBtnCancle = this.FindComponent<UIButton>("btnCancle");
        mBtnYes = this.FindComponent<UIButton>("btnYes");
        
        //~btn
        mLabel1 = this.FindComponent<UILabel>("label1");
        mLabel2 = this.FindComponent<UILabel>("label2");
        

    }
    protected override void OnOpen(object param = null)
    {
        mParam = (ItemUIParam)param;
        InitUI();
        InitLabel();
    }
    protected override void OnClose()
    {

    }
    public override void Update(uint elapsed)
    {
    }

    private void InitUI()
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        EventDelegate.Add(mBtnCancle.onClick, OnBtnCancleHandler);
        EventDelegate.Add(mBtnYes.onClick, OnBtnYesHandler);
    }

    private void OnBtnCancleHandler()
    {
        SoundManager.Instance.Play(15);
        WindowManager.Instance.CloseUI("saleagain");
    }

    private void OnBtnYesHandler()
    {
        SoundManager.Instance.Play(15);
        if (ItemManager.GetItemType((uint)mParam.itemid) == ItemType.Defence)
        {
            DefenceModule module = ModuleManager.Instance.FindModule<DefenceModule>();
            if (null == module)
                return;
            DefenceUIParam param = new DefenceUIParam();
            param.itemid = mParam.itemid;
            param.packpos = mParam.packpos;
            param.packtype = mParam.packtype;
            module.SaleDefence(param);
        }
        else
        {
            ItemSaleActionParam param = new ItemSaleActionParam();
            param.itemcount = mParam.mSaleNum;
            param.itemid = mParam.itemid;
            param.isSaleAll = mParam.isSaleAll;
            Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_ITEM_SALE, param);
        }
    }

    private void InitLabel()
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (null == module)
            return;


        if (PackageType.Pack_Equip == mParam.packtype)
        {
            SetDefenceLabel(module);
            mLabel2.text = StringHelper.GetString("defence_sale_notes");
        }
        else if (ItemManager.GetItemType((uint)mParam.itemid) == ItemType.Defence)
        {
            SetDefenceLabel(module);
            mLabel2.text = "";
        }
        else
        {
            ItemObj obj = module.GetItemByID(mParam.itemid);
            if (null == obj)
                return;

            mLabel1.text = string.Format(StringHelper.GetString("item_sale_money_count"), obj.GetRes().gameprice * mParam.mSaleNum);
            mLabel2.text = "";
        }
    }

    private void SetDefenceLabel(PlayerDataModule module)
    {
        DefenceObj defencedata = module.GetItemByIDAndPos(mParam.itemid, mParam.packpos, mParam.packtype) as DefenceObj;
        if (null == defencedata)
            return;
        string ss = string.Format(StringHelper.GetString("defence_sale_money_count"), defencedata.GetSaleMoney());
        if (defencedata.GetSaleStars() > 0)
            ss += "," + string.Format(StringHelper.GetString("defence_sale_starscout"), defencedata.GetSaleStars());
        if (defencedata.GetSaleStones() > 0)
            ss += "," + string.Format(StringHelper.GetString("defence_sale_stonescout"), defencedata.GetSaleStones());
        ss += ".";

        DefenceTableItem item = DataManager.DefenceTable[mParam.itemid] as DefenceTableItem;
        if (null == item)
            return;
        ConfigTableItem configitem = DataManager.ConfigTable[item.quality] as ConfigTableItem;
        if (null == configitem)
            return;

        ss += string.Format(StringHelper.GetString("is_sale"),"[" + configitem.value + "]" + item.name);
        mLabel1.text = ss;
    }
}
