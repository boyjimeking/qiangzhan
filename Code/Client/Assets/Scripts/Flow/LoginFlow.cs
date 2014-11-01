using UnityEngine;
using System.Collections;
using System;
using Message;
using System.Text;

//这个流程中请求角色数据  / 创建角色等 
public class LoginFlow : BaseFlow 
{
    private string mUserName;
    private string mPasswd;
    private bool mTencentLogin;

    bool BaseFlow.Init()
    {
        WindowManager.Instance.EnterFlow(UI_FLOW_TYPE.UI_FLOW_LOGIN);

        WindowManager.Instance.OpenUI("common");
        WindowManager.Instance.CloseUI("common");

        //打开登陆界面
        
        WindowManager.Instance.OpenUI("login");

        EventSystem.Instance.addEventListener(LoginEvent.LOGIN_EVENT_LOGIN, onLogin);
        EventSystem.Instance.addEventListener(CreateRoleEvent.LOGIN_EVENT_CREATE_ROLE, onCreateRole);

        return false;
    }


    void onLogin(EventBase evt)
    {

        LoginEvent lv = evt as LoginEvent;
        if (lv == null)
            return;

        Net.Instance.SetUrl(Environment.SeverAddress);

        mUserName = lv.UserName;
        mPasswd = lv.PassWord;
        mTencentLogin = lv.TencentLogin;

        if (Net.Instance.IsConnected())
        {
            Net.Instance.DisConnect();
        }

        Net.Instance.Connect();
    }

    void onCreateRole(EventBase evt)
    {
        CreateRoleActionParam param = new CreateRoleActionParam();
        CreateRoleEvent cre = evt as CreateRoleEvent;
        param.UserName = cre.UserName;
        param.id = cre.id;

        param.OpenId = PlatformSDK.OpenId;
        param.Platform = PlatformSDK.Platform;
        param.AccessToken = PlatformSDK.AccessToken;
        param.PayToken = PlatformSDK.PayToken;
        param.Pf = PlatformSDK.Pf;
        param.PfKey = PlatformSDK.PfKey;
        param.RegChannel = PlatformSDK.RegChannel;
        param.SetupChannel = PlatformSDK.SetupChannel;
        param.ClientSystem = PlatformSDK.ClientSystem;
        param.TXPlat = PlatformSDK.TXPlat;

        Net.Instance.DoAction((int)MESSAGE_ID.ID_MSG_CREATE_ROLE, param);
    }

    bool BaseFlow.Term()
    {

        EventSystem.Instance.removeEventListener(LoginEvent.LOGIN_EVENT_LOGIN, onLogin);
        EventSystem.Instance.removeEventListener(CreateRoleEvent.LOGIN_EVENT_CREATE_ROLE, onCreateRole);

        CreateRoleManager.Instance.Clear();

        WindowManager.Instance.LeaveFlow(UI_FLOW_TYPE.UI_FLOW_LOGIN);
        return false;
    }
    GAME_FLOW_ENUM BaseFlow.GetFlowEnum()
    {
        return GAME_FLOW_ENUM.GAME_FLOW_LOGIN;
    }
    FLOW_EXIT_CODE BaseFlow.Update(uint elapsed)
    {
        CreateRoleManager.Instance.Update(elapsed);

        return FLOW_EXIT_CODE.FLOW_EXIT_CODE_ERROR;
    }

    public void ConnectServerSucceed()
    {
        GameDebug.Log("连接到服务器成功");

        LoginActionParam param = new LoginActionParam();
        param.ClientVer = AppSystemInfo.GetAppVersion();
        param.UserName = mUserName;
        param.Passwd = mPasswd;
        param.TencentLogin = mTencentLogin;

        param.OpenId = PlatformSDK.OpenId;
        param.Platform = PlatformSDK.Platform;
        param.AccessToken = PlatformSDK.AccessToken;
        param.PayToken = PlatformSDK.PayToken;
        param.Pf = PlatformSDK.Pf;
        param.PfKey = PlatformSDK.PfKey;
        param.RegChannel = PlatformSDK.RegChannel;
        param.SetupChannel = PlatformSDK.SetupChannel;
        param.ClientSystem = PlatformSDK.ClientSystem;
        param.TXPlat = PlatformSDK.TXPlat;

        Net.Instance.DoAction((int)MESSAGE_ID.ID_MSG_LOGIN, param, true);
    }

    public void PlatformLogin()
    {
        LoginEvent e = new LoginEvent(LoginEvent.LOGIN_EVENT_TENCENT_LOGIN_RST);
        e.TencentLoginSucceed = PlatformSDK.PlatformLoginSucceed;
        e.OpenId = PlatformSDK.OpenId;
        e.Platform = PlatformSDK.Platform;
        e.AccessToken = PlatformSDK.AccessToken;
        e.PayToken = PlatformSDK.PayToken;
        e.Pf = PlatformSDK.Pf;
        e.PfKey = PlatformSDK.PfKey;

        EventSystem.Instance.PushEvent(e);
    }
}
