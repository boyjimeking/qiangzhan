using System;
using System.Collections.Generic;
using Message;

public class EnterSceneActionParam
{
	// 场景类型
	public SceneType scenetype;

	// 子类型id
	public int sceneid;
}

public class EnterSceneAction : LogicAction<request_msg_enterstage, respond_msg_enterstage>
{
	public EnterSceneAction()
		: base((int)MESSAGE_ID.ID_MSG_SCENE_ENTER)
    {
    }

	protected override void OnRequest(request_msg_enterstage request, object userdata)
    {
		EnterSceneActionParam param = userdata as EnterSceneActionParam;
		if (param == null)
            return;

		request.scene_type = (uint)param.scenetype;
		request.scene_id = (uint)param.sceneid;
    }

	protected override void OnRespond(respond_msg_enterstage respond, object userdata)
    {
		if(respond.result == (uint)ERROR_CODE.ERR_SCENE_ENTER_OK)
		{
            EnterSceneActionParam param = userdata as EnterSceneActionParam;
            if (param == null)
                return;
		    if (param.scenetype == SceneType.SceneType_City)
		    {
		        SceneManager.Instance.EnterScene((int) respond.scene_id);
		    }
		    else
		    {
                StageDataManager.Instance.CacheServerAward(respond.awards);
                SceneManager.Instance.EnterScene((int)respond.scene_id);
		    }
			
		}
		else
		{
			StageDataManager.Instance.PrintErrorCode((ERROR_CODE)(respond.result));
		}
    }
}
