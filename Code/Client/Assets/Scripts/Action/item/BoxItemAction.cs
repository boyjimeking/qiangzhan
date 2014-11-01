using System;
using Message;

public class BoxItemActionParam
{
    public int op_type = -1;
    public int bagType = -1;
    public int bagPos = -1;
}

class BoxItemAction : LogicAction<box_item_request, box_item_respond>
{
    public BoxItemAction()
        : base((int)MESSAGE_ID.ID_MSG_BOX_ITEM)
    {
    }

    protected override void OnRequest(box_item_request request, object userdata)
    {
        if (userdata is BoxItemActionParam)
        {
            BoxItemActionParam param = userdata as BoxItemActionParam;
            request.op_type = param.op_type;
            request.bagtype = param.bagType;
            request.bagpos = param.bagPos;
        }
    }
    protected override void OnRespond(box_item_respond respond, object userdata)
    {
        if (respond.errorcode == (uint)ERROR_CODE.ERR_OPEN_BOX_ITEM_FAILDD)
        {
            return;
        }

        WindowManager.Instance.CloseUI("iteminfo");

        if( respond.award_items != null && respond.award_items.Count > 0 )
        {
            for( int i = 0 ; i < respond.award_items.Count ; ++i )
            {
                string content = StringHelper.GetString("egg_get_item") + ItemManager.getItemNameWithColor(respond.award_items[i].resid) + " X " + respond.award_items[i].num;
                PopTipManager.Instance.AddNewTip(content);
            }  
        }

    }
}

