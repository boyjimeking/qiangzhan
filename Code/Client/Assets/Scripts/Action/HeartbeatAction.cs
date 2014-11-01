using System;
using System.Collections.Generic;
using Message;

public class HeartbeatAction : LogicAction<request_hearbeat, respond_hearbeat>
{
    public HeartbeatAction()
        : base((int)MESSAGE_ID.ID_MSG_HEARBEAT)
    {
    }

    protected override void OnRequest(request_hearbeat request, object userdata)
    {
     
    }

    protected override void OnRespond(respond_hearbeat respond, object userdata)
    {

    }
}
