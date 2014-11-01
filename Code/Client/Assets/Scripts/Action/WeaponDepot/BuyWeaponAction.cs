using System;
using System.Collections.Generic;
using Message;

public class BuyWeaponActionParam
{
    public int WeaponResId
    {
        get;
        set;
    }
}

public class BuyWeaponAction : LogicAction<request_buy_weapon, respond_buy_weapon>
{
    public BuyWeaponAction()
        : base((int)MESSAGE_ID.ID_MSG_WD_BUY_WEAPON)
    {

    }

    protected override void OnRequest(request_buy_weapon request, object userdata)
    {
        BuyWeaponActionParam param = userdata as BuyWeaponActionParam;
        if (param == null)
            return;

        request.resid = (uint)param.WeaponResId;
    }

    protected override void OnRespond(respond_buy_weapon respond, object userdata)
    {
        PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
        if (module == null)
            return;

        if(respond.errorcode != (int)Message.ERROR_CODE.ERR_WD_BUY_OK)
        {
            PromptUIManager.Instance.AddNewPrompt("error");
            return;
        }
    }
}
