using UnityEngine;
using System.Collections;

public class UIBossAlert : UIWindow
{
	private float mTimer = 0.0f;

    protected override void OnLoad()
    {

    }

	//界面打开
	protected override void OnOpen(object param = null)
	{
		mTimer = 0.0f;
	}
	//界面关闭
	protected override void OnClose()
	{

	}
	public override void Update(uint elapsed)
	{
		float delta = (float)elapsed / 1000.0f;
		mTimer += delta;

		if (mTimer > 3.0f)
		{
			WindowManager.Instance.CloseUI("bossalert");
			return;
		}
	}
}
