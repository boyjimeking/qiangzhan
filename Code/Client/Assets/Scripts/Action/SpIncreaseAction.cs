using System;
using System.Collections.Generic;
using Message;

public class SpIncreaseAction : LogicAction<request_msg_sp_increase, respond_msg_sp_increase>
{
	public SpIncreaseAction()
		: base((int)MESSAGE_ID.ID_MSG_SP_INCREASE)
    {

    }

	protected override void OnRequest(request_msg_sp_increase request, object userdata)
    {

    }

	protected override void OnRespond(respond_msg_sp_increase respond, object userdata)
    {

    }
}
