using UnityEngine;
using System.Collections;
using System.IO;
using LitJson;

public class PlatformSDK : MonoBehaviour
{
    private const string LoginType_Invalid  = "LoginType_Invalid";
	private const string LoginType_Auto     = "LoginType_Auto";
	private const string LoginType_QQ       = "LoginType_QQ";
    private const string LoginType_WeiXin   = "LoginType_WeiXin";

    private static PlatformSDK mInstance = null;

    public static string   OpenId   = "";
    public static string Platform   = "";
    public static string AccessToken= "";   
    public static string PayToken   = "";
    public static string Pf         = "";
    public static string PfKey      = "";
    public static string WxAccessToken  = "";
    public static long WxAccessTokenExpire  = 0;
    public static string WxRefreshToken     = "";
    public static long WxRefreshTokenExpire = 0;
    public static bool PlatformLoginSucceed = false;
    public static uint RegChannel = 0;
    public static uint SetupChannel = 0;
    public static string ClientSystem = "";
    public static string TXPlat = "";

#if UNITY_ANDROID && !UNITY_EDITOR

    private AndroidJavaClass mAndroidJavaClass;
    private AndroidJavaObject mAndroidJavaObject;

#endif

    public static PlatformSDK Instance()
    {
        return mInstance;
    }

	void Awake()
	{
        Application.targetFrameRate = 30;

        if (mInstance == null) 
		{
            mInstance = this;
			OnLoad();
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }

#if UNITY_ANDROID && !UNITY_EDITOR

        mAndroidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        mAndroidJavaObject = mAndroidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
#endif
    }

	void Start () 
    {
	}

	void OnLoad()
	{
		DontDestroyOnLoad(this);
	}

    public static void AutoLogin()
    {

#if UNITY_ANDROID && !UNITY_EDITOR
        PlatformSDK.Instance().mAndroidJavaObject.Call("AutoLogin");
#endif

    }

    public static void QQLogin()
    {

#if UNITY_ANDROID && !UNITY_EDITOR
        PlatformSDK.Instance().mAndroidJavaObject.Call("QQLogin");
#endif

    }

    public static void WeiXinLogin()
    {

#if UNITY_ANDROID && !UNITY_EDITOR
        PlatformSDK.Instance().mAndroidJavaObject.Call("WXLogin");
#endif

    }

    public static void BuyGameCoins(uint num, uint plylevel)
    {
        JsonData param = new JsonData();

        param["openid"] = OpenId;
        param["loginchannel"] = SetupChannel;
        param["num"] = num.ToString();
        param["level"] = plylevel;

#if UNITY_ANDROID && !UNITY_EDITOR
        PlatformSDK.Instance().mAndroidJavaObject.Call("BuyGameCoins", param.ToJson());
#endif

    }


    // 平台通知函数
    public void OnBuyGameCoinsNeedLogin(string param)
    {
        GameApp.Instance.OnBuyGameCoinsNeedLogin(param);
    }

    public void OnBuyGameCoinsRst(string param)
    {
        GameApp.Instance.OnBuyGameCoinsRst(param);
    }


    public void OnLoginNotify(string param)
    {
        // param 参数格式 "loginType=%s|succeed=1|platform=%s|openId=%s|accessToken=%s|pf=%s|pfKey=%s|payToken=%s"

        bool succeed = false;
        string loginType = "";
        string platform = "";
        string openId = "";
        string accessToken = "";
        string payToken = "";
        string pf = "";
        string pfKey = "";
        string wxAccessToken = "";
        long wxAccessTokenExpire = 0;
        string wxRefreshToken = "";
        long wxRefreshTokenExpire = 0;

        string[] param_vec = param.Split(new char[] { '|' });
        int cnt = param_vec.Length;
        for (int i = 0; i < cnt; i++)
        {
            string[] key_value = param_vec[i].Split(new char[] { '=' });
            if (key_value.Length != 2)
                continue;

            if (key_value[0] == "loginType")
            {
                loginType = key_value[1];
            }
            else if (key_value[0] == "succeed")
            {
                succeed = System.Convert.ToInt32(key_value[1]) != 0;
            }
            else if (key_value[0] == "platform")
            {
                platform = key_value[1];
            }
            else if (key_value[0] == "openId")
            {
                openId = key_value[1];
            }
            else if (key_value[0] == "accessToken")
            {
                accessToken = key_value[1];
            }
            else if (key_value[0] == "pf")
            {
                pf = key_value[1];
                string[] pf_value = pf.Split(new char[] { '-' });
                if(pf_value.Length >= 4)
                {
                    TXPlat = pf_value[0];
                    RegChannel = System.Convert.ToUInt32(pf_value[1]);
                    ClientSystem = pf_value[2];
                    SetupChannel = System.Convert.ToUInt32(pf_value[3]);
                }
            }
            else if (key_value[0] == "pfKey")
            {
                pfKey = key_value[1];
            }
            else if (key_value[0] == "payToken")
            {
                payToken = key_value[1];
            }

        }

        PlatformLoginSucceed = succeed;
        OpenId = openId;
        Platform = platform;
        AccessToken = accessToken;
        PayToken = payToken;
        Pf = pf;
        PfKey = pfKey;
        WxAccessToken = wxAccessToken;
        WxAccessTokenExpire = wxAccessTokenExpire;
        WxRefreshToken = wxRefreshToken;
        WxRefreshTokenExpire = wxRefreshTokenExpire;

        GameApp.Instance.PlatformNotify();
    }

    //------------------------版本更新相关-------------------------------------------

    /// <summary>
    /// 是否需要更新
    /// </summary>
    public void CheckNeedUpdate()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        PlatformSDK.Instance().mAndroidJavaObject.Call("CheckNeedUpdate");
#endif
    }

    public void OnCheckNeedUpdateInfo(string param)
    {
		UpdateUtility.Instance.GetPlatformUpdate().OnCheckNeedUpdateInfo (param);
    }

    public void  StartUpdate(int utype)
    {
#if UNITY_ANDROID && !UNITY_EDITOR

        object[] args = new object[] { utype };
       PlatformSDK.Instance().mAndroidJavaObject.Call("StartUpdate", args);   
#endif
    }

    public void OnDownloadAppProgressChanged(string param)
    {
        QQPlatformUpdate platform = UpdateUtility.Instance.GetPlatformUpdate() as QQPlatformUpdate;
        if (platform == null)
            return;
        platform.OnDownloadAppProgressChanged(param);

    }
    public void OnDownloadAppStateChanged(string param)
    {
        QQPlatformUpdate platform = UpdateUtility.Instance.GetPlatformUpdate() as QQPlatformUpdate;
        if (platform == null)
            return;
        platform.OnDownloadAppStateChanged(param);
    }

    public void OnDownloadYYBProgressChanged(string param)
    {
        QQPlatformUpdate platform = UpdateUtility.Instance.GetPlatformUpdate() as QQPlatformUpdate;
        if (platform == null)
            return;
        platform.OnDownloadYYBProgressChanged(param);
    }

    public void OnDownloadYYBStateChanged(string param)
    {
        QQPlatformUpdate platform = UpdateUtility.Instance.GetPlatformUpdate() as QQPlatformUpdate;
        if (platform == null)
            return;
        platform.OnDownloadYYBStateChanged(param);
    }


}
