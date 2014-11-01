using UnityEngine;
using System.Collections;

public class MainFlow : BaseFlow 
{
    private bool mBackToLogin = false;

    private TcpConnect mTcpConnect = null;

    bool BaseFlow.Init()
    {
        PlayerDataModule dataModule = ModuleManager.Instance.FindModule<PlayerDataModule>();

        if( GameConfig.GuideSceneID > 0 && !dataModule.IsStageHasPassed( GameConfig.GuideSceneID ))
        {
            SceneManager.Instance.RequestEnterScene(GameConfig.GuideSceneID);
        }
        else
        {
            SceneManager.Instance.RequestEnterScene(SceneManager.Instance.GetLastCityResId());
        }

        FightGradeManager.Instance.InitListeners();
		//初始化主流程的部分数据
        

        //临时放到这里
        PlayerDataEvent evt = new PlayerDataEvent(PlayerDataEvent.PLAYER_DATA_CHANGED);
        EventSystem.Instance.PushEvent(evt);

        //临时
        GuideManager.Instance.OnFistGame();

        string[] strList = Environment.ChatServerAddress.Split(new char[] { ':' });

        //进入聊天服务器
        mTcpConnect = new TcpConnect();
        mTcpConnect.Connect(strList[0], int.Parse(strList[1]));

        SettingManager.Instance.InitPlayer(dataModule.GetName());

        return false;
    }
    bool BaseFlow.Term()
    {
        if (mTcpConnect != null)
        {
            mTcpConnect.DisConnect();
        }
        return false;
    }
    //返回到登入界面
    public void BackToLogin()
    {
        Net.Instance.DisConnect();
        SceneManager.Instance.DestroyCurrentScene();
        PlayerController.Instance.Term();
        mBackToLogin = true;
    }

    GAME_FLOW_ENUM BaseFlow.GetFlowEnum()
    {
        return GAME_FLOW_ENUM.GAME_FLOW_MAIN;
    }
    FLOW_EXIT_CODE BaseFlow.Update(uint elapsed)
    {
        if (mBackToLogin)
        {
            mBackToLogin = false;
            Application.LoadLevel(0);
            return FLOW_EXIT_CODE.FLOW_EXIT_CODE_NEXT;
        }

        if (mTcpConnect != null && mTcpConnect.IsConnected())
        {
            mTcpConnect.Update(elapsed);

            MessageHead msg = mTcpConnect.PopMessage();
            if( msg != null )
            {
                OnChatServerMessage(msg);
            }
        }

        return FLOW_EXIT_CODE.FLOW_EXIT_CODE_ERROR;
    }

    public void SendToChatServer(MessageHead msg)
    {
        if( mTcpConnect != null )
        {
            mTcpConnect.SendMesssage(msg);
        }
    }
    private void OnChatServerMessage(MessageHead msg)
    {
        if( msg.msgid == (uint)TCP_MSG_ID.TCP_MSG_SC_CHAT )
        {
            SCChatMessage chatMsg = (SCChatMessage)msg;
            ChatEvent evt = new ChatEvent(ChatEvent.CHAT_SERVER_RECV_MESSAGE);
            evt.channel_type = chatMsg.channel_type;
            evt.name = chatMsg.name;
            evt.msg = chatMsg.msg;
            EventSystem.Instance.PushEvent(evt);
        }
    }

    public void OnBuyGameCoinsNeedLogin(string param)
    {
        BuyGameCoinsNeedLoginEvent e = new BuyGameCoinsNeedLoginEvent();
        EventSystem.Instance.PushEvent(e);
    }

    public void OnBuyGameCoinsRst(string param)
    {
        BuyGameCoinsRstEvent e = new BuyGameCoinsRstEvent();
        e.Param = param;
        EventSystem.Instance.PushEvent(e);
    }
}
