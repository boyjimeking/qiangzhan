using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class WeaponModule : ModuleBase
{
//     public const string WeaponShopGridUIPrefabName = "UI/equip/EquipItem0";
//     public const string PromoteGridUIPrefabName = "UI/equip/EquipItem1";
    private int mIndex = 0;//需要跳到的标签

    private int mWeaponSkillID = -1;

    private int mSuperWeapon = -1;
    protected override void OnEnable()
    {

    }

    /// <summary>
    /// 在openUI之后调用
    /// </summary>
    /// <param name="index"></param>
    /// <param name="notify"></param>
    public void SetTabIndex(int index, bool notify = true)
    {
        mIndex = index;

        if (!notify)
            return;

        WeaponCultivateEvent evt = new WeaponCultivateEvent(WeaponCultivateEvent.TAB_INDEX);
        EventSystem.Instance.PushEvent(evt);
    }

    public int GetTabIndex()
    {
        return mIndex;
    }

    public void BuyWeapon(int id)
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        PrestigeTableItem preres = DataManager.PrestigeTable[id] as PrestigeTableItem;
        if (preres == null)
            return;

        uint pre = module.GetProceeds(ProceedsType.Money_Prestige);
        if (pre < preres.value)
        {
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("presit_shortage"));
            return;
        }        

        if(!DataManager.WeaponTable.ContainsKey(id))
        {
            GameDebug.LogError("无效的武器id:" + id.ToString());
            return;
        }

        WeaponTableItem res = DataManager.WeaponTable[id] as WeaponTableItem;

        uint gamei = module.GetProceeds(ProceedsType.Money_Game);
        if (gamei < (uint)res.gameprice)
        {
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("money_game_shortage"));
            return;
        }

        //模拟服务器创建道具
        //module.CreateItemUnreal(id, PackageType.Pack_Weapon);
        BuyWeaponActionParam param = new BuyWeaponActionParam();
        param.WeaponResId = id;
        Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_WD_BUY_WEAPON, param);
    }

    //装备到主武器
    public void EquipMainWeapon(int id)
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        int subWeapon = module.GetSubWeaponId();
        int mainWeapon = module.GetMainWeaponId();

        //装备到主手的时候 必须有一个交换对象
//         if( subWeapon == -1 )
//         {
//             return ;
//         }

        if( subWeapon == id )
        {
            EquipWeapon(id, mainWeapon);
        }else
        {
            EquipWeapon(id, subWeapon);
        }
        //module.SetWeapon(id);

