using System;
using ProtoBuff.Serialization;
using Message;

public class DailyResetAction : LogicAction<request_daily_reset, respond_daily_reset>
{
    public DailyResetAction()
        : base((int)Message.MESSAGE_ID.ID_MSG_DAILY_RESET)
    {
       
    }

    protected override void OnRequest(request_daily_reset request, object userdata)
    {
    }

    protected override void OnRespond(respond_daily_reset respond, object userdata)
    {

    }
}
