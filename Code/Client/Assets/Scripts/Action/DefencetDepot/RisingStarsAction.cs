using System;
using System.Collections.Generic;
using Message;

public class RisingStarsActionParam
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

    public bool riseten
    {
        get;
        set;
    }
}

public class RisingStarsAction : LogicAction<request_rising_stars, respond_rising_stars>
{
    public RisingStarsAction()
        : base((int)MESSAGE_ID.ID_MSG_DEFENCE_RISING_STAR)
    {

    }

    protected override void OnRequest(request_rising_stars request, object userdata)
    {
        RisingStarsActionParam param = userdata as RisingStarsActionParam;
        if (param == null)
            return;

        request.defenceId = param.DefenceId;
        request.packtype = param.PackType;
        request.pos = param.pos;
        if (param.riseten)
            request.isstarsten = param.riseten;
    }

    protected override void OnRespond(respond_rising_stars respond, object userdata)
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        ItemEvent evt = new ItemEvent(ItemEvent.DEFENCE_RISING_STARS);
        if (respond.errorcode != (int)Message.ERROR_CODE.ERR_DEFENCE_RISING_STARS_OK)
        {
            // 进行错误提示
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("rising_failed"));
            EventSystem.Instance.PushEvent(evt);
            return;
        }
        else if (respond.errorstars == (int)Message.ERROR_CODE.ERR_DEFENCE_RISING_STARS_FAILED)
        {
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("rising_failed"));
            EventSystem.Instance.PushEvent(evt);
            return;
        }

        PopTipManager.Instance.AddNewTip(StringHelper.GetString("rising_success"));
        
        EventSystem.Instance.PushEvent(evt);
        EventSystem.Instance.PushEvent(new ItemEvent(ItemEvent.UPDATE_CHANGE));
    }

}
