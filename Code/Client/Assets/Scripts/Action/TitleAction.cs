using UnityEngine;
using System.Collections;
using Message;


public class TitlePutAction
{
    //操作类型op_type;
    public int OpType
    {
        get;
        set;
    }

    //称号id;
    public int ResId
    {
        get;
        set;
    }

    public TitlePutAction()
    {

    }
}

public class TitleAction : LogicAction<request_title_op , respond_title_op>
{
    public TitleAction()
        : base((int)MESSAGE_ID.ID_MSG_TITLE)
    {
    }


    protected override void OnRequest(request_title_op request, object userdata)
    {
        if (userdata is TitlePutAction)
        {
            TitlePutAction param = userdata as TitlePutAction;
            request.op_type = param.OpType;
            request.title_id = param.ResId;
        }
    }

    protected override void OnRespond(respond_title_op respond, object userdata)
    {
        if (respond.result != (int)Message.ERROR_CODE.ERR_SHOP_OK)
        {
            switch ((Message.ERROR_CODE)respond.result)
            {
                case ERROR_CODE.ERR_TITLE_FAILED:
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

        if (userdata is TitlePutAction)
        {
            TitlePutAction tpa = userdata as TitlePutAction;
            switch((Message.TITLE_OP_TYPE)tpa.OpType)
            {
                //卸下称号;
                case Message.TITLE_OP_TYPE.TITLE_PICK_OFF:
                    
                    break;
                //穿上称号;
                case Message.TITLE_OP_TYPE.TITLE_PICK_UP:
                    break;
                case TITLE_OP_TYPE.TITLE_GET:
                    break;
                case TITLE_OP_TYPE.TITLE_LOSE:
                    break;
            }
        }

        //改到人物shop物品属性改变后再刷新;
        //if (userdata is ShopRefreshAction)
        //{
        //    module.RefreshShop();
        //}

        //if (userdata is ShopFreeRefreshAction)
        //{
        //    module.RefreshShop();
        //}
    }
}
