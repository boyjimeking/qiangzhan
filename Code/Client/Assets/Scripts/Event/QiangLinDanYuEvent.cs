using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 关卡事件
public class QiangLinDanYuUpdateEvent : EventBase
{
    public static string QIANGLINDANYU_UPDATE_EVENT = "QIANGLINDANYU_UPDATE_EVENT";
    public uint score = 0;
    public uint playerCount = 0;
    public List<Message.qianglindanyu_role_info> sortInfo = new List<Message.qianglindanyu_role_info>();

    public QiangLinDanYuUpdateEvent()
        : base(QIANGLINDANYU_UPDATE_EVENT)
	{

	}
}

public class QiangLinDanYuOverEvent : EventBase
{
    public static string QIANGLINDANYU_OVER_EVENT = "QIANGLINDANYU_OVER_EVENT";

    public QiangLinDanYuOverEvent(string eventName)
		: base(eventName)
	{

	}
}
public class QiangLinDanYuKillEnemyEvent : EventBase
{
    public static string QIANGLINDANYU_KILL_ENEMY_EVENT = "QIANGLINDANYU_KILL_ENEMY_EVENT";
    public object msg;

    public QiangLinDanYuKillEnemyEvent(string eventName)
        : base(eventName)
    {

    }
}
