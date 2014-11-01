using UnityEngine;
using System.Collections;

// 竞技场事件
public class ArenaEvent : EventBase
{
	// 收到对手刷新数据
	public static string RECEIVE_REFRESH_DATA = "RECEIVE_ARENA_REFRESH_DATA";

	// 更新主角数据
	public static string RECEIVE_MAIN_DATA = "RECEIV_ARENAE_MAIN_DATA";

	// 更新战绩数据
	public static string RECEIVE_RECORD_DATA = "RECEIVE_ARENA_RECORD_DATA";

	// 获胜奖励
	public static string RECEIVE_END_DATA = "RECEIVE_ARENA_END_DATA";

	// 通知UI CD中
	public static string UI_ARENA_BEGIN_FAILED_CD = "UI_ARENA_BEGIN_FAILED_CD";

	// 通知UI 次数不够
	public static string UI_ARENA_BEGIN_FAILED_NOTIMES = "UI_ARENA_BEGIN_FAILED_NOTIMES";

	public ArenaEvent(string eventName)
		: base(eventName)
	{

	}
}
