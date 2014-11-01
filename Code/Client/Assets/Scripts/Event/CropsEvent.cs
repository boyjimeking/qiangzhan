using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class CropsEvent : EventBase
{

    //标签变化
    public static string TAB_INDEX = "TAB_INDEX";

    //兑换佣兵
    public static string BUY_CROPS = "BUY_CROPS";

    //
    public static string CHANGE_CROPS = "CHANGE_CROPS";

    //升星成功
    public static string RISE_STARS = "RISE_STARS";

    // 请求复活
    public static string MAIN_CROPS_RELIVE_REQUEST = "MAIN_CROPS_RELIVE_REQUEST";
    public static string SUB_CROPS_RELIVE_REQUEST = "SUB_CROPS_RELIVE_REQUEST";

    // 请求复活等待时间
    public static string MAIN_CROPS_RELIVE_TIME_DOWN = "MAIN_CROPS_RELIVE_TIME_DOWN";

    public static string SUB_CROPS_RELIVE_TIME_DOWN = "SUB_CROPS_RELIVE_TIME_DOWN";

    // 复活答复
    public static string MAIN_CROPS_RELIVE_RESPOND = "MAIN_CROPS_RELIVE_RESPOND";
    public static string SUB_CROPS_RELIVE_RESPOND = "SUB_CROPS_RELIVE_RESPOND";

    // 佣兵死亡
    public static string CROPS_DIE_NO_RELIVE = "CROPS_DIE_NO_RELIVE";

    // 佣兵复活请求，无等待时间
    public static string CROPS_RELIVE_NO_TIME_DOWN = "CROPS_RELIVE_NO_TIME_DOWN";

	public int cropsid = 0;

    public CropsEvent(string eventName)
        : base(eventName)
    {

    }
}
