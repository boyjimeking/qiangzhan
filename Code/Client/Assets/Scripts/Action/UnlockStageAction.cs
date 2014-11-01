using System;
using System.Collections.Generic;
using Message;

public class UnlockStageActionParam
{
	// ¹Ø¿¨id
	public int stageid;
}

public class UnlockStageAction : LogicAction<request_msg_unlockstage, respond_msg_unlockstage>
{
	public UnlockStageAction()
		: base((int)MESSAGE_ID.ID_MSG_SCENE_UNLOCK)
    {

    }

	protected override void OnRequest(request_msg_unlockstage request, object userdata)
    {
		UnlockStageActionParam param = userdata as UnlockStageActionParam;
		if (param == null)
            return;

		request.stage_id = (uint)param.stageid;
    }

	protected override void OnRespond(respond_msg_unlockstage respond, object userdata)
    {
		StageDataManager.Instance.SyncUnlockStage((int)respond.stage_id);
    }
}
