using Message;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class BattleUIModule : ModuleBase
{
	// 进度条
	private int mTotalProgNum = 0;
	private int mCurProgNum = 0;
	private string[] mAlias = null;
	private float mAngle = 0.0f;

	// 计时
	private bool mShowTimer = false;

	public void ShowProgress(int total, string alias, float angle)
	{
		mTotalProgNum = total;
		mCurProgNum = total;
		mAngle = angle;

		mAlias = alias.Split(',');

		BattleUIEvent e = new BattleUIEvent(BattleUIEvent.BATTLE_UI_PROGRESS_SHOW);
		e.msg = mCurProgNum;
		e.msg1 = mTotalProgNum;
		EventSystem.Instance.PushEvent(e);
	}

	public void OnKillEnemy(ObjectBase obj)
	{
		if (obj == null || mAlias == null || string.IsNullOrEmpty(obj.GetAlias()))
			return;

		foreach(string s in mAlias)
		{
			if(string.Compare(s, obj.GetAlias()) == 0)
			{
				Dec(1);
				break;
			}
		}
	}

	private void UpdateUI()
	{
		BattleUIEvent eUpdate = new BattleUIEvent(BattleUIEvent.BATTLE_UI_PROGRESS_UPDATE);
		eUpdate.msg = mCurProgNum;
		eUpdate.msg1 = mTotalProgNum;
		EventSystem.Instance.PushEvent(eUpdate);
	}

	private void Dec(int n)
	{
		mCurProgNum -= n;
		if (mCurProgNum < 0)
			mCurProgNum = 0;

		UpdateUI();

		if(mCurProgNum < 1)
		{
			BattleUIEvent eOver = new BattleUIEvent(BattleUIEvent.BATTLE_UI_PROGRESS_OVER);
			eOver.msg = mAngle;
			EventSystem.Instance.PushEvent(eOver);
		}
	}

	public void ShowTimer(bool b)
	{
		mShowTimer = b;

		BattleUIEvent e = new BattleUIEvent(BattleUIEvent.BATTLE_UI_SHOW_TIMER);
		e.msg = b;
		EventSystem.Instance.PushEvent(e);
	}

	public bool IsShowTimer()
	{
		return mShowTimer;
	}
}
