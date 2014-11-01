using System;
using System.Collections.Generic;
using Message;

public class QualifyingBeginActionParam
{
	public GUID guid; 
}

public class QualifyingBeginAction : LogicAction<request_msg_qualifying_begin, respond_msg_qualifying_begin>
{
	public QualifyingBeginAction()
		: base((int)MESSAGE_ID.ID_MSG_QUALIFYING_BEGIN)
    {

    }

	protected override void OnRequest(request_msg_qualifying_begin request, object userdata)
    {
		QualifyingBeginActionParam param = userdata as QualifyingBeginActionParam;
		if (param == null)
            return;

		request.guid = param.guid.ToMSGGuid();
    }

	protected override void OnRespond(respond_msg_qualifying_begin respond, object userdata)
    {
		QualifyingModule module = ModuleManager.Instance.FindModule<QualifyingModule>();
		module.SyncBeginData(respond);
    }
}