//         ChangeWeaponActionParam param = new ChangeWeaponActionParam();
//         param.WeaponResId = id;
//         Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_WD_CHANGE_WEAPON, param);
//        EquipWeapon(id, -1);
    }
    //装备到副武器
    public void EquipSubWeapon(int id)
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        int subWeapon = module.GetSubWeaponId();
        int mainWeapon = module.GetMainWeaponId();

        if (mainWeapon == id)
        {
            if( subWeapon >= 0 )
            {
                EquipWeapon(subWeapon, id);
            }
        }
        else
        {
            EquipWeapon(mainWeapon, id);
        }
    }

    private void EquipWeapon(int mainWeapon , int subWeapon)
    {
        ChangeWeaponActionParam param = new ChangeWeaponActionParam();
        param.WeaponResId = mainWeapon;
        param.SubWeaponResId = subWeapon;
        Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_WD_CHANGE_WEAPON, param);
    }

    public void StrenWeapon()
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        uint lv = module.GetStrenLv();

        StrenTableItem mres = DataManager.StrenTable[(int)(lv+1)] as StrenTableItem;
        if (mres == null)
        {//满级
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("weapon_stren_max"));
            return;
        }

        StrenTableItem sres = DataManager.StrenTable[(int)lv] as StrenTableItem;
        if (sres == null)
        {
            return;
        }

        uint stren_money = module.GetProceeds(ProceedsType.Money_Stren);
        if (stren_money < sres.cost)
        {
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("gun_stren_money_shortage"));
            return;
        }

        StrenWeaponActionParam param = new StrenWeaponActionParam();
        param.StrenLv = lv;
        param.money_stren_cost = sres.cost;

        Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_WD_STREN_WEAPON, param);
    }

    public void SetPromote(int resId)
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        WeaponObj wobj = module.GetItemByID(resId, PackageType.Pack_Weapon) as WeaponObj;
        if (wobj == null)
            return;

        int plv = (int)wobj.GetPromoteLv();
        PromoteTableItem curpres = wobj.GetPromoteRes();
        if (curpres == null)
        {
            GameDebug.LogError("资源ID为" + plv + "不存在表格promote.txt中 ");
            return;
        }

        int toLv = wobj.getProtemResId() + 1;

        PromoteTableItem nexpres = DataManager.PromoteTable[toLv] as PromoteTableItem;
        if (nexpres == null)
        {
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("weapon_promote_max"));
            return;
        }

        uint count = module.GetItemNumByID(curpres.item0);
        string item_name = "";
        bool isShortage = false;
        if (count < curpres.num0)
        {
            string ons = ItemManager.Instance.getItemName(curpres.item0);
            item_name += ons;
            isShortage = true;
        }
        count = module.GetItemNumByID(curpres.item1);
        if (count < curpres.num1)
        {
            string tns = ItemManager.Instance.getItemName(curpres.item1);
            if (!"".Equals(item_name))
                item_name += "、";
            item_name += tns;
            isShortage = true;
        }

        if (isShortage)
        {
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("item_need").Replace("?", item_name));
            return;
        }

        PackageManager pack = module.GetPackManager();

        int packpos = 0;
        foreach(KeyValuePair<int,ItemObj> value in pack.getPackDic(PackageType.Pack_Weapon))
        {
            if (null != value.Value && value.Value.GetResId() == resId)
            {
                packpos = value.Value.PackPos;
                break;
            }
        }
        SetPromoteActionParam param = new SetPromoteActionParam();
        param.WeaponResId = resId;
        param.WeaponPos = packpos;

        Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_WD_SET_PROMOTE, param);
    }

    public bool GetFittMinMax(int id, int proid,ref int min, ref int max)
    {
        FittingsTableItem fres = DataManager.FittingsTable[id] as FittingsTableItem;
        if (fres == null)
            return false;

        int ind = FittingsProperty.GetResId(proid);
        int minid = 0;
        int maxid = 0;
        if (!fres.GetIntervalByID(ind, ref minid, ref maxid))
            return false;

        for (int i= minid; i<=maxid; ++i)
        {
            FittoddsTableItem odres = DataManager.FittoddsTable[i] as FittoddsTableItem;
            if (odres == null)
            {
                GameDebug.LogError("资源ID为" + i + "不存在表格fittodds.txt中 ");
                return false;
            }
        }        

        if (!fres.GetIntervalByValue(ind, ref min, ref max))
            return false;

        return true;
    }

    public void BapFittings(uint pos, int fiId, bool[] lockIndex)
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        FittingsData fdata = module.GetFittingsData(pos);
        if (fdata == null)
            return;

         int id = fdata.GetId();

        FittingsTableItem fres = DataManager.FittingsTable[id] as FittingsTableItem;
        if (fres == null)
            return;

        uint chance = module.GetFittChance();

        BapFittingsActionParam param = new BapFittingsActionParam();
        param.pos = pos;
        param.itemId = (uint)fres.costid;

        if (chance <= 0)
        {
            int lnum = 0;
            for (int i = 0; i < (int)FittingsType.MAX_PROPERTY; ++i)
            {
                if (lockIndex[i])
                    lnum++;
            }
            uint cnum = 0;
            if (lnum == 0)
                cnum = fres.num_0;
            else if (lnum == 1)
                cnum = fres.num_1;
            else if (lnum == 2)
                cnum = fres.num_2;

            uint count = PlayerDataPool.Instance.MainData.mPack.GetNumByID(fres.costid);
            if (count < cnum)
            {
                string ons = ItemManager.Instance.getItemName(fres.costid);
                PopTipManager.Instance.AddNewTip(StringHelper.GetString("item_need").Replace("?", ons));
                return;
            }

            param.price = cnum;
        }
        else
        {
            param.price = 0;
        }

        int mCount = 0;
        for (int i = 0; i < (int)FittingsType.MAX_PROPERTY; ++i)
        {
            int proid = -1;
            if (lockIndex[mCount])
            {
                int value = -1;
                bool forbid = false;
                if (!fdata.GetProValue((uint)mCount, ref proid, ref value, ref forbid))
                    continue;
                param.set_mproperty(mCount, proid);
                param.set_mvalue(mCount, value);
                param.set_mlock(mCount, lockIndex[mCount]);
                param.set_start(mCount, (uint)0);
                param.set_end(mCount, (uint)1);
                mCount++;
                continue;
            }


            proid = fres.GetProId();
            if (proid == -1)
                continue;

            int ind = FittingsProperty.GetResId(proid);
            int minid = 0;
            int maxid = 0;
            if (!fres.GetIntervalByID(ind, ref minid, ref maxid))
                continue;

            FittoddsTableItem odres = DataManager.FittoddsTable[maxid] as FittoddsTableItem;
            if (odres == null)
            {
                GameDebug.LogError("fittodds.txt填写错误 id = " + maxid);
                return;
            }

            uint roodds = (uint)UnityEngine.Random.Range(0, (int)odres.odds);        
            
            for (int j = minid; j <= maxid; ++j)
            {
                FittoddsTableItem rares = DataManager.FittoddsTable[j] as FittoddsTableItem;
                if (rares == null)
                {
                    GameDebug.LogError("fittodds.txt填写错误 id = " + j);
                    return;
                }

                if (rares.odds > roodds)
                {
                    param.set_mproperty(mCount, proid);
                    param.set_mvalue(mCount, 1);
                    param.set_mlock(mCount, lockIndex[mCount]);
                    param.set_start(mCount, rares.start);
                    param.set_end(mCount, rares.end - rares.start == 0 ? 1 : rares.end - rares.start);
                    mCount++;
                    break;
                }
            }
        }
        Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_WD_BAP_FITTINGS, param);
    }

    protected override void OnDisable()
    {
    }

    public int GetWeaponSkillID(int weaponid)
    {
        if (mWeaponSkillID >= 0 && mSuperWeapon == weaponid)
        {
            return mWeaponSkillID;
        }

        if (!DataManager.WeaponTable.ContainsKey(weaponid))
            return -1;

        WeaponTableItem wres = DataManager.WeaponTable[weaponid] as WeaponTableItem;


        if (!DataManager.WeaponSkillTable.ContainsKey(wres.take_skill))
        {
            return -1;
        }

        WeaponSkillTableItem item = DataManager.WeaponSkillTable[wres.take_skill] as WeaponSkillTableItem;
        if (item == null)
        {
            return -1;
        }

        mWeaponSkillID = item.skillid;
        mSuperWeapon = weaponid;

        return mWeaponSkillID;
    }

    public string GetWeaponSkillIcon(int weaponid)
    {
        if (!DataManager.WeaponTable.ContainsKey(weaponid))
            return "";

        WeaponTableItem wres = DataManager.WeaponTable[weaponid] as WeaponTableItem;


        if (!DataManager.WeaponSkillTable.ContainsKey(wres.take_skill))
        {
            return "";
        }

        WeaponSkillTableItem item = DataManager.WeaponSkillTable[wres.take_skill] as WeaponSkillTableItem;
        if (item == null)
        {
            return "";
        }

        return item.icon;
    }
}
