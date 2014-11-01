using System;
using System.Collections.Generic;
using Message;


public class ChallengeStageSweepStageAction : LogicAction<request_challenge_stage_sweep, respond_challenge_stage_sweep>
{
    public ChallengeStageSweepStageAction()
        : base((int)MESSAGE_ID.ID_MSG_CHALLENGE_STAGE_SWEEP)
    {

    }

    protected override void OnRequest(request_challenge_stage_sweep request, object userdata)
    {
    }

    protected override void OnRespond(respond_challenge_stage_sweep respond, object userdata)
    {

        if(respond.challengeinfo != null)
        {
            ChallengeModule module = ModuleManager.Instance.FindModule<ChallengeModule>();
            module.SetDoingFloor(ModuleManager.Instance.FindModule<PlayerDataModule>().GetChallengeCurrentFloor());
        }

        List<ChallengeSweepParam> awarditems = new List<ChallengeSweepParam>();

        if(respond.awards != null)
        {
            for (int i = 0; i < respond.awards.Count; i++)
            {
                challenge_stage_sweep_floor_award flooraward = respond.awards[i];

                ChallengeSweepParam csp = new ChallengeSweepParam();

                csp.mFloor = (int)flooraward.floor;
                if (flooraward.award != null)
                {
                    for (int j = 0; j < flooraward.award.Count; j++)
                    {
                        csp.mDrops.Add(new DropItemParam(flooraward.award[j].resid, flooraward.award[j].num));
                    }
                }

                awarditems.Add(csp);      
            }
        }

        WindowManager.Instance.OpenUI("sweepDrop", awarditems);
    }

}
