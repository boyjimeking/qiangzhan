using System;
using System.Collections.Generic;
using Message;

public class JoinActivityActionParam
{
    public int ActivityId
    {
        get;
        set;
    }
}

public class JoinActivityAction : LogicAction<request_join_activity, respond_join_activity>
{
    public JoinActivityAction()
        : base((int)MESSAGE_ID.ID_MSG_ACTIVITY_JOIN)
    {

    }

    protected override void OnRequest(request_join_activity request, object userdata)
    {
        JoinActivityActionParam param = userdata as JoinActivityActionParam;
        if (param == null)
            return;

        request.resid = param.ActivityId;
    }

    protected override void OnRespond(respond_join_activity respond, object userdata)
    {
        if(respond.errorcode == (uint)Message.ERROR_CODE.ERR_ACTIVITY_JOIN_OK)
        {
			ActivityManager.Instance.Param = new SceneActivityParam(respond.start_time, respond.over_time, respond.scene_resid);

            SceneManager.Instance.RequestEnterScene(respond.scene_resid);
        }
        else
        {

        }
    }

}
