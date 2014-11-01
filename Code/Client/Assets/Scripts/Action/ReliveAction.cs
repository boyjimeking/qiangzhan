using System;
using System.Collections.Generic;
using Message;

public class ReliveActionParam
{
	// 场景类型
	public SceneType scenetype;

	// 子类型id
	public int sceneid;

	// 复活方式
	public int relivetype;
}

public class ReliveAction : LogicAction<request_msg_relive, respond_msg_relive>
{
	public ReliveAction()
		: base((int)MESSAGE_ID.ID_MSG_SCENE_RELIVE)
    {

    }

	protected override void OnRequest(request_msg_relive request, object userdata)
    {
		ReliveActionParam param = userdata as ReliveActionParam;
		if (param == null)
            return;

		request.scene_type = (uint)param.scenetype;
		request.scene_id = (uint)param.sceneid;
		request.relive_type = (uint)param.relivetype;
    }

	protected override void OnRespond(respond_msg_relive respond, object userdata)
    {
		if (respond.result == (uint)ERROR_CODE.ERR_SCENE_RELIVE_OK)
		{
			EventSystem.Instance.PushEvent(new StageReliveEvent(StageReliveEvent.STAGE_RELIVE_RESPOND, (ReliveType)respond.relive_type));
		}
		else
		{
			GameDebug.LogWarning("请求复活失败：" + respond.result);
		}
    }
}
