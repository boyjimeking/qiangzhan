using System;
using Message;

public class ItemEquipActionParam
{
    public int bagType = -1;
    public int bagPos = -1;
}

class ItemEquipAction : LogicAction<item_equip_request, item_equip_respond>
{
    public ItemEquipAction()
        : base((int)MESSAGE_ID.ID_MSG_ITEM_EQUIP)
    {
    }

    protected override void OnRequest(item_equip_request request, object userdata)
    {
         if (userdata is ItemEquipActionParam)
         {
             ItemEquipActionParam param = userdata as ItemEquipActionParam;
             request.bagType = param.bagType;
             request.bagPos = param.bagPos;
         }
    }
    protected override void OnRespond(item_equip_respond respond, object userdata)
    {
        if( respond.errorcode == (uint)ERROR_CODE.ERR_EQUIP_ITEM_FAIL )
        {
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("defence_unequip_failed"));
            return;
        }
        WindowManager.Instance.CloseUI("defence");
    }
}

