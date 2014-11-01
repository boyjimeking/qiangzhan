using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices;
class TcpSession
{
    private static byte DATA_MARK_VALUE = 0xAA;        //消息头标示


    private static byte FLAG_NORMAL_HEAD = 0x00;
    private static byte FLAG_TGW_HEAD = 0x04;

    private static int MAX_RECIVE_BUFFER = 1024 * 64;
    private Socket mSocket = null;

    private int mReciveLen = 0;
    private byte[] mTempBuffer = new byte[MAX_RECIVE_BUFFER];

    private TcpDataHead mTcpHead = new TcpDataHead();

    private MemoryStream mReciveBuffer = new MemoryStream();

    private MemoryStream mSendBuffer = new MemoryStream();

    private object mReciveLockObj = new object();
    private object mSendLockObj = new object();

    private Queue mReciveList = new Queue();

    private Queue mSendList = new Queue();

    private bool mAsynRecive = false;

    private short remote_msg_index_ = 0;
    private short myself_msg_index_ = 0;

    public TcpSession(Socket socket)
    {
        mSocket = socket;
    }

    void OnErrorClose()
    {

    }
    void BeginReceiveHead()
    {
        int headsize = Marshal.SizeOf(mTcpHead);
        mSocket.BeginReceive(mTempBuffer, 0, headsize, SocketFlags.None, new AsyncCallback(ReceiveHeadCallback), null);
    }
    void ReceiveHeadCallback(IAsyncResult result)
    {
        int size = mSocket.EndReceive(result);
        result.AsyncWaitHandle.Close();

        if( size > 0 )
        {
            int headsize = Marshal.SizeOf(mTcpHead);
            //构建消息头
            MemoryStream stream = new MemoryStream(mTempBuffer, 0, headsize);
            byte[] buf = new byte[4];
            stream.Read(buf, 0, 1);
            mTcpHead.mark_ = buf[0];
            stream.Read(buf, 0, 1);
            mTcpHead.flags_ = buf[0];
            stream.Read(buf, 0, 4);
            mTcpHead.data_size_ = BitConverter.ToUInt32(buf, 0);

            if (mTcpHead.mark_ != DATA_MARK_VALUE || mTcpHead.data_size_ <= 0 )
            {
                mAsynRecive = false;
                return;
            }

            mReciveLen = (int)mTcpHead.data_size_;
            mSocket.BeginReceive(mTempBuffer, 0, mReciveLen, SocketFlags.None, new AsyncCallback(ReceiveBodyCallback), null);
        }else
        {
            //close
            OnErrorClose();
        }
    }

    void ReceiveBodyCallback(IAsyncResult result)
    {
        int size = mSocket.EndReceive(result);
        result.AsyncWaitHandle.Close();

        if( size > 0 && size == mReciveLen )
        {
            short index_ = BitConverter.ToInt16(mTempBuffer, 0);
            short flags_ = BitConverter.ToInt16(mTempBuffer, 2);
            uint data_size_ = BitConverter.ToUInt32(mTempBuffer, 4);

            if( index_ != remote_msg_index_ )
            {
                OnErrorClose();

            }else
            {
                MemoryStream stream = new MemoryStream(mTempBuffer, 8, mReciveLen - 8);

                mReciveList.Enqueue(stream);
                BeginReceiveHead();
            }
            remote_msg_index_++;
            if (remote_msg_index_ >= 32767)
                remote_msg_index_ = 0;
            
        }else
        {
            //close
            OnErrorClose();
        }
    }

    void SendMsgCallback(IAsyncResult result)
    {
        int size = mSocket.EndSend(result);
        result.AsyncWaitHandle.Close();

        if( size <= 0 )
        {
            OnErrorClose();
        }
    }

    private bool UpdateRecvMessage()
    {
        if( !mAsynRecive )
        {
            BeginReceiveHead();

            mAsynRecive = true;
        }
        return true;
    }

