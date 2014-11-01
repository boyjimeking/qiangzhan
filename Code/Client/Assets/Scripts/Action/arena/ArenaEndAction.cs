using System;
using System.Collections.Generic;
using Message;

public class ArenaEndActionParam
{
	public GUID guid;

	public int result;
}

public class ArenaEndAction : LogicAction<request_msg_arena_end, respond_msg_arena_end>
{
	public ArenaEndAction()
		: base((int)MESSAGE_ID.ID_MSG_ARENA_END)
    {

    }

	protected override void OnRequest(request_msg_arena_end request, object userdata)
    {
		ArenaEndActionParam param = userdata as ArenaEndActionParam;
		if (param == null)
            return;

		request.guid = param.guid.ToMSGGuid();
		request.result = param.result;
    }

	protected override void OnRespond(respond_msg_arena_end respond, object userdata)
    {
		ArenaModule module = ModuleManager.Instance.FindModule<ArenaModule>();
		module.SyncEndData(respond);
    }
}
