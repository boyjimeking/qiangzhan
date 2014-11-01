using System;
using System.Collections.Generic;
using Message;
public class FirstChargeRewardAction : LogicAction<request_firstcharge_reward, respond_firstcharge_reward>
{
    public FirstChargeRewardAction()
        : base((int)MESSAGE_ID.ID_MSG_FIRST_CHARGE_REWARD)
    {

    }

    protected override void OnRequest(request_firstcharge_reward request, object userdata)
    {

    }
    protected override void OnRespond(respond_firstcharge_reward respond, object userdata)
    {

    }
}