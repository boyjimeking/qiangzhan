using System;
using Message;

public class planParam
{
    public int planid
    {
        get;
        set;
    }
}

class PlanStateAction : LogicAction<request_player_plan_report, respond_player_plan_report>
{
    public PlanStateAction()
        : base((int)MESSAGE_ID.ID_MSG_PLAYER_PLAN_STAGE)
    {
    }
    protected override void OnRequest(request_player_plan_report request, object userdata)
    {
        planParam param = userdata as planParam;
        request.planid = param.planid;
    }
    protected override void OnRespond(respond_player_plan_report respond, object userdata)
    {
        if (PlayerPlanModule.MIN_PLAN_NUM > respond.planid || PlayerPlanModule.MAX_PLAN_NUM < respond.planid)
            return;

        PlayerData data = PlayerDataPool.Instance.MainData;
        if (data == null)
            return;

        planData plan = new planData();
        plan.planid = respond.planid;
        plan.state = (PlayerPlanModule.BUTTON_STATE)respond.state;
        plan.jewel = respond.jewel;

        data.mPlanData.UpStateData(plan.planid,plan);

        EventSystem.Instance.PushEvent(new PlanyerPlan());
    }
}

