using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
struct TcpDataHead
{
    public byte mark_;
    public byte flags_;
    public uint data_size_;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
struct TcpMsgHead
{
    public short index_;
    public short flags_;
    public uint data_size_;
}
