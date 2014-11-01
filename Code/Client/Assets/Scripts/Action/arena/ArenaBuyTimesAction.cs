using System;
using System.Collections.Generic;
using Message;

public class ArenaBuyTimesAction : LogicAction<request_msg_arena_buy_times, respond_msg_arena_buy_times>
{
	public ArenaBuyTimesAction()
		: base((int)MESSAGE_ID.ID_MSG_ARENA_BUYTIMES)
    {

    }

	protected override void OnRequest(request_msg_arena_buy_times request, object userdata)
    {

    }

	protected override void OnRespond(respond_msg_arena_buy_times respond, object userdata)
    {
		ArenaModule module = ModuleManager.Instance.FindModule<ArenaModule>();
		module.SyncBuyTimesData(respond);
    }
}
