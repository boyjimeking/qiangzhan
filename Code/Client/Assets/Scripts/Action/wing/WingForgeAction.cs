using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Message;

public class WingForgeAction:LogicAction<request_forge_wing,respond_forge_wing>
{
    public WingForgeAction()
        : base((int) MESSAGE_ID.ID_MSG_WING_FORGE)
    {
        
    }

    protected override void OnRequest(request_forge_wing request, object userdata)
    {
        request.wingid = Convert.ToInt32(userdata);
        WingCommonTableItem commonRes = DataManager.WingCommonTable[request.wingid] as WingCommonTableItem;
        if (commonRes == null) return;
        PlayerDataModule pdm = ModuleManager.Instance.FindModule<PlayerDataModule>();
        uint itemNum = pdm.GetItemNumByID(commonRes.costId);
        if (itemNum == 0)
        {
            //GameDebug.Log("打开快速购买界面打开快速购买界面打开快速购买界面打开快速购买界面打开快速购买界面");
            return;
        }
       
        GameDebug.Log("请求精炼翅膀"+request.wingid);
    }

    protected override void OnRespond(respond_forge_wing respond, object userdata)
    {
        if (respond.result == (int) ERROR_CODE.ERR_WING_FORGE_OK)
        {
            GameDebug.Log("精炼成功"+Convert.ToInt32(userdata));
            WingUIEvent evt = new WingUIEvent(WingUIEvent.WING_UI_FORGE);
            evt.wingid = Convert.ToInt32(userdata);
            evt.result = respond.result;
            EventSystem.Instance.PushEvent(evt);
        }
        else if(respond.result==(int)ERROR_CODE.ERR_WING_FORGE_ITEM_NO_ENOUGH)
        {
            GameDebug.Log("道具不足");
        }
        else
        {
            GameDebug.Log("精炼失败"+Convert.ToInt32(userdata));
        }

		
    }
}

