//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: message_totalcharge.hxx
// Note: requires additional types generated from: message_guid.hxx
namespace Message
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"request_totalcharge_op")]
  public partial class request_totalcharge_op : global::ProtoBuf.IExtensible
  {
    public request_totalcharge_op() {}
    
    private int _op_type;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"op_type", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int op_type
    {
      get { return _op_type; }
      set { _op_type = value; }
    }

    private int _res_id = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"res_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int res_id
    {
      get { return _res_id; }
      set { _res_id = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"respond_totalcharge_op")]
  public partial class respond_totalcharge_op : global::ProtoBuf.IExtensible
  {
    public respond_totalcharge_op() {}
    
    private int _result;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"result", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int result
    {
      get { return _result; }
      set { _result = value; }
    }
    private int _op_type;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"op_type", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int op_type
    {
      get { return _op_type; }
      set { _op_type = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
    [global::ProtoBuf.ProtoContract(Name=@"TOTALCHARGE_OP_TYPE")]
    public enum TOTALCHARGE_OP_TYPE
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"TOTALCHARGE_GET_REWARD", Value=1)]
      TOTALCHARGE_GET_REWARD = 1
    }
  
}