using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Message;

public class WingEquipActionParam
{
	public	int wingid;
	// 0装备 1 卸载
	public int action;
}
public class WingEquipAction:LogicAction<request_equip_wing,respond_equip_wing>
{
    public WingEquipAction()
        : base((int) MESSAGE_ID.ID_MSG_WING_EQUIP)
    {
        
    }

    protected override void OnRequest(request_equip_wing request, object userdata)
    {
		WingEquipActionParam param = userdata as WingEquipActionParam;
		request.wingid = param.wingid;
		request.action = param.action;
		if(param.action == 0)
		{
			GameDebug.Log("请求装备翅膀" + request.wingid);
		}else 
		{
			GameDebug.Log("请求卸载翅膀" + request.wingid);
		}

       
    }

    protected override void OnRespond(respond_equip_wing respond, object userdata)
    {
		WingEquipActionParam param = userdata as WingEquipActionParam;
		if(param.action == 0)
		{
			if (respond.result == (int) ERROR_CODE.ERR_WING_EQUIP_OK)
			{
				Player ply = PlayerController.Instance.GetControlObj() as Player;
				if(ply != null)
				{
					ply.WingEquip(param.wingid, param.action);
				}

				GameDebug.Log("装备成功" + param.wingid);
				WingUIEvent evt = new WingUIEvent(WingUIEvent.WING_UI_EQUIP);
				evt.wingid = param.wingid;
				evt.result = respond.result;
				evt.action = param.action;
				EventSystem.Instance.PushEvent(evt);
			}
			else
			{
				GameDebug.Log("装备失败" + param.wingid);
			}

		}else
		{
			if(respond.result ==(int)ERROR_CODE.ERR_WING_TAKEOFF_OK)
			{
				GameDebug.Log("卸载成功" + param.wingid);
				Player ply = PlayerController.Instance.GetControlObj () as Player;
				if(ply != null)
				{
					ply.WingEquip(param.wingid, param.action);
				}

				WingUIEvent evt = new WingUIEvent(WingUIEvent.WING_UI_EQUIP);
				evt.wingid = param.wingid;
				evt.result = respond.result;
				evt.action = param.action;
				EventSystem.Instance.PushEvent(evt);
			}
			else
			{
				GameDebug.Log("卸载失败"+ param.wingid);
			}
		}
       


    }
}

