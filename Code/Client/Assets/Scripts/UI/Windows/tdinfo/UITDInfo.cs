using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UITDInfo : UIWindow
{
	// 剩余物资
	public UILabel mLifeText;

    protected override void OnLoad()
    {
		mLifeText = this.FindComponent<UILabel>("mLifeText");
    }

    //界面打开
    protected override void OnOpen(object param = null)
    {
		mLifeText.text = GameConfig.TDLifeCount.ToString();

		EventSystem.Instance.addEventListener(TDSceneLifeUpdateEvent.TD_SCENE_LIFE_UPDATE_EVENT, OnTDSceneLifeUpdate);
    }

    //界面关闭
    protected override void OnClose()
    {
		EventSystem.Instance.removeEventListener(TDSceneLifeUpdateEvent.TD_SCENE_LIFE_UPDATE_EVENT, OnTDSceneLifeUpdate);
    }

	private void OnTDSceneLifeUpdate(EventBase ev)
	{
		TDSceneLifeUpdateEvent e = ev as TDSceneLifeUpdateEvent;
		if(e == null)
			return;

		mLifeText.text = e.mLife.ToString();
	}
}
