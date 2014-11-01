using System;
using System.Collections.Generic;
using Message;

public class ZoneRewardActionParam
{
    public int zoneid
    {
        get;
        set;
    }
}

public class ZoneRewardAction : LogicAction<request_msg_zone_reward, respond_msg_zone_reward>
{
    public ZoneRewardAction()
        : base((int)MESSAGE_ID.ID_MSG_ZONE_REWARD)
    {

    }

    protected override void OnRequest(request_msg_zone_reward request, object userdata)
    {
        ZoneRewardActionParam param = userdata as ZoneRewardActionParam;
        if (param == null)
            return;

        request.zone_id = param.zoneid;
    }

    protected override void OnRespond(respond_msg_zone_reward respond, object userdata)
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        if (respond.errorcode == (int)Message.ERROR_CODE.ERR_ZONE_REWARD_AGAIN)
        {
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("zone_reward_again"));
            return;
        }
        else if (respond.errorcode == (int)Message.ERROR_CODE.ERR_ZONE_DATA_FAILED)
        {
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("zone_data_err"));
            return;
        }
        else if (respond.errorcode == (int)Message.ERROR_CODE.ERR_ZONE_REWARD_FULL_BAG)
        {
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("bag_full"));
            return;
        }

        PopTipManager.Instance.AddNewTip(StringHelper.GetString("zone_reward_obtain"));
        ZoneRewardEvent evt = new ZoneRewardEvent(ZoneRewardEvent.ZONE_REWARD_OBTAIN);
        EventSystem.Instance.PushEvent(evt);
    }
}
