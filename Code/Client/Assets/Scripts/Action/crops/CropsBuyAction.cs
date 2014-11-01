using System;
using System.Collections.Generic;
using Message;

public class CropsBuyActionParam
{
    public int cropsid
    {
        get;
        set;
    }
}

public class CropsBuyAction : LogicAction<request_crops_buy, respond_crops_buy>
{
    public CropsBuyAction()
        : base((int)MESSAGE_ID.ID_MSG_CROPS_BUY)
    {

    }

    protected override void OnRequest(request_crops_buy request, object userdata)
    {
        CropsBuyActionParam param = userdata as CropsBuyActionParam;
        if (param == null)
            return;

        request.resid = param.cropsid;
    }

    protected override void OnRespond(respond_crops_buy respond, object userdata)
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        if (respond.errorcode != (int)Message.ERROR_CODE.ERR_CROPS_BUY_OK)
        {
            // 进行错误提示
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("crops_buy_failed"));
            return;
        }

        PopTipManager.Instance.AddNewTip(StringHelper.GetString("crops_buy_ok"));
        CropsEvent evt = new CropsEvent(CropsEvent.BUY_CROPS);
        EventSystem.Instance.PushEvent(evt);
    }

}
