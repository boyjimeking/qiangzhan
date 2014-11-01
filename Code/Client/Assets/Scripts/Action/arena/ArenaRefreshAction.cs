using System;
using System.Collections.Generic;
using Message;

// 竞技场换一批对手

public class ArenaRefreshAction : LogicAction<request_msg_arena_refresh, respond_msg_arena_refresh>
{
	public ArenaRefreshAction()
		: base((int)MESSAGE_ID.ID_MSG_ARENA_REFRESH)
    {

    }

	protected override void OnRequest(request_msg_arena_refresh request, object userdata)
    {
		
    }

	protected override void OnRespond(respond_msg_arena_refresh respond, object userdata)
    {
		ArenaModule module = ModuleManager.Instance.FindModule<ArenaModule>();
		module.SyncFighterData(respond);
    }
}
