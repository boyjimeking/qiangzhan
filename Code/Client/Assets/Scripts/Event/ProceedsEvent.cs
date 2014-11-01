using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class ProceedsEvent : EventBase
{
    //所有收益
    public static string PROCEEDS_CHANGE_ALL = "PROCEEDS_CHANGE_ALL";
    //游戏币
    public static string PROCEEDS_CHANGE_ONE = "PROCEEDS_CHANGE_ONE";
    //rmb
    public static string PROCEEDS_CHANGE_TWO = "PROCEEDS_CHANGE_TWO";
    //声望
    public static string PROCEEDS_CHANGE_THREE = "PROCEEDS_CHANGE_THREE";
    //能源点
    public static string PROCEEDS_CHANGE_FOUR = "PROCEEDS_CHANGE_FOUR";
	//竞技场货币
	public static string PROCEEDS_CHANGE_FIVE = "PROCEEDS_CHANGE_FIVE";

	// 数值
	public uint value = 0;

    public ProceedsEvent(string eventName)
        : base(eventName)
    {

    }
}
