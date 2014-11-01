using UnityEngine;
using System.Collections;

public class UILogin : UIWindow
{
    private GameObject loginButton1;
    private UIInput mUserName;
    private UIInput mPasswd;
    private UILabel mMessage;
    private UIPopupList mServerList;

    private UIPopupList mResolutionList;

    private UIButton mQQLogin;
	private UIButton mWxLogin;

    private bool mAutoLogin = false;
    public UILogin()
    {

    }
    protected override void OnLoad()
    {
        loginButton1 = this.FindChild("Button1");
        mUserName = this.FindComponent<UIInput>("Username");
        mPasswd = this.FindComponent<UIInput>("passwd");
        mMessage = this.FindComponent<UILabel>("message");
        mServerList = this.FindComponent<UIPopupList>("serverList");
        mResolutionList = this.FindComponent<UIPopupList>("resolutionList");
        mQQLogin = this.FindComponent<UIButton>("QQLogin");
		mWxLogin = this.FindComponent<UIButton>("WXLogin");


		EventSystem.Instance.addEventListener(LoginEvent.LOGIN_EVENT_LOGIN_RST, onLoginRst);
        EventSystem.Instance.addEventListener(LoginEvent.LOGIN_EVENT_TENCENT_LOGIN_RST, onTencentLoginRst);

        UIEventListener.Get(loginButton1).onClick = onLogin;
        string username = PlayerPrefs.GetString("username");
        string passwd = PlayerPrefs.GetString("passwd");

        if (!string.IsNullOrEmpty(username))
        {
            mUserName.value = username;
        }

        if (!string.IsNullOrEmpty(passwd))
        {
            mPasswd.value = passwd;
        }

#if UNITY_ANDROID && !UNITY_EDITOR
		//mServerList.items.Add("http://77.77.1.104:8087");
        mServerList.items.Add(GameConfig.ServerAddress);
        mServerList.items.Add("http://182.254.203.110:8003");
#else
        //mServerList.items.Add(GameConfig.ServerAddress);
        //mServerList.items.Add("http://192.168.80.100:10000");
        mServerList.items.Add("http://127.0.0.1:8087");
#endif


        mServerList.value = PlayerPrefs.GetString("ip");

#if UNITY_ANDROID && !UNITY_EDITOR
        //loginButton1.gameObject.SetActive(false);
        //mUserName.gameObject.SetActive(false);
        //mPasswd.gameObject.SetActive(false);
        //mQQLogin.gameObject.SetActive(true);
        //mWxLogin.gameObject.SetActive(true);
        //FindChild("LoginBK").SetActive(false);
        //FindChild("Background").SetActive(false);
#else
        mQQLogin.gameObject.SetActive(false);
		mWxLogin.gameObject.SetActive(false);
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
        EventDelegate.Add(mQQLogin.onClick, onQQLogin);
		EventDelegate.Add(mWxLogin.onClick, onWeiXinLogin);
#endif

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        NGUITools.SetActive(mResolutionList.gameObject, true);
        mResolutionList.items.Add("Not Changed");
        mResolutionList.items.Add("Iphone4s-960*640");
        mResolutionList.items.Add("IPADmini2-1024*768");
        mResolutionList.items.Add("Iphone5-1136*640");
        mResolutionList.items.Add("xiaomi2-1280*720");

        string str = PlayerPrefs.GetString("resolution");
        mResolutionList.value = "Not Changed";
#else
        NGUITools.SetActive(mResolutionList.gameObject, false);

        //onAutoLogin();
#endif

    }

    void SetResolution()
    {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        string res = mResolutionList.value;
        if (res != "Not Changed")
        {
            string[] temp = res.Split(new char[] { '-' });
            string[] v = temp[1].Split(new char[] { '*' });
            Screen.SetResolution(int.Parse(v[0]), int.Parse(v[1]), false);
        }
#endif
    }

    void onLogin(GameObject target)
    {
        SetResolution();

        Environment.Operation = 0;

        string username = NGUIText.StripSymbols(mUserName.value);
        string passwd = NGUIText.StripSymbols(mPasswd.value);

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(passwd))
        {
            mMessage.text = "[ff0000]用户名或密码不可为空";
            return;
        }

        Environment.SeverAddress = mServerList.value;
        Environment.ChatServerAddress = GameConfig.ChatServerAddress;

        LoginEvent e = new LoginEvent(LoginEvent.LOGIN_EVENT_LOGIN);
        e.TencentLogin = false;
        e.UserName = username;
        e.PassWord = passwd;
   
        EventSystem.Instance.PushEvent(e);

        PlayerPrefs.SetString("username", username);
        PlayerPrefs.SetString("passwd", passwd);
        PlayerPrefs.SetString("ip", Environment.SeverAddress);
    }

    void onLoginRst(EventBase evt)
    {
        LoginEvent le = evt as LoginEvent;
        if (le == null)
            return;

        mMessage.text = "[ff0000]" + le.Message;
    }

    void onTencentLoginRst(EventBase evt)
    {
        LoginEvent le = evt as LoginEvent;
        if (le == null)
            return;

        if (mAutoLogin)
        {
            if(!le.TencentLoginSucceed)
            {// 自动tencent授权失败，重新授权
                mMessage.text = "[ff0000]自动授权失败，重新授权";
                return;
            }
        }
        
        if(!le.TencentLoginSucceed)
        {// 授权失败，提示重试
            mMessage.text = "[ff0000]授权失败，请重试";
            return;
        }

        mMessage.text = "[ff0000]授权成功";

        // tencent 授权成功，登陆到服务器进入游戏
        LoginEvent e = new LoginEvent(LoginEvent.LOGIN_EVENT_LOGIN);

        e.UserName = le.OpenId;
        e.PassWord = "111";

        e.TencentLogin = true;

        e.OpenId = le.OpenId;
        e.Platform = le.Platform;
        e.AccessToken = le.AccessToken;
        e.PayToken = le.PayToken;
        e.Pf = le.Pf;
        e.PfKey = le.PfKey;

        if(le.Platform == "QQ")
        {
            Environment.SeverAddress = GameConfig.QQServerAddress;
            Environment.ChatServerAddress = GameConfig.QQChatServerAddress;
        }
        else if(le.Platform == "WX")
        {
            Environment.SeverAddress = GameConfig.WXServerAddress;
            Environment.ChatServerAddress = GameConfig.WXChatServerAddress;
        }

        EventSystem.Instance.PushEvent(e);
    }

    void onQQLogin()
    {
        mAutoLogin = false;
        PlatformSDK.QQLogin();
    }

    void onWeiXinLogin()
    {
        mAutoLogin = false;
        PlatformSDK.WeiXinLogin();
    }

    void onAutoLogin()
    {
        mAutoLogin = true;
        PlatformSDK.AutoLogin();
    }
    protected override void OnOpen(object param = null)
    {
        //LoadingManager.Instance.CloseLoading();
    }
    protected override void OnPreOpen(object param = null)
    {
        LoadingManager.Instance.CloseLoading();
    }
    protected override void OnClose()
    {

    }


}
