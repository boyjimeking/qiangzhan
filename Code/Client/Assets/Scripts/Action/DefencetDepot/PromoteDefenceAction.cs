using System;
using System.Collections.Generic;
using Message;

public class PromoteDefenceActionParam
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
}

public class PromoteDefenceAction : LogicAction<request_defence_promote, respond_defence_promote>
{
    public PromoteDefenceAction()
        : base((int)MESSAGE_ID.ID_MSG_DEFENCE_PROMOTE)
    {

    }

    protected override void OnRequest(request_defence_promote request, object userdata)
    {
        PromoteDefenceActionParam param = userdata as PromoteDefenceActionParam;
        if (param == null)
            return;

        request.defenceId = param.DefenceId;
        request.packtype = param.PackType;
        request.pos = param.pos;
    }

    protected override void OnRespond(respond_defence_promote respond, object userdata)
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        if (respond.errorcode != (int)Message.ERROR_CODE.ERR_DEFENCE_PROMOTE_OK)
        {
            // 进行错误提示
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("promote_failed"));
            return;
        }

        PopTipManager.Instance.AddNewTip(StringHelper.GetString("promote_success"));
        ItemEvent evt = new ItemEvent(ItemEvent.DEFENCE_PROMOTE);
        EventSystem.Instance.PushEvent(evt);

        EventSystem.Instance.PushEvent(new ItemEvent(ItemEvent.UPDATE_CHANGE));
    }

}
