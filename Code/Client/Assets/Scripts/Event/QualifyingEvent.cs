using UnityEngine;
using System.Collections;

// 排位赛事件
public class QualifyingEvent : EventBase
{
	// 收到对手数据
	public static string RECEIVE_LIST_DATA = "RECEIVE_QUALIFYING_LIST_DATA";

	// 更新主角数据
	public static string RECEIVE_MAIN_DATA = "RECEIVE_QUALIFYING_MAIN_DATA";

	// 更新战绩数据
	public static string RECEIVE_RECORD_DATA = "RECEIVE_QUALIFYING_RECORD_DATA";

	// 获胜奖励
	public static string RECEIVE_END_DATA = "RECEIVE_QUALIFYING_END_DATA";

	// 通知UI CD中
	public static string UI_QUALIFYING_BEGIN_FAILED_CD = "UI_QUALIFYING_BEGIN_FAILED_CD";

	// 通知UI 次数不够
	public static string UI_QUALIFYING_BEGIN_FAILED_NOTIMES = "UI_QUALIFYING_BEGIN_FAILED_NOTIMES";

	public QualifyingEvent(string eventName)
		: base(eventName)
	{

	}
}
