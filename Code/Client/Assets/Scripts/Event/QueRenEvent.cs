using UnityEngine;
using System.Collections;

public class QueRenEvent : EventBase 
{
	public static string CONTENT_CHANGE = "CONTENT_CHANGE";

	public QueRenEvent(string eventName) :base(eventName)
	{

	}
}
