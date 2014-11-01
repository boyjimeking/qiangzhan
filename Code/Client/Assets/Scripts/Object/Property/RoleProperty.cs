using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoleProperty 
{
    private PropertyItem[] mPropertys = new PropertyItem[(int)PROPERTY_DEFINE.MAX_PROPERTY_NUMBER];

    public RoleProperty()
    {
        for( int i = 0 ; i < mPropertys.Length ; ++i )
        {
            mPropertys[i] = new PropertyItem();
        }
    }
	//角色基础属性
	private void __SetBaseProperty(int id , float baseValue)
	{
        if (id < 0 || id >= (int)PROPERTY_DEFINE.MAX_PROPERTY_NUMBER)
            return;
        PropertyItem item = mPropertys[id];
// 		if (!mPropertys.ContainsKey(id))
// 		{
// 			item = new PropertyItem();
// 			mPropertys.Add(id, item);
// 		}
// 		else
// 			item = mPropertys[id] as PropertyIte
		item.baseValue = baseValue;
	}

    private float __GetBaseProperty(int id)
	{
        if (id < 0 || id >= (int)PROPERTY_DEFINE.MAX_PROPERTY_NUMBER)
            return 0;
        PropertyItem item = mPropertys[id];
		//PropertyItem item = mPropertys[id] as PropertyItem;
		return item.baseValue;
	}

    private float __GetPropertyValue(int id)
	{
        if (id < 0 || id >= (int)PROPERTY_DEFINE.MAX_PROPERTY_NUMBER)
            return 0;
        PropertyItem item = mPropertys[id];
		//PropertyItem item = mPropertys[id] as PropertyItem;
		return item.GetValue();
	}

    private void __AddPropertyValue(int id, float addValue, float minValue)
	{
        if (id < 0 || id >= (int)PROPERTY_DEFINE.MAX_PROPERTY_NUMBER)
            return ;
        PropertyItem item = mPropertys[id];

        if (addValue < 0 && item.GetValue() + addValue < minValue)
        {
            addValue = minValue - item.GetValue() ;
        }

		item.addValue += addValue;
	}
	
	//对外接口
	public void AddProperty(int id , float addValue)
	{
        int basePro = PropertyBind.ToBind((PropertyTypeEnum)id);
        if( basePro < 0 )
        {
            return;
        }
        if (!PropertyBind.IsRate((PropertyTypeEnum)id))
		{
            __AddPropertyValue(basePro, addValue, PropertyBind.GetMinValue((PropertyTypeEnum)id));
        }
        else
		{
			float value = (addValue / 100.0f) * (float)__GetBaseProperty( basePro );
            __AddPropertyValue(basePro, value, PropertyBind.GetMinValue((PropertyTypeEnum)id));
        }
	}
    public float GetProperty(int id)
    {
        int basePro = PropertyBind.ToBind((PropertyTypeEnum)id);
        return __GetPropertyValue(basePro);
    }
    public void SetBaseProperty(int id, float addValue)
    {
        int basePro = PropertyBind.ToBind((PropertyTypeEnum)id);
        __SetBaseProperty(basePro, addValue);
    }
    public float GetBaseProperty(int id)
    {
        int basePro = PropertyBind.ToBind((PropertyTypeEnum)id);
        return __GetBaseProperty(basePro);
    }

    public static string GetPropertyName(int id)
    {
        PropertyTableItem item = DataManager.PropertyTable[id] as PropertyTableItem;
        if (item == null)
            return "error";

        return item.name;
    }

}
