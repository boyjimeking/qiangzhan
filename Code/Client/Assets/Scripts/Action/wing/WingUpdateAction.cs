
using System;
using Message;
public class WingUpdateAction:LogicAction<request_update_wing,respond_update_wing>
{
	public WingUpdateAction ()
		: base((int) MESSAGE_ID.ID_MSG_WING_UPDATE)
	{

	}

	protected override void OnRequest (request_update_wing request, object userdata)
	{
		GameDebug.Log("请求更新翅膀数据");
	}

	protected override void OnRespond (respond_update_wing respond, object userdata)
	{
	    if(respond.result == (int)ERROR_CODE.ERR_WING_UPDATE_OK)
		{
			GameDebug.Log("更新翅膀数据");
			EventSystem.Instance.PushEvent(new WingUIEvent(WingUIEvent.Wing_UI_UPDATE));
		}

	}


}
		


