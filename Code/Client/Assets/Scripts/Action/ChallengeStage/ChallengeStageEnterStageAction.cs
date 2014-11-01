using System;
using System.Collections.Generic;
using Message;

public class ChallengeStageEnterStageActionParam
{
    public uint Floor
    {
        get;
        set;
    }
}

public class ChallengeStageEnterStageAction : LogicAction<request_challenge_stage_begin, respond_challenge_stage_begin>
{
    public ChallengeStageEnterStageAction()
        : base((int)MESSAGE_ID.ID_MSG_CHALLENGE_STAGE_BEGIN)
    {

    }

    protected override void OnRequest(request_challenge_stage_begin request, object userdata)
    {
        ChallengeStageEnterStageActionParam param = userdata as ChallengeStageEnterStageActionParam;
        if (param == null)
            return;

        request.floor = param.Floor;
    }

    protected override void OnRespond(respond_challenge_stage_begin respond, object userdata)
    {
        if (respond.errorcode != (int)Message.ERROR_CODE.ERR_CHALLENGE_STAGE_ENTER_STAGE_OK)
            return;

        SceneManager.Instance.RequestEnterScene(respond.scene_resid);
    }

}
