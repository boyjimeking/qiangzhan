using Message;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;

public class BuyGameCoinsModule : ModuleBase
{
    public BuyGameCoinsModule()
    {
        EventSystem.Instance.addEventListener(BuyGameCoinsNeedLoginEvent.BUY_GAMECOINS_NEED_LOGIN_EVENT, OnGameCoinsNeedLogin);
        EventSystem.Instance.addEventListener(BuyGameCoinsRstEvent.BUY_GAMECOINS_RST_EVENT, OnGameCoinsRst);
    }

    private void OnGameCoinsNeedLogin(EventBase e)
    {
        BuyGameCoinsNeedLoginEvent ev = e as BuyGameCoinsNeedLoginEvent;
        if (ev == null)
            return;

        YesOrNoBoxManager.Instance.ShowYesOrNoUI("提示", "请重新登录", null, null);
    }

    private void OnGameCoinsRst(EventBase e)
    {
        BuyGameCoinsRstEvent ev = e as BuyGameCoinsRstEvent;
        if (ev == null)
            return;

        QueryRechargeResultActionParam param = new QueryRechargeResultActionParam();
        param.OpenId = PlatformSDK.OpenId;
        param.AccessToken = PlatformSDK.AccessToken;
        param.Pf = PlatformSDK.Pf;
        param.PfKey = PlatformSDK.PfKey;
        param.PayToken = PlatformSDK.PayToken;
        param.Platform = PlatformSDK.Platform;

        Net.Instance.DoAction((int)MESSAGE_ID.ID_MSG_QUERY_RECHARGE_RESULT, param);
    }
}