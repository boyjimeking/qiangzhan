using System;
using System.IO;
using Message;


public class ChallengeStageRankAction : LogicAction<request_challenge_ranking_list, respond_challenge_ranking_list>
{
    public ChallengeStageRankAction()
        : base((int) MESSAGE_ID.ID_MSG_CHALLENGE_RANK)
    {

    }

    protected override void OnRequest(request_challenge_ranking_list request, object userdata)
    {
        int version = Convert.ToInt32(userdata);
        //GameDebug.Log("请求急速挑战数据，当前版本："+version);   
        request.version = version;
      
    }

    protected override void OnRespond(respond_challenge_ranking_list respond, object userdata)
    {
        var data = PlayerDataPool.Instance.MainData.mChallengeStage;
        bool changed = false;
        if (respond.errorcode == (int) ERROR_CODE.ERR_CHALLENGE_RANKING_OK)
        {
            if (respond.version != data.mRankVersion)
            {
                changed = true;
                data.mRankVersion  = respond.version;
                GameDebug.Log("rankversion" + respond.version);
                data.mRankList.Clear();
                for (int i = 0; i < respond.challenge_ranking.Count; i++)
                {
                    GameDebug.Log(respond.challenge_ranking[i].name + ":" + respond.challenge_ranking[i].challenge_weekscore);
                    RankingChallengeInfo temp = new RankingChallengeInfo();
                    temp.challenge_weekscore = respond.challenge_ranking[i].challenge_weekscore;
                    temp.floor = respond.challenge_ranking[i].floor;
                    temp.level = respond.challenge_ranking[i].level;
                    temp.name = respond.challenge_ranking[i].name;
                    temp.resid = respond.challenge_ranking[i].resid;
                    temp.guid = respond.challenge_ranking[i].guid;

                    data.mRankList.Add(temp);
                }
            }

            if (PlayerDataPool.Instance.MainData.mChallengeStage.mRankNum != respond.rankindex)
            {
                PlayerDataPool.Instance.MainData.mChallengeStage.mRankNum = respond.rankindex;
                changed = true;
            }
            GameDebug.Log("respond.rankindex = " + respond.rankindex);

            if (changed)
            {
                GameDebug.Log("CHALLENGE_RANK_UPDATE");
                EventSystem.Instance.PushEvent(new ChallengeEvent(ChallengeEvent.CHALLENGE_RANK_UPDATE));
            }
           
        }
        
    }
}

