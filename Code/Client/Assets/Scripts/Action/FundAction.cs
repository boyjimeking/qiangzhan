using UnityEngine;
using System.Collections;
using Message;

public class FundJoinParam
{
    public FUND_OP_TYPE opType
    {
        get;
        set;
    }

    public FundJoinParam()
    {

    }
}

public class FundAction : LogicAction<request_fund_op, respond_fund_op>
{
    public FundAction()
        : base((int)MESSAGE_ID.ID_MSG_FUND)
    {

    }

    protected override void OnRequest(request_fund_op param, object userdata)
    {
        if (userdata is FundJoinParam)
        {
            FundJoinParam clickParam = userdata as FundJoinParam;

            param.op_type = (int)clickParam.opType;
        }
    }

    protected override void OnRespond(respond_fund_op respond, object userdata)
    {
        if (respond.result != (int)Message.ERROR_CODE.ERR_FUND_OK)
        {
            switch ((Message.ERROR_CODE)respond.result)
            {
                case ERROR_CODE.ERR_FUND_FAILED:
                    break;
                //case ERROR_CODE.ERR_MALL_BUY_NO_TIMES:
                //    break;
                //case ERROR_CODE.ERR_MALL_FAILED:
                //    break;
                default:
                    break;
            }
            return;
        }

        switch(respond.op_type)
        {
            case (int)Message.FUND_OP_TYPE.FUND_BUY:
                PopTipManager.Instance.AddNewTip(StringHelper.GetString("fund_join_sucess"));
                break;
        }
    }
}
