using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;

public class StageEndModule : ModuleBase
{
	// 通关时间
	private uint mPassTime;

	// 评分
	private StageGrade mGrade;

	// 获得经验
	private int mExp;

	// 状态阶段
	public enum UIState : int
	{
		// 原始状态
		STATE_ORIGINAL = 0,

		// 界面从上到下大特效
		STATE_0,

		// 标题出现
		STATE_1,

		// 杀伤率出现 播放特效
		STATE_2,

		// 连击出现 播放特效
		STATE_3,

		// 时间出现 播放特效
		STATE_4,

		// 评分球出现
		STATE_5,

		// 评分出现
		STATE_6,

		// 信息上浮 评分球缩小 上浮
		STATE_7,

		// 第二页信息出现
		STATE_8,

		// 进度条增长
		STATE_9,

		// 显示升级信息
		STATE_10,

		// 等待结束
		STATE_11,

		// 结束
		STATE_12,
	}

	// 阶段时间
	public static float STATETIME_0 = 0.1f;
	public static float STATETIME_1 = 0.15f;
	public static float STATETIME_2 = 0.15f;
	public static float STATETIME_3 = 0.15f;
	public static float STATETIME_4 = 0.5f;
	public static float STATETIME_5 = 0.5f;
	public static float STATETIME_6 = 5.0f;
	public static float STATETIME_7 = 1.5f;
	public static float STATETIME_8 = 0.5f;
	public static float STATETIME_9 = 5.0f;
	public static float STATETIME_10 = 5.0f;

	// 当前阶段
	private UIState mState = UIState.STATE_ORIGINAL;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

	// 当前阶段
	public UIState State
	{
		get
		{
			return mState;
		}

		set
		{
			mState = value;
		}
	}

	public void SetPassTime(uint time)
	{
		mPassTime = time;
	}

	public void SetGrade(StageGrade grade)
	{
		mGrade = grade;
	}

	public uint GetPassTime()
	{
		return mPassTime;
	}

	public StageGrade GetGrade()
	{
		return mGrade;
	}

	public int GetExp()
	{
		return mExp;
	}

	public void SetExp(int exp)
	{
		if(exp < 0)
		{
			mExp = 0;
		}
		else
		{
			mExp = exp;
		}
	}

	public string GetPassTimeStr()
	{
		if(mPassTime == uint.MaxValue)
		{
			return "N/A";
		}

		uint min = (mPassTime / 60000) % 60;
		uint sec = (mPassTime / 1000) % 60;
		string str = "";
		if(min > 0)
		{
			str += min.ToString() + "\'";
		}
		if(sec > 0)
		{
			str += sec.ToString() + "\"";
		}

		return str;
	}
}
