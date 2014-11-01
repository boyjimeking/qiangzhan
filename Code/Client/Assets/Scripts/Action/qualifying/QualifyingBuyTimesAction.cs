using System;
using System.Collections.Generic;
using Message;

public class QualifyingBuyTimesAction : LogicAction<request_msg_qualifying_buy_times, respond_msg_qualifying_buy_times>
{
	public QualifyingBuyTimesAction()
		: base((int)MESSAGE_ID.ID_MSG_QUALIFYING_BUYTIMES)
    {

    }

	protected override void OnRequest(request_msg_qualifying_buy_times request, object userdata)
    {

    }

	protected override void OnRespond(respond_msg_qualifying_buy_times respond, object userdata)
    {
		QualifyingModule module = ModuleManager.Instance.FindModule<QualifyingModule>();
		module.SyncBuyTimesData(respond);
    }
}
