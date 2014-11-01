using System;
using System.Collections.Generic;
using Message;

// 排位赛战报

public class QualifyingRecordAction : LogicAction<request_msg_qualifying_record, respond_msg_qualifying_record>
{
	public QualifyingRecordAction()
		: base((int)MESSAGE_ID.ID_MSG_QUALIFYING_RECORD)
    {

    }

	protected override void OnRequest(request_msg_qualifying_record request, object userdata)
    {

    }

	protected override void OnRespond(respond_msg_qualifying_record respond, object userdata)
    {
		QualifyingModule module = ModuleManager.Instance.FindModule<QualifyingModule>();
		module.SyncRecordData(respond);
    }
}
