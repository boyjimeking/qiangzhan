using System;
using System.Collections.Generic;
using Message;

public class StoneInlayActionParam
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

    public int stoneId
    {
        get;
        set;
    }

    public int stonepos
    {
        get;
        set;
    }
}

public class StoneInlayAction : LogicAction<request_stone_inlay, respond_stone_inlay>
{
    public StoneInlayAction()
        : base((int)MESSAGE_ID.ID_MSG_DEFENCE_STONE_INLAY)
    {

    }

    protected override void OnRequest(request_stone_inlay request, object userdata)
    {
        StoneInlayActionParam param = userdata as StoneInlayActionParam;
        if (param == null)
            return;

        request.defenceId = param.DefenceId;
        request.stoneId = param.stoneId;
        request.pos = param.pos;
        request.stonepos = param.stonepos;
        request.packtype = param.PackType;
    }

    protected override void OnRespond(respond_stone_inlay respond, object userdata)
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        if (respond.errorcode != (int)Message.ERROR_CODE.ERR_DEFENCE_STONE_INLAY_OK)
        {
            // 进行错误提示
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("stone_inlay_failed"));
            return;
        }

        PopTipManager.Instance.AddNewTip(StringHelper.GetString("stone_inlay_success"));
        ItemEvent evt = new ItemEvent(ItemEvent.STONE_INLAY);
        EventSystem.Instance.PushEvent(evt);

        EventSystem.Instance.PushEvent(new ItemEvent(ItemEvent.UPDATE_CHANGE));
    }

}
