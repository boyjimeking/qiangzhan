using System;
using Message;

class RankingActionParam
{
    public int version = 0;
}

class RankingAction : LogicAction<request_ranking_list, respond_ranking_list>
{
    public RankingAction()
        : base((int)MESSAGE_ID.ID_MSG_RANKING)
    {
    }

    protected override void OnRequest(request_ranking_list request, object userdata)
    {
        if( userdata is RankingActionParam )
        {
            RankingActionParam param = (RankingActionParam)userdata;
            request.version = param.version;
        }
    }
    protected override void OnRespond(respond_ranking_list respond, object userdata)
    {
        if (respond.errorcode == (uint)ERROR_CODE.ERR_RANKING_FAILED)
        {
            return;
        }
        RankingModule module = ModuleManager.Instance.FindModule<RankingModule>();
        module.SyncRankingList(respond);
    }
}

