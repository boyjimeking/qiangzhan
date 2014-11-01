using System;
using System.Collections.Generic;
using Message;

public class QualifyingEndActionParam
{
	public GUID guid;

	public int result;
}

public class QualifyingEndAction : LogicAction<request_msg_qualifying_end, respond_msg_qualifying_end>
{
	public QualifyingEndAction()
		: base((int)MESSAGE_ID.ID_MSG_QUALIFYING_END)
    {

    }

	protected override void OnRequest(request_msg_qualifying_end request, object userdata)
    {
		QualifyingEndActionParam param = userdata as QualifyingEndActionParam;
		if (param == null)
            return;

		request.guid = param.guid.ToMSGGuid();
		request.result = param.result;
    }

	protected override void OnRespond(respond_msg_qualifying_end respond, object userdata)
    {
		QualifyingModule module = ModuleManager.Instance.FindModule<QualifyingModule>();
		module.SyncEndData(respond);
    }
}
