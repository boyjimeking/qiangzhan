using System;
using System.Collections.Generic;
using Message;

public class StrenDefenceActionParam
{
    public int DefenceId
    {
        get;
        set;
    }

    public int PackType
    {
        get;
        set;
    }

    public int pos
    {
        get;
        set;
    }

    public bool strenten
    {
        get;
        set;
    }
}

public class StrenDefenceAction : LogicAction<request_defence_stren, respond_defence_stren>
{
    public StrenDefenceAction()
        : base((int)MESSAGE_ID.ID_MSG_DEFENCE_STEN)
    {

    }

    protected override void OnRequest(request_defence_stren request, object userdata)
    {
        StrenDefenceActionParam param = userdata as StrenDefenceActionParam;
        if (param == null)
            return;

        request.defenceId = param.DefenceId;
        request.packtype = param.PackType;
        request.pos = param.pos;
        if (param.strenten)
            request.isstrenten = param.strenten;
    }

    protected override void OnRespond(respond_defence_stren respond, object userdata)
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        ItemEvent evt = new ItemEvent(ItemEvent.DEFENCE_STREN);
        if (respond.errorcode != (int)Message.ERROR_CODE.ERR_DEFENCE_STREN_OK)
        {
            // 进行错误提示
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("stren_failed"));

            EventSystem.Instance.PushEvent(evt);
            return;
        }
        else if (respond.errorstren == (int)Message.ERROR_CODE.ERR_DEFENCE_STREN_FAILED)
        {
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("stren_failed"));
            EventSystem.Instance.PushEvent(evt);
            return;
        }

        PopTipManager.Instance.AddNewTip(StringHelper.GetString("stren_success"));
        EventSystem.Instance.PushEvent(evt);

        EventSystem.Instance.PushEvent(new ItemEvent(ItemEvent.UPDATE_CHANGE));
    }

}
