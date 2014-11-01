using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Text;

public class Net : MonoBehaviour
{
    private const uint respond_msg_type_common = 0;
    private const uint respond_msg_type_error = 1;

    private List<HttpPackage> mActionList = new List<HttpPackage>();
    private bool mActionDoing = false;
    private bool mIsConnected = false;
    private bool mIsConnecting = false;
    private bool mSuspend = false;

    private string strUrl = "";
    private uint mSessionId = 0xFFFFFFFF;
    private string mKey = "8087123";
    private uint mHearbeatTimer = 0;

    public delegate void RequestNotifyDelegate(Status eStatus);

    private HttpPackage mCurrrentRequest = null;

    /// <summary>
    /// 网络请求出错回调方法
    /// </summary>
    /// <param name="nType"></param>
    /// <param name="actionId"></param>
    /// <param name="strMsg"></param>
    public delegate void NetError(eNetError nType, int actionId, string strMsg);

    public delegate void NetConnectFailed();
    public delegate void NectConnectOk();

    public void Suspend()
    {
        mSuspend = true;
    }

    public void Resum()
    {
        mSuspend = false;
    }

    public void Ignore()
    {
        mCurrrentRequest = null;
        mSuspend = false;
    }

    public enum Status
    {
        eStartRequest = 0,
        eEndRequest = 1,
    }

    protected static readonly int OVER_TIME = 30;
    private static Net s_instance = null;
    private const int NETSUCCESS = 0;
    //private string strUrl;

    public enum eNetError
    {
        eConnectFailed = 0,
        eRequestFailed = 1,
        eInvalidSession = 2,
        eInvalidActionId = 3,
        eTimeOut = 4,
    }

    /// <summary>
    /// 请求代理通知
    /// </summary>
    public RequestNotifyDelegate RequestNotify { set; get; }

    /// <summary>
    /// 注册网络请求出错回调方法
    /// </summary>
    public NetError NetErrorCallback { get; set; }


    public NetConnectFailed NetConnectFailedCallback { get; set; }

    public NectConnectOk NetConnectOkCallback { get; set; }



    public void SetUrl(string szUrl)
    {
        strUrl = szUrl;
    }

    public string GetUrl()
    {
        return strUrl;
    }

    public int NetSuccess
    {
        get { return NETSUCCESS; }
    }
    void Start()
    {

    }

    void Awake()
    {
        UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
    }

    void Update()
    {
        if (!mIsConnected)
            return;

        if (mSuspend)
            return;

        // 先放到这里
        mHearbeatTimer += (uint)(Time.deltaTime * 1000.0f);
        if (mHearbeatTimer > 1000 * 60)
        {
            mHearbeatTimer = 0;
            Net.Instance.DoAction(6, null, false);
        }

        if (mActionDoing)
            return;

        if(mCurrrentRequest == null)
        {
            if(mActionList.Count != 0)
            {
                mCurrrentRequest = mActionList[0];
                mActionList.RemoveAt(0);
            }
        }

        if(mCurrrentRequest != null)
        {
            StartCoroutine(HttpPostRequest());
        }
    }

    public bool IsConnected()
    {
        return mIsConnected;
    }

    public bool IsConnecting()
    {
        return mIsConnecting;
    }

    public void DisConnect()
    {
        mIsConnected = false;
        mCurrrentRequest = null;
        mActionList.Clear();
    }

    public void Connect()
    {
        if (IsConnected() || IsConnecting())
            return;

        mIsConnecting = true;

        StartCoroutine(RequestSession());
    }

