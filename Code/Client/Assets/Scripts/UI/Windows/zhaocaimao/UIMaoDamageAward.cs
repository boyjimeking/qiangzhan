using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIMaoDamageAward : UIWindow
{
	private UILabel mAwardText;

	private uint mTimer = 0;

	protected override void OnLoad()
	{
		mAwardText = FindComponent<UILabel>("mAwardText");
	}

	//界面打开
	protected override void OnOpen(object param = null)
	{
		if (param == null)
			return;

		base.OnOpen(param);

		mView.SetActive(false);
		mTimer = 0;

		UpdateAwardText((int)param);
	}

	//界面关闭
	protected override void OnClose()
	{
		base.OnClose();
	}

	private void UpdateAwardText(int award)
	{
		mAwardText.text = "X" + award.ToString();
	}

	public override void Update(uint elapsed)
	{
		base.Update(elapsed);

		if (mTimer < 1500)
		{
			mTimer += elapsed;

			if (mTimer >= 1500)
			{
				mView.SetActive(true);
			}
		}
	}
}
