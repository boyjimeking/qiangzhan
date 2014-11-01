using UnityEngine;
using System.Collections;

//功能信息
public class FunctionEvent : EventBase
{
    public static string FUNCTION_LOCKED = "FUNCTION_LOCKED";
	public static string FUNCTION_UNLOCKED = "FUNCTION_UNLOCKED";
    public static string FUNCTION_RED_POINT = "FUNCTION_RED_POINT";

    public static string FUNCTION_CHECK_EVENT = "FUNCTION_CHECK_EVENT";

    public int functionid = -1;        //-1 更新所有
    public bool isShow = false;        // 是否打开;

    public FunctionEvent(string eventName)
        : base(eventName)
    {

    }
}
