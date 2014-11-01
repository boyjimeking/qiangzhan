using System;
using Message;

public class BagOpActionParam
{
    public int op_type = -1;
    public int bagType = -1;
}

class BagOpAction : LogicAction<bag_op_request, bag_op_respond>
{
    public BagOpAction()
        : base((int)MESSAGE_ID.ID_MSG_BAG_OPERATION)
    {
    }

    protected override void OnRequest(bag_op_request request, object userdata)
    {
        if (userdata is BagOpActionParam)
        {
            BagOpActionParam param = userdata as BagOpActionParam;
            request.op_type = param.op_type;
            request.bagType = param.bagType;
        }
    }
    protected override void OnRespond(bag_op_respond respond, object userdata)
    {
        if (respond.errorcode == (uint)ERROR_CODE.ERR_BAG_OP_FAIL)
        {
            return;
        }

        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();

        if( respond.op_type == (int)BAG_OP_TYPE.BAG_OP_TYPE_UNLOCK )
        {
            PopTipManager.Instance.AddNewTip(string.Format(StringHelper.GetString("item_bag_add_success")));
            module.GetPackManager().SyncPackInfo((PackageType)respond.bagType, respond.vaild_number, respond.max_number);

            ItemEvent ev = new ItemEvent(ItemEvent.BAG_OP_UNLOCK);
            EventSystem.Instance.PushEvent(ev);
        }
    }
}

