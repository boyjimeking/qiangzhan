using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;

public class StageBalanceModule : ModuleBase
{
	// 通关时间
	private uint mPassTime;

	// 评分
	private StageGrade mGrade;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
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
}
