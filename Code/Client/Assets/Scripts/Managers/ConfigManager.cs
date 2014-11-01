
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public enum ConfigItemKey : int
{
    KEY_CHALLENGE_SWEEP_ITEM_RES_ID = 0,
    KEY_CHALLENGE_SWEEP_ITEM_RES_NUM = 1, 

    SECRET_SHOP_REFRESH_PROC_TYPE = 2,       //�̵�ˢ�����Ļ�������;
    SECRET_SHOP_REFRESH_PROC_NUM = 3,        //�̵�ˢ�����Ļ���ֵ;
    SECRET_SHOP_REFRESH_TIME_BUCKET = 4,     //�̵�ˢ��ʱ���;9|12|18;

    ZOMBIE_PICK_ID = 5,                      //��ʬ����pick Id;
    ZOMBIE_BUFF_PICK_NUM = 6,                //��Ҫʰȡ��Ӧpick Id��Ʒ������;
    ZOMBIE_BUFF_ID = 7,                      //��ʬ����Ӧ��Buff id;

    FUND_CHARGE_NUM = 8,                     //������������ʯ��;
    FUND_REWARD_DAYS_NUM = 9,                //��������������;
    FUND_OPEN_LV = 12,                       //����������ŵȼ�;
    BATTLE_GRADE_LEVEL = 13,                 //ս��������;

};

public class ConfigManager
{

    public static int GetChallengeSweepNeedItemResID()
    {
        return (int)GetVal<int>(ConfigItemKey.KEY_CHALLENGE_SWEEP_ITEM_RES_ID);
    }

    public static int GetChallengeSweepNeedItemNum()
    {
        return (int)GetVal<int>(ConfigItemKey.KEY_CHALLENGE_SWEEP_ITEM_RES_NUM);
    }

    public static string GetValStr(ConfigItemKey key)
    {
        int k = (int)key;
        if (!DataManager.ConfigTable.ContainsKey(k))
            return null;

        ConfigTableItem item = DataManager.ConfigTable[k] as ConfigTableItem;

        return item.value;
    }

    /// <summary>
    /// ֻ֧��int/uint/string/float/double������;
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public static object GetVal<T>(ConfigItemKey key)
    {
        int k = (int)key;
        if (!DataManager.ConfigTable.ContainsKey(k))
            return default(T);

        ConfigTableItem item = DataManager.ConfigTable[k] as ConfigTableItem;

        if (typeof(T) == typeof(int))
            return int.Parse(item.value);
        
        if (typeof(T) == typeof(uint))
            return uint.Parse(item.value);

        if (typeof(T) == typeof(string))
            return item.value;

        if (typeof(T) == typeof(float))
            return float.Parse(item.value);

        if (typeof(T) == typeof(double))
            return double.Parse(item.value);
        //if (typeof(T) == typeof(System.Enum))
        //    return System.Enum.Parse(typeof(T), item.value);

        return default(T);
    }
}