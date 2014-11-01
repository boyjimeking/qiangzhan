using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CropsModule : ModuleBase
{
    private int mIndex = 0;//需要跳到的标签
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

        CropsEvent evt = new CropsEvent(CropsEvent.TAB_INDEX);
        EventSystem.Instance.PushEvent(evt);
    }

    public int GetTabIndex()
    {
        return mIndex;
    }

    protected override void OnDisable()
    {
    }

    //购买佣兵
    public void BuyCrops(int resid)
    {
        CropsBuyActionParam param = new CropsBuyActionParam();
        param.cropsid = resid;

        Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_CROPS_BUY,param);
    }

    //佣兵出战设置
    public void SetStateCrops(int mainid, int subid)
    {
        CropsChangeActionParam param = new CropsChangeActionParam();
        param.main_crops_resid = mainid;
        param.sub_crops_resid = subid;

        Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_CROPS_CHANGE, param);
    }
    //佣兵升星
    public void RiseCropsStars(int resid)
    {
        CropsPromoteActionParam param = new CropsPromoteActionParam();
        param.cropsid = resid;

        Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_CROPS_PROMOTE, param);
    }

    public void GetProperty(int resid, int starslv, ref float hp, ref float damage, ref float crits, ref float defence, ref float energy)
    {
        PlayerDataModule pmodule = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (null == pmodule)
            return;

        PlayerPropertyModule module = ModuleManager.Instance.FindModule<PlayerPropertyModule>();
        if (null == module)
            return;

        PropertyOperation pro = module.GetPlayerProperty();
        if (null == pro)
            return;

        CropsLevelTableItem itemlv = DataManager.CropsLevelTable[pmodule.GetLevel()] as CropsLevelTableItem;
        if (null == itemlv)
            return;

        CropsProRatioTableItem itempro = DataManager.CropsProRatioTable[resid] as CropsProRatioTableItem;
        if (null == itempro)
            return;

        //int starslv = pmodule.GetCropsStarsLv(resid) == -1 ? 1 : pmodule.GetCropsStarsLv(resid);
        //hp
        hp = (int)(pro.GetPro((int)PropertyTypeEnum.PropertyTypeMaxHP) * itempro.GetRatioPlayerByProId((int)PropertyTypeEnum.PropertyTypeMaxHP, starslv) + itemlv.life * itempro.GetRatioCropsByProId((int)PropertyTypeEnum.PropertyTypeMaxHP, starslv));
        //damage
        damage = (int)(pro.GetPro((int)PropertyTypeEnum.PropertyTypeDamage) * itempro.GetRatioPlayerByProId((int)PropertyTypeEnum.PropertyTypeDamage, starslv) + itemlv.damage * itempro.GetRatioCropsByProId((int)PropertyTypeEnum.PropertyTypeDamage, starslv));
        //crits
        crits = (int)(pro.GetPro((int)PropertyTypeEnum.PropertyTypeCrticalLV) * itempro.GetRatioPlayerByProId((int)PropertyTypeEnum.PropertyTypeCrticalLV, starslv) + itemlv.crits * itempro.GetRatioCropsByProId((int)PropertyTypeEnum.PropertyTypeCrticalLV, starslv));
        //defence
        defence = (int)(pro.GetPro((int)PropertyTypeEnum.PropertyTypeDefance) * itempro.GetRatioPlayerByProId((int)PropertyTypeEnum.PropertyTypeDefance, starslv) + itemlv.defence * itempro.GetRatioCropsByProId((int)PropertyTypeEnum.PropertyTypeDefance, starslv));
        //energy
        energy = (int)(pro.GetPro((int)PropertyTypeEnum.PropertyTypeMaxMana) * itempro.GetRatioPlayerByProId((int)PropertyTypeEnum.PropertyTypeMaxMana, starslv) + itemlv.energy * itempro.GetRatioCropsByProId((int)PropertyTypeEnum.PropertyTypeMaxMana, starslv));
    }
}
