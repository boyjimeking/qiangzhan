using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class DefenceModule : ModuleBase
{
    protected override void OnEnable()
    {
    }

    public void SaleDefence(DefenceUIParam uiparam)
    {
        SaleDefenceActionParam param = new SaleDefenceActionParam();
        param.DefenceId = uiparam.itemid;
        param.PackType = (int)uiparam.packtype;
        param.pos = uiparam.packpos;

        Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_DEFENCE_SALE, param);
    }
    public bool StrenDefence(DefenceUIParam uiparam)
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();

        if (null == module)
            return false;

        DefenceObj defencedata = module.GetItemByIDAndPos(uiparam.itemid, uiparam.packpos, uiparam.packtype) as DefenceObj;
        DefenceTableItem defenceItem = DataManager.DefenceTable[uiparam.itemid] as DefenceTableItem;

        if (null == defencedata || null == defenceItem)
            return false;

        //当前装备是否可以强化
        if (defencedata.GetStrenLv() >= defenceItem.strenLevelMax)
        {
            //弹窗.当前装备强化等级已达到最高等级，请提升装备品质
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("strenmax"));
            return false;
        }

        //根据当前装备属性取得强化等级及消耗并显示
        int strenlv = defencedata.GetStrenLv();
        DefenceStrenProItem strenproItem = DataManager.DefenceStrenProTable[defenceItem.strenSerialNumber + strenlv] as DefenceStrenProItem;
        DefenceStrenItem strenItem = DataManager.DefenceStrenTable[strenlv + 1] as DefenceStrenItem;

        if (strenproItem == null || strenItem == null)
            return false;

        uint x = module.GetProceeds(ProceedsType.Money_Game);

        if (module.GetProceeds(ProceedsType.Money_Game) < strenItem.money * strenproItem.ratio)
        {
            //金币不足，弹窗：提示玩家直接购买
            //TODO
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("money_game_shortage"));
            return false;
        }

        StrenDefenceActionParam param = new StrenDefenceActionParam();
        param.DefenceId = uiparam.itemid;
        param.PackType = (int)uiparam.packtype;
        param.pos = uiparam.packpos;
        param.strenten = false;

        Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_DEFENCE_STEN, param);
        return true;
    }

    public bool RisingStar(DefenceUIParam uiparam)
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();

        if (null == module)
            return false;

        DefenceObj defencedata = module.GetItemByIDAndPos(uiparam.itemid, uiparam.packpos, uiparam.packtype) as DefenceObj;
        DefenceTableItem defenceItem = DataManager.DefenceTable[uiparam.itemid] as DefenceTableItem;

        if (null == defencedata || null == defenceItem)
            return false;
        

        //判断装备是否可以升星
        int starslv = defencedata.GetStarsLv();
        if (starslv >= defenceItem.starsLevelMax)
        {
            //弹窗.当前装备星阶已达到最高等级，请提升装备品质
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("rising_stars_max"));
            return false;
        }

        DefenceStarsItem starsitem = DataManager.DefenceStarsTable[starslv + 1] as DefenceStarsItem;
        DefenceStarsProItem starsproitem = DataManager.DefenceStarsProTable[defenceItem.starsSerialNumber + starslv] as DefenceStarsProItem;
        //判断升星石是否足够
        if (module.GetItemNumByID(starsitem.starsstoneId, PackageType.Pack_Bag) < starsitem.cstarsstone * starsproitem.scale)
        {
            //升星石不足，弹窗：提示玩家直接购买
            //TODO
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("rising_stones_need"));
            return false;
        }
        
        RisingStarsActionParam param = new RisingStarsActionParam();
        param.DefenceId = uiparam.itemid;
        param.PackType = (int)uiparam.packtype;
        param.pos = uiparam.packpos;
        param.riseten = false;

        Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_DEFENCE_RISING_STAR, param);

        return true;
    }

    public void StoneInlay(DefenceUIParam uiparam) 
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();

        if (null == module)
            return;

        DefenceObj defencedata = module.GetItemByIDAndPos(uiparam.itemid, uiparam.packpos, uiparam.packtype) as DefenceObj;
        StoneTableItem stoneitem = DataManager.StoneTable[uiparam.stoneid] as StoneTableItem;

        if (null == defencedata || null == stoneitem)
        {
            PopTipManager.Instance.AddNewTip("请选择宝石");
            return;
        }

        List<Message.stone_info> stoneinfo = defencedata.GetStoneInfo();
        for (int i = 0; i < stoneinfo.Count; ++i)
        {
            StoneTableItem itemsss = DataManager.StoneTable[stoneinfo[i].stoneid] as StoneTableItem;
            if (null == itemsss)
                continue;
            if (itemsss.type == stoneitem.type)
            {
                //弹窗，不能镶嵌相同种类的宝石
                PopTipManager.Instance.AddNewTip(StringHelper.GetString("not_inlay_same_type"));
                return;
            }
        }

        StoneInlayActionParam param = new StoneInlayActionParam();
        param.DefenceId = uiparam.itemid;
        param.stoneId = uiparam.stoneid;
        param.PackType = (int)uiparam.packtype;
        param.pos = uiparam.packpos;
        param.stonepos = uiparam.stonepos;

        Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_DEFENCE_STONE_INLAY, param);
    }

    public void StoneComb(DefenceUIParam uiparam)
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();

        if (null == module)
            return;

        StoneTableItem stoneitem = null;
        if (uiparam.isequiped)
        {
            DefenceObj defencedata = module.GetItemByIDAndPos(uiparam.itemid, uiparam.packpos, uiparam.packtype) as DefenceObj;

            if (null == defencedata)
                return;

            stoneitem = DataManager.StoneTable[defencedata.GetStoneInfoByPos(uiparam.stonepos).stoneid] as StoneTableItem;
        }
        else
        {
            stoneitem = DataManager.StoneTable[uiparam.stoneid] as StoneTableItem;
        }

        if (null == stoneitem)
            return;

        if (-1 == stoneitem.combid)
        {
            //宝石已达最高等级
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("stonesmax"));
            return;
        }

        DefenceCombItem combItem = DataManager.DefenceCombTable[stoneitem.combid] as DefenceCombItem;
        if (null == combItem)
        {
            //弹窗：宝石无法合成
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("stone_comb_error"));
            return;
        }

        if (module.GetProceeds(ProceedsType.Money_Game) < combItem.moenyused)
        {
            //弹窗：玩家金币不足
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("money_game_shortage"));
            return;
        }

        uint playerhascitem = module.GetItemNumByID(combItem.item1, PackageType.Pack_Gem);
        if (uiparam.isequiped)
            playerhascitem += 1;
        if (playerhascitem < combItem.num1)
        {
            ////弹窗：道具【名称】【个数】不足
            stoneitem = DataManager.StoneTable[combItem.item1]  as StoneTableItem;
            if (null == stoneitem)
            {
                GameDebug.LogError("stone.txt中没有此宝石 id = " + combItem.item1);
                return;
            }
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("item_need").Replace("?", stoneitem.name));
            return;
        }

        StoneCombActionParam param = new StoneCombActionParam();
        param.DefenceId = uiparam.itemid;
        param.stoneId = stoneitem.id;
        param.PackType = (int)uiparam.packtype;
        param.pos = uiparam.packpos;
        param.stonepos = uiparam.stonepos;
        param.isequiped = uiparam.isequiped;
        Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_DEFENCE_STONE_COMB, param);
    }

    public void StoneUnInlay(DefenceUIParam uiparam)
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();

        if (null == module)
            return;

        DefenceObj defencedata = module.GetItemByIDAndPos(uiparam.itemid, uiparam.packpos, uiparam.packtype) as DefenceObj;
        StoneTableItem stoneitem = DataManager.StoneTable[uiparam.stoneid] as StoneTableItem;
        PackageManager pack = module.GetPackManager();

        if (null == defencedata || null == stoneitem || null == pack)
            return;

        
        // 判读Bag是否已满
        if (pack.GetPackMaxVaildSize(PackageType.Pack_Gem) >= pack.GetPackMaxSize(PackageType.Pack_Gem) && pack.GetNumByID(uiparam.stoneid,PackageType.Pack_Gem) >= stoneitem.maxFurl)
        {
            //弹窗：Bag已满
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("bag_full"));
            return;
        }

        StoneUnInlayActionParam param = new StoneUnInlayActionParam();
        param.DefenceId = uiparam.itemid;
        param.stoneId = uiparam.stoneid;
        param.PackType = (int)uiparam.packtype;
        param.pos = uiparam.packpos;
        param.stonepos = uiparam.stonepos;

        Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_DEFENCE_STONE_UNINLAY, param);
    }

    protected override void OnDisable()
    {
    }
}
