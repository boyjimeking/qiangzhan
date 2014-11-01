using System;
using Message;

public class ItemSaleActionParam
{
    public int itemcount = -1;
    public int itemid = -1;
    public bool isSaleAll = true;
}

class ItemSaleAction : LogicAction<item_sale_request, item_sale_respond>
{
    private bool isSaleAll = true;
    public ItemSaleAction()
        : base((int)MESSAGE_ID.ID_MSG_ITEM_SALE)
    {
    }

    protected override void OnRequest(item_sale_request request, object userdata)
    {
        if (userdata is ItemSaleActionParam)
         {
             ItemSaleActionParam param = userdata as ItemSaleActionParam;
             request.itemcount = param.itemcount;
             request.itemid = param.itemid;
             isSaleAll = param.isSaleAll;
         }
    }
    protected override void OnRespond(item_sale_respond respond, object userdata)
    {
        if( respond.errorcode == (uint)ERROR_CODE.ERR_SALE_ITEM_FAILED )
        {
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("sale_item_failed"));
            return;
        }
        PopTipManager.Instance.AddNewTip(string.Format(StringHelper.GetString("sale_item_success"),respond.salemoney));

        if (isSaleAll)
        {
            ItemEvent evt = new ItemEvent(ItemEvent.ITEM_SALE_ALL);
            EventSystem.Instance.PushEvent(evt);
        }
        else
        {
            ItemEvent evt = new ItemEvent(ItemEvent.ITEM_SALE_PART);
            EventSystem.Instance.PushEvent(evt);
        }
    }
}

