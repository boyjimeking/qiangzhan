//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: message_recharge.hxx
namespace Message
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"request_recharge_results")]
  public partial class request_recharge_results : global::ProtoBuf.IExtensible
  {
    public request_recharge_results() {}
    
    private string _openid;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"openid", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string openid
    {
      get { return _openid; }
      set { _openid = value; }
    }
    private string _platform;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"platform", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string platform
    {
      get { return _platform; }
      set { _platform = value; }
    }
    private string _accesstoken;
    [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name=@"accesstoken", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string accesstoken
    {
      get { return _accesstoken; }
      set { _accesstoken = value; }
    }
    private string _paytoken;
    [global::ProtoBuf.ProtoMember(4, IsRequired = true, Name=@"paytoken", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string paytoken
    {
      get { return _paytoken; }
      set { _paytoken = value; }
    }
    private string _pf;
    [global::ProtoBuf.ProtoMember(5, IsRequired = true, Name=@"pf", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string pf
    {
      get { return _pf; }
      set { _pf = value; }
    }
    private string _pfkey;
    [global::ProtoBuf.ProtoMember(6, IsRequired = true, Name=@"pfkey", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string pfkey
    {
      get { return _pfkey; }
      set { _pfkey = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"respond_recharge_results")]
  public partial class respond_recharge_results : global::ProtoBuf.IExtensible
  {
    public respond_recharge_results() {}
    
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}