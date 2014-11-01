using System;
using System.Collections.Generic;
using Message;
public class CreateRoleActionParam
{
    public string UserName
    {
        get;
        set;
    }
    public uint id
    {
        get;
        set;
    }

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
}

public class CreateRoleAction : BaseAction<request_create_role, respond_create_role>
{
    public CreateRoleAction()
        : base((int)MESSAGE_ID.ID_MSG_CREATE_ROLE)
    {
    }

    protected override void OnRequest(request_create_role request, object userdata)
    {
        CreateRoleActionParam param = userdata as CreateRoleActionParam;
        if (param == null)
            return;
        request.name = param.UserName;
        request.resid = param.id;

        request.openId = param.OpenId;
        request.platform = param.Platform;
        request.accesstoken = param.AccessToken;
        request.paytoken = param.PayToken;
        request.pf = param.Pf;
        request.pfkey = param.PfKey;
        request.regchannel = param.RegChannel;
        request.setupchannel = param.SetupChannel;
        request.clientsystem = param.ClientSystem;
        request.txplat = param.TXPlat;
    }

    protected override void OnRespond(respond_create_role respond, object userdata)
    {
        switch((ERROR_CODE)respond.result)
        {
            case ERROR_CODE.ERR_CREATE_OK:
                {
                    GameDebug.Log("创建角色成功：guid = " + respond.charguid);

                    GUID guid = respond.charguid;
                    Net.Instance.DoAction((int)MESSAGE_ID.ID_MSG_ENTER_GAME, guid);
                }
                break;
            case ERROR_CODE.ERR_CREATE_FAILED_NAME_TOO_LONG:
                {
                    GameDebug.Log("创建角色失败，角色名过长");
                    CreateRoleEvent e = new CreateRoleEvent(CreateRoleEvent.LOGIN_EVENT_CREATE_ROLE_RST);
                    e.Message = "创建角色失败，角色名过长";
                    EventSystem.Instance.PushEvent(e);
                }
                break;
            case ERROR_CODE.ERR_CREATE_FAILED_MAX_COUNT:
                {
                    GameDebug.Log("创建角色失败，已达最大的角色数据");
                    CreateRoleEvent e = new CreateRoleEvent(CreateRoleEvent.LOGIN_EVENT_CREATE_ROLE_RST);
                    e.Message = "创建角色失败，已达最大的角色数据";
                    EventSystem.Instance.PushEvent(e);
                }
                break;
            case ERROR_CODE.ERR_CREATE_FAILED_NAME:
                {
                    GameDebug.Log("创建角色失败，角色名不可使用");
                    CreateRoleEvent e = new CreateRoleEvent(CreateRoleEvent.LOGIN_EVENT_CREATE_ROLE_RST);
                    e.Message = "创建角色失败，角色名不可使用";
                    EventSystem.Instance.PushEvent(e);
                }
                break;
            default:
                break;
        };
    }
}
