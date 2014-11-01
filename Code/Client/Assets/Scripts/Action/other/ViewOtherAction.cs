using System;
using Message;

public class ViewOtherActionParam
{
    public GUID charGuid;
}

class ViewOtherAction : LogicAction<msg_other_player_request, msg_other_player_respond>
{
    public ViewOtherAction()
        : base((int)MESSAGE_ID.ID_MSG_REQUEST_OTHER)
    {
    }

    protected override void OnRequest(msg_other_player_request request, object userdata)
    {
        if (userdata is ViewOtherActionParam)
        {
            ViewOtherActionParam param = (ViewOtherActionParam)userdata;
            request.guid = param.charGuid.ToULong();
        }
    }
    protected override void OnRespond(msg_other_player_respond respond, object userdata)
    {
        if( respond.errorcode == (int)ERROR_CODE.ERR_OTHER_FAILED )
        {
            //请求目标玩家信息失败
            return;
        }

        OtherDataPool.Instance.SyncOtherData(respond.pro);

        WindowManager.Instance.OpenUI("viewplayer", OtherDataPool.Instance.GetOtherData(), null, "ranking");
    }
}

