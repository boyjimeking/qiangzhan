using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;

public class ChatCacheData
{
    public string name = null;
    public string msg = null;
}
public class ChatModule : ModuleBase
{
    private static int msMaxCacheNum = 50;
	private ArrayList mParamList;//参数列表;

    private Dictionary<int, Queue<ChatCacheData>> mChanelCaches = new Dictionary<int, Queue<ChatCacheData>>();
	public ChatModule()
	{

	}
	protected override void OnEnable()
	{
        EventSystem.Instance.addEventListener(ChatEvent.CHAT_SERVER_RECV_MESSAGE, onChatServerMsg);
	}
	
	protected override void OnDisable()
	{
        EventSystem.Instance.removeEventListener(ChatEvent.CHAT_SERVER_RECV_MESSAGE, onChatServerMsg);		
	}

    private void onChatServerMsg(EventBase e)
    {
         ChatEvent evt = (ChatEvent)e;
         PushMessageCache(evt.channel_type, evt.name, evt.msg);
    }

    private void PushMessageCache(int channel , string name , string msg)
    {
        if (channel < 0 || channel >= (int)ChatChannelType.ChannelType_Max)
        {
            GameDebug.Log("聊天信息频道出现错误 channel = " + channel.ToString());
            return;
        }
        if( !mChanelCaches.ContainsKey(channel) )
        {
            mChanelCaches.Add( channel , new Queue<ChatCacheData>() );
        }
        Queue<ChatCacheData> queue = mChanelCaches[channel];

        if( queue.Count >= msMaxCacheNum )
        {
            queue.Dequeue();
        }

        string chkMsg = StrFilterManager.Instance.CheckAndReplace(msg);


        ChatCacheData data = new ChatCacheData();
        data.name = name;
        data.msg = chkMsg;
        queue.Enqueue(data);


        ChatEvent e = new ChatEvent(ChatEvent.MODULE_TO_UI_MESSAGE_UPDATE);
        e.msg = msg;
        e.name = name;
        e.channel_type = channel;
        EventSystem.Instance.PushEvent(e);
    }

    public ChatCacheData[] GetMessageCache(ChatChannelType type)
    {
        if( !mChanelCaches.ContainsKey((int)type) )
        {
            return null;
        }
        return mChanelCaches[(int)type].ToArray();
    }

    public void SendText(ChatChannelType type , string msg)
	{
        if (msg.Length >= 1 && msg.StartsWith(".") )
        {
            string commend = "";
            commend = msg.Remove(0, 1);
            if (string.IsNullOrEmpty(commend) || string.IsNullOrEmpty(commend))
                return;

            string[] paras = commend.Split(new string[] { " " }, System.StringSplitOptions.None);

            if (paras.Length <= 0)
                return;

            if (mParamList == null)
                mParamList = new ArrayList();
            else
                mParamList.Clear();

            for (int i = 1; i < paras.Length; i++)
                mParamList.Add(paras[i]);

            if(!GMHandler.Instance.DoHandler(PlayerController.Instance.GetControlObj(), paras[0], mParamList))
            {
                GMActionParam param = new GMActionParam();
                param.Cmd = commend;

                Net.Instance.DoAction((int)Message.MESSAGE_ID.ID_MSG_GM, param);
            }
        }
        else
        {
            //系统频道不允许玩家发送
            if( type == ChatChannelType.ChannelType_System )
            {
                return;
            }

            PlayerDataModule module = ModuleManager.Instance.FindModule<PlayerDataModule>();
            if( module == null )
            {
                return ;
            }
                 
            MainFlow mainFlow = GameApp.Instance.GetCurFlow() as MainFlow;
            if( mainFlow != null )
            {
                CSChatMessage packet = new CSChatMessage();
                packet.channel_type = (int)type;
                packet.name = module.GetName();
                packet.msg = msg;

                mainFlow.SendToChatServer(packet);
            }
            Role role = PlayerController.Instance.GetControlObj() as Role;
            if (role != null)
            {
                role.Talk(msg);
            }
        }
	}
	
}
