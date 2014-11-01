using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using Message;

public class UIDefencePromoteHint : UIWindow
{
    //Button
    private UIButton mBtnclose = null;
    private UIButton mBtngotoobtain1 = null;
    private UIButton mBtngotoobtain2 = null;
    private UIButton mBtnpromote = null;
    //~Btn
    private UISprite mItem1pic = null;
    private UISprite mItem2pic = null;
    private UILabel mItem1name = null;//道具名称和需求的数量
    private UILabel mItem2name = null;
    private UILabel mHasitem1 = null;
    private UILabel mHasitem2 = null;
    private UILabel mLvhint = null;//等级

    DefenceUIParam uiparam = null;
    public UIDefencePromoteHint()
    {

    }
    protected override void OnLoad()
    {
        base.OnLoad();
        mBtnclose = this.FindComponent<UIButton>("btnclose");
        mBtngotoobtain1 = this.FindComponent<UIButton>("btngotoobtain1");
        mBtngotoobtain2 = this.FindComponent<UIButton>("btngotoobtain2");
        mBtnpromote = this.FindComponent<UIButton>("btnpromote");

        mItem1pic = this.FindComponent<UISprite>("item1/Sprite");
        mItem2pic = this.FindComponent<UISprite>("item2/Sprite");
        mItem1name = this.FindComponent<UILabel>("item1name");
        mItem2name = this.FindComponent<UILabel>("item2name");
        mHasitem1 = this.FindComponent<UILabel>("hasitem1");
        mHasitem2 = this.FindComponent<UILabel>("hasitem2");
        mLvhint = this.FindComponent<UILabel>("lvhint");
    }
    protected override void OnOpen(object param = null)
    {
        uiparam = (DefenceUIParam)param;
        EventDelegate.Add(mBtnclose.onClick, OnBtnClosedHandler);
        EventDelegate.Add(mBtngotoobtain1.onClick, OnBtnGotoObtain1Handler);
        EventDelegate.Add(mBtngotoobtain2.onClick, OnBtnGotoObtain2Handler);
        EventDelegate.Add(mBtnpromote.onClick, OnBtnPromoteHandler);
        InitUI();
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

        DefenceObj defencedata = module.GetItemByIDAndPos(uiparam.itemid, uiparam.packpos, uiparam.packtype) as DefenceObj;
        DefenceTableItem defenceItemold = DataManager.DefenceTable[uiparam.itemid] as DefenceTableItem;

        if (null == defencedata || null == defenceItemold)
            return;

        DefenceCombItem combItem = DataManager.DefenceCombTable[defenceItemold.combId] as DefenceCombItem;
        if (null == combItem)
        {
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("promote_max"));
            return;
        }

        DefenceTableItem defenceitempromote = DataManager.DefenceTable[combItem.defenceproducedId] as DefenceTableItem;

        //打开二级界面，显示需要的材料，玩家拥有的材料，装备升阶需要的等级
        NormalItemTableItem normalitem = DataManager.NormalItemTable[combItem.item1] as NormalItemTableItem;
        if (null == normalitem)
        {
            return;
        }
        UIAtlasHelper.SetSpriteImage(mItem1pic, normalitem.picname);
        mItem1name.text = normalitem.name + "X" + combItem.num1;
        normalitem = DataManager.NormalItemTable[combItem.item2] as NormalItemTableItem;
        UIAtlasHelper.SetSpriteImage(mItem2pic, normalitem.picname);
        mItem2name.text = normalitem.name + "X" + combItem.num2;

        uint playerhascitem1 = module.GetItemNumByID(combItem.item1, PackageType.Pack_Bag);
        uint playerhascitem2 = module.GetItemNumByID(combItem.item2, PackageType.Pack_Bag);

        if (combItem.num1 > playerhascitem1)
        {
            mHasitem1.text = "[E92224]";
        }
        else
        {
            mHasitem1.text = "[FAFDF4]";
        }

        if (combItem.num2 > playerhascitem2)
        {
            mHasitem2.text = "[E92224]";
        }
        else
        {
            mHasitem2.text = "[FAFDF4]";
        }
        mHasitem1.text += playerhascitem1.ToString();
        mHasitem2.text += playerhascitem2.ToString();

        if (defenceitempromote.uselevel > module.GetLevel())
        {
            mLvhint.text = "[E92224]";
        }
        else
        {
            mLvhint.text = "[FAFDF4]";
        }
        mLvhint.text += StringHelper.GetString("defencepromotehint").Replace("?",defenceitempromote.uselevel.ToString());
    }



    public void OnBtnPromoteHandler()
    {
        SoundManager.Instance.Play(15);
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();

        if (null == module)
            return;

        DefenceObj defencedata = module.GetItemByIDAndPos(uiparam.itemid, uiparam.packpos, uiparam.packtype) as DefenceObj;
        DefenceTableItem defenceItemold = DataManager.DefenceTable[uiparam.itemid] as DefenceTableItem;

        if (null == defencedata || null == defenceItemold)
            return;

        DefenceCombItem combItem = DataManager.DefenceCombTable[defenceItemold.combId] as DefenceCombItem;
        if (null == combItem)
        {
            //弹窗：装备无法提升
            return;
        }

        DefenceTableItem defenceitempromote = DataManager.DefenceTable[combItem.defenceproducedId] as DefenceTableItem;

        //打开二级界面，显示需要的材料，玩家拥有的材料，装备升阶需要的等级

        uint playerhascitem1 = module.GetItemNumByID(combItem.item1,PackageType.Pack_Bag);
        uint playerhascitem2 = module.GetItemNumByID(combItem.item2,PackageType.Pack_Bag);

        if (defenceitempromote.uselevel > module.GetLevel())
        {
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("roll_lv_need"));
            return;
        }

        if (playerhascitem1 < combItem.num1)
        {
            //弹窗：道具【名称】【个数】不足
            //TODO
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("item_need").Replace("?", ItemManager.Instance.getItemName(combItem.item1)));
            return;
        }

        if (playerhascitem2 < combItem.num2)
        {
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("item_need").Replace("?", ItemManager.Instance.getItemName(combItem.item2)));
            return;
        }

        PromoteDefenceActionParam param = new PromoteDefenceActionParam();
        param.DefenceId = uiparam.itemid;
        param.PackType = (int)uiparam.packtype;
        param.pos = uiparam.packpos;
        Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_DEFENCE_PROMOTE, param);
    }

    public void OnBtnClosedHandler()
    {
        WindowManager.Instance.CloseUI("defencepromotehint");
    }

    public void OnBtnGotoObtain1Handler()
    {
        SoundManager.Instance.Play(15);
    }

    public void OnBtnGotoObtain2Handler()
    {
        SoundManager.Instance.Play(15);
    }
}
