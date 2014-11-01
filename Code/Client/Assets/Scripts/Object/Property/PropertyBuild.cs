using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyOperation
{
    public Dictionary<int, float> values = new Dictionary<int, float>();
    public PropertyOperation()
    {

    }

    public void AddPro(int proId, float value)
    {
        if (values.ContainsKey(proId))
        {
            values[proId] = values[proId] + value;
        }
        else
        {
            values.Add(proId, value);
        }
    }

    public bool HasPro(int proId)
    {
        return values.ContainsKey(proId);
    }

    public float GetPro(int proId)
    {
        if (!HasPro(proId))
            return 0.0f;
        return values[proId];
    }

    public void Add(PropertyOperation data)
    {
        foreach (KeyValuePair<int, float> v in data.values)
        {
            this.AddPro(v.Key, v.Value);
        }
    }

    public void Clear()
    {
        values.Clear();
    }
}
static class PropertyBuild
{
    //基础属性
    public static void BuildBaseProperty(PlayerData mainData, PropertyOperation operation)
    {
        operation.Clear();

        PlayerTableItem player_res = DataManager.PlayerTable[mainData.resId] as PlayerTableItem;
        if (player_res == null)
            return;
        LevelTableItem levelRes = DataManager.LevelTable[mainData.level] as LevelTableItem;
        if (levelRes == null)
        {
            return;
        }
        operation.AddPro((int)PropertyTypeEnum.PropertyTypeHP, levelRes.maxhp);
        operation.AddPro((int)PropertyTypeEnum.PropertyTypeMaxHP, levelRes.maxhp);
        operation.AddPro((int)PropertyTypeEnum.PropertyTypeMana, levelRes.energy);
        operation.AddPro((int)PropertyTypeEnum.PropertyTypeMaxMana, levelRes.energy);
        operation.AddPro((int)PropertyTypeEnum.PropertyTypeDamage, levelRes.damage);
        operation.AddPro((int)PropertyTypeEnum.PropertyTypeCrticalLV, levelRes.crticalLV);
        operation.AddPro((int)PropertyTypeEnum.PropertyTypeDefance, levelRes.damageReduce);
        operation.AddPro((int)PropertyTypeEnum.PropertyTypeSpeed, player_res.speed);
    }
    //枪械属性
    public static void BuildWeaponProperty(PlayerData mainData , PropertyOperation operation)
    {
        operation.Clear();
        //主武器属性
        WeaponObj obj = mainData.mPack.GetItemByID(mainData.main_weaponId, PackageType.Pack_Weapon) as WeaponObj;
        if (null != obj)
            obj.BuildProperty(operation);
        //强化属性
        StrenTableItem item = DataManager.StrenTable[mainData.mStrenLv] as StrenTableItem;
        if (null == item)
        {
            GameDebug.LogError("没有此强化等级 lv = " + mainData.mStrenLv);
            return;
        }
        operation.AddPro((int)PropertyTypeEnum.PropertyTypeDamage, item.value);
        int starlv = (int)(mainData.mStrenLv / (int)STARS_RANK.MAX_STARS_RANK_NUMBER);
        if (starlv > 0 && (mainData.mStrenLv % (int)STARS_RANK.MAX_STARS_RANK_NUMBER) == 0)
            starlv -= 1;
        if (0 != starlv)
        {
            for (int i = 1; i <= starlv; ++i)
            {
                StrProTableItem itempro = DataManager.StrProTable[i] as StrProTableItem;
                if (null == itempro)
                {
                    GameDebug.LogError("strenthpro.txt中不存在id = " + starlv);
                    return;
                }
                if (itempro.life != uint.MaxValue)
                    operation.AddPro((int)PropertyTypeEnum.PropertyTypeMaxHP, itempro.life);
                if (itempro.damage != uint.MaxValue)
                    operation.AddPro((int)PropertyTypeEnum.PropertyTypeDamage, itempro.damage);
                if (itempro.crits != uint.MaxValue)
                    operation.AddPro((int)PropertyTypeEnum.PropertyTypeCrticalLV, itempro.crits);
                if (itempro.defence != uint.MaxValue)
                    operation.AddPro((int)PropertyTypeEnum.PropertyTypeDefance, itempro.defence);
                if (itempro.energy != uint.MaxValue)
                    operation.AddPro((int)PropertyTypeEnum.PropertyTypeMaxMana, itempro.energy);
            }
        }
        //配件属性
        FittingsData[] mFittings = mainData.mFittings;
        WeaponModule module = ModuleManager.Instance.FindModule<WeaponModule>();
        if (null == module)
            return;
        for (int i = 0; i < (int)FittingsType.MAX_FITTGINS; ++i)
        {
            FittingsTableItem fittingsitem = DataManager.FittingsTable[mFittings[i].GetId()] as FittingsTableItem;
            if (null == fittingsitem)
                break;
            for (int j = 0; j < (int)FittingsType.MAX_PROPERTY; ++j)
            {
                int min = 0;
                int max = 1;
                if (!module.GetFittMinMax(mFittings[i].GetId(), mFittings[i].GetProIdByPos(j), ref min, ref max))
                    continue;

                operation.AddPro(mFittings[i].GetProIdByPos(j), mFittings[i].GetProValueByPos(j));
            }
        }

    }

