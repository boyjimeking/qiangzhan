using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Message;
class OtherDataPool
{
    private static OtherDataPool msInstance = null;

    private PlayerData mOtherData = null;

    public static OtherDataPool Instance
    {
        get
        {
            return msInstance;
        }
    }
    public OtherDataPool()
    {
        msInstance = this;
    }

    public PlayerData GetOtherData()
    {
        return mOtherData;
    }
    private bool SyncBaseData(role_property serverPropery)
    {
        if (serverPropery.name != null)
        {
            mOtherData.name = serverPropery.name.value;
        }

        if (serverPropery.level != null)
        {
            mOtherData.level = (int)serverPropery.level.value;
        }

        if (serverPropery.resid != null)
        {
            mOtherData.resId = (int)serverPropery.resid.value;
        }

        if (serverPropery.vip_value != null)
        {
            mOtherData.vip_value = serverPropery.vip_value.value;
        }

        if (serverPropery.vip_level != null)
        {
            mOtherData.vip_level = serverPropery.vip_level.value;
        }

        if (serverPropery.weapon_id != null)
        {
            mOtherData.main_weaponId = (int)serverPropery.weapon_id.value;
        }

        if (serverPropery.sub_weapon_id != null)
        {
            mOtherData.sub_weaponId = (int)serverPropery.sub_weapon_id.value;
        }

        if (serverPropery.exp != null)
        {
            mOtherData.exp = (int)serverPropery.exp.value;
        }

        if (serverPropery.stren_lv != null)
        {
            mOtherData.mStrenLv = serverPropery.stren_lv.value;
        }

        if (serverPropery.sp != null)
        {
            mOtherData.sp = (int)serverPropery.sp.value;
        }

        if (serverPropery.grades != null && serverPropery.grades.Count > 0)
        {
            int count = (int)PlayerGradeEnum.PlayerGradeEnumMax;
            uint[] vals = new uint[count];

            for (int i = 0; i < serverPropery.grades.Count; ++i)
            {
                if (i < count)
                {
                    vals[i] = serverPropery.grades[i];
                }
            }

            mOtherData.mGrades.Grades = vals;
        }

        return true;
    }

    public void SyncBag(role_property serverProperty)
    {
        //只同步装备栏
        if (serverProperty.equipbag == null )
            return;

        mOtherData.mPack.Clear();
        mOtherData.mPack.SyncPackBag(PackageType.Pack_Equip, serverProperty.equipbag);
        mOtherData.mPack.SyncPackBag(PackageType.Pack_Weapon, serverProperty.weaponbag);
    }

    private void SyncWingData(role_property serverProperty)
    {
        if (serverProperty.wing_info == null) return;

        if (serverProperty.wing_info.unlock_count != null)
        {
            if (mOtherData.mWingData.wingItems.Count > (int)serverProperty.wing_info.unlock_count.value)
            {
                //除非重置数据否则不会出现这种情况
                mOtherData.mWingData.Clear();
            }
        }
        if (serverProperty.wing_info.wearid != null)
        {
            mOtherData.mWingData.mWearId = serverProperty.wing_info.wearid.value;
        }
        if (serverProperty.wing_info.items != null && serverProperty.wing_info.items.Count > 0)
        {
            for (int i = 0; i < serverProperty.wing_info.items.Count; ++i)
            {

                WingItemData wing_item_data =
                    mOtherData.mWingData.wingItems.Find(x => (x.id == serverProperty.wing_info.items[i].id));
                if (wing_item_data == null)
                {
                    wing_item_data = new WingItemData();
                    mOtherData.mWingData.wingItems.Add(wing_item_data);
                    wing_item_data.id = serverProperty.wing_info.items[i].id;
                    wing_item_data.level = serverProperty.wing_info.items[i].level;
                    mOtherData.mWingData.getPropertyTotal(ref wing_item_data);
                    // GameDebug.Log("添加WingitemData数据" + serverProperty.wing_info.items[i].id);
                }

                wing_item_data.id = serverProperty.wing_info.items[i].id;
                wing_item_data.level = serverProperty.wing_info.items[i].level;
                wing_item_data.process = serverProperty.wing_info.items[i].process;
                wing_item_data.UpdateProperty();

            }
        }
    }
    public bool SyncOtherData( role_property serverPropery)
    {
        if( mOtherData == null )
        {
            mOtherData = new PlayerData();
        }
        SyncBaseData(serverPropery);
        SyncBag(serverPropery);
        SyncWingData(serverPropery);
        return true;
    }
}
