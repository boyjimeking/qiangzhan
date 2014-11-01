using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//Boss血槽界面管理器
public class BossBloodUIManager
{
	private string mName = null;

	private string mIcon = null;

	private uint mLevel = 0;

	private int mCurProgress = 0;

	private int mMaxProgress = 0;

	private int mHpUnit = 0;

    private static BossBloodUIManager instance = null;

	private uint mTimer = 0;

    private bool mFury = false;

	public static BossBloodUIManager Instance
    {
        get
        {
            return instance;
        }
    }

	public BossBloodUIManager()
    {
        instance = this;
    }

	public void Update(uint elapsed)
	{
		if(mName == null)
		{
			return;
		}

		mTimer += elapsed;

		if(mTimer > 50)
		{
			EventSystem.Instance.PushEvent(new BossBloodUpdateEvent(BossBloodUpdateEvent.BOSS_BLOOD_UPDATE, mName, mIcon, mLevel, mCurProgress, mMaxProgress, mHpUnit,mFury));
			mName = null;
			mTimer = 0;
		}
	}

	public void ChangeHp(string name, string icon, uint level, int cur, int max, int unit,bool fury)
	{
		if (mName == null || !name.Equals(mName) || cur != mCurProgress)
		{
			mName = name;
			mIcon = icon;
			mLevel = level;
			mCurProgress = cur;
			mMaxProgress = max;
			mHpUnit = unit;
            mFury = fury;
		}
	}
}