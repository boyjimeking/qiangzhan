using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;  
using System.IO;
using System.Collections;

class TcpConnect
{
    private TcpSession mSession = null;

    private string mIp = "";
    private int mPort = 0;

    private bool mConnecting = false;

    private ArrayList mCacheList = new ArrayList();

    public TcpConnect()
    {

    }

    public bool Connect(string ip , int port)
    {
        if (mConnecting)
        {
            return false;
        }

        mIp = ip;
        mPort = port;

        DisConnect();

        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        if (socket == null)
        {
            GameDebug.LogError("创建Socket失败");
            return false;
        }

        IPAddress ipa = IPAddress.Parse(ip);

        IPEndPoint iep = new IPEndPoint(ipa, port);
        try
        {
            socket.BeginConnect(iep, new AsyncCallback(ConnectCallback), socket);
            mConnecting = true;
        }
        catch (Exception ex)
        {
            GameDebug.Log(ex.Message);
            return false;
        }  

        return true;
    }

    private void ReConnect()
    {
        Connect(mIp, mPort);
    }

    private void ConnectCallback(IAsyncResult result)
    {
        Socket socket = result.AsyncState as Socket;

        try
        {
            socket.EndConnect(result);
        }
        catch (Exception e)
        {
            GameDebug.LogError(e.Message);
        }
        finally
        {
            mSession = new TcpSession(socket);
            mSession.SendTGW("127.0.0.1" , "8087");
        }

        mConnecting = false;
    }

    public void SendMesssage(MessageHead msg)
    {
        if( mSession == null || !mSession.IsConnected() )
        {
            mCacheList.Add(msg);
            ReConnect();
            return;
        }
        mSession.PostMsg(msg);
    }

    public MessageHead PopMessage()
    {
        if (mSession == null || !mSession.IsConnected())
        {
            return null;
        }
        MemoryStream stream = mSession.PeekMsg();
        if (stream == null)
            return null;

        byte[] bytes = new byte[4];
        stream.Read(bytes, 0, 4);
        uint msgid = BitConverter.ToUInt32(bytes, 0);
        MessageHead head = null;
        if( msgid == (uint)TCP_MSG_ID.TCP_MSG_SC_CHAT )
        {
            head = new SCChatMessage();
            head.FromStream(stream);
        }
        return head;
    }

    public void Update(uint elapsed)
    {
        if ( mSession != null && mSession.IsConnected() )
        {
            mSession.Update(elapsed);

            for( int i = 0 ; i < mCacheList.Count ; ++i )
            {
                mSession.PostMsg(mCacheList[i] as MessageHead);
            }
            mCacheList.Clear();
        }
    }

    public void DisConnect()
    {
        if (mSession != null && mSession.IsConnected())
        {
            mSession.Close();
        }
    }
    public bool IsConnected()
    {
        if( mSession == null )
        {
            return false;
        }
        return mSession.IsConnected();
    }

}
