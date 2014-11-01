using System;
using ProtoBuff.Serialization;

public abstract class BaseAction<T, V> : GameAction where T:global::ProtoBuf.IExtensible, new() where V:global::ProtoBuf.IExtensible, new() 
{
    protected BaseAction(int actionId)
        : base(actionId)
    {
       
    }

    public override byte[] GetRequestMsg(object userdata)
    {
        T protoRequest = new T();

        OnRequest(protoRequest, userdata);

        return ProtoBufUtils.Serialize(protoRequest);
    }

    public override void PutRespondMsg(byte[] msg, object userdata)
    {
        V protoRespond = ProtoBufUtils.Deserialize<V>(msg);

        OnRespond(protoRespond, userdata);
    }

    protected abstract void OnRequest(T param, object userdata);
    protected abstract void OnRespond(V param, object userdata);
}
