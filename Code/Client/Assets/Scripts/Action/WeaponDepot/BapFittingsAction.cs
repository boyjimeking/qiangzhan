using System;
using System.Collections.Generic;
using Message;

public class BapFittingsActionParam
{
    public int[] mproperty = new int[(int)FittingsType.MAX_PROPERTY];
    public int[] mvalue = new int[(int)FittingsType.MAX_PROPERTY];
    public bool[] mlock = new bool[(int)FittingsType.MAX_PROPERTY];
    public uint[] start = new uint[(int)FittingsType.MAX_PROPERTY];
    public uint[] end = new uint[(int)FittingsType.MAX_PROPERTY];
    public uint pos //Ç¹ÐµÅä¼þID
    {
        get;
        set;
    }

    public uint price
    {
        get;
        set;
    }

    public uint itemId
    {
        get;
        set;
    }

    public int get_mproperty(int index)
    {
        return mproperty[index];
    }

    public bool set_mproperty(int index, int value)
    {
        if (index < 0 || index > (int)FittingsType.MAX_PROPERTY)
            return false;
        mproperty[index] = value;
        return true;
    }

    public int get_mvalue(int index)
    {
        return mvalue[index];
    }

    public bool set_mvalue(int index, int value)
    {
        if (index < 0 || index > (int)FittingsType.MAX_PROPERTY)
            return false;
        mvalue[index] = value;
        return true;
    }

    public bool get_mlock(int index)
    {
        return mlock[index];
    }

    public bool set_mlock(int index, bool value)
    {
        if (index < 0 || index > (int)FittingsType.MAX_PROPERTY)
            return false;
        mlock[index] = value;
        return true;
    }

    public uint get_start(int index)
    {
        return start[index];
    }

    public bool set_start(int index, uint value)
    {
        if (index < 0 || index > (int)FittingsType.MAX_PROPERTY)
            return false;
        start[index] = value;
        return true;
    }

    public uint get_end(int index)
    {
        return end[index];
    }

    public bool set_end(int index, uint value)
    {
        if (index < 0 || index > (int)FittingsType.MAX_PROPERTY)
            return false;
        end[index] = value;
        return true;
    }
}

public class BapFittingsAction : LogicAction<request_bap_fittings, respond_bap_fittings>
{
    public BapFittingsAction()
        : base((int)MESSAGE_ID.ID_MSG_WD_BAP_FITTINGS)
    {

    }

    protected override void OnRequest(request_bap_fittings request, object userdata)
    {
        BapFittingsActionParam param = userdata as BapFittingsActionParam;
        if (param == null)
            return;

        request.pos = param.pos;
        request.price = param.price;
        request.itemId = param.itemId;

        for (int i = 0; i < (int)FittingsType.MAX_PROPERTY; ++i)
        {
            request.mproperty.Add(param.get_mproperty(i));
            request.mvalue.Add(param.get_mvalue(i));
            request.mlock.Add(param.get_mlock(i));
            request.start.Add(param.get_start(i));
            request.end.Add(param.get_end(i));
        }
    }

    protected override void OnRespond(respond_bap_fittings respond, object userdata)
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        if (respond.errorcode != (int)Message.ERROR_CODE.ERR_WD_BAP_FITTINGS_OK)
        {
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("fitting_error"));
            return;
        }

        PopTipManager.Instance.AddNewTip(StringHelper.GetString("fitting_success"));
        WeaponCultivateEvent evt = new WeaponCultivateEvent(WeaponCultivateEvent.FITTING_CHANGE);
        EventSystem.Instance.PushEvent(evt);

        EventSystem.Instance.PushEvent(new ItemEvent(ItemEvent.WEAPON_CHANGE));
    }
}
