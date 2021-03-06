//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: message_shop.hxx
// Note: requires additional types generated from: message_guid.hxx
namespace Message
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"role_shop_item_info")]
  public partial class role_shop_item_info : global::ProtoBuf.IExtensible
  {
    public role_shop_item_info() {}
    
    private int _shopid;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"shopid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int shopid
    {
      get { return _shopid; }
      set { _shopid = value; }
    }
    private int _count;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"count", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int count
    {
      get { return _count; }
      set { _count = value; }
    }
    private int _pricetypeidx;
    [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name=@"pricetypeidx", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int pricetypeidx
    {
      get { return _pricetypeidx; }
      set { _pricetypeidx = value; }
    }
    private int _isbuy;
    [global::ProtoBuf.ProtoMember(4, IsRequired = true, Name=@"isbuy", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int isbuy
    {
      get { return _isbuy; }
      set { _isbuy = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"role_shop_refresh_seconds")]
  public partial class role_shop_refresh_seconds : global::ProtoBuf.IExtensible
  {
    public role_shop_refresh_seconds() {}
    
    private int _refresh_seconds;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"refresh_seconds", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int refresh_seconds
    {
      get { return _refresh_seconds; }
      set { _refresh_seconds = value; }
    }
    private int _refresh_buncket;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"refresh_buncket", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int refresh_buncket
    {
      get { return _refresh_buncket; }
      set { _refresh_buncket = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"request_shop_op")]
  public partial class request_shop_op : global::ProtoBuf.IExtensible
  {
    public request_shop_op() {}
    
    private int _op_type;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"op_type", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int op_type
    {
      get { return _op_type; }
      set { _op_type = value; }
    }

    private int _shopid = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"shopid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int shopid
    {
      get { return _shopid; }
      set { _shopid = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"respond_shop_op")]
  public partial class respond_shop_op : global::ProtoBuf.IExtensible
  {
    public respond_shop_op() {}
    
    private int _result;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"result", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int result
    {
      get { return _result; }
      set { _result = value; }
    }

    private int _shopid = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"shopid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int shopid
    {
      get { return _shopid; }
      set { _shopid = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
    [global::ProtoBuf.ProtoContract(Name=@"SHOP_OP_TYPE")]
    public enum SHOP_OP_TYPE
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"SHOP_OP_REFRESH", Value=1)]
      SHOP_OP_REFRESH = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"SHOP_OP_BUY", Value=2)]
      SHOP_OP_BUY = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"SHOP_OP_FREE_REFRESH", Value=3)]
      SHOP_OP_FREE_REFRESH = 3
    }
  
}