using System;
using System.Runtime.InteropServices;
using System.IO;

public enum TCP_MSG_ID
{
    TCP_MSG_CS_CHAT = 0x00000001,
    TCP_MSG_SC_CHAT = 0x00000002,
}



[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class MessageHead
{
    public uint msgid = uint.MaxValue;
    public virtual void BuidStream(MemoryStream stream)
    {
    }
    public virtual void FromStream(MemoryStream stream)
    {
    }
}


[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class CSChatMessage : MessageHead
{
    public int channel_type = 0;
    public string name = "";
    public string msg = "";

    public CSChatMessage()
    {
        msgid = (uint)TCP_MSG_ID.TCP_MSG_CS_CHAT;
    }
    public override void BuidStream(MemoryStream stream)
    {
        byte[] name_bytes = System.Text.Encoding.UTF8.GetBytes(name);
        byte[] msg_bytes = System.Text.Encoding.UTF8.GetBytes(msg);

        uint name_len = (uint)name_bytes.Length;
        uint msg_len = (uint)msg_bytes.Length;

        byte[] name_len_bytes = BitConverter.GetBytes(name_len);
        byte[] msg_len_bytes = BitConverter.GetBytes(msg_len);

        byte[] channel_bytes = BitConverter.GetBytes(channel_type);

        stream.Write(channel_bytes, 0, channel_bytes.Length);
        stream.Write(name_len_bytes, 0, name_len_bytes.Length);
        stream.Write(msg_len_bytes, 0, msg_len_bytes.Length);

        stream.Write(name_bytes, 0, name_bytes.Length);
        stream.Write(msg_bytes, 0, msg_bytes.Length);
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class SCChatMessage : MessageHead
{
    public int channel_type = 0;
    public string name = "";
    public string msg = "";

    public SCChatMessage()
    {
        msgid = (uint)TCP_MSG_ID.TCP_MSG_SC_CHAT;
    }
    public override void FromStream(MemoryStream stream)
    {
        byte[] bytes = new byte[4];
        stream.Read(bytes, 0, 4);
        channel_type = BitConverter.ToInt32(bytes, 0);

        stream.Read(bytes, 0, 4);
        uint name_len = BitConverter.ToUInt32(bytes, 0);
        stream.Read(bytes, 0, 4);
        uint msg_len = BitConverter.ToUInt32(bytes, 0);


        byte[] name_byte = new byte[name_len];
        stream.Read(name_byte, 0, (int)name_len);

        byte[] msg_byte = new byte[msg_len];
        stream.Read(msg_byte, 0, (int)msg_len);

        name = System.Text.Encoding.UTF8.GetString(name_byte);
        msg = System.Text.Encoding.UTF8.GetString(msg_byte);
    }
}
