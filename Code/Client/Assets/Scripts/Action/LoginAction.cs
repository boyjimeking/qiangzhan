
using System;
using System.Collections.Generic;
using System.Text;
using Message;
using System.Security.Cryptography;

public class LoginActionParam 
{
    public string ClientVer;
    public string OpenId;
    public string Platform;
    public string AccessToken;
    public string PayToken;
    public string Pf;
    public string PfKey;
    public uint RegChannel;
    public uint SetupChannel;
    public string ClientSystem;
    public string TXPlat;

    public string UserName;
    public string Passwd;

    public bool TencentLogin;
}

public class LoginAction : BaseAction<request_login, respond_login>
{
    public LoginAction()
        : base((int)MESSAGE_ID.ID_MSG_LOGIN)
    {
    }

    protected override void OnRequest(request_login request, object userdata)
    {
        LoginActionParam param = userdata as LoginActionParam;
        if (param == null)
            return;

        string md5_value = MD5Utils.Encrypt(param.Passwd);

        request.clientversion = param.ClientVer;
        request.tencentlogin = param.TencentLogin;

        request.usrname = param.UserName;
        request.passwd = HMACUtils.HMacSha1Encrypt(md5_value, Net.Instance.GetSessionKey());

        request.openId      = param.OpenId;		
        request.platform    = param.Platform;	
        request.accesstoken	= param.AccessToken;
        request.paytoken	= param.PayToken;
        request.pf          = param.Pf;
        request.pfkey       = param.PfKey;
        request.regchannel = param.RegChannel;
        request.setupchannel = param.SetupChannel;
        request.clientsystem = param.ClientSystem;
        request.txplat = param.TXPlat;
    }

    protected override void OnRespond(respond_login respond, object userdata)
    {
        if(respond.result == (uint)ERROR_CODE.ERR_LOGIN_OK)
        {
            LoginEvent e = new LoginEvent(LoginEvent.LOGIN_EVENT_LOGIN_RST);
            e.Message = "";
            EventSystem.Instance.PushEvent(e);

            GameDebug.Log("登陆成功");

            GUID charguid = respond.charguid;

            if (!charguid.IsValid())
            {
                GameDebug.Log("登陆成功，无角色，开始创建角色");

                //SceneManager.Instance.EnterScene(3);
                CreateRoleManager.Instance.EnterVirtualScene("city_train");
            }
            else
            {
                GameDebug.Log("登陆成功，角色已存在，开始进行游戏");

                Net.Instance.DoAction((int)MESSAGE_ID.ID_MSG_ENTER_GAME, charguid);
            }
        }
        else
        {
            if(respond.result == (uint)ERROR_CODE.ERR_LOGIN_FAILED_CLIENT_VERSION)
            {
                LoginEvent e = new LoginEvent(LoginEvent.LOGIN_EVENT_LOGIN_RST);
                e.Message = "请更新客户端";
                EventSystem.Instance.PushEvent(e);

                GameDebug.Log("登陆失败");
            }
            else
            {
                LoginEvent e = new LoginEvent(LoginEvent.LOGIN_EVENT_LOGIN_RST);
                e.Message = "角户名或密码不正确";
                EventSystem.Instance.PushEvent(e);

                GameDebug.Log("登陆失败");
            }
        }
    }
}
