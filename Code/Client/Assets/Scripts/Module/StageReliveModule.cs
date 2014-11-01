using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;

public class StageReliveModule : ModuleBase
{
	// 剩余复活次数
	private int mLeftTimes = 0;

	// 普通复活消耗
	private int mNormalCost = 0;

	// 普通消耗货币
	private string mNormalIcon = null;

	// 强力复活消耗
	private int mExtraCost = 0;

	// 强力复活货币
	private string mExtraIcon = null;

	// 等待复活消息答复
	private bool mWaitRelive = false;

	public bool WaitRelive
	{
		get
		{
			return mWaitRelive;
		}

		set
		{
			mWaitRelive = value;
		}
	}

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

	public void setReliveData(int leftTimes, int normalCost, string normalIcon, int extraCost, string extraIcon)
	{
		mLeftTimes = leftTimes;
		mNormalCost = normalCost;
		mNormalIcon = normalIcon;
		mExtraCost = extraCost;
		mExtraIcon = extraIcon;
	}

	// 得到剩余复活次数
	public int GetLeftTimes()
	{
		return mLeftTimes;
	}

	// 得到普通复活消耗
	public int GetNormalCost()
	{
		return mNormalCost;
	}

	// 得到普通消耗货币
	public string GetNormalIcon()
	{
		return mNormalIcon;
	}

	// 得到强力复活消耗
	public int GetExtraCost()
	{
		return mExtraCost;
	}

	// 得到强力复活货币
	public string GetExtraIcon()
	{
		return mExtraIcon;
	}
}
