using System;
using ProtoBuff.Serialization;
using Message;

public class QueryRechargeResultActionParam
{
    public string OpenId;
    public string Platform;
    public string AccessToken;
    public string PayToken;
    public string Pf;
    public string PfKey;
}

public class QueryRechargeResultAction : LogicAction<request_recharge_results, respond_recharge_results>
{
    public QueryRechargeResultAction()
        : base((int)Message.MESSAGE_ID.ID_MSG_QUERY_RECHARGE_RESULT)
    {
       
    }

    protected override void OnRequest(request_recharge_results request, object userdata)
    {
        QueryRechargeResultActionParam param = userdata as QueryRechargeResultActionParam;
        if (param == null)
            return;

        request.openid = param.OpenId;
        request.platform = param.Platform;
        request.accesstoken = param.AccessToken;
        request.paytoken = param.PayToken;
        request.pf = param.Pf;
        request.pfkey = param.PfKey;
    }

    protected override void OnRespond(respond_recharge_results respond, object userdata)
    {

    }
}
