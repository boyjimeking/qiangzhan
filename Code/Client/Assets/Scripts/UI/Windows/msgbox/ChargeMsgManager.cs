using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
public class ChargeMsgManager
{
    private static ChargeMsgManager sInstance = null;
    private UIMessageBoxParam mMsgBoxUIParam = null;
    public static ChargeMsgManager Instance
    {
        get
        {
            return sInstance;
        }
    }
    public ChargeMsgManager()
    {
        sInstance = this;
    }
    public void CreateChargeMsg()
    {
        mMsgBoxUIParam = new UIMessageBoxParam();
        mMsgBoxUIParam.mMsgText = string.Format(StringHelper.GetString("charge_title"));
        mMsgBoxUIParam.mOkBtnCallback = OnMsgBoxOkCallback;
        WindowManager.Instance.OpenUI("msgbox", mMsgBoxUIParam);
    }
    private void OnMsgBoxOkCallback()
    {
        WindowManager.Instance.OpenUI("charge");
    }
}