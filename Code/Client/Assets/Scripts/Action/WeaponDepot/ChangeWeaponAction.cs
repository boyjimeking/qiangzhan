using System;
using System.Collections.Generic;
using Message;

public class ChangeWeaponActionParam
{
    public int WeaponResId
    {
        get;
        set;
    }
    public int SubWeaponResId
    {
        get;
        set;
    }
}

public class ChangeWeaponAction : LogicAction<request_change_weapon, respond_change_weapon>
{
    public ChangeWeaponAction()
        : base((int)MESSAGE_ID.ID_MSG_WD_CHANGE_WEAPON)
    {

    }

    protected override void OnRequest(request_change_weapon request, object userdata)
    {
        ChangeWeaponActionParam param = userdata as ChangeWeaponActionParam;
        if (param == null)
            return;

        request.main_weapon_resid = param.WeaponResId;
        request.sub_weapon_resid = param.SubWeaponResId;
    }

    protected override void OnRespond(respond_change_weapon respond, object userdata)
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        if (respond.errorcode != (int)Message.ERROR_CODE.ERR_WD_CHANGE_WEAPON_OK)
        {
            PromptUIManager.Instance.AddNewPrompt("error");
            return;
        }

        EventSystem.Instance.PushEvent(new ItemEvent(ItemEvent.WEAPON_CHANGE));
    }

}
