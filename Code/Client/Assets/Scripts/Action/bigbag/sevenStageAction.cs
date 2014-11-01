using System;
using Message;

public class seven_stateparam
{
    public int week = -1;
    public int type = -1;
}
class SevenStageAction : LogicAction<request_seven_state, respond_seven_state>
{
    public SevenStageAction()
        : base((int)MESSAGE_ID.ID_MSG_SEVEN_AWARD_STAGE)
    {
    }

    protected override void OnRequest(request_seven_state request, object userdata)
    {
        if (userdata is seven_stateparam)
        {
            seven_stateparam param = userdata as seven_stateparam;
            request.week = param.week;
            request.type = param.type;
        }
    }
    protected override void OnRespond(respond_seven_state respond, object userdata)
    {
        if (BigBagModle.MIN_DATA_NUM > respond.week || BigBagModle.MAX_DATA_NUM < respond.week)
           return ;

       PlayerData data = PlayerDataPool.Instance.MainData;
       if (data == null)
           return;

       data.mStateData.updateState(respond.week, (BigBagModle.BUTTON_STATE)respond.result);

       EventSystem.Instance.PushEvent(new BigBagEvent());     
    }
}

