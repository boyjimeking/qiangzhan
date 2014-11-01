using System;
using UnityEngine;

public enum SHOOT_TYPE : int
{
    SHOOT_TYPE_NORMAL = 0,
    SHOOT_TYPE_AUTO = 1,
}

class SettingManager
{
    private static SettingManager instance = null;

    private string mPlayerName = "";

    private int mShootType = (int)SHOOT_TYPE.SHOOT_TYPE_NORMAL;

    public SettingManager()
	{
		instance = this;
	}
    public static SettingManager Instance
	{
		get
		{
			return instance;
		}
	}

    //全局配置
    public void InitGlobal()
    {

    }

    //玩家配置
    public void InitPlayer(string name)
    {
        mPlayerName = name;
        mShootType = PlayerPrefs.GetInt(mPlayerName + "_shoot_type", (int)SHOOT_TYPE.SHOOT_TYPE_NORMAL);
    }


    public int GetShootType()
    {
        return mShootType;
    }

    public void SetShootType(int type)
    {
        if( mShootType != type )
        {
            mShootType = type;
            PlayerPrefs.SetInt(mPlayerName + "_shoot_type", mShootType);

            BattleUIEvent ev = new BattleUIEvent(BattleUIEvent.BATTLE_UI_SHOOT_TYPE_CHANGE);
            EventSystem.Instance.PushEvent(ev);
        }
    }
}
