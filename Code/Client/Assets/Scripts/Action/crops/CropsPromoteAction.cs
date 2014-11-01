using System;
using System.Collections.Generic;
using Message;

public class CropsPromoteActionParam
{
    public int cropsid
    {
        get;
        set;
    }
}

public class CropsPromoteAction : LogicAction<request_crops_level, respond_crops_level>
{
    public CropsPromoteAction()
        : base((int)MESSAGE_ID.ID_MSG_CROPS_PROMOTE)
    {

    }

    protected override void OnRequest(request_crops_level request, object userdata)
    {
        CropsPromoteActionParam param = userdata as CropsPromoteActionParam;
        if (param == null)
            return;

        request.resid = param.cropsid;
    }

    protected override void OnRespond(respond_crops_level respond, object userdata)
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        if (respond.errorcode != (int)Message.ERROR_CODE.ERR_CROPS_PROMOTE_OK)
        {
            // 进行错误提示
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("crops_rise_failed"));
            return;
        }

        PopTipManager.Instance.AddNewTip(StringHelper.GetString("crops_rise_ok"));
        CropsEvent evt = new CropsEvent(CropsEvent.RISE_STARS);
        EventSystem.Instance.PushEvent(evt);
        //EventSystem.Instance.PushEvent(new ItemEvent(ItemEvent.UPDATE_CHANGE));
    }

}
