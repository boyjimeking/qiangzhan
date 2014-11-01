using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIMaoAlert : UIWindow
{
	private UILabel mTimeText;

	private int mTimer = 0;

	protected override void OnLoad()
	{
		mTimeText = this.FindComponent<UILabel>("mTimeText");
	}

	//界面打开
	protected override void OnOpen(object param = null)
	{
		if (param == null)
			return;

		base.OnOpen(param);

		mTimer = (int)param;
		UpdateTimeText();
	}

	//界面关闭
	protected override void OnClose()
	{
		base.OnClose();
	}

	public override void Update(uint elapsed)
	{
		base.Update(elapsed);

		if(mTimer > 0)
		{
			mTimer -= (int)elapsed;

			if (mTimer < 0)
				mTimer = 0;

			UpdateTimeText();
		}
	}

	private void UpdateTimeText()
	{
		mTimeText.text = (mTimer / 1000).ToString();
	}
}
