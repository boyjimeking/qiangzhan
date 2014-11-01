using UnityEngine;
using System.Collections;

public class EventBase 
{
    public EventBase next = null;

    public string mEventName;	
    public EventBase(string eventName)
    {
        mEventName = eventName;
    }
}
