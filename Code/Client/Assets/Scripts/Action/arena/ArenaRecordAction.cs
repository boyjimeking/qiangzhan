using System;
using System.Collections.Generic;
using Message;

// 竞技场战报

public class ArenaRecordAction : LogicAction<request_msg_arena_record, respond_msg_arena_record>
{
	public ArenaRecordAction()
		: base((int)MESSAGE_ID.ID_MSG_ARENA_RECORD)
    {

    }

	protected override void OnRequest(request_msg_arena_record request, object userdata)
    {

    }

	protected override void OnRespond(respond_msg_arena_record respond, object userdata)
    {
		ArenaModule module = ModuleManager.Instance.FindModule<ArenaModule>();
		module.SyncRecordData(respond);
    }
}
