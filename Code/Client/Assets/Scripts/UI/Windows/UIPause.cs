using System;
using UnityEngine;

class UIPause : UIWindow
{
    private UIButton mExitButton = null;
    private UIButton mBackButton = null;

    private UIButton mSetting = null;

    private UILabel mLabel1 = null;
    private UILabel mLabel2 = null;
    protected override void OnLoad()
    {
        base.OnLoad();

        mExitButton = this.FindComponent<UIButton>("Sprite/Exit");
        mBackButton = this.FindComponent<UIButton>("Sprite/Back");

        mSetting = this.FindComponent<UIButton>("Sprite/Button");

        mLabel1 = this.FindComponent<UILabel>("Sprite/Label1");
        mLabel2 = this.FindComponent<UILabel>("Sprite/Label2");

        EventDelegate.Add(mExitButton.onClick, onExitClick);
        EventDelegate.Add(mBackButton.onClick, onBackClick);
        EventDelegate.Add(mSetting.onClick, onSettingClick);

    }

    private void onExitClick()
    {
        WindowManager.Instance.CloseUI("pause");
        SceneManager.Instance.RequestEnterLastCity();
    }
    private void onBackClick()
    {
        WindowManager.Instance.CloseUI("pause");
    }
    private void onSettingClick()
    {
        int shoottype = SettingManager.Instance.GetShootType();
        if( shoottype ==  (int)SHOOT_TYPE.SHOOT_TYPE_NORMAL )
        {
            SettingManager.Instance.SetShootType( (int)SHOOT_TYPE.SHOOT_TYPE_AUTO );
        }else
        {
            SettingManager.Instance.SetShootType((int)SHOOT_TYPE.SHOOT_TYPE_NORMAL);
        }

        UpdateSetting();
    }
    protected override void OnOpen(object param = null)
    {
        base.OnOpen();
        SceneManager.Instance.LogicPause();
        UpdateSetting();
    }

    private void UpdateSetting()
    {
        int shoottype = SettingManager.Instance.GetShootType();
        if (shoottype == (int)SHOOT_TYPE.SHOOT_TYPE_NORMAL)
        {
            mSetting.gameObject.transform.localPosition = new Vector3(-46.0f, 0.0f, 0.0f);
            mLabel1.color = NGUIMath.HexToColor(0x81ffa5ff);
            mLabel2.color = NGUIMath.HexToColor(0xff2424ff);
        }
        else
        {
            mSetting.gameObject.transform.localPosition = new Vector3(45.0f, 0.0f, 0.0f);
            mLabel1.color = NGUIMath.HexToColor(0xff2424ff);
            mLabel2.color = NGUIMath.HexToColor(0x81ffa5ff);
        }
    }

    protected override void OnClose()
    {
        base.OnClose();
        SceneManager.Instance.LogicResume();
    }
}

