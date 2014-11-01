using System;
using System.Collections.Generic;
using Message;

public class StrenWeaponActionParam
{
    public uint StrenLv
    {
        get;
        set;
    }

    public uint money_stren_cost
    {
        get;
        set;
    }
}

public class StrenWeaponAction : LogicAction<request_stren_weapon, respond_stren_weapon>
{
    public StrenWeaponAction()
        : base((int)MESSAGE_ID.ID_MSG_WD_STREN_WEAPON)
    {

    }

    protected override void OnRequest(request_stren_weapon request, object userdata)
    {
        StrenWeaponActionParam param = userdata as StrenWeaponActionParam;
        if (param == null)
            return;

        request.money_stren_cost = param.money_stren_cost;
    }

    protected override void OnRespond(respond_stren_weapon respond, object userdata)
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        if (respond.errorcode != (int)Message.ERROR_CODE.ERR_WD_STREN_WEAPON_OK)
        {
            // 进行错误提示
            PopTipManager.Instance.AddNewTip(StringHelper.GetString("stren_failed"));
            return;
        }

        PopTipManager.Instance.AddNewTip(StringHelper.GetString("stren_success"));
        WeaponCultivateEvent evt = new WeaponCultivateEvent(WeaponCultivateEvent.STRENGTH_CHANGE);
        EventSystem.Instance.PushEvent(evt);

        EventSystem.Instance.PushEvent(new ItemEvent(ItemEvent.WEAPON_CHANGE));
    }

}
