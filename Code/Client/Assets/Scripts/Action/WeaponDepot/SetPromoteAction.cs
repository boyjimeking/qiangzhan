using System;
using System.Collections.Generic;
using Message;

public class SetPromoteActionParam
{
    public int WeaponResId
    {
        get;
        set;
    }

    public int WeaponPos
    {
        get;
        set;
    }
}

public class SetPromoteAction : LogicAction<request_set_promote, respond_set_promote>
{
    public SetPromoteAction()
        : base((int)MESSAGE_ID.ID_MSG_WD_SET_PROMOTE)
    {

    }

    protected override void OnRequest(request_set_promote request, object userdata)
    {
        SetPromoteActionParam param = userdata as SetPromoteActionParam;
        if (param == null)
            return;

        request.weapon_resid = param.WeaponResId;
        request.weapon_pos = param.WeaponPos;
    }

    protected override void OnRespond(respond_set_promote respond, object userdata)
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        if (respond.errorcode != (int)Message.ERROR_CODE.ERR_WD_SET_PROMOTE_OK)
        {
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("promote_failed"));
            return;
        }

        PopTipManager.Instance.AddNewTip(StringHelper.GetString("promote_success"));
        ItemEvent evt = new ItemEvent(ItemEvent.WEAPON_PROMOTE);
        EventSystem.Instance.PushEvent(evt);
        EventSystem.Instance.PushEvent(new ItemEvent(ItemEvent.WEAPON_CHANGE));
    }
}
