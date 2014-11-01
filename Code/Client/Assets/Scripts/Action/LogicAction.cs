using System;
using ProtoBuff.Serialization;

public abstract class LogicAction<T, V> : GameAction where T:global::ProtoBuf.IExtensible, new() where V:global::ProtoBuf.IExtensible, new() 
{
    protected LogicAction(int actionId)
        : base(actionId)
    {
       
    }

    public override byte[] GetRequestMsg(object userdata)
    {
        T protoRequest = new T();

        OnRequest(protoRequest, userdata);

        Message.request_logic_msg reqMsg = new Message.request_logic_msg();

        reqMsg.data = ProtoBufUtils.Serialize(protoRequest);

        return ProtoBufUtils.Serialize(reqMsg);
    }

    public override void PutRespondMsg(byte[] msg, object userdata)
    {
        Message.respond_logic_msg resMsg = ProtoBufUtils.Deserialize<Message.respond_logic_msg>(msg);

        V protoRespond = ProtoBufUtils.Deserialize<V>(resMsg.data);

        ModuleManager.Instance.FindModule<PlayerDataModule>().SyncPlayerProperty(resMsg.rolepro);

        OnRespond(protoRespond, userdata);
    }

    protected abstract void OnRequest(T param, object userdata);
    protected abstract void OnRespond(V param, object userdata);
}
