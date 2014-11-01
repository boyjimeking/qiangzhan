using UnityEngine;
using System.Collections;

public class BossBloodUpdateEvent : EventBase
{
	// Boss血量更新
	public static string BOSS_BLOOD_UPDATE = "BOSS_BLOOD_UPDATE";

    public static string BOSS_ENTER_FURY = "BOSS_ENTER_FURY";
    public static string BOSS_LEAVE_FURY = "BOSS_LEAVE_FURY";

	public string mName = null;

	public string mIcon = null;

	public uint mLevel = 0;

	public int mCurProgress = 0;

	public int mMaxProgress = 0;

	public int mHpUnit = 0;

    public bool mFury = false;

    public BossBloodUpdateEvent(string eventName)
        : base(eventName)
    {

    }
    public BossBloodUpdateEvent(string eventName, string name, string icon, uint level, int cur, int max, int unit, bool fury)
        : base(eventName)
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
