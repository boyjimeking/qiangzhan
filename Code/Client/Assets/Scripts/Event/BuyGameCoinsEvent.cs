using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 关卡事件
public class BuyGameCoinsNeedLoginEvent : EventBase
{
    public static string BUY_GAMECOINS_NEED_LOGIN_EVENT = "BUY_GAMECOINS_NEED_LOGIN_EVENT";

    public BuyGameCoinsNeedLoginEvent() : base(BUY_GAMECOINS_NEED_LOGIN_EVENT)
	{

	}
}

public class BuyGameCoinsRstEvent : EventBase
{
    public static string BUY_GAMECOINS_RST_EVENT = "BUY_GAMECOINS_RST_EVENT";

    public string Param;

    public BuyGameCoinsRstEvent() : base(BUY_GAMECOINS_RST_EVENT)
    {
    }
}
