using System;
using System.Collections.Generic;
using Message;

public class ChallengeStageContinueActionParam
{
    public uint Floor
    {
        get;
        set;
    }
}

public class ChallengeStageContinueAction : LogicAction<request_challenge_stage_continue, respond_challenge_stage_continue>
{
    public ChallengeStageContinueAction()
        : base((int)MESSAGE_ID.ID_MSG_CHALLENGE_STAGE_CONTINUE)
    {

    }

    protected override void OnRequest(request_challenge_stage_continue request, object userdata)
    {
        ChallengeStageContinueActionParam param = userdata as ChallengeStageContinueActionParam;
        if (param == null)
            return;

        request.floor = param.Floor;
    }

    protected override void OnRespond(respond_challenge_stage_continue respond, object userdata)
    {
        if (!respond.may)
            return;

        ModuleManager.Instance.FindModule<ChallengeModule>().OnRespondContinue((int)respond.floor);
    }

}
