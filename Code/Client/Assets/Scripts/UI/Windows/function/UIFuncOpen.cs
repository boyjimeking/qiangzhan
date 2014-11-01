using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

class UIFuncOpen : UIWindow
{
    private UILabel mLabel = null;
    private UIButton mButton = null;
    private UISprite mIcon = null;

    
    private int mCurFunctionID = -1;

    public delegate void FunctionCall(int id , bool guide = true);

    private Vector3 mSrcPos = Vector3.zero;

    private bool mFlying = false;

    private FunctionModule mModule = null;


    private bool mWaiting = false;
    private int mWaitingTime = 0;

    public UIFuncOpen()
    {

    }
    protected override void OnLoad()
    {
        mButton = this.FindComponent<UIButton>("Sprite");
        mLabel = this.FindComponent<UILabel>("Sprite/Label");
        mIcon = this.FindComponent<UISprite>("Sprite/Icon");

        mSrcPos = mIcon.gameObject.transform.position;

        EventDelegate.Add(mButton.onClick, OnButtonClick);
        mModule = ModuleManager.Instance.FindModule<FunctionModule>();
    }

    //界面打开
    protected override void OnOpen(object param = null)
    {
        //强制隐藏引导
        GuideModule module = ModuleManager.Instance.FindModule<GuideModule>();
        module.ForceHideGuide(true);

        InputSystem.Instance.SetLockMove(true);
    }
    //界面关闭
    protected override void OnClose()
    {
        GuideModule module = ModuleManager.Instance.FindModule<GuideModule>();
        module.ForceHideGuide(false);

        InputSystem.Instance.SetLockMove(false);
    }

    public override void Update(uint elapsed)
    {
        if( mCurFunctionID == -1 )
        {
            mCurFunctionID = mModule.PopCache();
            OnStart();
        }

        if( mWaiting )
        {
            mWaitingTime -= (int)elapsed;
            if( mWaitingTime <= 0 )
            {
                OnButtonClick();
            }
        }

    }

    private void OnStart()
    {
        if (!DataManager.MenuTable.ContainsKey(mCurFunctionID))
            return;
        MenuTableItem item = DataManager.MenuTable[mCurFunctionID] as MenuTableItem;
        UIAtlasHelper.SetSpriteImage(mIcon, item.icon);
        mLabel.text = "新功能开启 : " + item.desc;
        mWaitingTime = 3000;
        mWaiting = true;

    }

    private void OnEnd()
    {
        WindowManager.Instance.CloseUI("funcopen");
    }

    private void OnButtonClick()
    {
        mWaiting = false;

        if (mCurFunctionID == -1)
        {
            OnEnd();
            return ;
        }

        if (mFlying)
            return;

        //暂时去掉移动效果
        //OnButtonMoveEnd();
        MenuTableItem item = DataManager.MenuTable[mCurFunctionID] as MenuTableItem;

        Vector3 targetPos = Vector3.zero;

        UICityForm cityUI = WindowManager.Instance.GetUI("city") as UICityForm;
        if( cityUI != null )
        {
            targetPos = cityUI.GetNextMenuPos(item.type);
        }

        TweenPosition tp = TweenPosition.Begin(mIcon.gameObject, 0.5f, targetPos);
        tp.worldSpace = true;
        tp.from = mIcon.gameObject.transform.position;
        tp.to = targetPos;
        EventDelegate.Add(tp.onFinished, OnButtonMoveEnd);
        tp.PlayForward();
        mFlying = true;

    }

    private void OnButtonMoveEnd()
    {
        mIcon.gameObject.transform.position = mSrcPos;

        if (mModule.OnEnd(mCurFunctionID))
        {
            OnEnd();
        }
        mCurFunctionID = -1;
        mFlying = false;
    }
}
