using System;
using System.Collections.Generic;
using Message;

public class StoneCombActionParam
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

    public bool isequiped
    {
        get;
        set;
    }
}

public class StoneCombAction : LogicAction<request_stone_comb, respond_stone_comb>
{
    public StoneCombAction()
        : base((int)MESSAGE_ID.ID_MSG_DEFENCE_STONE_COMB)
    {

    }

    protected override void OnRequest(request_stone_comb request, object userdata)
    {
        StoneCombActionParam param = userdata as StoneCombActionParam;
        if (param == null)
            return;

        request.defenceId = param.DefenceId;
        request.stoneId = param.stoneId;
        request.pos = param.pos;
        request.stonepos = param.stonepos;
        request.packtype = param.PackType;
        request.isequiped = param.isequiped;
    }

    protected override void OnRespond(respond_stone_comb respond, object userdata)
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        if (respond.errorcode != (int)Message.ERROR_CODE.ERR_DEFENCE_STONE_COMB_OK && respond.errorcode != (int)Message.ERROR_CODE.ERR_DEFENCE_STONE_NOT_EQUIPED_COMB_OK)
        {
            // 进行错误提示
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("stone_comb_failed"));
            return;
        }

        PopTipManager.Instance.AddNewTip(StringHelper.GetString("stone_comb_success"));
        if (respond.errorcode == (int)Message.ERROR_CODE.ERR_DEFENCE_STONE_COMB_OK)
        {
            ItemEvent evt = new ItemEvent(ItemEvent.STONE_COMB);
            EventSystem.Instance.PushEvent(evt);
            EventSystem.Instance.PushEvent(new ItemEvent(ItemEvent.UPDATE_CHANGE));
        }
        else if (respond.errorcode == (int)Message.ERROR_CODE.ERR_DEFENCE_STONE_NOT_EQUIPED_COMB_OK)
        {
            //从背包点击宝石进入宝石升级
            ItemEvent evt = new ItemEvent(ItemEvent.STONE_RISE);
            EventSystem.Instance.PushEvent(evt);
        }
    }

}
