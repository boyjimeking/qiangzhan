using System;
using System.Collections.Generic;
using Message;

public class EnterGameAction : BaseAction<request_enter_game, respond_enter_game>
{
    public EnterGameAction()
        : base((int)MESSAGE_ID.ID_MSG_ENTER_GAME)
    {
    }

    protected override void OnRequest(request_enter_game request, object userdata)
    {
        GUID guid = userdata as GUID;
        if (guid == null)
            return;

        request.guid = guid.ToMSGGuid();
        request.openId = PlatformSDK.OpenId;
        request.accesstoken = PlatformSDK.AccessToken;
        request.pf = PlatformSDK.Pf;
        request.pfkey = PlatformSDK.PfKey;
        request.paytoken = PlatformSDK.PayToken;
        request.platform = PlatformSDK.Platform;
        request.regchannel = PlatformSDK.RegChannel;
        request.setupchannel = PlatformSDK.SetupChannel;
        request.clientsystem = PlatformSDK.ClientSystem;
        request.txplat = PlatformSDK.TXPlat;
    }

    protected override void OnRespond(respond_enter_game respond, object userdata)
    {
       if(respond.result == (int)ERROR_CODE.ERR_ENTER_GAME_OK)
       {
           GameDebug.Log("进入游戏成功");

		   PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
		   module.setGUID(respond.guid);
           module.SyncPlayerProperty(respond.prop);
           EnterGameEvent e = new EnterGameEvent(EnterGameEvent.ENTER_GAME);

           EventSystem.Instance.PushEvent(e);

           GameApp.Instance.setNextFlow(GAME_FLOW_ENUM.GAME_FLOW_MAIN);
       }
       else
       {
           GameDebug.Log("进入游戏失败");		
       }
    }
}
