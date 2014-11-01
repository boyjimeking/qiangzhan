using System;
using System.Collections.Generic;
using Message;

public class ChallengeStageOverStageActionParam
{
    public uint Floor
    {
        get;
        set;
    }

    public uint ConsumeTime
    {
        get;
        set;
    }
}

public class ChallengeStageOverStageAction : LogicAction<request_challenge_stage_end, respond_challenge_stage_end>
{
    public ChallengeStageOverStageAction()
        : base((int)MESSAGE_ID.ID_MSG_CHALLENGE_STAGE_END)
    {

    }

    protected override void OnRequest(request_challenge_stage_end request, object userdata)
    {
        ChallengeStageOverStageActionParam param = userdata as ChallengeStageOverStageActionParam;
        if (param == null)
            return;

        request.floor = param.Floor;
        request.succeed = true;
        request.time = param.ConsumeTime;
    }

    protected override void OnRespond(respond_challenge_stage_end respond, object userdata)
    {
        ChallengeCompleteParam param = new ChallengeCompleteParam();

        param.isFirstTime = respond.first_day;
        param.mAchieveOne = respond.complete_one;
        param.mAchieveTwo = respond.complete_two;
        param.mAchieveThree = respond.complete_three;
        param.mFloor = (int)respond.floor;
        param.mHistortyScore = (int)respond.historty_score;
        param.mScore = (int)respond.score;

        if(respond.award != null)
        {
            foreach (award_item_info info in respond.award)
            {
                param.mDrops.Add(new DropItemParam(info.resid, info.num));
            }
        }

        WindowManager.Instance.OpenUI("challengeDrop", param);
    }

}
