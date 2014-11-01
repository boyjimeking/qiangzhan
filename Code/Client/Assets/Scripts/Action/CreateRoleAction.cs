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
                    GameDebug.Log("������ɫ�ɹ���guid = " + respond.charguid);

                    GUID guid = respond.charguid;
                    Net.Instance.DoAction((int)MESSAGE_ID.ID_MSG_ENTER_GAME, guid);
                }
                break;
            case ERROR_CODE.ERR_CREATE_FAILED_NAME_TOO_LONG:
                {
                    GameDebug.Log("������ɫʧ�ܣ���ɫ������");
                    CreateRoleEvent e = new CreateRoleEvent(CreateRoleEvent.LOGIN_EVENT_CREATE_ROLE_RST);
                    e.Message = "������ɫʧ�ܣ���ɫ������";
                    EventSystem.Instance.PushEvent(e);
                }
                break;
            case ERROR_CODE.ERR_CREATE_FAILED_MAX_COUNT:
                {
                    GameDebug.Log("������ɫʧ�ܣ��Ѵ����Ľ�ɫ����");
                    CreateRoleEvent e = new CreateRoleEvent(CreateRoleEvent.LOGIN_EVENT_CREATE_ROLE_RST);
                    e.Message = "������ɫʧ�ܣ��Ѵ����Ľ�ɫ����";
                    EventSystem.Instance.PushEvent(e);
                }
                break;
            case ERROR_CODE.ERR_CREATE_FAILED_NAME:
                {
                    GameDebug.Log("������ɫʧ�ܣ���ɫ������ʹ��");
                    CreateRoleEvent e = new CreateRoleEvent(CreateRoleEvent.LOGIN_EVENT_CREATE_ROLE_RST);
                    e.Message = "������ɫʧ�ܣ���ɫ������ʹ��";
                    EventSystem.Instance.PushEvent(e);
                }
                break;
            default:
                break;
        };
    }
}
