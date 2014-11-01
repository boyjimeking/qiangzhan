using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Message;
public class WingActiveAction:LogicAction<request_active_wing,respond_active_wing>
{
    public WingActiveAction()
        : base((int) MESSAGE_ID.ID_MSG_WING_ACTIVE)
    {
        
    }

    protected override void OnRequest(request_active_wing request, object userdata)
    {
        request.wingid = Convert.ToInt32(userdata);
        GameDebug.Log("请求激活翅膀："+request.wingid);
    }

    protected override void OnRespond(respond_active_wing respond , object userdata)
    {
        if (respond.result == (int) ERROR_CODE.ERR_WING_ACTIVE_OK)
        {

            GameDebug.Log("激活成功" + Convert.ToInt32(userdata));
        }
        else
        {
            GameDebug.Log("激活失败" + Convert.ToInt32(userdata));
        }

		WingUIEvent evt = new WingUIEvent(WingUIEvent.WING_UI_ACTIVE);
		evt.wingid = Convert.ToInt32(userdata);
		evt.result = respond.result;
		EventSystem.Instance.PushEvent(evt);
    }
}

