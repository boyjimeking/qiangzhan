using UnityEngine;
using System.Collections;

public class UITitleOpen : UIWindow
{
    private UILabel mLabel = null;
    private UIButton mButton = null;
    private UISprite mIcon = null;

    
    private int mCurTitleID = -1;

    private Vector3 mSrcPos = Vector3.zero;

    private bool mFlying = false;

    private TitleModule mModule = null;


    private bool mWaiting = false;
    private int mWaitingTime = 0;

    public UITitleOpen()
    {

    }
    protected override void OnLoad()
    {
        mButton = this.FindComponent<UIButton>("Sprite");
        mLabel = this.FindComponent<UILabel>("Sprite/Label");
        mIcon = this.FindComponent<UISprite>("Sprite/Icon");

        mSrcPos = mIcon.gameObject.transform.position;

        EventDelegate.Add(mButton.onClick, OnButtonClick);
        mModule = ModuleManager.Instance.FindModule<TitleModule>();
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
        if( mCurTitleID == -1 )
        {
            mCurTitleID = mModule.PopCache();
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
        if (!DataManager.TitleItemTable.ContainsKey(mCurTitleID))
            return;
        TitleItemTableItem item = TitleModule.GetTitleItemById(mCurTitleID);
        UIAtlasHelper.SetSpriteImage(mIcon, item.picName,true);
        mLabel.text = StringHelper.GetString("get_new_title") + item.name;
        mWaitingTime = 3000;
        mWaiting = true;

    }

    private void OnEnd()
    {
        Debug.LogError("OnEnd");
        WindowManager.Instance.CloseUI("titleopen");
    }

    private void OnButtonClick()
    {
        mWaiting = false;

        if (mCurTitleID == -1)
        {
            OnEnd();
            return ;
        }

        if (mFlying)
            return;

        //暂时去掉移动效果
        //OnButtonMoveEnd();
        MenuTableItem item = DataManager.MenuTable[0] as MenuTableItem;

        Vector3 targetPos = Vector3.zero;

        UICityForm cityUI = WindowManager.Instance.GetUI("city") as UICityForm;
        if( cityUI != null )
        {
            //targetPos = cityUI.GetNextMenuPos(item.type);
            targetPos = cityUI.GetMenuPos(0, (FunctionType)item.menuOpType);
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
        mFlying = false;
        mIcon.gameObject.transform.position = mSrcPos;

        if (mModule.needClose(mCurTitleID))
        {
            OnEnd();
        }
        mCurTitleID = -1;
    }
	
}
