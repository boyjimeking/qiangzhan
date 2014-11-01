using UnityEngine;
using System.Collections;

public class LoadingEvent : EventBase
{
	public static string LOADING_PROGRESS = "LOADING_PROGRESS";

	public int progress;

    public string showname;

	public LoadingEvent(string eventName)
        : base(eventName)
    {

    }
}
