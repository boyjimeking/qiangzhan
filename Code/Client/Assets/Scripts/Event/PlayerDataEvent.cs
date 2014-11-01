using UnityEngine;
using System.Collections;

//玩家的数据改变  不是属性改变
public class PlayerDataEvent : EventBase
{
	public static string PLAYER_DATA_CHANGED = "PLAYER_DATA_CHANGED";

    public PlayerDataEvent(string eventName)
        : base(eventName)
    {

    }
}
