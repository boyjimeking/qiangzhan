//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: message_egg.hxx
// Note: requires additional types generated from: message_guid.hxx
namespace Message
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"role_egg_item_info")]
  public partial class role_egg_item_info : global::ProtoBuf.IExtensible
  {
    public role_egg_item_info() {}
    
    private int _opentimes;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"opentimes", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int opentimes
    {
      get { return _opentimes; }
      set { _opentimes = value; }
    }
    private int _seconds;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"seconds", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int seconds
    {
      get { return _seconds; }
      set { _seconds = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"role_egg_item_items")]
  public partial class role_egg_item_items : global::ProtoBuf.IExtensible
  {
    public role_egg_item_items() {}
    
    private int _itemid;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"itemid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int itemid
    {
      get { return _itemid; }
      set { _itemid = value; }
    }
    private int _itemnum;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"itemnum", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int itemnum
    {
      get { return _itemnum; }
      set { _itemnum = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"request_egg_op")]
  public partial class request_egg_op : global::ProtoBuf.IExtensible
  {
    public request_egg_op() {}
    
    private int _op_type;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"op_type", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int op_type
    {
      get { return _op_type; }
      set { _op_type = value; }
    }

    private int _eggid = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"eggid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int eggid
    {
      get { return _eggid; }
      set { _eggid = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"respond_egg_op")]
  public partial class respond_egg_op : global::ProtoBuf.IExtensible
  {
    public respond_egg_op() {}
    
    private int _result;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"result", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int result
    {
      get { return _result; }
      set { _result = value; }
    }

    private int _eggid = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"eggid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int eggid
    {
      get { return _eggid; }
      set { _eggid = value; }
    }
    private readonly global::System.Collections.Generic.List<role_egg_item_items> _items = new global::System.Collections.Generic.List<role_egg_item_items>();
    [global::ProtoBuf.ProtoMember(3, Name=@"items", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<role_egg_item_items> items
    {
      get { return _items; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
    [global::ProtoBuf.ProtoContract(Name=@"EGG_OP_TYPE")]
    public enum EGG_OP_TYPE
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"EGG_OP_OPEN", Value=1)]
      EGG_OP_OPEN = 1
    }
  
}