using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 关卡事件
public class TDSceneLifeUpdateEvent : EventBase
{
    public static string TD_SCENE_LIFE_UPDATE_EVENT = "TD_SCENE_LIFE_UPDATE_EVENT";
	public int mLife = 0;

	public TDSceneLifeUpdateEvent(int life = 0)
		: base(TD_SCENE_LIFE_UPDATE_EVENT)
	{
		mLife = life;
	}
}
