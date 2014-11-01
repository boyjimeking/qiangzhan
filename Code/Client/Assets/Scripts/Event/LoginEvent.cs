using UnityEngine;
using System.Collections;

public class LoginEvent : EventBase 
{
    public static string LOGIN_EVENT_LOGIN = "LOGIN_EVENT_LOGIN";
    public static string LOGIN_EVENT_LOGIN_RST = "LOGIN_EVENT_LOGIN_RST";
    public static string LOGIN_EVENT_TENCENT_LOGIN_RST = "LOGIN_EVENT_TENCENT_LOGIN_RST";

    public string OpenId;
    public string Platform;
    public string AccessToken;
    public string PayToken;
    public string Pf;
    public string PfKey;
    public bool TencentLoginSucceed;

    public bool TencentLogin = false;

    public LoginEvent(string eventName)
        : base(eventName)
    {
        
    }

    public string UserName
    {
        get;
        set;
    }

    public string PassWord
    {
        get;
        set;
    }

    public string Message
    {
        get;
        set;
    }
}