    private IEnumerator RequestSession()
    {
        if( RequestNotify != null )
        {
            RequestNotify(Status.eStartRequest);
        }
        string post_data = "request--session";

        WWW wwwObject = new WWW(strUrl, Encoding.ASCII.GetBytes(post_data));

        yield return wwwObject;

        string respondmsg = Encoding.ASCII.GetString(wwwObject.bytes);

        string[] data_vec = respondmsg.Split(new char[] { '&' });
    
        uint sign = 0;
        for(int i = 0; i < data_vec.Length ; i++)
        {
            string[] key_values = data_vec[i].Split(new char[] { '=' });
            if (key_values[0] == "respond--session") 
            {
                sign++;
            }
            else if(key_values[0] == "sessionid")
            {
                sign++;

                mSessionId = System.Convert.ToUInt32(key_values[1]);
            }
            else if (key_values[0] == "key")
            {
                sign++;
                mKey = key_values[1];
            }
        }

        if(sign == 3)
        {
            mIsConnected = true;
            mSuspend = false;

            if(NetConnectOkCallback != null)
            {
                NetConnectOkCallback();
            }
        }
        else
        {
            foreach (HttpPackage package in mActionList)
            {
                package.Action.OnError(package.UserData);
            }

            mActionList.Clear();
            mCurrrentRequest = null;

            if (NetConnectFailedCallback != null)
            {
                NetConnectFailedCallback();
            }
        }

        if (RequestNotify != null)
        {
            RequestNotify(Status.eEndRequest);
        }
        mIsConnecting = false;
    }

    public static Net Instance
    {
        get
        {
            s_instance = UnityEngine.Object.FindObjectOfType(typeof(Net)) as Net;
            if (s_instance == null)
            {
                GameObject obj2 = new GameObject("net");
                s_instance = obj2.AddComponent(typeof(Net)) as Net;
                if (s_instance != null)
                {
                    s_instance.RequestNotify = NetCallback.OnRequestDelegate;
                    s_instance.NetErrorCallback = NetCallback.OnNetError;
                    s_instance.NetConnectOkCallback = NetCallback.OnNetConnectOK;
                    s_instance.NetConnectFailedCallback = NetCallback.OnNetConnectFail;
                }
            }
            return s_instance;
        }
    }

    public bool DoAction(int actionId, object userData, bool bShowLoading = true)
    {
        if (!IsConnected())
            return false;

        GameAction gameAction = ActionFactory.Instance.Create(actionId);
        if (gameAction == null)
        {
            GameDebug.LogError(string.Format("Not found {0} of GameAction object.", actionId));
            return false;
        }

        HttpPackage httpPackage = new HttpPackage();
        httpPackage.ActionId = gameAction.ActionId;
        httpPackage.Action = gameAction;
        httpPackage.UserData = userData;
        httpPackage.ShowLoading = bShowLoading;
        httpPackage.PostData = CompositeData(mSessionId, (uint)gameAction.ActionId, gameAction.GetRequestMsg(userData));

        mActionList.Add(httpPackage);

        return true;
    }

    private IEnumerator HttpPostRequest()
    {
        mActionDoing = true;

        mCurrrentRequest.WwwObject = new WWW(strUrl, mCurrrentRequest.PostData);
      
        DateTime start = DateTime.Now;

        if (RequestNotify != null && mCurrrentRequest.ShowLoading)
        {
            RequestNotify(Net.Status.eStartRequest);
        }

        yield return mCurrrentRequest.WwwObject;

        if (RequestNotify != null && mCurrrentRequest.ShowLoading)
        {
            RequestNotify(Net.Status.eEndRequest);
        }

        //TimeSpan tsStart = new TimeSpan(start.Ticks);
        //TimeSpan tsEnd = new TimeSpan(DateTime.Now.Ticks);
        //TimeSpan ts = tsEnd.Subtract(tsStart).Duration();

        //if (ts.Seconds > OVER_TIME)
        //{
        //    mCurrrentRequest.IsOverTime = true;
        //}

        if (!OnHttpRespond())
        {
            mSuspend = true;
        }
        else
        {
            mCurrrentRequest = null;
        }

        mActionDoing = false;
        
    }
    
