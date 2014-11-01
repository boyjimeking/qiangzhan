using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum INF_PRO_TYPE:int
{
    INF_PRO_TYPE_EQUIP = 0,
    INF_PRO_TYPE_WEAPON = 1,
    INF_PRO_TYPE_BASE = 2,
    INF_PRO_TYPE_WING = 3,
    INF_PRO_TYPE_FASHION = 4,
    INF_PRO_TYPE_MAX = 5,
}

//属性刷新相关操作 影响Player
class PlayerPropertyModule : ModuleBase
{
    //对应的装备配置信息
    private int[] mEquipConfigs = new int[(int)EquipSlot.EquipSlot_MAX];

    private BitArray mMarks = new BitArray((int)INF_PRO_TYPE.INF_PRO_TYPE_MAX);
    //基础属性
    private PropertyOperation mPlayerBaseProperty = new PropertyOperation();
    //枪械属性
    private PropertyOperation mPlayerWeaponProperty = new PropertyOperation();
    //装备属性
    private PropertyOperation mPlayerEquipProperty = new PropertyOperation();
    //翅膀属性
    private PropertyOperation mPlayerWingProperty = new PropertyOperation();
    //时装属性
    private PropertyOperation mPlayerFashionProperty = new PropertyOperation();
    //玩家所有属性
    private PropertyOperation mPlayerAllProperty = new PropertyOperation();

    public PlayerPropertyModule()
    {
        mMarks.SetAll(false);
    }
    override protected void OnEnable()
    {
        EventSystem.Instance.addEventListener(ItemEvent.WEAPON_CHANGE, OnWeaponChange);
        EventSystem.Instance.addEventListener(ItemEvent.UPDATE_CHANGE, OnItemChange);
        EventSystem.Instance.addEventListener(PropertyEvent.MAIN_PROPERTY_CHANGE, OnMainPropertyChanged);
        EventSystem.Instance.addEventListener(WingUIEvent.Wing_UI_UPDATE, OnWingUpdate);
        EventSystem.Instance.addEventListener(FashionEvent.FASHION_UPDATE,OnFashionUpdate);
    }
    override protected void OnDisable()
    {
        EventSystem.Instance.removeEventListener(ItemEvent.WEAPON_CHANGE, OnWeaponChange);
        EventSystem.Instance.removeEventListener(ItemEvent.UPDATE_CHANGE, OnItemChange);
        EventSystem.Instance.removeEventListener(PropertyEvent.MAIN_PROPERTY_CHANGE, OnMainPropertyChanged);
        EventSystem.Instance.removeEventListener(WingUIEvent.Wing_UI_UPDATE, OnWingUpdate);
        EventSystem.Instance.removeEventListener(FashionEvent.FASHION_UPDATE, OnFashionUpdate);

    }

    public PropertyOperation GetPlayerProperty()
    {
        mPlayerAllProperty.Clear();
        mPlayerAllProperty.Add(mPlayerBaseProperty);
        mPlayerAllProperty.Add(mPlayerEquipProperty);
        mPlayerAllProperty.Add(mPlayerWeaponProperty);
        mPlayerAllProperty.Add(mPlayerWingProperty);
        mPlayerAllProperty.Add(mPlayerFashionProperty);
        return mPlayerAllProperty;
    }

    public int[] GetEquipConfigs()
    {
        return mEquipConfigs;
    }

    private void OnMainPropertyChanged(EventBase e)
    {
        mMarks.Set((int)INF_PRO_TYPE.INF_PRO_TYPE_BASE , true);
    }
    private void OnWeaponChange(EventBase e)
    {
        mMarks.Set((int)INF_PRO_TYPE.INF_PRO_TYPE_WEAPON, true);
    }
    private void OnItemChange(EventBase e)
    {
        mMarks.Set((int)INF_PRO_TYPE.INF_PRO_TYPE_EQUIP, true);
    }

    private void OnWingUpdate(EventBase e)
    {
        mMarks.Set((int)INF_PRO_TYPE.INF_PRO_TYPE_WING, true);
    }

    private void OnFashionUpdate(EventBase e)
    {
        mMarks.Set((int)INF_PRO_TYPE.INF_PRO_TYPE_FASHION,true);
    }


    public override void Update(uint elapsed)
    {
        bool changed = false;
        //基础
        if( mMarks.Get( (int)INF_PRO_TYPE.INF_PRO_TYPE_BASE ) )
        {
            changed = true;
            PropertyBuild.BuildBaseProperty(PlayerDataPool.Instance.MainData, mPlayerBaseProperty);
        }
        //装备
        if (mMarks.Get((int)INF_PRO_TYPE.INF_PRO_TYPE_EQUIP))
        {
            changed = true;

            PropertyBuild.BuildEquipProperty(PlayerDataPool.Instance.MainData, mPlayerEquipProperty);

            //更新外形信息
            PropertyBuild.BuildEquipView(PlayerDataPool.Instance.MainData, mEquipConfigs);

            Player player = PlayerController.Instance.GetControlObj() as Player;
            if (player != null)
            {
                player.ApplyEquipConfig(mEquipConfigs);
            }
        }
        //武器
        if (mMarks.Get((int)INF_PRO_TYPE.INF_PRO_TYPE_WEAPON))
        {
            changed = true;

            PropertyBuild.BuildWeaponProperty(PlayerDataPool.Instance.MainData, mPlayerWeaponProperty);
        }
        //翅膀
        if (mMarks.Get((int)INF_PRO_TYPE.INF_PRO_TYPE_WING))
        {
            changed = true;

            PropertyBuild.BuildWingProperty(PlayerDataPool.Instance.MainData, mPlayerWingProperty);
        }

        //时装
        if(mMarks.Get((int)INF_PRO_TYPE.INF_PRO_TYPE_FASHION))
        {
            changed = true;
            PropertyBuild.BuildFashionProperty(PlayerDataPool.Instance.MainData, mPlayerFashionProperty);
        }

        mMarks.SetAll(false);



        if( changed )
        {
            PropertyEvent evt = new PropertyEvent(PropertyEvent.PLAYER_DATA_PROPERTY_CHANGED);
            EventSystem.Instance.PushEvent(evt);
        }
    }
}
