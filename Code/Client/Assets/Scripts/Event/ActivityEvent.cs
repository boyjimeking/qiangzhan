using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 关卡事件
public class ActivityDataUpdateEvent : EventBase
{
    public static string ACTIVITYDATA_UPDATE_EVENT = "ACTIVITYDATA_UPDATE_EVENT";

    public ActivityDataUpdateEvent()
        : base(ACTIVITYDATA_UPDATE_EVENT)
	{

	}
}
