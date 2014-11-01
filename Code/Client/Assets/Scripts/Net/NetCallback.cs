using System;

//处理消息错误等 
class NetCallback
{
    public static void OnRequestDelegate(Net.Status eState)
    {
        if (eState == Net.Status.eStartRequest)
        {
            WindowManager.Instance.OpenUI("waiting");
        }
        else
        {
            WindowManager.Instance.CloseUI("waiting");
        }
    }
    public static void OnNetError(Net.eNetError nType, int actionId, string strMsg)
    {
        string msg = "";
        switch (nType)
        {
            case Net.eNetError.eRequestFailed:
            case Net.eNetError.eTimeOut:
                {
                    msg = string.Format("请求数据失败, 是否重试", actionId);
                    YesOrNoBoxManager.Instance.ShowYesOrNoUI("提示", msg, Retry, null, BackToLogin);
                }
                break;
            case Net.eNetError.eInvalidActionId:
                {
                    Net.Instance.Ignore();
                    msg = string.Format("无效的 actionId = {0}", actionId);
                    GameDebug.LogError(msg);
                }
                break;
            case Net.eNetError.eInvalidSession:
                {
                    Net.Instance.DisConnect();
                    msg = "请重新登陆";
                    YesOrNoBoxManager.Instance.ShowYesOrNoUI("提示", msg, BackToLogin, null, BackToLogin);
                }
                break;
            default:
                {
                    Net.Instance.DisConnect();
                    msg = "请重新登陆";
                    YesOrNoBoxManager.Instance.ShowYesOrNoUI("提示", msg, BackToLogin, null, BackToLogin);
                }
                break;
        }

        if (nType == Net.eNetError.eInvalidSession)
        {
            
        }
        else
        {
            GameDebug.LogError(msg);
        }
    }
    public static void OnNetConnectOK()
    {
        LoginFlow flow = GameApp.Instance.GetCurFlow() as LoginFlow;
        if (flow != null)
        {
            flow.ConnectServerSucceed();
        }
        GameDebug.Log("connect server ok");
    }

    public static void OnNetConnectFail()
    {
        GameDebug.Log("连接到服务器失败");

        YesOrNoBoxManager.Instance.ShowYesOrNoUI("提示", "连接服务器失败,是否重试?", ReConnect, null);

//         LoginEvent e = new LoginEvent(LoginEvent.LOGIN_EVENT_LOGIN_RST);
//         e.Message = "服务器无效";
//         EventSystem.Instance.PushEvent(e);
    }

    private static void ReConnect(object param)
    {
        if (Net.Instance.IsConnected())
        {
            Net.Instance.DisConnect();
        }

        Net.Instance.Connect();
    }
    private static void BackToLogin(object param)
    {
        MainFlow flow = GameApp.Instance.GetCurFlow() as MainFlow;
        if( flow != null )
        {
            flow.BackToLogin();
        }
    }

    private static void Retry(object param)
    {
        Net.Instance.Resum();
    }
}

