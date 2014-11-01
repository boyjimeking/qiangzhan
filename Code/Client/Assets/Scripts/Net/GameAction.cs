using System;
using UnityEngine;


public abstract class GameAction
{
    private object mUseData = null;

    private readonly int mActionId;

    protected GameAction(int actionId)  
    {
        mActionId = actionId;
        ActionFactory.Instance.RegisterAction(actionId, this);
    }

    public int ActionId
    {
        get { return mActionId; }
    }

    public abstract byte [] GetRequestMsg(object userdata = null);
    public abstract void PutRespondMsg(byte [] msg, object userdata = null);

    public virtual void OnError(object userdata) {}
}
