using UnityEngine;
using System.Collections;
using Message;

public class TotalChargeRewardParam
{
    public TOTALCHARGE_OP_TYPE opType
    {
        get;
        set;
    }

    public int ResId
    {
        get;
        set;
    }

    public TotalChargeRewardParam()
    {

    }
}

public class TotalChargeAction : LogicAction<request_totalcharge_op, respond_totalcharge_op>
{
    public TotalChargeAction()
        : base((int)MESSAGE_ID.ID_MSG_TOTALCHARGE)
    {

    }

    protected override void OnRequest(request_totalcharge_op param, object userdata)
    {
        if (userdata is FundJoinParam)
        {
            FundJoinParam clickParam = userdata as FundJoinParam;

            param.op_type = (int)clickParam.opType;
        }
    }

    protected override void OnRespond(respond_totalcharge_op respond, object userdata)
    {
        if (respond.result != (int)Message.ERROR_CODE.ERR_TOTALCHARGE_REWARD_OK)
        {
            switch ((Message.ERROR_CODE)respond.result)
            {
                case ERROR_CODE.ERR_TOTALCHARGE_REWARD_FAILED:
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

        switch (respond.op_type)
        {
            case (int)Message.TOTALCHARGE_OP_TYPE.TOTALCHARGE_GET_REWARD:
                PopTipManager.Instance.AddNewTip(StringHelper.GetString("totalcharge_reward_ok", FontColor.Red));
                break;
        }
    }
}
