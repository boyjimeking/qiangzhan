using System;
using System.Collections.Generic;
using Message;

// 排位赛对手列表

public class QualifyingListAction : LogicAction<request_msg_qualifying_list, respond_msg_qualifying_list>
{
	public QualifyingListAction()
		: base((int)MESSAGE_ID.ID_MSG_QUALIFYING_LIST)
    {

    }

	protected override void OnRequest(request_msg_qualifying_list request, object userdata)
    {
		
    }

	protected override void OnRespond(respond_msg_qualifying_list respond, object userdata)
    {
		QualifyingModule module = ModuleManager.Instance.FindModule<QualifyingModule>();
		module.SyncListData(respond);
    }
}