    public bool OnHttpRespond()
    {
        if (mCurrrentRequest.error != null)
        {
            mCurrrentRequest.Action.OnError(mCurrrentRequest.UserData);

            OnNetError(eNetError.eRequestFailed, mCurrrentRequest.ActionId, mCurrrentRequest.error);
            return false;
        }
        else
        {
            byte[] buffBytes = mCurrrentRequest.GetResponse();

            uint errorCode = 0;
            uint sessionId = 0xFFFFFFFF;
            uint actionId = 0xFFFFFFFF;
            byte [] msgBody = null;

            if(ParseError(buffBytes, ref errorCode))
            {// 发生系统错误

                if(errorCode == 2)
                {
                    OnNetError(eNetError.eInvalidActionId, mCurrrentRequest.ActionId);
                    return false;
                }

                OnNetError(eNetError.eInvalidSession, mCurrrentRequest.ActionId);
                return false;
            }
            
            if(!ParseData(buffBytes, ref sessionId, ref actionId, out msgBody))
            {
                mCurrrentRequest.Action.OnError(mCurrrentRequest.UserData);

                OnNetError(eNetError.eRequestFailed, mCurrrentRequest.ActionId, "ParseData");
                return false;
            }
          
            mCurrrentRequest.Action.PutRespondMsg(msgBody, mCurrrentRequest.UserData);
            return true;
        }
    }

    private void OnNetError(eNetError type, int nActionId, string str = "")
    {
        if (NetErrorCallback != null)
        {
            NetErrorCallback(type, nActionId, str);
        }
    }
    private void OnNetTimeOut(int nActionId)
    {
        if (NetErrorCallback != null)
        {
            NetErrorCallback(eNetError.eTimeOut, nActionId, "timeout.");
        }

    }

    private byte[] CompositeData(uint sessionId, uint actionId, byte [] msgBody)
    {
        string hmac_value = HMACUtils.HMacSha1Encrypt(msgBody, Encoding.ASCII.GetBytes(mKey));

        MemoryStream stream = new MemoryStream();

        byte [] sessiond_stream = BitConverter.GetBytes(sessionId);
        byte[] message_stream = BitConverter.GetBytes(actionId);
        byte[] msgbodyLength_stream = BitConverter.GetBytes(msgBody.Length);
        byte[] hmac_stream = Encoding.ASCII.GetBytes(hmac_value);

        stream.Write(sessiond_stream, 0, sessiond_stream.Length);
        stream.Write(message_stream, 0, message_stream.Length);
        stream.Write(hmac_stream, 0, hmac_stream.Length);
        stream.Write(msgbodyLength_stream, 0, msgbodyLength_stream.Length);
        stream.Write(msgBody, 0, msgBody.Length);

        return stream.ToArray();
        
    }

    public bool ParseError(byte[] data, ref uint errorCode)
    {
        if (data.Length < 4 + 4)
            return false;

        MemoryStream stream = new MemoryStream(data);
        byte[] buff4 = new byte[4];

        stream.Read(buff4, 0, 4);

        if (BitConverter.ToUInt32(buff4, 0) != respond_msg_type_error)
            return false;

        stream.Read(buff4, 0, 4);

        errorCode = BitConverter.ToUInt32(buff4, 0);
        return true;
    }

    private bool ParseData(byte[] data, ref uint sessionId, ref uint actionId, out byte [] msgBody)
    {
        msgBody = null;

        if (data.Length < 4 + 4 + 4 + 40 + 4)
            return false;

        MemoryStream stream = new MemoryStream(data);

        byte [] hmacvalue = new byte[40];

        byte [] buff4 = new byte[4];

        stream.Read(buff4, 0, 4);
        if (BitConverter.ToUInt32(buff4, 0) != respond_msg_type_common)
            return false;

        stream.Read(buff4, 0, 4);
        sessionId = BitConverter.ToUInt32(buff4, 0);

        stream.Read(buff4, 0, 4);
        actionId = BitConverter.ToUInt32(buff4, 0);

        stream.Read(hmacvalue, 0, 40);

        stream.Read(buff4, 0, 4);
        uint bodyLength = BitConverter.ToUInt32(buff4, 0);

        msgBody = new byte[bodyLength];

        if(msgBody.Length > 0)
        {
            stream.Read(msgBody, 0, (int)bodyLength);
        }

        if (HMACUtils.HMacSha1Encrypt(msgBody, Encoding.ASCII.GetBytes(mKey)).CompareTo(Encoding.ASCII.GetString(hmacvalue)) != 0)
            return false;

        return true;
    }

    public string GetSessionKey()
    {
        return mKey;
    }
}