    private bool UpdatePostMessage()
    {
        if (mSendList.Count <= 0)
        {
            return true;
        }
        MemoryStream stream = mSendList.Dequeue() as MemoryStream;

        //TcpDataHead 数据头
        byte mark_ = DATA_MARK_VALUE;
        byte flags_ = FLAG_NORMAL_HEAD;
        uint data_size_ = (uint)stream.Length;
               
        byte[] datasizebytes = BitConverter.GetBytes(data_size_);

        mSendBuffer = new MemoryStream();

        mSendBuffer.WriteByte(mark_);
        mSendBuffer.WriteByte(flags_);
        mSendBuffer.Write(datasizebytes, 0, datasizebytes.Length);

        mSendBuffer.Write(stream.ToArray(), 0 ,(int)stream.Length);

        mSocket.BeginSend( mSendBuffer.ToArray() , 0 , (int)mSendBuffer.Length , 0 ,new AsyncCallback(SendMsgCallback) ,null);

        return true;
    }

    public bool IsConnected()
    {
        if (mSocket == null)
            return false;
        return mSocket.Connected;
    }

    public void Close()
    {
        mSocket.Disconnect(true);
        mSocket.Close();
    }

    public MemoryStream PeekMsg()
    {
        if( mReciveList.Count <= 0 )
        {
            return null;
        }

        MemoryStream stream = mReciveList.Dequeue() as MemoryStream;
        return stream;
    }


    public void SendTGW(string ip , string port)
    {
        string tgw_data = "tgw_l7_forward\r\nHost:" + ip + ":" + port + "\r\n\r\n";

        MemoryStream msgStream = new MemoryStream();

        byte[] tgw_bytes = System.Text.Encoding.UTF8.GetBytes(tgw_data);

        msgStream.Write(tgw_bytes, 0, tgw_bytes.Length);

        MemoryStream stream = new MemoryStream();
        //TcpMsgHead 消息头
        short index = 0;
        short flags = 0;
        uint datasize = (uint)msgStream.Length;

        byte[] indexbytes = BitConverter.GetBytes(index);
        byte[] flagsbytes = BitConverter.GetBytes(flags);
        byte[] datasizebytes = BitConverter.GetBytes(datasize);

        stream.Write(indexbytes, 0, indexbytes.Length);
        stream.Write(flagsbytes, 0, flagsbytes.Length);
        stream.Write(datasizebytes, 0, datasizebytes.Length);

        stream.Write(msgStream.ToArray(), 0, (int)msgStream.Length);

        //TcpDataHead 数据头
        byte mark_ = DATA_MARK_VALUE;
        byte flags_ = FLAG_TGW_HEAD;
        uint data_size_ = (uint)stream.Length;

        datasizebytes = BitConverter.GetBytes(data_size_);

        MemoryStream SendBuffer = new MemoryStream();

        SendBuffer.WriteByte(mark_);
        SendBuffer.WriteByte(flags_);
        SendBuffer.Write(datasizebytes, 0, datasizebytes.Length);

        SendBuffer.Write(stream.ToArray(), 0, (int)stream.Length);

        mSocket.BeginSend(SendBuffer.ToArray(), 0, (int)SendBuffer.Length, 0, new AsyncCallback(SendMsgCallback), null);
    }


    public void PostMsg(MessageHead msg)
    {
        MemoryStream msgStream = new MemoryStream();

        byte[] msgid_bytes = BitConverter.GetBytes(msg.msgid);

        msgStream.Write(msgid_bytes, 0, msgid_bytes.Length);

        msg.BuidStream(msgStream);

        MemoryStream stream = new MemoryStream();
        //TcpMsgHead 消息头
        short index = myself_msg_index_++;
        short flags = 0;
        uint datasize = (uint)msgStream.Length;

        byte[] indexbytes = BitConverter.GetBytes(index);
        byte[] flagsbytes = BitConverter.GetBytes(flags);
        byte[] datasizebytes = BitConverter.GetBytes(datasize);

        stream.Write(indexbytes, 0, indexbytes.Length);
        stream.Write(flagsbytes, 0, flagsbytes.Length);
        stream.Write(datasizebytes, 0, datasizebytes.Length);

        stream.Write(msgStream.ToArray(), 0, (int)msgStream.Length);

        mSendList.Enqueue(stream);
        if (myself_msg_index_ >= 32767)
        {
            myself_msg_index_ = 0;
        }
    }

    public void Update(uint elapsed)
    {
        if( !mSocket.Connected )
        {
            return;
        }

        UpdateRecvMessage();
        UpdatePostMessage();
    }

}
