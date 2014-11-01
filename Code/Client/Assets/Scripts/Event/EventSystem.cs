using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventSystem
{

    public delegate void EventDelegate(EventBase evt);

    private Dictionary<string, ArrayList> mEvents = new Dictionary<string, ArrayList>();

    //private ArrayList mPushEvents = new ArrayList();

    private EventBase mFirstEvent = null;
    private EventBase mLastEvent = null;

    private static EventSystem instance;
    public EventSystem()
    {
        instance = this;
    }

    public static EventSystem Instance
    {
        get
        {
            return instance;
        }
    }
    public void addEventListener(string eventName , EventDelegate evt)
    {
        ArrayList delegates = null;
        if( !mEvents.ContainsKey( eventName ) )
        {
            mEvents[eventName] = delegates = new ArrayList();
        }
        else
        {
            delegates = mEvents[eventName];
        }
        delegates.Add(evt);
    }

    public void removeEventListener(string eventName , EventDelegate evt)
    {
        if (!mEvents.ContainsKey(eventName))
        {
            return;
        }
        ArrayList delegates = mEvents[eventName];

        if (delegates == null || delegates.Count <= 0)
        {
            return;
        }

        int idx = delegates.IndexOf(evt);
        if( idx < 0 )
        {
            return;
        }

        delegates.RemoveAt(idx);
    }

    public bool hasEventListener(string eventName , EventDelegate evt)
    {
        if( !mEvents.ContainsKey( eventName ) )
        {
            return false;
        }
        ArrayList delegates = mEvents[eventName];

        if (delegates == null || delegates.Count <= 0)
        {
            return false;
        }

        if( delegates.IndexOf( evt ) >= 0 )
        {
            return true;
        }
        return false;
    }

    public void PushEvent(EventBase evt)
    {
        if (evt == null)
            return;

        if (mFirstEvent == null)
        {
            mFirstEvent = evt;
        }

        if( mLastEvent == null )
        {
            mLastEvent = evt;
        }
        else
        {
            mLastEvent.next = evt;
            mLastEvent = evt;
        }
        //mPushEvents.Add(evt);
    }

    public void Update()
    {
        while( mFirstEvent != null )
        {
            EventBase evt = mFirstEvent;
            mFirstEvent = mFirstEvent.next;
            if (mFirstEvent == null)
                mLastEvent = null;

            fireEvent(evt);
        }
    }

    private void fireEvent(EventBase evt)
    {
        if (evt == null)
            return;
        dispatchEvent(evt);
    }

    private void dispatchEvent(EventBase evt)
    {
        if (!mEvents.ContainsKey(evt.mEventName))
        {
            return;
        }
        ArrayList delegates = mEvents[evt.mEventName];

        if (delegates == null || delegates.Count <= 0)
        {
            return;
        }
        foreach (object obj in delegates)
        {
            EventDelegate call = obj as EventDelegate;
            if (call != null)
            {
                call(evt);
            }
        }
    }


}
