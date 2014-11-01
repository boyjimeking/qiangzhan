using System;
using System.Collections.Generic;
using Message;

public class SaleDefenceActionParam
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

public class SaleDefenceAction : LogicAction<request_defence_sale, respond_defence_sale>
{
    public SaleDefenceAction()
        : base((int)MESSAGE_ID.ID_MSG_DEFENCE_SALE)
    {

    }

    protected override void OnRequest(request_defence_sale request, object userdata)
    {
        SaleDefenceActionParam param = userdata as SaleDefenceActionParam;
        if (param == null)
            return;

        request.defenceId = param.DefenceId;
        request.pos = param.pos;
        request.packtype = param.PackType;
    }

    protected override void OnRespond(respond_defence_sale respond, object userdata)
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        if (respond.errorcode != (int)Message.ERROR_CODE.ERR_DEFENCE_DEFENCE_SALE_OK)
        {
            // 进行错误提示
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("sale_defence_failed"));
            return;
        }

        string ss = string.Format(StringHelper.GetString("sale_defence_success"),respond.moneysaled);
        if (respond.starssaled > 0)
            ss += "," + string.Format(StringHelper.GetString("defence_sale_starscout"), respond.starssaled);
        if(respond.stonesaled > 0)
            ss += "," + string.Format(StringHelper.GetString("defence_sale_stonescout"), respond.starssaled);
        ss += ".";
        PopTipManager.Instance.AddNewTip(ss);
        ItemEvent evt = new ItemEvent(ItemEvent.DEFENCE_SALE);
        EventSystem.Instance.PushEvent(evt);

        EventSystem.Instance.PushEvent(new ItemEvent(ItemEvent.UPDATE_CHANGE));
    }

}