	//翅膀属性
	public static void BuildWingProperty(PlayerData mainData,PropertyOperation operation)
	{
		operation.Clear();
	    for (int i = 0; i < mainData.mWingData.wingItems.Count; ++i)
	    {

	        operation.AddPro((int) PropertyTypeEnum.PropertyTypeMaxHP, mainData.mWingData.wingItems[i].life);
	        operation.AddPro((int) PropertyTypeEnum.PropertyTypeDamage, mainData.mWingData.wingItems[i].attack);
	        operation.AddPro((int) PropertyTypeEnum.PropertyTypeDefance, mainData.mWingData.wingItems[i].defence);
	        operation.AddPro((int) PropertyTypeEnum.PropertyTypeCrticalLV, mainData.mWingData.wingItems[i].critical);
	        operation.AddPro((int) PropertyTypeEnum.PropertyTypeMaxMana, mainData.mWingData.wingItems[i].power);

	    }
	}

    //时装属性
    public static void BuildFashionProperty(PlayerData mainData, PropertyOperation operation)
    {
        operation.Clear();
        for (int i = 0; i < mainData.mFashion.unlock_count; ++i)
        {
           FashionTableItem res = DataManager.FashionTable[mainData.mFashion.items[i].id] as FashionTableItem;
           if(res == null) return;
            var pro_res = DataManager.FashionPropTable[mainData.mFashion.items[i].starnum + res.propid] as FashionPropTableItem;
            if (pro_res == null) return;

            operation.AddPro((int)PropertyTypeEnum.PropertyTypeMaxHP, (pro_res.life>0) ? pro_res.life: 0);
            operation.AddPro((int)PropertyTypeEnum.PropertyTypeDamage, (pro_res.fight > 0) ? pro_res.fight : 0);
            operation.AddPro((int)PropertyTypeEnum.PropertyTypeDefance, (pro_res.defence > 0) ? pro_res.defence : 0);
            operation.AddPro((int)PropertyTypeEnum.PropertyTypeCrticalLV, (pro_res.critical > 0) ? pro_res.critical : 0);
            operation.AddPro((int)PropertyTypeEnum.PropertyTypeMaxMana, (pro_res.power > 0) ? pro_res.power : 0);
        }
    }

    //装备属性
    public static void BuildEquipProperty(PlayerData mainData, PropertyOperation operation)
    {
        operation.Clear();
        Dictionary<int, ItemObj> dic = mainData.mPack.getPackDic(PackageType.Pack_Equip);

        foreach (KeyValuePair<int, ItemObj> v in dic)
        {
            v.Value.BuildProperty(operation);
        }
    }

    //构建装备显示外形列表
    public static void BuildEquipView(PlayerData mainData , int[] views)
    {
        Dictionary<int, ItemObj> dic = mainData.mPack.getPackDic(PackageType.Pack_Equip);

        for( int i = 0 ; i < (int)EquipSlot.EquipSlot_MAX ; ++i )
        {
            if( !dic.ContainsKey( i ) )
            {
                views[i] = -1;
                continue;
            }
            DefenceObj obj = dic[i] as DefenceObj;
            if( obj == null )
            {
                views[i] = -1;
                continue;
            }
            int slot = obj.GetDeRes().slot;
            if (slot < 0 || slot >= (int)EquipSlot.EquipSlot_MAX)
            {
                views[i] = -1;
                continue;
            }
            views[i] = obj.GetDeRes().partmodel;
        }
    }
}

