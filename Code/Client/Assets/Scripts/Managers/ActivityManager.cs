using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Message;

public class SceneActivityParam
{
	public uint mStartTime = uint.MaxValue;
	public uint mOverTime = uint.MaxValue;
	public int mSceneId = -1;

	public SceneActivityParam(uint s, uint o, int id)
	{
		mStartTime = s;
		mOverTime = o;
		mSceneId = id;
	}
}

public class ActivityManager
{
    private static ActivityManager instance;

	private SceneActivityParam mParam = null;

	private List<other_role_fight_property> mPartnerList = new List<other_role_fight_property>();

	public ActivityManager()
	{
		instance = this;
	}

	public static ActivityManager Instance
	{
		get
		{
			return instance;
		}
	}

	public SceneActivityParam Param
	{
		get
		{
			return mParam;
		}

		set
		{
			mParam = value;
		}
	}

	public void Clear()
	{
		mParam = null;
	}

	public List<other_role_fight_property> PartnerList
	{
		get
		{
			return mPartnerList;
		}

		set
		{
			mPartnerList = value;
		}
	}
}
