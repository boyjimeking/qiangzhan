using System;
using System.Collections.Generic;
using Message;

public class ZhaoCaiMaoPartnerAction : LogicAction<request_zhaocaimao_partner, respond_zhaocaimao_partner>
{
	public ZhaoCaiMaoPartnerAction()
        : base((int)MESSAGE_ID.ID_MSG_ZHAOCAIMAO_PARTNER)
    {

    }

	protected override void OnRequest(request_zhaocaimao_partner request, object userdata)
    {

    }

    protected override void OnRespond(respond_zhaocaimao_partner respond, object userdata)
    {
        if(respond.simpleroles != null)
        {
			ActivityManager.Instance.PartnerList = respond.simpleroles;
        }
    }

}
