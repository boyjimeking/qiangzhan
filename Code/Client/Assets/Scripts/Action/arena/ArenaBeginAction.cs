using System;
using System.Collections.Generic;
using Message;

public class ArenaBeginActionParam
{
	public GUID guid; 
}

public class ArenaBeginAction : LogicAction<request_msg_arena_begin, respond_msg_arena_begin>
{
	public ArenaBeginAction()
		: base((int)MESSAGE_ID.ID_MSG_ARENA_BEGIN)
    {

    }

	protected override void OnRequest(request_msg_arena_begin request, object userdata)
    {
		ArenaBeginActionParam param = userdata as ArenaBeginActionParam;
		if (param == null)
            return;

		request.guid = param.guid.ToMSGGuid();
    }

	protected override void OnRespond(respond_msg_arena_begin respond, object userdata)
    {
		ArenaModule module = ModuleManager.Instance.FindModule<ArenaModule>();
		module.SyncBeginData(respond);
    }
}
