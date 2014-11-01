using UnityEngine;
using System.Collections;

public class ChatEvent : EventBase 
{
    public static string MODULE_TO_UI_MESSAGE_UPDATE = "MODULE_TO_UI_MESSAGE_UPDATE";

    public static string CHAT_SERVER_RECV_MESSAGE = "CHAT_SERVER_RECV_MESSAGE";
    public int channel_type = -1;
    public string name = null;
	public string msg = null;
	public ChatEvent(string eventName) :base(eventName)
	{
		
	}
}
