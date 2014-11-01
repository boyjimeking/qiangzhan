using UnityEngine;
using System.Collections.Generic;
using Message;

public class ShopBuyItemAction
{
    //操作类型op_type;
    public int OpType
    {
        get;
        set;
    }

    //商店表resId;
    public int ResId
    {
        get;
        set;
    }

    public ShopBuyItemAction()
    {

    }
}

public class ShopRefreshAction
{
    //操作类型op_type;
    public int OpType
    {
        get;
        set;
    }

    public ShopRefreshAction()
    {

    }
}

public class ShopFreeRefreshAction
{
    //操作类型op_type;
    public int OpType
    {
        get;
        set;
    }

    public ShopFreeRefreshAction()
    {

    }
}

public class ShopAction : LogicAction<request_shop_op, respond_shop_op>
{
    public ShopAction()
        : base((int)MESSAGE_ID.ID_MSG_SHOP)
    {
    }

    protected override void OnRequest(request_shop_op request, object userdata)
    {
        if (userdata is ShopBuyItemAction)
        {
            ShopBuyItemAction param = userdata as ShopBuyItemAction;
            request.op_type = param.OpType;
            request.shopid = param.ResId;
        }

        if (userdata is ShopRefreshAction)
        {
            ShopRefreshAction param = userdata as ShopRefreshAction;
            request.op_type = param.OpType;
        }

        if (userdata is ShopFreeRefreshAction)
        {
            ShopFreeRefreshAction param = userdata as ShopFreeRefreshAction;
            request.op_type = param.OpType;
        }
    }
    protected override void OnRespond(respond_shop_op respond, object userdata)
    {
        ShopModule module = ModuleManager.Instance.FindModule<ShopModule>();
        if (module == null)
        {
            GameDebug.LogError("没有找到shopmodule");
            return;
        }

        if (respond.result != (int)Message.ERROR_CODE.ERR_SHOP_OK)
        {
            switch ((Message.ERROR_CODE)respond.result)
            {
                case ERROR_CODE.ERR_SHOP_FAILED:
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

        if (userdata is ShopBuyItemAction)
        {
            module.BuyShopItem(respond.shopid);
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
