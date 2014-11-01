using System;
using System.Collections.Generic;
using Message;

public class CropsChangeActionParam
{
    public int main_crops_resid
    {
        get;
        set;
    }

    public int sub_crops_resid
    {
        get;
        set;
    }
}

public class CropsChangeAction : LogicAction<request_change_crops, respond_change_crops>
{
    public CropsChangeAction()
        : base((int)MESSAGE_ID.ID_MSG_CROPS_CHANGE)
    {

    }

    protected override void OnRequest(request_change_crops request, object userdata)
    {
        CropsChangeActionParam param = userdata as CropsChangeActionParam;
        if (param == null)
            return;

        request.main_crops_resid = param.main_crops_resid;
        request.sub_crops_resid = param.sub_crops_resid;
    }

    protected override void OnRespond(respond_change_crops respond, object userdata)
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        if (respond.errorcode != (int)Message.ERROR_CODE.ERR_CROPS_CHANGE_OK)
        {
            // 进行错误提示
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("crops_change_failed"));
            return;
        }

        PopTipManager.Instance.AddNewTip(StringHelper.GetString("crops_change_ok"));
        CropsEvent evt = new CropsEvent(CropsEvent.CHANGE_CROPS);
        EventSystem.Instance.PushEvent(evt);
        //EventSystem.Instance.PushEvent(new ItemEvent(ItemEvent.UPDATE_CHANGE));
    }

}
