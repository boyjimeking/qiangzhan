using UnityEngine;
using System.Collections.Generic;
using Message;

public class MallBuyItemAction
{
    //商城表resId;
    public int ResId
    {
        get;
        set;
    }

    //哪组商品;
    public int SubIdx
    {
        get;
        set;
    }

    public MallBuyItemAction()
    {
 
    }
}

public class MallAction : LogicAction<request_mall_buy, respond_mall_buy>
{
     public MallAction()
        : base((int)MESSAGE_ID.ID_MSG_MALL_BUY)
    {
    }

    protected override void OnRequest(request_mall_buy request, object userdata)
    {
        if( userdata is MallBuyItemAction )
        {
            MallBuyItemAction param = userdata as MallBuyItemAction;
            request.mallid = param.ResId;
            request.subid = param.SubIdx;
        }
    }
    protected override void OnRespond(respond_mall_buy respond, object userdata)
    {
        MallFormModule module = ModuleManager.Instance.FindModule<MallFormModule>();
        if (module == null)
        {
            GameDebug.LogError("没有找到mallmodule");
            return;
        }

        if (respond.result != (int)Message.ERROR_CODE.ERR_MALL_OK)
        {
            switch ((Message.ERROR_CODE)respond.result)
            { 
                case ERROR_CODE.ERR_MALL_BUY_NO_MONEY:
                    break;
                case ERROR_CODE.ERR_MALL_BUY_NO_TIMES:
                    break;
                case ERROR_CODE.ERR_MALL_FAILED:
                    break;
                default:
                    break;
            }
            return;
        }

        if (userdata is MallBuyItemAction)
        {
            module.BuyMallItem(respond.mallid, respond.subid);
        }
    }
}